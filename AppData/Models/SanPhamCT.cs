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
        //[JsonIgnore]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey("MauSac")]
        public Guid IDMauSac { get; set; }
        [JsonIgnore]
        public virtual MauSac? MauSac { get; set; }
        [ForeignKey("ChatLieu")]
        public Guid IdChatLieu { get; set; }
        [JsonIgnore]
        public virtual ChatLieu? ChatLieu { get; set; }

        [ForeignKey("SizeAo")]
        public Guid IDSize { get; set; }
        [JsonIgnore]
        public virtual Size? SizeAo { get; set; }

        public int SoLuongTonKho { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GiaBan { get; set; }


        public DateTime? NgayTao { get; set; } 
        public DateTime? NgaySua { get; set; } 
        public bool TrangThai { get; set; }
        [JsonIgnore]
        // 1 sản phẩm có thể có nhiều record giảm giá khác nhau (theo thời gian hoặc sự kiện)
        public virtual ICollection<GiamGiaSPCT> SanPhamGiamGias { get; set; } = new List<GiamGiaSPCT>();
        [JsonIgnore]
        // 1 sản phẩm có thể xuất hiện nhiều lần trong hóa đơn chi tiết
        public virtual ICollection<HoaDonCT> HoaDonChiTiets { get; set; } = new List<HoaDonCT>();
        [JsonIgnore]
        public virtual ICollection<GioHangCT> GioHangChiTiets { get; set; } = new List<GioHangCT>();
    }
}
