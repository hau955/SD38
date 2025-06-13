using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebModels.Models
{
    public class HoaDonCT
    {
        [Key]
        public Guid IDHoaDonChiTiet { get; set; }

        // FK đến Hóa đơn (1 hóa đơn có nhiều chi tiết)
        [Required]
        [ForeignKey("HoaDon")]
        public Guid IDHoaDon { get; set; }
        public virtual HoaDon HoaDon { get; set; }

        // FK đến Sản phẩm (1 sản phẩm có thể xuất hiện ở nhiều chi tiết)
        [Required]
        [ForeignKey("SanPham")]
        public Guid IDSanPham { get; set; }
        public virtual SanPham SanPham { get; set; }

        [Required]
        public int SoLuongSanPham { get; set; }

        [Required]
        [MaxLength(255)]
        public string TenSanPham { get; set; } // Lưu tên sản phẩm để tránh join phức tạp (denormalized)

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaSanPham { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaSauGiamGia { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public DateTime NgaySua { get; set; } = DateTime.UtcNow;

        public bool TrangThai { get; set; } = true; // Có thể thêm giá trị mặc định
    }
}
