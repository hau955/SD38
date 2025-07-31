using AppApi.ViewModels.SanPham;
using AppView.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static AppView.Controllers.CTSanPhamController;

namespace AppView.Controllers
{
    public class SanPhamChiTietController : Controller
    {
        private readonly HttpClient _httpClient;

        public SanPhamChiTietController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/"); // Thay bằng URL API
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            AppView.ViewModels.SanPham.SanPhamView product = new AppView.ViewModels.SanPham.SanPhamView();
            var userId = HttpContext.Session.GetString("ID");
            ViewBag.UserId = string.IsNullOrEmpty(userId) ? null : userId; // Truyền UserId qua ViewBag

            try
            {
                var response = await _httpClient.GetAsync($"api/SanPham/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response Detail: {content}");
                var jsonResponse = JsonSerializer.Deserialize<JsonResponse<AppApi.ViewModels.SanPham.SanPhamView>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var apiProduct = jsonResponse?.Data;
                if (apiProduct != null)
                {
                    product = new AppView.ViewModels.SanPham.SanPhamView
                    {
                        IDSanPham = apiProduct.IDSanPham,
                        TenSanPham = apiProduct.TenSanPham,
                        MoTa = apiProduct.MoTa,
                        TrongLuong = apiProduct.TrongLuong,
                        GioiTinh = apiProduct.GioiTinh,
                        TrangThai = apiProduct.TrangThai,
                        DanhMucID = apiProduct.DanhMucID,
                        TenDanhMuc = apiProduct.TenDanhMuc,
                        ChiTiets = apiProduct.ChiTiets?.Select(ct => new AppView.ViewModels.SanPham.SanPhamCTViewModel
                        {
                            IDSanPhamCT = ct.IDSanPhamCT,
                            IDSanPham = ct.IDSanPham,
                            TenSanPham = ct.TenSanPham,
                            MoTa = ct.MoTa,
                            TrongLuong = ct.TrongLuong,
                            GioiTinh = ct.GioiTinh,
                            TrangThai = ct.TrangThai,
                            GiaBan = ct.GiaBan,
                            SoLuongTonKho = ct.SoLuongTonKho,
                            IdMauSac = ct.IdMauSac,
                            MauSac = ct.MauSac,
                            IdSize = ct.IdSize,
                            Size = ct.Size,
                            IDChatLieu = ct.IDChatLieu,
                            ChatLieu = ct.ChatLieu
                        }).ToList() ?? new List<AppView.ViewModels.SanPham.SanPhamCTViewModel>(),
                        DanhSachAnh = apiProduct.DanhSachAnh?.Select(a => new AppView.ViewModels.SanPham.AnhSanPhamViewModel
                        {
                            IdAnh = a.IdAnh,
                            IDSanPham = a.IDSanPham,
                            DuongDanAnh = a.DuongDanAnh,
                            AnhChinh = (bool)a.AnhChinh
                        }).ToList() ?? new List<AppView.ViewModels.SanPham.AnhSanPhamViewModel>()
                    };
                }
                if (product == null || product.IDSanPham == Guid.Empty)
                {
                    ViewBag.Error = "Không tìm thấy sản phẩm.";
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message} - Status: {ex.StatusCode}");
                ViewBag.Error = "Không thể tải chi tiết sản phẩm. Vui lòng thử lại sau. (Chi tiết: " + ex.Message + ")";
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                ViewBag.Error = "Lỗi phân tích dữ liệu từ server. Vui lòng thử lại sau. (Chi tiết: " + ex.Message + ")";
            }
            return View(product);
        }
    }
}

