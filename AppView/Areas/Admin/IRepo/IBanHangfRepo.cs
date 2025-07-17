
using AppView.Areas.Admin.ViewModels.BanHangViewModels;

namespace AppView.Areas.Admin.IRepo
{
    public interface IBanHangfRepo
    {
        Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request);
        Task<(bool IsSuccess, string Message)> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request);
        Task<List<HoaDonChoVM>> GetHoaDonChoAsync();
        Task<(bool IsSuccess, string Message)> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request);
        Task<(bool IsSuccess, string Message)> TruSanPhamHoaDonChoAsync(TruSanPham request);
    }
}
