using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebModels.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(255)]
        public string HoTen { get; set; } = string.Empty;

        public bool? GioiTinh { get; set; } // Không bắt buộc

        public DateTime? NgaySinh { get; set; } // Không bắt buộc

        [MaxLength(500)]
        public string? DiaChi { get; set; } // Không bắt buộc

        [MaxLength(500)]
        public string? HinhAnh { get; set; } // Không bắt buộc

        [Required]
        public bool TrangThai { get; set; } = true;

       

        // Quan hệ 1 - 1 với Giỏ hàng
        public virtual GioHang? GioHang { get; set; }

        // Quan hệ 1 - N với Hóa đơn
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

        // Quan hệ 1 - N với Địa chỉ nhận hàng
        public virtual ICollection<DiaChiNhanHang> DiaChiNhanHangs { get; set; } = new List<DiaChiNhanHang>();
    }
}
