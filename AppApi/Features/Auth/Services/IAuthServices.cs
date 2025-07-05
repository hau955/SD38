

using AppApi.Features.Auth.DTOs;

namespace AppApi.Features.Services
{
    public interface IAuthServices
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterDto model);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model);
        Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model);
    }
}
