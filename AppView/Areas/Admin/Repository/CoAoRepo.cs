using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class CoAoRepo : ICoAoRepo
    {
        private readonly HttpClient _httpClient;
        public CoAoRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CoAo>> GetAll()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<CoAo>>>("api/CoAo");
            return res?.Data ?? new List<CoAo>();
        }

        public async Task<CoAo?> GetByID(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<CoAo>>($"api/CoAo/{id}");
            return res?.Data;
        }

        public async Task<CoAo> Create(CoAo coao)
        {
            var response = await _httpClient.PostAsJsonAsync("api/CoAo", coao);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<CoAo>>();
            return result!.Data;
        }

        public async Task<CoAo?> Update(Guid id, CoAo coao)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/CoAo/{id}", coao);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<ApiResponse<CoAo?>>())?.Data
                : null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/CoAo/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"api/CoAo/ToggleStatus/{id}", null);
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
