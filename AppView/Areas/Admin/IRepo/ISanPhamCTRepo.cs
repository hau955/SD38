using AppData.Models;
using ViewModels;

namespace AppView.Areas.Admin.IRepo
{
    public interface ISanPhamCTRepo
    {
        Task<bool> CreateMultipleAsync(List<SanPhamCT> list);
        Task<List<MauSac>> GetMauSacsAsync();
        Task<List<Size>> GetSizesAsync();
        Task<List<ChatLieu>> GetChatLieuAsync();
       
        Task<List<SanPhamCT>> GetBySanPhamIdAsync(Guid idSanPham);
        Task<SanPhamCT?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(SanPhamCT model);
        Task<bool> ExistsAsync(Guid idSanPham, Guid idMauSac, Guid idSize, Guid idchatlieu);
        Task<List<SanPhamCTViewModel>> GetAllAsync();

    }
}
