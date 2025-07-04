using AppApi.IService;
using AppApi.ViewModels.SanPham;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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


       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sanPham = await _sanPhamService.GetByID(id);

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

       
    }
}
