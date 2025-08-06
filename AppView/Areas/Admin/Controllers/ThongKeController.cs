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
            var overview = await _thongKeRepo.GetOverviewAsync();
            ViewBag.Title = "Tổng quan thống kê";
            return View(overview);
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
            var request = new TimeRangeRequestViewModel
            {
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now,
                GroupType = TimeGroupTypeViewModel.Day
            };

            var report = await _thongKeRepo.GetCustomerReportAsync(request);
            ViewBag.Title = "Báo cáo khách hàng";
            ViewBag.Request = request;
            return View(report);
        }

        [HttpPost]
        [Route("Customers")]
        public async Task<IActionResult> Customers(TimeRangeRequestViewModel request)
        {
            var report = await _thongKeRepo.GetCustomerReportAsync(request);
            ViewBag.Title = "Báo cáo khách hàng";
            ViewBag.Request = request;
            return View(report);
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
