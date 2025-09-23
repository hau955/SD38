using AppApi.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppApi.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly ApplicationDbContext _context;

        public VoucherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers.ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.Vouchers.FindAsync(id);
        }

        public async Task<(bool IsSuccess, string Message, Voucher? Data)> CreateAsync(Voucher voucher)
        {
            // ✅ Validate CodeVoucher không trùng
            if (await _context.Vouchers.AnyAsync(x => x.CodeVoucher == voucher.CodeVoucher))
                return (false, "Mã voucher đã tồn tại", null);

            // ✅ Validate ngày
            if (voucher.StartDate >= voucher.EndDate)
                return (false, "Ngày bắt đầu phải nhỏ hơn ngày kết thúc", null);

            if (voucher.EndDate < DateTime.Now)
                return (false, "Ngày kết thúc không được nhỏ hơn ngày hiện tại", null);

            // ✅ Validate giảm giá
            if (voucher.PhanTram.HasValue && voucher.PhanTram > 100)
                return (false, "Phần trăm giảm không được lớn hơn 100%", null);

            if (voucher.SoTienGiam.HasValue && voucher.SoTienGiam < 0)
                return (false, "Số tiền giảm không hợp lệ", null);

            // ✅ Validate số lượng và điều kiện
            if (voucher.SoLuong < 0) return (false, "Số lượng không hợp lệ", null);
            if (voucher.DieuKienToiThieu < 0) return (false, "Điều kiện tối thiểu không hợp lệ", null);

            // ✅ Gán mặc định nếu chưa set số lần sử dụng tối đa
            if (voucher.SoLanSuDungToiDa <= 0)
                voucher.SoLanSuDungToiDa = 1;

            // ✅ Tạo mới
            voucher.IdVoucher = Guid.NewGuid();
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return (true, "Tạo voucher thành công", voucher);
        }

        public async Task<(bool IsSuccess, string Message, Voucher? Data)> UpdateAsync(Guid id, Voucher voucher)
        {
            var existing = await _context.Vouchers.FindAsync(id);
            if (existing == null)
                return (false, "Không tìm thấy voucher", null);

            // ✅ Validate trùng CodeVoucher (ngoại trừ chính nó)
            if (await _context.Vouchers.AnyAsync(x => x.CodeVoucher == voucher.CodeVoucher && x.IdVoucher != id))
                return (false, "Mã voucher đã tồn tại", null);

            // ✅ Validate ngày
            if (voucher.StartDate >= voucher.EndDate)
                return (false, "Ngày bắt đầu phải nhỏ hơn ngày kết thúc", null);

            if (voucher.EndDate < DateTime.Now)
                return (false, "Ngày kết thúc không được nhỏ hơn ngày hiện tại", null);

            // ✅ Validate giảm giá
            if (voucher.PhanTram.HasValue && voucher.PhanTram > 100)
                return (false, "Phần trăm giảm không được lớn hơn 100%", null);

            if (voucher.SoTienGiam.HasValue && voucher.SoTienGiam < 0)
                return (false, "Số tiền giảm không hợp lệ", null);

            // ✅ Validate số lượng và điều kiện
            if (voucher.SoLuong < 0) return (false, "Số lượng không hợp lệ", null);
            if (voucher.DieuKienToiThieu < 0) return (false, "Điều kiện tối thiểu không hợp lệ", null);

            // ✅ Gán mặc định nếu chưa set số lần sử dụng tối đa
            if (voucher.SoLanSuDungToiDa <= 0)
                voucher.SoLanSuDungToiDa = 1;

            // ✅ Cập nhật
            existing.CodeVoucher = voucher.CodeVoucher;
            existing.MoTa = voucher.MoTa;
            existing.PhanTram = voucher.PhanTram;
            existing.SoTienGiam = voucher.SoTienGiam;
            existing.DieuKienToiThieu = voucher.DieuKienToiThieu;
            existing.SoLuong = voucher.SoLuong;
            existing.SoLanSuDungToiDa = voucher.SoLanSuDungToiDa;
            existing.StartDate = voucher.StartDate;
            existing.EndDate = voucher.EndDate;

            _context.Vouchers.Update(existing);
            await _context.SaveChangesAsync();

            return (true, "Cập nhật thành công", existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.Vouchers.FindAsync(id);
            if (existing == null) return false;

            _context.Vouchers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
