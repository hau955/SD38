using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
            _httpClient.BaseAddress = new Uri("https://localhost:7221/api/");
        }

        // Hiển thị danh sách sản phẩm với tìm kiếm và lọc
        public async Task<IActionResult> Index(
                string? ten,
                Guid? danhMucId,
                decimal? giaMin,
                decimal? giaMax,
                string? mauSac,
                string? size,
                string? chatLieu,
                string? sapXep = "ten_az",
                int page = 1,
                int pageSize = 12)
        {
            try
            {
                // Load dữ liệu filter TRƯỚC khi xử lý tìm kiếm
                await LoadFilterDataAsync();

                // Tạo query string cho API
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(ten))
                    queryParams.Add($"ten={Uri.EscapeDataString(ten)}");

                if (danhMucId.HasValue)
                    queryParams.Add($"danhMucId={danhMucId}");

                if (giaMin.HasValue)
                    queryParams.Add($"giaMin={giaMin}");

                if (giaMax.HasValue)
                    queryParams.Add($"giaMax={giaMax}");

                if (!string.IsNullOrEmpty(mauSac))
                    queryParams.Add($"mauSac={Uri.EscapeDataString(mauSac)}");

                if (!string.IsNullOrEmpty(size))
                    queryParams.Add($"size={Uri.EscapeDataString(size)}");

                if (!string.IsNullOrEmpty(chatLieu))
                    queryParams.Add($"chatLieu={Uri.EscapeDataString(chatLieu)}");

                if (!string.IsNullOrEmpty(sapXep))
                    queryParams.Add($"sapXep={sapXep}");

                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var queryString = string.Join("&", queryParams);
                var apiUrl = $"SanPham/search?{queryString}";

                // Gọi API tìm kiếm
                var response = await _httpClient.GetAsync(apiUrl);

                SanPhamSearchResponse searchResult;
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<dynamic>(json);
                    searchResult = JsonConvert.DeserializeObject<SanPhamSearchResponse>(apiResult?.data?.ToString() ?? "{}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Search API Error: {response.StatusCode} - {errorContent}");
                    searchResult = new SanPhamSearchResponse { Data = new List<SanPhamDetailWithDiscountView>() };
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm sản phẩm.";
                }

                // Truyền dữ liệu tìm kiếm vào ViewBag
                ViewBag.Ten = ten;
                ViewBag.DanhMucId = danhMucId;
                ViewBag.GiaMin = giaMin;
                ViewBag.GiaMax = giaMax;
                ViewBag.MauSac = mauSac;
                ViewBag.Size = size;
                ViewBag.ChatLieu = chatLieu;
                ViewBag.SapXep = sapXep;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchResult = searchResult;

                return View(searchResult?.Data ?? new List<SanPhamDetailWithDiscountView>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Index: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                await LoadFilterDataAsync(); // Vẫn cần load data cho filter
                return View(new List<SanPhamDetailWithDiscountView>());
            }

        }
        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SanPham/by-id/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<dynamic>(json);
                var data = JsonConvert.DeserializeObject<SanPhamDetailWithDiscountView>(apiResult?.data?.ToString() ?? "{}");

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // API cho AJAX - Lấy gợi ý tìm kiếm
        [HttpGet]
        public async Task<JsonResult> GetSearchSuggestions(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query) || query.Length < 2)
                {
                    return Json(new List<string>());
                }

                var response = await _httpClient.GetAsync($"SanPham/suggestions?query={Uri.EscapeDataString(query)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(json);
                    return Json(result?.data ?? new List<string>());
                }

                return Json(new List<string>());
            }
            catch
            {
                return Json(new List<string>());
            }
        }

        // Private method để load dữ liệu cho filter dropdown
        private async Task LoadFilterDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("SanPham/filters/data");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<dynamic>(json);

                    if (apiResult?.data != null)
                    {
                        ViewBag.DanhMucs = JsonConvert.DeserializeObject<List<dynamic>>(apiResult.data.DanhMucs?.ToString() ?? "[]");
                        ViewBag.MauSacs = JsonConvert.DeserializeObject<List<dynamic>>(apiResult.data.MauSacs?.ToString() ?? "[]");
                        ViewBag.Sizes = JsonConvert.DeserializeObject<List<dynamic>>(apiResult.data.Sizes?.ToString() ?? "[]");
                        ViewBag.ChatLieus = JsonConvert.DeserializeObject<List<dynamic>>(apiResult.data.ChatLieus?.ToString() ?? "[]");
                        ViewBag.GiaRange = apiResult.data.GiaRange;
                    }
                    else
                    {
                        SetDefaultFilterData();
                    }
                }
                else
                {
                    // Log lỗi để debug
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    SetDefaultFilterData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LoadFilterDataAsync: {ex.Message}");
                SetDefaultFilterData();
            }
        }

        private void SetDefaultFilterData()
        {
            ViewBag.DanhMucs = new List<dynamic>();
            ViewBag.MauSacs = new List<dynamic>();
            ViewBag.Sizes = new List<dynamic>();
            ViewBag.ChatLieus = new List<dynamic>();
            ViewBag.GiaRange = new { GiaMin = 0m, GiaMax = 1000000m };
        }

    }
}
