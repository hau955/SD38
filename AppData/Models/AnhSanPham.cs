using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class AnhSanPham
    {
        [Key]
        public Guid IdAnh { get; set; }  // hoặc dùng int nếu bạn muốn int
        [ForeignKey("SanPham")]
        public Guid IDSanPham { get; set; }
        [JsonIgnore]
        public virtual SanPham? SanPham { get; set; }

        [MaxLength(500)]
        public string DuongDanAnh { get; set; }

        public bool AnhChinh { get; set; }


        
       
    }
}
