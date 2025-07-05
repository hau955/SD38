

using AppData.Models;

namespace AppApi.IService
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
