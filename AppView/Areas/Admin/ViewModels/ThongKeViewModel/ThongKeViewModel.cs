namespace AppView.Areas.Admin.ViewModels.ThongKeViewModel
{
    public class TimeRangeRequestViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public TimeGroupTypeViewModel GroupType { get; set; } = TimeGroupTypeViewModel.Day;
    }

    public enum TimeGroupTypeViewModel
    {
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        Year
    }

    public class DashboardOverviewViewModel
    {
        public RevenueSummaryViewModel Revenue { get; set; } = new();
        public OrderSummaryViewModel Orders { get; set; } = new();
        public CustomerSummaryViewModel Customers { get; set; } = new();
        public InventorySummaryViewModel Inventory { get; set; } = new();
        public EmployeeSummaryViewModel Employees { get; set; } = new();
    }

    public class RevenueSummaryViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueChangePercentage { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string Currency { get; set; } = "VND";
        public List<RevenueTrendViewModel> Trends { get; set; } = new();
    }

    public class RevenueTrendViewModel
    {
        public DateTime TimePeriod { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal ConversionRate { get; set; }
    }

    public class CustomerSummaryViewModel
    {
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal RetentionRate { get; set; }
    }

    public class InventorySummaryViewModel
    {
        public int ActiveProducts { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
    }

    public class RevenueReportViewModel
    {
        public List<RevenueByTimeViewModel> RevenueByTime { get; set; } = new();
        public List<RevenueByCategoryViewModel> RevenueByCategory { get; set; } = new();
        public List<RevenueByRegionViewModel> RevenueByRegion { get; set; } = new();
    }

    public class RevenueByTimeViewModel
    {
        public DateTime TimePeriod { get; set; }
        public string Label { get; set; } = string.Empty;
        public string FormattedTimeLabel { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public int OrderCount { get; set; }
    }

    public class RevenueByCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RevenueByRegionViewModel
    {
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int CustomerCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProductReportViewModel
    {
        public List<BestSellingProductViewModel> BestSellers { get; set; } = new();
        public List<WorstSellingProductViewModel> WorstSellers { get; set; } = new();
        public List<InventoryStatusViewModel> InventoryStatus { get; set; } = new();
    }

    public class BestSellingProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal RevenueContributionPercentage { get; set; }
    }

    public class WorstSellingProductViewModel : BestSellingProductViewModel
    {
        public int DaysInStock { get; set; }
    }

    public class InventoryStatusViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int SafetyStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }

    public class CustomerReportViewModel
    {
        public List<CustomerValueViewModel> CustomerSegments { get; set; } = new();
        public List<CustomerActivityViewModel> CustomerActivities { get; set; } = new();
    }

    public class CustomerValueViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tier { get; set; } = string.Empty;
        public decimal LifetimeValue { get; set; }
        public int OrderCount { get; set; }
        public DateTime LastOrderDate { get; set; }
        public decimal RevenueContributionPercentage { get; set; }
    }

    public class CustomerActivityViewModel
    {
        public DateTime Date { get; set; }
        public int NewCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
    }

    public class PromotionReportViewModel
    {
        public List<PromotionEffectivenessViewModel> Campaigns { get; set; } = new();
        public decimal TotalDiscountAmount { get; set; }
        public int RedeemedCoupons { get; set; }
    }

    public class PromotionEffectivenessViewModel
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OrdersUsed { get; set; }
        public decimal RevenueGenerated { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ROI { get; set; }
    }

    public class QuickMetricViewModel
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Unit { get; set; } = "VND";
        public decimal? CompareToPreviousPeriod { get; set; }
    }
    public class EmployeeReportViewModel
    {
        public List<EmployeePerformanceViewModel> TopPerformers { get; set; } = new();
        public List<EmployeeActivityViewModel> EmployeeActivities { get; set; } = new();
        public EmployeeSummaryViewModel Summary { get; set; } = new();
    }

    public class EmployeePerformanceViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        // Thống kê hiệu suất
        public int OrdersProcessed { get; set; }
        public decimal RevenueGenerated { get; set; }
        public int NewCustomersAcquired { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal ConversionRate { get; set; }

        // Đánh giá
        public double PerformanceRating { get; set; } // 1-5 sao
        public DateTime HireDate { get; set; }
    }

    public class EmployeeActivityViewModel
    {
        public DateTime Date { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }
        public int NewHires { get; set; }
        public int Terminations { get; set; }
    }

    public class EmployeeSummaryViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }
        public decimal TurnoverRate { get; set; }
        public decimal AverageTenure { get; set; }
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public Dictionary<string, int> EmployeesByPosition { get; set; } = new();
    }
}
