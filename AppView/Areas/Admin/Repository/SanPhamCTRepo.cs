using System.Net.Http.Json;
using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class SanPhamCTRepo : ISanPhamCTRepo
    {
        private readonly HttpClient _httpClient;

        public SanPhamCTRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SanPhamCT>> GetAll()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<SanPhamCT>>>("api/SanPhamCT");
            return response?.Data ?? new List<SanPhamCT>();
        }

        public async Task<SanPhamCT?> GetByID(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<SanPhamCT>>($"api/SanPhamCT/{id}");
            return response?.Data;
        }

        public async Task<SanPhamCT> Create(SanPhamCT sanPhamCT)
        {
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(sanPhamCT.IDSanPham.ToString()), "IdSanPham");
            form.Add(new StringContent(sanPhamCT.IDMauSac.ToString()), "IdMauSac");
            form.Add(new StringContent(sanPhamCT.IDSize.ToString()), "IdSize");
            form.Add(new StringContent(sanPhamCT.IDCoAo.ToString()), "IdCoAo");
            form.Add(new StringContent(sanPhamCT.IDTaAo.ToString()), "IdTaAo");
            form.Add(new StringContent(sanPhamCT.SoLuongTonKho.ToString()), "SoLuongTonKho");
            form.Add(new StringContent(sanPhamCT.GiaBan.ToString()), "GiaBan");
            form.Add(new StringContent(sanPhamCT.TrangThai.ToString()), "TrangThai");

            if (sanPhamCT.ImageFile != null && sanPhamCT.ImageFile.Length > 0)
            {
                var stream = new StreamContent(sanPhamCT.ImageFile.OpenReadStream());
                form.Add(stream, "ImageFile", sanPhamCT.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/SanPhamCT", form);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<SanPhamCT>>();
            return result!.Data;
        }

        public async Task<SanPhamCT?> Update(Guid id, SanPhamCT sanPhamCT)
        {
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(sanPhamCT.IDSanPham.ToString()), "IdSanPham");
            form.Add(new StringContent(sanPhamCT.IDMauSac.ToString()), "IdMauSac");
            form.Add(new StringContent(sanPhamCT.IDSize.ToString()), "IdSize");
            form.Add(new StringContent(sanPhamCT.IDCoAo.ToString()), "IdCoAo");
            form.Add(new StringContent(sanPhamCT.IDTaAo.ToString()), "IdTaAo");
            form.Add(new StringContent(sanPhamCT.SoLuongTonKho.ToString()), "SoLuongTonKho");
            form.Add(new StringContent(sanPhamCT.GiaBan.ToString()), "GiaBan");
            form.Add(new StringContent(sanPhamCT.TrangThai.ToString()), "TrangThai");
            form.Add(new StringContent(sanPhamCT.HinhAnh ?? ""), "HinhAnh");

            if (sanPhamCT.ImageFile != null && sanPhamCT.ImageFile.Length > 0)
            {
                var stream = new StreamContent(sanPhamCT.ImageFile.OpenReadStream());
                form.Add(stream, "ImageFile", sanPhamCT.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"api/SanPhamCT/{id}", form);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<SanPhamCT>>();
            return result?.Data;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/SanPhamCT/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"api/SanPhamCT/ToggleStatus/{id}", null);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            return json != null && json.ContainsKey("message") ? json["message"] : "Thành công";
        }

        private class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
