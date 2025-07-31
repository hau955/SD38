using Microsoft.AspNetCore.Mvc;
using AppView.ViewModels.SanPham;
using System.Text.Json;

namespace AppView.Controllers
{
    public class CTSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public CTSanPhamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/"); // Thay bằng URL API
        }

        public async Task<IActionResult> Index()
        {
            List<SanPhamView> products = new List<SanPhamView>();
            try
            {
                var response = await _httpClient.GetAsync("api/SanPham/with-chi-tiet"); // Thay đổi endpoint
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {content}"); // Log response
                var jsonResponse = JsonSerializer.Deserialize<List<SanPhamView>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                products = jsonResponse ?? new List<SanPhamView>();
                if (products.Count == 0)
                {
                    ViewBag.Info = "Không có sản phẩm nào trong danh sách.";
                }
                else
                {
                    Console.WriteLine($"Số lượng sản phẩm tải được: {products.Count}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message} - Status: {ex.StatusCode}");
                ViewBag.Error = "Không thể tải danh sách sản phẩm. Vui lòng thử lại sau. (Chi tiết: " + ex.Message + ")";
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                ViewBag.Error = "Lỗi phân tích dữ liệu từ server. Vui lòng thử lại sau. (Chi tiết: " + ex.Message + ")";
            }
            return View(products);
        }
    }

    public class JsonResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}

