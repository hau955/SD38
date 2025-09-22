using AppApi.Constants;
using AppView.Areas.OrderManagerment.Repositories;
using AppView.Areas.OrderManagerment.ViewModels;
using AppView.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppView.Areas.OrderManagerment.Controllers
{
    [Area("OrderManagerment")]
    //[Authorize(Roles = "Admin,Employee")]
    public class OrderManagermentController : Controller
    {
        private readonly IOrderManagementRepo _orderRepo;
        private readonly IMapper _mapper;

        public OrderManagermentController(IOrderManagementRepo orderRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(OrderFilterViewModel? filter = null)
        {
            try
            {
                filter ??= new OrderFilterViewModel();
                var orders = await _orderRepo.GetOrdersAsync(filter);
                var statisticsResponse = await _orderRepo.GetOrderStatisticsAsync();

                if (!statisticsResponse.IsSuccess)
                {
                    TempData["Error"] = statisticsResponse.Message;
                    return View(new PagedResult<OrderListViewModel>());
                }

                ViewBag.Filter = filter;
                ViewBag.Statistics = statisticsResponse.Data ?? new Dictionary<string, int>();
                ViewBag.OrderStatuses = OrderStatus.AllStatuses;
                ViewBag.PaymentStatuses = PaymentStatus.AllStatuses;
                ViewBag.StatusColors = OrderStatusColors.StatusColors;
                ViewBag.PaymentColors = OrderStatusColors.PaymentColors;

                return PartialView(orders.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in Index: {ex.Message}\n{ex.StackTrace}");
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return PartialView(new PagedResult<OrderListViewModel>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _orderRepo.GetOrderDetailAsync(id);

                if (!response.IsSuccess || response.Data == null)
                {
                    TempData["Error"] = response.Message ?? "Không tìm thấy đơn hàng";
                    return RedirectToAction(nameof(Index));
                }

                // Đảm bảo các trường quan trọng không null
                response.Data.TrangThaiDonHang ??= "Không xác định";
                response.Data.TrangThaiThanhToan ??= "Không xác định";
                response.Data.ChiTietSanPhams ??= new List<OrderItemViewModel>();

                var nextStatuses = OrderStatus.AllowedTransitions.TryGetValue(response.Data.TrangThaiDonHang, out var list)
                    ? list
                    : new List<string>();

                ViewBag.NextStatuses = nextStatuses;
                ViewBag.PaymentStatuses = PaymentStatus.AllStatuses;
                ViewBag.StatusColors = OrderStatusColors.StatusColors;
                ViewBag.PaymentColors = OrderStatusColors.PaymentColors;

                return View(response.Data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatusHistory(Guid id)
        {
            try
            {
                var response = await _orderRepo.GetOrderStatusHistoryAsync(id);

                return Json(new
                {
                    isSuccess = response.IsSuccess,
                    data = response.Data,
                    message = response.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in GetOrderStatusHistory: {ex.Message}");
                return Json(new
                {
                    isSuccess = false,
                    data = (object)null,
                    message = "Lỗi khi lấy lịch sử trạng thái"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(Guid id)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("ID");
                if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
                {
                    TempData["Error"] = "Không thể xác định người dùng";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var result = await _orderRepo.ConfirmOrderAsync(id, userId);
                TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var userIdString = HttpContext.Session.GetString("ID");
                if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không thể xác định người dùng"
                    });
                }

                vm.IDNguoiCapNhat = userId;
                var result = await _orderRepo.UpdateOrderStatusAsync(vm);

                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentStatusViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var userIdString = HttpContext.Session.GetString("ID");
                if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không thể xác định người dùng"
                    });
                }

                vm.IDNguoiCapNhat = userId;
                var result = await _orderRepo.UpdatePaymentStatusAsync(vm);

                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatistics()
        {
            var result = await _orderRepo.GetOrderStatisticsAsync();
            return Json(new
            {
                isSuccess = result.IsSuccess,
                data = result.Data
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderViewModel vm)
        {
            try
            {
                Console.WriteLine($"🎯 CancelOrder called - IDHoaDon: {vm?.IDHoaDon}");
                Console.WriteLine($"📝 LyDoHuy: {vm?.LyDoHuy}");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    Console.WriteLine($"❌ ModelState errors: {string.Join(", ", errors)}");

                    return Json(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = errors
                    });
                }

                var userIdString = HttpContext.Session.GetString("ID");
                Console.WriteLine($"👤 User ID from session: {userIdString}");

                if (!Guid.TryParse(userIdString, out Guid userId) || userId == Guid.Empty)
                {
                    Console.WriteLine("❌ Invalid user ID");
                    return Json(new
                    {
                        success = false,
                        message = "Không xác định được người dùng. Vui lòng đăng nhập lại."
                    });
                }

                vm.IDNguoiHuy = userId;
                Console.WriteLine($"🔄 Calling repository...");

                var result = await _orderRepo.CancelOrderAsync(vm);
                Console.WriteLine($"✅ Repository result: Success={result.IsSuccess}, Message={result.Message}");

                return Json(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    redirectUrl = result.IsSuccess ? Url.Action("Details", new { id = vm.IDHoaDon }) : null
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Exception in CancelOrder: {ex.Message}");
                Console.WriteLine($"📚 StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }
}