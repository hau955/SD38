namespace AppView.Constants
{
    public static class OrderConstants
    {
        public static class OrderStatuses
        {
            public const string CHO_XAC_NHAN = "Chờ xác nhận";
            public const string DA_XAC_NHAN = "Đã xác nhận";
            public const string DANG_GIAO_HANG = "Đang giao hàng";
            public const string HOAN_TAT = "Hoàn tất";
            public const string DA_HUY = "Đã hủy";

            public static readonly List<string> All = new()
            {
                CHO_XAC_NHAN, DA_XAC_NHAN, DANG_GIAO_HANG, HOAN_TAT, DA_HUY
            };

            public static readonly Dictionary<string, string> Colors = new()
            {
                { CHO_XAC_NHAN, "warning" },
                { DA_XAC_NHAN, "info" },
                { DANG_GIAO_HANG, "primary" },
                { HOAN_TAT, "success" },
                { DA_HUY, "danger" }
            };
        }

        public static class PaymentStatuses
        {
            public const string CHUA_THANH_TOAN = "Chưa thanh toán";
            public const string DA_THANH_TOAN = "Đã thanh toán";
            public const string THANH_TOAN_KHI_NHAN = "Thanh toán khi nhận";

            public static readonly List<string> All = new()
            {
                CHUA_THANH_TOAN, DA_THANH_TOAN, THANH_TOAN_KHI_NHAN
            };

            public static readonly Dictionary<string, string> Colors = new()
            {
                { CHUA_THANH_TOAN, "danger" },
                { DA_THANH_TOAN, "success" },
                { THANH_TOAN_KHI_NHAN, "warning" }
            };
        }
    }
}
