using AppView.Areas.Admin.ViewModels.ThongKeViewModel;

namespace AppView.Areas.Admin.IRepo
{
    public interface IThongKeRepo
    {
        Task<DashboardOverviewViewModel> GetOverviewAsync();
        Task<RevenueReportViewModel> GetRevenueReportAsync(TimeRangeRequestViewModel request);
        Task<ProductReportViewModel> GetProductReportAsync(TimeRangeRequestViewModel request);
        Task<CustomerReportViewModel> GetCustomerReportAsync(TimeRangeRequestViewModel request);
        Task<PromotionReportViewModel> GetPromotionReportAsync(TimeRangeRequestViewModel request);
        Task<EmployeeReportViewModel> GetEmployeeReportAsync(TimeRangeRequestViewModel request);
        Task<List<QuickMetricViewModel>> GetQuickMetricsAsync(TimeRangeRequestViewModel request);
    }
}
