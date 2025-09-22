using AppApi.IService;
using AppData.Models;
using AppApi.ViewModels.TrangThai;
using Microsoft.EntityFrameworkCore;
using AppApi.ViewModels.HoaDonDTOs;

namespace AppApi.Service
{
    public class HoaDonService : IHoaDonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGioHangService _gioHangService;

        public HoaDonService(ApplicationDbContext context, IGioHangService gioHangService)
        {
            _context = context;
            _gioHangService = gioHangService;
        }

        /// <summary>
        /// Tạo hóa đơn từ giỏ hàng (COD hoặc Online)
        /// </summary>
        /// <summary>
        /// Tạo hóa đơn từ giỏ hàng (COD hoặc Online)
        /// </summary>
        public async Task<HoaDon> TaoHoaDonTuGioHangAsync(
            Guid idUser,
            Guid idHinhThucTT,
            Guid? idVoucher = null,
            Guid? idDiaChi = null,
            string? ghiChu = null)
        {
            // 1️⃣ Lấy giỏ hàng chi tiết
            var gioHang = await _gioHangService.TaoGioHangNeuChuaCoAsync(idUser);

            var chiTiets = await _context.GioHangChiTiets
                .Include(ct => ct.SanPhamCT)
                    .ThenInclude(sp => sp.SanPham)
                .Where(ct => ct.IDGioHang == gioHang.IDGioHang)
                .ToListAsync();

            if (!chiTiets.Any())
                throw new Exception("❌ Giỏ hàng đang trống, không thể tạo hóa đơn");

            // 2️⃣ Tính tiền trước giảm (theo giá gốc SP)
            decimal tongTienTruocGiam = chiTiets.Sum(ct => ct.SanPhamCT.GiaBan * ct.SoLuong);

            // 3️⃣ Tính tiền sau giảm từ giỏ hàng (sau khi SP có khuyến mãi)
            decimal tongTienSauGiamSanPham = chiTiets.Sum(ct => ct.DonGia * ct.SoLuong);

            // Giảm từ sản phẩm
            decimal tienGiam = tongTienTruocGiam - tongTienSauGiamSanPham;

            // 4️⃣ Xử lý voucherif (voucher.SoLanSuDungToiDa.HasValue && voucher.SoLanSuDungToiDa.Value > 0)
            


            decimal tienGiamHoaDon = 0;
            if (idVoucher.HasValue)
            {
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.IdVoucher == idVoucher);

                if (voucher == null || !voucher.StartDate.HasValue || !voucher.EndDate.HasValue
                    || DateTime.Now < voucher.StartDate.Value || DateTime.Now > voucher.EndDate.Value)
                {
                    throw new Exception("❌ Voucher không hợp lệ hoặc đã hết hạn");
                }

                if (voucher.DieuKienToiThieu.HasValue && tongTienSauGiamSanPham < voucher.DieuKienToiThieu.Value)
                {
                    throw new Exception("❌ Hóa đơn không đạt điều kiện tối thiểu để áp dụng voucher");
                }

                if (voucher.PhanTram.HasValue)
                {
                    tienGiamHoaDon = tongTienSauGiamSanPham * (decimal)(voucher.PhanTram.Value / 100);
                }
                else if (voucher.SoTienGiam.HasValue)
                {
                    tienGiamHoaDon = voucher.SoTienGiam.Value;
                }

                if (tienGiamHoaDon > tongTienSauGiamSanPham)
                    tienGiamHoaDon = tongTienSauGiamSanPham;

                if (voucher.SoLuong.HasValue && voucher.SoLuong > 0)
                {
                    voucher.SoLuong -= 1;
                }
                else
                {
                    throw new Exception("❌ Voucher đã hết lượt sử dụng");
                }
                if (voucher.SoLanSuDungToiDa.HasValue && voucher.SoLanSuDungToiDa.Value > 0)
                {
                    // Lấy số lần user này đã dùng voucher
                    int daSuDung = await _context.HoaDons
                        .CountAsync(hd => hd.IDUser == idUser && hd.IdVoucher == idVoucher);

                    if (daSuDung >= voucher.SoLanSuDungToiDa.Value)
                        throw new Exception("❌ Bạn đã đạt giới hạn số lần sử dụng voucher này");
                }
            }

