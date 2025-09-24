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





        public async Task<SanPhamView?> GetSanPhamByIdAsync(Guid id)
        {
            var sp = await _context.SanPhams
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.SizeAo)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.ChatLieu)
                .Include(x => x.AnhSanPhams)
                .Include(x => x.DanhMuc)
                .Where(x => x.IDSanPham == id)
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
                        IDSanPham = ct.IDSanPham,
                        TenSanPham = sp.TenSanPham,
                        MoTa = sp.MoTa,
                        TrongLuong = sp.TrongLuong,
                        GioiTinh = sp.GioiTinh,
                        TrangThai = ct.TrangThai,
                        GiaBan = ct.GiaBan,
                        SoLuongTonKho = ct.SoLuongTonKho,

                        IdMauSac = ct.IDMauSac,
                        TenMau = ct.MauSac != null ? ct.MauSac.TenMau : null,   // ✅ đúng
                        IdSize = ct.IDSize,
                        SoSize = ct.SizeAo != null ? ct.SizeAo.SoSize : null,   // ✅ đúng
                        IDChatLieu = ct.IdChatLieu,
                        TenChatLieu = ct.ChatLieu != null ? ct.ChatLieu.TenChatLieu : null, // ✅ đúng
                        ChatLieu = ct.ChatLieu != null ? ct.ChatLieu.TenChatLieu : null
                    }).ToList()


                })
                .FirstOrDefaultAsync();

            return sp;
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

        public async Task<List<SanPhamView>> GetAll()
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

        public async Task<List<SanPhamDetailWithDiscountView>> GetAllSanPhamsAsync()
        {
            var sanPhams = await _context.SanPhams
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.SizeAo)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.ChatLieu)
                .Include(x => x.AnhSanPhams)
                .Include(x => x.DanhMuc)
                .ToListAsync();

            var result = new List<SanPhamDetailWithDiscountView>();

            foreach (var sp in sanPhams)
            {
                // 🔎 Giá gốc = lấy giá rẻ nhất trong chi tiết sp
                var giaGoc = sp.SanPhamChiTiets.Any() ? sp.SanPhamChiTiets.Min(ct => ct.GiaBan) : 0;

                // 🔎 Lấy giảm giá theo sản phẩm
                var giamGiaSPs = await _context.GiamGiaSanPham
                    .Include(x => x.GiamGia)
                    .Where(x => x.IDSanPham == sp.IDSanPham &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

                // 🔎 Lấy giảm giá theo danh mục
                var giamGiaDMs = await _context.GiamGiaDanhMuc
                    .Include(x => x.GiamGia)
                    .Where(x => x.DanhMucId == sp.DanhMucId &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

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

                    if (giaTmp < giaSauGiam)
                    {
                        giaSauGiam = giaTmp;
                        giaTriGiam = soTienGiam;
                        bestDiscount = giamGia;
                    }
                }

                result.Add(new SanPhamDetailWithDiscountView
                {
                    IDSanPham = sp.IDSanPham,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.MoTa,
                    TrongLuong = sp.TrongLuong ?? 0,
                    GioiTinh = sp.GioiTinh ?? false,
                    TrangThai = sp.TrangThai,
                    DanhMucID = sp.DanhMucId,
                    TenDanhMuc = sp.DanhMuc?.TenDanhMuc,
                    DanhSachAnh = sp.AnhSanPhams.Select(a => new AnhSanPhamViewModel
                    {
                        IdAnh = a.IdAnh,
                        IDSanPham = a.IDSanPham,
                        DuongDanAnh = a.DuongDanAnh,
                        AnhChinh = a.AnhChinh
                    }).ToList(),
                    ChiTiets = sp.SanPhamChiTiets.Select(ct =>
                    {
                        decimal giaGocCT = ct.GiaBan;
                        decimal giaSauGiamCT = giaGocCT;
                        decimal? giaTriGiamCT = null;

                        foreach (var giamGia in allDiscounts)
                        {
                            decimal giaTmp = giaGocCT;
                            decimal soTienGiam = 0;

                            if (giamGia.LoaiGiamGia == "PhanTram")
                            {
                                soTienGiam = giaGocCT * (giamGia.GiaTri / 100);
                                if (giamGia.GiaTriGiamToiDa.HasValue)
                                    soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                                giaTmp = giaGocCT - soTienGiam;
                            }
                            else if (giamGia.LoaiGiamGia == "SoTien")
                            {
                                soTienGiam = giamGia.GiaTri;
                                giaTmp = Math.Max(0, giaGocCT - soTienGiam);
                            }

                            if (giaTmp < giaSauGiamCT)
                            {
                                giaSauGiamCT = giaTmp;
                                giaTriGiamCT = soTienGiam;
                            }
                        }

                        return new SanPhamCTViewModel
                        {
                            IDSanPhamCT = ct.IDSanPhamCT,
                            IDSanPham = ct.IDSanPham,
                            TenSanPham = sp.TenSanPham,
                            MoTa = sp.MoTa,
                            TrongLuong = sp.TrongLuong,
                            GioiTinh = sp.GioiTinh,
                            TrangThai = ct.TrangThai,

                            GiaBan = giaGocCT,
                            GiaSauGiam = giaSauGiamCT,   // ✅ thêm giá sau giảm cho từng chi tiết
                            SoLuongTonKho = ct.SoLuongTonKho,

                            IdMauSac = ct.IDMauSac,
                            MauSac = ct.MauSac?.TenMau,
                            IdSize = ct.IDSize,
                            Size = ct.SizeAo?.SoSize,
                            IDChatLieu = ct.IdChatLieu,
                            ChatLieu = ct.ChatLieu?.TenChatLieu
                        };
                    }).ToList(),
                    // Thông tin giảm giá
                    GiaGoc = giaGoc,
                    GiaSauGiam = giaSauGiam,
                    GiaTriGiam = giaTriGiam,
                    TenGiamGia = bestDiscount?.TenGiamGia,
                    LoaiGiamGia = bestDiscount?.LoaiGiamGia
                });
            }

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

        public async Task<SanPhamDetailWithDiscountView?> GetSanPhamDetailWithDiscountAsync(Guid idSanPham)
        {
            var sp = await _context.SanPhams
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.SizeAo)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.ChatLieu)
                .Include(x => x.AnhSanPhams)
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.IDSanPham == idSanPham);

            if (sp == null) return null;

            // 🔎 Tính giá gốc = lấy giá rẻ nhất trong các chi tiết sp
            var giaGoc = sp.SanPhamChiTiets.Any() ? sp.SanPhamChiTiets.Min(ct => ct.GiaBan) : 0;

            // 🔎 Lấy giảm giá theo sản phẩm
            var giamGiaSPs = await _context.GiamGiaSanPham
                .Include(x => x.GiamGia)
                .Where(x => x.IDSanPham == idSanPham &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

            // 🔎 Lấy giảm giá theo danh mục
            var giamGiaDMs = await _context.GiamGiaDanhMuc
                .Include(x => x.GiamGia)
                .Where(x => x.DanhMucId == sp.DanhMucId &&
                            x.GiamGia.NgayBatDau <= DateTime.Now &&
                            x.GiamGia.NgayKetThuc >= DateTime.Now &&
                            x.GiamGia.TrangThai)
                .Select(x => x.GiamGia)
                .ToListAsync();

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

                if (giaTmp < giaSauGiam)
                {
                    giaSauGiam = giaTmp;
                    giaTriGiam = soTienGiam;
                    bestDiscount = giamGia;
                }
            }

            return new SanPhamDetailWithDiscountView
            {
                IDSanPham = sp.IDSanPham,
                TenSanPham = sp.TenSanPham,
                MoTa = sp.MoTa,
                TrongLuong = sp.TrongLuong ?? 0,
                GioiTinh = sp.GioiTinh ?? false,
                TrangThai = sp.TrangThai,
                DanhMucID = sp.DanhMucId,
                TenDanhMuc = sp.DanhMuc?.TenDanhMuc,
                DanhSachAnh = sp.AnhSanPhams.Select(a => new AnhSanPhamViewModel
                {
                    IdAnh = a.IdAnh,
                    IDSanPham = a.IDSanPham,
                    DuongDanAnh = a.DuongDanAnh,
                    AnhChinh = a.AnhChinh
                }).ToList(),
                ChiTiets = sp.SanPhamChiTiets.Select(ct =>
                {
                    decimal giaGocCT = ct.GiaBan;
                    decimal giaSauGiamCT = giaGocCT;
                    decimal? giaTriGiamCT = null;

                    foreach (var giamGia in allDiscounts)
                    {
                        decimal giaTmp = giaGocCT;
                        decimal soTienGiam = 0;

                        if (giamGia.LoaiGiamGia == "PhanTram")
                        {
                            soTienGiam = giaGocCT * (giamGia.GiaTri / 100);
                            if (giamGia.GiaTriGiamToiDa.HasValue)
                                soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                            giaTmp = giaGocCT - soTienGiam;
                        }
                        else if (giamGia.LoaiGiamGia == "SoTien")
                        {
                            soTienGiam = giamGia.GiaTri;
                            giaTmp = Math.Max(0, giaGocCT - soTienGiam);
                        }

                        if (giaTmp < giaSauGiamCT)
                        {
                            giaSauGiamCT = giaTmp;
                            giaTriGiamCT = soTienGiam;
                        }
                    }

                    return new SanPhamCTViewModel
                    {
                        IDSanPhamCT = ct.IDSanPhamCT,
                        IDSanPham = ct.IDSanPham,
                        TenSanPham = sp.TenSanPham,
                        MoTa = sp.MoTa,
                        TrongLuong = sp.TrongLuong,
                        GioiTinh = sp.GioiTinh,
                        TrangThai = ct.TrangThai,

                        GiaBan = giaGocCT,
                        GiaSauGiam = giaSauGiamCT,   // ✅ thêm giá sau giảm cho từng chi tiết
                        SoLuongTonKho = ct.SoLuongTonKho,

                        IdMauSac = ct.IDMauSac,
                        MauSac = ct.MauSac?.TenMau,
                        IdSize = ct.IDSize,
                        Size = ct.SizeAo?.SoSize,
                        IDChatLieu = ct.IdChatLieu,
                        ChatLieu = ct.ChatLieu?.TenChatLieu
                    };
                }).ToList(),

                // Thêm info giảm giá
                GiaGoc = giaGoc,
                GiaSauGiam = giaSauGiam,
                GiaTriGiam = giaTriGiam,
                TenGiamGia = bestDiscount?.TenGiamGia,
                LoaiGiamGia = bestDiscount?.LoaiGiamGia
            };
        }

        public async Task<List<SanPhamDetailWithDiscountView>> SearchAndFilterAsync(SanPhamSearchRequest request)
        {
            var query = _context.SanPhams
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.SizeAo)
                .Include(x => x.SanPhamChiTiets)
                    .ThenInclude(ct => ct.ChatLieu)
                .Include(x => x.AnhSanPhams)
                .Include(x => x.DanhMuc)
                .AsQueryable();

            // Lọc theo tên sản phẩm
            if (!string.IsNullOrEmpty(request.Ten))
            {
                query = query.Where(sp => sp.TenSanPham.ToLower().Contains(request.Ten.ToLower()));
            }

            // Lọc theo danh mục
            if (request.DanhMucId.HasValue)
            {
                query = query.Where(sp => sp.DanhMucId == request.DanhMucId.Value);
            }

            // Lọc theo giới tính
            if (request.GioiTinh.HasValue)
            {
                query = query.Where(sp => sp.GioiTinh == request.GioiTinh.Value);
            }

            // Lọc theo trạng thái
            if (request.TrangThai.HasValue)
            {
                query = query.Where(sp => sp.TrangThai == request.TrangThai.Value);
            }
            else
            {
                // Mặc định chỉ lấy sản phẩm đang hoạt động
                query = query.Where(sp => sp.TrangThai == true);
            }

            // Lọc theo màu sắc
            if (!string.IsNullOrEmpty(request.MauSac))
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct =>
                    ct.MauSac != null && ct.MauSac.TenMau.ToLower().Contains(request.MauSac.ToLower())));
            }

            // Lọc theo size
            if (!string.IsNullOrEmpty(request.Size))
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct =>
                    ct.SizeAo != null && ct.SizeAo.SoSize.ToLower().Contains(request.Size.ToLower())));
            }

            // Lọc theo chất liệu
            if (!string.IsNullOrEmpty(request.ChatLieu))
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct =>
                    ct.ChatLieu != null && ct.ChatLieu.TenChatLieu.ToLower().Contains(request.ChatLieu.ToLower())));
            }

            // Lọc theo khoảng giá
            if (request.GiaMin.HasValue || request.GiaMax.HasValue)
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct =>
                    (!request.GiaMin.HasValue || ct.GiaBan >= request.GiaMin.Value) &&
                    (!request.GiaMax.HasValue || ct.GiaBan <= request.GiaMax.Value)));
            }

            var sanPhams = await query.ToListAsync();
            var result = new List<SanPhamDetailWithDiscountView>();

            foreach (var sp in sanPhams)
            {
                // Tính giá gốc = lấy giá rẻ nhất trong chi tiết sp
                var giaGoc = sp.SanPhamChiTiets.Any() ? sp.SanPhamChiTiets.Min(ct => ct.GiaBan) : 0;

                // Lấy giảm giá theo sản phẩm
                var giamGiaSPs = await _context.GiamGiaSanPham
                    .Include(x => x.GiamGia)
                    .Where(x => x.IDSanPham == sp.IDSanPham &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

                // Lấy giảm giá theo danh mục
                var giamGiaDMs = await _context.GiamGiaDanhMuc
                    .Include(x => x.GiamGia)
                    .Where(x => x.DanhMucId == sp.DanhMucId &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

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

                    if (giaTmp < giaSauGiam)
                    {
                        giaSauGiam = giaTmp;
                        giaTriGiam = soTienGiam;
                        bestDiscount = giamGia;
                    }
                }

                // Lọc theo giá sau khi tính giảm giá (nếu cần)
                if ((request.GiaMinSauGiam.HasValue && giaSauGiam < request.GiaMinSauGiam.Value) ||
                    (request.GiaMaxSauGiam.HasValue && giaSauGiam > request.GiaMaxSauGiam.Value))
                {
                    continue;
                }

                result.Add(new SanPhamDetailWithDiscountView
                {
                    IDSanPham = sp.IDSanPham,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.MoTa,
                    TrongLuong = sp.TrongLuong ?? 0,
                    GioiTinh = sp.GioiTinh ?? false,
                    TrangThai = sp.TrangThai,
                    DanhMucID = sp.DanhMucId,
                    TenDanhMuc = sp.DanhMuc?.TenDanhMuc,
                    DanhSachAnh = sp.AnhSanPhams.Select(a => new AnhSanPhamViewModel
                    {
                        IdAnh = a.IdAnh,
                        IDSanPham = a.IDSanPham,
                        DuongDanAnh = a.DuongDanAnh,
                        AnhChinh = a.AnhChinh
                    }).ToList(),
                    ChiTiets = sp.SanPhamChiTiets.Select(ct =>
                    {
                        decimal giaGocCT = ct.GiaBan;
                        decimal giaSauGiamCT = giaGocCT;
                        decimal? giaTriGiamCT = null;

                        foreach (var giamGia in allDiscounts)
                        {
                            decimal giaTmp = giaGocCT;
                            decimal soTienGiam = 0;

                            if (giamGia.LoaiGiamGia == "PhanTram")
                            {
                                soTienGiam = giaGocCT * (giamGia.GiaTri / 100);
                                if (giamGia.GiaTriGiamToiDa.HasValue)
                                    soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                                giaTmp = giaGocCT - soTienGiam;
                            }
                            else if (giamGia.LoaiGiamGia == "SoTien")
                            {
                                soTienGiam = giamGia.GiaTri;
                                giaTmp = Math.Max(0, giaGocCT - soTienGiam);
                            }

                            if (giaTmp < giaSauGiamCT)
                            {
                                giaSauGiamCT = giaTmp;
                                giaTriGiamCT = soTienGiam;
                            }
                        }

                        return new SanPhamCTViewModel
                        {
                            IDSanPhamCT = ct.IDSanPhamCT,
                            IDSanPham = ct.IDSanPham,
                            TenSanPham = sp.TenSanPham,
                            MoTa = sp.MoTa,
                            TrongLuong = sp.TrongLuong,
                            GioiTinh = sp.GioiTinh,
                            TrangThai = ct.TrangThai,

                            GiaBan = giaGocCT,
                            GiaSauGiam = giaSauGiamCT,
                            SoLuongTonKho = ct.SoLuongTonKho,

                            IdMauSac = ct.IDMauSac,
                            MauSac = ct.MauSac?.TenMau,
                            IdSize = ct.IDSize,
                            Size = ct.SizeAo?.SoSize,
                            IDChatLieu = ct.IdChatLieu,
                            ChatLieu = ct.ChatLieu?.TenChatLieu
                        };
                    }).ToList(),

                    GiaGoc = giaGoc,
                    GiaSauGiam = giaSauGiam,
                    GiaTriGiam = giaTriGiam,
                    TenGiamGia = bestDiscount?.TenGiamGia,
                    LoaiGiamGia = bestDiscount?.LoaiGiamGia
                });
            }

            // Sắp xếp kết quả
            switch (request.SapXep?.ToLower())
            {
                case "ten_az":
                    result = result.OrderBy(x => x.TenSanPham).ToList();
                    break;
                case "ten_za":
                    result = result.OrderByDescending(x => x.TenSanPham).ToList();
                    break;
                case "gia_tang":
                    result = result.OrderBy(x => x.GiaSauGiam).ToList();
                    break;
                case "gia_giam":
                    result = result.OrderByDescending(x => x.GiaSauGiam).ToList();
                    break;
                case "moi_nhat":
                    result = result.OrderByDescending(x => x.IDSanPham).ToList(); // Giả sử ID mới hơn có giá trị lớn hơn
                    break;
                default:
                    result = result.OrderBy(x => x.TenSanPham).ToList();
                    break;
            }

            return result;
        }
        public async Task<List<SanPhamDetailWithDiscountView>> QuickFilterAsync(SanPhamQuickFilterRequest request)
        {
            var query = _context.SanPhams
                .Include(sp => sp.SanPhamChiTiets).ThenInclude(ct => ct.MauSac)
                .Include(sp => sp.SanPhamChiTiets).ThenInclude(ct => ct.SizeAo)
                .Include(sp => sp.SanPhamChiTiets).ThenInclude(ct => ct.ChatLieu)
                .Include(sp => sp.AnhSanPhams)
                .Include(sp => sp.DanhMuc)
                .AsQueryable();

            // lọc danh mục
            if (request.DanhMucId != null && request.DanhMucId.Any())
            {
                query = query.Where(sp => request.DanhMucId.Contains(sp.DanhMucId));
            }

            // lọc màu sắc
            if (request.IDMauSac != null && request.IDMauSac.Any())
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct => request.IDMauSac.Contains(ct.IDMauSac)));
            }

            // lọc chất liệu
            if (request.IDChatLieu != null && request.IDChatLieu.Any())
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct => request.IDChatLieu.Contains(ct.IdChatLieu)));
            }

            // lọc size
            if (request.IDSize != null && request.IDSize.Any())
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct => request.IDSize.Contains(ct.IDSize)));
            }

            // lọc giá
            if (request.GiaMin.HasValue || request.GiaMax.HasValue)
            {
                query = query.Where(sp => sp.SanPhamChiTiets.Any(ct =>
                    (!request.GiaMin.HasValue || ct.GiaBan >= request.GiaMin.Value) &&
                    (!request.GiaMax.HasValue || ct.GiaBan <= request.GiaMax.Value)));
            }

            var sanPhams = await query.ToListAsync();
            var result = new List<SanPhamDetailWithDiscountView>();

            foreach (var sp in sanPhams)
            {
                var giaGoc = sp.SanPhamChiTiets.Any() ? sp.SanPhamChiTiets.Min(ct => ct.GiaBan) : 0;

                // lấy discount theo sp và danh mục (giống GetAllSanPhamsAsync)
                var giamGiaSPs = await _context.GiamGiaSanPham
                    .Include(x => x.GiamGia)
                    .Where(x => x.IDSanPham == sp.IDSanPham &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

                var giamGiaDMs = await _context.GiamGiaDanhMuc
                    .Include(x => x.GiamGia)
                    .Where(x => x.DanhMucId == sp.DanhMucId &&
                                x.GiamGia.NgayBatDau <= DateTime.Now &&
                                x.GiamGia.NgayKetThuc >= DateTime.Now &&
                                x.GiamGia.TrangThai)
                    .Select(x => x.GiamGia)
                    .ToListAsync();

                var allDiscounts = giamGiaSPs.Any() ? giamGiaSPs : giamGiaDMs;

                decimal giaSauGiam = giaGoc;
                decimal? giaTriGiam = null;
                foreach (var giamGia in allDiscounts)
                {
                    var tmp = giaGoc;
                    var soTienGiam = 0m;

                    if (giamGia.LoaiGiamGia == "PhanTram")
                    {
                        soTienGiam = giaGoc * (giamGia.GiaTri / 100);
                        if (giamGia.GiaTriGiamToiDa.HasValue)
                            soTienGiam = Math.Min(soTienGiam, giamGia.GiaTriGiamToiDa.Value);

                        tmp = giaGoc - soTienGiam;
                    }
                    else if (giamGia.LoaiGiamGia == "SoTien")
                    {
                        soTienGiam = giamGia.GiaTri;
                        tmp = Math.Max(0, giaGoc - soTienGiam);
                    }

                    if (tmp < giaSauGiam)
                    {
                        giaSauGiam = tmp;
                        giaTriGiam = soTienGiam;
                    }
                }

                result.Add(new SanPhamDetailWithDiscountView
                {
                    IDSanPham = sp.IDSanPham,
                    TenSanPham = sp.TenSanPham,
                    MoTa = sp.MoTa,
                    TenDanhMuc = sp.DanhMuc?.TenDanhMuc,
                    GiaGoc = giaGoc,
                    GiaSauGiam = giaSauGiam,
                    GiaTriGiam = giaTriGiam,
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
                        GiaBan = ct.GiaBan,
                        SoLuongTonKho = ct.SoLuongTonKho,
                        IdMauSac = ct.IDMauSac,
                        MauSac = ct.MauSac?.TenMau,
                        IdSize = ct.IDSize,
                        Size = ct.SizeAo?.SoSize,
                        IDChatLieu = ct.IdChatLieu,
                        ChatLieu = ct.ChatLieu?.TenChatLieu
                    }).ToList()
                });
            }

            return result;
        }
        public async Task<SanPhamFilterDataResponse> GetFilterDataAsync()
        {
            var result = new SanPhamFilterDataResponse
            {
                DanhMucs = await _context.DanhMucs.ToListAsync(),
                MauSacs = await _context.MauSacs.ToListAsync(),
                ChatLieus = await _context.ChatLieus.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync()
            };

            return result;
        }


    }
}