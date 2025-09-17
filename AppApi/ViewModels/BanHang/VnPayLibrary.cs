using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AppApi.ViewModels.BanHang
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData.Add(key, value);
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            var hashData = new StringBuilder();
            foreach (var kv in _requestData.OrderBy(k => k.Key))
            {
                data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value) + "&");

                // ❗ Chỉ bỏ vnp_SecureHash ra khỏi dữ liệu hash, GIỮ LẠI vnp_SecureHashType
                if (kv.Key != "vnp_SecureHash")
                    hashData.Append(kv.Key + "=" + kv.Value + "&");
            }


            string rawData = hashData.ToString().TrimEnd('&');
            string secureHash = HmacSHA512(vnp_HashSecret, rawData);
            data.Append("vnp_SecureHash=" + secureHash);

            return baseUrl + "?" + data.ToString();
        }

        public bool ValidateSignature(IQueryCollection query, string hashSecret)
        {
            var inputData = query
                .Where(kvp => kvp.Key.StartsWith("vnp_") && kvp.Key != "vnp_SecureHash")

                .OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            var rawData = string.Join("&", inputData.Select(kvp => kvp.Key + "=" + kvp.Value));
            var secureHash = query["vnp_SecureHash"].ToString();
            var checkHash = HmacSHA512(hashSecret, rawData);

            return secureHash.Equals(checkHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public SortedList<string, string> GetFullResponseData(IQueryCollection query)
        {
            foreach (var key in query.Keys)
            {
                if (key.StartsWith("vnp_"))
                    _responseData[key] = query[key];
            }
            return _responseData;
        }

        public static string HmacSHA512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using var hmac = new HMACSHA512(keyBytes);
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}
