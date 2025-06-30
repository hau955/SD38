using AppView.Areas.Admin.IRepo;
using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class TaAoRepo : ITaAoRepo
    {
        private readonly HttpClient _httpClient;
        public TaAoRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TaAo>> GetAll()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<TaAo>>>("https://localhost:7221/api/TaAo");
            return res?.Data ?? new List<TaAo>();
        }

        public async Task<TaAo?> GetByID(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<TaAo>>($"https://localhost:7221/api/TaAo/{id}");
            return res?.Data;
        }

        public async Task<TaAo> Create(TaAo taao)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/TaAo", taao);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TaAo>>();
            return result!.Data;
        }

        public async Task<TaAo?> Update(Guid id, TaAo taao)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7221/api/TaAo/{id}", taao);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<ApiResponse<TaAo?>>())?.Data
                : null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7221/api/TaAo/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"https://localhost:7221/api/TaAo/ToggleStatus/{id}", null);
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
