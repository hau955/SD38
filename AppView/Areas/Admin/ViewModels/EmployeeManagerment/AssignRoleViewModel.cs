using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Admin.ViewModels.EmployeeManagerment
{
    public class AssignRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; }

        // Optional: list vai trò để hiển thị ra dropdown trong Razor
        public List<string>? AvailableRoles { get; set; }
    }
}
