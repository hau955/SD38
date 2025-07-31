using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppApi.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly ApplicationDbContext _context;
        public GioHangService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<GioHang?> GetByUserIdAsync(Guid userId)
        {
            return await _context.GioHangs
         .Include(g => g.GioHangChiTiets)
         .FirstOrDefaultAsync(g => g.User.Id == userId);
        }
        
        public async Task<object> CapNhatSoLuong(Guid idGioHangChiTiet, int soLuong)
        {
            try
            {
                var chiTiet = await _context.GioHangChiTiets
                    .Include(ct => ct.SanPhamCT)
                    .FirstOrDefaultAsync(ct => ct.IDGioHangChiTiet == idGioHangChiTiet);
                if (chiTiet == null)
                {
                    return new { success = false, message = "Chi tiết giỏ hàng không tồn tại" };
                }

                if (chiTiet.SanPhamCT == null || chiTiet.SanPhamCT.SoLuongTonKho < soLuong)
                {
                    return new { success = false, message = "Số lượng vượt quá tồn kho" };
                }

                chiTiet.SoLuong = soLuong;
                _context.GioHangChiTiets.Update(chiTiet);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Updated SoLuong for IDGioHangChiTiet {idGioHangChiTiet} to {soLuong}");
                return new { success = true, soLuong = chiTiet.SoLuong, message = "Cập nhật số lượng thành công" };
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException in CapNhatSoLuong: {ex.InnerException?.Message}");
                return new { success = false, message = "Lỗi cập nhật cơ sở dữ liệu.", detail = ex.InnerException?.Message };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CapNhatSoLuong: {ex.Message}");
                return new { success = false, message = "Lỗi server. Vui lòng thử lại.", detail = ex.Message };
            }
        }

        public async Task<List<GioHangCT>> LayDanhSachGioHang(Guid idUser)
        {
            return await _context.GioHangChiTiets
        .Where(ct => ct.IDGioHang == idUser && ct.TrangThai) 
        .Include(ct => ct.SanPhamCT)
        .ThenInclude(spct => spct.SanPham)
        .AsNoTracking()
        .ToListAsync();
        }

        public async Task<object> ThemVaoGioHang(Guid idSanPhamCT, Guid idUser, int soLuong)
        {
            // Tìm giỏ hàng dựa trên IdNguoiDung (User.Id)
            var gioHang = await _context.GioHangs
                .Include(g => g.GioHangChiTiets)
                .FirstOrDefaultAsync(g => g.User.Id == idUser);

            if (gioHang == null)
            {
                // Kiểm tra xem ApplicationUser có tồn tại không
                var user = await _context.Users.FindAsync(idUser);
                if (user == null)
                {
                    throw new InvalidOperationException("Người dùng không tồn tại.");
                }

                gioHang = new GioHang
                {
                    IDGioHang = Guid.NewGuid(), // Tạo ID mới cho giỏ hàng
                    User = user,               // Liên kết với ApplicationUser
                    TrangThai = true
                };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync(); // Lưu để lấy IDGioHang
            }

            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(ct => ct.IDGioHang == gioHang.IDGioHang && ct.IDSanPhamCT == idSanPhamCT);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
                _context.GioHangChiTiets.Update(chiTiet);
            }
            else
            {
                // Kiểm tra tồn kho
                var sanPhamCT = await _context.SanPhamChiTiets
                    .FirstOrDefaultAsync(sp => sp.IDSanPhamCT == idSanPhamCT);
                if (sanPhamCT == null || sanPhamCT.SoLuongTonKho < soLuong)
                {
                    throw new InvalidOperationException("Sản phẩm không đủ tồn kho hoặc không tồn tại.");
                }

                chiTiet = new GioHangCT
                {
                    IDGioHangChiTiet = Guid.NewGuid(),
                    IDGioHang = gioHang.IDGioHang,
                    IDSanPhamCT = idSanPhamCT,
                    SoLuong = soLuong,
                    DonGia = await GetGiaBanSanPham(idSanPhamCT),
                    TrangThai = true
                };
                _context.GioHangChiTiets.Add(chiTiet);
            }

            await _context.SaveChangesAsync();
            return new { success = true, soLuong = chiTiet.SoLuong, idGioHangChiTiet = chiTiet.IDGioHangChiTiet };
        }

        public async Task<object> XoaKhoiGioHang(Guid idGioHangChiTiet)
        {
            try
            {
                var chiTiet = await _context.GioHangChiTiets.FindAsync(idGioHangChiTiet);
                if (chiTiet == null)
                {
                    return new { success = false, message = "Chi tiết giỏ hàng không tồn tại" };
                }

                _context.GioHangChiTiets.Remove(chiTiet);
                await _context.SaveChangesAsync();
                return new { success = true, message = "Xóa sản phẩm thành công" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in XoaKhoiGioHang: {ex.Message}");
                return new { success = false, message = "Lỗi server. Vui lòng thử lại.", detail = ex.Message };
            }
        }
        private async Task<decimal> GetGiaBanSanPham(Guid idSanPhamCT)
        {
            var spct = await _context.SanPhamChiTiets.FindAsync(idSanPhamCT);
            return spct?.GiaBan ?? 0;
        }
    }
}
