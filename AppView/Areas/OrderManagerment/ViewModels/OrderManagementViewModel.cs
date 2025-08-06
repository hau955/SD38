namespace AppView.Areas.OrderManagerment.ViewModels
{
    using System.ComponentModel.DataAnnotations;


    public class OrderListViewModel
    {
        public Guid IDHoaDon { get; set; }
        public string? TenKhachHang { get; set; }
        public string? EmailKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public string TrangThaiDonHang { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public int SoLuongSanPham { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? TenNguoiTao { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Guid IDHoaDon { get; set; }
        public string? TenKhachHang { get; set; }
        public string? EmailKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public decimal TienGiam { get; set; }
        public string TrangThaiDonHang { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? TenNguoiTao { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? SoDienThoaiNhan { get; set; }
        public List<OrderItemViewModel> ChiTietSanPhams { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public Guid IDHoaDonChiTiet { get; set; }
        public Guid IDSanPhamCT { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int SoLuongSanPham { get; set; }
        public decimal GiaSanPham { get; set; }
        public decimal GiaSauGiamGia { get; set; }
        public string? MauSac { get; set; }
        public string? Size { get; set; }
        public string? ChatLieu { get; set; }
        public string? HinhAnh { get; set; }
    }

    public class UpdateOrderStatusViewModel
    {
        [Required(ErrorMessage = "ID hóa đơn là bắt buộc")]
        public Guid IDHoaDon { get; set; }

        [Required(ErrorMessage = "Trạng thái mới là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string TrangThaiMoi { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "ID người cập nhật là bắt buộc")]
        public Guid IDNguoiCapNhat { get; set; }
    }

    public class UpdatePaymentStatusViewModel
    {
        [Required(ErrorMessage = "ID hóa đơn là bắt buộc")]
        public Guid IDHoaDon { get; set; }

        [Required(ErrorMessage = "Trạng thái thanh toán là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Trạng thái thanh toán không được vượt quá 50 ký tự")]
        public string TrangThaiThanhToan { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID người cập nhật là bắt buộc")]
        public Guid IDNguoiCapNhat { get; set; }

        public DateTime? NgayThanhToan { get; set; }
    }

    public class CancelOrderViewModel
    {
        [Required(ErrorMessage = "ID hóa đơn là bắt buộc")]
        public Guid IDHoaDon { get; set; }

        [Required(ErrorMessage = "Lý do hủy là bắt buộc")]
        [MaxLength(1000, ErrorMessage = "Lý do hủy không được vượt quá 1000 ký tự")]
        public string LyDoHuy { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID người hủy là bắt buộc")]
        public Guid IDNguoiHuy { get; set; }
    }

    public class OrderFilterViewModel
    {
        public string? TrangThaiDonHang { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "NgayTao";
        public string SortDirection { get; set; } = "desc";
    }
    public class OrderStatusViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class PaymentStatusViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class OrderStatusOptionsViewModel
    {
        public List<OrderStatusViewModel> OrderStatuses { get; set; } = new List<OrderStatusViewModel>();
        public List<PaymentStatusViewModel> PaymentStatuses { get; set; } = new List<PaymentStatusViewModel>();
    }
}

