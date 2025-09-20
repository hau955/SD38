using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AppData.Models
{
    public class ChatLieu
    {
        [Key]
        public Guid IDChatLieu { get; set; }

        [Required(ErrorMessage = "Tên chất liệu không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên chất liệu phải có từ 2-50 ký tự.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s\d\-_]+$", ErrorMessage = "Tên chất liệu chỉ được chứa chữ cái, số, dấu gạch ngang và khoảng trắng.")]
        [Display(Name = "Tên chất liệu")]
        public string TenChatLieu { get; set; } = null!;

        [Display(Name = "Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [Display(Name = "Ngày sửa")]
        public DateTime? NgaySua { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 chatlieu có nhiều SanPhamChiTiet
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public ChatLieu()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
