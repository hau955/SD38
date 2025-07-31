using AppData.Models;

namespace AppView.ViewModels.GioHang
{
    public class GioHangView
    {
        public Guid IDGioHangChiTiet { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}
