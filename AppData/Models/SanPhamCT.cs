using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class SanPhamCT
    {
        [Key]
        public Guid IDSanPhamCT { get; set; }

        [ForeignKey("SanPham")]
        public Guid IDSanPham { get; set; }
        [JsonIgnore]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("MauSac")]
        public Guid IDMauSac { get; set; }
        [JsonIgnore]
        public virtual MauSac? MauSac { get; set; }

        [ForeignKey("SizeAo")]
        public Guid IDSize { get; set; }
        [JsonIgnore]
        public virtual Size? SizeAo { get; set; }

        [ForeignKey("CoAo")]
        public Guid IDCoAo { get; set; }
        [JsonIgnore]
        public virtual CoAo? CoAo { get; set; }

        [ForeignKey("TaAo")]
        public Guid IDTaAo { get; set; }
        [JsonIgnore]
        public virtual TaAo? TaAo { get; set; }

        public int SoLuongTonKho { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaBan { get; set; }


        public DateTime NgayTao { get; set; } 
        public DateTime NgaySua { get; set; } 
        public bool TrangThai { get; set; }
    }
}
