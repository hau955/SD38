using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class MauSac
    {
        [Key]
        public Guid IDMauSac { get; set; }

        [Required(ErrorMessage = "Tên màu không được để trống.")]
        [MaxLength(50, ErrorMessage = "Tên màu không được vượt quá 50 ký tự.")]
        public string TenMau { get; set; } = null!;

        public DateTime? NgayTao { get; set; } 

        public DateTime? NgaySua { get; set; }

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
