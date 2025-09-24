using AppApi.IService;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangCTController : ControllerBase
    {
        private readonly IGioHangCTService _gioHangCTService;

        public GioHangCTController(IGioHangCTService gioHangCTService)
        {
            _gioHangCTService = gioHangCTService;
        }

        [HttpPost("them")]
        public async Task<IActionResult> Them([FromBody] ThemChiTietRequest request)
        {
            var kq = await _gioHangCTService.ThemChiTietAsync(request.IdUser, request.IdSanPhamCT, request.SoLuong);
            return Ok(new { message = kq });
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> CapNhat([FromBody] CapNhatSoLuongRequest request)
        {
            var kq = await _gioHangCTService.CapNhatSoLuongAsync(request.IdGioHangCT, request.SoLuongMoi);
            return Ok(new { message = kq });
        }

        [HttpDelete("xoa/{idGioHangCT}")]
        public async Task<IActionResult> Xoa(Guid idGioHangCT)
        {
            var kq = await _gioHangCTService.XoaChiTietAsync(idGioHangCT);
            return Ok(new { message = kq });
        }

        [HttpGet("lay-theo-user/{idUser}")]
        public async Task<IActionResult> LayTheoUser(Guid idUser)
        {
            var ds = await _gioHangCTService.LayChiTietTheoUserAsync(idUser);
            return Ok(ds);
        }
    }

    // DTOs
    public class ThemChiTietRequest
    {
        public Guid IdUser { get; set; }
        public Guid IdSanPhamCT { get; set; }
        public int SoLuong { get; set; }
    }

    public class CapNhatSoLuongRequest
    {
        public Guid IdGioHangCT { get; set; }
        public int SoLuongMoi { get; set; }
    }
}
