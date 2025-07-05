using AppApi.IService;
using AppData.Models;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.SanPhamChiTietViewModels;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamCTController : Controller
    {
        private readonly ISanPhamCTRepo _service;

        public SanPhamCTController(ISanPhamCTRepo service)
        {
            _service = service;
        }
        public async Task<IActionResult> ThemChiTiet(Guid idSanPham)
        {
            ViewBag.IDSanPham = idSanPham;
            ViewBag.MauSacs = await _service.GetMauSacsAsync();
            ViewBag.Sizes = await _service.GetSizesAsync();
            ViewBag.CoAos = await _service.GetCoAosAsync();
            ViewBag.TaAos = await _service.GetTaAosAsync();

            return View(new List<SanPhamCT> { new SanPhamCT { IDSanPham = idSanPham } });
        }


        public async Task<IActionResult> DanhSach(Guid idSanPham)
        {
            var list = await _service.GetBySanPhamIdAsync(idSanPham);
            var mauSacs = await _service.GetMauSacsAsync();
            var sizes = await _service.GetSizesAsync();
            var coAos = await _service.GetCoAosAsync();
            var taAos = await _service.GetTaAosAsync();

            // Join dữ liệu
            foreach (var item in list)
            {
                item.MauSac = mauSacs.FirstOrDefault(m => m.IDMauSac == item.IDMauSac);
                item.SizeAo = sizes.FirstOrDefault(s => s.IDSize == item.IDSize);
                item.CoAo = coAos.FirstOrDefault(c => c.IDCoAo == item.IDCoAo);
                item.TaAo = taAos.FirstOrDefault(t => t.IDTaAo == item.IDTaAo);
               
            }

            return View(list);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMultiple(List<SanPhamCT> list)
        {
            var success = await _service.CreateMultipleAsync(list);
            if (success)
                return RedirectToAction("DanhSach", new { idSanPham = list.First().IDSanPham });

            TempData["Error"] = "Thêm thất bại!";
            return RedirectToAction("ThemChiTiet", new { idSanPham = list.First().IDSanPham });
        }
        [HttpGet]
        public async Task<IActionResult> CapNhat(Guid id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null) return NotFound();

            ViewBag.MauSacs = await _service.GetMauSacsAsync();
            ViewBag.Sizes = await _service.GetSizesAsync();
            ViewBag.CoAos = await _service.GetCoAosAsync();
            ViewBag.TaAos = await _service.GetTaAosAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhat(SanPhamCT model)
        {
            var success = await _service.UpdateAsync(model);
            if (success)
                return RedirectToAction("DanhSach", new { idSanPham = model.IDSanPham });

            TempData["Error"] = "Cập nhật thất bại!";
            return View(model);
        }

    }
}
