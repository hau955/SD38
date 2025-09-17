using AppView.Areas.OrderManagerment.ViewModels;
using AppView.Helper;
using AppView.Areas.Admin;
namespace AppView.Areas.OrderManagerment.Repositories
{
    public interface IOrderManagementRepo
    {
        Task<ApiResponse<PagedResult<OrderListViewModel>>> GetOrdersAsync(OrderFilterViewModel filter);
        Task<ApiResponse<OrderDetailViewModel>> GetOrderDetailAsync(Guid id);
        Task<ApiResponse<bool>> ConfirmOrderAsync(Guid id, Guid userId);
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusViewModel model);
        Task<ApiResponse<bool>> UpdatePaymentStatusAsync(UpdatePaymentStatusViewModel model);
        Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderViewModel model);
        Task<ApiResponse<Dictionary<string, int>>> GetOrderStatisticsAsync();
        Task<ApiResponse<bool>> CanUpdateStatusAsync(Guid id, string status);
        Task<ApiResponse<List<string>>> GetOrderStatusesAsync();
        Task<ApiResponse<List<string>>> GetPaymentStatusesAsync();
        Task<ApiResponse<List<string>>> GetNextStatusesAsync(Guid id);
    }
}
