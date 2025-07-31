using AppApi.ViewModels.BanHang;

namespace AppApi.IService
{
    public interface IBanHangService
    {
        Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request);
        Task<(bool IsSuccess, string Message)> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request);
        Task<(bool IsSuccess, string Message)> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request);
        Task<(bool IsSuccess, string Message)> TruSanPhamKhoiHoaDonChoAsync(TruSanPham request);
        Task<(bool IsSuccess, string Message)> HuyHoaDonAsync(Guid idHoaDon);
        Task<(bool IsSuccess, string Message, HoaDonChiTietViewModel? Data)> XemChiTietHoaDonAsync(Guid idHoaDon);
    }
}

