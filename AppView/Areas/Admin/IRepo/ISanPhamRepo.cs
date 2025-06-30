using AppView.Areas.Admin.ViewModels.SanPhamViewModels;
using WebModels.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface ISanPhamRepo
    {
        Task<bool> CreateSanPhamAsync(SanPhamCreateViewModel model);
        Task<bool> UpdateSanPhamAsync(SanPhamCreateViewModel model);
        Task<List<SanPhamView>> GetAllSanPhamAsync();
        Task<SanPhamCreateViewModel?> GetByIdAsync(Guid id);
    }
}
