using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Admin.ViewModels;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using System.Net.Http;
using System.Text.Json;
using AppData.Models;
using static System.Net.WebRequestMethods;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ISanPhamRepo _repo;
        private readonly IHttpClientFactory _httpClientFactory;

        public SanPhamController(IHttpClientFactory httpClientFactory, ISanPhamRepo repo, HttpClient httpClient)
        {
            _httpClientFactory = httpClientFactory; _repo = repo; _httpClient = httpClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sanPhams = await _repo.GetAllSanPhamAsync();
            return View(sanPhams);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new SanPhamCreateViewModel();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7221/api/DanhMuc");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<ApiDanhMucResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var danhMucs = apiResult?.Data;

                viewModel.DanhMucList = danhMucs?.Select(dm => new SelectListItem
                {
                    Value = dm.DanhMucId.ToString(),
                    Text = dm.TenDanhMuc
                }).ToList();

            }
            else
            {
                viewModel.DanhMucList = new List<SelectListItem>(); // fallback
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamCreateViewModel model)
        {
            Console.WriteLine(">> DanhMucID = " + model.DanhMucID);
            if (!ModelState.IsValid)
            {
                // Gọi lại danh mục nếu có lỗi form
                model.DanhMucList = await LoadDanhMucList();
                return View(model);
            }
            model.NgayTao = DateTime.Now;
            model.NgaySua = DateTime.Now;
            var result = await _repo.CreateSanPhamAsync(model);
            if (result)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Thêm sản phẩm thất bại");
            model.DanhMucList = await LoadDanhMucList();
            return View(model);
        }

        private async Task<List<SelectListItem>> LoadDanhMucList()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7221/api/DanhMuc");

            if (!response.IsSuccessStatusCode) return new List<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();

            var apiResult = JsonSerializer.Deserialize<ApiDanhMucResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var danhMucs = apiResult?.Data;

            return danhMucs?.Select(dm => new SelectListItem
            {
                Value = dm.DanhMucId.ToString(),
                Text = dm.TenDanhMuc
            }).ToList() ?? new List<SelectListItem>();
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid idSanPham)
        {
            try
            {
                var model = await _repo.GetByIdAsync(idSanPham);
                if (model == null) 
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm với ID: " + idSanPham;
                    return RedirectToAction("Index");
                }
                model.DanhMucList = await LoadDanhMucList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin sản phẩm: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid idSanPham)
        {
            try
            {
                var model = await _repo.GetByIdAsync(idSanPham);
                if (model == null) 
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm với ID: " + idSanPham;
                    return RedirectToAction("Index");
                }
                model.DanhMucList = await LoadDanhMucList();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải thông tin sản phẩm: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(SanPhamCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu lỗi form thì load lại danh mục để giữ dropdown
                model.DanhMucList = await LoadDanhMucList();
                return View(model);
            }

            var success = await _repo.UpdateSanPhamAsync(model);
            if (success)
                return RedirectToAction("Index"); // hoặc Redirect về Danh sách

            TempData["Error"] = "Cập nhật thất bại!";
            model.DanhMucList = await LoadDanhMucList(); // load lại danh sách
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SetAsMainImage(Guid idAnh, Guid idSanPham)
        {
            var response = await _httpClient.PutAsync($"https://localhost:7221/api/SanPham/sanpham/anh/{idAnh}/chinh", null);

            if (!response.IsSuccessStatusCode)
            {
                // Xử lý lỗi nếu cần
                return StatusCode((int)response.StatusCode, "Đặt ảnh chính thất bại");
            }

            return RedirectToAction("Update", new { id = idSanPham });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteImage(Guid idAnh, Guid idSanPham)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7221/api/SanPham/sanpham/anh/{idAnh}");

            if (!response.IsSuccessStatusCode)
            {
                // Xử lý lỗi nếu cần
                return StatusCode((int)response.StatusCode, "Xóa ảnh thất bại");
            }

            return RedirectToAction("Update", new { id = idSanPham });
        }

    }
}
