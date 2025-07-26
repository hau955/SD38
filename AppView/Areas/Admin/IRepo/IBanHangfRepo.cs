
using AppView.Areas.Admin.ViewModels.BanHangViewModels;
using AppView.Areas.Admin.Common;

namespace AppView.Areas.Admin.IRepo
{
    public interface IBanHangfRepo
    {
        Task<ApiResult<Guid?>> BanTaiQuayAsync(BanHangViewModel request);
        Task<ApiResult<bool>> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request);
        Task<ApiResult<bool>> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request);
        Task<ApiResult<bool>> TruSanPhamKhoiHoaDonChoAsync(TruSanPham request);
        Task<ApiResult<bool>> HuyHoaDonAsync(Guid idHoaDon);
        Task<List<HoaDonResponseViewModel>> GetHoaDonChoAsync();
        Task<(bool IsSuccess, string Message, HoaDonChiTietViewModel? Data)> XemChiTietHoaDonAsync(Guid idHoaDon);
    }
   
}
