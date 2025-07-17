using AppApi.IService;
using AppData.Models;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.SanPhamChiTietViewModels;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

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
            var model = new CreateSanPhamCTViewModel
            {
                IDSanPham = idSanPham,
                MauSacList = (await _service.GetMauSacsAsync())
                                .Select(x => new SelectListItem { Value = x.IDMauSac.ToString(), Text = x.TenMau })
                                .ToList(),
                SizeList = (await _service.GetSizesAsync())
                                .Select(x => new SelectListItem { Value = x.IDSize.ToString(), Text = x.SoSize })
                                .ToList(),
                ChatLieuList = (await _service.GetChatLieuAsync())
                                .Select(x => new SelectListItem { Value = x.IDChatLieu.ToString(), Text = x.TenChatLieu })
                                .ToList(),
                //TaAoList = (await _service.GetTaAosAsync())
                //                .Select(x => new SelectListItem { Value = x.IDTaAo.ToString(), Text = x.TenTaAo })
                               // .ToList()
            };

            return View(model);
        }


        public async Task<IActionResult> DanhSach(Guid idSanPham)
        {
            var list = await _service.GetBySanPhamIdAsync(idSanPham);
            var mauSacs = await _service.GetMauSacsAsync();
            var sizes = await _service.GetSizesAsync();
            var chatlieus = await _service.GetChatLieuAsync();
            //var taAos = await _service.GetTaAosAsync();

            // Join dữ liệu
            foreach (var item in list)
            {
                item.MauSac = mauSacs.FirstOrDefault(m => m.IDMauSac == item.IDMauSac);
                item.SizeAo = sizes.FirstOrDefault(s => s.IDSize == item.IDSize);
               item.ChatLieu = chatlieus.FirstOrDefault(c => c.IDChatLieu == item.IdChatLieu);
                //item.TaAo = taAos.FirstOrDefault(t => t.IDTaAo == item.IDTaAo);
               
            }

            return View(list);
        }


        [HttpPost]
        public async Task<IActionResult> CreateMultiple(CreateSanPhamCTViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin và chọn ít nhất 1 lựa chọn cho mỗi mục.";
                // Load lại danh sách để hiển thị lại
                vm.MauSacList = (await _service.GetMauSacsAsync())
     .Select(x => new SelectListItem
     {
         Value = x.IDMauSac.ToString(),
         Text = x.TenMau
     }).ToList();

                vm.SizeList = (await _service.GetSizesAsync())
                    .Select(x => new SelectListItem
                    {
                        Value = x.IDSize.ToString(),
                        Text = x.SoSize.ToString()
                    }).ToList();

                vm.ChatLieuList = (await _service.GetChatLieuAsync())
                    .Select(x => new SelectListItem
                    {
                        Value = x.IDChatLieu.ToString(),
                        Text = x.TenChatLieu
                    }).ToList();

               
                return View("ThemChiTiet", vm);
            }
            var combinations = from mau in vm.SelectedMauSacs
                               from size in vm.SelectedSizes
                               from Chatlieu in vm.SelectedChatlieus

                               select new SanPhamCT
                               {
                                   IDSanPham = vm.IDSanPham,
                                   IDMauSac = mau,
                                   IDSize = size,
                                  IdChatLieu=Chatlieu,
                                   GiaBan = vm.GiaBan,
                                   SoLuongTonKho = vm.SoLuongTonKho,
                                   TrangThai = true
                               };

            var resultList = new List<SanPhamCT>();
            foreach (var item in combinations)
            {
                var exists = await _service.ExistsAsync(item.IDSanPham, item.IDMauSac, item.IDSize ,item.IdChatLieu);
                if (!exists)
                    resultList.Add(item);
            }

            if (resultList.Count == 0)
            {
                TempData["Error"] = "Tất cả các tổ hợp đều đã tồn tại.";
                return RedirectToAction("ThemChiTiet", new { idSanPham = vm.IDSanPham });
            }

            var success = await _service.CreateMultipleAsync(resultList);

            if (success)
                return RedirectToAction("DanhSach", new { idSanPham = vm.IDSanPham });

            TempData["Error"] = "Thêm thất bại!";
            return RedirectToAction("ThemChiTiet", new { idSanPham = vm.IDSanPham });
        }


        [HttpGet]
        public async Task<IActionResult> CapNhat(Guid id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null) return NotFound();

            ViewBag.MauSacs = await _service.GetMauSacsAsync();
            ViewBag.Sizes = await _service.GetSizesAsync();
            ViewBag.ChatLieus = await _service.GetChatLieuAsync();
           // ViewBag.TaAos = await _service.GetTaAosAsync();

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
