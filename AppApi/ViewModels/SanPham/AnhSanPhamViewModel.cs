namespace AppApi.ViewModels.SanPham
{
    public class AnhSanPhamViewModel
    {
        public Guid IdAnh { get; set; }
        public Guid IDSanPham { get; set; }
        public string DuongDanAnh { get; set; } = string.Empty;
        public bool? AnhChinh { get; set; }
    }
}
