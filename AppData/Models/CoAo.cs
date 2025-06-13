using System.ComponentModel.DataAnnotations;

namespace WebModels.Models
{
    public class CoAo
    {
        [Key]
        public Guid IDCoAo { get; set; }

        [Required(ErrorMessage = "Tên cổ áo không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên cổ áo không được vượt quá 100 ký tự.")]
        public string TenCoAo { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Kiểu dáng không được vượt quá 100 ký tự.")]
        public string? KieuDang { get; set; }

        [MaxLength(100, ErrorMessage = "Chất liệu không được vượt quá 100 ký tự.")]
        public string? ChatLieu { get; set; }

        [MaxLength(100, ErrorMessage = "Trang trí không được vượt quá 100 ký tự.")]
        public string? TrangTri { get; set; }

        [MaxLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự.")]
        public string? MauSac { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public DateTime NgaySua { get; set; } = DateTime.UtcNow;

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public CoAo()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
