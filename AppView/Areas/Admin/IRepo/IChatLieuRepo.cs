using AppData.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface IChatLieuRepo
    {
        Task<List<ChatLieu>> GetAll();
        Task<ChatLieu?> GetByID(Guid id);
        Task<ChatLieu> Create(ChatLieu ChatLieu);
        Task<ChatLieu?> Update(Guid id, ChatLieu ChatLieu);
        Task<bool> Delete(Guid id);
        Task<string> Toggle(Guid id);
    }
}
