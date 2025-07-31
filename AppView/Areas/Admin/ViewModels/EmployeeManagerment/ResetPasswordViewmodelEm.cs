using System.ComponentModel.DataAnnotations;
namespace AppView.Areas.Admin.ViewModels.EmployeeManagerment
{
    public class ResetPasswordViewmodelEm
    {
        [Required(ErrorMessage = "UserId không được để trống.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6 đến 100 ký tự.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
