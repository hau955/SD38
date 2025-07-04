using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;
using AppData.Models;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaAoController : Controller
    {
        private readonly ITaAoRepo _taAoRepo;

        public TaAoController(ITaAoRepo taAoRepo)
        {
            _taAoRepo = taAoRepo;
        }

        // GET: TaAo
        public async Task<IActionResult> Index()
        {
            var list = await _taAoRepo.GetAll();
            return View(list);
        }

        // GET: TaAo/Create
        public IActionResult Create() => View();

        // POST: TaAo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaAo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;

                var result = await _taAoRepo.Create(model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không thể tạo tà áo.");
                    return View(model);
                }

                TempData["Message"] = "✅ Tạo tà áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo tà áo: " + ex.Message);
                return View(model);
            }
        }

        // GET: TaAo/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _taAoRepo.GetByID(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy tà áo.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: TaAo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TaAo model)
        {
            if (id != model.IDTaAo)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;

                var result = await _taAoRepo.Update(id, model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy tà áo để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật tà áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật: " + ex.Message);
                return View(model);
            }
        }

        // GET: TaAo/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _taAoRepo.GetByID(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy tà áo.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: TaAo/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var result = await _taAoRepo.Delete(id);
                if (!result)
                {
                    TempData["Error"] = "Không thể xoá tà áo.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = "✅ Xoá tà áo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá tà áo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: TaAo/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var message = await _taAoRepo.Toggle(id);
                TempData["Message"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi đổi trạng thái: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
