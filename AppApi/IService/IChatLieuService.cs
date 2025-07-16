using AppData.Models;

namespace AppApi.IService
{
    public interface IChatLieuService
    {
        Task<IEnumerable<ChatLieu>> GetAllChatLieusAsync();
        Task<ChatLieu> GetChatLieuByIdAsync(Guid id);
        Task<ChatLieu> CreateChatLieuAsync(ChatLieu chatLieu);
        Task<bool> UpdateChatLieuAsync(ChatLieu chatLieu);
        Task<bool> DeleteChatLieuAsync(Guid id);
        Task<bool> ChatLieuExistsAsync(Guid id);
    }
}
