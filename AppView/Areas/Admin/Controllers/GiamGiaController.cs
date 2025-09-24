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
                TenSanPham = spct.TenSanPham,
                SoSize = spct.SoSize,            // ✅ Size
                TenMau = spct.TenMau,            // ✅ Màu
                TenChatLieu = spct.TenChatLieu,// ✅ Chất liệu
                GiaBan = spct.GiaBan,
                SoLuongTonKho = spct.SoLuongTonKho
            }).ToList();
        }

        // ---------------- UPDATE (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var giamGia = await _repo.GetByIdAsync(id);
            if (giamGia == null) return NotFound();

            var vm = new GiamGiaCreateVM
            {
                GiamGia = giamGia
            };

            await LoadDropdowns(vm);
            return View(vm);
        }

        // ---------------- UPDATE (POST) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGiaCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }

            try
            {
                var updated = await _repo.UpdateAsync(id, vm.GiamGia);
                if (!updated)
                {
                    ModelState.AddModelError("", "Không tìm thấy giảm giá để cập nhật");
                    await LoadDropdowns(vm);
                    return View(vm);
                }

                TempData["Success"] = "Cập nhật giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi cập nhật giảm giá: {ex.Message}");
                await LoadDropdowns(vm);
                return View(vm);
            }
        }// ---------------- DELETE (GET) ----------------
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var giamGia = await _repo.GetByIdAsync(id);
            if (giamGia == null) return NotFound();

            return View(giamGia); // View confirm delete
        }

        // ---------------- DELETE (POST) ----------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var deleted = await _repo.DeleteAsync(id);
                if (!deleted)
                {
                    TempData["Error"] = "Không tìm thấy giảm giá để xóa!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Xóa giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa giảm giá: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
