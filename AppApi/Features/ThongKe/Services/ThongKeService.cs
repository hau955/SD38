using AppApi.Constants;
using AppApi.Features.ThongKe.DTOs;
using AppData.Models;
using AppData.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .SumAsync(h => h.TongTienSauGiam);

            var previousPeriodRevenue = await _context.HoaDons
                .Where(h => h.NgayTao >= sixtyDaysAgo && h.NgayTao < thirtyDaysAgo && h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .SumAsync(h => h.TongTienSauGiam);

            var revenueChangePercentage = previousPeriodRevenue > 0
                ? ((currentPeriodRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100
                : 0;

            // Order calculations
            var totalOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo)
                .CountAsync();

            var completedOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == OrderStatus.HOAN_TAT)
                .CountAsync();

            var pendingOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == OrderStatus.CHO_XAC_NHAN)
                .CountAsync();

            var cancelledOrders = await _context.HoaDons
                .Where(h => h.NgayTao >= thirtyDaysAgo && h.TrangThaiDonHang == OrderStatus.DA_HUY)
                .CountAsync();

            // Customer calculations
            var newCustomers = await _context.Users
                .Where(u => u.HoaDonsAsKhachHang.Any(h => h.NgayTao >= thirtyDaysAgo))
                .CountAsync();

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
                .Where(h => h.NgayTao >= today.AddDays(-7) && h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
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
            var retentionRate = newCustomers > 0 ? (decimal)returningCustomers / newCustomers * 100 : 0;

            // Áo Dài Category Statistics
            var aoDaiStats = await GetCategoryStatsAsync();

            // Top selling Áo Dài
            var topSellingAoDai = await GetTopSellingAoDaiAsync();

            // Top customers
            var topCustomers = await GetTopCustomersAsync();

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
                    ReturningCustomers = returningCustomers,
                    NewCustomers = newCustomers,
                    RetentionRate = retentionRate
                },
                Inventory = new InventorySummaryDto
                {
                    ActiveProducts = activeProducts,
                    LowStockItems = lowStockItems,
                    OutOfStockItems = outOfStockItems
                },
                AoDaiStats = aoDaiStats,
                TopSellingAoDai = topSellingAoDai,
                TopCustomers = topCustomers
            };
        }

        public async Task<List<CategoryOrderCountDto>> GetCategoryStatsAsync()
        {
            var categories = await _context.DanhMucs.ToListAsync();

            var result = await _context.HoaDonChiTiets
                .Include(hd => hd.HoaDon)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.SanPham)
                .Where(hd => hd.HoaDon.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .GroupBy(hd => hd.SanPhamCT.SanPham.DanhMuc.TenDanhMuc)
                .Select(g => new CategoryOrderCountDto
                {
                    CategoryName = g.Key,
                    OrderCount = g.Count()
                })
                .ToListAsync();

            // Merge để đảm bảo những danh mục chưa có đơn hàng vẫn hiện với OrderCount = 0
            var final = categories.Select(c => new CategoryOrderCountDto
            {
                CategoryName = c.TenDanhMuc,
                OrderCount = result.FirstOrDefault(r => r.CategoryName == c.TenDanhMuc)?.OrderCount ?? 0
            }).ToList();

            return final;
        }


        public async Task<List<TopSellingAoDaiDto>> GetTopSellingAoDaiAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var topProducts = await _context.HoaDonChiTiets
                .Include(hd => hd.HoaDon)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.SanPham)
                .ThenInclude(sp => sp.DanhMuc)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.MauSac)
                .Include(hd => hd.SanPhamCT)
                .ThenInclude(sp => sp.SizeAo)
                .Where(hd => hd.HoaDon.NgayTao >= thirtyDaysAgo &&
                           hd.HoaDon.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .GroupBy(hd => new {
                    hd.IDSanPhamCT,
                    hd.TenSanPham,
                    ProductCode = hd.SanPhamCT.IDSanPhamCT.ToString(),
                    Category = hd.SanPhamCT.SanPham.DanhMuc.TenDanhMuc,
                    Color = hd.SanPhamCT.MauSac != null ? hd.SanPhamCT.MauSac.TenMau : "Không xác định",
                    Size = hd.SanPhamCT.SizeAo != null ? hd.SanPhamCT.SizeAo.SoSize : "Không xác định"
                })
                .Select(g => new TopSellingAoDaiDto
                {
                    ProductId = (int)g.Key.IDSanPhamCT.GetHashCode(),
                    ProductName = g.Key.TenSanPham,
                    ProductCode = g.Key.ProductCode,
                    Category = g.Key.Category,
                    Color = g.Key.Color,
                    Size = g.Key.Size,
                    QuantitySold = g.Sum(x => x.SoLuongSanPham),
                    Revenue = g.Sum(x => x.GiaSauGiamGia * x.SoLuongSanPham),
                    Price = g.Average(x => x.GiaSauGiamGia)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            // Thêm rank
            for (int i = 0; i < topProducts.Count; i++)
            {
                topProducts[i].Rank = i + 1;
            }

            return topProducts;
        }

        private async Task<List<CustomerListDto>> GetTopCustomersAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var query = await _context.Users
                .Where(u => u.HoaDonsAsKhachHang.Any())
                .Select(u => new
                {
                    CustomerId = (int)u.Id.GetHashCode(),
                    CustomerName = u.HoTen ?? "N/A",
                    Email = u.Email ?? "N/A",
                    Phone = u.PhoneNumber ?? "N/A",
                    Orders = u.HoaDonsAsKhachHang
                        .Where(h => h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                        .ToList(),
                    OrderCount = u.HoaDonsAsKhachHang.Count(),
                    LastOrderDate = u.HoaDonsAsKhachHang
                        .Where(h => h.NgayTao.HasValue)
                        .OrderByDescending(h => h.NgayTao)
                        .Select(h => h.NgayTao.Value)
                        .FirstOrDefault(),
                    Status = u.TrangThai ? "Active" : "Inactive"
                })
                .ToListAsync();  // <- lấy từ DB về bộ nhớ

            var topCustomers = query
                .AsEnumerable() // chuyển sang LINQ to Objects
                .Select(u => new CustomerListDto
                {
                    CustomerId = u.CustomerId,
                    CustomerName = u.CustomerName,
                    Email = u.Email,
                    Phone = u.Phone,
                    CustomerType = DetermineCustomerType(u.Orders), // chạy trong bộ nhớ
                    TotalSpent = u.Orders.Sum(h => h.TongTienSauGiam),
                    OrderCount = u.OrderCount,
                    LastOrderDate = u.LastOrderDate,
                    Status = u.Status
                })
                .Where(c => c.OrderCount > 0)
                .OrderByDescending(c => c.TotalSpent)
                .Take(20)
                .ToList();

            return topCustomers;
        }
        public async Task<List<CustomerOrderDto>> GetCustomerOrdersByEmailAsync(string email)
        {
            return await _context.HoaDons
                .Where(h => h.User.Email == email) // Join sang AspNetUsers
                .Select(h => new CustomerOrderDto
                {
                    OrderId = h.IDHoaDon,
                    OrderDate = h.NgayTao ?? DateTime.MinValue,
                    Status = h.TrangThaiDonHang.ToString(),
                    PaymentStatus = h.TrangThaiThanhToan.ToString(),
                    TotalAmount = h.TongTienSauGiam,
                    ItemCount = h.HoaDonChiTiets.Sum(c => c.SoLuongSanPham)
                })
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        private string DetermineCustomerType(List<HoaDon> orders)
        {
            if (!orders.Any()) return "Mới";

            var totalSpent = orders
                .Where(h => h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .Sum(h => h.TongTienSauGiam);

            if (totalSpent >= 50000000) return "VIP";
            if (totalSpent >= 20000000) return "Khách hàng thân thiết";
            if (orders.Count >= 5) return "Khách hàng thường xuyên";

            var productNames = orders.SelectMany(h => h.HoaDonChiTiets)
                .Select(hd => hd.TenSanPham?.ToLower())
                .Where(name => !string.IsNullOrEmpty(name));

            if (productNames.Any(p => p.Contains("cưới"))) return "Khách cưới";
            if (productNames.Any(p => p.Contains("học sinh"))) return "Học sinh";

            return "Thường";
        }


        public async Task<RevenueReportDto> GetRevenueReportAsync(TimeRangeRequestDto request)
        {
            // Validate date range
            if (request.StartDate > request.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
            }

            if (request.EndDate > DateTime.Now)
            {
                request.EndDate = DateTime.Now;
            }

            var revenueByTime = await GetRevenueByTimeAsync(request);
            var revenueByCategory = await GetRevenueByCategoryAsync(request);
            var revenueByRegion = await GetRevenueByRegionAsync(request);

            // Calculate percentages for categories and regions
            var totalCategoryRevenue = revenueByCategory.Sum(c => c.Revenue);
            foreach (var category in revenueByCategory)
            {
                category.Percentage = totalCategoryRevenue > 0 ? (category.Revenue / totalCategoryRevenue) * 100 : 0;
            }

            var totalRegionRevenue = revenueByRegion.Sum(r => r.Revenue);
            foreach (var region in revenueByRegion)
            {
                region.Percentage = totalRegionRevenue > 0 ? (region.Revenue / totalRegionRevenue) * 100 : 0;
            }

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
                           hd.HoaDon.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
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

        public async Task<EmployeeReportDto> GetEmployeeReportAsync(TimeRangeRequestDto request)
        {
            // Lấy role Employee
            var employeeRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Employees");

            if (employeeRole == null)
            {
                return new EmployeeReportDto(); // Trả về empty report nếu không có role Employee
            }

            // Lấy danh sách nhân viên (users có role Employee)
            var employeeUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == employeeRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Lấy thông tin nhân viên và hóa đơn họ xử lý
            var employees = await _context.Users
                .Where(u => employeeUserIds.Contains(u.Id))
                .Include(u => u.HoaDonsAsNguoiTao)
                .ToListAsync();

            // Tính toán thống kê
            var topPerformers = employees
                .Select(u => new EmployeePerformanceDto
                {
                    EmployeeId = u.Id.GetHashCode(),
                    EmployeeName = u.HoTen,
                    OrdersProcessed = u.HoaDonsAsNguoiTao?
                        .Count(h => h.NgayTao >= request.StartDate &&
                                  h.NgayTao <= request.EndDate) ?? 0,
                    RevenueGenerated = u.HoaDonsAsNguoiTao?
                        .Where(h => h.NgayTao >= request.StartDate &&
                                   h.NgayTao <= request.EndDate &&
                                   h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                        .Sum(h => h.TongTienSauGiam) ?? 0,
                    AverageOrderValue = u.HoaDonsAsNguoiTao?
                        .Where(h => h.NgayTao >= request.StartDate &&
                                   h.NgayTao <= request.EndDate &&
                                   h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                        .Average(h => h.TongTienSauGiam) ?? 0,
                    PerformanceRating = CalculatePerformance(u),
                    HireDate = GetEmployeeHireDate(u) // Sử dụng ngày hóa đơn đầu tiên làm ngày vào làm
                })
                .OrderByDescending(e => e.RevenueGenerated)
                .Take(10)
                .ToList();

            // Thống kê hoạt động (không dùng NgayTao)
            var employeeActivities = new List<EmployeeActivityDto>();

            // Tổng hợp
            var totalEmployees = employees.Count;
            var activeEmployees = employees.Count(u => u.TrangThai);

            return new EmployeeReportDto
            {
                TopPerformers = topPerformers,
                EmployeeActivities = employeeActivities,
                Summary = new EmployeeSummaryDto
                {
                    TotalEmployees = totalEmployees,
                    ActiveEmployees = activeEmployees,
                    OnLeaveEmployees = 0,
                    TurnoverRate = 0,
                    AverageTenure = CalculateAverageTenure(employees),
                    EmployeesByDepartment = new Dictionary<string, int> { { "Tổng", totalEmployees } }
                }
            };
        }

        // Hàm xác định ngày vào làm dựa trên hóa đơn đầu tiên
        private DateTime GetEmployeeHireDate(ApplicationUser user)
        {
            return user.HoaDonsAsNguoiTao?
                .OrderBy(h => h.NgayTao)
                .Select(h => h.NgayTao)
                .FirstOrDefault()?.Date ?? DateTime.Now.Date;
        }

        // Hàm tính điểm hiệu suất
        private double CalculatePerformance(ApplicationUser user)
        {
            var orderCount = user.HoaDonsAsNguoiTao?
                .Count(h => h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN) ?? 0;
            var revenue = user.HoaDonsAsNguoiTao?
                .Where(h => h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                .Sum(h => h.TongTienSauGiam) ?? 0;

            // Điểm từ 1-5
            var score = 1.0 + (orderCount * 0.1) + (double)(revenue / 10000000);
            return Math.Min(Math.Round(score, 1), 5.0);
        }

        // Hàm tính thâm niên trung bình
        private decimal CalculateAverageTenure(List<ApplicationUser> employees)
        {
            if (!employees.Any()) return 0;

            var totalDays = employees.Sum(u =>
            {
                var hireDate = GetEmployeeHireDate(u);
                return (DateTime.Now - hireDate).TotalDays;
            });

            return (decimal)(totalDays / employees.Count / 365); // Trả về số năm
        }

        public async Task<CustomerReportDto> GetCustomerReportAsync(TimeRangeRequestDto request)
        {
            // Lấy danh sách khách hàng với thông tin cơ bản
            var customerSegments = await _context.Users
                .Where(u => u.HoTen != null && u.Email != null) // Chỉ lấy user có thông tin đầy đủ
                .Select(u => new CustomerValueDto
                {
                    CustomerId = (int)u.Id.GetHashCode(),
                    CustomerName = u.HoTen ?? "N/A",
                    Email = u.Email ?? "N/A",
                    Tier = "Standard", // You might have a tier system
                    LifetimeValue = u.HoaDonsAsKhachHang.Where(h => h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                                             .Sum(h => h.TongTienSauGiam),
                    OrderCount = u.HoaDonsAsKhachHang.Count(),
                    LastOrderDate = u.HoaDonsAsKhachHang.Where(h => h.NgayTao.HasValue)
                                            .OrderByDescending(h => h.NgayTao)
                                            .Select(h => h.NgayTao.Value)
                                            .FirstOrDefault()
                })
                .OrderByDescending(c => c.LifetimeValue)
                .Take(50)
                .ToListAsync();

            // Lấy hoạt động khách hàng theo ngày
            var customerActivities = await _context.HoaDons
                .Where(h => h.NgayTao >= request.StartDate && h.NgayTao <= request.EndDate && h.NgayTao.HasValue)
                .GroupBy(h => h.NgayTao.Value.Date)
                .Select(g => new CustomerActivityDto
                {
                    Date = g.Key,
                    NewCustomers = g.Select(h => h.IDUser).Distinct().Count(),
                    RepeatCustomers = g.Count() - g.Select(h => h.IDUser).Distinct().Count(), // Khách quay lại = tổng hóa đơn - khách mới
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
                           h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
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
                           h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN);

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
                           hd.HoaDon.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
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
                           h.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
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