using AppData.Models;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChatLieuController : Controller
    {
        private readonly IChatLieuRepo _ChatLieuRepo;

        public ChatLieuController(IChatLieuRepo ChatLieuRepo)
        {
            _ChatLieuRepo = ChatLieuRepo;
        }

        // GET: ChatLieu
        public async Task<IActionResult> Index()
        {
            var list = await _ChatLieuRepo.GetAll();
            return View(list);
        }

        // GET: ChatLieu/Create
        public async Task<IActionResult> Create()
        {
            await LoadChatLieuListAsync();
            return View();
        }

        // POST: ChatLieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChatLieu model)
        {
            await LoadChatLieuListAsync();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.IDChatLieu = Guid.NewGuid();
                model.NgayTao = DateTime.Now;
                model.NgaySua = DateTime.Now;
     
                await _ChatLieuRepo.Create(model);
                TempData["Message"] = "✅ Tạo ChatLieu thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi tạo ChatLieu: " + ex.Message);
                return View(model);
            }
        }

        // GET: ChatLieu/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var ChatLieu = await _ChatLieuRepo.GetByID(id);
            if (ChatLieu == null)
                return NotFound();

            await LoadChatLieuListAsync();
            return View(ChatLieu);
        }

        // POST: ChatLieu/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChatLieu model)
        {
            await LoadChatLieuListAsync();

            if (id != model.IDChatLieu)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var success = await _ChatLieuRepo.Update(id, model);

                if (success == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy ChatLieu để cập nhật.");
                    return View(model);
                }

                TempData["Message"] = "✅ Cập nhật ChatLieu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "❌ Lỗi khi cập nhật ChatLieu: " + ex.Message);
                return View(model);
            }
        }

        // GET: ChatLieu/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var ChatLieu = await _ChatLieuRepo.GetByID(id);
            if (ChatLieu == null) return NotFound();
            return View(ChatLieu);
        }

        // POST: ChatLieu/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                await _ChatLieuRepo.Delete(id);
                TempData["Message"] = "🗑️ Xoá ChatLieu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi xoá ChatLieu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // PATCH: ChatLieu/ToggleStatus
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var msg = await _ChatLieuRepo.Toggle(id);
                TempData["Message"] = msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi đổi trạng thái: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Gán danh sách ChatLieu vào ViewBag nếu cần sử dụng dropdown trong View.
        /// </summary>
        private async Task LoadChatLieuListAsync()
        {
            var list = await _ChatLieuRepo.GetAll();
            ViewBag.ChatLieuList = list.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.IDChatLieu.ToString(),
                Text = s.TenChatLieu
            }).ToList();
        }
    }
}
