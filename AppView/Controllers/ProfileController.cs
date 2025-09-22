using AppApi.ViewModels.Profile;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly IProfileRepo _profileRepo;
		public ProfileController(IProfileRepo profileRepo)
		{
			_profileRepo = profileRepo;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
				return RedirectToAction("Login", "Auth", new { area = "Auth" });

			var profile = await _profileRepo.GetProfileAsync(userId);
			if (profile == null) return NotFound();
			return View(profile);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(UpdateProfileViewModel model, IFormFile? avatar)
		{
			var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
				return RedirectToAction("Login", "Auth", new { area = "Auth" });

			if (avatar != null)
			{
				var url = await _profileRepo.UploadAvatarAsync(userId, avatar);
				if (url != null) model.HinhAnh = url;
			}

			var ok = await _profileRepo.UpdateProfileAsync(userId, model);
			if (!ok) ModelState.AddModelError("", "Cập nhật thất bại");
			var profile = await _profileRepo.GetProfileAsync(userId);
			return View("Index", profile);
		}

		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}

		public class ChangePasswordVm
		{
			public string OldPassword { get; set; } = string.Empty;
			public string NewPassword { get; set; } = string.Empty;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordVm vm)
		{
			var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
				return RedirectToAction("Login", "Auth", new { area = "Auth" });

			using var client = new HttpClient();
			var res = await client.PostAsJsonAsync($"https://localhost:7221/api/Profile/{userId}/change-password", new { oldPassword = vm.OldPassword, newPassword = vm.NewPassword });
			if (res.IsSuccessStatusCode)
			{
				TempData["Success"] = "Đổi mật khẩu thành công";
				return RedirectToAction("Index");
			}
			TempData["Error"] = await res.Content.ReadAsStringAsync();
			return View(vm);
		}
	}
}

