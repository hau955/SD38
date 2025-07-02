using Microsoft.AspNetCore.Mvc;
using WebModels.Models;
using AppApi.IService;

namespace AppApi.Controllers
{
    [Route("api/TaAo")]
    [ApiController]
    public class TaAoController : ControllerBase
    {
        private readonly ITaAoService _service;

        public TaAoController(ITaAoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<TaAo>>
            {
                Message = "Lấy danh sách tà áo thành công.",
                Data = list.ToList()
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new ApiResponse<TaAo>
                {
                    Message = "Không tìm thấy tà áo.",
                    Data = null!
                });
            }

            return Ok(new ApiResponse<TaAo>
            {
                Message = "Lấy tà áo thành công.",
                Data = item
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaAo taao)
        {
            var created = await _service.CreateAsync(taao);
            return Ok(new ApiResponse<TaAo>
            {
                Message = "Tạo tà áo thành công.",
                Data = created
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaAo taao)
        {
            var updated = await _service.UpdateAsync(id, taao);
            if (!updated)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Không tìm thấy tà áo để cập nhật.",
                    Data = "Failed"
                });
            }

            var updatedItem = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<TaAo>
            {
                Message = "Cập nhật tà áo thành công.",
                Data = updatedItem!
            });
        }

        //[HttpDelete("{id}")]
        //public Task<IActionResult> Delete(Guid id)
        //{
        //    return StatusCode(501, new ApiResponse<string>
        //    {
        //        Message = "Xoá tà áo chưa được cài đặt.",
        //        Data = "Not implemented"
        //    });
        //}
    }
}
