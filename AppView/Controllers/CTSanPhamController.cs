using Microsoft.AspNetCore.Mvc;
using AppView.ViewModels.SanPham;

namespace AppView.Controllers
{
    public class CTSanPhamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CTSanPhamController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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

            var response = await _httpClient.GetFromJsonAsync<List<SanPhamView>>("api/SanPham/with-chi-tiet") ?? new List<SanPhamView>();
            ViewBag.Token = token;
            return View(response);
        }
    }
}
