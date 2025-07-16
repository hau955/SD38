using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class ChatLieuService : IChatLieuService
    {
        // Implement methods for ChatLieuService here
        private readonly ApplicationDbContext _db;
        public ChatLieuService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<ChatLieu>> GetAllChatLieusAsync()
        {
            return await _db.ChatLieus.ToListAsync();
        }
        public async Task<ChatLieu> GetChatLieuByIdAsync(Guid id)
        {
            return await _db.ChatLieus.FindAsync(id);
        }

        public async Task<ChatLieu> CreateChatLieuAsync(ChatLieu chatLieu)
        {
            chatLieu.NgayTao = DateTime.Now;
            chatLieu.NgaySua = DateTime.Now;
            chatLieu.TrangThai = true;
            chatLieu.TenChatLieu = chatLieu.TenChatLieu.Trim();

            _db.ChatLieus.Add(chatLieu);
            await _db.SaveChangesAsync();
            return chatLieu;
        }

        public async Task<bool> UpdateChatLieuAsync(ChatLieu chatLieu)
        {
            var existingchatLieu = await _db.ChatLieus.FindAsync(chatLieu.IDChatLieu);
            if (existingchatLieu == null)
            {
                return false;
            }

            existingchatLieu.TenChatLieu = chatLieu.TenChatLieu;
            existingchatLieu.TrangThai = chatLieu.TrangThai;
            existingchatLieu.NgaySua = DateTime.Now;
            existingchatLieu.NgayTao = DateTime.Now;

            _db.Entry(existingchatLieu).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ChatLieuExistsAsync(chatLieu.IDChatLieu))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteChatLieuAsync(Guid id)
        {
            var chatLieu = await _db.ChatLieus.FindAsync(id);
            if (chatLieu == null)
            {
                return false;
            }

            _db.ChatLieus.Remove(chatLieu);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChatLieuExistsAsync(Guid id)
        {
            return await _db.ChatLieus.AnyAsync(e => e.IDChatLieu == id);
        }

    }
}
