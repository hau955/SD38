using Microsoft.AspNetCore.Http;

namespace AppApi.Payments;
public interface IVNPayService
{
    Task<string> CreatePaymentUrlAsync(Guid orderId, string clientIp);
    Task<(string rspCode, string message)> HandleIpnAsync(IQueryCollection query);
}
