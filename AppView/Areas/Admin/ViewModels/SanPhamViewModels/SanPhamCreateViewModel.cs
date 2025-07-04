using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppView.Areas.Admin.ViewModels.SanPhamViewModels
{
    public class SanPhamCreateViewModel
    {
        public Guid? IDSanPham { get; set; }
        public Guid  DanhMucID { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
        public double TrongLuong { get; set; }
        public bool GioiTinh { get; set; } 
        public string? HinhAnh { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool TrangThai { get; set; } = true; // Mặc định là true (còn hàng)
        public DateTime NgayTao { get; set; } 
        public DateTime NgaySua { get; set; }
        public List<SelectListItem>? DanhMucList { get; set; }
    }
}
