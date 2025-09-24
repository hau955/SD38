using Microsoft.AspNetCore.Mvc;

namespace AppView.Controllers
{
    public class PaymentController : Controller
    {
        // VNPay redirect về đây sau khi người dùng thanh toán
        [HttpGet("/payment/result")]
        public IActionResult Result()
        {
            var q = Request.Query;
            // CHỈ hiển thị, KHÔNG cập nhật DB ở đây
            var success = q["vnp_ResponseCode"] == "00" && q["vnp_TransactionStatus"] == "00";
            ViewBag.IsSuccess = success;
            ViewBag.OrderCode = q["vnp_TxnRef"].ToString();
            ViewBag.Amount = decimal.TryParse(q["vnp_Amount"], out var a) ? (a / 100m) : 0m;
            ViewBag.TransactionNo = q["vnp_TransactionNo"].ToString();
            ViewBag.Msg = success ? "Thanh toán thành công"
                                  : $"Thanh toán thất bại ({q["vnp_ResponseCode"]}/{q["vnp_TransactionStatus"]})";
            return View();
        }
    }
}
