using AppView.Areas.Admin.ViewModels;
using WebModels.Models;

namespace AppApi.Service
{
    public interface ISanPhamService
    {
        Task<List<SanPham>> GetAll();
        Task<SanPham?> GetByID(Guid id);
        Task<SanPham?> GetByIDWithDetails(Guid id);
        Task<SanPham> Create(SanPhamCTViewModel sanpham);
        Task<SanPham?> Update(Guid id, SanPham Updatesanpham);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
