using AppView.Repository;
using Microsoft.AspNetCore.Mvc;
using WebModels.Models;

namespace AppView.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ISanPhamRepo _sanPhamRepo;
        public SanPhamController(ISanPhamRepo sanPhamRepo)
        {
            _sanPhamRepo = sanPhamRepo;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _sanPhamRepo.GetAll();
                ViewBag.Message = "Lấy danh sách sản phẩm thành công.";
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi khi lấy danh sách sản phẩm: " + ex.Message;
                return View(new List<SanPham>());
            }
        }
        public async Task<IActionResult> Details(Guid id)
        {
            // Kiểm tra ID có hợp lệ không
            if (id == Guid.Empty)
            {
                TempData["Error"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                // Gọi repo để lấy sản phẩm theo ID
                var sanPham = await _sanPhamRepo.GetByID(id);

                if (sanPham == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm.";
                    return RedirectToAction("Index");
                }

                // Trả về view với sản phẩm
                return View(sanPham);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi lấy thông tin sản phẩm: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Create GET
        public IActionResult Create()
        {
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham)
        {
            if (!ModelState.IsValid)
            {
                return View(sanPham);
            }

            try
            {
                // ❌ Không còn xử lý ảnh

                // Gửi sản phẩm qua API
                var created = await _sanPhamRepo.Create(sanPham);

                TempData["Message"] = "Tạo sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo sản phẩm: " + ex.Message);
                return View(sanPham);
            }
        }

        // Edit GET
        public async Task<IActionResult> Edit(Guid id)
        {
            var sanPham = await _sanPhamRepo.GetByID(id);
            if (sanPham == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }
            return View(sanPham);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPham sanPham)
        {
            if (id != sanPham.IDSanPham)
            {
                return BadRequest("ID không khớp.");
            }

            if (!ModelState.IsValid)
            {
                return View(sanPham);
            }

            try
            {
                // ❌ Bỏ phần gán ImageFile
                var updated = await _sanPhamRepo.Update(id, sanPham);
                if (updated == null)
                {
                    return NotFound("Không tìm thấy sản phẩm để cập nhật.");
                }

                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật sản phẩm: " + ex.Message);
                return View(sanPham);
            }
        }



        // GET: /SanPham/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var sanPham = await _sanPhamRepo.GetByID(id);
            if (sanPham == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }
            return View(sanPham);
        }

        // POST: /SanPham/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _sanPhamRepo.Detele(id);
                if (!result)
                {
                    return NotFound("Không tìm thấy sản phẩm để xoá.");
                }
                TempData["Message"] = "Xoá sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xoá sản phẩm: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var message = await _sanPhamRepo.Toggle(id); // Trả về thông báo từ API
                TempData["Message"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật trạng thái sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

    }
}
