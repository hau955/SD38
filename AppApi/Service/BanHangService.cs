using AppApi.IService;
using AppApi.ViewModels.BanHang;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class BanHangService : IBanHangService
    {
        private readonly ApplicationDbContext _context;

        public BanHangService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = new HoaDon
                {
                    IDHoaDon = Guid.NewGuid(),
                    //IDUser = request.IDUser,                     // Có thể null
                    //IDNguoiTao = request.IDNguoiTao,             // Có thể null
                   // IDHinhThucTT = request.IDHinhThucTT,
                    //IDDiaChiNhanHang = request.IDDiaChiNhanHang,
                    NgayTao = DateTime.Now,
                    TrangThaiDonHang = "Đã bán",
                    TrangThaiThanhToan = "Đã thanh toán",
                    GhiChu = request.GhiChu
                };

                decimal tongTien = 0;

                foreach (var sp in request.DanhSachSanPham)
                {
                    var sanPhamCT = await _context.SanPhamChiTiets
                        .Include(x => x.SanPham)
                        .FirstOrDefaultAsync(x => x.IDSanPhamCT == sp.IDSanPhamCT);

                    if (sanPhamCT == null)
                        return (false, $"Không tìm thấy sản phẩm chi tiết {sp.IDSanPhamCT}", null);

                    if (sanPhamCT.SoLuongTonKho < sp.SoLuong)
                        return (false, $"Sản phẩm \"{sanPhamCT.SanPham?.TenSanPham}\" không đủ tồn kho", null);

                    sanPhamCT.SoLuongTonKho -= sp.SoLuong;

                    var thanhTien = sanPhamCT.GiaBan * sp.SoLuong;
                    tongTien += thanhTien;

                    hoaDon.HoaDonChiTiets.Add(new HoaDonCT
                    {
                        IDHoaDonChiTiet = Guid.NewGuid(),
                        IDHoaDon = hoaDon.IDHoaDon,
                        IDSanPham = sanPhamCT.IDSanPham,
                        TenSanPham = sanPhamCT.SanPham?.TenSanPham ?? "Không rõ",
                        SoLuongSanPham = sp.SoLuong,
                        GiaSanPham = sanPhamCT.GiaBan,
                        GiaSauGiamGia = sanPhamCT.GiaBan,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });
                }

                decimal tienGiam = 0;
            //    if (request.PhanTramGiamHoaDon.HasValue)
              //      tienGiam = tongTien * ((decimal)request.PhanTramGiamHoaDon.Value / 100);

                hoaDon.TongTienTruocGiam = tongTien;
                hoaDon.TienGiam = tienGiam;
                hoaDon.TongTienSauGiam = tongTien - tienGiam;
                //hoaDon.PhanTramGiamGiaHoaDon = request.PhanTramGiamHoaDon;
                hoaDon.TienGiamHoaDon = tienGiam;

                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Bán hàng thành công", hoaDon.IDHoaDon);
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                await transaction.RollbackAsync();
                return (false, $"Lỗi khi lưu dữ liệu: {innerMessage}", null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi không xác định: {ex.Message}", null);
            }

        }
    }
}
