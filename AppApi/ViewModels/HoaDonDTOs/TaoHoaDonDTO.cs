namespace AppApi.ViewModels.HoaDonDTOs
{
    public class TaoHoaDonDTO
    {
        public Guid IDUser { get; set; }

        public Guid IDDiaChiNhanHang { get; set; }
        public Guid IDHinhThucTT { get; set; }

        public string? GhiChu { get; set; }

        public List<TaoHoaDonChiTietDTO> ChiTietHoaDon { get; set; } = new();
    }
}
