using AppApi.ViewModels.Profile;
using AppView.Areas.Admin.IRepo;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace AppView.Areas.Admin.Repository
{
    public class ProfileRepo : IProfileRepo
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileRepo(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ProfileViewModel?> GetProfileAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7221/api/Profile/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProfileViewModel>(json);
        }

        public async Task<bool> UpdateProfileAsync(Guid id, UpdateProfileViewModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"https://localhost:7221/api/Profile/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UploadAvatarAsync(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"https://localhost:7221/api/Profile/{id}/upload-avatar", content);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic? result = JsonConvert.DeserializeObject<dynamic>(responseJson);
            return result?.imageUrl;
        }
    }
}
