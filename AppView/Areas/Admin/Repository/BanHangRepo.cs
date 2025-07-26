
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.Common;

using AppView.Areas.Admin.ViewModels.BanHangViewModels;


namespace AppView.Areas.Admin.Repository
{
    public class BanHangRepo : IBanHangfRepo
    {
        private readonly HttpClient _httpClient;
        private readonly string BASE_URL = "https://localhost:7221/api/BanHang";

        public BanHangRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResult<Guid?>> BanTaiQuayAsync(BanHangViewModel request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/ban-tai-quay", request);
            return await response.Content.ReadFromJsonAsync<ApiResult<Guid?>>()
                   ?? new ApiResult<Guid?> { IsSuccess = false, Message = "Lỗi không xác định" };
        }

        public async Task<ApiResult<bool>> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/thanh-toan-hoa-don-cho", request);
            return await response.Content.ReadFromJsonAsync<ApiResult<bool>>()
                   ?? new ApiResult<bool> { IsSuccess = false, Message = "Lỗi không xác định", Data = false };
        }

        public async Task<ApiResult<bool>> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/them-san-pham-vao-hoa-don-cho", request);
            return await response.Content.ReadFromJsonAsync<ApiResult<bool>>()
                   ?? new ApiResult<bool> { IsSuccess = false, Message = "Lỗi không xác định", Data = false };
        }

        public async Task<ApiResult<bool>> TruSanPhamKhoiHoaDonChoAsync(TruSanPham request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/tru-san-pham-hoa-don-cho", request);
            return await response.Content.ReadFromJsonAsync<ApiResult<bool>>()
                   ?? new ApiResult<bool> { IsSuccess = false, Message = "Lỗi không xác định", Data = false };
        }

        public async Task<ApiResult<bool>> HuyHoaDonAsync(Guid idHoaDon)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BASE_URL}/huy-hoa-don", new { IDHoaDon = idHoaDon });
            return await response.Content.ReadFromJsonAsync<ApiResult<bool>>()
                   ?? new ApiResult<bool> { IsSuccess = false, Message = "Lỗi không xác định", Data = false };
        }

        public async Task<List<HoaDonResponseViewModel>> GetHoaDonChoAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<HoaDonResponseViewModel>>(
                $"{BASE_URL}/hoa-don-cho") ?? new List<HoaDonResponseViewModel>();
        }

        public async Task<(bool IsSuccess, string Message, HoaDonChiTietViewModel? Data)> XemChiTietHoaDonAsync(Guid idHoaDon)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}/xem-hoa-don/{idHoaDon}");

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return (false, $"Không lấy được dữ liệu: {message}", null);
                }

                var data = await response.Content.ReadFromJsonAsync<HoaDonChiTietViewModel>();

                if (data == null)
                    return (false, "Dữ liệu trả về bị null", null);

                return (true, "Lấy dữ liệu thành công", data);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống: {ex.Message}", null);
            }
        }

    }
}