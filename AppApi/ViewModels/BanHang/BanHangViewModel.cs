namespace AppApi.ViewModels.BanHang
{
    public class BanHangViewModel
    {
        public Guid? IDNguoiTao { get; set; }               // Có thể null
        public Guid? IDUser { get; set; }                   // Có thể null
       // public Guid IDHinhThucTT { get; set; }              // Bắt buộc
        //public Guid IDDiaChiNhanHang { get; set; }          // Bắt buộc (tạm dùng default nếu chưa cần giao hàng)
       // public float? PhanTramGiamHoaDon { get; set; }      // Tùy chọn
        public string? GhiChu { get; set; }
        public bool IsHoaDonCho { get; set; } = false;
        public List<SanPhamBanRequest> DanhSachSanPham { get; set; } = new();
    }

    public class SanPhamBanRequest
    {
        public Guid IDSanPhamCT { get; set; }
        public int SoLuong { get; set; }
    }
}
