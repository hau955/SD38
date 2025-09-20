using AppData.Models;

namespace AppApi.IService
{
    public interface IGioHangService 
    {
        Task<GioHang> TaoGioHangNeuChuaCoAsync(Guid idUser);
        Task<string> ThemSanPhamVaoGioAsync(Guid idUser, Guid idSanPhamCT, int soLuong);
        Task<IEnumerable<GioHangCT>> LayDanhSachSanPhamAsync(Guid idUser);
    }
}
