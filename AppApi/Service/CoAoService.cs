using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

namespace AppApi.Service
{
    public class CoAoService : ICoAoService
    {
        private readonly ApplicationDbContext _db;

        public CoAoService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CoAo> CreateAsync(CoAo coao)
        {
            coao.IDCoAo = Guid.NewGuid();
            coao.NgayTao = DateTime.UtcNow;
            coao.NgaySua = DateTime.UtcNow;
            coao.TrangThai = true;

            _db.CoAos.Add(coao);
            await _db.SaveChangesAsync();
            return coao;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.CoAos.FindAsync(id);
            if (existing == null)
                return false;

            _db.CoAos.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CoAo>> GetAllAsync()
        {
            return await _db.CoAos.ToListAsync();
        }

        public async Task<CoAo?> GetByIdAsync(Guid id)
        {
            return await _db.CoAos.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Guid id, CoAo coao)
        {
            var existing = await _db.CoAos.FindAsync(id);
            if (existing == null)
                return false;

            // Cập nhật tất cả thuộc tính cần thiết
            existing.TenCoAo = coao.TenCoAo;
            existing.KieuDang = coao.KieuDang;
            existing.ChatLieu = coao.ChatLieu;
            existing.TrangTri = coao.TrangTri;
            existing.MauSac = coao.MauSac;
            existing.NgaySua = DateTime.UtcNow;
            existing.TrangThai = coao.TrangThai;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
