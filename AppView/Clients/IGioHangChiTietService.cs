using AppData.Models;

namespace AppView.Clients
{
    public interface IGioHangChiTietService
    {
        Task<List<GioHangCT>> GetByUserAsync(Guid userId);
        Task<bool> UpdateQtyAsync(Guid idGioHangCT, int soLuongMoi);
        Task<bool> DeleteAsync(Guid idGioHangCT);
    }
}
