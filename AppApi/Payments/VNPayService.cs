using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AppData.Models; // DbContext, HoaDon, HinhThucTT

namespace AppApi.Payments;

public class VNPayService : IVNPayService
{
    private readonly VNPaySettings _cfg;
    private readonly  ApplicationDbContext _db;
    public VNPayService(IOptions<VNPaySettings> cfg, ApplicationDbContext db)
    { _cfg = cfg.Value; _db = db; }

    public async Task<string> CreatePaymentUrlAsync(Guid orderId, string clientIp)
    {
        var order = await _db.HoaDons.FirstOrDefaultAsync(x => x.IDHoaDon == orderId)
                    ?? throw new InvalidOperationException("Không tìm thấy hóa đơn");

        // Chỉ cho thanh toán khi chưa paid
        if (!string.Equals(order.TrangThaiThanhToan, "Unpaid", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(order.TrangThaiThanhToan, "Chưa thanh toán", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Đơn không ở trạng thái thanh toán được");

        // Tính số tiền gửi sang VNPay từ DB (tổng sau giảm + ship)
        var total = order.TongTienSauGiam + (order.PhiVanChuyen ?? 0m);
        if (total <= 0) throw new InvalidOperationException("Số tiền không hợp lệ");

        // VNPay: amount = VND * 100 (không có phần thập phân)
        var vnpAmount = ((long)Math.Round(total * 100m, 0)).ToString();

        // Dùng GUID dạng N làm mã tham chiếu (duy nhất)
        var txnRef = order.IDHoaDon.ToString("N");
        var now = DateTime.UtcNow.AddHours(7);
        var expire = now.AddMinutes(15);

        var vnp = new SortedDictionary<string, string>
        {
            ["vnp_Version"] = _cfg.Version,
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _cfg.TmnCode,
            ["vnp_Amount"] = vnpAmount,
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = RemoveDiacritics($"Thanh toan don hang {txnRef}"),
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = _cfg.Locale,
            ["vnp_ReturnUrl"] = _cfg.ReturnUrl,
            ["vnp_IpAddr"] = clientIp ?? "127.0.0.1",
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
            ["vnp_ExpireDate"] = expire.ToString("yyyyMMddHHmmss")
        };

        var query = BuildQuery(vnp);
        var toSign = BuildSignData(vnp);
        var secureHash = HmacSHA512(_cfg.HashSecret, toSign);

        return $"{_cfg.BaseUrl}?{query}&vnp_SecureHash={secureHash}";
    }

    public async Task<(string rspCode, string message)> HandleIpnAsync(IQueryCollection query)
    {
        // Lấy tham số vnp_*
        var raw = query.Where(kv => kv.Key.StartsWith("vnp_"))
                       .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        if (!raw.TryGetValue("vnp_SecureHash", out var receivedHash))
            return ("97", "Missing signature");

        raw.Remove("vnp_SecureHash");
        raw.Remove("vnp_SecureHashType");

        // Ký lại để kiểm tra checksum
        var toSign = BuildSignData(new SortedDictionary<string, string>(raw));
        var calcHash = HmacSHA512(_cfg.HashSecret, toSign);
        if (!calcHash.Equals(receivedHash, StringComparison.OrdinalIgnoreCase))
            return ("97", "Invalid signature");

        // Tìm đơn theo vnp_TxnRef (GUID dạng N)
        var txnRef = raw["vnp_TxnRef"];
        if (!Guid.TryParseExact(txnRef, "N", out var orderId))
            return ("01", "Invalid order");

        var order = await _db.HoaDons.FirstOrDefaultAsync(x => x.IDHoaDon == orderId);
        if (order == null) return ("01", "Order not found");

        // Đối chiếu số tiền (VNPay trả về x100)
        if (!long.TryParse(raw["vnp_Amount"], out var vnpAmount))
            return ("99", "Invalid amount");
        var amountVnd = vnpAmount / 100m;

        var shouldBe = order.TongTienSauGiam + (order.PhiVanChuyen ?? 0m);
        if (amountVnd != shouldBe) return ("04", "Amount mismatch");

        // Idempotent
        if (string.Equals(order.TrangThaiThanhToan, "Paid", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(order.TrangThaiThanhToan, "Đã thanh toán", StringComparison.OrdinalIgnoreCase))
            return ("02", "Order already confirmed");

        var respCode = raw.GetValueOrDefault("vnp_ResponseCode");
        var txnStatus = raw.GetValueOrDefault("vnp_TransactionStatus");
        var payDate = raw.GetValueOrDefault("vnp_PayDate");

        if (respCode == "00" && txnStatus == "00")
        {
            order.TrangThaiThanhToan = "Paid";
            order.NgayThanhToan = ParsePayDate(payDate);

            // Gắn hình thức thanh toán VNPay nếu có
            var vnp = await _db.HinhThucTTs
                .FirstOrDefaultAsync(x => x.TenHinhThucTT.Contains("VNPay") || x.TenHinhThucTT.Contains("VNPAY"));
            if (vnp != null) order.IDHinhThucTT = vnp.IDHinhThucTT;

            await _db.SaveChangesAsync();
            return ("00", "Confirm Success");
        }

        // Thất bại/hủy
        order.TrangThaiThanhToan = "Failed";
        await _db.SaveChangesAsync();
        return ("00", "Recorded failure");
    }

    // Helpers
    private static string BuildQuery(SortedDictionary<string, string> dict) =>
        string.Join("&", dict.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

    private static string BuildSignData(SortedDictionary<string, string> dict) =>
        string.Join("&", dict.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));

    private static string HmacSHA512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static DateTime? ParsePayDate(string? yyyymmddhhmmss) =>
        DateTime.TryParseExact(yyyymmddhhmmss, "yyyyMMddHHmmss", null,
           System.Globalization.DateTimeStyles.AssumeUniversal, out var dt) ? dt : null;

    private static string RemoveDiacritics(string s)
    {
        var norm = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in norm)
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch)
                != System.Globalization.UnicodeCategory.NonSpacingMark) sb.Append(ch);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
