using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppData.Models;
using AppApi.IService;

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
            return Ok(result);
        }

        // PUT: api/ChatLieux/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChatLieu chatLieu)
        {
            if (id != chatLieu.IDChatLieu)
                return BadRequest("ID không khớp.");

            var updated = await _service.UpdateChatLieuAsync(chatLieu);
            return updated ? Ok() : NotFound();
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
            return deleted ? Ok() : NotFound();
        }
       
        


    }
}
