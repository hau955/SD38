using AppApi.IService;
using AppData.Models;
using Microsoft.AspNetCore.Mvc;
using WebModels.Models;

namespace AppApi.Controllers
{
    [Route("api/DanhMuc")]
    [ApiController]
    public class DanhMucController : ControllerBase
    {
        private readonly IDanhMucSPService _service;

        public DanhMucController(IDanhMucSPService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllDanhMucSPsAsync();
            return Ok(new ApiResponse<List<DanhMuc>>
            {
                Message = "Lấy danh sách danh mục thành công",
                Data = result.ToList()
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetDanhMucSPByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DanhMuc danhMuc)
        {
            danhMuc.DanhMucId = Guid.NewGuid(); // Tạo ID mới
            var result = await _service.CreateDanhMucSPAsync(danhMuc);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DanhMuc danhMuc)
        {
            if (id != danhMuc.DanhMucId)
                return BadRequest("ID không khớp.");

            var updated = await _service.UpdateDanhMucSPAsync(danhMuc);
            return updated ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteDanhMucSPAsync(id);
            return deleted ? Ok() : NotFound();
        }
        public class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
