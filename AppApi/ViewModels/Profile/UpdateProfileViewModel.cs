using System.ComponentModel.DataAnnotations;

namespace AppApi.ViewModels.Profile
{
    public class UpdateProfileViewModel
    {
        [Required]
        [MaxLength(255)]
        public string HoTen { get; set; } = string.Empty;

        public bool? GioiTinh { get; set; }

        public DateTime? NgaySinh { get; set; }

        [MaxLength(500)]
        public string? DiaChi { get; set; }

        [MaxLength(500)]
        public string? HinhAnh { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
