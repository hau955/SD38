namespace AppApi.ViewModels.HoaDonDTOs
{
    public class GiamGiaDTO
    {
        public Guid IDGiamGia { get; set; }
        public string TenMaGiamGia { get; set; } = string.Empty;
        public bool LoaiGiamGia { get; set; }
        public float? GiamTheoPhanTram { get; set; }
        public decimal? GiamTheoTien { get; set; }
        public string? DieuKienGiamGia { get; set; }
    }
}
