using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebModels.Models
{
    public class HoaDon
    {
        [Key]
        public Guid IDHoaDon { get; set; }

        // Hóa đơn thuộc về 1 User (Khách hàng)
        [Required]
        [ForeignKey("User")]
        public Guid IDUser { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Hóa đơn sử dụng 1 địa chỉ nhận hàng trong số các địa chỉ của User
        [Required]
        [ForeignKey("DiaChiNhanHang")]
        public Guid IDDiaChiNhanHang { get; set; }
        public virtual DiaChiNhanHang DiaChiNhanHang { get; set; }

        // Hóa đơn sử dụng 1 hình thức thanh toán trong số các hình thức của User
        [Required]
        [ForeignKey("HinhThucTT")]
        public Guid IDHinhThucTT { get; set; }
        public virtual HinhThucTT HinhThucTT { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTienTruocGiam { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTienSauGiam { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TienGiam { get; set; }

        public DateTime? NgayThanhToan { get; set; }

        [MaxLength(1000)]
        public string? GhiChu { get; set; }

        public float? PhanTramGiamGiaHoaDon { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TienGiamHoaDon { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThaiDonHang { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThaiThanhToan { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime NgaySua { get; set; } 

        // 1 Hóa đơn có nhiều chi tiết hóa đơn
        public virtual ICollection<HoaDonCT> HoaDonChiTiets { get; set; } = new List<HoaDonCT>();
    }
}
