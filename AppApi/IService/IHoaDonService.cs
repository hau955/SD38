using AppApi.ViewModels.HoaDonDTOs;
using AppData.Models;

namespace AppApi.IService
{
    public interface IHoaDonService
    {
        Task<HoaDon> TaoHoaDonTuGioHangAsync(
            Guid idUser,
            Guid idHinhThucTT,
            Guid? idVoucher = null,
             Guid? idDiaChi = null,
            string? ghiChu = null);
        Task CapNhatTrangThaiAsync(Guid idHoaDon, string trangThaiMoi, string? nguoiCapNhat = null);
        Task ChuyenSangDaXacNhan(Guid idHoaDon, string? nguoiCapNhat = null);
        Task ChuyenSangDangGiao(Guid idHoaDon, string? nguoiCapNhat = null);
         Task ChuyenSangDaGiao(Guid idHoaDon, string? nguoiCapNhat = null);
        Task ChuyenSangHuy(Guid idHoaDon, string? nguoiCapNhat = null);
        Task KhachHangHuyHoaDonAsync(Guid idHoaDon, string tenKhachHang);
        Task<List<HoaDonTrangThai>> LayLichSuTrangThaiAsync(Guid idHoaDon);
        Task<List<HoaDon>> LayHoaDonTheoUserAsync(Guid idUser);
        Task<HoaDonView?> XemChiTietHoaDonAsync(Guid idHoaDon);
        Task<List<DiaChiNhanHang>> GetDiaChiByUserAsync(Guid idUser);
        Task<List<Voucher>> LayTatCaVoucherAsync();
        Task<List<HinhThucTT>> GetAllAsync();
    }
}
