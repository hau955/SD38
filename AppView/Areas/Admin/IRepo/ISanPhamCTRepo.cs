using AppData.Models;

namespace AppView.Areas.Admin.IRepo
{
    public interface ISanPhamCTRepo
    {
        Task<bool> CreateMultipleAsync(List<SanPhamCT> list);
        Task<List<MauSac>> GetMauSacsAsync();
        Task<List<Size>> GetSizesAsync();
       
        Task<List<SanPhamCT>> GetBySanPhamIdAsync(Guid idSanPham);
        Task<SanPhamCT?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(SanPhamCT model);
        Task<bool> ExistsAsync(Guid idSanPham, Guid idMauSac, Guid idSize);

    }
}
