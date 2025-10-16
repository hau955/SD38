using AppApi.IService;
using AppApi.ViewModels.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileServive _profileService;

        public ProfileController(IProfileServive profileService)
        {
            _profileService = profileService;
        }

        // Truyền id lên khi gọi API
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var result = await _profileService.GetProfileAsync(id);
            if (result == null) return NotFound("Không tìm thấy user.");

            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileViewModel dto)
        {
            var success = await _profileService.UpdateProfileAsync(id, dto);
            if (!success) return NotFound("Không tìm thấy user.");

            return Ok("Cập nhật thành công");
        }
        [HttpPost("{id}/upload-avatar")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            var result = await _profileService.UploadAvatarAsync(id, file);
            if (result == null)
                return BadRequest("Tải ảnh thất bại.");

            return Ok(new
            {
                success = true,
                imageUrl = result
            });
        }
        public class ChangePasswordDto
        {
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Mật khẩu không hợp lệ");

            var success = await _profileService.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword);
            if (!success) return BadRequest("Đổi mật khẩu thất bại");

            return Ok("Đổi mật khẩu thành công");
        }
    }
}
