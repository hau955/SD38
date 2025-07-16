using AppApi.IService;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

namespace AppApi.Service
{
    public class SanPhamCTService : ISanPhamCTService
    {
        private readonly ApplicationDbContext _db;

        public SanPhamCTService(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<SanPhamCT?> GetByIdAsync(Guid id)
        {
            return await _db.SanPhamChiTiets.FindAsync(id);
        }

        public async Task<bool> UpdateAsync(SanPhamCT model)
        {
            var existing = await _db.SanPhamChiTiets.FindAsync(model.IDSanPhamCT);
            if (existing == null) return false;

            existing.IDMauSac = model.IDMauSac;
            existing.IDSize = model.IDSize;
            existing.IdChatLieu = model.IdChatLieu;
           
            existing.SoLuongTonKho = model.SoLuongTonKho;
            existing.GiaBan = model.GiaBan;
            existing.TrangThai = model.TrangThai;
            existing.NgaySua = DateTime.Now;

            _db.SanPhamChiTiets.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task AddAsync(SanPhamCT model)
        {
            model.IDSanPhamCT = Guid.NewGuid();
            model.NgayTao = DateTime.Now;
            model.NgaySua = DateTime.Now;
            _db.SanPhamChiTiets.Add(model);
            await _db.SaveChangesAsync();
        }

        public async Task<List<SanPhamCT>> GetBySanPhamIdAsync(Guid sanPhamId)
        {
            return await _db.SanPhamChiTiets
                 .Include(x => x.MauSac)
                 .Include(x => x.SizeAo)
                 
                // .Include(x => x.ChatLieu)
                 .Where(x => x.IDSanPham == sanPhamId)
                 .ToListAsync();
        }

        public async Task AddRangeAsync(List<SanPhamCT> models)
        {
            foreach (var item in models)
            {
                item.IDSanPhamCT = Guid.NewGuid();
                item.NgayTao = DateTime.Now;
                item.NgaySua = DateTime.Now;
            }

            await _db.SanPhamChiTiets.AddRangeAsync(models);
            await _db.SaveChangesAsync();
        }

       

        public async Task<bool> ExistsAsync(Guid idSanPham, Guid idMauSac, Guid idSize, Guid idChatlieu)
        {
            return await _db.SanPhamChiTiets.AnyAsync(x =>
                x.IDSanPham == idSanPham &&
                x.IDMauSac == idMauSac &&
                x.IDSize == idSize &&
                x.IdChatLieu == idChatlieu);
        }
    }
}
