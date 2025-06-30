using Microsoft.AspNetCore.Mvc;
using WebModels.Models;
using AppApi.IService;

namespace AppApi.Controllers
{
    [Route("api/CoAo")]
    [ApiController]
    public class CoAoController : ControllerBase
    {
        private readonly ICoAoService _service;

        public CoAoController(ICoAoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<CoAo>>
            {
                Message = "Lấy danh sách cổ áo thành công",
                Data = list.ToList()
            });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<CoAo> { Message = "Không tìm thấy cổ áo" });

            return Ok(new ApiResponse<CoAo>
            {
                Message = "Lấy cổ áo thành công",
                Data = item
            });

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoAo coao)
        {
            coao.IDCoAo = Guid.NewGuid();
            var created = await _service.CreateAsync(coao);
            return Ok(new ApiResponse<CoAo>
            {
                Message = "Tạo cổ áo thành công",
                Data = created
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CoAo coao)
        {
            var updated = await _service.UpdateAsync(id, coao);
            if (!updated)
                return NotFound(new ApiResponse<string> { Message = "Không tìm thấy để cập nhật" });

            var updatedItem = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<CoAo>
            {
                Message = "Cập nhật cổ áo thành công",
                Data = updatedItem!
            });

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
            ? Ok(new ApiResponse<string> { Message = "Xóa cổ áo thành công", Data = "OK" })
            : NotFound(new ApiResponse<string> { Message = "Không tìm thấy để xóa" });

        }
        public class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
