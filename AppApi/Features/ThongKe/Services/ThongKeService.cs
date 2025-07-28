using AppApi.Features.ThongKe.DTOs;
using AppData.Models;
using AppData.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppApi.Features.ThongKe.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly ApplicationDbContext _context;

        public ThongKeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
        {
            var today = DateTime.Now.Date;
            var thirtyDaysAgo = today.AddDays(-30);
            var sixtyDaysAgo = today.AddDays(-60);

            // Revenue calculations
            var currentPeriodRevenue = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiThanhToan == "Đã thanh toán")
                .SumAsync(h => h.TongTienSauGiam);

            var previousPeriodRevenue = await _context.HoaDons
                .Where(h => h.NgayTao >= sixtyDaysAgo && h.NgayTao < thirtyDaysAgo && h.TrangThaiThanhToan == "Đã thanh toán")
                .SumAsync(h => h.TongTienSauGiam);

            var revenueChangePercentage = previousPeriodRevenue > 0
                ? ((currentPeriodRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100
                : 0;

            // Order calculations
            var totalOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo)
                .CountAsync();

            var completedOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == "Hoàn thành")
                .CountAsync();

            var pendingOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == "Đang xử lý")
                .CountAsync();

            var cancelledOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == "Đã hủy")
                .CountAsync();

            //// Customer calculations
            //var newCustomers = await _context.Users
            //    .Where(u => u.>= thirtyDaysAgo)
            //    .CountAsync();

            var returningCustomers = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo)
                .GroupBy(h => h.IDUser)
                .Where(g => g.Count() > 1)
                .CountAsync();

            // Inventory calculations
            var activeProducts = await _context.SanPhamChiTiets
                .Where(sp => sp.TrangThai == true)
                .CountAsync();

            var lowStockItems = await _context.SanPhamChiTiets
                .Where(sp => sp.SoLuongTonKho <= 10 && sp.SoLuongTonKho > 0)
                .CountAsync();

            var outOfStockItems = await _context.SanPhamChiTiets
                .Where(sp => sp.SoLuongTonKho <= 0)
                .CountAsync();

            // Revenue trends (last 7 days)
            var revenueTrends = await _context.HoaDons
                .Where(h => h.NgayTao >= today.AddDays(-7) && h.TrangThaiThanhToan == "Đã thanh toán")
                .GroupBy(h => h.NgayTao.Value.Date)
                .Select(g => new RevenueTrendDto
                {
                    TimePeriod = g.Key,
                    Label = g.Key.ToString("dd/MM"),
                    Revenue = g.Sum(h => h.TongTienSauGiam)
                })
                .OrderBy(r => r.TimePeriod)
                .ToListAsync();

            var averageOrderValue = totalOrders > 0 ? currentPeriodRevenue / totalOrders : 0;
            var conversionRate = totalOrders > 0 ? (decimal)completedOrders / totalOrders * 100 : 0;
            //var retentionRate = newCustomers > 0 ? (decimal)returningCustomers / newCustomers * 100 : 0;

            return new DashboardOverviewDto
            {
                Revenue = new RevenueSummaryDto
                {
                    TotalRevenue = currentPeriodRevenue,
                    RevenueChangePercentage = revenueChangePercentage,
                    AverageOrderValue = averageOrderValue,
                    Trends = revenueTrends
                },
                Orders = new OrderSummaryDto
                {
                    TotalOrders = totalOrders,
                    CompletedOrders = completedOrders,
                    PendingOrders = pendingOrders,
                    CancelledOrders = cancelledOrders,
                    ConversionRate = conversionRate
                },
                Customers = new CustomerSummaryDto
                {
                    ReturningCustomers = returningCustomers
                },
                Inventory = new InventorySummaryDto
                {
                    ActiveProducts = activeProducts,
                    LowStockItems = lowStockItems,
                    OutOfStockItems = outOfStockItems
                }
            };
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(TimeRangeRequestDto request)
        {
            var revenueByTime = await GetRevenueByTimeAsync(request);
            var revenueByCategory = await GetRevenueByCategoryAsync(request);
            var revenueByRegion = await GetRevenueByRegionAsync(request);

            return new RevenueReportDto
            {
                RevenueByTime = revenueByTime,
                RevenueByCategory = revenueByCategory,
                RevenueByRegion = revenueByRegion
            };
        }

        public async Task<ProductReportDto> GetProductReportAsync(TimeRangeRequestDto request)
        {
            var bestSellers = await _context.HoaDonChiTiets
                .Include(hd => hd.HoaDon)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.SanPham)
                .ThenInclude(sp => sp.DanhMuc)
                .Where(hd => hd.HoaDon.NgayTao >= request.StartDate &&
                           hd.HoaDon.NgayTao <= request.EndDate &&
                           hd.HoaDon.TrangThaiThanhToan == "Đã thanh toán")
                .GroupBy(hd => new { hd.IDSanPhamCT, hd.TenSanPham })
                .Select(g => new BestSellingProductDto
                {
                    ProductId = (int)g.Key.IDSanPhamCT.GetHashCode(),
                    ProductName = g.Key.TenSanPham,
                    ProductCode = g.First().SanPhamCT.IDSanPhamCT.ToString(),
                    Category = g.First().SanPhamCT.SanPham.DanhMuc.TenDanhMuc,
                    QuantitySold = g.Sum(x => x.SoLuongSanPham),
                    Revenue = g.Sum(x => x.GiaSauGiamGia * x.SoLuongSanPham),
                    ProfitMargin = g.Average(x => ((x.GiaSauGiamGia - x.GiaSanPham) / x.GiaSauGiamGia) * 100)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            var worstSellers = await _context.SanPhamChiTiets
                .Include(sp => sp.SanPham)
                .ThenInclude(sp => sp.DanhMuc)
                .Where(sp => sp.TrangThai == true)
                .Select(sp => new WorstSellingProductDto
                {
                    ProductId = sp.IDSanPhamCT.GetHashCode(),
                    ProductName = sp.SanPham.TenSanPham,
                    ProductCode = sp.IDSanPhamCT.ToString(),
                    Category = sp.SanPham.DanhMuc.TenDanhMuc,
                    QuantitySold = sp.HoaDonChiTiets.Where(hd => hd.HoaDon.NgayTao >= request.StartDate &&
                                                          hd.HoaDon.NgayTao <= request.EndDate)
                                                 .Sum(hd => hd.SoLuongSanPham),
                    Revenue = sp.HoaDonChiTiets.Where(hd => hd.HoaDon.NgayTao >= request.StartDate &&
                                                     hd.HoaDon.NgayTao <= request.EndDate)
                                         .Sum(hd => hd.GiaSauGiamGia * hd.SoLuongSanPham),
                    DaysInStock = (DateTime.Now - sp.NgayTao.Value).Days
                })
                .OrderBy(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            var inventoryStatus = await _context.SanPhamChiTiets
                .Include(sp => sp.SanPham)
                .Select(sp => new InventoryStatusDto
                {
                    ProductId = sp.IDSanPhamCT.GetHashCode(),
                    ProductName = sp.SanPham.TenSanPham,
                    ProductCode = sp.IDSanPhamCT.ToString(),
                    CurrentStock = sp.SoLuongTonKho,
                    SafetyStock = 10, // Assuming safety stock is 10
                    StockStatus = sp.SoLuongTonKho <= 0 ? "Hết hàng" :
                                 sp.SoLuongTonKho <= 10 ? "Sắp hết" : "Đủ hàng"
                })
                .ToListAsync();

            return new ProductReportDto
            {
                BestSellers = bestSellers,
                WorstSellers = worstSellers,
                InventoryStatus = inventoryStatus
            };
        }

        public async Task<CustomerReportDto> GetCustomerReportAsync(TimeRangeRequestDto request)
        {
            var customerSegments = await _context.Users
                .Select(u => new CustomerValueDto
                {
                    CustomerId = (int)u.Id.GetHashCode(),
                    CustomerName = u.HoTen,
                    Email = u.Email,
                    Tier = "Standard", // You might have a tier system
                    LifetimeValue = u.HoaDonsAsKhachHang.Where(h => h.TrangThaiThanhToan == "Đã thanh toán")
                                             .Sum(h => h.TongTienSauGiam),
                    OrderCount = u.HoaDonsAsKhachHang.Count(),
                    LastOrderDate = u.HoaDonsAsKhachHang.OrderByDescending(h => h.NgayTao)
                                            .Select(h => h.NgayTao.Value)
                                            .FirstOrDefault()
                })
                .OrderByDescending(c => c.LifetimeValue)
                .Take(50)
                .ToListAsync();

            var customerActivities = await _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate && h.NgayTao <= request.EndDate)
                .GroupBy(h => h.NgayTao.Value.Date)
                .Select(g => new CustomerActivityDto
                {
                    Date = g.Key,
                    NewCustomers = g.Select(h => h.IDUser).Distinct().Count(),
                    RepeatCustomers = g.Where(h => h.HoaDonChiTiets.Count() > 1).Count(),
                    ChurnedCustomers = 0 // This would require more complex logic
                })
                .OrderBy(c => c.Date)
                .ToListAsync();

            return new CustomerReportDto
            {
                CustomerSegments = customerSegments,
                CustomerActivities = customerActivities
            };
        }

        public async Task<PromotionReportDto> GetPromotionReportAsync(TimeRangeRequestDto request)
        {
            // Assuming you have a Promotion/Voucher system
            var campaigns = new List<PromotionEffectivenessDto>();

            var totalDiscountAmount = await _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate &&
                           h.NgayTao <= request.EndDate &&
                           h.TienGiamHoaDon.HasValue)
                .SumAsync(h => h.TienGiamHoaDon.Value);

            return new PromotionReportDto
            {
                Campaigns = campaigns,
                TotalDiscountAmount = totalDiscountAmount,
                RedeemedCoupons = 0 // Implement based on your voucher system
            };
        }

        public async Task<List<QuickMetricDto>> GetQuickMetricsAsync(TimeRangeRequestDto request)
        {
            var currentRevenue = await _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate &&
                           h.NgayTao <= request.EndDate &&
                           h.TrangThaiThanhToan == "Đã thanh toán")
                .SumAsync(h => h.TongTienSauGiam);

            var totalOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate && h.NgayTao <= request.EndDate)
                .CountAsync();

            var averageOrderValue = totalOrders > 0 ? currentRevenue / totalOrders : 0;

            return new List<QuickMetricDto>
            {
                new QuickMetricDto { Label = "Tổng doanh thu", Value = currentRevenue, Unit = "VNĐ" },
                new QuickMetricDto { Label = "Tổng đơn hàng", Value = totalOrders, Unit = "đơn" },
                new QuickMetricDto { Label = "Giá trị đơn trung bình", Value = averageOrderValue, Unit = "VNĐ" }
            };
        }

        private async Task<List<RevenueByTimeDto>> GetRevenueByTimeAsync(TimeRangeRequestDto request)
        {
            var query = _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate &&
                           h.NgayTao <= request.EndDate &&
                           h.TrangThaiThanhToan == "Đã thanh toán");

            return request.GroupType switch
            {
                TimeGroupType.Day => await query
                    .GroupBy(h => h.NgayTao.Value.Date)
                    .Select(g => new RevenueByTimeDto
                    {
                        TimePeriod = g.Key,
                        Label = g.Key.ToString("dd/MM/yyyy"),
                        FormattedTimeLabel = g.Key.ToString("dd/MM"),
                        Revenue = g.Sum(h => h.TongTienSauGiam),
                        Cost = g.Sum(h => h.TongTienTruocGiam - h.TongTienSauGiam),
                        Profit = g.Sum(h => h.TongTienSauGiam) * 0.2m, // Assuming 20% profit margin
                        OrderCount = g.Count()
                    })
                    .OrderBy(r => r.TimePeriod)
                    .ToListAsync(),

                TimeGroupType.Month => await query
                    .GroupBy(h => new { h.NgayTao.Value.Year, h.NgayTao.Value.Month })
                    .Select(g => new RevenueByTimeDto
                    {
                        TimePeriod = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Label = $"{g.Key.Month:00}/{g.Key.Year}",
                        FormattedTimeLabel = $"Tháng {g.Key.Month}",
                        Revenue = g.Sum(h => h.TongTienSauGiam),
                        Cost = g.Sum(h => h.TongTienTruocGiam - h.TongTienSauGiam),
                        Profit = g.Sum(h => h.TongTienSauGiam) * 0.2m,
                        OrderCount = g.Count()
                    })
                    .OrderBy(r => r.TimePeriod)
                    .ToListAsync(),

                _ => new List<RevenueByTimeDto>()
            };
        }

        private async Task<List<RevenueByCategoryDto>> GetRevenueByCategoryAsync(TimeRangeRequestDto request)
        {
            return await _context.HoaDonChiTiets
                .Include(hd => hd.HoaDon)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.SanPham)
                .ThenInclude(sp => sp.DanhMuc)
                .Where(hd => hd.HoaDon.NgayTao >= request.StartDate &&
                           hd.HoaDon.NgayTao <= request.EndDate &&
                           hd.HoaDon.TrangThaiThanhToan == "Đã thanh toán")
                .GroupBy(hd => new { hd.SanPhamCT.SanPham.DanhMuc.DanhMucId, hd.SanPhamCT.SanPham.DanhMuc.TenDanhMuc })
                .Select(g => new RevenueByCategoryDto
                {
                    CategoryId = (int)g.Key.DanhMucId.GetHashCode(),
                    CategoryName = g.Key.TenDanhMuc,
                    Revenue = g.Sum(x => x.GiaSauGiamGia * x.SoLuongSanPham),
                    Percentage = 0 // Calculate after getting total
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();
        }

        private async Task<List<RevenueByRegionDto>> GetRevenueByRegionAsync(TimeRangeRequestDto request)
        {
            return await _context.HoaDons
                .Include(h => h.DiaChiNhanHang)
                .Where(h => h.NgayTao >= request.StartDate &&
                           h.NgayTao <= request.EndDate &&
                           h.TrangThaiThanhToan == "Đã thanh toán")
                .GroupBy(h => new { h.DiaChiNhanHang.HoTenNguoiNhan, h.DiaChiNhanHang.DiaChiChiTiet })
                .Select(g => new RevenueByRegionDto
                {
                    Region = g.Key.HoTenNguoiNhan ?? "Không xác định",
                    City = g.Key.DiaChiChiTiet ?? "Không xác định",
                    Revenue = g.Sum(h => h.TongTienSauGiam),
                    CustomerCount = g.Select(h => h.IDUser).Distinct().Count(),
                    Percentage = 0 // Calculate after getting total
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();
        }
    }
}