using AppView.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using WebModels.Models;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamCTController : Controller
    {
        private readonly ISanPhamCTRepo _repo;

        public SanPhamCTController(ISanPhamCTRepo repo)
        {
            _repo = repo;
        }

        // GET: SanPhamCT
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAll();
            return View(list);
        }

        // GET: SanPhamCT/Create
        public IActionResult Create() => View();

        // POST: SanPhamCT/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamCT model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;

                var result = await _repo.Create(model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không thể tạo sản phẩm chi tiết.");
                    return View(model);
                }

                TempData["Message"] = "✅ Tạo sản phẩm chi tiết thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo: " + ex.Message);
                return View(model);
            }
        }

        // GET: SanPhamCT/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _repo.GetByID(id);
            return item == null ? NotFound() : View(item);
        }

        // POST: SanPhamCT/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamCT model)
        {
            if (id != model.IDSanPhamCT)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var result = await _repo.Update(id, model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy sản phẩm chi tiết để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // GET: SanPhamCT/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repo.GetByID(id);
            return item == null ? NotFound() : View(item);
        }

        // POST: SanPhamCT/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var result = await _repo.Delete(id);
                if (!result)
                {
                    TempData["Error"] = "Không thể xoá sản phẩm chi tiết.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = "✅ Xoá thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: SanPhamCT/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var msg = await _repo.Toggle(id);
                TempData["Message"] = msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi đổi trạng thái: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
