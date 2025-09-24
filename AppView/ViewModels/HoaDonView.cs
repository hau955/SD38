using AppData.Models;

namespace AppView.ViewModels
{
    public class HoaDonChiTietView
    {
        public Guid IdHoaDonChiTiet { get; set; }      // ID chi tiết hóa đơn
        public int SoLuong { get; set; }              // Số lượng sản phẩm
        public decimal DonGia { get; set; }           // Giá gốc sản phẩm
        public decimal GiaSauGiam { get; set; }       // Giá sau giảm (nếu có)

        public string TenSanPham { get; set; } = string.Empty;
        public string? MauSac { get; set; }          // Màu sắc nếu có
        public string? Size { get; set; }            // Size nếu có
        public string? ChatLieu { get; set; }
        public string DuongDanAnh { get; set; }// Size nếu có
    }

    public class HoaDonView
    {
        public Guid IdHoaDon { get; set; }

        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public decimal? TienGiam { get; set; }
        public decimal? TienGiamhoadon { get; set; }
        public decimal? phiship { get; set; }

        public string TrangThaiDonHang { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = string.Empty;

        // Navigation
        public HinhThucTT? HinhThucTT { get; set; }
        public DateTime? NgayTao { get; set; }
        public Voucher? Voucher { get; set; }

        public List<HoaDonChiTietView> ChiTietSanPhams { get; set; } = new();
    }
}
