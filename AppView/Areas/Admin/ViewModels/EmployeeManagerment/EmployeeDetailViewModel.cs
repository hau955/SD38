namespace AppView.Areas.Admin.ViewModels.EmployeeManagerment
{
    public class EmployeeDetailViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? AvatarUrl { get; set; }

        public bool? Gender { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
