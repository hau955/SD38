
using AppView.Areas.Auth.Repository;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppData.Models;
using AppApi.Features.Auth.DTOs;

namespace AppView.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthRepository authRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // ========== Register ==========
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authRepository.RegisterAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Đăng ký thất bại.");
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }

        // ========== Login ==========
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return PartialView("_LoginPartial", model);


            var result = await _authRepository.LoginAsync(model);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Đăng nhập thất bại.");
                return PartialView("_LoginPartial", model);
            }

           

            TempData["Success"] = "Đăng nhập thành công";

            if (result.Data.Roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "SanPham", new { area = "Admin" });
            }
            HttpContext.Session.SetString("Token", result.Data.Token);
            HttpContext.Session.SetString("Email", result.Data.Email);
            HttpContext.Session.SetString("Roles", string.Join(",", result.Data.Roles));
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authRepository.ForgotPasswordAsync(model);
            TempData["Message"] = result.Message;
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordDto { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authRepository.ResetPasswordAsync(model);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message ?? "Đặt lại mật khẩu thất bại.");
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Liên kết xác nhận không hợp lệ.";
                return View();
            }

            var result = await _authRepository.ConfirmEmailAsync(email, token);
            ViewBag.Message = result.IsSuccess ? "✅ Xác nhận email thành công!" : $"❌ {result.Message}";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}
