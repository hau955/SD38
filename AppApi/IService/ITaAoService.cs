using WebModels.Models;

namespace AppApi.IService
{
    public interface ITaAoService
    {
        Task<IEnumerable<TaAo>> GetAllAsync();
        Task<TaAo?> GetByIdAsync(Guid id);
        Task<TaAo> CreateAsync(TaAo taao);
        Task<bool> UpdateAsync(Guid id, TaAo taao);
        Task<bool> DeleteAsync(Guid id);
    }
}
