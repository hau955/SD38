using Microsoft.EntityFrameworkCore;
using WebModels.Models;

namespace AppApi.Service
{
    public class SanPhamCTService : ISanPhamCTService
    {
        private readonly ApplicationDbContext _db;

        public SanPhamCTService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SanPhamCT>> GetAllAsync()
        {
            return await _db.SanPhamChiTiets
                .Include(spct => spct.SanPham)
                .Include(spct => spct.SizeAo)
                .Include(spct => spct.MauSac)
                .Include(spct => spct.CoAo)
                .Include(spct => spct.TaAo)
                .ToListAsync();
        }

        public async Task<SanPhamCT?> GetByIdAsync(Guid id)
        {
            return await _db.SanPhamChiTiets
                .Include(spct => spct.SanPham)
                .Include(spct => spct.SizeAo)
                .Include(spct => spct.MauSac)
                .Include(spct => spct.CoAo)
                .Include(spct => spct.TaAo)
                .FirstOrDefaultAsync(spct => spct.IDSanPhamCT == id);
        }

        public async Task<SanPhamCT?> CreateAsync(SanPhamCT spct)
        {
            // Kiểm tra tồn tại FK
            var isValid = await ValidateForeignKeys(spct);
            if (!isValid) return null;

            spct.IDSanPhamCT = Guid.NewGuid();
            spct.NgayTao = DateTime.UtcNow;
            spct.NgaySua = DateTime.UtcNow;
            spct.TrangThai = true;

            _db.SanPhamChiTiets.Add(spct);
            await _db.SaveChangesAsync();
            return spct;
        }

        public async Task<bool> UpdateAsync(Guid id, SanPhamCT spct)
        {
            var existing = await _db.SanPhamChiTiets.FindAsync(id);
            if (existing == null) return false;

            var isValid = await ValidateForeignKeys(spct);
            if (!isValid) return false;

            existing.IDSanPham = spct.IDSanPham;
            existing.IDSize = spct.IDSize;
            existing.IDCoAo = spct.IDCoAo;
            existing.IDTaAo = spct.IDTaAo;
            existing.IDMauSac = spct.IDMauSac;
            existing.SoLuongTonKho = spct.SoLuongTonKho;
            existing.GiaBan = spct.GiaBan;
            existing.HinhAnh = spct.HinhAnh;
            existing.NgaySua = DateTime.UtcNow;
            existing.TrangThai = spct.TrangThai;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.SanPhamChiTiets.FindAsync(id);
            if (existing == null) return false;

            _db.SanPhamChiTiets.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task<bool> ValidateForeignKeys(SanPhamCT spct)
        {
            var sizeExists = await _db.Sizes.AnyAsync(x => x.IDSize == spct.IDSize);
            var coAoExists = await _db.CoAos.AnyAsync(x => x.IDCoAo == spct.IDCoAo);
            var taAoExists = await _db.TaAos.AnyAsync(x => x.IDTaAo == spct.IDTaAo);
            var mauSacExists = await _db.MauSacs.AnyAsync(x => x.IDMauSac == spct.IDMauSac);
            var sanPhamExists = await _db.SanPhams.AnyAsync(x => x.IDSanPham == spct.IDSanPham);

            return sizeExists && coAoExists && taAoExists && mauSacExists && sanPhamExists;
        }
    }
}
