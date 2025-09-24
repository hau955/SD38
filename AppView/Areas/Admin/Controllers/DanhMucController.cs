using AppApi.IService;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;

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


public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 10)
    {
        var list = await _service.GetAllDanhMucsAsync();

        // Lọc theo từ khóa
        if (!string.IsNullOrEmpty(searchTerm))
        {
            list = list.Where(x => x.TenDanhMuc.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Gửi filter về ViewBag để giữ lại giá trị form
        ViewBag.CurrentFilter = searchTerm;
        ViewBag.PageSize = pageSize;

        // Convert sang PagedList
        var pagedList = list.ToPagedList(page, pageSize);

        return View(pagedList);
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

            var updated = await _service.UpdateDanhMucAsync(model);
            if (updated != null)
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
