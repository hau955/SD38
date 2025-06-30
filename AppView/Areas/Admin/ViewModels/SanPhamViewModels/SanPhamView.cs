namespace AppView.Areas.Admin.ViewModels.SanPhamViewModels
{
    public class SanPhamView
    {
        public Guid IDSanPham { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
        public double TrongLuong { get; set; }
        public bool GioiTinh { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }
        public string? HinhAnh { get; set; }
        public Guid DanhMucID { get; set; }
        public string? TenDanhMuc { get; set; }
    }
}
