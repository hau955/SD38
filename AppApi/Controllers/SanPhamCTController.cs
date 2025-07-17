using Microsoft.AspNetCore.Mvc;
using AppApi.IService;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamCTController : ControllerBase
    {
        private readonly ISanPhamCTService _sanPhamCTService;

        public SanPhamCTController(ISanPhamCTService sanPhamCTService)
        {
            _sanPhamCTService = sanPhamCTService;
        }

        
        [HttpPost("create-multiple")]
        public async Task<IActionResult> CreateMultipleSanPhamCT([FromBody] List<SanPhamCT> list)
        {
            if (list == null || !list.Any())
                return BadRequest("Danh sách sản phẩm chi tiết rỗng.");

            await _sanPhamCTService.AddRangeAsync(list);
            return Ok(new { message = "Tạo nhiều sản phẩm chi tiết thành công!" });
        }




        [HttpGet("by-sanpham/{id}")]
        public async Task<IActionResult> GetBySanPhamId(Guid id)
        {
            try
            {
                var result = await _sanPhamCTService.GetBySanPhamIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _sanPhamCTService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSanPhamCT([FromBody] SanPhamCT model)
        {
            if (model == null || model.IDSanPhamCT == Guid.Empty)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await _sanPhamCTService.UpdateAsync(model);
            if (!result)
                return NotFound("Không tìm thấy sản phẩm chi tiết để cập nhật.");

            return Ok(new { message = "Cập nhật thành công!" });
        }
        [HttpGet("exists")]
        public async Task<IActionResult> CheckExists(Guid idSanPham, Guid idMau, Guid idSize, Guid idchatlieu)
        {
            var exists = await _sanPhamCTService.ExistsAsync(idSanPham, idMau, idSize, idchatlieu);
            return Ok(exists);
        }
    }


}
