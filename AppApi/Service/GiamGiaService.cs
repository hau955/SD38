using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class GiamGiaService : IGiamGiaService
    {
        private readonly ApplicationDbContext _context;

        public GiamGiaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GiamGia>> GetAllAsync()
        {
            return await _context.GiamGias
                .Include(x => x.GiamGiaSanPhams)
                .ToListAsync();
        }

        public async Task<GiamGia?> GetByIdAsync(Guid id)
        {
            return await _context.GiamGias
                .Include(x => x.GiamGiaSanPhams)
                .FirstOrDefaultAsync(x => x.IDGiamGia == id);
        }

        public async Task<GiamGia> CreateAsync(GiamGia giamGia)
        {
            giamGia.IDGiamGia = Guid.NewGuid();
            giamGia.NgayTao = DateTime.Now;
            giamGia.NgaySua = DateTime.Now;

            _context.GiamGias.Add(giamGia);
            await _context.SaveChangesAsync();
            return giamGia;
        }

        public async Task<bool> UpdateAsync(GiamGia giamGia)
        {
            var exist = await _context.GiamGias.FindAsync(giamGia.IDGiamGia);
            if (exist == null) return false;

            giamGia.NgaySua = DateTime.Now;
            _context.Entry(exist).CurrentValues.SetValues(giamGia);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var giamGia = await _context.GiamGias.FindAsync(id);
            if (giamGia == null) return false;

            _context.GiamGias.Remove(giamGia);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<GiamGiaSPCT>> GetSanPhamCTByDiscountAsync(Guid giamGiaId)
        {
            return await _context.GiamGiaSPCT
                .Include(x => x.SanPhamCT)
                    .ThenInclude(spct => spct.SanPham) // nếu muốn hiện tên sản phẩm cha
                .Where(x => x.IDGiamGia == giamGiaId)
                .ToListAsync();
        }

        // Thêm giảm giá cho sản phẩm chi tiết
        public async Task<GiamGiaSPCT?> AddSanPhamCTToDiscountAsync(Guid giamGiaId, Guid sanPhamCTId)
        {
            // Check tồn tại giảm giá
            if (!await _context.GiamGias.AnyAsync(x => x.IDGiamGia == giamGiaId))
                return null;

            // Check tồn tại sản phẩm chi tiết
            if (!await _context.SanPhamChiTiets.AnyAsync(x => x.IDSanPhamCT == sanPhamCTId))
                return null;

            // Check trùng
            var exist = await _context.GiamGiaSPCT
                .FirstOrDefaultAsync(x => x.IDGiamGia == giamGiaId && x.IDSanPhamCT == sanPhamCTId);

            if (exist != null) return exist;

            var giamGiaSPCT = new GiamGiaSPCT
            {
                ID = Guid.NewGuid(),
                IDGiamGia = giamGiaId,
                IDSanPhamCT = sanPhamCTId
            };

            _context.GiamGiaSPCT.Add(giamGiaSPCT);
            await _context.SaveChangesAsync();

            return giamGiaSPCT;
        }

        public async Task<GiamGiaSanPham?> AddProductToDiscountAsync(Guid giamGiaId, Guid sanPhamId)
        {
            if (!await _context.GiamGias.AnyAsync(x => x.IDGiamGia == giamGiaId))
                return null;

            if (!await _context.SanPhams.AnyAsync(x => x.IDSanPham == sanPhamId))
                return null;

            var exist = await _context.GiamGiaSanPham
                .FirstOrDefaultAsync(x => x.IDGiamGia == giamGiaId && x.IDSanPham == sanPhamId);

            if (exist != null) return exist;

            var giamGiaSP = new GiamGiaSanPham
            {
                ID = Guid.NewGuid(),
                IDGiamGia = giamGiaId,
                IDSanPham = sanPhamId
            };

            _context.GiamGiaSanPham.Add(giamGiaSP);
            await _context.SaveChangesAsync();

            return giamGiaSP;
        }

        public async Task<List<GiamGiaSanPham>> GetProductsByDiscountAsync(Guid giamGiaId)
        {
            return await _context.GiamGiaSanPham
                .Include(x => x.SanPham)
                .Where(x => x.IDGiamGia == giamGiaId)
                .ToListAsync();
        }
        public async Task<bool> AddCategoryToDiscount(Guid idGiamGia, Guid idDanhMuc)
        {
            var exists = await _context.GiamGiaDanhMuc
                .AnyAsync(x => x.IDGiamGia == idGiamGia && x.DanhMucId == idDanhMuc);
            if (exists) return false;

            var entity = new GiamGiaDanhMuc
            {
                IDGiamGia = idGiamGia,
                DanhMucId = idDanhMuc
            };

            _context.GiamGiaDanhMuc.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<(decimal GiaGoc, decimal GiaSauGiam, decimal? SoTienGiam)?>
   TinhGiaSauGiamAsync(Guid idSanPhamCT, Guid danhMucId)
        {
            var sanPhamCT = await _context.SanPhamChiTiets
                .Include(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.IDSanPhamCT == idSanPhamCT);

            if (sanPhamCT == null) return null;

            decimal giaGoc = sanPhamCT.GiaBan;
            decimal giaSauGiam = giaGoc;
            decimal? soTienGiam = null;
            // 🔎 Lấy giảm giá sản phẩm chi tiết
            var giamGiaSPCT = await _context.GiamGiaSPCT
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPhamCT == idSanPhamCT &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .OrderByDescending(x => x.GiamGia.GiaTri)
                .FirstOrDefaultAsync();

            // 🔎 Lấy giảm giá sản phẩm
            var giamGiaSP = await _context.GiamGiaSanPham
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPham == sanPhamCT.IDSanPham &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .OrderByDescending(x => x.GiamGia.GiaTri)
                .FirstOrDefaultAsync();

            // 🔎 Lấy giảm giá danh mục
            var giamGiaDM = await _context.GiamGiaDanhMuc
                .Include(x => x.GiamGia)
                .Where(x => x.DanhMucId == danhMucId &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .OrderByDescending(x => x.GiamGia.GiaTri)
                .FirstOrDefaultAsync();

            var giamGia = giamGiaSPCT?.GiamGia ?? giamGiaSP?.GiamGia ?? giamGiaDM?.GiamGia;


            if (giamGia != null)
            {
                if (giamGia.LoaiGiamGia == "PhanTram")
                {
                    var soTien = giaGoc * (giamGia.GiaTri / 100);
                    if (giamGia.GiaTriGiamToiDa.HasValue)
                        soTien = Math.Min(soTien, giamGia.GiaTriGiamToiDa.Value);

                    giaSauGiam = giaGoc - soTien;
                    soTienGiam = soTien;
                }
                else if (giamGia.LoaiGiamGia == "SoTien")
                {
                    giaSauGiam = Math.Max(0, giaGoc - giamGia.GiaTri);
                    soTienGiam = giamGia.GiaTri;
                }
            }

            return (giaGoc, giaSauGiam, soTienGiam);
        }
       
    }
}

