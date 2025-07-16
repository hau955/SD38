using AppApi.ViewModels.BanHang;

namespace AppApi.IService
{
    public interface IBanHangService
    {
        Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request);
    }
}

