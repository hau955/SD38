﻿using AppData.Models;
using AppView.Areas.Auth.Repository;
using AppView.Areas.Auth.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
namespace AppView.Areas.Auth.Controllers
{
    [Area("Auth")]
    [Route("[area]/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IAuthRepository authRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // ========== Register ==========
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng kiểm tra lại thông tin nhập.",
                    errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                });
            }

            var result = await _authRepository.RegisterAsync(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors,
                showLoginModal = result.IsSuccess
            });
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = "Thông tin không hợp lệ.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                });
            }

            var result = await _authRepository.LoginAsync(model);

            if (!result.IsSuccess)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = result.Message ?? "Đăng nhập thất bại.",
                    RequiresConfirmation = result.Message?.Contains("xác nhận email") ?? false,
                    Email = model.Email
                });
            }

            // Lưu session
            HttpContext.Session.SetString("Token", result.Data.Token ?? "");
            HttpContext.Session.SetString("Email", result.Data.Email ?? "");
            HttpContext.Session.SetString("HinhAnh", result.Data.hinhanh ?? "/admin/assets/img/avatars/default.png");
            HttpContext.Session.SetString("HoTen", result.Data.hoten ?? "");
            HttpContext.Session.SetString("ID", result.Data.Id.ToString() ?? "");
            HttpContext.Session.SetString("Roles", result.Data.Roles != null ? string.Join(",", result.Data.Roles) : "");

            return Json(new
            {
                IsSuccess = true,
                Email = result.Data.Email,
                IsAdmin = result.Data.Roles.Contains("Admin"),
                RedirectUrl = result.Data.Roles.Contains("Admin")
                    ? Url.Action("Index", "SanPham", new { area = "Admin" })
                    : result.Data.Roles.Contains("Employee")
                        ? Url.Action("Index", "Home", new { area = "Employee" })
                        : Url.Action("Index", "Home", new { area = "" })
            });
        }
        // ========== Forgot Password ==========
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ForgotPasswordPartial", model);

            var result = await _authRepository.ForgotPasswordAsync(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }

        // ========== Reset Password ==========
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authRepository.ResetPasswordAsync(model);

            if (result.IsSuccess)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Login");
            }

            TempData["Error"] = result.Message;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (!Request.Query.ContainsKey("email") || !Request.Query.ContainsKey("token"))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToAction("ConfirmEmailResult", new { success = false, message = "Thiếu thông tin xác nhận" });
                }

                // Giải mã
                string decodedEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email));
                string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

                var result = await _authRepository.ConfirmEmailAsync(decodedEmail, decodedToken);

                return RedirectToAction("ConfirmEmailResult", new
                {
                    success = result.IsSuccess,
                    email = decodedEmail,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xác nhận email");
                return RedirectToAction("ConfirmEmailResult", new
                {
                    success = false,
                    message = "Lỗi hệ thống khi xác nhận email"
                });
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmailResult(bool success, string email, string message = null)
        {
            if (success && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Success = success;
            ViewBag.Email = email;
            ViewBag.Message = message;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email không được để trống" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Email không tồn tại trong hệ thống" });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return Json(new { success = false, message = "Email đã được xác nhận trước đó" });
            }

            var result = await _authRepository.ResendConfirmationEmailAsync(email);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.Message
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();

                // Xóa cookie authentication
                if (HttpContext.Request.Cookies[".AspNetCore.Identity.Application"] != null)
                    HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

                // Xóa JWT token nếu có
                if (HttpContext.Request.Cookies["Token"] != null)
                    HttpContext.Response.Cookies.Delete("Token");

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home", new { area = "" }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi logout");
                return Json(new { success = false, message = "Đăng xuất thất bại" });
            }
        }
    }
}