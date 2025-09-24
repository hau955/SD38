namespace AppApi.Payments
{
    public record VNPaySettings
    {
        public string TmnCode { get; init; } = "";
        public string HashSecret { get; init; } = "";
        public string BaseUrl { get; init; } = "";
        public string ReturnUrl { get; init; } = "";
        public string IpnUrl { get; init; } = "";
        public string Version { get; init; } = "2.1.0";
        public string Locale { get; init; } = "vn";
    }
}
