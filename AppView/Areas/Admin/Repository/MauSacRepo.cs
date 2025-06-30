using AppView.Areas.Admin.IRepo;
using System.Text.Json;
using WebModels.Models;

namespace AppView.Areas.Admin.Repository
{
    public class MauSacRepo : IMauSacRepo
    {
        private readonly HttpClient _httpClient;
        public MauSacRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MauSac>> GetAll()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<MauSac>>>("https://localhost:7221/api/MauSac");
            return res?.Data ?? new List<MauSac>();
        }

        public async Task<MauSac?> GetByID(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<MauSac>>($"https://localhost:7221/api/MauSac/{id}");
            return res?.Data;
        }

        public async Task<MauSac> Create(MauSac mauSac)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/MauSac", mauSac);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<MauSac>>();
            return result!.Data;
        }

        public async Task<MauSac?> Update(Guid id, MauSac mauSac)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7221/api/MauSac/{id}", mauSac);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<ApiResponse<MauSac?>>())?.Data
                : null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7221/api/MauSac/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            var response = await _httpClient.PatchAsync($"https://localhost:7221/api/MauSac/ToggleStatus/{id}", null);
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
