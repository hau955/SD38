using AppApi.Features.ShippingAddress.DTOs;

namespace AppApi.Features.ShippingAddress.Service
{
    public interface IShippingAddressService
    {
        Task<IEnumerable<ShippingAddressResponseDto>> GetByUserIdAsync(Guid userId);
        Task<ShippingAddressResponseDto?> GetByIdAsync(Guid id);
        Task<ShippingAddressResponseDto> CreateAsync(Guid userId, ShippingAddressDto dto);
        Task<ShippingAddressResponseDto?> UpdateAsync(Guid id, Guid userId, ShippingAddressDto dto);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> SetDefaultAsync(Guid id, Guid userId);
        Task<ShippingAddressResponseDto?> GetDefaultAddressAsync(Guid userId);
    }
}
