using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Admin.ViewModels.EmployeeManagerment
{
    public class AddEmployeeViewModel
    {
        [Required]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public bool? Gender { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public IFormFile AvatarFile { get; set; } = null!; // Sử dụng null! để tránh cảnh báo nullability, sẽ được kiểm tra khi submit form
    }

}
