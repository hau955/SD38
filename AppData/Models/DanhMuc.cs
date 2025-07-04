using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebModels.Models;

namespace AppData.Models
{
    public class DanhMuc
    {
        [Key]
        public Guid DanhMucId { get; set; }  // hoặc dùng int nếu bạn muốn int

        [Required]
        [MaxLength(100)]
        public string TenDanhMuc { get; set; } = null!;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

       

        // Quan hệ 1 - N: 1 danh mục có nhiều sản phẩm
        [JsonIgnore]
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
