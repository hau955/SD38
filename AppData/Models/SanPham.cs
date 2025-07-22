using AppData.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class SanPham
    {
        [Key]
        public Guid IDSanPham { get; set; }

        // FK - sản phẩm phải thuộc phân loại  public virtual PhanLoai PhanLoai { get; set; }


        [MaxLength(255)]
        public string TenSanPham { get; set; } = string.Empty;

        public bool? GioiTinh { get; set; }  // Ví dụ: true=Nam, false=Nữ, null=unisex

        public double? TrongLuong { get; set; }

        [MaxLength(2000)]
        public string? MoTa { get; set; }

      

        [NotMapped]
       
        public IFormFile? ImageFile { get; set; }

        public DateTime? NgayTao { get; set; } 

        public DateTime? NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;

        // 1 sản phẩm có thể có nhiều chi tiết (ví dụ size, màu)
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; } = new List<SanPhamCT>();
        [JsonIgnore]
        public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();
       
       
        
        public Guid DanhMucId { get; set; }  // Khóa ngoại đến DanhMuc

        public DanhMuc DanhMuc { get; set; } = null!;
        

    }
}
