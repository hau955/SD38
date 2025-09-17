namespace AppApi.Constants
{
    public static class OrderStatus
    {
        public const string CHO_XAC_NHAN = "Chờ xác nhận";
        public const string DA_XAC_NHAN = "Đã xác nhận";
        public const string DANG_GIAO_HANG = "Đang giao hàng";
        public const string HOAN_TAT = "Hoàn tất";
        public const string DA_HUY = "Đã hủy";

        public static readonly List<string> AllStatuses = new List<string>
        {
            CHO_XAC_NHAN, DA_XAC_NHAN, DANG_GIAO_HANG, HOAN_TAT, DA_HUY
        };

        public static readonly Dictionary<string, string> NextStatus = new Dictionary<string, string>
        {
            { CHO_XAC_NHAN, DA_XAC_NHAN },
            { DA_XAC_NHAN, DANG_GIAO_HANG },
            { DANG_GIAO_HANG, HOAN_TAT }
        };

        public static readonly Dictionary<string, List<string>> AllowedTransitions = new Dictionary<string, List<string>>
        {
            { CHO_XAC_NHAN, new List<string> { DA_XAC_NHAN, DA_HUY } },
            { DA_XAC_NHAN, new List<string> { DANG_GIAO_HANG, DA_HUY } },
            { DANG_GIAO_HANG, new List<string> { HOAN_TAT, DA_HUY } },
            { HOAN_TAT, new List<string>() }, // Không thể chuyển từ hoàn tất
            { DA_HUY, new List<string>() } // Không thể chuyển từ đã hủy
        };
    }

    public static class PaymentStatus
    {
        public const string CHUA_THANH_TOAN = "Chưa thanh toán";
        public const string DA_THANH_TOAN = "Đã thanh toán";
        public const string THANH_TOAN_KHI_NHAN = "Thanh toán khi nhận";

        public static readonly List<string> AllStatuses = new List<string>
        {
            CHUA_THANH_TOAN, DA_THANH_TOAN, THANH_TOAN_KHI_NHAN
        };
    }

    public static class OrderStatusColors
    {
        public static readonly Dictionary<string, string> StatusColors = new Dictionary<string, string>
        {
            { OrderStatus.CHO_XAC_NHAN, "warning" },
            { OrderStatus.DA_XAC_NHAN, "info" },
            { OrderStatus.DANG_GIAO_HANG, "primary" },
            { OrderStatus.HOAN_TAT, "success" },
            { OrderStatus.DA_HUY, "danger" }
        };

        public static readonly Dictionary<string, string> PaymentColors = new Dictionary<string, string>
        {
            { PaymentStatus.CHUA_THANH_TOAN, "danger" },
            { PaymentStatus.DA_THANH_TOAN, "success" },
            { PaymentStatus.THANH_TOAN_KHI_NHAN, "warning" }
        };
    }
}
