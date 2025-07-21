using AppApi.ViewModels.SanPham;
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
        public List<IFormFile>? ImageFiles { get; set; } // Sửa từ ImageFile sang ImageFiles

        public bool TrangThai { get; set; } = true; // Mặc định là true (còn hàng)
        public DateTime NgayTao { get; set; } 
        public DateTime NgaySua { get; set; }
        public List<SelectListItem>? DanhMucList { get; set; }
        public List<AnhSanPhamViewModel>? DanhSachAnh { get; set; } = new List<AnhSanPhamViewModel>();

    }
    public class AnhSanPhamViewModel
    {
        public Guid IdAnh { get; set; }
        public Guid IDSanPham { get; set; }
        public string DuongDanAnh { get; set; } = "";
        public bool AnhChinh { get; set; }

    }
}
