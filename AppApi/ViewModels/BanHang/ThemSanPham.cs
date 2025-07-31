namespace AppApi.ViewModels.BanHang
{
    public class ThemSanPham
    {
        public Guid IDHoaDon { get; set; }
        public List<SanPhamBanRequest> DanhSachSanPham { get; set; } = new();
    }
}
