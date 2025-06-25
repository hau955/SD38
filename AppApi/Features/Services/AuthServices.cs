using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using WebModels.Models;
using AppApi.Features.ViewModels;

namespace AppApi.Features.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthServices(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return ApiResponse<object>.Fail("Email đã được sử dụng.", 409);

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Tạo tài khoản thất bại: {errors}", 500);
            }
            await _userManager.AddToRoleAsync(user, "Customer");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_configuration["Frontend:EmailConfirmationUrl"]}?email={user.Email}&token={encodedToken}";

            var subject = "Xác thực tài khoản của bạn";
            var body = $"<p>Chào bạn,</p><p>Vui lòng xác thực tài khoản bằng cách <a href=\"{confirmationLink}\">bấm vào đây</a>.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return ApiResponse<object>.Success(null, "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.", 201);
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


        public async Task<ApiResponse<object>> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ApiResponse<object>.Fail("Người dùng không tồn tại.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return ApiResponse<object>.Fail("Xác thực email thất bại.");

            return ApiResponse<object>.Success(null, "Xác thực email thành công.");
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetLink = $"{_configuration["Frontend:ResetPasswordUrl"]}?email={model.Email}&token={encodedToken}";

                var subject = "Đặt lại mật khẩu";
                var body = $"<p>Vui lòng <a href=\"{resetLink}\">bấm vào đây</a> để đặt lại mật khẩu của bạn.</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return ApiResponse<object>.Success(null, "Nếu email tồn tại, một liên kết đặt lại mật khẩu đã được gửi.");
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ApiResponse<object>.Fail("Yêu cầu không hợp lệ.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponse<object>.Fail($"Đặt lại mật khẩu thất bại: {errors}");
            }

            return ApiResponse<object>.Success(null, "Đặt lại mật khẩu thành công.");
        }

        private AuthResponseDto GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

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
                Email = user.Email,
                Roles = roles
            };
        }
    }
}
