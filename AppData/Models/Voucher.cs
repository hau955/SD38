using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class Voucher
    {
        [Key]
        public Guid IdVoucher { get; set; }

        [Required]
        [MaxLength(50)]
        public string CodeVoucher { get; set; } // Mã code giảm giá

        public string? MoTa { get; set; }

        public double? PhanTram { get; set; } // % giảm
        public decimal? SoTienGiam { get; set; } // Giảm cố định

        public decimal? DieuKienToiThieu { get; set; } // Giá trị đơn hàng tối thiểu

        public int? SoLuong { get; set; } // Số lượng voucher
        public int? SoLanSuDungToiDa { get; set; } // Mỗi KH được dùng tối đa X lần

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual HoaDon? HoaDons { get; set; }
    }
}
