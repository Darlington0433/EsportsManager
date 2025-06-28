namespace EsportsManager.BL.Constants;

/// <summary>
/// Constants cho các thao tác liên quan đến ví điện tử
/// </summary>
public static class WalletConstants
{
    /// <summary>
    /// Số tiền rút tối thiểu cho người chơi
    /// </summary>
    public const decimal MIN_WITHDRAWAL_AMOUNT = 50_000m;

    /// <summary>
    /// Số tiền rút tối đa cho người chơi
    /// </summary>
    public const decimal MAX_WITHDRAWAL_AMOUNT = 50_000_000m;

    /// <summary>
    /// Phí rút tiền
    /// </summary>
    public const decimal WITHDRAWAL_FEE = 10_000m;

    /// <summary>
    /// Số tiền nạp tối thiểu cho người xem
    /// </summary>
    public const decimal MIN_TOP_UP_AMOUNT = 20_000m;

    /// <summary>
    /// Số tiền nạp tối đa cho người xem
    /// </summary>
    public const decimal MAX_TOP_UP_AMOUNT = 20_000_000m;

    /// <summary>
    /// Số tiền donate tối thiểu
    /// </summary>
    public const decimal MIN_DONATION_AMOUNT = 10_000m;

    /// <summary>
    /// Số tiền donate tối đa
    /// </summary>
    public const decimal MAX_DONATION_AMOUNT = 10_000_000m;

    /// <summary>
    /// Thông báo UI
    /// </summary>
    public static class Messages
    {
        public const string INVALID_OPTION = AppConstants.UI.INVALID_OPTION;
        public const string WALLET_NOT_FOUND = "Không tìm thấy ví!";
        public const string PRESS_ANY_KEY = AppConstants.UI.PRESS_ANY_KEY;
        public const string WITHDRAWAL_SUCCESS = "Rút tiền thành công! Số dư đã được cập nhật.";
        public const string WITHDRAWAL_FAILED = "Rút tiền thất bại!";
        public const string OPERATION_CANCELLED = AppConstants.UI.OPERATION_CANCELLED;
    }

    /// <summary>
    /// Phương thức rút tiền
    /// </summary>
    public static class WithdrawalMethods
    {
        public static readonly Dictionary<string, string> OPTIONS = new()
        {
            { "BankTransfer", "Chuyển khoản ngân hàng" },
            { "EWallet", "Ví điện tử" },
            { "Cash", "Tiền mặt" }
        };
    }

    /// <summary>
    /// Nhà cung cấp ví điện tử
    /// </summary>
    public static class EWalletProviders
    {
        public static readonly Dictionary<int, string> OPTIONS = new()
        {
            { 0, "Momo" },
            { 1, "ZaloPay" },
            { 2, "VNPay" }
        };
    }

    /// <summary>
    /// Phí nạp tiền
    /// </summary>
    public const decimal TOP_UP_FEE = 0m;

    /// <summary>
    /// Phí donate
    /// </summary>
    public const decimal DONATION_FEE_PERCENTAGE = 0.01M; // 1%

    /// <summary>
    /// Phương thức thanh toán hợp lệ
    /// </summary>
    public static readonly string[] VALID_PAYMENT_METHODS = new[]
    {
        "BANK_TRANSFER",
        "MOMO",
        "ZALOPAY",
        "VNPAY",
        "CASH"
    };

    /// <summary>
    /// Trạng thái giao dịch
    /// </summary>
    public const string STATUS_PENDING = "PENDING";
    public const string STATUS_COMPLETED = "COMPLETED";
    public const string STATUS_FAILED = "FAILED";
    public const string STATUS_CANCELLED = "CANCELLED";

    /// <summary>
    /// Format số tiền theo định dạng tiền tệ
    /// </summary>
    public static string FormatCurrency(decimal amount)
    {
        return $"{amount.ToString(AppConstants.Currency.CURRENCY_FORMAT)} {AppConstants.Currency.CURRENCY_CODE}";
    }

    /// <summary>
    /// Số tiền giao dịch tối đa (VND)
    /// </summary>
    public const decimal MAX_TRANSACTION_AMOUNT = 100_000_000;

    /// <summary>
    /// Độ dài tối thiểu của mã giao dịch
    /// </summary>
    public const int MIN_TRANSACTION_ID_LENGTH = 8;

    /// <summary>
    /// Độ dài tối đa của mã giao dịch
    /// </summary>
    public const int MAX_TRANSACTION_ID_LENGTH = 50;

    /// <summary>
    /// Độ dài tối đa của mô tả giao dịch
    /// </summary>
    public const int MAX_DESCRIPTION_LENGTH = 500;

    /// <summary>
    /// Phí giao dịch mặc định (%)
    /// </summary>
    public const decimal DEFAULT_TRANSACTION_FEE_PERCENT = 1.5m;

    /// <summary>
    /// Phí giao dịch tối thiểu (VND)
    /// </summary>
    public const decimal MIN_TRANSACTION_FEE = 1_000;

    /// <summary>
    /// Phí giao dịch tối đa (VND)
    /// </summary>
    public const decimal MAX_TRANSACTION_FEE = 1_000_000;
}
