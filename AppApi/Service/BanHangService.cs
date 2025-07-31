using AppApi.IService;
using AppApi.ViewModels.BanHang;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class BanHangService : IBanHangService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public BanHangService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<(bool IsSuccess, string Message, Guid? HoaDonId)> BanTaiQuayAsync(BanHangViewModel request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ✅ Kiểm tra số lượng hóa đơn chờ
                if (request.IsHoaDonCho)
                {
                    var hoaDonChoCount = await _context.HoaDons
                        .CountAsync(h => h.TrangThaiDonHang == "Chờ thanh toán" && h.TrangThaiThanhToan == "Chưa thanh toán");

                    if (hoaDonChoCount >= 20)
                    {
                        return (false, "Đã đạt giới hạn 20 hóa đơn chờ. Vui lòng xử lý bớt trước khi tạo mới.", null);
                    }
                }

                var hoaDon = new HoaDon
                {
                    IDHoaDon = Guid.NewGuid(),
                    NgayTao = DateTime.Now,
                    IDUser = new Guid("07E1FC26-D86A-44F6-AC5A-F28452D4E22B"),

                    IDNguoiTao = request.IDNguoiTao,
                    TrangThaiDonHang = request.IsHoaDonCho ? "Chờ thanh toán" : "Đã bán",
                    TrangThaiThanhToan = request.IsHoaDonCho ? "Chưa thanh toán" : "Đã thanh toán",
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

                    // ✅ Trừ kho luôn (nếu bạn muốn hóa đơn chờ cũng trừ kho)
                    sanPhamCT.SoLuongTonKho -= sp.SoLuong;

                    var thanhTien = sanPhamCT.GiaBan * sp.SoLuong;
                    tongTien += thanhTien;

                    hoaDon.HoaDonChiTiets.Add(new HoaDonCT
                    {
                        IDHoaDonChiTiet = Guid.NewGuid(),
                        IDHoaDon = hoaDon.IDHoaDon,
                        IDSanPhamCT = sanPhamCT.IDSanPhamCT,
                        TenSanPham = sanPhamCT.SanPham!.TenSanPham ,
                        SoLuongSanPham = sp.SoLuong,
                        GiaSanPham = sanPhamCT.GiaBan,
                        GiaSauGiamGia = sanPhamCT.GiaBan,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });
                }

                decimal tienGiam = 0;

                hoaDon.TongTienTruocGiam = tongTien;
                hoaDon.TienGiam = tienGiam;
                hoaDon.TongTienSauGiam = tongTien - tienGiam;
                hoaDon.TienGiamHoaDon = tienGiam;

                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Tạo hóa đơn thành công", hoaDon.IDHoaDon);
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
        public async Task<(bool IsSuccess, string Message)> ThanhToanHoaDonChoAsync(ThanhToanHoaDonRequest request)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.IDHoaDon == request.IDHoaDon);

            if (hoaDon == null)
                return (false, "Không tìm thấy hóa đơn");

            if (hoaDon.TrangThaiThanhToan == "Đã thanh toán")
                return (false, "Hóa đơn này đã được thanh toán rồi");

            hoaDon.TrangThaiDonHang = "Đã bán";
            hoaDon.TrangThaiThanhToan = "Đã thanh toán";
            hoaDon.NgayThanhToan = DateTime.Now;
            hoaDon.NgaySua = DateTime.Now;
           
            hoaDon.GhiChu += " | " + request.GhiChuThanhToan;

            await _context.SaveChangesAsync();
            return (true, "Thanh toán hóa đơn thành công");
        }
        public async Task<(bool IsSuccess, string Message)> ThemSanPhamVaoHoaDonChoAsync(ThemSanPham request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await _context.HoaDons
                    .FirstOrDefaultAsync(h => h.IDHoaDon == request.IDHoaDon);

                if (hoaDon == null || hoaDon.TrangThaiThanhToan == "Đã thanh toán")
                    return (false, "Hóa đơn không hợp lệ hoặc đã thanh toán");

                decimal tongTienThem = 0;

                foreach (var sp in request.DanhSachSanPham)
                {
                    var sanPhamCT = await _context.SanPhamChiTiets
                        .Include(x => x.SanPham)
                        .FirstOrDefaultAsync(x => x.IDSanPhamCT == sp.IDSanPhamCT);


                    if (sanPhamCT == null)
                        return (false, $"Không tìm thấy sản phẩm chi tiết {sp.IDSanPhamCT}");

                    if (sanPhamCT.SoLuongTonKho < sp.SoLuong)
                        return (false, $"Sản phẩm không đủ tồn kho");

                    sanPhamCT.SoLuongTonKho -= sp.SoLuong;

                    if (_context.Entry(sanPhamCT).State == EntityState.Detached)
                        _context.SanPhamChiTiets.Attach(sanPhamCT);

                    _context.Entry(sanPhamCT).Property(x => x.SoLuongTonKho).IsModified = true;

                    var thanhTien = sanPhamCT.GiaBan * sp.SoLuong;
                    tongTienThem += thanhTien;

                    _context.HoaDonChiTiets.Add(new HoaDonCT
                    {
                        IDHoaDonChiTiet = Guid.NewGuid(),
                        IDHoaDon = hoaDon.IDHoaDon,
                        IDSanPhamCT = sanPhamCT.IDSanPhamCT,
                        TenSanPham = sanPhamCT.SanPham!.TenSanPham ,
                        SoLuongSanPham = sp.SoLuong,
                        GiaSanPham = sanPhamCT.GiaBan,
                        GiaSauGiamGia = sanPhamCT.GiaBan,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });

                }

                hoaDon.TongTienTruocGiam += tongTienThem;
                hoaDon.TongTienSauGiam += tongTienThem;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Thêm sản phẩm thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return (false, "Lỗi đồng bộ dữ liệu. Có thể sản phẩm đã thay đổi hoặc bị xóa.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException?.Message ?? ex.Message;
                return (false, $"Lỗi hệ thống chi tiết: {inner}");
            }

        }

        public async Task<(bool IsSuccess, string Message)> TruSanPhamKhoiHoaDonChoAsync(TruSanPham request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .FirstOrDefaultAsync(h => h.IDHoaDon == request.IDHoaDon);

                if (hoaDon == null || hoaDon.TrangThaiThanhToan == "Đã thanh toán")
                    return (false, "Hóa đơn không tồn tại hoặc đã thanh toán");

                // ✅ Tìm chi tiết hóa đơn theo ID sản phẩm chi tiết
                var chiTiet = hoaDon.HoaDonChiTiets
                    .FirstOrDefault(x => x.IDSanPhamCT == request.IDSanPhamCT && x.TrangThai);

                if (chiTiet == null)
                    return (false, "Không tìm thấy sản phẩm chi tiết trong hóa đơn");

                if (request.SoLuong > chiTiet.SoLuongSanPham)
                    return (false, "Số lượng cần trừ lớn hơn số lượng trong hóa đơn");

                // ✅ Cộng lại tồn kho
                var sanPhamCT = await _context.SanPhamChiTiets
                    .FirstOrDefaultAsync(sp => sp.IDSanPhamCT == request.IDSanPhamCT);

                if (sanPhamCT == null)
                    return (false, "Không tìm thấy sản phẩm chi tiết");

                sanPhamCT.SoLuongTonKho += request.SoLuong;

                // ✅ Trừ số lượng, hoặc xóa nếu hết
                var tienTru = request.SoLuong * chiTiet.GiaSauGiamGia;

                chiTiet.SoLuongSanPham -= request.SoLuong;

                if (chiTiet.SoLuongSanPham <= 0)
                {
                    _context.HoaDonChiTiets.Remove(chiTiet);
                }
                else
                {
                    chiTiet.NgayTao = DateTime.Now;
                    _context.HoaDonChiTiets.Update(chiTiet);
                }

                // ✅ Cập nhật tổng tiền
                hoaDon.TongTienTruocGiam -= tienTru;
                hoaDon.TongTienSauGiam -= tienTru;

                _context.HoaDons.Update(hoaDon);
                _context.SanPhamChiTiets.Update(sanPhamCT);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Đã trừ sản phẩm khỏi hóa đơn");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> HuyHoaDonAsync(Guid idHoaDon)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                    .FirstOrDefaultAsync(h => h.IDHoaDon == idHoaDon);

                if (hoaDon == null)
                    return (false, "Không tìm thấy hóa đơn");

                if (hoaDon.TrangThaiThanhToan == "Đã thanh toán")
                    return (false, "Hóa đơn đã thanh toán, không thể hủy");

                if (hoaDon.TrangThaiThanhToan == "Đã hủy")
                    return (false, "Hóa đơn đã bị hủy trước đó");
                if (hoaDon.TrangThaiDonHang == "Đã hủy")
                    return (false, "Hóa đơn đã bị hủy trước đó");

                // ✅ Trả lại tồn kho & xóa chi tiết hóa đơn
                foreach (var ct in hoaDon.HoaDonChiTiets.ToList())
                {
                    var spCT = await _context.SanPhamChiTiets
                        .FirstOrDefaultAsync(x => x.IDSanPham == ct.IDSanPhamCT);

                    if (spCT != null)
                    {
                        spCT.SoLuongTonKho += ct.SoLuongSanPham;
                        _context.SanPhamChiTiets.Update(spCT);
                    }

                    _context.HoaDonChiTiets.Remove(ct); // 👉 XÓA
                }

                // ✅ Cập nhật hóa đơn
                hoaDon.TongTienTruocGiam = 0;
                hoaDon.TongTienSauGiam = 0;
                hoaDon.TrangThaiThanhToan = "Đã hủy";
                hoaDon.NgayTao = DateTime.Now;

                _context.HoaDons.Update(hoaDon);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Hủy hóa đơn thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Lỗi hệ thống: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        public async Task<(bool IsSuccess, string Message, HoaDonChiTietViewModel? Data)> XemChiTietHoaDonAsync(Guid idHoaDon)
        {
            var hoaDon = await _context.HoaDons
     .Include(h => h.HoaDonChiTiets)
         .ThenInclude(ct => ct.SanPhamCT)
             .ThenInclude(spct => spct.SanPham)
     .Include(h => h.HoaDonChiTiets)
         .ThenInclude(ct => ct.SanPhamCT)
             .ThenInclude(spct => spct.SizeAo)
     .Include(h => h.HoaDonChiTiets)
         .ThenInclude(ct => ct.SanPhamCT)
             .ThenInclude(spct => spct.MauSac)
             .Include(h => h.HoaDonChiTiets)
         .ThenInclude(ct => ct.SanPhamCT)
             .ThenInclude(spct => spct.ChatLieu)
             .Include(h => h.User2).Include(h => h.User)
     .FirstOrDefaultAsync(h => h.IDHoaDon == idHoaDon);
                
     


            if (hoaDon == null)
            {
                return (false, "Không tìm thấy hóa đơn", null);
            }

            var result = new HoaDonChiTietViewModel
            {
                IDHoaDon = hoaDon.IDHoaDon,
                NgayTao = hoaDon.NgayTao ?? DateTime.Now,
                NgayThanhToan = hoaDon.NgayThanhToan ?? DateTime.Now,

                TenNguoiTao = hoaDon.User2?.HoTen ?? "Không rõ",
                NguoiMuaHang=hoaDon.User?.HoTen?? "Không rõ",
                TrangThaiDonHang = hoaDon.TrangThaiDonHang,
                TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                TongTienTruocGiam = hoaDon.TongTienTruocGiam,
                TienGiam = hoaDon.TienGiam,
                TongTienSauGiam = hoaDon.TongTienSauGiam,
                GhiChu = hoaDon.GhiChu,
                DanhSachSanPham = hoaDon.HoaDonChiTiets
    .GroupBy(ct => ct.IDSanPhamCT)
    .Select(g =>
    {
        var first = g.First();
        return new ChiTietSanPhamViewModel
        {
            IDSanPhamCT = g.Key,
            TenSanPham = $"{first.SanPhamCT.SanPham.TenSanPham} - Chất liệu: {first.SanPhamCT.ChatLieu.TenChatLieu} - Màu: {first.SanPhamCT.MauSac.TenMau} - Size: {first.SanPhamCT.SizeAo.SoSize}  - {first.SanPhamCT.GiaBan:N0} đ",
            SoLuong = g.Sum(x => x.SoLuongSanPham),
            DonGia = first.GiaSanPham
        };
    })
    .ToList()

            };

            return (true, "Lấy chi tiết hóa đơn thành công", result);
        }

        public async Task<string> TaoUrlThanhToanAsync(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);
            if (hoaDon == null || hoaDon.TrangThaiThanhToan == "Đã thanh toán")
                throw new Exception("Hóa đơn không tồn tại hoặc đã thanh toán");

            var vnp_TmnCode = _config["VnPay:TmnCode"];
            var vnp_HashSecret = _config["VnPay:HashSecret"];
            var vnp_Url = _config["VnPay:Url"];
            var vnp_Returnurl = _config["VnPay:ReturnUrl"];

            var price = (int)hoaDon.TongTienSauGiam * 100;

            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", "2.1.0");
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnPay.AddRequestData("vnp_Amount", price.ToString());
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1"); // dùng IP client nếu có
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toan hoa don #" + hoaDonId);
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnPay.AddRequestData("vnp_TxnRef", hoaDonId.ToString());
            vnPay.AddRequestData("vnp_SecureHashType", "HMACSHA512");



            var paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public async Task<(bool IsSuccess, string Message)> XuLyKetQuaThanhToanAsync(IQueryCollection query)
        {
            var vnpay = new VnPayLibrary();
            var response = vnpay.GetFullResponseData(query);

            var isSuccess = vnpay.ValidateSignature(query, _config["VnPay:HashSecret"]);

            if (!isSuccess)
                return (false, "Chữ ký không hợp lệ");

            var hoaDonId = Guid.Parse(response["vnp_TxnRef"]);
            var hoaDon = await _context.HoaDons.FindAsync(hoaDonId);

            if (hoaDon == null)
                return (false, "Không tìm thấy hóa đơn");

            hoaDon.TrangThaiThanhToan = "Đã thanh toán";
            hoaDon.NgayThanhToan = DateTime.Now;
            await _context.SaveChangesAsync();

            return (true, "Thanh toán thành công");
        }
    }
}
