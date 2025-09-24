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
        // ✅ Tính giá sau giảm (ưu tiên SPCT > SP > Danh mục)
        private async Task<decimal> TinhGiaSauGiam(Guid idSanPhamCT, decimal giaGoc)
        {
            var spct = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                .ThenInclude(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.IDSanPhamCT == idSanPhamCT);

            if (spct == null) return giaGoc;

            // 🔎 Giảm giá theo sản phẩm chi tiết
            var giamGiaSPCTs = await _context.GiamGiaSPCT
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPhamCT == idSanPhamCT &&
                            x.GiamGia.TrangThai &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // 🔎 Giảm giá theo sản phẩm
            var giamGiaSPs = await _context.GiamGiaSanPham
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPham == spct.IDSanPham &&
                            x.GiamGia.TrangThai &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // 🔎 Giảm giá theo danh mục
            var giamGiaDMs = await _context.GiamGiaDanhMuc
                .Include(x => x.GiamGia)
                .Where(x => x.DanhMucId == spct.SanPham.DanhMucId &&
                            x.GiamGia.TrangThai &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // Ưu tiên: SPCT > SP > DM
            var allDiscounts = giamGiaSPCTs.Any() ? giamGiaSPCTs
                             : giamGiaSPs.Any() ? giamGiaSPs
                             : giamGiaDMs;

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
        // ✅ Thêm sản phẩm vào giỏ (có giảm giá + giới hạn)
        public async Task<string> ThemSanPhamVaoGioAsync(Guid idUser, Guid idSanPhamCT, int soLuong)
        {
            if (soLuong <= 0) return "❌ Số lượng không hợp lệ";

            var gioHang = await TaoGioHangNeuChuaCoAsync(idUser);

            var sanPhamCT = await _context.SanPhamChiTiets
                .Include(ct => ct.SanPham)
                .FirstOrDefaultAsync(ct => ct.IDSanPhamCT == idSanPhamCT);

            if (sanPhamCT == null) return "❌ Sản phẩm không tồn tại";

            // 🔎 Giới hạn số loại sản phẩm
            var soLoaiSPTrongGio = await _context.GioHangChiTiets
                .CountAsync(x => x.IDGioHang == gioHang.IDGioHang);
            var chiTiet = await _context.GioHangChiTiets
                .FirstOrDefaultAsync(ct => ct.IDGioHang == gioHang.IDGioHang && ct.IDSanPhamCT == idSanPhamCT);

            if (soLoaiSPTrongGio >= 10 && chiTiet == null)
                return "❌ Giỏ hàng chỉ được chứa tối đa 10 loại sản phẩm.";

            // 🔎 Giới hạn 10 cái mỗi sản phẩm
            if (chiTiet != null)
            {
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
                    DonGia = sanPhamCT.GiaBan,
                    TrangThai = true
                };
                _context.GioHangChiTiets.Add(chiTiet);
            }

            // ✅ tính giá sau giảm
            chiTiet.DonGia = await TinhGiaSauGiam(sanPhamCT.IDSanPhamCT, sanPhamCT.GiaBan);

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
                var giaSauGiam = await TinhGiaSauGiam(ct.SanPhamCT.IDSanPhamCT, ct.SanPhamCT.GiaBan);
                ct.DonGia = giaSauGiam;
            }

            return chiTiets;
        }

    }
}
