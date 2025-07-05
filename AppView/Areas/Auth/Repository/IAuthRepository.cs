using AppApi.Features.Auth.DTOs;

namespace AppView.Areas.Auth.Repository
{
    public interface IAuthRepository
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterDto model);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model);
        Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model);
    }
}
