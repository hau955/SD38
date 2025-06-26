using AppView.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;
using WebModels.Models;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoAoController : Controller
    {
        private readonly ICoAoRepo _coAoRepo;

        public CoAoController(ICoAoRepo coAoRepo)
        {
            _coAoRepo = coAoRepo;
        }

        // GET: CoAo
        public async Task<IActionResult> Index()
        {
            var list = await _coAoRepo.GetAll();
            return View(list);
        }

        // GET: CoAo/Create
        public IActionResult Create() => View();

        // POST: CoAo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoAo model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;

                var result = await _coAoRepo.Create(model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không thể tạo cổ áo.");
                    return View(model);
                }

                TempData["Message"] = "✅ Tạo cổ áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo cổ áo: " + ex.Message);
                return View(model);
            }
        }

        // GET: CoAo/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _coAoRepo.GetByID(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy cổ áo.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: CoAo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CoAo model)
        {
            if (id != model.IDCoAo)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;

                var result = await _coAoRepo.Update(id, model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy cổ áo để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật cổ áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // GET: CoAo/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _coAoRepo.GetByID(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy cổ áo.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: CoAo/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var result = await _coAoRepo.Delete(id);
                if (!result)
                {
                    TempData["Error"] = "Không tìm thấy cổ áo để xoá.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = "✅ Xoá cổ áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá cổ áo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: CoAo/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var message = await _coAoRepo.Toggle(id);
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
