using AppApi.IService;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhMucController : Controller
    {
        private readonly IDanhMucRePo _service;

        public DanhMucController(IDanhMucRePo danhMucService)
        {
            _service = danhMucService;
        }

        public async Task<IActionResult> Index()
        {
            var danhMucs = await _service.GetAllDanhMucsAsync();
            return View(danhMucs);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DanhMucViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.CreateDanhMucAsync(model);
            if (result)
                return RedirectToAction("Index");

            ViewBag.ErrorMessage = "Không thể thêm danh mục. Vui lòng kiểm tra lại.";
            return View(model);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _service.GetDanhMucByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DanhMucViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _service.UpdateDanhMucAsync(model);
            if (result)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại");
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteDanhMucAsync(id);
            return RedirectToAction("Index");
        }
    }
}
