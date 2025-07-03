using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class HinhThucTT
    {
        [Key]
        public Guid IDHinhThucTT { get; set; }

        [Required(ErrorMessage = "Tên hình thức thanh toán không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên hình thức thanh toán không được vượt quá 100 ký tự.")]
        public string TenHinhThucTT { get; set; } = null!;  // Đảm bảo không null

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? MoTa { get; set; }

        public DateTime NgayTao { get; set; } 
        public DateTime NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;

        // Navigation property
        public virtual ICollection<HoaDon> HoaDons { get; set; }

        public HinhThucTT()
        {
            HoaDons = new HashSet<HoaDon>(); // Khởi tạo collection
        }
    }
}
