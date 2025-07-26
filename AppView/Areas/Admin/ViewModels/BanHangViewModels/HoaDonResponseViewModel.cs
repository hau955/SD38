namespace AppView.Areas.Admin.ViewModels.BanHangViewModels
{
    public class HoaDonResponseViewModel
    {
        public Guid IDHoaDon { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public string? GhiChu { get; set; }
    }
}
