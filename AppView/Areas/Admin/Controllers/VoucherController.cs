using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly IVoucherRepo _voucherService;

        public VoucherController(IVoucherRepo voucherService)
        {
            _voucherService = voucherService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _voucherService.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VoucherVM voucher)
        {
            if (!ModelState.IsValid) return View(voucher);

            var result = await _voucherService.CreateAsync(voucher);
            if (result.IsSuccess) return RedirectToAction(nameof(Index));

            // nếu fail thì báo lỗi
            ViewBag.ErrorMessage = result.Message;
            return View(voucher);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, VoucherVM voucher)
        {
            if (!ModelState.IsValid) return View(voucher);

            var result = await _voucherService.UpdateAsync(id, voucher);
            if (result.IsSuccess) return RedirectToAction(nameof(Index));

            ViewBag.ErrorMessage = result.Message;
            return View(voucher);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            await _voucherService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
