using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppData.Models
{
    public class SanPhamGG
    {
        [Key]
        public Guid IDSPGiamGia { get; set; }

        [Required]
        public Guid IDSanPham { get; set; }

        [ForeignKey(nameof(IDSanPham))]
        public virtual SanPham SanPham { get; set; }

        [Required]
        public Guid IDGiamGia { get; set; }

        [ForeignKey(nameof(IDGiamGia))]
        public virtual GiamGia GiamGia { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DonGia { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SoTienConLai { get; set; }

        
        public DateTime NgayTao { get; set; } 

       
        public DateTime NgaySua { get; set; } 

        
        public bool TrangThai { get; set; }
    }
}
