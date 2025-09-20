using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly ApplicationDbContext _context;

        public GioHangService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Nếu user chưa có giỏ thì tạo mới
        public async Task<GioHang> TaoGioHangNeuChuaCoAsync(Guid idUser)
        {
            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.IDGioHang == idUser);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    IDGioHang = idUser,
                    TrangThai = true
                };

                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            return gioHang;
        }

        // ✅ Lấy giá sau giảm (nếu có khuyến mãi)
        private async Task<decimal> TinhGiaSauGiam(Guid idSanPham, decimal giaGoc)
        {
            var sp = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.IDSanPham == idSanPham);

            if (sp == null) return giaGoc;

            // Lấy giảm giá theo sản phẩm
            var giamGiaSPs = await _context.GiamGiaSanPham
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPham == idSanPham &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // Lấy giảm giá theo danh mục
            var giamGiaDMs = await _context.GiamGiaDanhMuc
                .Include(x => x.GiamGia)
                .Where(x => x.DanhMucId == sp.DanhMucId &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

            var allDiscounts = giamGiaSPs.Any() ? giamGiaSPs : giamGiaDMs;

            decimal giaSauGiam = giaGoc;

            foreach (var giamGia in allDiscounts)
            {
                decimal giaTmp = giaGoc;

                if (giamGia.LoaiGiamGia == "PhanTram")
                {
                    var soTienGiam = giaGoc * (giamGia.GiaTri / 100);
                    if (giamGia.GiaTriGiamToiDa.HasValue)
                        soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                    giaTmp = giaGoc - soTienGiam;
                }
                else if (giamGia.LoaiGiamGia == "SoTien")
                {
                    giaTmp = Math.Max(0, giaGoc - giamGia.GiaTri);
                }

                if (giaTmp < giaSauGiam)
                {
                    giaSauGiam = giaTmp;
                }
            }

            return giaSauGiam;
        }

        // ✅ Thêm sản phẩm vào giỏ (có giảm giá)
        public async Task<string> ThemSanPhamVaoGioAsync(Guid idUser, Guid idSanPhamCT, int soLuong)
        {
            // B1: lấy hoặc tạo giỏ hàng cho user
            var gioHang = await TaoGioHangNeuChuaCoAsync(idUser);

            // B2: kiểm tra sản phẩm chi tiết
            var sanPhamCT = await _context.SanPhamChiTiets
                .Include(ct => ct.SanPham)
                .FirstOrDefaultAsync(ct => ct.IDSanPhamCT == idSanPhamCT);

            if (sanPhamCT == null) return "❌ Sản phẩm không tồn tại";

            // B3: tính giá sau giảm
            var giaSauGiam = await TinhGiaSauGiam(sanPhamCT.IDSanPham, sanPhamCT.GiaBan);

            // B4: tìm xem sản phẩm đã có trong giỏ chưa
            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(ct => ct.IDGioHang == gioHang.IDGioHang && ct.IDSanPhamCT == idSanPhamCT);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
                chiTiet.DonGia = giaSauGiam; // cập nhật lại giá theo KM mới
            }
            else
            {
                chiTiet = new GioHangCT
                {
                    IDGioHangChiTiet = Guid.NewGuid(),
                    IDGioHang = gioHang.IDGioHang,
                    IDSanPhamCT = idSanPhamCT,
                    SoLuong = soLuong,
                    DonGia = giaSauGiam,
                    TrangThai = true
                };
                _context.GioHangChiTiets.Add(chiTiet);
            }

            await _context.SaveChangesAsync();
            return "✅ Đã thêm sản phẩm vào giỏ hàng";
        }

        // ✅ Lấy danh sách sản phẩm trong giỏ (có giảm giá)
        public async Task<IEnumerable<GioHangCT>> LayDanhSachSanPhamAsync(Guid idUser)
        {
            var gioHang = await TaoGioHangNeuChuaCoAsync(idUser);

            var chiTiets = await _context.GioHangChiTiets
                .Where(ct => ct.IDGioHang == gioHang.IDGioHang)
                .Include(ct => ct.SanPhamCT)
                    .ThenInclude(sp => sp.SanPham)
                .ToListAsync();

            // cập nhật lại giá theo KM mới nhất
            foreach (var ct in chiTiets)
            {
                var giaSauGiam = await TinhGiaSauGiam(ct.SanPhamCT.IDSanPham, ct.SanPhamCT.GiaBan);
                ct.DonGia = giaSauGiam;
            }

            return chiTiets;
        }

    }
}
