namespace AppApi.ViewModels
{
    public class GiamGiaChiTietDto
    {
        public Guid IdGiamGia { get; set; }
        public string TenGiamGia { get; set; } = string.Empty;
        public string LoaiGiamGia { get; set; } = string.Empty;
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public List<string> SanPhamChiTiet { get; set; } = new();
        public List<string> SanPham { get; set; } = new();
        public List<string> DanhMuc { get; set; } = new();
    }
}
