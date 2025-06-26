using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class SizeRepo : ISizeRepo
    {
        private readonly HttpClient _httpClient;
        public SizeRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Size>> GetAll()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<Size>>>("api/Size");
            return res?.Data ?? new List<Size>();
        }

        public async Task<Size?> GetByID(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<Size>>($"api/Size/{id}");
            return res?.Data;
        }

        public async Task<Size> Create(Size size)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Size", size);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<Size>>();
            return result!.Data;
        }

        public async Task<Size?> Update(Guid id, Size size)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Size/{id}", size);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<ApiResponse<Size?>>())?.Data
                : null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Size/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"api/Size/ToggleStatus/{id}", null);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            return json != null && json.ContainsKey("message") ? json["message"] : "Thành công";
        }

        private class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
