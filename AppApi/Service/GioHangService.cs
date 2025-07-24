using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppApi.Service
{
    public class GioHangService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GioHangService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<GioHang> GetOrCreateGioHangAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            if (!Guid.TryParse(userId, out Guid userGuid))
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var gioHang = await _db.GioHangs
                .Include(g => g.GioHangChiTiets)
                .ThenInclude(gct => gct.SanPham)
                .FirstOrDefaultAsync(g => g.IDGioHang == userGuid);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    IDGioHang = userGuid,
                    TrangThai = true,
                    GioHangChiTiets = new List<GioHangCT>()
                };
                _db.GioHangs.Add(gioHang);
                await _db.SaveChangesAsync();
                Console.WriteLine($"Đã tạo mới GioHang với ID: {gioHang.IDGioHang}");
            }
            return gioHang;
        }

        public async Task AddToGioHangAsync(Guid sanPhamId, int soLuong)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            if (!Guid.TryParse(userId, out Guid userGuid))
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var gioHang = await GetOrCreateGioHangAsync();

            var trackedGioHang = await _db.GioHangs
                .Include(g => g.GioHangChiTiets)
                .ThenInclude(gct => gct.SanPham)
                .FirstOrDefaultAsync(g => g.IDGioHang == userGuid);
            if (trackedGioHang == null) throw new Exception("Không thể tải giỏ hàng.");

            var sanPham = await _db.SanPhams
                .Include(s => s.SanPhamChiTiets)
                .FirstOrDefaultAsync(s => s.IDSanPham == sanPhamId);

            if (sanPham == null)
                throw new Exception("Sản phẩm không tồn tại.");

            var sanPhamChiTiets = sanPham.SanPhamChiTiets?.Where(sct => sct.TrangThai).ToList() ?? new List<SanPhamCT>();
            Console.WriteLine($"Số lượng SanPhamChiTiets cho IDSanPham {sanPhamId}: {sanPhamChiTiets.Count}");
            if (!sanPhamChiTiets.Any())
                throw new Exception("Sản phẩm không có chi tiết tồn kho hợp lệ.");

            var totalStock = sanPhamChiTiets.Sum(sct => sct.SoLuongTonKho);
            Console.WriteLine($"Tổng tồn kho cho IDSanPham {sanPhamId}: {totalStock}");
            if (totalStock < soLuong)
                throw new Exception($"Số lượng vượt quá tồn kho. Yêu cầu: {soLuong}, Tồn kho: {totalStock}");

            decimal donGia = sanPhamChiTiets.FirstOrDefault()?.GiaBan ?? 0m;
            if (donGia <= 0) throw new Exception("Giá bán không hợp lệ.");
            Console.WriteLine($"Đơn giá cho IDSanPham {sanPhamId}: {donGia}");

            var existingItem = trackedGioHang.GioHangChiTiets
                .FirstOrDefault(g => g.IDSanPham == sanPhamId); // Cần mở rộng để kiểm tra chi tiết nếu có

            if (existingItem != null)
            {
                existingItem.SoLuong += soLuong;
                if (totalStock < existingItem.SoLuong)
                    throw new Exception($"Số lượng vượt quá tồn kho. Yêu cầu: {existingItem.SoLuong}, Tồn kho: {totalStock}");
                _db.Entry(existingItem).State = EntityState.Modified;
            }
            else
            {
                var gioHangCT = new GioHangCT
                {
                    IDGioHangChiTiet = Guid.NewGuid(),
                    IDGioHang = trackedGioHang.IDGioHang,
                    IDSanPham = sanPhamId,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    TrangThai = true
                };
                trackedGioHang.GioHangChiTiets.Add(gioHangCT);
                _db.GioHangChiTiets.Add(gioHangCT);
            }

            try
            {
                await _db.SaveChangesAsync();
                Console.WriteLine($"Đã thêm GioHangCT với ID: {trackedGioHang.GioHangChiTiets.Last().IDGioHangChiTiet}, IDGioHang: {trackedGioHang.IDGioHang}, IDSanPham: {sanPhamId}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.Reload();
                await _db.SaveChangesAsync();
                Console.WriteLine($"Xử lý concurrency cho GioHangCT với ID: {trackedGioHang.GioHangChiTiets.Last().IDGioHangChiTiet}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateGioHangAsync(Guid gioHangCTId, int soLuong)
        {
            var item = await _db.GioHangChiTiets
                .Include(g => g.SanPham)
                .ThenInclude(s => s.SanPhamChiTiets)
                .FirstOrDefaultAsync(g => g.IDGioHangChiTiet == gioHangCTId);

            if (item == null) throw new Exception("Không tìm thấy sản phẩm trong giỏ hàng.");
            var totalStock = item.SanPham.SanPhamChiTiets?.Sum(sct => sct.SoLuongTonKho) ?? 0;
            if (totalStock < soLuong)
                throw new Exception("Số lượng vượt quá tồn kho.");

            item.SoLuong = soLuong;
            await _db.SaveChangesAsync();
        }

        public async Task RemoveFromGioHangAsync(Guid gioHangCTId)
        {
            var item = await _db.GioHangChiTiets.FindAsync(gioHangCTId);
            if (item != null)
            {
                _db.GioHangChiTiets.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<GioHangCT>> GetGioHangChiTietsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");

            if (!Guid.TryParse(userId, out Guid userGuid))
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var gioHang = await _db.GioHangs
                .Include(g => g.GioHangChiTiets)
                .ThenInclude(gct => gct.SanPham)
                .FirstOrDefaultAsync(g => g.IDGioHang == userGuid);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    IDGioHang = userGuid,
                    TrangThai = true,
                    GioHangChiTiets = new List<GioHangCT>()
                };
                _db.GioHangs.Add(gioHang);
                await _db.SaveChangesAsync();
                Console.WriteLine($"Đã tạo mới GioHang với ID: {gioHang.IDGioHang} vì không tìm thấy.");
            }
            else
            {
                Console.WriteLine($"Đã tìm thấy GioHang với ID: {gioHang.IDGioHang}, Số lượng GioHangChiTiets: {gioHang.GioHangChiTiets?.Count ?? 0}");
            }

            return gioHang.GioHangChiTiets.Where(g => g.TrangThai).ToList();
        }
    }
}
