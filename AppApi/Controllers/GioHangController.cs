using AppApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GioHangController : ControllerBase
    {
        private readonly GioHangService _gioHangService;

        public GioHangController(GioHangService gioHangService)
        {
            _gioHangService = gioHangService ?? throw new ArgumentNullException(nameof(gioHangService));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                await _gioHangService.AddToGioHangAsync(dto.SanPhamId, dto.SoLuong);
                return Ok(new { message = "Thêm vào giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var items = await _gioHangService.GetGioHangChiTietsAsync();
            return Ok(items);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartDto dto)
        {
            try
            {
                await _gioHangService.UpdateGioHangAsync(dto.GioHangCTId, dto.SoLuong);
                return Ok(new { message = "Cập nhật giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartDto dto)
        {
            try
            {
                await _gioHangService.RemoveFromGioHangAsync(dto.GioHangCTId);
                return Ok(new { message = "Xóa sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class AddToCartDto
    {
        public Guid SanPhamId { get; set; }
        public int SoLuong { get; set; }
    }

    public class UpdateCartDto
    {
        public Guid GioHangCTId { get; set; }
        public int SoLuong { get; set; }
    }

    public class RemoveCartDto
    {
        public Guid GioHangCTId { get; set; }
    }
}

