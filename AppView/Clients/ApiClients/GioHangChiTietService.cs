using AppApi.IService;
using AppData.Models;
using System.Text.Json;

namespace AppView.Clients.ApiClients
{
    public class GioHangChiTietService : IGioHangChiTietService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public GioHangChiTietService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<GioHangCT>> GetByUserAsync(Guid userId)
        {
            var res = await _http.GetAsync($"https://localhost:7221/api/GioHangCT/lay-theo-user/{userId}");
            if (!res.IsSuccessStatusCode) return new List<GioHangCT>();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GioHangCT>>(json, _options) ?? new List<GioHangCT>();
        }

        public async Task<bool> UpdateQtyAsync(Guid idGioHangCT, int soLuongMoi)
        {
            var body = new { IdGioHangCT = idGioHangCT, SoLuongMoi = soLuongMoi };
            var res = await _http.PutAsJsonAsync("https://localhost:7221/api/GioHangCT/cap-nhat", body);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid idGioHangCT)
        {
            var res = await _http.DeleteAsync($"https://localhost:7221/api/GioHangCT/xoa/{idGioHangCT}");
            return res.IsSuccessStatusCode;
        }
    }
}
