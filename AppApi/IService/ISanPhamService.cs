using AppApi.ViewModels.SanPham;
using AppView.Areas.Admin.ViewModels;
using AppData.Models;

namespace AppApi.IService
{
    public interface ISanPhamService
    {
        Task<List<SanPhamView>> GetAll();
        Task<SanPham?> GetByID(Guid id);

        Task<SanPham> Create(SanPhamCreateRequest sanpham);
        Task<SanPham> Update(SanPhamCreateRequest sanpham);

    }
}
