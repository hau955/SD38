using AppData.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface IGiamGiaRepo
    {
        Task<List<GiamGia>> GetAllAsync();
        Task<GiamGia?> GetByIdAsync(Guid id);
        Task<GiamGia?> CreateAsync(GiamGia model);
        Task<bool> UpdateAsync(Guid id, GiamGia model);
        Task<bool> DeleteAsync(Guid id);

        // Sản phẩm
        Task<GiamGiaSanPham?> AddProductToDiscountAsync(Guid giamGiaId, Guid sanPhamId);
        Task<List<GiamGiaSanPham>> GetProductsByDiscountAsync(Guid giamGiaId);

        // Sản phẩm chi tiết
        Task<GiamGiaSPCT?> AddSanPhamCTToDiscountAsync(Guid giamGiaId, Guid sanPhamCTId);
        Task<List<GiamGiaSPCT>> GetSanPhamCTByDiscountAsync(Guid giamGiaId);

        // Danh mục
        Task<bool> AddCategoryToDiscountAsync(Guid giamGiaId, Guid danhMucId);

        // Tính giá sau giảm
        Task<TinhGiaResult?> TinhGiaSauGiamAsync(Guid idSanPhamCT, Guid danhMucId);
    }
    public class TinhGiaResult
    {
        public decimal GiaGoc { get; set; }
        public decimal GiaSauGiam { get; set; }
        public decimal? SoTienGiam { get; set; }
    }
}

