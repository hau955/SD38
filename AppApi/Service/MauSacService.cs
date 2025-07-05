
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
            mauSac.NgayTao = DateTime.Now;
            mauSac.NgaySua = DateTime.Now;
            mauSac.TrangThai = true;

            _db.MauSacs.Add(mauSac);
            await _db.SaveChangesAsync();
            return mauSac;
        }

        public async Task<bool> DeleteMauSacAsync(Guid id)
        {
            var mauSac = await _db.MauSacs.FindAsync(id);
            if (mauSac == null)
            {
                return false;
            }

            _db.MauSacs.Remove(mauSac);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MauSac>> GetAllMauSacsAsync()
        {
            return await _db.MauSacs.ToListAsync();
        }

        public async Task<MauSac> GetMauSacByIdAsync(Guid id)
        {
            return await _db.MauSacs.FindAsync(id);
        }

        public async Task<bool> MauSacExistsAsync(Guid id)
        {
            return await _db.MauSacs.AnyAsync(e => e.IDMauSac == id);
        }

        public async Task<bool> UpdateMauSacAsync(MauSac mauSac)
        {
            var existingMauSac = await _db.MauSacs.FindAsync(mauSac.IDMauSac);
            if (existingMauSac == null)
            {
                return false; 
            }

            existingMauSac.TenMau = mauSac.TenMau;
            existingMauSac.TrangThai = mauSac.TrangThai;
            existingMauSac.NgaySua = DateTime.Now; 

            _db.Entry(existingMauSac).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MauSacExistsAsync(mauSac.IDMauSac))
                {
                    return false; 
                }
                else
                {
                    throw; 
                }
            }
        }
    }
}
