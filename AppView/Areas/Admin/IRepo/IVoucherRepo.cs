using AppView.Areas.Admin.ViewModels;

namespace AppView.Areas.Admin.IRepo
{
    public interface IVoucherRepo
    {
        Task<IEnumerable<VoucherVM>> GetAllAsync();
        Task<VoucherVM?> GetByIdAsync(Guid id);
        Task<(bool IsSuccess, string Message)> CreateAsync(VoucherVM voucher);
        Task<(bool IsSuccess, string Message)> UpdateAsync(Guid id, VoucherVM voucher);
        Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id);
    }
}
