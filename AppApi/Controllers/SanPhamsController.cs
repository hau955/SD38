using AppApi.IService;
using AppApi.ViewModels.SanPham;
using ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppData.Models;

namespace AppApi.Controllers
{
    [Route("api/SanPham")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ApplicationDbContext _context;
        public SanPhamsController(ISanPhamService sanPhamService, ApplicationDbContext context)
        {
            _sanPhamService = sanPhamService;
            _context = context;
        }
        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetSanPhamChiTiet(Guid id)
        //{
        //    var result = await _sanPhamService.GetSanPhamChiTiet(id);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}
        // GET: api/SanPhams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _sanPhamService.GetAll();
                return Ok(new
                {
                    message = "Lấy danh sách sản phẩm thành công.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpGet("with-chi-tiet")]
        public async Task<IActionResult> Get()
        {
            var result = await _sanPhamService.GetAllSanPhamsAsync();
            return Ok(result);
        }

        [HttpGet("by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sanPham = await _sanPhamService.GetSanPhamDetailWithDiscountAsync(id);

                if (sanPham == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm", data = (object?)null });

                return Ok(new { message = "Thành công", data = sanPham });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, detail = ex.StackTrace });
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateSanPham([FromForm] SanPhamCreateRequest model)
        {
            try
            {
                var result = await _sanPhamService.Create(model);
                return Ok(new
                {
                    message = "Thêm sản phẩm thành công!",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi server: " + ex.Message });
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] SanPhamCreateRequest model)
        {
            if (model.IDSanPham == null)
                return BadRequest(new { error = "Thiếu ID sản phẩm để cập nhật." });

            try
            {
                var result = await _sanPhamService.Update(model);
                return Ok(new { message = "Cập nhật sản phẩm thành công!", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpDelete("sanpham/anh/{id}")]
        public async Task<IActionResult> XoaAnh(Guid id)
        {
            var anh = await _context.AnhSanPham.FindAsync(id);
            if (anh == null) return NotFound();

            _context.AnhSanPham.Remove(anh);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("sanpham/anh/{id}/chinh")]
        public async Task<IActionResult> DatAnhChinh(Guid id)
        {
            var anh = await _context.AnhSanPham.FindAsync(id);
            if (anh == null) return NotFound();

            var allAnh = _context.AnhSanPham
                .Where(a => a.IDSanPham == anh.IDSanPham);

            foreach (var a in allAnh)
            {
                a.AnhChinh = false;
            }

            anh.AnhChinh = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
