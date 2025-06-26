using WebModels.Models;

namespace AppApi.Service
{
    public interface ICoAoService
    {
        Task<IEnumerable<CoAo>> GetAllAsync();
        Task<CoAo?> GetByIdAsync(Guid id);
        Task<CoAo> CreateAsync(CoAo coao);
        Task<bool> UpdateAsync(Guid id, CoAo coao);
        Task<bool> DeleteAsync(Guid id);
    }
}
