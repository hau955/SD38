using AppApi.IService;
using AppApi.ViewModels.BanHang;
using AppData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanHangController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IBanHangService _banHangService;

        public BanHangController(IBanHangService banHangService,ApplicationDbContext dbContext)
        {
            _banHangService = banHangService;
            _context = dbContext;
        }

        [HttpPost("ban-tai-quay")]
        public async Task<IActionResult> BanTaiQuay([FromBody] BanHangViewModel request)
        {
            var result = await _banHangService.BanTaiQuayAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, hoaDonId = result.HoaDonId });
        }
        [HttpPost("thanh-toan-hoa-don-cho")]
        public async Task<IActionResult> ThanhToanHoaDonCho([FromBody] ThanhToanHoaDonRequest request)
        {
            var result = await _banHangService.ThanhToanHoaDonChoAsync(request);
            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
        [HttpGet("hoa-don-cho")]
        public async Task<IActionResult> GetHoaDonCho([FromQuery] Guid idNguoiTao)
        {
            var hoaDons = await _context.HoaDons
                .Where(h => h.TrangThaiDonHang == "Chờ thanh toán" && h.TrangThaiThanhToan == "Chưa thanh toán" && h.IDNguoiTao==idNguoiTao)
                .OrderByDescending(h => h.NgayTao)
                .Include(h => h.User2) // <<-- Nạp User2
                .Select(h => new
                {
                    h.IDHoaDon,
                    h.NgayTao,
                    h.TongTienTruocGiam,
                    h.TongTienSauGiam,
                    NguoiTao = h.User2 != null ? h.User2.HoTen : "Không rõ"
                })
                .ToListAsync();


            return Ok(hoaDons);
        }
        [HttpPost("them-san-pham-vao-hoa-don-cho")]
        public async Task<IActionResult> ThemSanPhamVaoHoaDonCho([FromBody] ThemSanPham request)
        {
            var result = await _banHangService.ThemSanPhamVaoHoaDonChoAsync(request);
            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
        [HttpPost("tru-san-pham-hoa-don-cho")]
        public async Task<IActionResult> TruSanPhamHoaDonCho([FromBody] TruSanPham request)
        {
            var result = await _banHangService.TruSanPhamKhoiHoaDonChoAsync(request);
            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
        [HttpPost("huy-hoa-don")]
        public async Task<IActionResult> HuyHoaDon([FromBody] HuyHoaDon request)
        {
            var result = await _banHangService.HuyHoaDonAsync(request.IDHoaDon);
            if (result.IsSuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
        [HttpGet("xem-hoa-don/{idHoaDon}")]
        public async Task<IActionResult> XemHoaDonChiTiet(Guid idHoaDon)
        {
            var result = await _banHangService.XemChiTietHoaDonAsync(idHoaDon);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }
        [HttpGet("tao-link")]
        public async Task<IActionResult> TaoLinkThanhToan([FromQuery] Guid hoaDonId)
        {
            var url = await _banHangService.TaoUrlThanhToanAsync(hoaDonId);
            return Ok(new { Url = url });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayCallback()
        {
            var result = await _banHangService.XuLyKetQuaThanhToanAsync(Request.Query);
            return Ok(result);
        }

    }
}