            // 5️⃣ Cộng phí vận chuyển
            decimal phiVanChuyen = 30000;

            // ✅ Tổng tiền cuối cùng
            decimal tongTienSauGiam = tongTienSauGiamSanPham - tienGiamHoaDon + phiVanChuyen;

            // 6️⃣ Xác định trạng thái thanh toán
            var hinhThuc = await _context.HinhThucTTs.FindAsync(idHinhThucTT);
            if (hinhThuc == null) throw new Exception("❌ Hình thức thanh toán không hợp lệ");

            string trangThaiThanhToan = "ChuaThanhToan";
            DateTime? ngayThanhToan = null;

            if (hinhThuc.TenHinhThucTT.ToLower().Contains("online"))
            {
                trangThaiThanhToan = "DaThanhToan";
                ngayThanhToan = DateTime.Now;
            }

            // 7️⃣ Lấy địa chỉ nhận hàng
            DiaChiNhanHang? diaChi;
            if (idDiaChi.HasValue)
            {
                diaChi = await _context.DiaChiNhanHangs
                    .Where(dc => dc.IDDiaChiNhanHang == idDiaChi && dc.IDUser == idUser && dc.TrangThai)
                    .FirstOrDefaultAsync();
            }
            else
            {
                diaChi = await _context.DiaChiNhanHangs
                    .Where(dc => dc.IDUser == idUser && dc.TrangThai && dc.IsDefault)
                    .FirstOrDefaultAsync();
            }

            if (diaChi == null)
                throw new Exception("❌ Chưa có địa chỉ nhận hàng hợp lệ cho user này");

            // 8️⃣ Tạo hóa đơn
            var hoaDon = new HoaDon
            {
                IDHoaDon = Guid.NewGuid(),
                IDUser = idUser,
                IDHinhThucTT = idHinhThucTT,
                IDDiaChiNhanHang = idDiaChi,
                IdVoucher = idVoucher,
                TongTienTruocGiam = tongTienTruocGiam,
                PhiVanChuyen = phiVanChuyen,
                TongTienSauGiam = tongTienSauGiam,
                TienGiam = tienGiam,                 // ✅ giảm từ sản phẩm
                TienGiamHoaDon = tienGiamHoaDon,     // ✅ giảm từ voucher
                GhiChu = ghiChu,
                TrangThaiDonHang = TrangThaiDonHang.DangXuLy,
                TrangThaiThanhToan = trangThaiThanhToan,
                NgayThanhToan = ngayThanhToan,
                NgayTao = DateTime.Now,
                HoaDonChiTiets = new List<HoaDonCT>()
            };

            // 9️⃣ Thêm chi tiết hóa đơn
            foreach (var ct in chiTiets)
            {
                hoaDon.HoaDonChiTiets.Add(new HoaDonCT
                {
                    IDHoaDonChiTiet = Guid.NewGuid(),
                    IDHoaDon = hoaDon.IDHoaDon,
                    IDSanPhamCT = ct.IDSanPhamCT ?? Guid.Empty,
                    SoLuongSanPham = ct.SoLuong,
                    TenSanPham = ct.SanPhamCT.SanPham.TenSanPham,
                    GiaSanPham = ct.SanPhamCT.GiaBan,
                    GiaSauGiamGia = ct.DonGia,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                });

                // Trừ tồn kho
                ct.SanPhamCT.SoLuongTonKho -= ct.SoLuong;
            }

            // 🔟 Xóa giỏ hàng sau khi checkout
            _context.GioHangChiTiets.RemoveRange(chiTiets);

            // 1️⃣1️⃣ Lưu hóa đơn
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // 1️⃣2️⃣ Lưu lịch sử trạng thái
            _context.HoaDonTrangThai.Add(new HoaDonTrangThai
            {
                IDHoaDon = hoaDon.IDHoaDon,
                TrangThai = hoaDon.TrangThaiDonHang,
                NgayCapNhat = DateTime.Now,
                NguoiCapNhat = "Hệ thống"
            });
            await _context.SaveChangesAsync();

