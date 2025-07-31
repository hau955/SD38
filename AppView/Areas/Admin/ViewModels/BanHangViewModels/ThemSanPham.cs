using AppApi.ViewModels.BanHang;

namespace AppView.Areas.Admin.ViewModels.BanHangViewModels
{
    public class ThemSanPham
    {
        public Guid IDHoaDon { get; set; }
        public List<SanPhamBanRequest> DanhSachSanPham { get; set; } = new();
    }
}
