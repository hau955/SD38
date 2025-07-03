using AppData.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface ICoAoRepo
    {
        Task<List<CoAo>> GetAll();
        Task<CoAo?> GetByID(Guid id);
        Task<CoAo> Create(CoAo coao);
        Task<CoAo?> Update(Guid id, CoAo coao);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
