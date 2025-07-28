using AppApi.Features.Auth.DTOs;
using AppApi.Features.ThongKe.DTOs;
using AppApi.Features.ThongKe.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Features.ThongKe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                var result = await _thongKeService.GetDashboardOverviewAsync();
                return Ok(ApiResponse<DashboardOverviewDto>.Success(result, "Lấy dữ liệu tổng quan thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<DashboardOverviewDto>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }

        [HttpPost("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromBody] TimeRangeRequestDto request)
        {
            try
            {
                var result = await _thongKeService.GetRevenueReportAsync(request);
                return Ok(ApiResponse<RevenueReportDto>.Success(result, "Lấy báo cáo doanh thu thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RevenueReportDto>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }

        [HttpPost("products")]
        public async Task<IActionResult> GetProductReport([FromBody] TimeRangeRequestDto request)
        {
            try
            {
                var result = await _thongKeService.GetProductReportAsync(request);
                return Ok(ApiResponse<ProductReportDto>.Success(result, "Lấy báo cáo sản phẩm thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ProductReportDto>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }

        [HttpPost("customers")]
        public async Task<IActionResult> GetCustomerReport([FromBody] TimeRangeRequestDto request)
        {
            try
            {
                var result = await _thongKeService.GetCustomerReportAsync(request);
                return Ok(ApiResponse<CustomerReportDto>.Success(result, "Lấy báo cáo khách hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CustomerReportDto>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }

        [HttpPost("promotions")]
        public async Task<IActionResult> GetPromotionReport([FromBody] TimeRangeRequestDto request)
        {
            try
            {
                var result = await _thongKeService.GetPromotionReportAsync(request);
                return Ok(ApiResponse<PromotionReportDto>.Success(result, "Lấy báo cáo khuyến mãi thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<PromotionReportDto>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }

        [HttpPost("quick-metrics")]
        public async Task<IActionResult> GetQuickMetrics([FromBody] TimeRangeRequestDto request)
        {
            try
            {
                var result = await _thongKeService.GetQuickMetricsAsync(request);
                return Ok(ApiResponse<List<QuickMetricDto>>.Success(result, "Lấy chỉ số nhanh thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<QuickMetricDto>>.Fail($"Lỗi: {ex.Message}", 500));
            }
        }
    }
}
