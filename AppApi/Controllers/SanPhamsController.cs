using AppApi.Service;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            try
            {
                var sanPham = await _sanPhamService.GetByIDWithDetails(id);

                if (sanPham == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm", data = (object?)null });

                return Ok(new { message = "Thành công", data = sanPham });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, detail = ex.StackTrace });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SanPhamCTViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var sanPham = await _sanPhamService.Create(model);

            return Ok(new ApiResponse<SanPham>
            {
                Message = "Tạo sản phẩm thành công (chưa lưu DB)",
                Data = sanPham
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

            // ✅ Nếu có ảnh mới thì xử lý lưu ảnh và cập nhật đường dẫn
            if (sanPham.ImageFile != null && sanPham.ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(sanPham.ImageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await sanPham.ImageFile.CopyToAsync(stream);
                }

                sanPham.HinhAnh = "/images/" + fileName;
            }
            else
            {
                // ✅ Nếu không có ảnh mới thì giữ ảnh cũ
                sanPham.HinhAnh = existing.HinhAnh;
            }

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
            var result = await _sanPhamService.Delete(id);
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
