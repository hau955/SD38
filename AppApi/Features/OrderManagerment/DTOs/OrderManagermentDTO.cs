using System.ComponentModel.DataAnnotations;

namespace AppApi.Features.OrderManagerment.DTOs
{
    public class OrderListDto
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

    public class OrderDetailDto
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

        // Địa chỉ giao hàng
        public string? DiaChiGiaoHang { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? SoDienThoaiNhan { get; set; }

        // Chi tiết sản phẩm
        public List<OrderItemDto> ChiTietSanPhams { get; set; } = new List<OrderItemDto>();

        // Lịch sử trạng thái
        public List<OrderStatusHistoryDto> LichSuTrangThai { get; set; } = new List<OrderStatusHistoryDto>();
    }

    public class OrderItemDto
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

    public class OrderStatusHistoryDto
    {
        public string TrangThai { get; set; } = string.Empty;
        public DateTime NgayCapNhat { get; set; }
        public string? NguoiCapNhat { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public Guid IDHoaDon { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThaiMoi { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? GhiChu { get; set; }

        [Required]
        public Guid IDNguoiCapNhat { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        [Required]
        public Guid IDHoaDon { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThaiThanhToan { get; set; } = string.Empty;

        [Required]
        public Guid IDNguoiCapNhat { get; set; }

        public DateTime? NgayThanhToan { get; set; }
    }

    public class CancelOrderDto
    {
        [Required]
        public Guid IDHoaDon { get; set; }

        [Required]
        [MaxLength(1000)]
        public string LyDoHuy { get; set; } = string.Empty;

        [Required]
        public Guid IDNguoiHuy { get; set; }
    }

    public class OrderFilterDto
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
}