using AppApi.IService;
using AppApi.ViewModels.SanPham;
using AppView.Areas.Admin.ViewModels;
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
            string? imagePath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    throw new InvalidOperationException("Định dạng ảnh không hợp lệ.");

                // Kiểm tra kích thước ảnh (ví dụ: tối đa 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (model.ImageFile.Length > maxFileSize)
                    throw new InvalidOperationException("Ảnh quá lớn, vui lòng chọn ảnh có kích thước dưới 5MB.");

                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                var fileName = $"{timestamp}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

                // Đảm bảo thư mục tồn tại
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.ImageFile.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Không thể lưu ảnh: " + ex.Message);
                }

                imagePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/{fileName}";
            }
           
            var sanPham = new SanPham
            {
                DanhMucId = model.DanhMucID, // Khóa ngoại đến DanhMuc
                IDSanPham = Guid.NewGuid(),
                TenSanPham = model.TenSanPham,
                MoTa = model.MoTa,
                TrongLuong = model.TrongLuong,
               GioiTinh = model.GioiTinh,
               // HinhAnh = imagePath,
                TrangThai = model.TrangThai,
                NgayTao =  DateTime.Now,
                NgaySua =    DateTime.Now

            };

            try
            {
                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();
            }
           
               catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                var innerStack = ex.InnerException?.StackTrace ?? ex.StackTrace;
                throw new InvalidOperationException($"Lỗi khi lưu sản phẩm: {innerMessage}\nChi tiết: {innerStack}");
            }

        

            return sanPham;
        }


       
       
        public async Task<SanPham?> GetByID(Guid id)
        {
            return await _context.SanPhams
                .Include(s => s.SanPhamGiamGias)
                .Include(s => s.SanPhamChiTiets)
                .FirstOrDefaultAsync(sp => sp.IDSanPham == id);
        }
        public async Task<SanPham> Update(SanPhamCreateRequest model)
        {
            var sanPham = await _context.SanPhams.FindAsync(model.IDSanPham);
            if (sanPham == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sản phẩm.");
            }

            // Xử lý cập nhật ảnh nếu có file mới
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(model.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    throw new InvalidOperationException("Định dạng ảnh không hợp lệ.");

                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (model.ImageFile.Length > maxFileSize)
                    throw new InvalidOperationException("Ảnh quá lớn, vui lòng chọn ảnh nhỏ hơn 5MB.");

                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                var fileName = $"{timestamp}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.ImageFile.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Không thể lưu ảnh: " + ex.Message);
                }

                // Gán đường dẫn ảnh mới
               // sanPham.HinhAnh = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/{fileName}";
            }

            // Cập nhật các thông tin khác
            sanPham.TenSanPham = model.TenSanPham;
            sanPham.MoTa = model.MoTa;
            sanPham.TrongLuong = model.TrongLuong;
            sanPham.GioiTinh = model.GioiTinh;
            sanPham.TrangThai = model.TrangThai;
            sanPham.DanhMucId = model.DanhMucID;
            sanPham.NgaySua = DateTime.Now;

            try
            {
                _context.SanPhams.Update(sanPham);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new InvalidOperationException("Lỗi khi cập nhật sản phẩm: " + innerMessage);
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
            TrongLuong = sp.TrongLuong.Value,
            GioiTinh = sp.GioiTinh.Value,
            TrangThai = sp.TrangThai,
            NgayTao = sp.NgayTao.Value,
            NgaySua = sp.NgaySua.Value,
          
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
                  
                  .Include(sp => sp.DanhMuc)
                  .Select(sp => new SanPhamView
                  {
                      IDSanPham = sp.IDSanPham,
                      TenSanPham = sp.TenSanPham,
                      MoTa = sp.MoTa,
                     // HinhAnh = sp.HinhAnh,
                      TenDanhMuc = sp.DanhMuc.TenDanhMuc,
                      TrangThai=sp.TrangThai,
                      ChiTiets = sp.SanPhamChiTiets.Select(ct => new SanPhamCTViewModel
                      {
                          IDSanPhamCT = ct.IDSanPhamCT,
                          IdMauSac = ct.IDMauSac,
                          MauSac = ct.MauSac != null ? ct.MauSac.TenMau : null,

                          IdSize = ct.IDSize,
                          Size = ct.SizeAo != null ? ct.SizeAo.SoSize : null,

                         
                         
                          GiaBan = ct.GiaBan,
                          SoLuongTonKho = ct.SoLuongTonKho,
                          TrangThai=ct.TrangThai
                      }).ToList()
                  })
                  .ToListAsync();

            return result;
        
    }
    }
}
