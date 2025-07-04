using AppData.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface ISizeRepo
    {
        Task<List<Size>> GetAll();
        Task<Size?> GetByID(Guid id);
        Task<Size> Create(Size size);
        Task<Size?> Update(Guid id, Size size);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
