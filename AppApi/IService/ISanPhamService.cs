using AppApi.ViewModels.SanPham;
using AppView.Areas.Admin.ViewModels;
using WebModels.Models;

namespace AppApi.IService
{
    public interface ISanPhamService
    {
        Task<List<SanPhamView>> GetAll();
        Task<SanPham?> GetByID(Guid id);
        Task<List<SanPhamView>> GetAllSanPhamsAsync();
        Task<SanPham> Create(SanPhamCreateRequest sanpham);
        Task<SanPham> Update(SanPhamCreateRequest sanpham);

    }
}
