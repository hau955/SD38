using AppApi.Features.Auth.DTOs;
using AppApi.Helpers;
using AppApi.ViewModels.EmployeeManagementDTOs;
namespace AppApi.IService
{
    public interface IEmployeeManagementService
    {
        Task<PagedResult<EmployeeListDto>> GetAllAsync(int page = 1,
    int pageSize = 10,
    string? fullName = null,
    string? email = null,
    string? phoneNumber = null,
    bool? isActive = null,
    bool? gender = null);
        Task<EmployeeDetailDto?> GetByIdAsync(string id);
        Task<ApiResponse<object>> CreateAsync(AddEmployeeDto dto);
        Task<ApiResponse<object>> UpdateAsync(string id, UpdateEmployeeDto dto);
        Task<ApiResponse<object>> ToggleActiveStatusAsync(string id);
        Task<ApiResponse<object>> AssignRoleAsync(AssignRoleDto dto);
        Task<ApiResponse<object>> ResetPasswordAsync(Features.Auth.DTOs.ResetPasswordDto dto);
    }
}
