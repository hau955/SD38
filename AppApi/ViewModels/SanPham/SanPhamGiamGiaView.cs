namespace AppApi.ViewModels.SanPham
{
    public class SanPhamGiamGiaView
    {
        
            public Guid IDSanPham { get; set; }
            public string TenSanPham { get; set; }
            public decimal GiaGoc { get; set; }
            public decimal GiaSauGiam { get; set; }
            public string? TenGiamGia { get; set; }
            public string? LoaiGiamGia { get; set; }
            public decimal? GiaTriGiam { get; set; }
        

    }
}
