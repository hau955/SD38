using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppData.Models
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
       
        [MaxLength(500)]
        public string? DiaChiChiTiet { get; set; } 

       
        [MaxLength(20)]
        public string SoDienThoai { get; set; } 

       
        [MaxLength(100)]
        public string? HoTenNguoiNhan { get; set; } 

        
        public DateTime? NgayTao { get; set; } 
        public bool IsDefault { get; set; } = false; // Địa chỉ mặc định cho người dùng

        public DateTime? NgaySua { get; set; } 

        [Required]
        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 - N với HoaDon
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
