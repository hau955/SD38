using AppData.Models;

namespace AppApi.IService
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);

        // Trả về kết quả kèm message để validate
        Task<(bool IsSuccess, string Message, Voucher? Data)> CreateAsync(Voucher voucher);

        Task<(bool IsSuccess, string Message, Voucher? Data)> UpdateAsync(Guid id, Voucher voucher);

        Task<bool> DeleteAsync(Guid id);
    }
}
