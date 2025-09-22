using AppApi.IService;
using AppApi.ViewModels.SanPham;
using ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppData.Models;

namespace AppApi.Controllers
{
    [Route("api/SanPham")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ApplicationDbContext _context;
        public SanPhamsController(ISanPhamService sanPhamService, ApplicationDbContext context)
        {
            _sanPhamService = sanPhamService;
            _context = context;
        }
        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetSanPhamChiTiet(Guid id)
        //{
        //    var result = await _sanPhamService.GetSanPhamChiTiet(id);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}
        // GET: api/SanPhams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _sanPhamService.GetAll();
                return Ok(new
                {
                    message = "Lấy danh sách sản phẩm thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpGet("with-chi-tiet")]
        public async Task<IActionResult> Get()
        {
            var result = await _sanPhamService.GetAllSanPhamsAsync();
            return Ok(result);
        }

        [HttpGet("by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sanPham = await _sanPhamService.GetSanPhamDetailWithDiscountAsync(id);

                if (sanPham == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm", data = (object?)null });

                return Ok(new { message = "Thành công", data = sanPham });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, detail = ex.StackTrace });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSanPham([FromForm] SanPhamCreateRequest model)
        {
            try
            {
                var result = await _sanPhamService.Create(model);
                return Ok(new
                {
                    message = "Thêm sản phẩm thành công!",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi server: " + ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] SanPhamCreateRequest model)
        {
            if (model.IDSanPham == null)
                return BadRequest(new { error = "Thiếu ID sản phẩm để cập nhật." });

            try
            {
                var result = await _sanPhamService.Update(model);
                return Ok(new { message = "Cập nhật sản phẩm thành công!", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("sanpham/anh/{id}")]
        public async Task<IActionResult> XoaAnh(Guid id)
        {
            var anh = await _context.AnhSanPham.FindAsync(id);
            if (anh == null) return NotFound();

            _context.AnhSanPham.Remove(anh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("sanpham/anh/{id}/chinh")]
        public async Task<IActionResult> DatAnhChinh(Guid id)
        {
            var anh = await _context.AnhSanPham.FindAsync(id);
            if (anh == null) return NotFound();

            var allAnh = _context.AnhSanPham
                .Where(a => a.IDSanPham == anh.IDSanPham);

            foreach (var a in allAnh)
            {
                a.AnhChinh = false;
            }

            anh.AnhChinh = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSanPhamById(Guid id)
        {
            var result = await _sanPhamService.GetSanPhamDetailWithDiscountAsync(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm" });

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAndFilter([FromQuery] SanPhamSearchRequest request)
        {
            try
            {
                // Validate request
                if (!request.IsValidPriceRange())
                {
                    return BadRequest(new { message = "Giá tối thiểu không được lớn hơn giá tối đa" });
                }

                if (!request.IsValidDiscountPriceRange())
                {
                    return BadRequest(new { message = "Giá sau giảm tối thiểu không được lớn hơn giá tối đa" });
                }

                var allResults = await _sanPhamService.SearchAndFilterAsync(request);

                // Phân trang
                var totalCount = allResults.Count;
                var pagedResults = allResults
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = new SanPhamSearchResponse
                {
                    Data = pagedResults,
                    TotalCount = totalCount,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize
                };

                return Ok(new
                {
                    message = "Tìm kiếm thành công",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        // FIXED VERSION - API GetFilterData với logging và error handling tốt hơn
        [HttpGet("filters/data")]
        public async Task<IActionResult> GetFilterData()
        {
            try
            {
                // Kiểm tra kết nối database
                if (!await _context.Database.CanConnectAsync())
                {
                    return StatusCode(500, new { message = "Không thể kết nối đến cơ sở dữ liệu" });
                }

                // Lấy danh sách danh mục
                var danhMucs = await _context.DanhMucs
                    .Where(dm => dm.TrangThai == true)
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new { dm.DanhMucId, dm.TenDanhMuc })
                    .ToListAsync();

                // Lấy danh sách màu sắc (đảm bảo tên bảng và trường chính xác)
                var mauSacs = await _context.MauSacs
                    .Where(ms => ms.TrangThai == true)
                    .OrderBy(ms => ms.TenMau)
                    .Select(ms => new { ms.IDMauSac, ms.TenMau })
                    .ToListAsync();

                // Lấy danh sách size (đảm bảo tên bảng và trường chính xác)
                var sizes = await _context.Sizes
                    .Where(s => s.TrangThai == true)
                    .OrderBy(s => s.SoSize)
                    .Select(s => new { s.IDSize, s.SoSize })
                    .ToListAsync();

                // Lấy danh sách chất liệu (đảm bảo tên bảng và trường chính xác)
                var chatLieus = await _context.ChatLieus
                    .Where(cl => cl.TrangThai == true)
                    .OrderBy(cl => cl.TenChatLieu)
                    .Select(cl => new { cl.IDChatLieu, cl.TenChatLieu })
                    .ToListAsync();

                // Lấy khoảng giá từ SanPhamChiTiet
                var giaRange = new { GiaMin = 0m, GiaMax = 1000000m };
                try
                {
                    var actualRange = await _context.SanPhamChiTiets
                        .Where(ct => ct.TrangThai == true && ct.SanPham.TrangThai == true)
                        .GroupBy(ct => 1)
                        .Select(g => new {
                            GiaMin = g.Min(ct => ct.GiaBan),
                            GiaMax = g.Max(ct => ct.GiaBan)
                        })
                        .FirstOrDefaultAsync();

                    if (actualRange != null)
                    {
                        giaRange = actualRange;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning - Could not get price range: {ex.Message}");
                    // Sử dụng giá trị mặc định đã khởi tạo ở trên
                }

                // Debug logging
                Console.WriteLine($"Filter data loaded - DanhMuc: {danhMucs.Count}, MauSac: {mauSacs.Count}, Size: {sizes.Count}, ChatLieu: {chatLieus.Count}");

                var result = new
                {
                    message = "Lấy dữ liệu filter thành công",
                    data = new
                    {
                        DanhMucs = danhMucs,
                        MauSacs = mauSacs,
                        Sizes = sizes,
                        ChatLieus = chatLieus,
                        GiaRange = giaRange
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error in GetFilterData: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    message = "Lỗi server khi lấy dữ liệu filter: " + ex.Message,
                    detail = ex.InnerException?.Message ?? ex.StackTrace
                });
            }
        }

        // API lấy gợi ý tìm kiếm
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSearchSuggestions(string? query)
        {
            try
            {
                if (string.IsNullOrEmpty(query) || query.Length < 2)
                {
                    return Ok(new { data = new List<string>() });
                }

                var suggestions = await _context.SanPhams
                    .Where(sp => sp.TrangThai && sp.TenSanPham.ToLower().Contains(query.ToLower()))
                    .Select(sp => sp.TenSanPham)
                    .Distinct()
                    .Take(10)
                    .ToListAsync();

                return Ok(new { data = suggestions });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSearchSuggestions: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}