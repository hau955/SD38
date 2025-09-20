using AppData.Models;

namespace AppApi.IService
{
    public interface IGioHangCTService
    {
        Task<string> ThemChiTietAsync(Guid idUser, Guid idSanPhamCT, int soLuong);
        Task<string> CapNhatSoLuongAsync(Guid idGioHangCT, int soLuongMoi);
        Task<string> XoaChiTietAsync(Guid idGioHangCT);
        Task<IEnumerable<GioHangCT>> LayChiTietTheoUserAsync(Guid idUser);
    }
}
