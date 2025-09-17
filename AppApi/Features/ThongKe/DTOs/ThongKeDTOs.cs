namespace AppApi.Features.ThongKe.DTOs
{
    public class TimeRangeRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeGroupType GroupType { get; set; } = TimeGroupType.Day;
    }

    public enum TimeGroupType
    {
        Hour,
        Day,
        Week,
        Month,
        Quarter,
        Year
    }

    public class DashboardOverviewDto
    {
        public RevenueSummaryDto Revenue { get; set; } = new();
        public OrderSummaryDto Orders { get; set; } = new();
        public CustomerSummaryDto Customers { get; set; } = new();
        public InventorySummaryDto Inventory { get; set; } = new();
        public EmployeeSummaryDto Employees { get; set; } = new();
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueChangePercentage { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string Currency { get; set; } = "VND";
        public List<RevenueTrendDto> Trends { get; set; } = new();
    }

    public class RevenueTrendDto
    {
        public DateTime TimePeriod { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class OrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal ConversionRate { get; set; }
    }

    public class CustomerSummaryDto
    {
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal RetentionRate { get; set; }
    }

    public class InventorySummaryDto
    {
        public int ActiveProducts { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
    }

    public class RevenueReportDto
    {
        public List<RevenueByTimeDto> RevenueByTime { get; set; } = new();
        public List<RevenueByCategoryDto> RevenueByCategory { get; set; } = new();
        public List<RevenueByRegionDto> RevenueByRegion { get; set; } = new();
    }

    public class RevenueByTimeDto
    {
        public DateTime TimePeriod { get; set; }
        public string Label { get; set; } = string.Empty;
        public string FormattedTimeLabel { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Profit { get; set; }
        public int OrderCount { get; set; }
    }

    public class RevenueByCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RevenueByRegionDto
    {
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int CustomerCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProductReportDto
    {
        public List<BestSellingProductDto> BestSellers { get; set; } = new();
        public List<WorstSellingProductDto> WorstSellers { get; set; } = new();
        public List<InventoryStatusDto> InventoryStatus { get; set; } = new();
    }

    public class BestSellingProductDto
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

    public class WorstSellingProductDto : BestSellingProductDto
    {
        public int DaysInStock { get; set; }
    }

    public class InventoryStatusDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int SafetyStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }

    public class CustomerReportDto
    {
        public List<CustomerValueDto> CustomerSegments { get; set; } = new();
        public List<CustomerActivityDto> CustomerActivities { get; set; } = new();
    }

    public class CustomerValueDto
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

    public class CustomerActivityDto
    {
        public DateTime Date { get; set; }
        public int NewCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
    }

    public class PromotionReportDto
    {
        public List<PromotionEffectivenessDto> Campaigns { get; set; } = new();
        public decimal TotalDiscountAmount { get; set; }
        public int RedeemedCoupons { get; set; }
    }

    public class PromotionEffectivenessDto
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

    public class QuickMetricDto
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Unit { get; set; } = "VND";
        public decimal? CompareToPreviousPeriod { get; set; }
    }

    public class EmployeeReportDto
    {
        public List<EmployeePerformanceDto> TopPerformers { get; set; } = new();
        public List<EmployeeActivityDto> EmployeeActivities { get; set; } = new();
        public EmployeeSummaryDto Summary { get; set; } = new();
    }

    public class EmployeePerformanceDto
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

    public class EmployeeActivityDto
    {
        public DateTime Date { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }
        public int NewHires { get; set; }
        public int Terminations { get; set; }
    }

    public class EmployeeSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int OnLeaveEmployees { get; set; }
        public decimal TurnoverRate { get; set; }
        public decimal AverageTenure { get; set; } // Số năm trung bình
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
        public Dictionary<string, int> EmployeesByPosition { get; set; } = new();
    }
}
