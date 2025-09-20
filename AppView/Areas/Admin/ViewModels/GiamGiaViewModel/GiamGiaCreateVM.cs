using AppData.Models;
using AppView.Areas.Admin.ViewModels.SanPhamViewModels;

using ViewModels;



namespace AppView.Areas.Admin.ViewModels
{
    public class GiamGiaCreateVM
    {
        public GiamGia GiamGia { get; set; } = new();

        // Các lựa chọn từ view (checkbox)
        public List<Guid> SelectedSanPhams { get; set; } = new();
        public List<Guid> SelectedDanhMucs { get; set; } = new();
       
        // Dữ liệu hiển thị (đổ ra dropdown/checkbox list)
        public List<SanPhamView> SanPhams { get; set; } = new();
        public List<DanhMucViewModel> DanhMucs { get; set; } = new();
       
    }
}
