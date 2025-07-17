using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using AppApi.Features.Auth.DTOs;
using AppData.Models;

namespace AppApi.Features.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthServices> _logger;
        public AuthServices(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService, ILogger<AuthServices> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterDto model)
        {
            try
            {
                var email = model.Email?.Trim().ToLower() ?? string.Empty;
                var userByEmail = await _userManager.FindByEmailAsync(email);
                var userByUserName = await _userManager.FindByNameAsync(email);

                if (userByEmail != null || userByUserName != null)
                {
                    return ApiResponse<object>.Fail("Email đã được sử dụng. Vui lòng chọn email khác.", 400);
                }

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    DiaChi = model.Address,
                    HoTen = model.FullName,
                    SoDienThoai = model.PhoneNumber,
                    GioiTinh = model.Gender,
                    NgaySinh = model.DateOfBirth,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return ApiResponse<object>.Fail($"Đăng ký thất bại: {errors}", 400);
                }

                await _userManager.AddToRoleAsync(user, "Customer");

                await SendConfirmationEmailAsync(user);
                return ApiResponse<object>.Success(new object(), "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Không thể gửi email xác nhận: {ex.Message}", 500);
            }
        }
        public async Task<ApiResponse<object>> SendConfirmationEmailAsync(ApplicationUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));

                // Sửa lại URL - loại bỏ "Auth" thừa
                var confirmationLink = $"{_configuration["Frontend:EmailConfirmationUrl"]}"
                      + $"?email={encodedEmail}"
                      + $"&token={encodedToken}";

                _logger.LogInformation($"[CONFIRMATION] Token: {encodedToken}");
                _logger.LogInformation($"[CONFIRMATION] Link: {confirmationLink}");

                var emailBody = $@"
<p>Nhấn vào link sau để xác nhận email:</p>
<a href='{confirmationLink}'>XÁC NHẬN EMAIL</a>
<p>Hoặc mở thủ công: {confirmationLink}</p>";

                await _emailService.SendEmailAsync(user.Email ?? string.Empty, "Xác nhận email", emailBody);

                return ApiResponse<object>.Success(new object(), "Đã gửi email xác nhận.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email xác nhận");
                return ApiResponse<object>.Fail("Không thể gửi email xác nhận", 500);
            }
        }
        public async Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                _logger.LogInformation("===== XÁC NHẬN EMAIL =====");
                _logger.LogInformation($"Email nhận (mã hóa): {email}");
                _logger.LogInformation($"Token nhận: {token}");

                // ✅ GIẢI MÃ email từ Base64Url
                try
                {
                    var emailBytes = WebEncoders.Base64UrlDecode(email);
                    email = Encoding.UTF8.GetString(emailBytes);
                    _logger.LogInformation($"Email sau khi giải mã: {email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Giải mã email thất bại.");
                    return ApiResponse<object>.Fail("Liên kết xác nhận không hợp lệ", 400);
                }

                // ✅ GIẢI MÃ token như bạn đã làm
                string decodedToken;
                try
                {
                    var tokenBytes = WebEncoders.Base64UrlDecode(token);
                    decodedToken = Encoding.UTF8.GetString(tokenBytes);
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, "❌ Token không hợp lệ (decode thất bại)");
                    return ApiResponse<object>.Fail("Token xác nhận không hợp lệ", 400);
                }

                // ✅ Tìm user với email đã giải mã
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy người dùng với email này.");
                    return ApiResponse<object>.Fail("Tài khoản không tồn tại", 404);
                }

                // 3. Kiểm tra đã xác nhận chưa
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogInformation("✅ Email đã được xác nhận trước đó.");
                    return ApiResponse<object>.Success(new object(), "Email đã được xác nhận trước đó");
                }

                // 4. Xác nhận token
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    _logger.LogInformation("✅ Xác nhận email thành công.");
                    return ApiResponse<object>.Success(new object(), "Xác nhận email thành công");
                }

                // 5. Nếu xác nhận thất bại → log tất cả lỗi
                var errorMessages = result.Errors.Select(e => e.Description).ToList();
                foreach (var err in errorMessages)
                {
                    _logger.LogError("❌ Xác nhận thất bại: " + err);
                }

                return ApiResponse<object>.Fail("Xác nhận thất bại: " + string.Join(" | ", errorMessages), 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi hệ thống khi xác nhận email.");
                return ApiResponse<object>.Fail("Lỗi hệ thống khi xác nhận email", 500);
            }
        }

        public async Task<ApiResponse<object>> ResendConfirmEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return ApiResponse<object>.Fail("Vui lòng nhập email.", 400);
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<object>.Fail("Email không tồn tại.", 404);
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return ApiResponse<object>.Fail("Email đã được xác nhận.", 400);
            }

            return await SendConfirmationEmailAsync(user);
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Return success even if user doesn't exist to prevent email enumeration
                return ApiResponse<object>.Success(new object(), "Nếu email tồn tại, một liên kết đặt lại mật khẩu đã được gửi.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{_configuration["Frontend:ResetPasswordUrl"]}?email={model.Email}&token={encodedToken}";

            var subject = "Đặt lại mật khẩu";
            var body = $@"
        <h2>Đặt lại mật khẩu</h2>
        <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản tại Áo Dài Việt.</p>
        <p>Vui lòng nhấn vào nút bên dưới để đặt lại mật khẩu của bạn:</p>
        <p style='text-align: center; margin: 30px 0;'>
            <a href='{resetLink}' style='background-color: #fb923c; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>
                Đặt Lại Mật Khẩu
            </a>
        </p>
        <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        <p>Liên kết này sẽ hết hạn sau 24 giờ.</p>";

            await _emailService.SendEmailAsync(user.Email ?? string.Empty, subject, body);
            return ApiResponse<object>.Success(new object(), "Nếu email tồn tại, một liên kết đặt lại mật khẩu đã được gửi.");
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Return success even if user doesn't exist to prevent email enumeration
                return ApiResponse<object>.Success(new object(), "Đặt lại mật khẩu thành công.");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Đặt lại mật khẩu thất bại: {errors}", 400);
            }

            return ApiResponse<object>.Success(new object(), "Đặt lại mật khẩu thành công.");
        }
        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return ApiResponse<AuthResponseDto>.Fail("Tên đăng nhập hoặc mật khẩu không chính xác.", 401);

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return ApiResponse<AuthResponseDto>.Fail("Vui lòng xác thực email của bạn trước khi đăng nhập.", 403);

            var roles = await _userManager.GetRolesAsync(user);
            var authResponse = GenerateJwtToken(user, roles);

            return ApiResponse<AuthResponseDto>.Success(authResponse);
        }
        private AuthResponseDto GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["Jwt:Key"] ?? string.Empty;
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Email = user.Email ?? string.Empty,
                Roles = roles
            };
        }
    }
}