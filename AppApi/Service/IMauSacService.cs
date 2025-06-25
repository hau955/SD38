using WebModels.Models;

namespace AppApi.Service
{
    public interface IMauSacService
    {
        Task<IEnumerable<MauSac>> GetAllMauSacsAsync();
        Task<MauSac> GetMauSacByIdAsync(Guid id);
        Task<MauSac> CreateMauSacAsync(MauSac mauSac);
        Task<bool> UpdateMauSacAsync(MauSac mauSac);
        Task<bool> DeleteMauSacAsync(Guid id);
        Task<bool> MauSacExistsAsync(Guid id);
    }
}
