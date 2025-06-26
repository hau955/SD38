using Microsoft.EntityFrameworkCore;
using WebModels.Models;

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
            size.IDSize = Guid.NewGuid();
            size.NgayTao = DateTime.UtcNow;
            size.NgaySua = DateTime.UtcNow;
            size.TrangThai = true;

            _db.Sizes.Add(size);
            await _db.SaveChangesAsync();
            return size;
        }

        public async Task<IEnumerable<Size>> GetAllAsync()
        {
            return await _db.Sizes.ToListAsync();
        }

        public async Task<Size?> GetByIdAsync(Guid id)
        {
            return await _db.Sizes.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Guid id, Size size)
        {
            var existing = await _db.Sizes.FindAsync(id);
            if (existing == null)
                return false;

            existing.SoSize = size.SoSize;
            existing.TrangThai = size.TrangThai;
            existing.NgaySua = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Sizes.FindAsync(id);
            if (existing == null)
                return false;

            _db.Sizes.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
