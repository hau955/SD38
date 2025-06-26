using AppView.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;
using WebModels.Models;

namespace AppApi.Service
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SanPhamService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<SanPham> Create(SanPhamCTViewModel model)
        {
            string imagePath = null!;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);
                imagePath = "/images/" + fileName;
            }

            var sanPham = new SanPham
            {
                IDSanPham = Guid.NewGuid(),
                TenSanPham = model.TenSanPham,
                MoTa = model.MoTa,
                TrongLuong = model.TrongLuong,
                GioiTinh = model.GioiTinh,
                HinhAnh = imagePath
            };

            _context.SanPhams.Add(sanPham);
            var spct = new SanPhamCT
            {
                IDSanPhamCT = Guid.NewGuid(),
                IDSanPham = sanPham.IDSanPham,
                IDSize = model.IdSize,
                IDMauSac = model.IdMauSac,
                IDCoAo = model.IdCoAo,
                IDTaAo = model.IdTaAo,
                GiaBan = model.GiaBan,
                SoLuongTonKho = model.SoLuongTonKho,
                TrangThai = true,
                NgayTao = DateTime.Now,
                NgaySua = DateTime.Now
            };

            _context.SanPhamChiTiets.Add(spct);

            await _context.SaveChangesAsync();
            sanPham.SanPhamChiTiets = new List<SanPhamCT> { spct };
            return sanPham;
        }

        public async Task<bool> Delete(Guid id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return false;

            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SanPham>> GetAll()
        {
            return await _context.SanPhams
                .Include(s => s.SanPhamChiTiets)
                .Include(s => s.SanPhamGiamGias)
                .ToListAsync();
        }
        public async Task<SanPham?> GetByID(Guid id)
        {
            return await _context.SanPhams
                .Include(s => s.SanPhamGiamGias)
                .Include(s => s.SanPhamChiTiets)
                .FirstOrDefaultAsync(sp => sp.IDSanPham == id);
        }
        public async Task<SanPham?> GetByIDWithDetails(Guid id)
        {
            var sanPham = await _context.SanPhams
    .Include(sp => sp.SanPhamChiTiets)
    .FirstOrDefaultAsync(sp => sp.IDSanPham == id);
            return sanPham;
        }
        public async Task<string> Toggle(Guid id)
        {
            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null) return "Sản phẩm không tồn tại.";

            sp.TrangThai = !sp.TrangThai;
            sp.NgaySua = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return sp.TrangThai ? "Sản phẩm đã được bật." : "Sản phẩm đã bị tắt.";
        }

        public async Task<SanPham?> Update(Guid id, SanPham Updatesanpham)
        {
            var existing = await _context.SanPhams.FindAsync(id);
            if (existing == null) return null;

            existing.TenSanPham = Updatesanpham.TenSanPham;
            existing.GioiTinh = Updatesanpham.GioiTinh;
            existing.TrongLuong = Updatesanpham.TrongLuong;
            existing.MoTa = Updatesanpham.MoTa;
            existing.HinhAnh = Updatesanpham.HinhAnh; // ✅ THÊM DÒNG NÀY
            existing.NgaySua = DateTime.UtcNow;
            existing.TrangThai = Updatesanpham.TrangThai;
            _context.SanPhams.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

    }
}
