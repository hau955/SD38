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
            try
            {
                var result = await _service.GetAllMauSacsAsync();
                return Ok(new ApiResponse<List<MauSac>>
                {
                    Message = "Lấy danh sách màu sắc thành công",
                    Data = result.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<MauSac>>
                {
                    Message = $"Lỗi server: {ex.Message}",
                    Data = new List<MauSac>()
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new ApiResponse<MauSac> { Message = "ID không hợp lệ" });

                var mauSac = await _service.GetMauSacByIdAsync(id);
                return Ok(new ApiResponse<MauSac>
                {
                    Message = "Lấy màu sắc thành công",
                    Data = mauSac
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<MauSac> { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MauSac>
                {
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MauSac mauSac)
        {
            try
            {
                if (mauSac == null)
                    return BadRequest(new ApiResponse<MauSac> { Message = "Dữ liệu màu sắc không hợp lệ" });

                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse<MauSac> 
                    { 
                        Message = "Dữ liệu không hợp lệ", 
                        Data = mauSac 
                    });

                mauSac.IDMauSac = Guid.NewGuid();
                var result = await _service.CreateMauSacAsync(mauSac);
                
                return Ok(new ApiResponse<MauSac>
                {
                    Message = "Tạo màu sắc thành công",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<MauSac> { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<MauSac> { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MauSac>
                {
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauSac mauSac)
        {
            try
            {
                if (mauSac == null)
                    return BadRequest(new ApiResponse<MauSac> { Message = "Dữ liệu màu sắc không hợp lệ" });

                if (id == Guid.Empty || id != mauSac.IDMauSac)
                    return BadRequest(new ApiResponse<MauSac> { Message = "ID không khớp hoặc không hợp lệ" });

                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse<MauSac> 
                    { 
                        Message = "Dữ liệu không hợp lệ", 
                        Data = mauSac 
                    });

                var updated = await _service.UpdateMauSacAsync(id, mauSac);
                
                return Ok(new ApiResponse<MauSac>
                {
                    Message = "Cập nhật màu sắc thành công",
                    Data = mauSac
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<MauSac> { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<MauSac> { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MauSac>
                {
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteMauSacAsync(id);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object> { Message = "Không tìm thấy màu sắc để xoá" });
                }

                return Ok(new ApiResponse<object>
                {
                    Message = "Xóa màu sắc thành công",
                    Data = null
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<object> { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<object> { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        [HttpPatch("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new ApiResponse<object> { Message = "ID không hợp lệ" });

                var result = await _service.ToggleStatusAsync(id);
                
                return Ok(new ApiResponse<object>
                {
                    Message = "Thay đổi trạng thái thành công",
                    Data = null
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<object> { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        public class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }

    }
}
