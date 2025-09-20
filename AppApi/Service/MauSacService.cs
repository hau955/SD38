
using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class MauSacService : IMauSacService
    {
        private readonly ApplicationDbContext _db;
        public MauSacService(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<MauSac> CreateMauSacAsync(MauSac mauSac)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(mauSac.TenMau))
                    throw new ArgumentException("Tên màu không được để trống.");

                // Kiểm tra trùng lặp tên màu (case-insensitive)
                var existingMauSac = await _db.MauSacs
                    .FirstOrDefaultAsync(m => m.TenMau.ToLower().Trim() == mauSac.TenMau.ToLower().Trim());
                
                if (existingMauSac != null)
                    throw new InvalidOperationException($"Màu sắc '{mauSac.TenMau}' đã tồn tại trong hệ thống.");

                // Chuẩn hóa dữ liệu
                mauSac.TenMau = mauSac.TenMau.Trim();
                mauSac.NgayTao = DateTime.Now;
                mauSac.NgaySua = DateTime.Now;
                mauSac.TrangThai = true;

                _db.MauSacs.Add(mauSac);
                await _db.SaveChangesAsync();
                return mauSac;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi lưu màu sắc vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi tạo màu sắc: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteMauSacAsync(Guid id)
        {
            try
            {
                var mauSac = await _db.MauSacs
                    .Include(m => m.SanPhamChiTiets)
                    .FirstOrDefaultAsync(m => m.IDMauSac == id);
                
                if (mauSac == null)
                    throw new ArgumentException("Không tìm thấy màu sắc để xóa.");

                // Kiểm tra xem màu sắc có đang được sử dụng trong sản phẩm không
                if (mauSac.SanPhamChiTiets.Any())
                    throw new InvalidOperationException($"Không thể xóa màu sắc '{mauSac.TenMau}' vì đang được sử dụng trong {mauSac.SanPhamChiTiets.Count} sản phẩm.");

                _db.MauSacs.Remove(mauSac);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi xóa màu sắc khỏi cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi xóa màu sắc: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<MauSac>> GetAllMauSacsAsync()
        {
            try
            {
                return await _db.MauSacs
                    .OrderBy(m => m.TenMau)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy danh sách màu sắc: {ex.Message}", ex);
            }
        }

        public async Task<MauSac> GetMauSacByIdAsync(Guid id)
        {
            try
            {
                var mauSac = await _db.MauSacs.FindAsync(id);
                if (mauSac == null)
                    throw new ArgumentException("Không tìm thấy màu sắc với ID đã cho.");
                
                return mauSac;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thông tin màu sắc: {ex.Message}", ex);
            }
        }

        public async Task<bool> MauSacExistsAsync(Guid id)
        {
            try
            {
                return await _db.MauSacs.AnyAsync(e => e.IDMauSac == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi kiểm tra tồn tại màu sắc: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateMauSacAsync(Guid id, MauSac mauSac)
        {
            try
            {
                // Validation business logic
                if (string.IsNullOrWhiteSpace(mauSac.TenMau))
                    throw new ArgumentException("Tên màu không được để trống.");

                var existingMauSac = await _db.MauSacs.FindAsync(id);
                if (existingMauSac == null)
                    throw new ArgumentException("Không tìm thấy màu sắc để cập nhật.");

                // Kiểm tra trùng lặp tên màu (trừ chính nó)
                var duplicateMauSac = await _db.MauSacs
                    .FirstOrDefaultAsync(m => m.TenMau.ToLower().Trim() == mauSac.TenMau.ToLower().Trim() 
                                           && m.IDMauSac != id);
                
                if (duplicateMauSac != null)
                    throw new InvalidOperationException($"Màu sắc '{mauSac.TenMau}' đã tồn tại trong hệ thống.");

                // Cập nhật thông tin
                existingMauSac.TenMau = mauSac.TenMau.Trim();
                existingMauSac.TrangThai = mauSac.TrangThai;
                existingMauSac.NgaySua = DateTime.Now;

                _db.Entry(existingMauSac).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng làm mới và thử lại.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi cập nhật màu sắc vào cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi cập nhật màu sắc: {ex.Message}", ex);
            }
        }
        public async Task<bool> ToggleStatusAsync(Guid id)
        {
            try
            {
                var mauSac = await _db.MauSacs.FindAsync(id);
                if (mauSac == null)
                    throw new ArgumentException("Không tìm thấy màu sắc để thay đổi trạng thái.");

                mauSac.TrangThai = !mauSac.TrangThai;
                mauSac.NgaySua = DateTime.Now;

                _db.MauSacs.Update(mauSac);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Lỗi khi thay đổi trạng thái màu sắc trong cơ sở dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi không xác định khi thay đổi trạng thái màu sắc: {ex.Message}", ex);
            }
        }

    }
}
