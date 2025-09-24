using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;
using Newtonsoft.Json;
using System.Text;

namespace AppView.Areas.Admin.Repository
{
    public class VoucherRepo : IVoucherRepo
    {
        private readonly HttpClient _client;

        public VoucherRepo(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7221/api/"); // link API backend
        }

        public async Task<IEnumerable<VoucherVM>> GetAllAsync()
        {
            var response = await _client.GetAsync("voucher");
            if (!response.IsSuccessStatusCode) return new List<VoucherVM>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<VoucherVM>>(json) ?? new List<VoucherVM>();
        }

        public async Task<VoucherVM?> GetByIdAsync(Guid id)
        {
            var response = await _client.GetAsync($"voucher/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VoucherVM>(json);
        }

        public async Task<(bool IsSuccess, string Message)> CreateAsync(VoucherVM voucher)
        {
            var json = JsonConvert.SerializeObject(voucher);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("voucher", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(responseContent)?["message"]?.ToString() ?? "Tạo voucher thất bại";
                return (false, msg);
            }

            return (true, "Tạo voucher thành công");
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(Guid id, VoucherVM voucher)
        {
            var json = JsonConvert.SerializeObject(voucher);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"voucher/{id}", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var msg = JsonConvert.DeserializeObject<dynamic>(responseContent)?["message"]?.ToString() ?? "Cập nhật voucher thất bại";
                return (false, msg);
            }

            return (true, "Cập nhật voucher thành công");
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id)
        {
            var response = await _client.DeleteAsync($"voucher/{id}");
            if (!response.IsSuccessStatusCode)
                return (false, "Xóa voucher thất bại");

            return (true, "Xóa voucher thành công");
        }
    }
}
