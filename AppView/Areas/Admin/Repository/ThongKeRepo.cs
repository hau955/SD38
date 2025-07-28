using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.ThongKeViewModel;
using Newtonsoft.Json;
using System.Text;

namespace AppView.Areas.Admin.Repository
{
    public class ThongKeRepo : IThongKeRepo
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public ThongKeRepo(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7221/api";
        }

        public async Task<DashboardOverviewViewModel> GetOverviewAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/thongke/overview");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<DashboardOverviewViewModel>>(content);
                    return apiResponse?.Data ?? new DashboardOverviewViewModel();
                }
                return new DashboardOverviewViewModel();
            }
            catch
            {
                return new DashboardOverviewViewModel();
            }
        }

        public async Task<RevenueReportViewModel> GetRevenueReportAsync(TimeRangeRequestViewModel request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseApiUrl}/thongke/revenue", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RevenueReportViewModel>>(responseContent);
                    return apiResponse?.Data ?? new RevenueReportViewModel();
                }
                return new RevenueReportViewModel();
            }
            catch
            {
                return new RevenueReportViewModel();
            }
        }

        public async Task<ProductReportViewModel> GetProductReportAsync(TimeRangeRequestViewModel request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseApiUrl}/thongke/products", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ProductReportViewModel>>(responseContent);
                    return apiResponse?.Data ?? new ProductReportViewModel();
                }
                return new ProductReportViewModel();
            }
            catch
            {
                return new ProductReportViewModel();
            }
        }

        public async Task<CustomerReportViewModel> GetCustomerReportAsync(TimeRangeRequestViewModel request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseApiUrl}/thongke/customers", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<CustomerReportViewModel>>(responseContent);
                    return apiResponse?.Data ?? new CustomerReportViewModel();
                }
                return new CustomerReportViewModel();
            }
            catch
            {
                return new CustomerReportViewModel();
            }
        }

        public async Task<PromotionReportViewModel> GetPromotionReportAsync(TimeRangeRequestViewModel request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseApiUrl}/thongke/promotions", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PromotionReportViewModel>>(responseContent);
                    return apiResponse?.Data ?? new PromotionReportViewModel();
                }
                return new PromotionReportViewModel();
            }
            catch
            {
                return new PromotionReportViewModel();
            }
        }

        public async Task<List<QuickMetricViewModel>> GetQuickMetricsAsync(TimeRangeRequestViewModel request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseApiUrl}/thongke/quick-metrics", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<QuickMetricViewModel>>>(responseContent);
                    return apiResponse?.Data ?? new List<QuickMetricViewModel>();
                }
                return new List<QuickMetricViewModel>();
            }
            catch
            {
                return new List<QuickMetricViewModel>();
            }
        }
    }
}
