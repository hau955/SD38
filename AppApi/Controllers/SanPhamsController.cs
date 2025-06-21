using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebModels.Models;

namespace AppApi.Controllers
{
    [Route("api/SanPham")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;
        public SanPhamsController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        // GET: api/SanPhams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _sanPhamService.GetAll();
            return Ok(new
            {
                message = "Lấy danh sách sản phẩm thành công.",
                data = list
            });
        }

        // GET: api/SanPham/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _sanPhamService.GetByID(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            return Ok(new
            {
                message = "Lấy sản phẩm theo ID thành công.",
                data = result
            });
        }

        // POST: api/SanPham
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SanPham sanPham)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ.", errors = ModelState });

            var created = await _sanPhamService.Create(sanPham);

            return CreatedAtAction(nameof(GetById), new { id = created.IDSanPham }, new
            {
                message = "Tạo sản phẩm thành công.",
                data = created
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] SanPham sanPham)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ.", errors = ModelState });

            var existing = await _sanPhamService.GetByID(id);
            if (existing == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm để cập nhật." });

            // Giữ nguyên ảnh cũ nếu không có xử lý upload mới
            sanPham.HinhAnh = existing.HinhAnh;

            var updated = await _sanPhamService.Update(id, sanPham);
            return Ok(new
            {
                message = "Cập nhật sản phẩm thành công.",
                data = updated
            });
        }



        // DELETE: api/SanPham/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sanPhamService.Detele(id);
            if (!result)
                return NotFound(new { message = "Không tìm thấy sản phẩm để xoá." });

            return Ok(new { message = "Xoá sản phẩm thành công." });
        }

        // PATCH: api/SanPham/ToggleStatus/{id}
        [HttpPatch("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var message = await _sanPhamService.Toggle(id);
            if (message.Contains("không tồn tại"))
                return NotFound(new { message });

            return Ok(new { message });
        }
    }
}
