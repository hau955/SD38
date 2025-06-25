using AppView.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using WebModels.Models;

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

        // GET: MauSac
        public async Task<IActionResult> Index()
        {
            var list = await _mauSacRepo.GetAll();
            return View(list);
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
                return View(model);

            try
            {
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;

                var created = await _mauSacRepo.Create(model);
                if (created == null)
                {
                    ModelState.AddModelError("", "Không thể tạo màu sắc.");
                    return View(model);
                }

                TempData["Message"] = "✅ Tạo màu sắc thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo màu sắc: " + ex.Message);
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
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var updated = await _mauSacRepo.Update(id, model);

                if (updated == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy màu sắc để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật màu sắc thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // GET: MauSac/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _mauSacRepo.GetByID(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: MauSac/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var deleted = await _mauSacRepo.Delete(id);
                if (!deleted)
                {
                    TempData["Error"] = "Không tìm thấy màu sắc để xoá.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = "✅ Xoá màu sắc thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: MauSac/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var message = await _mauSacRepo.Toggle(id);
                TempData["Message"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi thay đổi trạng thái: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
