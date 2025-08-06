using AppData.Models;
using AppView.ViewModels.GioHang;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json;

namespace AppView.Controllers
{
    public class GioHangController : Controller
    {
       private readonly HttpClient _httpClient;

        public GioHangController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/");
        }

        public async Task<IActionResult> Index()
        {
            List<GioHangView> cartItems = new List<GioHangView>();
            ViewBag.Error = null;
            try
            {
                var id = HttpContext.Session.GetString("ID");
                if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid userId))
                {
                    ViewBag.Error = "Người dùng chưa đăng nhập hoặc ID không hợp lệ.";
                    ViewBag.Token = "";
                    ViewBag.UserId = "";
                    return View(cartItems);
                }

                var token = HttpContext.Session.GetString("Token");
                ViewBag.Token = token ?? "";
                ViewBag.UserId = id;
                _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                    ? null
                    : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/GioHang/lay-gio-hang/{userId}");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseString}");

                var result = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseString);
                if (result.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var gioHangList = System.Text.Json.JsonSerializer.Deserialize<List<GioHangCT>>(dataElement.GetRawText(), options);
                    if (gioHangList != null && gioHangList.Any())
                    {
                        cartItems = gioHangList.Select(item => new GioHangView
                        {
                            IDGioHangChiTiet = item.IDGioHangChiTiet,
                            TenSanPham = item.SanPhamCT?.SanPham?.TenSanPham ?? "Không có tên",
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia
                        }).ToList();
                    }
                    else
                    {
                        ViewBag.Error = "Giỏ hàng trống.";
                    }
                }
                else
                {
                    ViewBag.Error = "Dữ liệu giỏ hàng không hợp lệ từ server.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ViewBag.Error = "Không thể tải giỏ hàng. Vui lòng thử lại sau.";
                ViewBag.Token = "";
                ViewBag.UserId = "";
            }
            return View(cartItems);
        }

        private List<GioHangView> MapToGioHangView(List<GioHangCT> data)
        {
            return data.Select(item => new GioHangView
            {
                IDGioHangChiTiet = item.IDGioHangChiTiet,
                TenSanPham = item.SanPhamCT?.SanPham?.TenSanPham ?? "Không có tên",
                SoLuong = item.SoLuong,
                DonGia = item.DonGia
            }).ToList();
        }
    }
}


