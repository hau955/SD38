using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppApi.Features.ShippingAddress.DTOs
{
    public class ShippingAddressDto
    {
        [Required(ErrorMessage = "Số nhà không được để trống")]
        [MaxLength(100, ErrorMessage = "Số nhà không được vượt quá 100 ký tự")]
        public string SoNha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phường/Xã không được để trống")]
        [MaxLength(100, ErrorMessage = "Phường/Xã không được vượt quá 100 ký tự")]
        public string PhuongXa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quận/Huyện không được để trống")]
        [MaxLength(100, ErrorMessage = "Quận/Huyện không được vượt quá 100 ký tự")]
        public string QuanHuyen { get; set; } = string.Empty;

        public Guid IDUser { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Tỉnh/Thành không được để trống")]
        [MaxLength(100, ErrorMessage = "Tỉnh/Thành không được vượt quá 100 ký tự")]
        public string TinhThanh { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Địa điểm gần không được vượt quá 200 ký tự")]
        public string? DiaDiemGan { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(\+84|84|0[3|5|7|8|9])[0-9]{8,9}$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        [MaxLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên người nhận không được để trống")]
        [MaxLength(100, ErrorMessage = "Họ tên người nhận không được vượt quá 100 ký tự")]
        [MinLength(2, ErrorMessage = "Họ tên người nhận phải có ít nhất 2 ký tự")]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        // Thông tin shipping nâng cao
        [MaxLength(50, ErrorMessage = "Mã bưu cục không được vượt quá 50 ký tự")]
        public string? MaBuuCuc { get; set; }

        [MaxLength(100, ErrorMessage = "Tên bưu cục không được vượt quá 100 ký tự")]
        public string? TenBuuCuc { get; set; }

        [Range(0, 1000, ErrorMessage = "Khoảng cách phải từ 0 đến 1000 km")]
        public decimal? KhoangCach { get; set; }

        [Range(1, 168, ErrorMessage = "Thời gian giao hàng phải từ 1 đến 168 giờ")]
        public int? ThoiGianGiaoHang { get; set; }

        [Range(0, 1000000, ErrorMessage = "Phí vận chuyển không hợp lệ")]
        public decimal? PhiVanChuyen { get; set; }

        [MaxLength(50, ErrorMessage = "Đơn vị vận chuyển không được vượt quá 50 ký tự")]
        public string? DonViVanChuyen { get; set; }
    }

    public class ShippingAddressResponseDto
    {
        public Guid Id { get; set; }

        public string DiaChiChiTiet { get; set; } = string.Empty;

        public string SoDienThoai { get; set; } = string.Empty;

        public string HoTenNguoiNhan { get; set; } = string.Empty;

        public DateTime? NgayTao { get; set; }

        public bool TrangThai { get; set; }

        public bool IsDefault { get; set; }

        public Guid IDUser { get; set; }

        // Các trường riêng lẻ để có thể edit
        public string SoNha { get; set; } = string.Empty;
        public string PhuongXa { get; set; } = string.Empty;
        public string QuanHuyen { get; set; } = string.Empty;
        public string TinhThanh { get; set; } = string.Empty;
        public string? DiaDiemGan { get; set; }

        // Thông tin shipping nâng cao
        public string? MaBuuCuc { get; set; }
        public string? TenBuuCuc { get; set; }
        public decimal? KhoangCach { get; set; }
        public int? ThoiGianGiaoHang { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public string? DonViVanChuyen { get; set; }
    }

    /// <summary>
    /// DTO đơn giản để hiển thị danh sách địa chỉ
    /// </summary>
    public class ShippingAddressListDto
    {
        public Guid Id { get; set; }
        public string DiaChiChiTiet { get; set; } = string.Empty;
        public string HoTenNguoiNhan { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool TrangThai { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public int? ThoiGianGiaoHang { get; set; }
    }

    /// <summary>
    /// DTO cho API response với thông báo
    /// </summary>
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
    }

    /// <summary>
    /// DTO cho tính toán phí vận chuyển
    /// </summary>
    public class ShippingCalculationDto
    {
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public Guid ShippingAddressId { get; set; }

        [Required(ErrorMessage = "Tổng tiền hàng không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền hàng phải lớn hơn 0")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Trọng lượng không được để trống")]
        [Range(0.1, 1000, ErrorMessage = "Trọng lượng phải từ 0.1 đến 1000 kg")]
        public decimal Weight { get; set; }

        public string? CouponCode { get; set; }

        public bool IsExpressDelivery { get; set; } = false;
    }

    /// <summary>
    /// DTO cho kết quả tính toán phí vận chuyển
    /// </summary>
    public class ShippingCalculationResultDto
    {
        public decimal BaseShippingFee { get; set; }
        public decimal WeightFee { get; set; }
        public decimal DistanceFee { get; set; }
        public decimal ExpressFee { get; set; }
        public decimal TotalShippingFee { get; set; }
        public int EstimatedDeliveryTime { get; set; } // Giờ
        public string? RecommendedCarrier { get; set; }
        public List<ShippingOptionDto> AvailableOptions { get; set; } = new List<ShippingOptionDto>();
    }

    /// <summary>
    /// DTO cho các tùy chọn vận chuyển
    /// </summary>
    public class ShippingOptionDto
    {
        public string CarrierName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public int DeliveryTime { get; set; } // Giờ
        public string? Description { get; set; }
        public bool IsRecommended { get; set; }
    }

    /// <summary>
    /// DTO cho tracking đơn hàng
    /// </summary>
    public class ShippingTrackingDto
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CurrentLocation { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public List<ShippingTrackingEventDto> Events { get; set; } = new List<ShippingTrackingEventDto>();
    }

    /// <summary>
    /// DTO cho sự kiện tracking
    /// </summary>
    public class ShippingTrackingEventDto
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}