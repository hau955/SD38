

using Microsoft.AspNetCore.Mvc;
using AppView.Areas.BanHangTaiQuay.IRepo;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IBanHangfRepo _api;

        public HomeController(IBanHangfRepo api)
        {
            _api = api;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
        //public async Task<IActionResult> DanhSachHoaDonCho()
        //{
        //    var hoaDons = await _api.GetHoaDonChoAsync(); // kiểu List<HoaDonChoVM>
        //    return View(hoaDons); 
        //}

        //[HttpPost]
        //public async Task<IActionResult> BanTaiQuay(BanHangViewModel request)
        //{
        //    var result = await _api.BanTaiQuayAsync(request);
        //    if (!result.IsSuccess)
        //    {
        //        TempData["Error"] = result.Message;
        //        return RedirectToAction("DanhSachHoaDonCho");
        //    }

        //    TempData["Success"] = result.Message;
        //    return RedirectToAction("HoaDonChiTiet", new { id = result.HoaDonId });
        //}

        //[HttpPost]
        //public async Task<IActionResult> ThanhToanHoaDonCho(ThanhToanHoaDonRequest request)
        //{
        //    var result = await _api.ThanhToanHoaDonChoAsync(request);
        //    TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        //    return RedirectToAction("DanhSachHoaDonCho");
        //}

        //[HttpPost]
        //public async Task<IActionResult> ThemSanPhamVaoHoaDonCho(ThemSanPham request)
        //{
        //    var result = await _api.ThemSanPhamVaoHoaDonChoAsync(request);
        //    TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        //    return RedirectToAction("HoaDonChiTiet", new { id = request.IDHoaDon });
        //}

        //[HttpPost]
        //public async Task<IActionResult> TruSanPhamHoaDonCho(TruSanPham request)
        //{
        //    var result = await _api.TruSanPhamHoaDonChoAsync(request);
        //    TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
        //    return RedirectToAction("HoaDonChiTiet", new { id = request.IDHoaDon });
        //}

        //public IActionResult HoaDonChiTiet(Guid id)
        //{
        //    // TODO: Gọi API lấy chi tiết hóa đơn (tùy yêu cầu)
        //    return View(); // view rỗng chờ xử lý
        //}
    }
}
