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

        [Required]
        [MaxLength(255)]
        public string TenSanPham { get; set; }

        public bool? GioiTinh { get; set; }  // Ví dụ: true=Nam, false=Nữ, null=unisex

        public double? TrongLuong { get; set; }

        [MaxLength(2000)]
        public string? MoTa { get; set; }

        [MaxLength(500)]
        public string? HinhAnh { get; set; }

        [NotMapped]
       
        public IFormFile? ImageFile { get; set; }

        public DateTime NgayTao { get; set; } 

        public DateTime NgaySua { get; set; } 

        public bool TrangThai { get; set; } = true;

        // 1 sản phẩm có thể có nhiều chi tiết (ví dụ size, màu)
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; } = new List<SanPhamCT>();
        [JsonIgnore]
        // 1 sản phẩm có thể có nhiều record giảm giá khác nhau (theo thời gian hoặc sự kiện)
        public virtual ICollection<SanPhamGG> SanPhamGiamGias { get; set; } = new List<SanPhamGG>();
        [JsonIgnore]
        // 1 sản phẩm có thể xuất hiện nhiều lần trong hóa đơn chi tiết
        public virtual ICollection<HoaDonCT> HoaDonChiTiets { get; set; } = new List<HoaDonCT>();
        [JsonIgnore] 
        public virtual ICollection<GioHangCT> GioHangChiTiets { get; set; } = new List<GioHangCT>();
        public Guid DanhMucId { get; set; }  // Khóa ngoại đến DanhMuc

        public DanhMuc DanhMuc { get; set; } = null!;

    }
}
