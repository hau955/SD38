using ViewModels;

namespace AppApi.ViewModels.SanPham
{
    public class SanPhamView
    {
        public Guid IDSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public double? TrongLuong { get; set; }
        public bool? GioiTinh { get; set; }
        public bool TrangThai { get; set; }
      
       // public string? HinhAnh { get; set; }
        public Guid? DanhMucID { get; set; }
        public string? TenDanhMuc { get; set; }
        public List<SanPhamCTViewModel> ChiTiets { get; set; } = new();
        public List<AnhSanPhamViewModel>? DanhSachAnh { get; set; }

    }
}
