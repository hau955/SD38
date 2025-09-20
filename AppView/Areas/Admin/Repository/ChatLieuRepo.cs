using AppData.Models;
using AppView.Areas.Admin.IRepo;
using System.Text.Json;

namespace AppView.Areas.Admin.Repository
{
    public class ChatLieuRepo: IChatLieuRepo
    {
        private readonly HttpClient _httpClient;
        public ChatLieuRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatLieu>> GetAll()
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<List<ChatLieu>>>("https://localhost:7221/api/ChatLieux");
            return res?.Data ?? new List<ChatLieu>();
        }

        public async Task<ChatLieu?> GetByID(Guid id)
        {
            var res = await _httpClient.GetFromJsonAsync<ApiResponse<ChatLieu>>($"https://localhost:7221/api/ChatLieux/{id}");
            return res?.Data;
        }

        public async Task<ChatLieu> Create(ChatLieu ChatLieu)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/ChatLieux", ChatLieu);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ChatLieu>>();
            Console.WriteLine("Request gửi đi: " + JsonSerializer.Serialize(ChatLieu));
            Console.WriteLine("Response trả về: " + result);

            return result!.Data;
        }

        public async Task<ChatLieu?> Update(Guid id, ChatLieu ChatLieu)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7221/api/ChatLieux/{id}", ChatLieu);
            return response.IsSuccessStatusCode
                ? (await response.Content.ReadFromJsonAsync<ApiResponse<ChatLieu?>>())?.Data
                : null;
        }

        public async Task<bool> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7221/api/ChatLieux/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Toggle(Guid id)
        {
            // gọi API Toggle
            var response = await _httpClient.PutAsync($"https://localhost:7221/api/ChatLieux/Toggle/{id}", null);

            // đọc dữ liệu trả về
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            if (result != null && response.IsSuccessStatusCode)
            {
                return result.Message ?? "Đổi trạng thái thành công";
            }

            return result?.Message ?? "Có lỗi xảy ra khi đổi trạng thái";
        }


        private class ApiResponse<T>
        {
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; } = default!;
        }
    }
}
