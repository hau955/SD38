using AppView.Areas.Admin;
using AppView.Areas.OrderManagerment.ViewModels;
using AppView.Helper;
using System.Net;
using System.Text.Json;

namespace AppView.Areas.OrderManagerment.Repositories
{
    public class OrderManagementRepo : IOrderManagementRepo
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/OrderManagements";
        public OrderManagementRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<PagedResult<OrderListViewModel>>> GetOrdersAsync(OrderFilterViewModel filter)
        {
            var query = QueryHelper.ToQueryString(filter);
            var response = await _httpClient.GetAsync($"api/OrderManagements{query}");

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Lỗi Server: {content}");
                throw new Exception($"Lỗi API: {response.StatusCode} - {content}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<OrderListViewModel>>>();
            return result!;
        }
        public async Task<ApiResponse<OrderDetailViewModel>> GetOrderDetailAsync(Guid id)
        {
            try
            {
                // Kiểm tra ID hợp lệ
                if (id == Guid.Empty)
                {
                    return ApiResponse<OrderDetailViewModel>.Fail("ID đơn hàng không hợp lệ", 400);
                }

                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");

                // Kiểm tra status code
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Lỗi khi lấy chi tiết đơn hàng: {response.StatusCode} - {errorContent}");

                    return response.StatusCode switch
                    {
                        HttpStatusCode.NotFound => ApiResponse<OrderDetailViewModel>.Fail("Không tìm thấy đơn hàng", 404),
                        _ => ApiResponse<OrderDetailViewModel>.Fail($"Lỗi khi lấy chi tiết đơn hàng: {response.StatusCode}", (int)response.StatusCode)
                    };
                }

                // Đọc và trả về dữ liệu
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetailViewModel>>();

                if (result == null)
                {
                    return ApiResponse<OrderDetailViewModel>.Fail("Dữ liệu trả về không hợp lệ", 500);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Lỗi kết nối API: {ex.Message}");
                return ApiResponse<OrderDetailViewModel>.Fail("Lỗi kết nối đến server", 503);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ Lỗi parse JSON: {ex.Message}");
                return ApiResponse<OrderDetailViewModel>.Fail("Lỗi xử lý dữ liệu", 500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi không xác định: {ex.Message}");
                return ApiResponse<OrderDetailViewModel>.Fail($"Lỗi hệ thống: {ex.Message}", 500);
            }
        }
        public async Task<ApiResponse<bool>> ConfirmOrderAsync(Guid id, Guid userId)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{id}/confirm?userId={userId}", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>() ?? ApiResponse<bool>.Fail("Lỗi không xác định");
        }


        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{model.IDHoaDon}/status", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>() ?? ApiResponse<bool>.Fail("Lỗi không xác định");
        }

        public async Task<ApiResponse<bool>> UpdatePaymentStatusAsync(UpdatePaymentStatusViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{model.IDHoaDon}/payment", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>() ?? ApiResponse<bool>.Fail("Lỗi không xác định");
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{model.IDHoaDon}/cancel", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>() ?? ApiResponse<bool>.Fail("Lỗi không xác định");
        }


        public async Task<ApiResponse<Dictionary<string, int>>> GetOrderStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/statistics");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error getting statistics: {response.StatusCode} - {errorContent}");
                    return ApiResponse<Dictionary<string, int>>.Fail("Không thể lấy thống kê đơn hàng", (int)response.StatusCode);
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, int>>>();
                return result ?? ApiResponse<Dictionary<string, int>>.Fail("Dữ liệu thống kê không hợp lệ", 500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in GetOrderStatisticsAsync: {ex.Message}");
                return ApiResponse<Dictionary<string, int>>.Fail($"Lỗi khi lấy thống kê: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> CanUpdateStatusAsync(Guid id, string status)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<bool>>($"{BaseUrl}/{id}/can-update/{status}");
            return response!;
        }

        public async Task<ApiResponse<List<string>>> GetOrderStatusesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<string>>>($"{BaseUrl}/statuses");
            return response!;
        }

        public async Task<ApiResponse<List<string>>> GetPaymentStatusesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<string>>>($"{BaseUrl}/payment-statuses");
            return response!;
        }

        public async Task<ApiResponse<List<string>>> GetNextStatusesAsync(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<string>>>($"{BaseUrl}/{id}/next-statuses");
            return response!;
        }
    }
}
