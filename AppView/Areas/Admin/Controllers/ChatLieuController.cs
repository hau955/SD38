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
                TempData["Message"] = "? T?o ChatLieu th�nh c�ng!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "? L?i khi t?o ChatLieu: " + ex.Message);
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
                return BadRequest("ID kh�ng kh?p.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var success = await _ChatLieuRepo.Update(id, model);

                if (success == null)
                {
                    ModelState.AddModelError("", "Kh�ng t�m th?y ChatLieu d? c?p nh?t.");
                    return View(model);
                }

                TempData["Message"] = "? C?p nh?t ChatLieu th�nh c�ng!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "? L?i khi c?p nh?t ChatLieu: " + ex.Message);
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCL(Guid id)
        {
            try
            {
                await _ChatLieuRepo.Delete(id);
                TempData["Message"] = "??? Xo� ChatLieu th�nh c�ng!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "? L?i khi xo� ChatLieu: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }


        // POST: ChatLieu/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var msg = await _ChatLieuRepo.Toggle(id);
                TempData["Message"] = msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "? L?i khi d?i tr?ng th�i: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// G�n danh s�ch ChatLieu v�o ViewBag n?u c?n s? d?ng dropdown trong View.
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
