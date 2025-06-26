using AppView.Areas.Admin.Repository;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using System.Net.Http;
using WebModels.Models;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamRepo _sanPhamRepo;
        private readonly ISizeRepo _sizeRepo;
        private readonly IMauSacRepo _mauSacRepo;
        private readonly ICoAoRepo _coAoRepo;
        private readonly ITaAoRepo _taAoRepo;
        private readonly ISanPhamCTRepo _sanPhamCTRepo;

        public SanPhamController(
            ISanPhamRepo sanPhamRepo,
            ISizeRepo sizeRepo,
            IMauSacRepo mauSacRepo,
            ICoAoRepo coAoRepo,
            ITaAoRepo taAoRepo,
            ISanPhamCTRepo sanPhamCTRepo)
        {
            _sanPhamRepo = sanPhamRepo;
            _sizeRepo = sizeRepo;
            _mauSacRepo = mauSacRepo;
            _coAoRepo = coAoRepo;
            _taAoRepo = taAoRepo;
            _sanPhamCTRepo = sanPhamCTRepo;
        }

        public async Task<IActionResult> Index(int? page, string sortOrder, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var sanPhams = await _sanPhamRepo.GetAll();

            if (!String.IsNullOrEmpty(searchString))
                sanPhams = sanPhams.Where(s => s.TenSanPham.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

            sanPhams = sortOrder switch
            {
                "name_desc" => sanPhams.OrderByDescending(s => s.TenSanPham).ToList(),
                "Date" => sanPhams.OrderBy(s => s.NgayTao).ToList(),
                "date_desc" => sanPhams.OrderByDescending(s => s.NgayTao).ToList(),
                _ => sanPhams.OrderBy(s => s.TenSanPham).ToList(),
            };

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View(sanPhams.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var sanPham = await _sanPhamRepo.GetByID(id);
            if (sanPham == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            var chiTiet = sanPham.SanPhamChiTiets?.FirstOrDefault();
            if (chiTiet == null)
            {
                TempData["Error"] = "Sản phẩm chưa có thông tin chi tiết.";
                return RedirectToAction("Index");
            }

            var model = new SanPhamCTViewModel
            {
                IDSanPham = sanPham.IDSanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                TrongLuong = sanPham.TrongLuong,
                GioiTinh = sanPham.GioiTinh,
                TrangThai = sanPham.TrangThai,
                HinhAnh = sanPham.HinhAnh,
                IdSize = chiTiet.IDSize,
                IdMauSac = chiTiet.IDMauSac,
                IdCoAo = chiTiet.IDCoAo,
                IdTaAo = chiTiet.IDTaAo
            };

            await LoadDropdowns(model); // Gán SizeList, CoAoList, TaAoList, MauSacList
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new SanPhamCTViewModel();
            await LoadDropdowns(model);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamCTViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            try
            {
                var created = await _sanPhamRepo.Create(model);
                TempData["Message"] = "Tạo sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo: " + ex.Message);
                await LoadDropdowns(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var sanPham = await _sanPhamRepo.GetByID(id);
            if (sanPham == null) return NotFound();

            // Nếu bạn muốn dùng ViewModel cho Edit:
            var model = new SanPhamCTViewModel
            {
                IDSanPham = sanPham.IDSanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                TrongLuong = sanPham.TrongLuong,
                GioiTinh = sanPham.GioiTinh,
                TrangThai = sanPham.TrangThai,
                HinhAnh = sanPham.HinhAnh
                // Gán các ID chi tiết nếu cần
            };

            await LoadDropdowns(model);
            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamCTViewModel model)
        {
            if (id != model.IDSanPham) return BadRequest("ID không khớp.");
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            try
            {
                // xử lý ảnh nếu có
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.ImageFile.CopyToAsync(stream);
                    model.HinhAnh = "/images/" + fileName;
                }

                var updated = await _sanPhamRepo.Update(model);
                if (updated == null) return NotFound();

                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                await LoadDropdowns(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var sanPham = await _sanPhamRepo.GetByID(id);
            return sanPham == null ? NotFound() : View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _sanPhamRepo.Delete(id);
                if (!result) return NotFound();

                TempData["Message"] = "Xoá sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xoá sản phẩm: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                TempData["Message"] = await _sanPhamRepo.Toggle(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật trạng thái: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        private async Task LoadDropdowns(SanPhamCTViewModel model)
        {
            var sizes = await _sizeRepo.GetAll() ?? new List<Size>();
            var coAos = await _coAoRepo.GetAll() ?? new List<CoAo>();
            var taAos = await _taAoRepo.GetAll() ?? new List<TaAo>();
            var mauSacs = await _mauSacRepo.GetAll() ?? new List<MauSac>();

            model.SizeList = sizes.Select(x => new SelectListItem { Value = x.IDSize.ToString(), Text = x.SoSize }).ToList();
            model.CoAoList = coAos.Select(x => new SelectListItem { Value = x.IDCoAo.ToString(), Text = x.TenCoAo }).ToList();
            model.TaAoList = taAos.Select(x => new SelectListItem { Value = x.IDTaAo.ToString(), Text = x.TenTaAo }).ToList();
            model.MauSacList = mauSacs.Select(x => new SelectListItem { Value = x.IDMauSac.ToString(), Text = x.TenMau }).ToList();
        }
    }
}
