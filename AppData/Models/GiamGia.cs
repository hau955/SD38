using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class GiamGia
    {
        [Key]
        public Guid IDGiamGia { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenGiamGia { get; set; } = string.Empty; // Ví dụ: "SUMMER20"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTri { get; set; }   // Có thể là % hoặc số tiền

        [Required]
        [MaxLength(20)]
        public string LoaiGiamGia { get; set; } = "PhanTram"; // "PhanTram" | "SoTien"
        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriGiamToiDa { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgaySua { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // Navigation
        [JsonIgnore]
        public virtual ICollection<GiamGiaSanPham> GiamGiaSanPhams { get; set; } = new List<GiamGiaSanPham>();

        [JsonIgnore]
        public virtual ICollection<GiamGiaDanhMuc> GiamGiaDanhMucs { get; set; } = new List<GiamGiaDanhMuc>();
        [JsonIgnore]
        public virtual ICollection<GiamGiaSPCT> GiamGiaSPCTs { get; set; } = new List<GiamGiaSPCT>();
    }
}
