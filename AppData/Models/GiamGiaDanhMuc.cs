using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class GiamGiaDanhMuc
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid IDGiamGia { get; set; }
        [ForeignKey("IDGiamGia")]
        public virtual GiamGia GiamGia { get; set; } = null!;

        [Required]
        public Guid DanhMucId { get; set; }
        [ForeignKey("DanhMucId")]
        public virtual DanhMuc DanhMuc { get; set; } = null!;
    }
}
