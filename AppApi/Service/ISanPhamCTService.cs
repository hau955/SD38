using WebModels.Models;

namespace AppApi.Service
{
    public interface ISanPhamCTService
    {
        Task<IEnumerable<SanPhamCT>> GetAllAsync();
        Task<SanPhamCT?> GetByIdAsync(Guid id);
        Task<SanPhamCT?> CreateAsync(SanPhamCT sanPhamCT);
        Task<bool> UpdateAsync(Guid id, SanPhamCT sanPhamCT);
        Task<bool> DeleteAsync(Guid id);
    }
}
