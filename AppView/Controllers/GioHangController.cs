using AppData.Models;
using AppView.ViewModels.GioHang;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
    public class GioHangController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token") ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            List<GioHangView> response = new List<GioHangView>();
            try
            {
                var rawResponse = await _httpClient.GetFromJsonAsync<List<GioHangCT>>("api/GioHang") ?? new List<GioHangCT>();
                response = MapToGioHangView(rawResponse);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching cart: {ex.Message}");
                ViewBag.Error = "Không thể tải giỏ hàng. Vui lòng thử lại sau.";
            }

            ViewBag.Token = token;
            return View(response);
        }

        private List<GioHangView> MapToGioHangView(List<GioHangCT> gioHangCTs)
        {
            return gioHangCTs.Select(gct => new GioHangView
            {
                IDGioHangChiTiet = gct.IDGioHangChiTiet,
                TenSanPham = gct.SanPham?.TenSanPham ?? "Không có tên",
                SoLuong = gct.SoLuong,
                DonGia = gct.DonGia
            }).ToList();
        }
    }
}

