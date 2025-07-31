namespace AppApi.ViewModels.HoaDonDTOs
{
    public class HoaDonDTO
    {
        public Guid IDHoaDon { get; set; }
        public Guid IDUser { get; set; }
        public Guid IDDiaChiNhanHang { get; set; }
        public Guid IDHinhThucTT { get; set; }

        public decimal TongTienTruocGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public decimal TienGiam { get; set; }

        public float? PhanTramGiamGiaHoaDon { get; set; }
        public decimal? TienGiamHoaDon { get; set; }

        public DateTime? NgayThanhToan { get; set; }
        public string? GhiChu { get; set; }

        public string TrangThaiDonHang { get; set; } = string.Empty;
        public string TrangThaiThanhToan { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; }
        public DateTime NgaySua { get; set; }

        // Nếu có mã giảm giá (mở rộng)
        public GiamGiaDTO? MaGiamGia { get; set; }

        public List<HoaDonChiTietDTO> HoaDonChiTiets { get; set; } = new();
    }

}
