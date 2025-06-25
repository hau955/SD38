using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Admin.ViewModels
{
    public class SanPhamCTViewModel
    {
        // Dữ liệu sản phẩm chính
        public Guid IDSanPham { get; set; }

        [Required]
        public string TenSanPham { get; set; }

        public string? MoTa { get; set; }

        public double? TrongLuong { get; set; }

        public bool? GioiTinh { get; set; }

        public bool TrangThai { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? HinhAnh { get; set; } 

        [Required]
        public Guid IdSize { get; set; }

        [Required]
        public Guid IdMauSac { get; set; }

        [Required]
        public Guid IdCoAo { get; set; }

        [Required]
        public Guid IdTaAo { get; set; }

        [Required]
        public int GiaBan { get; set; }

        [Required]
        public int SoLuongTonKho { get; set; }

        // Dữ liệu cho dropdown
        public List<SelectListItem>? SizeList { get; set; }
        public List<SelectListItem>? MauSacList { get; set; }
        public List<SelectListItem>? CoAoList { get; set; }
        public List<SelectListItem>? TaAoList { get; set; }
    }

}
