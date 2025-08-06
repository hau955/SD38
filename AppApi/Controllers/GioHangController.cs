using AppApi.IService;
using AppApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        [HttpGet("lay-gio-hang/{idNguoiDung}")]
        public async Task<IActionResult> LayDanhSachGioHang(Guid idNguoiDung)
        {
            var gioHangChiTiets = await _gioHangService.LayDanhSachGioHang(idNguoiDung);
            var result = gioHangChiTiets.Select(ct => new
            {
                ct.IDGioHangChiTiet,
                ct.IDGioHang,
                ct.IDSanPhamCT,
                ct.SoLuong,
                ct.DonGia,
                ct.TrangThai,
                SanPhamCT = new
                {
                    ct.SanPhamCT.IDSanPhamCT,
                    ct.SanPhamCT.IDSanPham, 
                    ct.SanPhamCT.SoLuongTonKho,
                    ct.SanPhamCT.GiaBan,
                    SanPham = new { ct.SanPhamCT.SanPham?.TenSanPham } // Sẽ null do [JsonIgnore]
                }
            }).ToList();
            return Ok(new { message = "Lấy giỏ hàng thành công", data = result });
        }

        [HttpPost("them")]
        public async Task<IActionResult> ThemVaoGioHang([FromBody] GioHangThemModel model)
        {
            try
            {
                if (model == null || model.idSanPhamCT == Guid.Empty || model.idNguoiDung == Guid.Empty || model.soLuong <= 0)
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }
                var result = await _gioHangService.ThemVaoGioHang(model.idSanPhamCT, model.idNguoiDung, model.soLuong);
                return Ok(new { message = "Thêm vào giỏ hàng thành công", data = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Them: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi server. Vui lòng thử lại.", detail = ex.Message });
            }
        }

        [HttpDelete("xoa/{idGioHangChiTiet}")]
        public async Task<IActionResult> XoaKhoiGioHang(Guid idGioHangChiTiet)
        {
            var result = await _gioHangService.XoaKhoiGioHang(idGioHangChiTiet);
            return Ok(result);
        }

        [HttpPut("cap-nhat-so-luong")]
        public async Task<IActionResult> CapNhatSoLuong([FromBody] CapNhatSoLuongRequest request)
        {
            var result = await _gioHangService.CapNhatSoLuong(request.idGioHangChiTiet, request.soLuong);
            return Ok(result);
        }

        // Model để ánh xạ body JSON
      
    }
    public class CapNhatSoLuongRequest
    {
        public Guid idGioHangChiTiet { get; set; }
        public int soLuong { get; set; }
    }
    public class GioHangThemModel
    {
        public Guid idSanPhamCT { get; set; }
        public Guid idNguoiDung { get; set; }
        public int soLuong { get; set; }
    }
}

