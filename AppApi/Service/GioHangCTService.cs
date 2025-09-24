using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class GioHangCTService : IGioHangCTService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGioHangService _gioHangService;

        public GioHangCTService(ApplicationDbContext context, IGioHangService gioHangService)
        {
            _context = context;
            _gioHangService = gioHangService;
        }

        // ✅ Thêm chi tiết giỏ hàng (giới hạn 10 loại, 10 cái/loại)
        public async Task<string> ThemChiTietAsync(Guid idUser, Guid idSanPhamCT, int soLuong)
        {
            if (soLuong <= 0) return "❌ Số lượng không hợp lệ";

            var gioHang = await _gioHangService.TaoGioHangNeuChuaCoAsync(idUser);

            var spct = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.IDSanPhamCT == idSanPhamCT);

            if (spct == null) return "❌ Sản phẩm không tồn tại";

            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(x => x.IDGioHang == gioHang.IDGioHang && x.IDSanPhamCT == idSanPhamCT);

            // 🔎 Giới hạn số loại sản phẩm trong giỏ
            var soLoaiSPTrongGio = await _context.GioHangChiTiets
                .CountAsync(x => x.IDGioHang == gioHang.IDGioHang);
            if (soLoaiSPTrongGio >= 10 && chiTiet == null)
                return "❌ Giỏ hàng chỉ được chứa tối đa 10 loại sản phẩm.";

            if (chiTiet != null)
            {
                // 🔎 Giới hạn 10 cái mỗi sản phẩm
                if (chiTiet.SoLuong + soLuong > 10)
                    return "❌ Mỗi sản phẩm chỉ được mua tối đa 10 cái.";

                chiTiet.SoLuong += soLuong;
            }
            else
            {
                if (soLuong > 10)
                    return "❌ Mỗi sản phẩm chỉ được mua tối đa 10 cái.";

                chiTiet = new GioHangCT
                {
                    IDGioHangChiTiet = Guid.NewGuid(),
                    IDGioHang = gioHang.IDGioHang,
                    IDSanPhamCT = idSanPhamCT,
                    SoLuong = soLuong,
                    DonGia = spct.GiaBan,
                    TrangThai = true
                };
                _context.GioHangChiTiets.Add(chiTiet);
            }

            await _context.SaveChangesAsync();
            return "✅ Đã thêm chi tiết giỏ hàng";
        }

        // ✅ Cập nhật số lượng (giới hạn 10 cái)
        public async Task<string> CapNhatSoLuongAsync(Guid idGioHangCT, int soLuongMoi)
        {
            var chiTiet = await _context.GioHangChiTiets.FindAsync(idGioHangCT);
            if (chiTiet == null) return "❌ Không tìm thấy chi tiết giỏ hàng";

            if (soLuongMoi <= 0) return "❌ Số lượng không hợp lệ";
            if (soLuongMoi > 10) return "❌ Mỗi sản phẩm chỉ được mua tối đa 10 cái.";

            chiTiet.SoLuong = soLuongMoi;
            await _context.SaveChangesAsync();

            return "✅ Đã cập nhật số lượng";
        }

        // ✅ Xóa chi tiết
        public async Task<string> XoaChiTietAsync(Guid idGioHangCT)
        {
            var chiTiet = await _context.GioHangChiTiets.FindAsync(idGioHangCT);
            if (chiTiet == null) return "❌ Không tìm thấy chi tiết giỏ hàng";

            _context.GioHangChiTiets.Remove(chiTiet);
            await _context.SaveChangesAsync();

            return "✅ Đã xóa chi tiết giỏ hàng";
        }

        // ✅ Lấy chi tiết theo User
        public async Task<IEnumerable<GioHangCT>> LayChiTietTheoUserAsync(Guid idUser)
        {
            var gioHang = await _gioHangService.TaoGioHangNeuChuaCoAsync(idUser);

            var list = await _context.GioHangChiTiets
                .Where(x => x.IDGioHang == gioHang.IDGioHang)
                .Include(x => x.SanPhamCT)
                    .ThenInclude(spct => spct.SanPham)
                        .ThenInclude(sp => sp.AnhSanPhams)
                .Include(x => x.SanPhamCT.SizeAo)
                .Include(x => x.SanPhamCT.MauSac)
                .Include(x => x.SanPhamCT.ChatLieu)
                .ToListAsync();

            return list;
        }
    }
}
