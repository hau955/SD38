using WebModels.Models;

namespace AppView.Repository
{
    public interface ISanPhamRepo
    {
        Task<List<SanPham>> GetAll();
        Task<SanPham?> GetByID(Guid id);
        Task<SanPham> Create(SanPham sanpham);
        Task<SanPham?> Update(Guid id, SanPham sanpham);
        Task<bool> Detele(Guid id);
        Task<string> Toggle(Guid id);
    }
}
