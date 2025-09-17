
using AppData.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;

namespace AppApi.Controllers
{
    [Area("Admin")]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaRepo _repo;
        private readonly ISanPhamRepo _spRepo;       // repo sản phẩm
        private readonly ISanPhamCTRepo _spctRepo;   // repo spct
        private readonly IDanhMucRePo _dmRepo;       // repo danh mục

        public GiamGiaController(
            IGiamGiaRepo repo,
            ISanPhamRepo spRepo,
            ISanPhamCTRepo spctRepo,
            IDanhMucRePo dmRepo)
        {
            _repo = repo;
            _spRepo = spRepo;
            _spctRepo = spctRepo;
            _dmRepo = dmRepo;
        }

        // ---------------- API load sản phẩm chi tiết ----------------
        [HttpGet("GetSanPhamCTsBySanPhamId")]
        public async Task<IActionResult> GetSanPhamCTsBySanPhamId([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return Json(new List<SanPhamCTViewModel>());

            var spcts = await _spctRepo.GetBySanPhamIdAsync(id);

            var result = spcts.Select(x => new
            {
                idSanPhamCT = x.IDSanPhamCT,
                tenSanPham = x.SanPham?.TenSanPham ?? "",
                size = x.SizeAo,
                mauSac = x.MauSac,
                chatlieu = x.ChatLieu
            });

            return Json(result);
        }

        // ---------------- CRUD GIẢM GIÁ ----------------
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();
            return View(list);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var vm = new GiamGiaCreateVM
            {
                GiamGia = new GiamGia
                {
                    NgayBatDau = DateTime.Today,
                    NgayKetThuc = DateTime.Today.AddDays(7)
                },
                SanPhams = await _spRepo.GetAllSanPhamAsync(),
                DanhMucs = await _dmRepo.GetAllDanhMucsAsync(),
                SanPhamCTs = new List<SanPhamCTViewModel>() // để trống, load bằng AJAX
            };

            return View(vm);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaCreateVM vm)
        {
            if (ModelState.IsValid)
            {
                // Tạo mới giảm giá
                vm.GiamGia.IDGiamGia = Guid.NewGuid();
                await _repo.CreateAsync(vm.GiamGia);

                // Gắn sản phẩm
                foreach (var spId in vm.SelectedSanPhams ?? new List<Guid>())
                {
                    await _repo.AddProductToDiscountAsync(vm.GiamGia.IDGiamGia, spId);
                }

                // Gắn danh mục
                foreach (var dmId in vm.SelectedDanhMucs ?? new List<Guid>())
                {
                    await _repo.AddCategoryToDiscountAsync(vm.GiamGia.IDGiamGia, dmId);
                }

                // Gắn sản phẩm chi tiết
                foreach (var spctId in vm.SelectedSanPhamCTs ?? new List<Guid>())
                {
                    await _repo.AddSanPhamCTToDiscountAsync(vm.GiamGia.IDGiamGia, spctId);
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi validate thì load lại dữ liệu
            vm.SanPhams = await _spRepo.GetAllSanPhamAsync();
            vm.DanhMucs = await _dmRepo.GetAllDanhMucsAsync();
            vm.SanPhamCTs = new List<SanPhamCTViewModel>(); // vẫn để trống, load qua AJAX

            return View(vm);
        }
    }
}

