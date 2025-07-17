namespace AppApi.ViewModels.Profile
{
    public class ProfileViewModel
    {
        
            public Guid Id { get; set; }
            public string HoTen { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? PhoneNumber { get; set; }
            public bool? GioiTinh { get; set; }
            public DateTime? NgaySinh { get; set; }
            public string? DiaChi { get; set; }
            public string? HinhAnh { get; set; }
       

    }
}
