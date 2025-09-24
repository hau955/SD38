using AppData.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;
using PagedList;

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
        [HttpGet("/Admin/GiamGia/GetSanPhamCTsBySanPhamId")]
        public async Task<IActionResult> GetSanPhamCTsBySanPhamId([FromQuery] Guid id)
        {
            if (id == Guid.Empty)
                return Json(new List<object>());

            var spcts = await _spctRepo.GetBySanPhamIdAsync(id);

            var result = spcts.Select(x => new
            {
                id = x.IDSanPhamCT,
                text = $"{x.SanPham?.TenSanPham} - Size: {x.SizeAo} - Màu: {x.MauSac} - Chất liệu: {x.ChatLieu}"
            });

            return Json(result);
        }

    public async Task<IActionResult> Index(string searchTerm, string statusFilter, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
    {
        var list = await _repo.GetAllAsync();

        // Lọc dữ liệu (ví dụ)
        if (!string.IsNullOrEmpty(searchTerm))
        {
            list = list.Where(x => x.TenGiamGia.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Chuyển sang PagedList
        var pagedList = list.ToPagedList(page, pageSize);

        return View(pagedList);
    }


    [HttpGet]
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
              
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                // load lại dữ liệu nếu form lỗi
                vm.DanhMucs = await _dmRepo.GetAllDanhMucsAsync();
                vm.SanPhams = await _spRepo.GetAllSanPhamAsync();
               // vm.SanPhamCTs = await _repo.GetSanPhamCTAsync();
                return View(vm);
            }

            try
            {
                // 1. Tạo Giảm Giá qua API (API sẽ tự sinh Guid)
                var created = await _repo.CreateAsync(vm.GiamGia);

                // 2. Gắn sản phẩm
                if (vm.SelectedSanPhams != null)
                {
                    foreach (var spId in vm.SelectedSanPhams)
                    {
                        await _repo.AddProductToDiscountAsync(created.IDGiamGia, spId);
                    }
                }

                // 3. Gắn danh mục
                if (vm.SelectedDanhMucs != null)
                {
                    foreach (var dmId in vm.SelectedDanhMucs)
                    {
                        await _repo.AddCategoryToDiscountAsync(created.IDGiamGia, dmId);
                    }
                }


                TempData["Success"] = "Tạo giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo giảm giá: {ex.Message}");
                vm.DanhMucs = await _dmRepo.GetAllDanhMucsAsync();
                vm.SanPhams = await _spRepo.GetAllSanPhamAsync();
             
                return View(vm);
            }
        }
    }
}
