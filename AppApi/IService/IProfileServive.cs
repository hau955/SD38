using AppApi.ViewModels.Profile;

namespace AppApi.IService
{
    public interface IProfileServive
    {
        Task<ProfileViewModel?> GetProfileAsync(Guid id);
        Task<bool> UpdateProfileAsync(Guid id, UpdateProfileViewModel dto);
        Task<string?> UploadAvatarAsync(Guid id, IFormFile file);
        Task<bool> ChangePasswordAsync(Guid id, string oldPassword, string newPassword);
    }
}
