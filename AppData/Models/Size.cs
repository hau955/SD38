using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebModels.Models
{
    public class Size
    {
        [Key]
        public Guid IDSize { get; set; }

        [Required(ErrorMessage = "Số size không được để trống.")]
        [MaxLength(20, ErrorMessage = "Số size không được vượt quá 20 ký tự.")]
        public string SoSize { get; set; } = null!;

        public DateTime NgayTao { get; set; }

        public DateTime NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public Size()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
