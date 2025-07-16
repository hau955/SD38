using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppData.Models
{
    public class ChatLieu
    {
        [Key]
        public Guid IDChatLieu { get; set; }

        [Required(ErrorMessage = "Tên chất liệu không được để trống.")]
        [MaxLength(50, ErrorMessage = "Tên chất liệu không được vượt quá 50 ký tự.")]
        public string TenChatLieu { get; set; } = null!;

        public DateTime? NgayTao { get; set; }

        public DateTime? NgaySua { get; set; }

        public bool TrangThai { get; set; } = true;

        // Quan hệ 1 chatlieu có nhiều SanPhamChiTiet
        [JsonIgnore]
        public virtual ICollection<SanPhamCT> SanPhamChiTiets { get; set; }

        public ChatLieu()
        {
            SanPhamChiTiets = new HashSet<SanPhamCT>();
        }
    }
}