            return hoaDon;
        }

        /// <summary>
        /// Cập nhật trạng thái hóa đơn và lưu lịch sử
        /// </summary>
        public async Task CapNhatTrangThaiAsync(Guid idHoaDon, string trangThaiMoi, string? nguoiCapNhat = null)
        {
            var hoaDon = await _context.HoaDons.FindAsync(idHoaDon);
            if (hoaDon == null) throw new Exception("❌ Hóa đơn không tồn tại");

            // 1️⃣ Update trạng thái hiện tại
            hoaDon.TrangThaiDonHang = trangThaiMoi;
            hoaDon.NgaySua = DateTime.Now;

            // 2️⃣ Thêm vào lịch sử
            _context.HoaDonTrangThai.Add(new HoaDonTrangThai
            {
                IDHoaDon = hoaDon.IDHoaDon,
                TrangThai = trangThaiMoi,
                NgayCapNhat = DateTime.Now,
                NguoiCapNhat = nguoiCapNhat ?? "Hệ thống"
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Các hàm tiện lợi cho nút chuyển trạng thái
        /// </summary>
        public async Task ChuyenSangDaXacNhan(Guid idHoaDon, string? nguoiCapNhat = null)
        {
            await CapNhatTrangThaiAsync(idHoaDon, TrangThaiDonHang.DaXacNhan, nguoiCapNhat);
        }

        public async Task ChuyenSangDangGiao(Guid idHoaDon, string? nguoiCapNhat = null)
        {
            await CapNhatTrangThaiAsync(idHoaDon, TrangThaiDonHang.DangGiao, nguoiCapNhat);
        }

        public async Task ChuyenSangDaGiao(Guid idHoaDon, string? nguoiCapNhat = null)
        {
            await CapNhatTrangThaiAsync(idHoaDon, TrangThaiDonHang.DaGiao, nguoiCapNhat);
        }

        public async Task ChuyenSangHuy(Guid idHoaDon, string? nguoiCapNhat = null)
        {
            await CapNhatTrangThaiAsync(idHoaDon, TrangThaiDonHang.Huy, nguoiCapNhat);
        }
        public async Task KhachHangHuyHoaDonAsync(Guid idHoaDon, string tenKhachHang)
        {
            var hoaDon = await _context.HoaDons.FindAsync(idHoaDon);
            if (hoaDon == null) throw new Exception("Hóa đơn không tồn tại");

            // Chỉ cho hủy khi đang xử lý hoặc chưa xác nhận
            if (hoaDon.TrangThaiDonHang != TrangThaiDonHang.DangXuLy &&
                hoaDon.TrangThaiDonHang != TrangThaiDonHang.DaXacNhan)
            {
                throw new Exception("Không thể hủy hóa đơn ở trạng thái hiện tại");
            }

            // Cập nhật trạng thái
            await CapNhatTrangThaiAsync(idHoaDon, TrangThaiDonHang.Huy, tenKhachHang);
        }
        public async Task<List<HoaDonTrangThai>> LayLichSuTrangThaiAsync(Guid idHoaDon)
        {
            var lichSu = await _context.HoaDonTrangThai
                .Where(x => x.IDHoaDon == idHoaDon)
                .OrderBy(x => x.NgayCapNhat)
                .ToListAsync();

            if (!lichSu.Any())
                throw new Exception("Không có lịch sử trạng thái cho hóa đơn này");

            return lichSu;
        }
        public async Task<List<HoaDon>> LayHoaDonTheoUserAsync(Guid idUser)
        {
            var hoaDons = await _context.HoaDons
                .Include(hd => hd.HoaDonChiTiets)        // kèm chi tiết hóa đơn
                .Include(hd => hd.HinhThucTT)            // kèm thông tin hình thức thanh toán
                //.Include(hd => hd.Voucher)               // kèm thông tin voucher (nếu có)
                .Where(hd => hd.IDUser == idUser)
                .OrderByDescending(hd => hd.NgayTao)
                .ToListAsync();

            if (!hoaDons.Any())
                throw new Exception("❌ Người dùng chưa có hóa đơn nào");

            return hoaDons;
        }
        public async Task<HoaDonView?> XemChiTietHoaDonAsync(Guid idHoaDon)
        {
            var hoaDon = await _context.HoaDons
                .Include(hd => hd.HoaDonChiTiets)
                    .ThenInclude(ct => ct.SanPhamCT)
                        .ThenInclude(spct => spct.SanPham).ThenInclude(anh => anh.AnhSanPhams)
                .Include(hd => hd.HoaDonChiTiets)
                    .ThenInclude(ct => ct.SanPhamCT.MauSac)
                .Include(hd => hd.HoaDonChiTiets)
                    .ThenInclude(ct => ct.SanPhamCT.SizeAo)
                .Include(hd => hd.HoaDonChiTiets)
                    .ThenInclude(ct => ct.SanPhamCT.ChatLieu)
                    
                .Include(hd => hd.HinhThucTT)
               // .Include(hd => hd.Voucher)
                .FirstOrDefaultAsync(hd => hd.IDHoaDon == idHoaDon);

            if (hoaDon == null)
                return null;

            var hoaDonView = new HoaDonView
            {
                IdHoaDon = hoaDon.IDHoaDon,
                TongTienTruocGiam = hoaDon.TongTienTruocGiam,
                PhiVanChuyen = hoaDon.PhiVanChuyen??=0,
                TongTienSauGiam = hoaDon.TongTienSauGiam,
                TienGiam = hoaDon.TienGiam,
                TrangThaiDonHang = hoaDon.TrangThaiDonHang,
                TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                HinhThucTT = hoaDon.HinhThucTT,
                NgayTao=hoaDon.NgayTao,
              //  Voucher = hoaDon.Voucher,
                ChiTietSanPhams = hoaDon.HoaDonChiTiets.Select(ct => new HoaDonChiTietView
                {
                    IdHoaDonChiTiet = ct.IDHoaDonChiTiet,
                    SoLuong = ct.SoLuongSanPham,
                    DonGia = ct.GiaSanPham,
                    GiaSauGiam = ct.GiaSauGiamGia,
                    TenSanPham = ct.SanPhamCT?.SanPham?.TenSanPham ?? ct.TenSanPham ?? "Chưa có tên",
                    MauSac = ct.SanPhamCT?.MauSac?.TenMau,
                    Size = ct.SanPhamCT?.SizeAo?.SoSize,
                    ChatLieu = ct.SanPhamCT?.ChatLieu?.TenChatLieu,
                    DuongDanAnh = ct.SanPhamCT?.SanPham?.AnhSanPhams.FirstOrDefault(a => a.AnhChinh == true)?.DuongDanAnh ?? "no-image.png"
                }).ToList()
            };

            return hoaDonView;
        }


        public async Task<List<DiaChiNhanHang>> GetDiaChiByUserAsync(Guid idUser)
        {
            return await _context.DiaChiNhanHangs
                .Where(dc => dc.IDUser == idUser && dc.TrangThai)
                .OrderByDescending(dc => dc.IsDefault)
                .ToListAsync();
        }
        public async Task<List<Voucher>> LayTatCaVoucherAsync()
        {
            var now = DateTime.Now;

            return await _context.Vouchers
                .Where(v => v.StartDate <= now && v.EndDate >= now) // chỉ lấy voucher trong thời gian hiệu lực
                .Where(v => v.SoLuong == null || v.SoLuong > 0)    // còn số lượng
                .OrderByDescending(v => v.StartDate)               // sắp xếp mới nhất trước
                .ToListAsync();
        }


        public async Task<List<HinhThucTT>> GetAllAsync()
        {
            return await _context.HinhThucTTs
                                 .AsNoTracking()  // Không tracking để tối ưu khi chỉ đọc
                                 .ToListAsync();
        }
    }
}
