using AppData.Models;
using AppData.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HoaDon
{
    [Key]
    public Guid IDHoaDon { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid IDUser { get; set; }
    public virtual ApplicationUser User { get; set; }

    [Required]
    [ForeignKey("DiaChiNhanHang")]
    public Guid IDDiaChiNhanHang { get; set; }
    public virtual DiaChiNhanHang DiaChiNhanHang { get; set; }

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
    public TrangThaiDonHang TrangThaiDonHang { get; set; }

    [Required]
    public TrangThaiThanhToan TrangThaiThanhToan { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgaySua { get; set; }

    public virtual ICollection<HoaDonCT> HoaDonChiTiets { get; set; } = new List<HoaDonCT>();
}
