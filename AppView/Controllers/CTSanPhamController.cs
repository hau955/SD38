using Microsoft.AspNetCore.Mvc;
using AppView.ViewModels.SanPham;

namespace AppView.Controllers
{
    public class CTSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;

        public CTSanPhamController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/"); // địa chỉ API của bạn
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetFromJsonAsync<List<SanPhamView>>("api/SanPham/with-chi-tiet");

            return View(response);
        }
    }
}
