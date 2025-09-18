using AppApi.IService;
using AppData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaService _service;

        public GiamGiaController(IGiamGiaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var gg = await _service.GetByIdAsync(id);
            if (gg == null) return NotFound();
            return Ok(gg);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGia giamGia)
        {
            var created = await _service.CreateAsync(giamGia);
            return CreatedAtAction(nameof(GetById), new { id = created.IDGiamGia }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGia giamGia)
        {
            giamGia.IDGiamGia = id;
            var result = await _service.UpdateAsync(giamGia);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // Gắn sản phẩm vào giảm giá
        [HttpPost("{giamGiaId}/add-product/{sanPhamId}")]
        public async Task<IActionResult> AddProduct(Guid giamGiaId, Guid sanPhamId)
        {
            var result = await _service.AddProductToDiscountAsync(giamGiaId, sanPhamId);
            if (result == null) return BadRequest("Không thể gắn sản phẩm vào giảm giá");
            return Ok(result);
        }

        // Lấy danh sách sản phẩm trong 1 giảm giá
        [HttpGet("{giamGiaId}/products")]
        public async Task<IActionResult> GetProducts(Guid giamGiaId)
        {
            return Ok(await _service.GetProductsByDiscountAsync(giamGiaId));
        }
        [HttpPost("{idGiamGia}/add-category/{idDanhMuc}")]
        public async Task<IActionResult> AddCategory(Guid idGiamGia, Guid idDanhMuc)
        {
            var result = await _service.AddCategoryToDiscount(idGiamGia, idDanhMuc);
            if (!result) return BadRequest("Danh mục đã được gắn giảm giá hoặc không tồn tại");
            return Ok("Gắn giảm giá cho danh mục thành công");
        }
        [HttpPost("{idGiamGia}/add-spct/{idSanPhamCT}")]
        public async Task<IActionResult> AddSanPhamCT(Guid idGiamGia, Guid idSanPhamCT)
        {
            var result = await _service.AddSanPhamCTToDiscountAsync(idGiamGia, idSanPhamCT);
            if (result == null) return BadRequest("Không thể gắn sản phẩm chi tiết vào giảm giá");
            return Ok(result);
        }

        // 🔹 Lấy danh sách SPCT trong 1 giảm giá
        [HttpGet("{idGiamGia}/spcts")]
        public async Task<IActionResult> GetSanPhamCTs(Guid idGiamGia)
        {
            return Ok(await _service.GetSanPhamCTByDiscountAsync(idGiamGia));
        }

    }
}
