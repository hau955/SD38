using System.Net.Http;
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
        public async Task<SanPham> Create(SanPham sanpham)
        {
            using var content = new MultipartFormDataContent();

            // Các field dạng chuỗi/số
            content.Add(new StringContent(sanpham.TenSanPham ?? ""), "TenSanPham");
            content.Add(new StringContent(sanpham.MoTa ?? ""), "MoTa");
            content.Add(new StringContent(sanpham.TrongLuong.ToString()), "TrongLuong");
            content.Add(new StringContent(sanpham.GioiTinh.ToString()), "GioiTinh");

            // Gửi file nếu có
            if (sanpham.ImageFile != null && sanpham.ImageFile.Length > 0)
            {
                var fileStream = sanpham.ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(sanpham.ImageFile.ContentType);
                content.Add(fileContent, "ImageFile", sanpham.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/SanPham", content);
            response.EnsureSuccessStatusCode();

            var createdResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SanPham>>();
            if (createdResponse == null)
                throw new Exception("Không nhận được dữ liệu tạo sản phẩm.");

            return createdResponse.Data;
        }


        public async Task<bool> Detele(Guid id)
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



        public async Task<SanPham?> Update(Guid id, SanPham sanpham)
        {
            using var form = new MultipartFormDataContent();

            // Thêm các trường thông thường
            form.Add(new StringContent(sanpham.TenSanPham ?? ""), "TenSanPham");
            form.Add(new StringContent(sanpham.GioiTinh?.ToString() ?? ""), "GioiTinh");
            form.Add(new StringContent(sanpham.TrongLuong.ToString()), "TrongLuong");
            form.Add(new StringContent(sanpham.MoTa ?? ""), "MoTa");
            form.Add(new StringContent(sanpham.TrangThai.ToString()), "TrangThai");

            // Nếu có ảnh được chọn thì thêm vào
            if (sanpham.ImageFile != null && sanpham.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(sanpham.ImageFile.OpenReadStream());
                form.Add(streamContent, "ImageFile", sanpham.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"api/SanPham/{id}", form);

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
