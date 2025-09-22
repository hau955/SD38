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
        public async Task<IActionResult> Revenue()
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
            return View(report);
        }

        [HttpPost]
        [Route("Revenue")]
        public async Task<IActionResult> Revenue(TimeRangeRequestViewModel request)
        {
            var report = await _thongKeRepo.GetRevenueReportAsync(request);
            ViewBag.Title = "Báo cáo doanh thu";
            ViewBag.Request = request;
            return View(report);
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
        [HttpPost]
        [Route("QuickMetrics")]
        public async Task<IActionResult> GetQuickMetrics([FromBody] TimeRangeRequestViewModel request)
        {
            var metrics = await _thongKeRepo.GetQuickMetricsAsync(request);
            return Json(metrics);
        }
    }
}
