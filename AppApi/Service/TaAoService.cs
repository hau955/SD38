using Microsoft.EntityFrameworkCore;
using WebModels.Models;

namespace AppApi.Service
{
    public class TaAoService : ITaAoService
    {
        private readonly ApplicationDbContext _db;

        public TaAoService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TaAo>> GetAllAsync()
        {
            return await _db.TaAos.ToListAsync();
        }

        public async Task<TaAo?> GetByIdAsync(Guid id)
        {
            return await _db.TaAos.FindAsync(id);
        }

        public async Task<TaAo> CreateAsync(TaAo taao)
        {
            taao.IDTaAo = Guid.NewGuid();
            taao.NgayTao = DateTime.UtcNow;
            taao.NgaySua = DateTime.UtcNow;
            taao.TrangThai = true;

            _db.TaAos.Add(taao);
            await _db.SaveChangesAsync();
            return taao;
        }

        public async Task<bool> UpdateAsync(Guid id, TaAo taao)
        {
            var existing = await _db.TaAos.FindAsync(id);
            if (existing == null)
                return false;

            existing.TenTaAo = taao.TenTaAo;
            existing.ChieuDaiTaAo = taao.ChieuDaiTaAo;
            existing.SoLuongTaAo = taao.SoLuongTaAo;
            existing.ChatLieu = taao.ChatLieu;
            existing.DuongMayTaAo = taao.DuongMayTaAo;
            existing.KieuTa = taao.KieuTa;
            existing.TrangTri = taao.TrangTri;
            existing.MauSac = taao.MauSac;
            existing.NgaySua = DateTime.UtcNow;
            existing.TrangThai = taao.TrangThai;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.TaAos.FindAsync(id);
            if (existing == null)
                return false;

            _db.TaAos.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
