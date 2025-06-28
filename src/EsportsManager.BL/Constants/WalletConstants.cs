namespace EsportsManager.BL.Constants;

/// <summary>
/// Business constants for wallet operations
/// </summary>
public static class WalletConstants
{
    /// <summary>
    /// Minimum withdrawal amount for players
    /// </summary>
    public const decimal MIN_WITHDRAWAL_AMOUNT = 50000m;

    /// <summary>
    /// Withdrawal fee amount
    /// </summary>
    public const decimal WITHDRAWAL_FEE = 5000m;

    /// <summary>
    /// Minimum top-up amount for viewers
    /// </summary>
    public const decimal MIN_TOP_UP_AMOUNT = 10000m;

    /// <summary>
    /// Maximum top-up amount for viewers
    /// </summary>
    public const decimal MAX_TOP_UP_AMOUNT = 10000000m;

    /// <summary>
    /// Minimum donation amount
    /// </summary>
    public const decimal MIN_DONATION_AMOUNT = 1000m;

    // UI Messages
    public const string INVALID_OPTION_MESSAGE = "Lựa chọn không hợp lệ!";
    public const string WALLET_NOT_FOUND_MESSAGE = "Không tìm thấy ví!";
    public const string PRESS_ANY_KEY_MESSAGE = "\nNhấn phím bất kỳ để tiếp tục...";
    public const string WITHDRAWAL_SUCCESS_MESSAGE = "Rút tiền thành công! Số dư đã được cập nhật.";
    public const string WITHDRAWAL_FAILED_MESSAGE = "Rút tiền thất bại!";
    public const string OPERATION_CANCELLED_MESSAGE = "Thao tác đã bị hủy!";

    // Withdrawal methods
    public static readonly Dictionary<string, string> WITHDRAWAL_METHODS = new()
    {
        { "BankTransfer", "Chuyển khoản ngân hàng" },
        { "EWallet", "Ví điện tử" },
        { "Cash", "Tiền mặt" }
    };

    // E-wallet providers
    public static readonly Dictionary<int, string> EWALLET_PROVIDERS = new()
    {
        { 0, "Momo" },
        { 1, "ZaloPay" },
        { 2, "VNPay" }
    };
}
