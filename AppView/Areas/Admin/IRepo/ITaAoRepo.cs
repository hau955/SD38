using WebModels.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface ITaAoRepo
    {
        Task<List<TaAo>> GetAll();
        Task<TaAo?> GetByID(Guid id);
        Task<TaAo> Create(TaAo taao);
        Task<TaAo?> Update(Guid id, TaAo taao);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
