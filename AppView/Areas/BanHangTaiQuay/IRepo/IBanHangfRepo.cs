using AppView.Areas.Admin.Common;
using AppView.Areas.BanHangTaiQuay.ViewModels.BanHangViewModels;

namespace AppView.Areas.BanHangTaiQuay.IRepo
{
    public interface IBanHangfRepo
    {
        Task<ApiResult<Guid?>> BanTaiQuayAsync(BanHangViewModel request);
        Task<ApiResult<bool>> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request);
        Task<ApiResult<bool>> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request);
        Task<ApiResult<bool>> TruSanPhamKhoiHoaDonChoAsync(TruSanPham request);
        Task<ApiResult<bool>> HuyHoaDonAsync(Guid idHoaDon);
        Task<List<HoaDonResponseViewModel>> GetHoaDonChoAsync(Guid idnguoitao );
        Task<(bool IsSuccess, string Message, HoaDonChiTietViewModel? Data)> XemChiTietHoaDonAsync(Guid idHoaDon);
    }
   
}
