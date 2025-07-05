using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AppView.Areas.Admin.Repository
{
    public class DanhMucRepo : IDanhMucRePo
    {
        private readonly HttpClient _httpClient;

        public DanhMucRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7221/api/"); // đổi lại nếu port khác
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<DanhMucViewModel>> GetAllDanhMucsAsync()
        {
            var response = await _httpClient.GetAsync("DanhMuc");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<DanhMucViewModel>>>(jsonData);
                return result?.Data ?? new List<DanhMucViewModel>();
            }
            return new List<DanhMucViewModel>();
        }

        private class ApiResponse<T>
        {
            public string Message { get; set; }
            public T Data { get; set; }
        }
        public async Task<DanhMucViewModel?> GetDanhMucByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"DanhMuc/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DanhMucViewModel>(json);
            }
            return null;
        }

        public async Task<bool> CreateDanhMucAsync(DanhMucViewModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("DanhMuc", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("❌ Lỗi khi thêm danh mục: " + error);
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateDanhMucAsync(DanhMucViewModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"DanhMuc/{model.DanhMucId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDanhMucAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"DanhMuc/{id}");
            return response.IsSuccessStatusCode;
        }

    }
}
