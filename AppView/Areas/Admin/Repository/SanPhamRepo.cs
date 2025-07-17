using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using AppData.Models;

namespace AppView.Areas.Admin.Repository
{
    public class SanPhamRepo : ISanPhamRepo
    {
        private readonly HttpClient _httpClient;

        public SanPhamRepo(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7221/");
        }

        public async Task<bool> CreateSanPhamAsync(SanPhamCreateViewModel model)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.TenSanPham), "TenSanPham");
            content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
            content.Add(new StringContent(model.TrongLuong.ToString()), "TrongLuong");
            content.Add(new StringContent(model.GioiTinh.ToString().ToLower()), "GioiTinh");
            content.Add(new StringContent(model.TrangThai.ToString().ToLower()), "TrangThai");
            content.Add(new StringContent(model.DanhMucID.ToString()), "DanhMucID");

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileStream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType);
                content.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/SanPham/create", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API lỗi: {error}");
            }

            return true;
        }

        public async Task<List<SanPhamView>> GetAllSanPhamAsync()
        {
            var response = await _httpClient.GetAsync("api/SanPham");
            if (!response.IsSuccessStatusCode)
                return new List<SanPhamView>();

            var content = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResult<List<SanPhamView>>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResult?.Data ?? new List<SanPhamView>();
        }

        public async Task<SanPhamCreateViewModel?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/SanPham/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API lỗi: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResult<SanPham>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var sanPham = apiResult?.Data;
            if (sanPham == null) return null;

            return new SanPhamCreateViewModel
            {
                IDSanPham = sanPham.IDSanPham,
                TenSanPham = sanPham.TenSanPham,
                MoTa = sanPham.MoTa,
                TrongLuong = sanPham.TrongLuong ?? 0,
                GioiTinh = sanPham.GioiTinh ?? false,
                TrangThai = sanPham.TrangThai,
                DanhMucID = sanPham.DanhMucId,
                //HinhAnh = sanPham.HinhAnh // để hiển thị ảnh nếu cần
            };
        }

        public async Task<bool> UpdateSanPhamAsync(SanPhamCreateViewModel model)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.IDSanPham.ToString()), "IDSanPham");
            content.Add(new StringContent(model.TenSanPham), "TenSanPham");
            content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
            content.Add(new StringContent(model.TrongLuong.ToString()), "TrongLuong");
            content.Add(new StringContent(model.GioiTinh.ToString().ToLower()), "GioiTinh");
            content.Add(new StringContent(model.TrangThai.ToString().ToLower()), "TrangThai");
            content.Add(new StringContent(model.DanhMucID.ToString()), "DanhMucID");

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileStream = model.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType);
                content.Add(fileContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync("api/SanPham/update", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API lỗi: {error}");
            }

            return true;
        }

        public class ApiResult<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
