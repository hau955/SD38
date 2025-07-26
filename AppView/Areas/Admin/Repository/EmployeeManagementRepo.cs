using AppApi.ViewModels.EmployeeManagementDTOs;
using AppView.Areas.Admin.IRepo;
using AppView.Areas.Admin.ViewModels.EmployeeManagerment;
using AppView.Helper;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AppView.Areas.Admin.Repository
{
    public class EmployeeManagementRepo : IEmployeeManagementRepo
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7221/api/EmployeeManagements";
        public EmployeeManagementRepo(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<PagedResult<EmployeeListViewModel>> GetAllAsync(
      int page = 1, int pageSize = 10,
      string? fullName = null, string? email = null, string? phoneNumber = null,
      bool? isActive = null, bool? gender = null)
        {
            try
            {
                // Xây dựng URL với các tham số
                var url = $"{BaseUrl}?page={page}&pageSize={pageSize}";

                if (!string.IsNullOrWhiteSpace(fullName))
                    url += $"&fullName={Uri.EscapeDataString(fullName)}";
                if (!string.IsNullOrWhiteSpace(email))
                    url += $"&email={Uri.EscapeDataString(email)}";
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                    url += $"&phoneNumber={Uri.EscapeDataString(phoneNumber)}";
                if (isActive.HasValue)
                    url += $"&isActive={isActive}";
                if (gender.HasValue)
                    url += $"&gender={gender}";

                // Gọi API
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API returned status code: {response.StatusCode}");
                    return new PagedResult<EmployeeListViewModel>(
                        new List<EmployeeListViewModel>(),
                        0,
                        pageSize,
                        page,
                        0);
                }

                // Đọc và deserialize response
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<PagedResult<EmployeeListDto>>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Mapping dữ liệu
                var viewModels = apiResponse?.Items?.Select(dto => new EmployeeListViewModel
                {
                    Id = dto.Id, // Chuyển đổi string sang Guid
                    FullName = dto.FullName ?? string.Empty,
                    Email = dto.Email ?? string.Empty,
                    PhoneNumber = dto.PhoneNumber,
                    Gender = dto.Gender,
                    AvatarUrl = dto.AvatarUrl,
                    Role = dto.Role ?? "Chưa phân quyền",
                    IsActive = dto.IsActive,
                    Address = dto.Address,
                    DateOfBirth = dto.DateOfBirth
                }).ToList() ?? new List<EmployeeListViewModel>();

                return new PagedResult<EmployeeListViewModel>(
                    viewModels,
                    apiResponse?.TotalCount ?? 0,
                    apiResponse?.PageSize ?? pageSize,
                    apiResponse?.CurrentPage ?? page,
                    apiResponse?.TotalPages ?? 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAllAsync: {ex}");
                return new PagedResult<EmployeeListViewModel>(
                    new List<EmployeeListViewModel>(),
                    0,
                    pageSize,
                    page,
                    0);
            }
        }
        public async Task<EmployeeDetailViewModel?> GetByIdAsync(string id) // Đổi từ string sang Guid
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API returned status code: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var employee = JsonSerializer.Deserialize<EmployeeDetailDto>(
      content,
      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (employee == null)
                    return null;

                return new EmployeeDetailViewModel
                {
                    Id = employee.Id,
                    FullName = employee.FullName ?? string.Empty,
                    Email = employee.Email ?? string.Empty,
                    PhoneNumber = employee.PhoneNumber,
                    Gender = employee.Gender,
                    AvatarUrl = employee.AvatarUrl,
                    Role = employee.Role ?? "Chưa có",
                    IsActive = employee.IsActive,
                    Address = employee.Address,
                    DateOfBirth = employee.DateOfBirth
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetByIdAsync: {ex}");
                return null;
            }
        }
        public async Task<ApiResponse<object>> CreateAsync(AddEmployeeViewModel model)
        {
            try
            {
                // Validate model trước khi gửi
                if (model.Password != model.ConfirmPassword)
                {
                    return ApiResponse<object>.Fail("Mật khẩu xác nhận không khớp", 400);
                }

                var formData = new MultipartFormDataContent();

                // Thêm các trường bắt buộc
                formData.Add(new StringContent(model.FullName), "FullName");
                formData.Add(new StringContent(model.Email), "Email");
                formData.Add(new StringContent(model.Password), "Password");
                formData.Add(new StringContent(model.ConfirmPassword), "ConfirmPassword"); // Thêm confirm password

                // Thêm các trường không bắt buộc
                if (!string.IsNullOrEmpty(model.PhoneNumber))
                    formData.Add(new StringContent(model.PhoneNumber), "PhoneNumber");

                if (!string.IsNullOrEmpty(model.Address))
                    formData.Add(new StringContent(model.Address), "Address");

                // Xử lý ngày tháng
                if (model.DateOfBirth.HasValue)
                    formData.Add(new StringContent(model.DateOfBirth.Value.ToString("yyyy-MM-dd")), "DateOfBirth");

                // Xử lý giới tính
                if (model.Gender.HasValue)
                    formData.Add(new StringContent(model.Gender.Value.ToString()), "Gender");

                // Xử lý file ảnh đại diện
                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var fileContent = new StreamContent(model.AvatarFile.OpenReadStream());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.AvatarFile.ContentType);
                    formData.Add(fileContent, "AvatarFile", model.AvatarFile.FileName);
                }

                var response = await _httpClient.PostAsync($"{BaseUrl}", formData);

                // Kiểm tra status code
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ApiResponse<object>.Fail($"API trả về lỗi: {response.StatusCode} - {errorContent}", (int)response.StatusCode);
                }

                var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (content == null)
                {
                    return ApiResponse<object>.Fail("Không thể đọc phản hồi từ API", 500);
                }

                return content;
            }
            catch (HttpRequestException httpEx)
            {
                return ApiResponse<object>.Fail($"Lỗi kết nối API: {httpEx.Message}", 500);
            }
            catch (JsonException jsonEx)
            {
                return ApiResponse<object>.Fail($"Lỗi xử lý dữ liệu JSON: {jsonEx.Message}", 500);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi khi tạo nhân viên: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> UpdateAsync(string id, UpdateEmployeeViewModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", model);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (content != null)
                    return content;

                return ApiResponse<object>.Fail("Không nhận được phản hồi từ API", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi khi cập nhật nhân viên: {ex.Message}", 500);
            }
        }
        public async Task<ApiResponse<object>> ToggleStatusAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/toggle-status/{id}", null);

                if (response.Content?.Headers?.ContentLength == 0)
                    return ApiResponse<object>.Fail("Không nhận được phản hồi từ API", (int)response.StatusCode);

                var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                return content ?? ApiResponse<object>.Fail("Không nhận được phản hồi hợp lệ", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi khi chuyển trạng thái tài khoản: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> AssignRoleAsync(string userId, string role)
        {
            try
            {
                var model = new AssignRoleViewModel
                {
                    UserId = userId,
                    Role = role
                };

                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/assign-role", model);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (content != null)
                    return content;

                return ApiResponse<object>.Fail("Không nhận được phản hồi từ API", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi khi gán quyền: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordViewmodelEm model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/reset-password", model);
                var content = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (content != null)
                    return content;

                return ApiResponse<object>.Fail("Không nhận được phản hồi từ API", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail($"Lỗi khi đặt lại mật khẩu: {ex.Message}", 500);
            }
        }
    }
}