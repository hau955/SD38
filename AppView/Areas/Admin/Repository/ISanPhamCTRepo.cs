using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public interface ISanPhamCTRepo
    {
        Task<List<SanPhamCT>> GetAll();
        Task<SanPhamCT?> GetByID(Guid id);
        Task<SanPhamCT> Create(SanPhamCT sanPhamCT);
        Task<SanPhamCT?> Update(Guid id, SanPhamCT sanPhamCT);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
