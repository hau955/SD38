using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppData.Models;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatLieuxController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatLieuxController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ChatLieux
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatLieu>>> GetChatLieus()
        {
            return await _context.ChatLieus.ToListAsync();
        }

        // GET: api/ChatLieux/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatLieu>> GetChatLieu(Guid id)
        {
            var chatLieu = await _context.ChatLieus.FindAsync(id);

            if (chatLieu == null)
            {
                return NotFound();
            }

            return chatLieu;
        }

        // PUT: api/ChatLieux/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatLieu(Guid id, ChatLieu chatLieu)
        {
            if (id != chatLieu.IDChatLieu)
            {
                return BadRequest();
            }

            _context.Entry(chatLieu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatLieuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChatLieux
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChatLieu>> PostChatLieu(ChatLieu chatLieu)
        {
            _context.ChatLieus.Add(chatLieu);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatLieu", new { id = chatLieu.IDChatLieu }, chatLieu);
        }

        // DELETE: api/ChatLieux/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatLieu(Guid id)
        {
            var chatLieu = await _context.ChatLieus.FindAsync(id);
            if (chatLieu == null)
            {
                return NotFound();
            }

            _context.ChatLieus.Remove(chatLieu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatLieuExists(Guid id)
        {
            return _context.ChatLieus.Any(e => e.IDChatLieu == id);
        }
    }
}
