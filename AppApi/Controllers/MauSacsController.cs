using Microsoft.AspNetCore.Mvc;
using AppApi.IService;
using AppData.Models;

namespace AppApi.Controllers
{
    [Route("api/MauSac")]
    [ApiController]
    public class MauSacController : ControllerBase
    {
        private readonly IMauSacService _service;

        public MauSacController(IMauSacService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllMauSacsAsync();
            return Ok(new ApiResponse<List<MauSac>>
            {
                Message = "Lấy danh sách màu sắc thành công",
                Data = result.ToList()
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var size = await _service.GetMauSacByIdAsync(id);
            if (size == null)
                return NotFound(new ApiResponse<Size> { Message = "Không tìm thấy size" });

            return Ok(new ApiResponse<Size>
            {
                Message = "Lấy size thành công",
                
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MauSac mauSac)
        {
            var result = await _service.CreateMauSacAsync(mauSac);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauSac mauSac)
        {
            if (id != mauSac.IDMauSac)
                return BadRequest("ID không khớp.");

            var updated = await _service.UpdateMauSacAsync(id,mauSac);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteMauSacAsync(id);
            return deleted ? Ok() : NotFound();
        }
        public class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }

    }
}
