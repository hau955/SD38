using WebModels.Models;

namespace AppApi.Service
{
    public interface ISizeService
    {
        Task<IEnumerable<Size>> GetAllAsync();
        Task<Size?> GetByIdAsync(Guid id);
        Task<Size> CreateAsync(Size size);
        Task<bool> UpdateAsync(Guid id, Size size);
        Task<bool> DeleteAsync(Guid id);
    }
}
