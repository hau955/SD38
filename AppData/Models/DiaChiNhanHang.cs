using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebModels.Models
{
    public class DiaChiNhanHang
    {
        [Key]
        public Guid IDDiaChiNhanHang { get; set; }

        // Quan hệ N - 1 với ApplicationUser
        [Required]
        [ForeignKey("User")]
        public Guid IDUser { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Thông tin địa chỉ nhận hàng
        [Required]
        [MaxLength(500)]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        [Required]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime NgaySua { get; set; } = DateTime.UtcNow;

        [Required]
        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 - N với HoaDon
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
