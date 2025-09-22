using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Admin.ViewModels
{
    public class VoucherVM
    {
        public Guid IdVoucher { get; set; }

        [Required(ErrorMessage = "Mã code không được để trống")]
        [StringLength(20, ErrorMessage = "Mã code tối đa 20 ký tự")]
        public string CodeVoucher { get; set; } = null!;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập % giảm")]
        [Range(1, 100, ErrorMessage = "Phần trăm phải từ 1 đến 100")]
        public double? PhanTram { get; set; }

        [Range(1000, int.MaxValue, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn 1000đ")]
        public decimal? SoTienGiam { get; set; } // ✅ giảm tối đa, optional

        [Range(0, int.MaxValue, ErrorMessage = "Điều kiện tối thiểu không hợp lệ")]
        public decimal? DieuKienToiThieu { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        public int? SoLuong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndDate { get; set; }
    }
}
