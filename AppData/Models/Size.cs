using System.ComponentModel.DataAnnotations;

namespace WebModels.Models
{
    public class Size
    {
        [Key]
        public Guid IDSize { get; set; }

        [Required(ErrorMessage = "Số size không được để trống.")]
        [MaxLength(20, ErrorMessage = "Số size không được vượt quá 20 ký tự.")]
        public string SoSize { get; set; } = null!;

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public DateTime NgaySua { get; set; } = DateTime.UtcNow;

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public Size()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
