using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebModels.Models
{
    public class GioHangCT
    {
        [Key]
        public Guid IDGioHangChiTiet { get; set; }

        [Required]
        public Guid IDGioHang { get; set; }
        [ForeignKey(nameof(IDGioHang))]
        public virtual GioHang GioHang { get; set; }

        [Required]
        public Guid IDSanPham { get; set; }
        [ForeignKey(nameof(IDSanPham))]
        public virtual SanPham SanPham { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DonGia { get; set; }

        public bool TrangThai { get; set; } = true; // mặc định true (hoạt động)
    }
}
