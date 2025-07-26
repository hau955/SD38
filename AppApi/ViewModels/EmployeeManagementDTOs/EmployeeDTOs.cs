using System.ComponentModel.DataAnnotations;

namespace AppApi.ViewModels.EmployeeManagementDTOs
{
    public class AddEmployeeDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự.")]
        public string? Address { get; set; }

        public IFormFile? AvatarFile { get; set; }
    }
    public class UpdateEmployeeDto
    {
        [Required(ErrorMessage = "ID không được để trống.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự.")]
        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
    }
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "UserId không được để trống.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        public EmployeeRole Role { get; set; }
    }

    public class EmployeeDetailDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? AvatarUrl { get; set; }

        public bool? Gender { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }

    public class EmployeeListDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "UserId không được để trống.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 đến 100 ký tự.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public enum EmployeeRole
    {
        Admin,
        Employee,
        Customer
    }
}
