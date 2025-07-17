using AppView.Areas.Admin;
using AppView.Areas.Auth.ViewModel;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace AppView.Areas.Auth.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthRepository> _logger;
        public AuthRepository(HttpClient httpClient, ILogger<AuthRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<ApiResponse<object>> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/register", model);
            return await ParseApiResponse<object>(response);
        }
        public async Task<ApiResponse<AuthResponseViewModel>> LoginAsync(LoginViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Login failed: {errorContent}");
                    return ApiResponse<AuthResponseViewModel>.Fail("Đăng nhập thất bại", (int)response.StatusCode);
                }

                var result = await ParseApiResponse<AuthResponseViewModel>(response);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling login API");
                return ApiResponse<AuthResponseViewModel>.Fail("Lỗi kết nối đến server");
            }
        }
        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/forgot-password", model);
            return await ParseApiResponse<object>(response);
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/reset-password", model);
            return await ParseApiResponse<object>(response);
        }
        public async Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                // Encode lại email và token để đảm bảo an toàn
                var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(email));
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var url = $"https://localhost:7221/api/auths/confirm-email" +
                         $"?email={encodedEmail}" +
                         $"&token={encodedToken}";

                _logger.LogInformation($"Calling API: {url}");

                var response = await _httpClient.GetAsync(url);
                return await ParseApiResponse<object>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API xác nhận email");
                return ApiResponse<object>.Fail($"Lỗi kết nối API: {ex.Message}");
            }
        }
        private async Task<ApiResponse<T>> ParseApiResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var result = JsonSerializer.Deserialize<ApiResponse<T>>(json, options);

                if (result == null)
                    return ApiResponse<T>.Fail("Không thể đọc phản hồi từ API.");

                if (!result.IsSuccess)
                {
                    return ApiResponse<T>.Fail(result.Message ?? "Lỗi từ API", (int?)response.StatusCode ?? 400);
                }

                return result;
            }
            catch (JsonException ex)
            {
                return ApiResponse<T>.Fail($"Lỗi khi phân tích JSON: {ex.Message}. Raw: {json}");
            }
        }

        public async Task<ApiResponse<object>> ResendConfirmationEmailAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/auths/resend-confirmation", new { email });
            return await ParseApiResponse<object>(response);
        }

        public Task<ApiResponse<object>> LogoutAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
