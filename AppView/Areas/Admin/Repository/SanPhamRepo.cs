using AppView.Areas.Admin.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class SanPhamRepo : ISanPhamRepo
    {
        private readonly HttpClient _httpClient;
        public SanPhamRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<SanPham> Create(SanPhamCTViewModel model)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.TenSanPham ?? ""), "TenSanPham");
            content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
            content.Add(new StringContent(model.TrongLuong.ToString()), "TrongLuong");
            content.Add(new StringContent(model.GioiTinh.ToString()), "GioiTinh");

            content.Add(new StringContent(model.IdSize.ToString()), "IDSize");
            content.Add(new StringContent(model.IdMauSac.ToString()), "IDMauSac");
            content.Add(new StringContent(model.IdCoAo.ToString()), "IDCoAo");
            content.Add(new StringContent(model.IdTaAo.ToString()), "IDTaAo");

            content.Add(new StringContent(model.GiaBan.ToString()), "GiaBan");
            content.Add(new StringContent(model.SoLuongTonKho.ToString()), "SoLuongTonKho");

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(model.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageFile.ContentType);
                content.Add(streamContent, "ImageFile", model.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/SanPham", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception("Tạo sản phẩm thất bại: " + error);
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<SanPham>>();

            return result?.Data ?? throw new Exception("Không nhận được dữ liệu từ API.");
        }
            public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/SanPham/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SanPham>> GetAll()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<SanPham>>>("api/SanPham");
            return response?.Data ?? new List<SanPham>();
        }

        public async Task<SanPham?> GetByID(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<SanPham?>>($"api/SanPham/{id}");
            return response?.Data;
        }
        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"api/SanPham/ToggleStatus/{id}", null);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Deserialize kết quả thành Dictionary để lấy giá trị "message"
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                return result != null && result.ContainsKey("message")
                    ? result["message"]
                    : "Cập nhật thành công.";
            }
            else
            {
                var error = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
                throw new Exception(error != null && error.ContainsKey("message")
                    ? error["message"]
                    : "Lỗi không xác định.");
            }
        }



        public async Task<SanPham?> Update(SanPhamCTViewModel sanpham)
        {
            using var form = new MultipartFormDataContent();

            // Thêm các trường thông thường
            form.Add(new StringContent(sanpham.TenSanPham ?? ""), "TenSanPham");
            form.Add(new StringContent(sanpham.GioiTinh?.ToString() ?? ""), "GioiTinh");
            form.Add(new StringContent(sanpham.TrongLuong.ToString()), "TrongLuong");
            form.Add(new StringContent(sanpham.MoTa ?? ""), "MoTa");
            form.Add(new StringContent(sanpham.TrangThai.ToString()), "TrangThai");

            // ✅ BỔ SUNG đường dẫn ảnh nếu có
            form.Add(new StringContent(sanpham.HinhAnh ?? ""), "HinhAnh");

            // Nếu có ảnh được chọn thì thêm vào (ưu tiên file ảnh mới nếu có)
            if (sanpham.ImageFile != null && sanpham.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(sanpham.ImageFile.OpenReadStream());
                form.Add(streamContent, "ImageFile", sanpham.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"api/SanPham/{sanpham.IDSanPham}", form);

            if (!response.IsSuccessStatusCode) return null;

            var updatedResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SanPham?>>();
            return updatedResponse?.Data;
        }



    private class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
    }

    private class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
}
