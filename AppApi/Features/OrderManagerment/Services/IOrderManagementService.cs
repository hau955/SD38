using AppApi.Features.Auth.DTOs;
using AppApi.Features.OrderManagerment.DTOs;
using AppApi.Helpers;

namespace AppApi.Features.OrderManagerment.Services
{
    public interface IOrderManagementService
    {
        Task<PagedResult<OrderListDto>> GetOrdersAsync(OrderFilterDto filter);
        Task<OrderDetailDto?> GetOrderDetailAsync(Guid orderId);
        Task<ApiResponse<bool>> ConfirmOrderAsync(Guid orderId, Guid userId);
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<ApiResponse<bool>> UpdatePaymentStatusAsync(UpdatePaymentStatusDto dto);
        Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderDto dto);
        Task<Dictionary<string, int>> GetOrderStatisticsAsync();
        Task<bool> CanUpdateOrderStatusAsync(Guid orderId, string newStatus);
    }
}
