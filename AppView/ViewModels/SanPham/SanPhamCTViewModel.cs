using AppApi.ViewModels.SanPham;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppView.ViewModels.SanPham
{
    public class SanPhamCTViewModel
    {
        // Dữ liệu sản phẩm chính
        public Guid IDSanPhamCT { get; set; }
        public Guid IDSanPham { get; set; }


        public string TenSanPham { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public double? TrongLuong { get; set; }

        public bool? GioiTinh { get; set; }

        public bool TrangThai { get; set; }

        public IFormFile? ImageFile { get; set; }

       // public string HinhAnh { get; set; }

        public Guid IdMauSac { get; set; }
        public string? MauSac { get; set; }

        public Guid IdSize { get; set; }
        public string? Size { get; set; }

        public Guid IDChatLieu { get; set; }
        public string? ChatLieu { get; set; }


        [Required]
        public decimal? GiaBan { get; set; }

        [Required]
        public int SoLuongTonKho { get; set; }

        // Dữ liệu cho dropdown
        public List<SelectListItem>? SizeList { get; set; }
        public List<SelectListItem>? MauSacList { get; set; }
        public List<SelectListItem>? ChatLieuList { get; set; }
       
    }

}
