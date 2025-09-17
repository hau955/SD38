using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class GiamGiaSanPham
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid IDGiamGia { get; set; }
        [ForeignKey("IDGiamGia")]
        public virtual GiamGia GiamGia { get; set; } = null!;

        [Required]
        public Guid IDSanPham { get; set; }
        [ForeignKey("IDSanPham")]
        public virtual SanPham SanPham { get; set; } = null!;
    }
}
