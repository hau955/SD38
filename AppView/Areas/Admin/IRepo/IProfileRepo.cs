using AppApi.ViewModels.Profile;

namespace AppView.Areas.Admin.IRepo
{
    public interface IProfileRepo
    {
        Task<ProfileViewModel?> GetProfileAsync(Guid id);
        Task<bool> UpdateProfileAsync(Guid id, UpdateProfileViewModel model);
        Task<string?> UploadAvatarAsync(Guid id, IFormFile file);
    }
}
