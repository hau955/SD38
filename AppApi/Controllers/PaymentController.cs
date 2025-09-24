using AppApi.Payments;
using Microsoft.AspNetCore.Mvc;

namespace AppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IVNPayService _vnpay;
    public PaymentController(IVNPayService vnpay) => _vnpay = vnpay;

    // MVC gọi để nhận VNPay URL
    [HttpPost("vnpay/create")]
    public async Task<IActionResult> CreateVNPayPayment([FromBody] CreatePaymentRequest req)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = await _vnpay.CreatePaymentUrlAsync(req.OrderId, clientIp);
        return Ok(new { paymentUrl = url });
    }

    // VNPay IPN (server->server) xác nhận kết quả
    [HttpGet("vnpay-ipn")]
    public async Task<IActionResult> VNPayIpn()
    {
        var (code, msg) = await _vnpay.HandleIpnAsync(Request.Query);
        return Ok(new { RspCode = code, Message = msg });
    }
}
