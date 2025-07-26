namespace AppApi.ViewModels.BanHang
{
    public class HoaDonChiTietViewModel
    {
        public Guid IDHoaDon { get; set; }
        public string? TenNguoiTao { get; set; }
        public string? NguoiMuaHang { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public DateTime NgayTao { get; set; }
        public string? TrangThaiDonHang { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public decimal TongTienTruocGiam { get; set; }
        public decimal TienGiam { get; set; }
        public decimal TongTienSauGiam { get; set; }
        public string? GhiChu { get; set; }

        public List<ChiTietSanPhamViewModel> DanhSachSanPham { get; set; } = new();
    }

    public class ChiTietSanPhamViewModel
    {
        public Guid IDSanPhamCT { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

}
