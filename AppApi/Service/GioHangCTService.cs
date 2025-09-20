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

        // ✅ Thêm chi tiết giỏ hàng
        public async Task<string> ThemChiTietAsync(Guid idUser, Guid idSanPhamCT, int soLuong)
        {
            var gioHang = await _gioHangService.TaoGioHangNeuChuaCoAsync(idUser);

            var spct = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.IDSanPhamCT == idSanPhamCT);

            if (spct == null) return "❌ Sản phẩm không tồn tại";

            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(x => x.IDGioHang == gioHang.IDGioHang && x.IDSanPhamCT == idSanPhamCT);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
            }
            else
            {
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

        // ✅ Cập nhật số lượng
        public async Task<string> CapNhatSoLuongAsync(Guid idGioHangCT, int soLuongMoi)
        {
            var chiTiet = await _context.GioHangChiTiets.FindAsync(idGioHangCT);
            if (chiTiet == null) return "❌ Không tìm thấy chi tiết giỏ hàng";

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

            Console.WriteLine($"[DEBUG] idUser: {idUser}, gioHang.IDGioHang: {gioHang.IDGioHang}");

            var list = await _context.GioHangChiTiets
                .Where(x => x.IDGioHang == gioHang.IDGioHang)
                .Include(x => x.SanPhamCT)
                    .ThenInclude(sp => sp.SanPham)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] SoLuong chi tiet: {list.Count}");
            foreach (var item in list)
            {
                Console.WriteLine($" - CT: {item.IDGioHangChiTiet}, SPCT: {item.IDSanPhamCT}, SoLuong: {item.SoLuong}");
            }

            return list;
        }

    }
}
