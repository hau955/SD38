using AppApi.Features.ThongKe.DTOs;

namespace AppApi.Features.ThongKe.Services
{
    public interface IThongKeService
    {
        Task<DashboardOverviewDto> GetDashboardOverviewAsync();
        Task<RevenueReportDto> GetRevenueReportAsync(TimeRangeRequestDto request);
        Task<ProductReportDto> GetProductReportAsync(TimeRangeRequestDto request);
        Task<CustomerReportDto> GetCustomerReportAsync(TimeRangeRequestDto request);
        Task<PromotionReportDto> GetPromotionReportAsync(TimeRangeRequestDto request);
        Task<EmployeeReportDto> GetEmployeeReportAsync(TimeRangeRequestDto request);
        Task<List<QuickMetricDto>> GetQuickMetricsAsync(TimeRangeRequestDto request);
    }
}