﻿

using AppApi.Features.Auth.DTOs;
using AppData.Models;

namespace AppApi.Features.Services
{
    public interface IAuthServices
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterDto model);
        Task<ApiResponse<object>> RegisterAdminAsync(RegisterDto model);
        Task<ApiResponse<object>> RegisterEmPloyee(RegisterDto model);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model);
        Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model);
        Task<ApiResponse<object>> ResendConfirmEmailAsync(string email);
    }
}
