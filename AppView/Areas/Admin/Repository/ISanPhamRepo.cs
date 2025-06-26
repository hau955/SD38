using AppView.Areas.Admin.ViewModels;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public interface ISanPhamRepo
    {
        Task<List<SanPham>> GetAll();
        Task<SanPham?> GetByID(Guid id);
        Task<SanPham> Create(SanPhamCTViewModel model);
        Task<SanPham?> Update(SanPhamCTViewModel sanpham);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
