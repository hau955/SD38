using AppData.Models;

namespace AppApi.IService
{
    public interface IGioHangService 
    {
        Task<object> ThemVaoGioHang(Guid idSanPhamCT, Guid idNguoiDung, int soLuong);
        Task<object> XoaKhoiGioHang(Guid idGioHangChiTiet);
        Task<object> CapNhatSoLuong(Guid idGioHangChiTiet, int soLuong);
        Task<List<GioHangCT>> LayDanhSachGioHang(Guid idNguoiDung);
        Task<GioHang?> GetByUserIdAsync(Guid userId);
    }
}
