using AppApi.IService;
using AppApi.ViewModels.Profile;
using AppData.Models;
using Microsoft.AspNetCore.Identity;

namespace AppApi.Service
{
    public class ProfileService : IProfileServive
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public ProfileService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public async Task<ProfileViewModel?> GetProfileAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            return new ProfileViewModel
            {
                Id = user.Id,
                HoTen = user.HoTen,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                GioiTinh = user.GioiTinh,
                NgaySinh = user.NgaySinh,
                DiaChi = user.DiaChi,
                HinhAnh = user.HinhAnh
            };
        }

        public async Task<bool> UpdateProfileAsync(Guid id, UpdateProfileViewModel dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            user.HoTen = dto.HoTen;
            user.GioiTinh = dto.GioiTinh;
            user.NgaySinh = dto.NgaySinh;
            user.DiaChi = dto.DiaChi;
            user.HinhAnh = dto.HinhAnh;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(Guid id, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }
       
        public async Task<string?> UploadAvatarAsync(Guid id, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Định dạng ảnh không hợp lệ.");

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new InvalidOperationException("Ảnh vượt quá 5MB.");

            var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(_env.WebRootPath, "avatars");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Cập nhật đường dẫn ảnh
            var imageUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/avatars/{fileName}";
            user.HinhAnh = imageUrl;

            await _userManager.UpdateAsync(user);
            return imageUrl;
        }
    }
}
