using AppData.Models;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;
using AppData.Models;

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
                TempData["Message"] = "? T?o size th�nh c�ng!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "? L?i khi t?o size: " + ex.Message);
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
                return BadRequest("ID kh�ng kh?p.");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.NgaySua = DateTime.Now;
                var success = await _sizeRepo.Update(id, model);

                if (success == null)
                {
                    ModelState.AddModelError("", "Kh�ng t�m th?y size d? c?p nh?t.");
                    return View(model);
                }

                TempData["Message"] = "? C?p nh?t size th�nh c�ng!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "? L?i khi c?p nh?t size: " + ex.Message);
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteS(Guid id)
        {
            try
            {
                var deleteSize = await _sizeRepo.Delete(id);
                if (!deleteSize) { TempData["Message"] = "Xo� size kh�ng th�nh c�ng!"; return RedirectToAction(nameof(Index)); }
                TempData["Message"] = "? Xo� size th�nh c�ng!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "? L?i khi xo� size: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: Size/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var msg = await _sizeRepo.Toggle(id);
                TempData["Message"] = msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "? L?i khi d?i tr?ng th�i: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// G�n danh s�ch size v�o ViewBag n?u c?n s? d?ng dropdown trong View.
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
