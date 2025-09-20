using Microsoft.AspNetCore.Mvc;
//using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using System.Text.Json;
using AppView.Areas.Admin.IRepo;
using Newtonsoft.Json;
using AppApi.ViewModels.SanPham;

namespace AppView.Controllers
{
    public class CTSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public CTSanPhamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/api/"); // ✅ đổi thành URL API của bạn
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("SanPham/with-chi-tiet");
            if (!response.IsSuccessStatusCode) return View(new List<SanPhamView>());

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<SanPhamView>>(json);

            return View(data ?? new List<SanPhamView>());
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetAsync($"SanPham/by-id/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<dynamic>(json);

            var data = JsonConvert.DeserializeObject<SanPhamView>(apiResult.data.ToString());

            return View(data);
        }
    }

}