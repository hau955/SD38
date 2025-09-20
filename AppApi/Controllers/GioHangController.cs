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

        // ✅ Thêm sản phẩm vào giỏ
        [HttpPost("them")]
        public async Task<IActionResult> ThemSanPham([FromBody] ThemSanPhamRequest request)
        {
            var ketQua = await _gioHangService.ThemSanPhamVaoGioAsync(request.IdUser, request.IdSanPhamCT, request.SoLuong);
            return Ok(new { message = ketQua });
        }

        // ✅ Lấy danh sách sản phẩm trong giỏ
        [HttpGet("lay-danh-sach/{idUser}")]
        public async Task<IActionResult> LayDanhSach(Guid idUser)
        {
            var danhSach = await _gioHangService.LayDanhSachSanPhamAsync(idUser);
            return Ok(danhSach);
        }
    }

    // DTO để nhận dữ liệu từ client
    public class ThemSanPhamRequest
    {
        public Guid IdUser { get; set; }
        public Guid IdSanPhamCT { get; set; }
        public int SoLuong { get; set; }
    }
}

