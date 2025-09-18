using AppData.Models;

namespace AppApi.IService
{
    public interface IGiamGiaService
    {
        Task<(decimal GiaGoc, decimal GiaSauGiam, decimal? SoTienGiam)?>TinhGiaSauGiamAsync(Guid idSanPhamCT, Guid danhMucId);
       // Task<(decimal GiaSauGiam, decimal GiaTriGiam, GiamGia? GiamGia)>TinhGiamGiaSanPham(Guid idSanPham, decimal giaGoc);
        Task<List<GiamGia>> GetAllAsync();
        Task<GiamGia?> GetByIdAsync(Guid id);
        Task<GiamGia> CreateAsync(GiamGia giamGia);
        Task<bool> UpdateAsync(GiamGia giamGia);
        Task<bool> DeleteAsync(Guid id);

        // Gắn sản phẩm vào giảm giá
        Task<GiamGiaSanPham?> AddProductToDiscountAsync(Guid giamGiaId, Guid sanPhamId);
        Task<bool> AddCategoryToDiscount(Guid idGiamGia, Guid idDanhMuc);
        Task<List<GiamGiaSanPham>> GetProductsByDiscountAsync(Guid giamGiaId);
        Task<List<GiamGiaSPCT>> GetSanPhamCTByDiscountAsync(Guid giamGiaId);
        Task<GiamGiaSPCT?> AddSanPhamCTToDiscountAsync(Guid giamGiaId, Guid sanPhamCTId);
    }
}
