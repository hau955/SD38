namespace AppView.Areas.Admin.ViewModels.BanHangViewModels
{
    public class HoaDonChoVM
    {
        public Guid IDHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public string? GhiChu { get; set; }
        public List<SanPhamTrongHoaDonViewModel> SanPhams { get; set; } = new();
    }
    public class SanPhamTrongHoaDonViewModel
    {
        public Guid IdSanPhamCT { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
    }
}
