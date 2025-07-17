using AppView.Areas.Auth.ViewModel;
using AppView.Areas.Admin;
namespace AppView.Areas.Auth.Repository
{
    public interface IAuthRepository
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterViewModel model);
        Task<ApiResponse<AuthResponseViewModel>> LoginAsync(LoginViewModel model);
        Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<ApiResponse<object>> ResendConfirmationEmailAsync(string email);
        Task<ApiResponse<object>> LogoutAsync(string userId);
    }
}
