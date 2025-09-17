using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppApi.Features.ShippingAddress.DTOs;
using AppApi.Features.ShippingAddress.Service;
using System.Security.Claims;

namespace AppApi.Features.ShippingAddress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Yêu cầu authentication
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _shippingAddressService;

        public ShippingAddressController(IShippingAddressService shippingAddressService)
        {
            _shippingAddressService = shippingAddressService;
        }

        // GET: api/ShippingAddress
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingAddressListDto>>> GetUserAddresses()
        {
            try
            {
                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var addresses = await _shippingAddressService.GetByUserIdAsync(userId);

                var result = addresses.Select(x => new ShippingAddressListDto
                {
                    Id = x.Id,
                    DiaChiChiTiet = x.DiaChiChiTiet,
                    HoTenNguoiNhan = x.HoTenNguoiNhan,
                    SoDienThoai = x.SoDienThoai,
                    IsDefault = x.IsDefault,
                    TrangThai = x.TrangThai
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách địa chỉ", error = ex.Message });
            }
        }

        // GET: api/ShippingAddress/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingAddressResponseDto>> GetAddress(Guid id)
        {
            try
            {
                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");
                var address = await _shippingAddressService.GetByIdAsync(id);

                if (address == null || address.IDUser != userId)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ" });
                }

                return Ok(address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thông tin địa chỉ", error = ex.Message });
            }
        }

        // GET: api/ShippingAddress/default
        [HttpGet("default")]
        public async Task<ActionResult<ShippingAddressResponseDto>> GetDefaultAddress()
        {
            try
            {
                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var address = await _shippingAddressService.GetDefaultAddressAsync(userId);

                if (address == null)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ mặc định" });
                }

                return Ok(address);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy địa chỉ mặc định", error = ex.Message });
            }
        }

        // POST: api/ShippingAddress
        [HttpPost]
        public async Task<ActionResult<ShippingAddressResponseDto>> CreateAddress([FromBody] ShippingAddressDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var result = await _shippingAddressService.CreateAsync(dto.IDUser, dto);

                return CreatedAtAction(nameof(GetAddress), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo địa chỉ", error = ex.Message });
            }
        }

        // PUT: api/ShippingAddress/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ShippingAddressResponseDto>> UpdateAddress(Guid id, [FromBody] ShippingAddressDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var result = await _shippingAddressService.UpdateAsync(id, userId, dto);

                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ hoặc bạn không có quyền cập nhật" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật địa chỉ", error = ex.Message });
            }
        }

        // DELETE: api/ShippingAddress/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAddress(Guid id)
        {
            try
            {
                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var result = await _shippingAddressService.DeleteAsync(id, userId);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ hoặc bạn không có quyền xóa" });
                }

                return Ok(new { message = "Xóa địa chỉ thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa địa chỉ", error = ex.Message });
            }
        }

        // PUT: api/ShippingAddress/{id}/set-default
        [HttpPut("{id}/set-default")]
        public async Task<ActionResult> SetDefaultAddress(Guid id)
        {
            try
            {
                //var userId = GetCurrentUserId();
                Guid userId = Guid.Parse("D7636DAD-DA6F-402B-AF3D-08DDCD245669");

                var result = await _shippingAddressService.SetDefaultAsync(id, userId);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy địa chỉ hoặc bạn không có quyền thiết lập" });
                }

                return Ok(new { message = "Đã thiết lập địa chỉ mặc định thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi thiết lập địa chỉ mặc định", error = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định user ID");
            }
            return userId;
        }
    }
}