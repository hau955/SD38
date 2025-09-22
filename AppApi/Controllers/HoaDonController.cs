using AppApi.IService;
using AppApi.Service;
using AppData.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }
    
        
        // 1️⃣ Checkout (tạo hóa đơn từ giỏ hàng)
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromQuery] Guid idUser,
    [FromQuery] Guid idHinhThucTT,
    [FromQuery] Guid? idVoucher = null,
    [FromQuery] Guid? iddiachi = null,
    [FromQuery] string? ghiChu = null)
        {
            try
            {
                var hoaDon = await _hoaDonService.TaoHoaDonTuGioHangAsync(idUser, idHinhThucTT, idVoucher,iddiachi, ghiChu);
                return Ok(new { message = "✅ Checkout thành công", hoaDon });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        // 2️⃣ Xác nhận đơn (admin)
        [HttpPost("{idHoaDon}/xacnhan")]
        public async Task<IActionResult> XacNhan(Guid idHoaDon)
        {
            try
            {
                await _hoaDonService.ChuyenSangDaXacNhan(idHoaDon, "Admin");
                return Ok(new { message = "✅ Hóa đơn đã được xác nhận" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3️⃣ Giao hàng (admin)
        [HttpPost("{idHoaDon}/dangGiao")]
        public async Task<IActionResult> DangGiao(Guid idHoaDon)
        {
            try
            {
                await _hoaDonService.ChuyenSangDangGiao(idHoaDon, "Admin");
                return Ok(new { message = "✅ Hóa đơn đang giao" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4️⃣ Đã giao (admin)
        [HttpPost("{idHoaDon}/daGiao")]
        public async Task<IActionResult> DaGiao(Guid idHoaDon)
        {
            try
            {
                await _hoaDonService.ChuyenSangDaGiao(idHoaDon, "Admin");
                return Ok(new { message = "✅ Hóa đơn đã giao" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5️⃣ Hủy hóa đơn (có thể khách hoặc admin)
        [HttpPost("{idHoaDon}/huy")]
        public async Task<IActionResult> HuyHoaDon(Guid idHoaDon, string nguoiHuy)
        {
            try
            {
                await _hoaDonService.ChuyenSangHuy(idHoaDon, nguoiHuy);
                return Ok(new { message = "✅ Hóa đơn đã bị hủy" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 6️⃣ Lấy chi tiết lịch sử trạng thái
        [HttpGet("{idHoaDon}/lichsu")]
        public async Task<IActionResult> LichSuTrangThai(Guid idHoaDon)
        {
            try
            {
                var lichSu = await _hoaDonService.LayLichSuTrangThaiAsync(idHoaDon);
                return Ok(new { hoaDonId = idHoaDon, lichSu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("user/{idUser}")]
        public async Task<IActionResult> GetByUser(Guid idUser)
        {
            try
            {
                var hoaDons = await _hoaDonService.LayHoaDonTheoUserAsync(idUser);
                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("ChiTiet/{idHoaDon}")]
        public async Task<IActionResult> GetChiTietHoaDon(Guid idHoaDon)
        {
            try
            {
                var hoaDon = await _hoaDonService.XemChiTietHoaDonAsync(idHoaDon);
                return Ok(new
                {
                    success = true,
                    data = hoaDon
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet("diachi/{idUser}")]
        public async Task<IActionResult> GetDiaChiUser(Guid idUser)
        {
            var diaChiList = await _hoaDonService.GetDiaChiByUserAsync(idUser);
            if (!diaChiList.Any())
                return NotFound(new { message = "Chưa có địa chỉ nhận hàng nào cho user này" });

            return Ok(diaChiList);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllVoucher()
        {
            var vouchers = await _hoaDonService.LayTatCaVoucherAsync();
            return Ok(new
            {
                success = true,
                message = "Danh sách voucher đang hoạt động",
                data = vouchers
            });
        }
        [HttpGet("get-hinhthucthanhtoan")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _hoaDonService.GetAllAsync();
            return Ok(data);
        }
    }
}
