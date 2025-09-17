using AppApi.ViewModels.SanPham;
using AppData.Models;
using ViewModels;

namespace AppApi.IService
{
    public interface ISanPhamService
    {
        Task<SanPhamGiamGiaView?> GetSanPhamChiTiet(Guid idSanPham);
        Task<List<SanPhamView>> GetAll();
        Task<SanPhamView?> GetByID(Guid id);
        Task<List<SanPhamView>> GetAllSanPhamsAsync();
        Task<SanPham> Create(SanPhamCreateRequest sanpham);
        Task<SanPham> Update(SanPhamCreateRequest sanpham);

    }
}
