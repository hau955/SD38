using AppData.Models;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly ISizeRepo _sizeRepo;

        public SizeController(ISizeRepo sizeRepo)
        {
            _sizeRepo = sizeRepo;
        }

        // GET: Size
        public async Task<IActionResult> Index()
        {
            var list = await _sizeRepo.GetAll();
            return View(list);
        }

        // GET: Size/Create
        public async Task<IActionResult> Create()
        {
            await LoadSizeListAsync();
            return View();
        }

        // POST: Size/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Size model)
        {
            await LoadSizeListAsync();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.IDSize = Guid.NewGuid();
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;
                model.TrangThai = true;

                await _sizeRepo.Create(model);
                TempData["Message"] = "✅ Tạo size thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo size: " + ex.Message);
                return View(model);
            }
        }

        // GET: Size/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var size = await _sizeRepo.GetByID(id);
            if (size == null)
                return NotFound();

            await LoadSizeListAsync();
            return View(size);
        }

        // POST: Size/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Size model)
        {
            await LoadSizeListAsync();

            if (id != model.IDSize)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var success = await _sizeRepo.Update(id, model);

                if (success == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy size để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật size thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật size: " + ex.Message);
                return View(model);
            }
        }

        // GET: Size/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var size = await _sizeRepo.GetByID(id);
            if (size == null) return NotFound();
            return View(size);
        }

        // POST: Size/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                await _sizeRepo.Delete(id);
                TempData["Message"] = "🗑️ Xoá size thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá size: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: Size/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var msg = await _sizeRepo.Toggle(id);
                TempData["Message"] = msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi đổi trạng thái: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Gán danh sách size vào ViewBag nếu cần sử dụng dropdown trong View.
        /// </summary>
        private async Task LoadSizeListAsync()
        {
            var list = await _sizeRepo.GetAll();
            ViewBag.SizeList = list.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.IDSize.ToString(),
                Text = s.SoSize
            }).ToList();
        }
    }
}
