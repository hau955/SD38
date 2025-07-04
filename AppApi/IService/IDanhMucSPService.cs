using AppData.Models;
using WebModels.Models;

namespace AppApi.IService
{
    public interface IDanhMucSPService
    {
        Task<IEnumerable<DanhMuc>> GetAllDanhMucSPsAsync();
        Task<DanhMuc> GetDanhMucSPByIdAsync(Guid id);
        Task<DanhMuc> CreateDanhMucSPAsync(DanhMuc danhMuc);
        Task<bool> UpdateDanhMucSPAsync(DanhMuc danhMuc);
        Task<bool> DeleteDanhMucSPAsync(Guid id);
        Task<bool> DanhMucSPExistsAsync(Guid id);
    }
}
