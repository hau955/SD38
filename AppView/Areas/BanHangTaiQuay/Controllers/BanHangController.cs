using Microsoft.AspNetCore.Mvc;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using AppView.Areas.BanHangTaiQuay.ViewModels.BanHangViewModels;
using AppView.Areas.BanHangTaiQuay.IRepo;
namespace AppView.Areas.BanHangTaiQuay.Controllers
{
    [Area("BanHangTaiQuay")]
    public class BanHangController : Controller
    {
        private readonly IBanHangfRepo _banHangRepo;

        public BanHangController(IBanHangfRepo banHangRepo)
        {
            _banHangRepo = banHangRepo;
        }
        [HttpGet]
        public async Task<IActionResult> SanPham(Guid? idHoaDon)
        {
            // 🔁 1. Gọi API local để lấy danh sách sản phẩm
            using var client = new HttpClient();
            var sanPhams = await client.GetFromJsonAsync<List<SanPhamView>>("https://localhost:7221/api/SanPham/with-chi-tiet");

            // 🔁 2. Truyền ID hóa đơn xuống View
            ViewBag.IDHoaDon = idHoaDon;

            // 🔁 3. Gọi repo để lấy chi tiết hóa đơn nếu có
            if (idHoaDon.HasValue)
            {
                var hoaDonResult = await _banHangRepo.XemChiTietHoaDonAsync(idHoaDon.Value);
                if (hoaDonResult.IsSuccess)
                {
                    ViewBag.HoaDonChiTiet = hoaDonResult.Data;
                }
                else
                {
                    ViewBag.HoaDonChiTiet = null;
                    ViewBag.HoaDonError = hoaDonResult.Message;
                }
            }

            return View(sanPhams ?? new List<SanPhamView>());
        }


        public async Task<IActionResult> HoaDonCho()
        {

            var idnguoitao = HttpContext.Session.GetString("ID");

            if (string.IsNullOrEmpty(idnguoitao) || !Guid.TryParse(idnguoitao, out var idNguoiTao))
            {
                return Unauthorized(); // hoặc RedirectToAction("Login");
            }

            var hoaDons = await _banHangRepo.GetHoaDonChoAsync(idNguoiTao);
            return View(hoaDons);
        }

        [HttpPost]
        public async Task<IActionResult> BanTaiQuay(BanHangViewModel model)
        {
            Guid idNguoiTao = model.IDNguoiTao ?? Guid.Empty;


            var result = await _banHangRepo.BanTaiQuayAsync(model);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("HoaDonCho");
            }

            TempData["Success"] = "Tạo hóa đơn thành công";
            return RedirectToAction("HoaDonCho");
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToanHoaDonCho(ThanhToanHoaDonRequest model)
        {
            var result = await _banHangRepo.ThanhToanHoaDonChoAsync(model);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("HoaDonCho");
            }

            // Thay vì return View(...) => redirect tới action GET
            return RedirectToAction("InHoaDon", new { id = model.IDHoaDon });
        }


        [HttpPost]
        public async Task<IActionResult> ThemSanPhamVaoHoaDonCho(ThemSanPham model)
        {
            var result = await _banHangRepo.ThemSanPhamVaoHoaDonChoAsync(model);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("SanPham", new { idHoaDon = model.IDHoaDon });
        }

        [HttpPost]
        public async Task<IActionResult> TruSanPhamKhoiHoaDonCho(TruSanPham model)
        {
            var result = await _banHangRepo.TruSanPhamKhoiHoaDonChoAsync(model);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("SanPham", new { idHoaDon = model.IDHoaDon });
        }

        [HttpPost]
        public async Task<IActionResult> HuyHoaDon(Guid idHoaDon)
        {
            var result = await _banHangRepo.HuyHoaDonAsync(idHoaDon);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("HoaDonCho");
        }
        [HttpGet]
        public async Task<IActionResult> InHoaDon(Guid id)
        {
            var hoaDonResult = await _banHangRepo.XemChiTietHoaDonAsync(id);
            if (!hoaDonResult.IsSuccess)
            {
                TempData["Error"] = hoaDonResult.Message;
                return RedirectToAction("HoaDonCho");
            }

            return View("InHoaDon", hoaDonResult.Data);
        }

    }
}
