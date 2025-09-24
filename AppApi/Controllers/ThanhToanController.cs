using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhToanController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ThanhToanController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("momo")]
        public async Task<IActionResult> TaoThanhToanMoMo(Guid idHoaDon, decimal tongTien)
        {
            string endpoint = _config["MoMo:Endpoint"];
            string partnerCode = _config["MoMo:PartnerCode"];
            string accessKey = _config["MoMo:AccessKey"];
            string secretKey = _config["MoMo:SecretKey"];

            string orderInfo = "Thanh toán đơn hàng " + idHoaDon;
            string redirectUrl = _config["MoMo:RedirectUrl"];
            string ipnUrl = _config["MoMo:IpnUrl"];
            string requestId = Guid.NewGuid().ToString();
            string orderId = idHoaDon.ToString();
            string amount = tongTien.ToString("0");
            string requestType = "captureWallet";

            // raw data để ký
            string rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            // ký bằng HMAC SHA256
            string signature;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                signature = BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }

            var message = new
            {
                partnerCode,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                requestType,
                extraData = "",
                signature,
                lang = "vi"
            };

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(endpoint, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(result);

                if (json?.payUrl != null)
                {
                    return Ok(new { url = json.payUrl.ToString() });
                }
                else
                {
                    return BadRequest(result);
                }
            }
        }

        // MoMo gọi callback sau khi thanh toán
        [HttpPost("callback")]
        public IActionResult MoMoCallback([FromBody] dynamic data)
        {
            Console.WriteLine("MoMo Callback: " + data.ToString());

            if (data != null && data.resultCode == 0)
            {
                // TODO: cập nhật trạng thái hóa đơn = Đã thanh toán
            }

            return Ok();
        }
    }
}
