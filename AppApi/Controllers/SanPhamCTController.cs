using Microsoft.AspNetCore.Mvc;
using WebModels.Models;
using AppApi.Service;

namespace AppApi.Controllers
{
    [Route("api/SanPhamCT")]
    [ApiController]
    public class SanPhamCTController : ControllerBase
    {
        private readonly ISanPhamCTService _service;

        public SanPhamCTController(ISanPhamCTService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<SanPhamCT>>
            {
                Message = "Lấy danh sách sản phẩm chi tiết thành công",
                Data = result.ToList()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new ApiResponse<SanPhamCT> { Message = "Không tìm thấy sản phẩm chi tiết" });

            return Ok(new ApiResponse<SanPhamCT>
            {
                Message = "Lấy sản phẩm chi tiết thành công",
                Data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SanPhamCT spct)
        {
            var created = await _service.CreateAsync(spct);
            if (created == null)
                return BadRequest(new ApiResponse<string> { Message = "Tạo thất bại, kiểm tra dữ liệu liên kết" });

            return Ok(new ApiResponse<SanPhamCT>
            {
                Message = "Tạo sản phẩm chi tiết thành công",
                Data = created
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamCT spct)
        {
            var updated = await _service.UpdateAsync(id, spct);
            if (!updated)
                return NotFound(new ApiResponse<string> { Message = "Không tìm thấy sản phẩm để cập nhật" });

            var updatedItem = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<SanPhamCT>
            {
                Message = "Cập nhật thành công",
                Data = updatedItem!
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? Ok(new ApiResponse<string> { Message = "Xoá sản phẩm chi tiết thành công", Data = "OK" })
                : NotFound(new ApiResponse<string> { Message = "Không tìm thấy sản phẩm để xoá" });
        }

        [HttpGet("by-sanpham/{idSanPham}")]
        public async Task<IActionResult> GetBySanPhamId(Guid idSanPham)
        {
            // ⚠️ Có thể bạn đang dùng sai hàm. Nếu cần lấy danh sách theo ID sản phẩm cha, hãy viết hàm riêng trong service.
            var allItems = await _service.GetAllAsync();
            var filtered = allItems.Where(x => x.IDSanPham == idSanPham).ToList();

            return Ok(new ApiResponse<List<SanPhamCT>>
            {
                Message = $"Lấy danh sách sản phẩm chi tiết của sản phẩm {idSanPham} thành công",
                Data = filtered
            });
        }
    }

}
