using AppView.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppView.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly HttpClient _httpClient;

        public HoaDonController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/api/"); // URL API của bạn
        }

        // Danh sách hóa đơn của user
        public async Task<IActionResult> Index()
        {
            var idUserString = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(idUserString))
            {
                TempData["Error"] = "Bạn chưa đăng nhập!";
                return RedirectToAction("Login", "Auth"); // tuỳ logic login của bạn
            }

            Guid idUser = Guid.Parse(idUserString);

            var response = await _httpClient.GetAsync($"HoaDon/user/{idUser}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không lấy được danh sách hóa đơn";
                return View(new List<HoaDonView>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var hoaDons = JsonConvert.DeserializeObject<List<HoaDonView>>(json);

            return View(hoaDons);
        }

        // Chi tiết hóa đơn
        // Chi tiết hóa đơn
        public async Task<IActionResult> Details(Guid idHoaDon)
        {
            var response = await _httpClient.GetAsync($"HoaDon/ChiTiet/{idHoaDon}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không lấy được chi tiết hóa đơn";
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            // Nếu API trả ra { success: true, data: {...} } thì phải bóc data
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(json);

            var hoaDonJson = apiResponse.data.ToString();
            var hoaDon = JsonConvert.DeserializeObject<HoaDonView>(hoaDonJson);

            return View(hoaDon); // ✅ Truyền đúng model cho View
        }

    }
}
