using AppView.Areas.Admin.ViewModels;

namespace AppView.Areas.Admin.IRepo
{
    public interface IDanhMucRePo
    {
        Task<List<DanhMucViewModel>> GetAllDanhMucsAsync();
        Task<DanhMucViewModel?> GetDanhMucByIdAsync(Guid id);
        Task<bool> CreateDanhMucAsync(DanhMucViewModel model);
        Task<DanhMucViewModel?> UpdateDanhMucAsync(DanhMucViewModel model);
        Task<bool> DeleteDanhMucAsync(Guid id);
    }
}
