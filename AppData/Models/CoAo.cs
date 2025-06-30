using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        public DateTime NgayTao { get; set; } 

        public DateTime NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public CoAo()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
