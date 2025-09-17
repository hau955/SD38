using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppData.Models
{
    public class GiamGiaSPCT
    {
        [Key]
        public Guid ID { get; set; }

        public Guid? IDSanPhamCT { get; set; }

        [ForeignKey(nameof(IDSanPhamCT))]
        public virtual SanPhamCT? SanPhamCT { get; set; }

        [Required]
        public Guid IDGiamGia { get; set; }

        [ForeignKey(nameof(IDGiamGia))]
        public virtual GiamGia GiamGia { get; set; }

       
    }
}
 