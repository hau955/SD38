using AppApi.Constants;
using AppApi.Features.Auth.DTOs;
using AppApi.Features.OrderManagerment.DTOs;
using AppApi.Helpers;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Features.OrderManagerment.Services
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly ApplicationDbContext _context;

        public OrderManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<OrderListDto>> GetOrdersAsync(OrderFilterDto filter)
        {
            try
            {
                var query = _context.HoaDons
    .Include(h => h.User)
    .Include(h => h.User2)
    .Include(h => h.DiaChiNhanHang)
    .Include(h => h.HoaDonChiTiets)
    .AsQueryable();

                // Áp dụng bộ lọc
                if (!string.IsNullOrEmpty(filter.TrangThaiDonHang))
                {
                    query = query.Where(h => h.TrangThaiDonHang == filter.TrangThaiDonHang);
                }

                if (!string.IsNullOrEmpty(filter.TrangThaiThanhToan))
                {
                    query = query.Where(h => h.TrangThaiThanhToan == filter.TrangThaiThanhToan);
                }

                if (filter.TuNgay.HasValue)
                {
                    query = query.Where(h => h.NgayTao >= filter.TuNgay.Value);
                }

                if (filter.DenNgay.HasValue)
                {
                    query = query.Where(h => h.NgayTao <= filter.DenNgay.Value.AddDays(1));
                }

                if (!string.IsNullOrEmpty(filter.TenKhachHang))
                {
                    query = query.Where(h => h.User != null &&
                        h.User.HoTen.Contains(filter.TenKhachHang));
                }

                if (!string.IsNullOrEmpty(filter.SoDienThoai))
                {
                    query = query.Where(h => h.User != null &&
                        h.User.PhoneNumber != null &&
                        h.User.PhoneNumber.Contains(filter.SoDienThoai));
                }

                // Đếm tổng số
                var totalCount = await query.CountAsync();

                // Sắp xếp
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Phân trang
                var orders = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(h => new OrderListDto
                    {
                        IDHoaDon = h.IDHoaDon,
                        TenKhachHang = h.User != null ? h.User.HoTen : "Khách vãng lai",
                        EmailKhachHang = h.User != null ? h.User.Email : "",
                        SoDienThoai = h.User != null ? h.User.PhoneNumber : "",
                        NgayTao = h.NgayTao,
                        TongTienSauGiam = h.TongTienSauGiam,
                        TrangThaiDonHang = h.TrangThaiDonHang,
                        TrangThaiThanhToan = h.TrangThaiThanhToan,
                        GhiChu = h.GhiChu,
                        SoLuongSanPham = h.HoaDonChiTiets.Sum(ct => ct.SoLuongSanPham),
                        DiaChiGiaoHang = h.DiaChiNhanHang != null ? h.DiaChiNhanHang.DiaChiChiTiet : "",
                        NgayThanhToan = h.NgayThanhToan,
                        TenNguoiTao = h.User2 != null ? h.User2.HoTen : ""
                    })
                    .ToListAsync();

                return new PagedResult<OrderListDto>
                {
                    Items = orders,
                    TotalCount = totalCount,
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in GetOrdersAsync: " + ex.Message);
                throw; // hoặc return null
            }
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(Guid orderId)
        {
            try
            {
                var order = await _context.HoaDons
              .Include(h => h.User)
              .Include(h => h.User2)
              .Include(h => h.DiaChiNhanHang)
              .Include(h => h.HoaDonChiTiets)
                  .ThenInclude(ct => ct.SanPhamCT)
                      .ThenInclude(sp => sp.SanPham)
              .Include(h => h.HoaDonChiTiets)
                  .ThenInclude(ct => ct.SanPhamCT)
                      .ThenInclude(sp => sp.MauSac)
              .Include(h => h.HoaDonChiTiets)
                  .ThenInclude(ct => ct.SanPhamCT)
                      .ThenInclude(sp => sp.SizeAo)
              .Include(h => h.HoaDonChiTiets)
                  .ThenInclude(ct => ct.SanPhamCT)
                      .ThenInclude(sp => sp.ChatLieu)
              .FirstOrDefaultAsync(h => h.IDHoaDon == orderId);

                if (order == null) return null;

                return new OrderDetailDto
                {
                    IDHoaDon = order.IDHoaDon,
                    TenKhachHang = order.User?.HoTen ?? "Khách vãng lai",
                    EmailKhachHang = order.User?.Email ?? "",
                    SoDienThoai = order.User?.PhoneNumber ?? "",
                    NgayTao = order.NgayTao,
                    TongTienTruocGiam = order.TongTienTruocGiam,
                    TongTienSauGiam = order.TongTienSauGiam,
                    TienGiam = order.TienGiam,
                    TrangThaiDonHang = order.TrangThaiDonHang,
                    TrangThaiThanhToan = order.TrangThaiThanhToan,
                    GhiChu = order.GhiChu,
                    NgayThanhToan = order.NgayThanhToan,
                    TenNguoiTao = order.User2?.HoTen ?? "",
                    DiaChiGiaoHang = order.DiaChiNhanHang?.DiaChiChiTiet ?? "",
                    TenNguoiNhan = order.DiaChiNhanHang?.HoTenNguoiNhan ?? "",
                    SoDienThoaiNhan = order.DiaChiNhanHang?.SoDienThoai ?? "",
                    ChiTietSanPhams = order.HoaDonChiTiets.Select(ct => new OrderItemDto
                    {
                        IDHoaDonChiTiet = ct.IDHoaDonChiTiet,
                        IDSanPhamCT = ct.IDSanPhamCT,
                        TenSanPham = ct.TenSanPham,
                        SoLuongSanPham = ct.SoLuongSanPham,
                        GiaSanPham = ct.GiaSanPham,
                        GiaSauGiamGia = ct.GiaSauGiamGia,
                        MauSac = ct.SanPhamCT?.MauSac?.TenMau ?? "",
                        Size = ct.SanPhamCT?.SizeAo?.SoSize ?? "",
                        ChatLieu = ct.SanPhamCT?.ChatLieu?.TenChatLieu ?? "",
                        HinhAnh = ct.SanPhamCT?.SanPham?.AnhSanPhams?.FirstOrDefault()?.DuongDanAnh ?? ""
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in GetOrderDetailAsync: {ex.Message}\n{ex.StackTrace}");
                throw; // vẫn để throw để giữ nguyên response code 500 cho dev biết
            }
        }
        public async Task<ApiResponse<bool>> ConfirmOrderAsync(Guid orderId, Guid userId)
        {
            try
            {
                var order = await _context.HoaDons.FindAsync(orderId);
                if (order == null)
                    return ApiResponse<bool>.Fail("Không tìm thấy đơn hàng", 404);

                if (order.TrangThaiDonHang != OrderStatus.CHO_XAC_NHAN)
                    return ApiResponse<bool>.Fail("Chỉ có thể xác nhận đơn hàng ở trạng thái 'Chờ xác nhận'", 400);

                order.TrangThaiDonHang = OrderStatus.DA_XAC_NHAN;
                order.IDNguoiTao = userId;
                order.NgaySua = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Xác nhận đơn hàng thành công", 200);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xác nhận đơn hàng",
                    StatusCode = 500,
                    Errors = new Dictionary<string, string[]>
            {
                { "Exception", new[] { ex.Message } }
            }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _context.HoaDons.FindAsync(dto.IDHoaDon);
                if (order == null)
                    return ApiResponse<bool>.Fail("Không tìm thấy đơn hàng", 404);

                if (!OrderStatus.AllowedTransitions.TryGetValue(order.TrangThaiDonHang, out var allowedNextStatuses)
                    || !allowedNextStatuses.Contains(dto.TrangThaiMoi))
                {
                    return ApiResponse<bool>.Fail(
                        $"Không thể chuyển từ trạng thái '{order.TrangThaiDonHang}' sang '{dto.TrangThaiMoi}'", 400);
                }

                order.TrangThaiDonHang = dto.TrangThaiMoi;
                order.IDNguoiTao = dto.IDNguoiCapNhat;
                order.NgaySua = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(dto.GhiChu))
                {
                    order.GhiChu = string.IsNullOrEmpty(order.GhiChu)
                        ? dto.GhiChu
                        : $"{order.GhiChu}\n[{DateTime.Now:dd/MM/yyyy HH:mm}] {dto.GhiChu}";
                }

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Cập nhật trạng thái đơn hàng thành công", 200);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng",
                    StatusCode = 500,
                    Errors = new Dictionary<string, string[]>
            {
                { "Exception", new[] { ex.Message } }
            }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdatePaymentStatusAsync(UpdatePaymentStatusDto dto)
        {
            try
            {
                var order = await _context.HoaDons.FindAsync(dto.IDHoaDon);
                if (order == null)
                    return ApiResponse<bool>.Fail("Không tìm thấy đơn hàng", 404);

                if (order.TrangThaiDonHang == OrderStatus.DA_HUY)
                    return ApiResponse<bool>.Fail("Không thể cập nhật thanh toán cho đơn hàng đã hủy", 400);

                order.TrangThaiThanhToan = dto.TrangThaiThanhToan;
                order.IDNguoiTao = dto.IDNguoiCapNhat;
                order.NgaySua = DateTime.UtcNow;

                if (dto.TrangThaiThanhToan == PaymentStatus.DA_THANH_TOAN)
                {
                    order.NgayThanhToan = dto.NgayThanhToan ?? DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Cập nhật trạng thái thanh toán thành công", 200);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái thanh toán",
                    StatusCode = 500,
                    Errors = new Dictionary<string, string[]>
            {
                { "Exception", new[] { ex.Message } }
            }
                };
            }
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderDto dto)
        {
            try
            {
                var order = await _context.HoaDons.FindAsync(dto.IDHoaDon);
                if (order == null)
                    return ApiResponse<bool>.Fail("Không tìm thấy đơn hàng", 404);

                if (order.TrangThaiDonHang == OrderStatus.HOAN_TAT)
                    return ApiResponse<bool>.Fail("Không thể hủy đơn hàng đã hoàn tất", 400);

                if (order.TrangThaiDonHang == OrderStatus.DA_HUY)
                    return ApiResponse<bool>.Fail("Đơn hàng đã được hủy trước đó", 400);

                order.TrangThaiDonHang = OrderStatus.DA_HUY;
                order.IDNguoiTao = dto.IDNguoiHuy;
                order.NgaySua = DateTime.UtcNow;

                var cancelNote = $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Đơn hàng đã bị hủy. Lý do: {dto.LyDoHuy}";
                order.GhiChu = string.IsNullOrEmpty(order.GhiChu)
                    ? cancelNote
                    : $"{order.GhiChu}\n{cancelNote}";

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Hủy đơn hàng thành công", 200);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi hủy đơn hàng",
                    StatusCode = 500,
                    Errors = new Dictionary<string, string[]>
            {
                { "Exception", new[] { ex.Message } }
            }
                };
            }
        }
        public async Task<Dictionary<string, int>> GetOrderStatisticsAsync()
        {
            try
            {
                var stats = await _context.HoaDons
                    .GroupBy(h => h.TrangThaiDonHang)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                // Log raw data
                Console.WriteLine("Raw statistics from database:");
                foreach (var stat in stats)
                {
                    Console.WriteLine($"{stat.Key}: {stat.Value}");
                }

                // Đảm bảo tất cả trạng thái đều có trong kết quả
                foreach (var status in OrderStatus.AllStatuses)
                {
                    if (!stats.ContainsKey(status))
                    {
                        stats[status] = 0;
                    }
                }

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR in GetOrderStatisticsAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        public async Task<bool> CanUpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _context.HoaDons.FindAsync(orderId);
            if (order == null) return false;

            return OrderStatus.AllowedTransitions.ContainsKey(order.TrangThaiDonHang) &&
                   OrderStatus.AllowedTransitions[order.TrangThaiDonHang].Contains(newStatus);
        }

        private IQueryable<HoaDon> ApplySorting(IQueryable<HoaDon> query, string sortBy, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortBy.ToLower() switch
            {
                "ngaytao" => isAscending ? query.OrderBy(h => h.NgayTao) : query.OrderByDescending(h => h.NgayTao),
                "tongtien" => isAscending ? query.OrderBy(h => h.TongTienSauGiam) : query.OrderByDescending(h => h.TongTienSauGiam),
                "trangthai" => isAscending ? query.OrderBy(h => h.TrangThaiDonHang) : query.OrderByDescending(h => h.TrangThaiDonHang),
                "khachhang" => isAscending ? query.OrderBy(h => h.User!.HoTen) : query.OrderByDescending(h => h.User!.HoTen),
                _ => query.OrderByDescending(h => h.NgayTao)
            };
        }
    }
}
