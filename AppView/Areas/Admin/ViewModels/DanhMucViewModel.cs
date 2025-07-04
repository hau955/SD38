namespace AppView.Areas.Admin.ViewModels
{
    public class DanhMucViewModel
    {
        public Guid DanhMucId { get; set; }
        public string? TenDanhMuc { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
