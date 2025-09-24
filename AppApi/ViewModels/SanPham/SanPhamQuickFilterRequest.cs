namespace AppApi.ViewModels.SanPham
{
    public class SanPhamQuickFilterRequest
    {
        public List<Guid>? DanhMucId { get; set; }
        public List<Guid>? IDMauSac { get; set; }
        public List<Guid>? IDChatLieu { get; set; }
        public List<Guid>? IDSize { get; set; }
       

        public decimal? GiaMin { get; set; }
        public decimal? GiaMax { get; set; }
    }
}
