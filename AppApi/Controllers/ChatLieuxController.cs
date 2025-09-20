using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppData.Models;
using AppApi.IService;
using AppData.Models.DTO;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatLieuxController : ControllerBase
    {
        private readonly IChatLieuService _service;

        public ChatLieuxController(IChatLieuService service)
        {
            _service = service;
        }

        // GET: api/ChatLieux
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllChatLieusAsync();
            return Ok(new ApiResponse<List<ChatLieu>>
            {
                Message = "Lấy danh sách chất liệu thành công",
                Data = result.ToList()
            });
        }

        // GET: api/ChatLieux/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatLieu>> GetById(Guid id)
        {
            var result = await _service.GetChatLieuByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(new ApiResponse<ChatLieu>
            {
                Message = "Lấy thông tin chất liệu thành công",
                Data = result
            });
        }

        // PUT: api/ChatLieux/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChatLieu chatLieu)
        {
            if (id != chatLieu.IDChatLieu)
                return BadRequest("ID không khớp.");

            var updated = await _service.UpdateChatLieuAsync(chatLieu);
            return updated ? Ok(new ApiResponse<ChatLieu>
            {
                Message = "Cập nhật chất liệu thành công",
                Data = chatLieu
            }) : NotFound();
        }
       

        // POST: api/ChatLieux
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatLieu chatLieu)
        {
            chatLieu.IDChatLieu = Guid.NewGuid(); // Tạo ID mới
            var result = await _service.CreateChatLieuAsync(chatLieu);
            return Ok(result);
        }
        

        // DELETE: api/ChatLieux/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteChatLieuAsync(id);
            return deleted ? Ok(new ApiResponse<object>
            {
                Message = "Xóa chất liệu thành công",
                Data = null
            }) : NotFound();
        }
        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _service.ToggleChatLieuAsync(id);
            return result
                ? Ok(new ApiResponse<string> {Message = "Đổi trạng thái thành công", Data = "OK" })
                : NotFound(new ApiResponse<string> {Message = "Không tìm thấy chất liệu" });
        }

    }
}
