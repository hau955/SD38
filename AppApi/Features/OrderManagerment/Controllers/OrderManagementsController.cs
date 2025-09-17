using AppApi.Constants;
using AppApi.Features.Auth.DTOs;
using AppApi.Features.OrderManagerment.DTOs;
using AppApi.Features.OrderManagerment.Services;
using AppApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class OrderManagementsController : ControllerBase
{
    private readonly IOrderManagementService _orderService;

    public OrderManagementsController(IOrderManagementService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrderFilterDto filter)
    {
        var result = await _orderService.GetOrdersAsync(filter);
        return Ok(ApiResponse<PagedResult<OrderListDto>>.Success(result));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetail(Guid id)
    {
        try
        {
            var order = await _orderService.GetOrderDetailAsync(id);

            if (order == null)
                return NotFound(ApiResponse<OrderDetailDto>.Fail("Không tìm thấy đơn hàng", 404));

            return Ok(ApiResponse<OrderDetailDto>.Success(order));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR in GetOrderDetail controller: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ApiResponse<OrderDetailDto>.Fail("Lỗi nội bộ", 500));
        }
    }
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid id, [FromQuery] Guid userId)
    {
        if (userId == Guid.Empty)
            return Unauthorized(ApiResponse<bool>.Fail("Không thể xác định người dùng", 401));

        var result = await _orderService.ConfirmOrderAsync(id, userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        if (id != dto.IDHoaDon)
            return BadRequest(ApiResponse<bool>.Fail("ID đơn hàng không khớp", 400));

        if (dto.IDNguoiCapNhat == Guid.Empty)
        {
            dto.IDNguoiCapNhat = GetCurrentUserId();
        }

        if (dto.IDNguoiCapNhat == Guid.Empty)
            return Unauthorized(ApiResponse<bool>.Fail("Không thể xác định người dùng", 401));

        var result = await _orderService.UpdateOrderStatusAsync(dto);
        return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{id}/payment")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusDto dto)
    {
        if (id != dto.IDHoaDon)
            return BadRequest(ApiResponse<bool>.Fail("ID đơn hàng không khớp", 400));

        // Chỉ ghi đè nếu chưa có IDNguoiCapNhat
        if (dto.IDNguoiCapNhat == Guid.Empty)
        {
            dto.IDNguoiCapNhat = GetCurrentUserId();
        }

        if (dto.IDNguoiCapNhat == Guid.Empty)
            return Unauthorized(ApiResponse<bool>.Fail("Không thể xác định người dùng", 401));

        var result = await _orderService.UpdatePaymentStatusAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDto dto)
    {
        if (id != dto.IDHoaDon)
            return BadRequest(ApiResponse<bool>.Fail("ID đơn hàng không khớp", 400));

        // Nếu client đã truyền IDNguoiHuy hợp lệ, không cần ghi đè
        if (dto.IDNguoiHuy == Guid.Empty)
        {
            dto.IDNguoiHuy = GetCurrentUserId();
        }

        if (dto.IDNguoiHuy == Guid.Empty)
            return Unauthorized(ApiResponse<bool>.Fail("Không thể xác định người dùng", 401));

        var result = await _orderService.CancelOrderAsync(dto);
        return StatusCode(result.StatusCode, result);
    }


    [HttpGet("statistics")]
    public async Task<IActionResult> GetOrderStatistics()
    {
        try
        {
            var stats = await _orderService.GetOrderStatisticsAsync();

            // Log để debug
            Console.WriteLine($"Statistics data: {string.Join(", ", stats.Select(x => $"{x.Key}: {x.Value}"))}");

            return Ok(ApiResponse<Dictionary<string, int>>.Success(stats));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR in GetOrderStatistics: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, ApiResponse<Dictionary<string, int>>.Fail("Lỗi khi lấy thống kê", 500));
        }
    }

    [HttpGet("{id}/can-update/{status}")]
    public async Task<IActionResult> CanUpdateStatus(Guid id, string status)
    {
        var canUpdate = await _orderService.CanUpdateOrderStatusAsync(id, status);
        return Ok(ApiResponse<bool>.Success(canUpdate));
    }

    [HttpGet("statuses")]
    public IActionResult GetOrderStatuses()
    {
        return Ok(ApiResponse<List<string>>.Success(OrderStatus.AllStatuses));
    }

    [HttpGet("payment-statuses")]
    public IActionResult GetPaymentStatuses()
    {
        return Ok(ApiResponse<List<string>>.Success(PaymentStatus.AllStatuses));
    }

    [HttpGet("{id}/next-statuses")]
    public async Task<IActionResult> GetNextStatuses(Guid id)
    {
        var order = await _orderService.GetOrderDetailAsync(id);
        if (order == null)
            return NotFound(ApiResponse<List<string>>.Fail("Không tìm thấy đơn hàng", 404));

        var nextStatuses = OrderStatus.AllowedTransitions.TryGetValue(order.TrangThaiDonHang, out var list)
            ? list
            : new List<string>();

        return Ok(ApiResponse<List<string>>.Success(nextStatuses));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
