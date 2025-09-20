using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AppData.Models
{
    public class Size
    {
        [Key]
        public Guid IDSize { get; set; }

        [Required(ErrorMessage = "Số size không được để trống.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Số size phải có từ 1-20 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\-\/\.]+$", ErrorMessage = "Số size chỉ được chứa chữ cái, số, dấu gạch ngang, dấu gạch chéo và dấu chấm.")]
        [Display(Name = "Số size")]
        public string SoSize { get; set; } = null!;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Ngày sửa")]
        public DateTime NgaySua { get; set; } 

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;
        
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public Size()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
