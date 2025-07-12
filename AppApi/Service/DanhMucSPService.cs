using AppApi.IService;
using AppApi.ViewModels.SanPham;
using AppData.Models;
using Microsoft.EntityFrameworkCore;


namespace AppApi.Service
{
    public class DanhMucSPService : IDanhMucSPService
    {
        private readonly ApplicationDbContext _db;
        public DanhMucSPService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<DanhMuc>> GetAllDanhMucSPsAsync()
        {
            return await _db.DanhMucs.ToListAsync();
        }
        public async Task<DanhMuc> GetDanhMucSPByIdAsync(Guid id)
        {
            return await _db.DanhMucs.FindAsync(id);
        }

        public async Task<DanhMuc> CreateDanhMucSPAsync(DanhMuc danhMuc)
        {
            danhMuc.NgayTao = DateTime.Now;
            danhMuc.NgaySua = DateTime.Now;
            danhMuc.TrangThai = true;
            danhMuc.TenDanhMuc = danhMuc.TenDanhMuc.Trim();

            _db.DanhMucs.Add(danhMuc);
            await _db.SaveChangesAsync();
            return danhMuc;
        }

        public async Task<bool> UpdateDanhMucSPAsync(DanhMuc danhMuc)
        {
            var existingdanhMuc = await _db.DanhMucs.FindAsync(danhMuc.DanhMucId);
            if (existingdanhMuc == null)
            {
                return false;
            }

            existingdanhMuc.TenDanhMuc = danhMuc.TenDanhMuc;
            existingdanhMuc.TrangThai = danhMuc.TrangThai;
            existingdanhMuc.NgaySua = DateTime.Now;
            existingdanhMuc.NgayTao = DateTime.Now;

            _db.Entry(existingdanhMuc).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DanhMucSPExistsAsync(danhMuc.DanhMucId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteDanhMucSPAsync(Guid id)
        {
            var danhMuc = await _db.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return false;
            }

            _db.DanhMucs.Remove(danhMuc);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DanhMucSPExistsAsync(Guid id)
        {
            return await _db.DanhMucs.AnyAsync(e => e.DanhMucId == id);
        }

    }
}

