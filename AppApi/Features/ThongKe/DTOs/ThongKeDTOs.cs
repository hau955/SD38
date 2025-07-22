namespace AppApi.Features.ThongKe.DTOs
{
    public class DoanhThuTheoNgayDto
    {
        public DateTime Ngay { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class DoanhThuTheoThangDto
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class SanPhamBanChayDto
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuongDaBan { get; set; }
    }

    public class TongQuanThongKeDto
    {
        public int TongHoaDon { get; set; }
        public int DonHangThanhCong { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int KhachHangMoi { get; set; }
        public int SanPhamDangBan { get; set; }
        public int TongGiamGiaDangHoatDong { get; set; }
    }
}
