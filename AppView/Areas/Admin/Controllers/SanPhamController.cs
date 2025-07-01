using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Repository;
using AppView.Areas.Admin.ViewModels;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using System.Net.Http;
using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamRepo _repo;
        private readonly IHttpClientFactory _httpClientFactory;

        public SanPhamController(IHttpClientFactory httpClientFactory, ISanPhamRepo repo)
        {
            _httpClientFactory = httpClientFactory; _repo = repo;
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
            var response = await client.GetAsync("https://localhost:7221/api/DanhMucs");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var danhMucs = JsonSerializer.Deserialize<List<DanhMucResponse>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

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
            var response = await client.GetAsync("https://localhost:7221/api/DanhMucs");

            if (!response.IsSuccessStatusCode) return new List<SelectListItem>();

            var json = await response.Content.ReadAsStringAsync();
            var danhMucs = JsonSerializer.Deserialize<List<DanhMucResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return danhMucs?.Select(dm => new SelectListItem
            {
                Value = dm.DanhMucId.ToString(),
                Text = dm.TenDanhMuc
            }).ToList() ?? new List<SelectListItem>();
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            model.DanhMucList = await LoadDanhMucList();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _repo.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            model.DanhMucList = await LoadDanhMucList();
            return View(model);
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

    }
}
