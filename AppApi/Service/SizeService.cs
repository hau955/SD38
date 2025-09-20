using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class SizeService : ISizeService
    {
        private readonly ApplicationDbContext _db;

        public SizeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Size> CreateAsync(Size size)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(size.SoSize))
                    throw new ArgumentException("Số size không được để trống.");

                // Kiểm tra trùng lặp số size (case-insensitive)
                var existingSize = await _db.Sizes
                    .FirstOrDefaultAsync(s => s.SoSize.ToLower().Trim() == size.SoSize.ToLower().Trim());
                
                if (existingSize != null)
                    throw new InvalidOperationException($"Size '{size.SoSize}' đã tồn tại trong hệ thống.");

                // Chuẩn hóa dữ liệu
                size.IDSize = Guid.NewGuid();
                size.SoSize = size.SoSize.Trim();
                size.NgayTao = DateTime.UtcNow;
                size.NgaySua = DateTime.UtcNow;
                size.TrangThai = true;

                _db.Sizes.Add(size);
                await _db.SaveChangesAsync();
                return size;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi lưu size vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi tạo size: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Size>> GetAllAsync()
        {
            try
            {
                return await _db.Sizes
                    .OrderBy(s => s.SoSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy danh sách size: {ex.Message}", ex);
            }
        }

        public async Task<Size?> GetByIdAsync(Guid id)
        {
            try
            {
                var size = await _db.Sizes.FindAsync(id);
                if (size == null)
                    throw new ArgumentException("Không tìm thấy size với ID đã cho.");
                
                return size;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thông tin size: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(Guid id, Size size)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(size.SoSize))
                    throw new ArgumentException("Số size không được để trống.");

                var existing = await _db.Sizes.FindAsync(id);
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy size để cập nhật.");

                // Kiểm tra trùng lặp số size (trừ chính nó)
                var duplicateSize = await _db.Sizes
                    .FirstOrDefaultAsync(s => s.SoSize.ToLower().Trim() == size.SoSize.ToLower().Trim() 
                                           && s.IDSize != id);
                
                if (duplicateSize != null)
                    throw new InvalidOperationException($"Size '{size.SoSize}' đã tồn tại trong hệ thống.");

                // Cập nhật thông tin
                existing.SoSize = size.SoSize.Trim();
                existing.TrangThai = size.TrangThai;
                existing.NgaySua = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng làm mới và thử lại.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi cập nhật size vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi cập nhật size: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _db.Sizes
                    .Include(s => s.SanPhamChiTiets)
                    .FirstOrDefaultAsync(s => s.IDSize == id);
                
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy size để xóa.");

                // Kiểm tra xem size có đang được sử dụng trong sản phẩm không
                if (existing.SanPhamChiTiets.Any())
                    throw new InvalidOperationException($"Không thể xóa size '{existing.SoSize}' vì đang được sử dụng trong {existing.SanPhamChiTiets.Count} sản phẩm.");

                _db.Sizes.Remove(existing);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi xóa size khỏi cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi xóa size: {ex.Message}", ex);
            }
        }
    }
}
