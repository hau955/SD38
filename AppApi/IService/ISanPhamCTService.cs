using WebModels.Models;

namespace AppApi.IService
{
    public interface ISanPhamCTService
    {
        Task AddAsync(SanPhamCT model);
        Task AddRangeAsync(List<SanPhamCT> models);
        Task<List<SanPhamCT>> GetBySanPhamIdAsync(Guid sanPhamId);
        Task<SanPhamCT?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(SanPhamCT model);

    }
}
