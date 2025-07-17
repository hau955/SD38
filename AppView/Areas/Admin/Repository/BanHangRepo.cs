
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.BanHangViewModels;

namespace AppView.Areas.Admin.Repository
{
    public class BanHangRepo : IBanHangfRepo
    {
        private readonly HttpClient _httpClient;

        public BanHangRepo(HttpClient factory)
        {
            _httpClient = factory;
        }

        public async Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request)
        {
            var res = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/BanHang/ban-tai-quay", request);
            var body = await res.Content.ReadFromJsonAsync<dynamic>();
            return res.IsSuccessStatusCode
                ? (true, (string)body.message, Guid.Parse((string)body.hoaDonId))
                : (false, (string)body.message, null);
        }

        public async Task<(bool IsSuccess, string Message)> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request)
        {
            var res = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/BanHang/thanh-toan-hoa-don-cho", request);
            var body = await res.Content.ReadFromJsonAsync<dynamic>();
            return (res.IsSuccessStatusCode, (string)body.message);
        }

        public async Task<List<HoaDonChoVM>> GetHoaDonChoAsync()
        {
            var res = await _httpClient.GetAsync("https://localhost:7221/api/BanHang/hoa-don-cho");
            if (!res.IsSuccessStatusCode) return new();
            return await res.Content.ReadFromJsonAsync<List<HoaDonChoVM>>();
        }

        public async Task<(bool IsSuccess, string Message)> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request)
        {
            var res = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/BanHang/them-san-pham-vao-hoa-don-cho", request);
            var body = await res.Content.ReadFromJsonAsync<dynamic>();
            return (res.IsSuccessStatusCode, (string)body.message);
        }

        public async Task<(bool IsSuccess, string Message)> TruSanPhamHoaDonChoAsync(TruSanPham request)
        {
            var res = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/BanHang/tru-san-pham-hoa-don-cho", request);
            var body = await res.Content.ReadFromJsonAsync<dynamic>();
            return (res.IsSuccessStatusCode, (string)body.message);
        }
    }
}
