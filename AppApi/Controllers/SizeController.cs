using Microsoft.AspNetCore.Mvc;
using WebModels.Models;
using AppApi.IService;

namespace AppApi.Controllers
{
    [Route("api/Size")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _service;

        public SizeController(ISizeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sizes = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<Size>>
            {
                Message = "Lấy danh sách size thành công",
                Data = sizes.ToList()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var size = await _service.GetByIdAsync(id);
            if (size == null)
                return NotFound(new ApiResponse<Size> { Message = "Không tìm thấy size" });

            return Ok(new ApiResponse<Size>
            {
                Message = "Lấy size thành công",
                Data = size
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Size size)
        {
            var created = await _service.CreateAsync(size);
            return Ok(new ApiResponse<Size>
            {
                Message = "Tạo size thành công",
                Data = created
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Size size)
        {
            var updated = await _service.UpdateAsync(id, size);
            if (!updated)
                return NotFound(new ApiResponse<Size> { Message = "Không tìm thấy để cập nhật" });

            var updatedItem = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<Size>
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
                ? Ok(new ApiResponse<string> { Message = "Xoá thành công", Data = "OK" })
                : NotFound(new ApiResponse<string> { Message = "Không tìm thấy để xoá" });
        }

        [HttpPatch("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var size = await _service.GetByIdAsync(id);
            if (size == null)
                return NotFound(new ApiResponse<string> { Message = "Không tìm thấy để đổi trạng thái" });

            size.TrangThai = !size.TrangThai;
            size.NgaySua = DateTime.UtcNow;
            await _service.UpdateAsync(id, size);

            return Ok(new ApiResponse<string>
            {
                Message = "Đổi trạng thái thành công",
                Data = size.TrangThai ? "Hoạt động" : "Không hoạt động"
            });
        }
    }

    // Bổ sung class ApiResponse nếu chưa có
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
    }
}
