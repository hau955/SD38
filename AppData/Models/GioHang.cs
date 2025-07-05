using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppData.Models
{
    public class GioHang
    {
        [Key]
        [ForeignKey("User")]
        public Guid IDGioHang { get; set; }

        // Quan hệ 1 - 1 với ApplicationUser
        public virtual ApplicationUser User { get; set; }

        public bool TrangThai { get; set; }

        // Quan hệ 1 - n với GioHangChiTiet
        public virtual ICollection<GioHangCT> GioHangChiTiets { get; set; } = new List<GioHangCT>();
    }
}
