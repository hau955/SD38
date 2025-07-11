﻿using AppApi.Features.DTOs;
using System.Text.Json;

namespace AppView.Areas.Auth.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        public AuthRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<object>> RegisterAsync(RegisterDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/register", model);
            return await ParseApiResponse<object>(response);
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/login", model);
            return await ParseApiResponse<AuthResponseDto>(response);
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/forgot-password", model);
            return await ParseApiResponse<object>(response);
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/reset-password", model);
            return await ParseApiResponse<object>(response);
        }

        public async Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token)
        {
            var emailEncoded = Uri.EscapeDataString(email);
            var tokenEncoded = Uri.EscapeDataString(token);
            var url = $"/api/auths/confirm-email?email={emailEncoded}&token={tokenEncoded}";

            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result ?? ApiResponse<object>.Fail("Lỗi xác nhận.");
        }



        private async Task<ApiResponse<T>> ParseApiResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
    }
}
