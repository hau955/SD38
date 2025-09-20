using AppApi.ViewModels.SanPham;
using AppData.Models;
using ViewModels;

namespace AppApi.IService
{
    public interface ISanPhamService
    {
        Task<SanPhamDetailWithDiscountView?> GetSanPhamDetailWithDiscountAsync(Guid idSanPham);
        Task<SanPhamGiamGiaView?> GetSanPhamChiTiet(Guid idSanPham);
        Task<List<SanPhamView>> GetAll();
        Task<SanPhamView?> GetSanPhamByIdAsync(Guid id);
        Task<List<SanPhamDetailWithDiscountView>> GetAllSanPhamsAsync();
        Task<SanPham> Create(SanPhamCreateRequest sanpham);
        Task<SanPham> Update(SanPhamCreateRequest sanpham);

    }
}
