using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppData.Models
{
    public class GiamGia
    {
        [Key]
        public Guid IDGiamGia { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenMaGiamGia { get; set; } // VD: "SUMMER20", "NEWUSER"

        [Range(0, 1)]
        public float? GiamTheoPhanTram { get; set; } // Ví dụ: 0.1 cho 10%

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue)]
        public decimal? GiamTheoTien { get; set; } // Số tiền giảm cố định

        [MaxLength(500)]
        public string? DieuKienGiamGia { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        public bool LoaiGiamGia { get; set; } // false = Percentage, true = Fixed Amount

       
        public DateTime NgayTao { get; set; } 

      
        public DateTime NgaySua { get; set; } 

      
        public bool TrangThai { get; set; } // Active, Expired, Draft, v.v.

        // Navigation property
        public virtual ICollection<SanPhamGG> SanPhamGiamGias { get; set; } = new List<SanPhamGG>();
    }
}
