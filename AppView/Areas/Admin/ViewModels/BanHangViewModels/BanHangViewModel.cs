namespace AppView.Areas.Admin.ViewModels.BanHangViewModels
{
    public class BanHangViewModel
    {
        public Guid? IDNguoiTao { get; set; }               // Có thể null
        public Guid? IDUser { get; set; }  
        public string? GhiChu { get; set; }
        public bool IsHoaDonCho { get; set; }
        public List<SanPhamRequest> DanhSachSanPham { get; set; } = new();
    }
    public class SanPhamRequest
    {
        public Guid IDSanPhamCT { get; set; }
        public int SoLuong { get; set; }
       
    }
}
