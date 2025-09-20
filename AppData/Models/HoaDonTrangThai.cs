using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppData.Models
{
    public class HoaDonTrangThai
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("HoaDon")]
        public Guid IDHoaDon { get; set; }
        public virtual HoaDon HoaDon { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? NguoiCapNhat { get; set; }
    }
}
