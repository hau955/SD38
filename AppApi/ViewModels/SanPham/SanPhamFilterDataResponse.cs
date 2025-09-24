using AppData.Models;

namespace AppApi.ViewModels.SanPham
{
    public class SanPhamFilterDataResponse
    {
        public List<DanhMuc>? DanhMucs { get; set; }
        public List<MauSac>? MauSacs { get; set; }
        public List<ChatLieu>? ChatLieus { get; set; }
        public List<Size>? Sizes { get; set; }
    }
}
