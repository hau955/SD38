using AppApi.Features.ShippingAddress.DTOs;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppApi.Features.ShippingAddress.Service
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShippingAddressService> _logger;

        public ShippingAddressService(ApplicationDbContext context, ILogger<ShippingAddressService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ShippingAddressResponseDto>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting shipping addresses for user {UserId}", userId);

                return await _context.DiaChiNhanHangs
                    .Where(x => x.IDUser == userId && x.TrangThai)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenByDescending(x => x.NgayTao)
                    .Select(x => new ShippingAddressResponseDto
                    {
                        Id = x.IDDiaChiNhanHang,
                        DiaChiChiTiet = x.DiaChiChiTiet,
                        SoDienThoai = x.SoDienThoai,
                        HoTenNguoiNhan = x.HoTenNguoiNhan,
                        NgayTao = x.NgayTao,
                        TrangThai = x.TrangThai,
                        IsDefault = x.IsDefault,
                        IDUser = x.IDUser,
                        // Thêm các trường riêng lẻ để có thể edit
                        SoNha = ExtractSoNha(x.DiaChiChiTiet),
                        PhuongXa = ExtractPhuongXa(x.DiaChiChiTiet),
                        QuanHuyen = ExtractQuanHuyen(x.DiaChiChiTiet),
                        TinhThanh = ExtractTinhThanh(x.DiaChiChiTiet),
                        DiaDiemGan = ExtractDiaDiemGan(x.DiaChiChiTiet)
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shipping addresses for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ShippingAddressResponseDto?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting shipping address {AddressId}", id);

                return await _context.DiaChiNhanHangs
                    .Where(x => x.IDDiaChiNhanHang == id && x.TrangThai)
                    .Select(x => new ShippingAddressResponseDto
                    {
                        Id = x.IDDiaChiNhanHang,
                        DiaChiChiTiet = x.DiaChiChiTiet,
                        SoDienThoai = x.SoDienThoai,
                        HoTenNguoiNhan = x.HoTenNguoiNhan,
                        NgayTao = x.NgayTao,
                        TrangThai = x.TrangThai,
                        IsDefault = x.IsDefault,
                        IDUser = x.IDUser,
                        SoNha = ExtractSoNha(x.DiaChiChiTiet),
                        PhuongXa = ExtractPhuongXa(x.DiaChiChiTiet),
                        QuanHuyen = ExtractQuanHuyen(x.DiaChiChiTiet),
                        TinhThanh = ExtractTinhThanh(x.DiaChiChiTiet),
                        DiaDiemGan = ExtractDiaDiemGan(x.DiaChiChiTiet)
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shipping address {AddressId}", id);
                throw;
            }
        }

        public async Task<ShippingAddressResponseDto> CreateAsync(Guid userId, ShippingAddressDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Creating new shipping address for user {UserId}", userId);

                // Kiểm tra số lượng địa chỉ tối đa (VD: 10 địa chỉ)
                var addressCount = await _context.DiaChiNhanHangs
                    .CountAsync(x => x.IDUser == userId && x.TrangThai);

                if (addressCount >= 10)
                {
                    throw new InvalidOperationException("Bạn chỉ có thể tạo tối đa 10 địa chỉ giao hàng");
                }

                var hasExistingAddress = addressCount > 0;

                var newAddress = new DiaChiNhanHang
                {
                    IDDiaChiNhanHang = Guid.NewGuid(),
                    DiaChiChiTiet = GenerateDiaChiChiTiet(dto),
                    SoDienThoai = dto.SoDienThoai.Trim(),
                    HoTenNguoiNhan = dto.HoTenNguoiNhan.Trim(),
                    NgayTao = DateTime.UtcNow,
                    TrangThai = true,
                    IsDefault = !hasExistingAddress,
                    IDUser = userId
                };

                _context.DiaChiNhanHangs.Add(newAddress);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Created shipping address {AddressId} for user {UserId}", newAddress.IDDiaChiNhanHang, userId);

                return new ShippingAddressResponseDto
                {
                    Id = newAddress.IDDiaChiNhanHang,
                    DiaChiChiTiet = newAddress.DiaChiChiTiet,
                    SoDienThoai = newAddress.SoDienThoai,
                    HoTenNguoiNhan = newAddress.HoTenNguoiNhan,
                    NgayTao = newAddress.NgayTao,
                    TrangThai = newAddress.TrangThai,
                    IsDefault = newAddress.IsDefault,
                    IDUser = newAddress.IDUser,
                    SoNha = dto.SoNha,
                    PhuongXa = dto.PhuongXa,
                    QuanHuyen = dto.QuanHuyen,
                    TinhThanh = dto.TinhThanh,
                    DiaDiemGan = dto.DiaDiemGan
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating shipping address for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ShippingAddressResponseDto?> UpdateAsync(Guid id, Guid userId, ShippingAddressDto dto)
        {
            try
            {
                _logger.LogInformation("Updating shipping address {AddressId} for user {UserId}", id, userId);

                var address = await _context.DiaChiNhanHangs
                    .FirstOrDefaultAsync(x => x.IDDiaChiNhanHang == id && x.IDUser == userId && x.TrangThai);

                if (address == null)
                {
                    _logger.LogWarning("Shipping address {AddressId} not found for user {UserId}", id, userId);
                    return null;
                }

                address.DiaChiChiTiet = GenerateDiaChiChiTiet(dto);
                address.SoDienThoai = dto.SoDienThoai.Trim();
                address.HoTenNguoiNhan = dto.HoTenNguoiNhan.Trim();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated shipping address {AddressId} for user {UserId}", id, userId);

                return new ShippingAddressResponseDto
                {
                    Id = address.IDDiaChiNhanHang,
                    DiaChiChiTiet = address.DiaChiChiTiet,
                    SoDienThoai = address.SoDienThoai,
                    HoTenNguoiNhan = address.HoTenNguoiNhan,
                    NgayTao = address.NgayTao,
                    TrangThai = address.TrangThai,
                    IsDefault = address.IsDefault,
                    IDUser = address.IDUser,
                    SoNha = dto.SoNha,
                    PhuongXa = dto.PhuongXa,
                    QuanHuyen = dto.QuanHuyen,
                    TinhThanh = dto.TinhThanh,
                    DiaDiemGan = dto.DiaDiemGan
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shipping address {AddressId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Deleting shipping address {AddressId} for user {UserId}", id, userId);

                var address = await _context.DiaChiNhanHangs
                    .FirstOrDefaultAsync(x => x.IDDiaChiNhanHang == id && x.IDUser == userId && x.TrangThai);

                if (address == null)
                {
                    _logger.LogWarning("Shipping address {AddressId} not found for user {UserId}", id, userId);
                    return false;
                }

                address.TrangThai = false;

                // Nếu đây là địa chỉ mặc định, chọn địa chỉ khác làm mặc định
                if (address.IsDefault)
                {
                    var fallback = await _context.DiaChiNhanHangs
                        .Where(x => x.IDUser == userId && x.TrangThai && x.IDDiaChiNhanHang != id)
                        .OrderBy(x => x.NgayTao)
                        .FirstOrDefaultAsync();

                    if (fallback != null)
                    {
                        fallback.IsDefault = true;
                        _logger.LogInformation("Set fallback address {FallbackId} as default for user {UserId}", fallback.IDDiaChiNhanHang, userId);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Deleted shipping address {AddressId} for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting shipping address {AddressId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<bool> SetDefaultAsync(Guid id, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Setting shipping address {AddressId} as default for user {UserId}", id, userId);

                var address = await _context.DiaChiNhanHangs
                    .FirstOrDefaultAsync(x => x.IDDiaChiNhanHang == id && x.IDUser == userId && x.TrangThai);

                if (address == null)
                {
                    _logger.LogWarning("Shipping address {AddressId} not found for user {UserId}", id, userId);
                    return false;
                }

                // Bỏ default của tất cả địa chỉ khác
                await _context.DiaChiNhanHangs
                    .Where(x => x.IDUser == userId && x.TrangThai && x.IDDiaChiNhanHang != id)
                    .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsDefault, false));

                address.IsDefault = true;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Set shipping address {AddressId} as default for user {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error setting shipping address {AddressId} as default for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ShippingAddressResponseDto?> GetDefaultAddressAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting default shipping address for user {UserId}", userId);

                return await _context.DiaChiNhanHangs
                    .Where(x => x.IDUser == userId && x.TrangThai && x.IsDefault)
                    .Select(x => new ShippingAddressResponseDto
                    {
                        Id = x.IDDiaChiNhanHang,
                        DiaChiChiTiet = x.DiaChiChiTiet,
                        SoDienThoai = x.SoDienThoai,
                        HoTenNguoiNhan = x.HoTenNguoiNhan,
                        NgayTao = x.NgayTao,
                        TrangThai = x.TrangThai,
                        IsDefault = x.IsDefault,
                        IDUser = x.IDUser,
                        SoNha = ExtractSoNha(x.DiaChiChiTiet),
                        PhuongXa = ExtractPhuongXa(x.DiaChiChiTiet),
                        QuanHuyen = ExtractQuanHuyen(x.DiaChiChiTiet),
                        TinhThanh = ExtractTinhThanh(x.DiaChiChiTiet),
                        DiaDiemGan = ExtractDiaDiemGan(x.DiaChiChiTiet)
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default shipping address for user {UserId}", userId);
                throw;
            }
        }

        private static string GenerateDiaChiChiTiet(ShippingAddressDto dto)
        {
            var parts = new List<string?>
            {
                dto.SoNha?.Trim(),
                dto.PhuongXa?.Trim(),
                dto.QuanHuyen?.Trim(),
                dto.TinhThanh?.Trim()
            };

            var diaChi = string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));

            if (!string.IsNullOrWhiteSpace(dto.DiaDiemGan))
                diaChi += $" (Gần {dto.DiaDiemGan.Trim()})";

            return diaChi;
        }

        // Các method extract để parse lại từ DiaChiChiTiet (tạm thời, nên lưu riêng từng trường)
        private static string ExtractSoNha(string diaChiChiTiet)
        {
            if (string.IsNullOrWhiteSpace(diaChiChiTiet)) return string.Empty;

            var parts = diaChiChiTiet.Split(',');
            return parts.Length >= 1 ? parts[0].Trim() : string.Empty;
        }

        private static string ExtractPhuongXa(string diaChiChiTiet)
        {
            if (string.IsNullOrWhiteSpace(diaChiChiTiet)) return string.Empty;

            var parts = diaChiChiTiet.Split(',');
            return parts.Length >= 2 ? parts[1].Trim() : string.Empty;
        }

        private static string ExtractQuanHuyen(string diaChiChiTiet)
        {
            if (string.IsNullOrWhiteSpace(diaChiChiTiet)) return string.Empty;

            var parts = diaChiChiTiet.Split(',');
            return parts.Length >= 3 ? parts[2].Trim() : string.Empty;
        }

        private static string ExtractTinhThanh(string diaChiChiTiet)
        {
            if (string.IsNullOrWhiteSpace(diaChiChiTiet)) return string.Empty;

            var parts = diaChiChiTiet.Split(',');
            if (parts.Length < 4) return string.Empty;

            var lastPart = parts[3].Trim();

            var ganIndex = lastPart.IndexOf(" (Gần");
            if (ganIndex > 0)
                return lastPart.Substring(0, ganIndex).Trim();

            return lastPart;
        }

        private static string? ExtractDiaDiemGan(string diaChiChiTiet)
        {
            if (string.IsNullOrWhiteSpace(diaChiChiTiet)) return null;

            var ganStart = diaChiChiTiet.IndexOf("(Gần ");
            if (ganStart < 0) return null;

            var ganEnd = diaChiChiTiet.IndexOf(")", ganStart);
            if (ganEnd > ganStart)
            {
                return diaChiChiTiet.Substring(ganStart + 6, ganEnd - ganStart - 6).Trim();
            }
            return null;
        }
    }
}