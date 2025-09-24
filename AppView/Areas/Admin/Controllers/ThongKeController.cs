using AppApi.Features.ThongKe.DTOs;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.ThongKeViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class ThongKeController : Controller
    {
        private readonly IThongKeRepo _thongKeRepo;

        public ThongKeController(IThongKeRepo thongKeRepo)
        {
            _thongKeRepo = thongKeRepo;
        }

        [HttpGet]
        [Route("")]
        [Route("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var overview = await _thongKeRepo.GetOverviewAsync();
                ViewBag.Title = "Tổng quan thống kê";
                
                // Kiểm tra nếu không có dữ liệu
                if (overview?.Revenue?.TotalRevenue == 0 && overview?.Orders?.TotalOrders == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu thống kê. Vui lòng kiểm tra kết nối API hoặc thêm dữ liệu mẫu.";
                    ViewBag.ShowDebugInfo = true; // Hiển thị thông tin debug
                }
                
                return View(overview);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu thống kê: {ex.Message}";
                return View(new DashboardOverviewViewModel());
            }
        }

        [HttpGet]
        [Route("Revenue")]
        public async Task<IActionResult> Revenue(int page = 1)
        {
            var request = new TimeRangeRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                GroupType = TimeGroupTypeViewModel.Day
            };

            var report = await _thongKeRepo.GetRevenueReportAsync(request);
            ViewBag.Title = "Báo cáo doanh thu";
            ViewBag.Request = request;
            ViewBag.Page = page;
            return View(report);
        }

        [HttpPost]
        [Route("Revenue")]
        public async Task<IActionResult> Revenue(TimeRangeRequestViewModel request, int page = 1)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            }

            try
            {
                var report = await _thongKeRepo.GetRevenueReportAsync(request);
                ViewBag.Title = "Báo cáo doanh thu";
                ViewBag.Request = request;
                ViewBag.Page = page;

                if (report?.RevenueByTime?.Count == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu trong khoảng thời gian đã chọn.";
                }

                return View(report);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return View(new RevenueReportViewModel());
            }
        }

        [HttpGet]
        [Route("Products")]
        public async Task<IActionResult> Products()
        {
            var request = new TimeRangeRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                GroupType = TimeGroupTypeViewModel.Day
            };

            var report = await _thongKeRepo.GetProductReportAsync(request);
            ViewBag.Title = "Báo cáo sản phẩm";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpPost]
        [Route("Products")]
        public async Task<IActionResult> Products(TimeRangeRequestViewModel request)
        {
            var report = await _thongKeRepo.GetProductReportAsync(request);
            ViewBag.Title = "Báo cáo sản phẩm";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpGet]
        [Route("Customers")]
        public async Task<IActionResult> Customers()
        {
            try
            {
                var request = new TimeRangeRequestViewModel
                {
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now,
                    GroupType = TimeGroupTypeViewModel.Day
                };

                var report = await _thongKeRepo.GetCustomerReportAsync(request);
                ViewBag.Title = "Báo cáo khách hàng";
                ViewBag.Request = request;
                
                // Kiểm tra nếu không có dữ liệu
                if (report?.CustomerSegments?.Count == 0 && report?.CustomerActivities?.Count == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu khách hàng. Vui lòng kiểm tra kết nối API hoặc thêm dữ liệu mẫu.";
                    ViewBag.ShowDebugInfo = true;
                }
                
                return View(report);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu khách hàng: {ex.Message}";
                ViewBag.ShowDebugInfo = true;
                return View(new CustomerReportViewModel());
            }
        }

        [HttpPost]
        [Route("Customers")]
        public async Task<IActionResult> Customers(TimeRangeRequestViewModel request)
        {
            try
            {
                var report = await _thongKeRepo.GetCustomerReportAsync(request);
                ViewBag.Title = "Báo cáo khách hàng";
                ViewBag.Request = request;
                
                // Kiểm tra nếu không có dữ liệu
                if (report?.CustomerSegments?.Count == 0 && report?.CustomerActivities?.Count == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu khách hàng trong khoảng thời gian đã chọn.";
                    ViewBag.ShowDebugInfo = true;
                }
                
                return View(report);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu khách hàng: {ex.Message}";
                ViewBag.ShowDebugInfo = true;
                return View(new CustomerReportViewModel());
            }
        }

        [HttpGet]
        [Route("Promotions")]
        public async Task<IActionResult> Promotions()
        {
            var request = new TimeRangeRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                GroupType = TimeGroupTypeViewModel.Day
            };

            var report = await _thongKeRepo.GetPromotionReportAsync(request);
            ViewBag.Title = "Báo cáo khuyến mãi";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpPost]
        [Route("Promotions")]
        public async Task<IActionResult> Promotions(TimeRangeRequestViewModel request)
        {
            var report = await _thongKeRepo.GetPromotionReportAsync(request);
            ViewBag.Title = "Báo cáo khuyến mãi";
            ViewBag.Request = request;
            return View(report);
        }
        [HttpGet]
        [Route("Employees")]
        public async Task<IActionResult> Employees()
        {
            var request = new TimeRangeRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                GroupType = TimeGroupTypeViewModel.Day
            };

            var report = await _thongKeRepo.GetEmployeeReportAsync(request);
            ViewBag.Title = "Báo cáo nhân viên";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpPost]
        [Route("Employees")]
        public async Task<IActionResult> Employees(TimeRangeRequestViewModel request)
        {
            var report = await _thongKeRepo.GetEmployeeReportAsync(request);
            ViewBag.Title = "Báo cáo nhân viên";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> Categories()
        {
            try
            {
                var stats = await _thongKeRepo.GetCategoryStatsAsync();
                ViewBag.Title = "Thống kê danh mục";

                if (stats == null || stats.Count == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu thống kê danh mục.";
                    ViewBag.ShowDebugInfo = true;
                }

                return View(stats);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu thống kê danh mục: {ex.Message}";
                ViewBag.ShowDebugInfo = true;
                return View(new List<CategoryOrderCountViewModel>());
            }
        }
        [HttpGet("GetCustomerOrders")]
        public async Task<IActionResult> GetCustomerOrders(string email)
        {
            try
            {
                var orders = await _thongKeRepo.GetCustomerOrdersByEmailAsync(email);
                return Json(new
                {
                    isSuccess = true,
                    data = orders,
                    message = "Lấy đơn hàng thành công"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    data = new List<CustomerOrderViewModel>(),
                    message = $"Lỗi khi lấy đơn hàng: {ex.Message}"
                });
            }
        }


        [HttpGet]
        [Route("TopAoDai")]
        public async Task<IActionResult> TopAoDai()
        {
            try
            {
                var topSelling = await _thongKeRepo.GetTopSellingAoDaiAsync();
                ViewBag.Title = "Top áo dài bán chạy";

                if (topSelling == null || topSelling.Count == 0)
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu top áo dài bán chạy.";
                    ViewBag.ShowDebugInfo = true;
                }

                return View(topSelling);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi khi tải dữ liệu top áo dài bán chạy: {ex.Message}";
                ViewBag.ShowDebugInfo = true;
                return View(new List<TopSellingAoDaiViewModel>());
            }
        }

        [HttpPost]
        [Route("QuickMetrics")]
        public async Task<IActionResult> GetQuickMetrics([FromBody] TimeRangeRequestViewModel request)
        {
            var metrics = await _thongKeRepo.GetQuickMetricsAsync(request);
            return Json(metrics);
        }
    }
}
