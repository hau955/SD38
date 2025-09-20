using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AppData.Models
{
    public class MauSac
    {
        [Key]
        public Guid IDMauSac { get; set; }

        [Required(ErrorMessage = "Tên màu không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên màu phải có từ 2-50 ký tự.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s\d\-_]+$", ErrorMessage = "Tên màu chỉ được chứa chữ cái, số, dấu gạch ngang và khoảng trắng.")]
        [Display(Name = "Tên màu sắc")]
        public string TenMau { get; set; } = null!;

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; } 

        [Display(Name = "Ngày sửa")]
        public DateTime? NgaySua { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 MauSac có nhiều SanPhamChiTiet
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public MauSac()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
