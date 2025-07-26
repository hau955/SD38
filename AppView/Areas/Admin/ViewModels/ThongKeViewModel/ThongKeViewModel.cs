namespace AppView.Areas.Admin.ViewModels.ThongKeViewModel
{
    public class DoanhThuTheoNgayViewModel
    {
        public DateTime Ngay { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class DoanhThuTheoThangViewModel
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class SanPhamBanChayViewModel
    {
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuongDaBan { get; set; }
    }

    public class TongQuanThongKeViewModel
    {
        public int TongHoaDon { get; set; }
        public int DonHangThanhCong { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int KhachHangMoi { get; set; }
        public int SanPhamDangBan { get; set; }
        public int TongGiamGiaDangHoatDong { get; set; }
    }
}
