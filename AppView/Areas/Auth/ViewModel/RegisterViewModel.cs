using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Auth.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Tên người dùng phải có ít nhất 2 ký tự")]
        public string FullName { get; set; }

        [Required]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public bool IsMale { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-18); // Mặc định 18 tuổi
        public string? Address { get; set; }
    }
  

}
