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
            try
            {
                return await _db.ChatLieus
                    .OrderBy(c => c.TenChatLieu)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy danh sách chất liệu: {ex.Message}", ex);
            }
        }
        
        public async Task<ChatLieu> GetChatLieuByIdAsync(Guid id)
        {
            try
            {
                var chatLieu = await _db.ChatLieus.FindAsync(id);
                if (chatLieu == null)
                    throw new ArgumentException("Không tìm thấy chất liệu với ID đã cho.");
                
                return chatLieu;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thông tin chất liệu: {ex.Message}", ex);
            }
        }

        public async Task<ChatLieu> CreateChatLieuAsync(ChatLieu chatLieu)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(chatLieu.TenChatLieu))
                    throw new ArgumentException("Tên chất liệu không được để trống.");

                // Kiểm tra trùng lặp tên chất liệu (case-insensitive)
                var existingChatLieu = await _db.ChatLieus
                    .FirstOrDefaultAsync(c => c.TenChatLieu.ToLower().Trim() == chatLieu.TenChatLieu.ToLower().Trim());
                
                if (existingChatLieu != null)
                    throw new InvalidOperationException($"Chất liệu '{chatLieu.TenChatLieu}' đã tồn tại trong hệ thống.");

                // Chuẩn hóa dữ liệu
                chatLieu.TenChatLieu = chatLieu.TenChatLieu.Trim();
                chatLieu.NgayTao = DateTime.Now;
                chatLieu.NgaySua = DateTime.Now;
                chatLieu.TrangThai = true;

                _db.ChatLieus.Add(chatLieu);
                await _db.SaveChangesAsync();
                return chatLieu;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi lưu chất liệu vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi tạo chất liệu: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateChatLieuAsync(ChatLieu chatLieu)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(chatLieu.TenChatLieu))
                    throw new ArgumentException("Tên chất liệu không được để trống.");

                var existingchatLieu = await _db.ChatLieus.FindAsync(chatLieu.IDChatLieu);
                if (existingchatLieu == null)
                    throw new ArgumentException("Không tìm thấy chất liệu để cập nhật.");

                // Kiểm tra trùng lặp tên chất liệu (trừ chính nó)
                var duplicateChatLieu = await _db.ChatLieus
                    .FirstOrDefaultAsync(c => c.TenChatLieu.ToLower().Trim() == chatLieu.TenChatLieu.ToLower().Trim() 
                                           && c.IDChatLieu != chatLieu.IDChatLieu);
                
                if (duplicateChatLieu != null)
                    throw new InvalidOperationException($"Chất liệu '{chatLieu.TenChatLieu}' đã tồn tại trong hệ thống.");

                // Cập nhật thông tin
                existingchatLieu.TenChatLieu = chatLieu.TenChatLieu.Trim();
                existingchatLieu.TrangThai = chatLieu.TrangThai;
                existingchatLieu.NgaySua = DateTime.Now;

                _db.Entry(existingchatLieu).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng làm mới và thử lại.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi cập nhật chất liệu vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi cập nhật chất liệu: {ex.Message}", ex);
            }
        }
        public async Task<bool> ToggleChatLieuAsync(Guid id)
        {
            try
            {
                var chatLieu = await _db.ChatLieus.FindAsync(id);
                if (chatLieu == null)
                    throw new ArgumentException("Không tìm thấy chất liệu để thay đổi trạng thái.");

                chatLieu.TrangThai = !chatLieu.TrangThai;
                chatLieu.NgaySua = DateTime.Now;

                _db.ChatLieus.Update(chatLieu);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi thay đổi trạng thái chất liệu trong cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi thay đổi trạng thái chất liệu: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteChatLieuAsync(Guid id)
        {
            try
            {
                var chatLieu = await _db.ChatLieus
                    .Include(c => c.SanPhamChiTiets)
                    .FirstOrDefaultAsync(c => c.IDChatLieu == id);
                
                if (chatLieu == null)
                    throw new ArgumentException("Không tìm thấy chất liệu để xóa.");

                // Kiểm tra xem chất liệu có đang được sử dụng trong sản phẩm không
                if (chatLieu.SanPhamChiTiets.Any())
                    throw new InvalidOperationException($"Không thể xóa chất liệu '{chatLieu.TenChatLieu}' vì đang được sử dụng trong {chatLieu.SanPhamChiTiets.Count} sản phẩm.");

                _db.ChatLieus.Remove(chatLieu);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi xóa chất liệu khỏi cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi xóa chất liệu: {ex.Message}", ex);
            }
        }

        public async Task<bool> ChatLieuExistsAsync(Guid id)
        {
            try
            {
                return await _db.ChatLieus.AnyAsync(e => e.IDChatLieu == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi kiểm tra tồn tại chất liệu: {ex.Message}", ex);
            }
        }

    }
}
