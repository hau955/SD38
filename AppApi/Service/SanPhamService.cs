using AppApi.IService;
using AppApi.ViewModels.SanPham;
using ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

namespace AppApi.Service
{
    public class SanPhamService : ISanPhamService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SanPhamService(ApplicationDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<SanPham> Create(SanPhamCreateRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var sanPham = new SanPham
            {
                IDSanPham = Guid.NewGuid(),
                TenSanPham = model.TenSanPham,
                MoTa = model.MoTa,
                TrongLuong = model.TrongLuong,
                GioiTinh = model.GioiTinh,
                TrangThai = model.TrangThai,
                DanhMucId = model.DanhMucID,
                NgayTao = DateTime.Now,
                NgaySua = DateTime.Now
            };

            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            // Lưu nhiều ảnh
            if (model.ImageFiles != null && model.ImageFiles.Any())
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var folderPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var file in model.ImageFiles)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext)) continue;

                    if (file.Length > 5 * 1024 * 1024) continue; // Skip ảnh > 5MB

                    var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Thêm vào bảng ảnh
                    var anh = new AnhSanPham
                    {
                        IdAnh = Guid.NewGuid(),
                        IDSanPham = sanPham.IDSanPham,
                        DuongDanAnh = $"images/{fileName}",
                        AnhChinh = false // tất cả mặc định là ảnh phụ
                    };
                    _context.AnhSanPham.Add(anh);
                }

                await _context.SaveChangesAsync();
            }

            return sanPham;
        }





        public async Task<SanPhamView?> GetByID(Guid id)
        {
            return await _context.SanPhams
                .Include(s => s.SanPhamChiTiets)
                .ThenInclude(ct => ct.MauSac)
                .Include(s => s.SanPhamChiTiets)
                .ThenInclude(ct => ct.SizeAo)
                .Include(s => s.SanPhamChiTiets)
                .ThenInclude(ct => ct.ChatLieu)
                .Include(s => s.AnhSanPhams)
                .Where(sp => sp.IDSanPham == id)
                .Select(sp => new SanPhamView
                {
                    IDSanPham = sp.IDSanPham,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.MoTa,
                    TrongLuong = sp.TrongLuong ?? 0,
                    GioiTinh = sp.GioiTinh ?? false,
                    TrangThai = sp.TrangThai,
                    DanhMucID = sp.DanhMucId,
                    TenDanhMuc = sp.DanhMuc!.TenDanhMuc,
                    DanhSachAnh = sp.AnhSanPhams.Select(a => new AnhSanPhamViewModel
                    {
                        IdAnh = a.IdAnh,
                        IDSanPham = a.IDSanPham,
                        DuongDanAnh = a.DuongDanAnh,
                        AnhChinh = a.AnhChinh
                    }).ToList(),
                    ChiTiets = sp.SanPhamChiTiets.Select(ct => new SanPhamCTViewModel
                    {
                        IDSanPhamCT = ct.IDSanPhamCT,
                        IDSanPham = ct.IDSanPham, // Đảm bảo không null
                        TenSanPham = sp.TenSanPham,
                        MoTa = sp.MoTa,
                        TrongLuong = sp.TrongLuong,
                        GioiTinh = sp.GioiTinh,
                        TrangThai = ct.TrangThai,
                        GiaBan = ct.GiaBan,
                        SoLuongTonKho = ct.SoLuongTonKho,
                        IdMauSac = ct.IDMauSac,
                        MauSac = ct.MauSac != null ? ct.MauSac.TenMau : "Không xác định", // Xử lý null với giá trị mặc định
                        IdSize = ct.IDSize,
                        Size = ct.SizeAo != null ? ct.SizeAo.SoSize : "Không xác định", // Xử lý null với giá trị mặc định
                        IDChatLieu = ct.IdChatLieu,
                        ChatLieu = ct.ChatLieu != null ? ct.ChatLieu.TenChatLieu : "Không xác định" // Xử lý null với giá trị mặc định
                    }).ToList()
                }).FirstOrDefaultAsync();
        }
        public async Task<SanPham> Update(SanPhamCreateRequest model)
        {
            var sanPham = await _context.SanPhams.FindAsync(model.IDSanPham);
            if (sanPham == null)
                throw new KeyNotFoundException("Không tìm thấy sản phẩm.");

            // Cập nhật thông tin cơ bản
            sanPham.TenSanPham = model.TenSanPham;
            sanPham.MoTa = model.MoTa;
            sanPham.TrongLuong = model.TrongLuong;
            sanPham.GioiTinh = model.GioiTinh;
            sanPham.TrangThai = model.TrangThai;
            sanPham.DanhMucId = model.DanhMucID;
            sanPham.NgaySua = DateTime.Now;

            // Nếu có ảnh mới
            if (model.ImageFiles != null && model.ImageFiles.Any())
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var folderPath = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                foreach (var file in model.ImageFiles)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(ext)) continue;
                    if (file.Length > 5 * 1024 * 1024) continue;

                    var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var anh = new AnhSanPham
                    {
                        IdAnh = Guid.NewGuid(),
                        IDSanPham = sanPham.IDSanPham,
                        DuongDanAnh = $"images/{fileName}",
                        AnhChinh = false // Mặc định là ảnh phụ
                    };

                    _context.AnhSanPham.Add(anh);
                }
            }

            try
            {
                _context.SanPhams.Update(sanPham);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException("Lỗi khi cập nhật sản phẩm: " + msg);
            }

            return sanPham;
        }

        public async  Task<List<SanPhamView>> GetAll()
        {
            return await _context.SanPhams
        .Include(sp => sp.DanhMuc)
        .Select(sp => new SanPhamView
        {
            IDSanPham = sp.IDSanPham,
            TenSanPham = sp.TenSanPham,
            MoTa = sp.MoTa,
            TrongLuong = sp.TrongLuong ?? 0,
            GioiTinh = sp.GioiTinh ?? false,
            TrangThai = sp.TrangThai,
           
          
            DanhMucID = sp.DanhMucId,
            TenDanhMuc = sp.DanhMuc!.TenDanhMuc
        }).ToListAsync();
        }

        public async Task<List<SanPhamView>> GetAllSanPhamsAsync()
        {
            var result = await _context.SanPhams
                .Include(sp => sp.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(sp => sp.SanPhamChiTiets)
                    .ThenInclude(ct => ct.SizeAo)
                .Include(sp => sp.SanPhamChiTiets)
                    .ThenInclude(ct => ct.ChatLieu)
                .Include(sp => sp.AnhSanPhams)
                
                .Include(sp => sp.DanhMuc)
                .Select(sp => new SanPhamView
                {
                    IDSanPham = sp.IDSanPham,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.MoTa,
                    TrongLuong = sp.TrongLuong ?? 0,
                    GioiTinh = sp.GioiTinh ?? false,
                    TrangThai = sp.TrangThai,
                    DanhMucID = sp.DanhMucId,
                    TenDanhMuc = sp.DanhMuc.TenDanhMuc,
                    DanhSachAnh = sp.AnhSanPhams.Select(a => new AnhSanPhamViewModel
                    {
                        IdAnh = a.IdAnh,
                        IDSanPham = a.IDSanPham,
                        DuongDanAnh = a.DuongDanAnh,
                        AnhChinh = a.AnhChinh
                    }).ToList(),
                    ChiTiets = sp.SanPhamChiTiets.Select(ct => new SanPhamCTViewModel
                    {
                        IDSanPhamCT = ct.IDSanPhamCT,
                        IDSanPham = ct.IDSanPham, // Đảm bảo không bị null

                        TenSanPham = sp.TenSanPham,
                        MoTa = sp.MoTa,
                        TrongLuong = sp.TrongLuong,
                        GioiTinh = sp.GioiTinh,
                        TrangThai = ct.TrangThai,

                        GiaBan = ct.GiaBan,
                        SoLuongTonKho = ct.SoLuongTonKho,

                        IdMauSac = ct.IDMauSac,
                        MauSac = ct.MauSac != null ? ct.MauSac.TenMau : null,

                        IdSize = ct.IDSize,
                        Size = ct.SizeAo != null ? ct.SizeAo.SoSize : null,

                        IDChatLieu = ct.IdChatLieu,
                        ChatLieu = ct.ChatLieu != null ? ct.ChatLieu.TenChatLieu : null
                    }).ToList(),
                    
                }).ToListAsync();

            return result;
        }

        public async Task<SanPhamGiamGiaView?> GetSanPhamChiTiet(Guid idSanPham)
        {
            var sp = await _context.SanPhams
                .Include(x => x.SanPhamChiTiets)
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.IDSanPham == idSanPham);

            if (sp == null) return null;

            var giaGoc = sp.SanPhamChiTiets.FirstOrDefault()?.GiaBan ?? 0;

            // 🔎 Lấy tất cả giảm giá sản phẩm
            var giamGiaSPs = await _context.GiamGiaSanPham
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPham == idSanPham &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // 🔎 Lấy tất cả giảm giá theo danh mục
            var giamGiaDMs = await _context.GiamGiaDanhMuc
                .Include(x => x.GiamGia)
                .Where(x => x.DanhMucId == sp.DanhMucId &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // Gộp lại → nhưng ưu tiên sản phẩm trước
            var allDiscounts = giamGiaSPs.Any() ? giamGiaSPs : giamGiaDMs;

            decimal giaSauGiam = giaGoc;
            decimal? giaTriGiam = null;
            GiamGia? bestDiscount = null;

            foreach (var giamGia in allDiscounts)
            {
                decimal giaTmp = giaGoc;
                decimal soTienGiam = 0;

                if (giamGia.LoaiGiamGia == "PhanTram")
                {
                    soTienGiam = giaGoc * (giamGia.GiaTri / 100);
                    if (giamGia.GiaTriGiamToiDa.HasValue)
                        soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                    giaTmp = giaGoc - soTienGiam;
                }
                else if (giamGia.LoaiGiamGia == "SoTien")
                {
                    soTienGiam = giamGia.GiaTri;
                    giaTmp = Math.Max(0, giaGoc - soTienGiam);
                }

                // Chọn cái giảm nhiều nhất
                if (giaTmp < giaSauGiam)
                {
                    giaSauGiam = giaTmp;
                    giaTriGiam = soTienGiam;
                    bestDiscount = giamGia;
                }
            }

            return new SanPhamGiamGiaView
            {
                IDSanPham = sp.IDSanPham,
                TenSanPham = sp.TenSanPham,
                GiaGoc = giaGoc,
                GiaSauGiam = giaSauGiam,
                TenGiamGia = bestDiscount?.TenGiamGia,
                LoaiGiamGia = bestDiscount?.LoaiGiamGia,
                GiaTriGiam = giaTriGiam
            };
        }


    }
}
