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
        private readonly ISanPhamRepo _spRepo;
        private readonly ISanPhamCTRepo _spctRepo;
        private readonly IDanhMucRePo _dmRepo;

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

        // ---------------- INDEX ----------------
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();
            return View(list);
        }

        // ---------------- CREATE (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new GiamGiaCreateVM
            {
                GiamGia = new GiamGia
                {
                    NgayBatDau = DateTime.Today,
                    NgayKetThuc = DateTime.Today.AddDays(7)
                }
            };

            await LoadDropdowns(vm);
            return View(vm);
        }

        // ---------------- CREATE (POST) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }

            try
            {
                // Tạo giảm giá
                var created = await _repo.CreateAsync(vm.GiamGia);

                // Chỉ chọn 1 loại trong 3
                if (vm.SelectedDanhMucs != null && vm.SelectedDanhMucs.Any())
                {
                    foreach (var dmId in vm.SelectedDanhMucs)
                    {
                        await _repo.AddCategoryToDiscountAsync(created.IDGiamGia, dmId);
                    }
                }
                else if (vm.SelectedSanPhams != null && vm.SelectedSanPhams.Any())
                {
                    foreach (var spId in vm.SelectedSanPhams)
                    {
                        await _repo.AddProductToDiscountAsync(created.IDGiamGia, spId);
                    }
                }
                else if (vm.SelectedSPCTs != null && vm.SelectedSPCTs.Any())
                {
                    foreach (var spctId in vm.SelectedSPCTs)
                    {
                        await _repo.AddSanPhamCTToDiscountAsync(created.IDGiamGia, spctId);
                    }
                }

                TempData["Success"] = "Tạo giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo giảm giá: {ex.Message}");
                await LoadDropdowns(vm);
                return View(vm);
            }
        }

        // ---------------- PRIVATE HELPER ----------------
        private async Task LoadDropdowns(GiamGiaCreateVM vm)
        {
            vm.DanhMucs = await _dmRepo.GetAllDanhMucsAsync();
            vm.SanPhams = await _spRepo.GetAllSanPhamAsync();

            vm.SPCTs = (await _spctRepo.GetAllAsync()).Select(spct => new SanPhamCTViewModel
            {
                IDSanPhamCT = spct.IDSanPhamCT,
                IDSanPham = spct.IDSanPham,
                TenSanPham = spct.SanPham?.TenSanPham,
                Size = spct.SizeAo?.SoSize,
                MauSac = spct.MauSac?.TenMau,
                GiaBan = spct.GiaBan
            }).ToList();
        }
    }
}
