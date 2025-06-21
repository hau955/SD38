using WebModels.Models;

namespace AppApi.Service
{
    public interface ISanPhamService
    {
        Task<List<SanPham>> GetAll();
        Task<SanPham?> GetByID(Guid id);
        Task<SanPham> Create(SanPham sanpham);
        Task<SanPham?> Update(Guid id, SanPham Updatesanpham);
        Task<bool> Detele(Guid id);
        Task<string> Toggle(Guid id);
    }
}
