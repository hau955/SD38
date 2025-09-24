using AppData.Models;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MauSacController : Controller
    {
        private readonly IMauSacRepo _mauSacRepo;

        public MauSacController(IMauSacRepo mauSacRepo)
        {
            _mauSacRepo = mauSacRepo;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? statusFilter, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var list = await _mauSacRepo.GetAll();

            // 🔎 Tìm kiếm
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                list = list
                    .Where(x => x.TenMau.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // ⚡ Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                list = statusFilter switch
                {
                    "active" => list.Where(x => x.TrangThai).ToList(),
                    "inactive" => list.Where(x => !x.TrangThai).ToList(),
                    _ => list
                };
            }

            // ↕️ Sắp xếp
            ViewBag.SortOrder = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            list = sortOrder switch
            {
                "name" => list.OrderBy(x => x.TenMau).ToList(),
                "name_desc" => list.OrderByDescending(x => x.TenMau).ToList(),
                _ => list
            };

            // ⚙️ Giữ filter cho View
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.PageSize = pageSize;

            // 📄 Phân trang
            var pagedList = list.ToPagedList(page, pageSize);

            return View(pagedList);
        }

        // GET: MauSac/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: MauSac/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MauSac model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.";
                return View(model);
            }

            try
            {
                // Validation bổ sung
                if (string.IsNullOrWhiteSpace(model.TenMau))
                {
                    ModelState.AddModelError("TenMau", "Tên màu sắc không được để trống.");
                    return View(model);
                }

                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;

                var created = await _mauSacRepo.Create(model);
                if (created == null)
                {
                    TempData["Error"] = "❌ Không thể tạo màu sắc. Vui lòng thử lại.";
                    return View(model);
                }

                TempData["Message"] = "✅ Tạo màu sắc thành công!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi dữ liệu: {ex.Message}");
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi nghiệp vụ: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi hệ thống: {ex.Message}");
                TempData["Error"] = "❌ Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.";
                return View(model);
            }
        }

        // GET: MauSac/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _mauSacRepo.GetByID(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: MauSac/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MauSac model)
        {
            if (id != model.IDMauSac)
            {
                TempData["Error"] = "❌ ID không khớp. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.";
                return View(model);
            }

            try
            {
                // Validation bổ sung
                if (string.IsNullOrWhiteSpace(model.TenMau))
                {
                    ModelState.AddModelError("TenMau", "Tên màu sắc không được để trống.");
                    return View(model);
                }

                model.NgaySua = DateTime.Now;
                var updated = await _mauSacRepo.Update(id, model);

                if (updated == null)
                {
                    TempData["Error"] = "❌ Không tìm thấy màu sắc để cập nhật.";
                    return RedirectToAction("Index");
                }

                TempData["Message"] = "✅ Cập nhật màu sắc thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi dữ liệu: {ex.Message}");
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi nghiệp vụ: {ex.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"❌ Lỗi hệ thống: {ex.Message}");
                TempData["Error"] = "❌ Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMS(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "❌ ID không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                var message = await _mauSacRepo.Delete(id);
                TempData["Message"] = message;

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = $"❌ Lỗi dữ liệu: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"❌ Lỗi nghiệp vụ: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MauSac/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "❌ ID không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                var message = await _mauSacRepo.Toggle(id);
                TempData["Message"] = message;
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = $"❌ Lỗi dữ liệu: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"❌ Lỗi nghiệp vụ: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Lỗi hệ thống: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
