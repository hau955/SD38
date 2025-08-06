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
}