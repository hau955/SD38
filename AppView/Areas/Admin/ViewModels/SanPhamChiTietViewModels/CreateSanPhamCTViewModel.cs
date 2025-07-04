using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AppView.Areas.Admin.ViewModels.SanPhamChiTietViewModels
{
    public class CreateSanPhamCTViewModel
    {
        public Guid IDSanPham { get; set; }

        [Required(ErrorMessage = "Chọn ít nhất 1 màu sắc")]
        public List<Guid> SelectedMauSacs { get; set; } = new();

        [Required(ErrorMessage = "Chọn ít nhất 1 size")]
        public List<Guid> SelectedSizes { get; set; } = new();

        [Required(ErrorMessage = "Chọn ít nhất 1 cổ áo")]
        public List<Guid> SelectedCoAos { get; set; } = new();

        [Required(ErrorMessage = "Chọn ít nhất 1 tà áo")]
        public List<Guid> SelectedTaAos { get; set; } = new();

        [Range(1, 100000000, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal GiaBan { get; set; }

        [Range(1, 100000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuongTonKho { get; set; }

        public List<SelectListItem> MauSacList { get; set; } = new();
        public List<SelectListItem> SizeList { get; set; } = new();
        public List<SelectListItem> CoAoList { get; set; } = new();
        public List<SelectListItem> TaAoList { get; set; } = new();

    }
}
