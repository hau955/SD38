using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppView.Areas.Admin.ViewModels.SanPhamChiTietViewModels
{
    public class CreateSanPhamCTViewModel
    {
        public Guid IDSanPham { get; set; }

        public List<Guid> SelectedMauSacs { get; set; } = new();
        public List<Guid> SelectedSizes { get; set; } = new();
        public List<Guid> SelectedCoAos { get; set; } = new();
        public List<Guid> SelectedTaAos { get; set; } = new();

        public decimal GiaBan { get; set; }
        public int SoLuongTonKho { get; set; }

        public List<SelectListItem> MauSacList { get; set; } = new();
        public List<SelectListItem> SizeList { get; set; } = new();
        public List<SelectListItem> CoAoList { get; set; } = new();
        public List<SelectListItem> TaAoList { get; set; } = new();

    }
}
