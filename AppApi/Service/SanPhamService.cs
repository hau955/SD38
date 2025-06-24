using Microsoft.EntityFrameworkCore;
using WebModels.Models;

namespace AppApi.Service
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ApplicationDbContext _db;
        public SanPhamService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<SanPham> Create(SanPham sanpham)
        {
            sanpham.IDSanPham = Guid.NewGuid();
            sanpham.NgayTao = DateTime.UtcNow;
            sanpham.NgaySua = DateTime.UtcNow;
            // Không cần xử lý ảnh ở đây
            _db.SanPhams.Add(sanpham);
            await _db.SaveChangesAsync();
            return sanpham;
        }

        public async Task<bool> Detele(Guid id)
        {
            var sp = await _db.SanPhams.FindAsync(id);
            if (sp == null) return false;

            _db.SanPhams.Remove(sp);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<SanPham>> GetAll()
        {
            return await _db.SanPhams
                .Include(s => s.SanPhamChiTiets)
                .Include(s => s.SanPhamGiamGias)
                .ToListAsync();
        }

        public async Task<SanPham?> GetByID(Guid id)
        {
            return await _db.SanPhams
                .Include(s => s.SanPhamGiamGias)
                .Include(s => s.SanPhamChiTiets)
                .FirstOrDefaultAsync(sp => sp.IDSanPham == id);
        }

        public async Task<string> Toggle(Guid id)
        {
            var sp = await _db.SanPhams.FindAsync(id);
            if (sp == null) return "Sản phẩm không tồn tại.";

            sp.TrangThai = !sp.TrangThai;
            sp.NgaySua = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return sp.TrangThai ? "Sản phẩm đã được bật." : "Sản phẩm đã bị tắt.";
        }

        public async Task<SanPham?> Update(Guid id, SanPham Updatesanpham)
        {
            var existing = await _db.SanPhams.FindAsync(id);
            if (existing == null) return null;

            existing.TenSanPham = Updatesanpham.TenSanPham;
            existing.GioiTinh = Updatesanpham.GioiTinh;
            existing.TrongLuong = Updatesanpham.TrongLuong;
            existing.MoTa = Updatesanpham.MoTa;
            // Bỏ xử lý ảnh
            existing.NgaySua = DateTime.UtcNow;

            _db.SanPhams.Update(existing);
            await _db.SaveChangesAsync();
            return existing;
        }
    }
}
