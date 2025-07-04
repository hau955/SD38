using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebModels.Models
{
    public class TaAo
    {
        [Key]
        public Guid IDTaAo { get; set; }

        [Required(ErrorMessage = "Tên tà áo không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên tà áo không được vượt quá 100 ký tự.")]
        public string TenTaAo { get; set; } = null!;

        public int? ChieuDaiTaAo { get; set; }

        public int? SoLuongTaAo { get; set; }

        [MaxLength(100, ErrorMessage = "Đường may không được vượt quá 100 ký tự.")]
        public string? DuongMayTaAo { get; set; }

        [MaxLength(100, ErrorMessage = "Kiểu tà không được vượt quá 100 ký tự.")]
        public string? KieuTa { get; set; }

        [MaxLength(100, ErrorMessage = "Chất liệu không được vượt quá 100 ký tự.")]
        public string? ChatLieu { get; set; }

        [MaxLength(100, ErrorMessage = "Trang trí không được vượt quá 100 ký tự.")]
        public string? TrangTri { get; set; }

        [MaxLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự.")]
        public string? MauSac { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public TaAo()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
