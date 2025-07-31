using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Auth.ViewModel
{
    public class AuthResponseViewModel
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Email { get; set; }
        public string hoten { get; set; }
        public string? hinhanh { get; set; }
        public IList<string> Roles { get; set; }
       
      
    }
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmNewPassword { get; set; }
    }
}
