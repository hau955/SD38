using AppView.Areas.Admin.ViewModels.EmployeeManagerment;
using AppView.Areas.Auth.ViewModel;
using AppView.Helper;

namespace AppView.Areas.Admin.IRepo
{
    public interface IEmployeeManagementRepo
    {
        Task<PagedResult<EmployeeListViewModel>> GetAllAsync(int page = 1, int pageSize = 10,
              string? fullName = null,
              string? email = null,
              string? phoneNumber = null,
              bool? isActive = null,
              bool? gender = null);

        Task<EmployeeDetailViewModel?> GetByIdAsync(string id);
        Task<ApiResponse<object>> CreateAsync(AddEmployeeViewModel model);
        Task<ApiResponse<object>> UpdateAsync(string id, UpdateEmployeeViewModel model);
        Task<ApiResponse<object>> ToggleStatusAsync(string id);
        Task<ApiResponse<object>> AssignRoleAsync(string userId, string role);
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordViewmodelEm model);

    }
}
