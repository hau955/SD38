using AppApi.IService;
using AppView.Clients;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
    public class GioHangChiTietController : Controller
    {
        private readonly IGioHangChiTietService _cartService;

        public GioHangChiTietController(IGioHangChiTietService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("ID");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdStr);
            var items = await _cartService.GetByUserAsync(userId);
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQty(Guid IdGioHangCT, int SoLuongMoi, Guid userId)
        {
            var userIdStr = HttpContext.Session.GetString("ID");
            await _cartService.UpdateQtyAsync(IdGioHangCT, SoLuongMoi);
            return RedirectToAction("Index", new { userIdStr });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid IdGioHangCT, Guid userId)
        {
            var userIdStr = HttpContext.Session.GetString("ID");
            await _cartService.DeleteAsync(IdGioHangCT);
            return RedirectToAction("Index", new { userIdStr });
        }
    }
}
