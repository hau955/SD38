namespace AppApi.ViewModels.HoaDonDTOs
{
    public class HoaDonChiTietDTO
    {
        public Guid ID { get; set; }

        public Guid IDSanPhamCT { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string MauSac { get; set; } = string.Empty;

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal TongTien => DonGia * SoLuong;
    }
}
