using AppApi.ViewModels.Profile;
using AppView.Areas.Admin.IRepo;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly IProfileRepo _profileRepo;

        public ProfileController(IProfileRepo profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var userIdStr = HttpContext.Session.GetString("ID");


            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                return RedirectToAction("Login", "Auth", new { area = "Auth" });

            var profile = await _profileRepo.GetProfileAsync(userId);

            if (profile == null)
                return NotFound("Không tìm thấy thông tin hồ sơ.");

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, UpdateProfileViewModel model, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            if (avatar != null)
            {
                var imageUrl = await _profileRepo.UploadAvatarAsync(id, avatar);
                if (imageUrl != null)
                {
                    model.HinhAnh = imageUrl;
                }
            }

            var result = await _profileRepo.UpdateProfileAsync(id, model);
            if (!result)
                ModelState.AddModelError("", "Cập nhật thất bại!");

            var updatedProfile = await _profileRepo.GetProfileAsync(id);
            return View("Index", updatedProfile);
        }
    }

}
