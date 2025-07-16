using AppApi.IService;
using AppApi.ViewModels.BanHang;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanHangController : ControllerBase
    {
        private readonly IBanHangService _banHangService;

        public BanHangController(IBanHangService banHangService)
        {
            _banHangService = banHangService;
        }

        [HttpPost("ban-tai-quay")]
        public async Task<IActionResult> BanTaiQuay([FromBody] BanHangViewModel request)
        {
            var result = await _banHangService.BanTaiQuayAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, hoaDonId = result.HoaDonId });
        }
    }
}
