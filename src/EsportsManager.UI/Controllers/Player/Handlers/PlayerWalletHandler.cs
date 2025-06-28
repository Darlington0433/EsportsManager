using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.BL.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Player.Handlers;

/// <summary>
/// Handler for player wallet operations following 3-layer architecture
/// Business logic moved to BL layer
/// </summary>
public class PlayerWalletHandler
{
    private readonly UserProfileDto _currentUser;
    private readonly IWalletService _walletService;

    public PlayerWalletHandler(
        UserProfileDto currentUser,
        IWalletService walletService)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
    }

    /// <summary>
    /// Main wallet management menu for players
    /// </summary>
    public async Task HandleWalletManagementAsync()
    {
        while (true)
        {
            var walletOptions = new[]
            {
                "Xem số dư ví",
                "Lịch sử giao dịch",
                "Rút tiền",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu(
                "QUẢN LÝ VÍ QUYÊN GÓP", walletOptions);

            switch (selection)
            {
                case 0:
                    await ViewWalletBalanceAsync();
                    break;
                case 1:
                    await ViewTransactionHistoryAsync();
                    break;
                case 2:
                    await WithdrawMoneyAsync();
                    break;
                case 3:
                case -1:
                    return;
                default:
                    ConsoleRenderingService.ShowNotification(
                        WalletConstants.Messages.INVALID_OPTION, ConsoleColor.Red);
                    break;
            }
        }
    }

    /// <summary>
    /// Display wallet balance information
    /// </summary>
    private async Task ViewWalletBalanceAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("SỐ DƯ VÍ QUYÊN GÓP", 60, 12);

            var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);

            if (wallet != null)
            {
                Console.WriteLine($"\n💰 Số dư hiện tại: {wallet.Balance:N0} VND");
                Console.WriteLine($"📅 Cập nhật lần cuối: {wallet.LastUpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");

                // Display recent donation summary using BL service
                var summary = await _walletService.GetWalletStatsAsync(_currentUser.Id);
                if (summary != null)
                {
                    Console.WriteLine($"\n📊 Thống kê giao dịch:");
                    Console.WriteLine($"   - Tổng thu nhập: {summary.TotalIncome:N0} VND");
                    Console.WriteLine($"   - Tổng chi tiêu: {summary.TotalExpense:N0} VND");
                    Console.WriteLine($"   - Số giao dịch: {summary.TotalTransactions}");
                }
            }
            else
            {
                ConsoleRenderingService.ShowNotification(
                    WalletConstants.Messages.WALLET_NOT_FOUND, ConsoleColor.Yellow);
            }

            Console.WriteLine(WalletConstants.Messages.PRESS_ANY_KEY);
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            // Show detailed error for debugging
            Console.WriteLine($"Chi tiết lỗi: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Log the current user ID for debugging
            Console.WriteLine($"Current User ID: {_currentUser.Id}");

            ConsoleRenderingService.ShowMessageBox(
                $"Không thể tải thông tin ví. Lỗi: {ex.Message}", true, 5000);
        }
    }

    /// <summary>
    /// Display transaction history
    /// </summary>
    private async Task ViewTransactionHistoryAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("LỊCH SỬ GIAO DỊCH", 80, 20);

            var transactions = await _walletService.GetTransactionHistoryAsync(_currentUser.Id);

            if (transactions == null || !transactions.Any())
            {
                ConsoleRenderingService.ShowNotification(
                    "Không có giao dịch nào!", ConsoleColor.Yellow);
            }
            else
            {
                DisplayTransactionTable(transactions);
            }

            Console.WriteLine(WalletConstants.Messages.PRESS_ANY_KEY);
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chi tiết lỗi transaction history: {ex.Message}");
            ConsoleRenderingService.ShowMessageBox(
                $"Không thể tải lịch sử giao dịch. Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Handle money withdrawal with BL validation
    /// </summary>
    private async Task WithdrawMoneyAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("RÚT TIỀN", 60, 15);

            // Get current balance using BL service
            var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
            if (wallet == null)
            {
                ConsoleRenderingService.ShowNotification(
                    WalletConstants.Messages.WALLET_NOT_FOUND, ConsoleColor.Red);
                return;
            }

            Console.WriteLine($"💰 Số dư hiện tại: {wallet.Balance:N0} VND");
            Console.WriteLine($"💡 Số tiền tối thiểu để rút: {WalletConstants.MIN_WITHDRAWAL_AMOUNT:N0} VND");

            // Get withdrawal amount with BL validation
            Console.Write("Nhập số tiền muốn rút (VND): ");
            var amountInput = Console.ReadLine();

            var validation = WalletValidationService.ValidateWithdrawalAmount(amountInput, wallet.Balance);
            if (!validation.IsValid)
            {
                ConsoleRenderingService.ShowNotification(validation.ErrorMessage, ConsoleColor.Red);
                Thread.Sleep(2000);
                return;
            }

            // Get withdrawal method
            var withdrawalMethod = GetWithdrawalMethod();
            if (string.IsNullOrEmpty(withdrawalMethod))
            {
                return; // User cancelled
            }

            // Get withdrawal details
            var withdrawalDetails = GetWithdrawalDetails(withdrawalMethod);
            if (string.IsNullOrEmpty(withdrawalDetails))
            {
                return; // User cancelled or invalid input
            }

            // Confirm withdrawal
            Console.WriteLine($"\n📋 Xác nhận thông tin rút tiền:");
            Console.WriteLine($"   Số tiền: {validation.ValidatedAmount:N0} VND");
            Console.WriteLine($"   Phương thức: {withdrawalMethod}");
            Console.WriteLine($"   Chi tiết: {withdrawalDetails}");
            Console.WriteLine($"   Phí rút tiền: {WalletConstants.WITHDRAWAL_FEE:N0} VND");
            Console.WriteLine($"   Số tiền thực nhận: {validation.ValidatedAmount - WalletConstants.WITHDRAWAL_FEE:N0} VND");

            Console.Write("\nXác nhận rút tiền? (y/n): ");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                var withdrawalRequest = new WithdrawalDto
                {
                    Amount = validation.ValidatedAmount,
                    BankAccount = withdrawalDetails,
                    BankName = withdrawalMethod
                };

                var result = await _walletService.WithdrawAsync(_currentUser.Id, withdrawalRequest);

                if (result.Success)
                {
                    // Show success message with updated balance
                    var updatedWallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
                    var balanceMessage = updatedWallet != null
                        ? $"\n💰 Số dư mới: {updatedWallet.Balance:N0} VND"
                        : "";

                    ConsoleRenderingService.ShowNotification(
                        WalletConstants.Messages.WITHDRAWAL_SUCCESS + balanceMessage, ConsoleColor.Green);
                }
                else
                {
                    ConsoleRenderingService.ShowNotification(
                        result.Message ?? WalletConstants.Messages.WITHDRAWAL_FAILED,
                        ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleRenderingService.ShowNotification(
                    WalletConstants.Messages.OPERATION_CANCELLED, ConsoleColor.Yellow);
            }

            Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chi tiết lỗi withdrawal: {ex.Message}");
            ConsoleRenderingService.ShowMessageBox(
                $"Không thể thực hiện rút tiền. Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Get withdrawal method from user
    /// </summary>
    private string GetWithdrawalMethod()
    {
        var methods = WalletConstants.WithdrawalMethods.OPTIONS;
        var methodOptions = methods.Select(m => m.Value).Concat(new[] { "❌ Hủy" }).ToArray();

        int methodSelection = InteractiveMenuService.DisplayInteractiveMenu(
            "CHỌN PHƯƠNG THỨC RÚT TIỀN", methodOptions);

        if (methodSelection == -1 || methodSelection == methods.Count)
            return string.Empty;

        return methods.ElementAt(methodSelection).Key;
    }

    /// <summary>
    /// Get withdrawal details based on method
    /// </summary>
    private string GetWithdrawalDetails(string method)
    {
        try
        {
            switch (method)
            {
                case "BankTransfer":
                    Console.Write("Số tài khoản ngân hàng: ");
                    var bankAccount = Console.ReadLine()?.Trim();
                    Console.Write("Tên ngân hàng: ");
                    var bankName = Console.ReadLine()?.Trim();
                    Console.Write("Chủ tài khoản: ");
                    var accountHolder = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(bankAccount) || string.IsNullOrEmpty(bankName) ||
                        string.IsNullOrEmpty(accountHolder))
                    {
                        ConsoleRenderingService.ShowNotification(
                            "Vui lòng nhập đầy đủ thông tin!", ConsoleColor.Red);
                        return string.Empty;
                    }

                    return $"Bank: {bankName}, Account: {bankAccount}, Holder: {accountHolder}";

                case "EWallet":
                    var ewalletOptions = WalletConstants.EWalletProviders.OPTIONS.Values.Concat(new[] { "❌ Hủy" }).ToArray();
                    int ewalletChoice = InteractiveMenuService.DisplayInteractiveMenu(
                        "CHỌN VÍ ĐIỆN TỬ", ewalletOptions);

                    if (ewalletChoice == -1 || ewalletChoice == WalletConstants.EWalletProviders.OPTIONS.Count)
                        return string.Empty;

                    var selectedEwallet = WalletConstants.EWalletProviders.OPTIONS.ElementAt(ewalletChoice);
                    Console.Write($"Số điện thoại {selectedEwallet.Value}: ");
                    var phone = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(phone))
                    {
                        ConsoleRenderingService.ShowNotification(
                            "Số điện thoại không được để trống!", ConsoleColor.Red);
                        return string.Empty;
                    }

                    return $"{selectedEwallet.Value}: {phone}";

                case "Cash":
                    return "Nhận tiền mặt tại văn phòng";

                default:
                    return string.Empty;
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Display transaction history in table format
    /// </summary>
    private void DisplayTransactionTable(IEnumerable<TransactionDto> transactions)
    {
        var header = string.Format("{0,-15} {1,-12} {2,-15} {3,-20} {4,-15}",
            "Ngày", "Loại", "Số tiền", "Từ/Đến", "Trạng thái");

        Console.WriteLine(header);
        Console.WriteLine(new string('─', 77));

        foreach (var transaction in transactions.Take(10)) // Show last 10 transactions
        {
            var typeDisplay = transaction.TransactionType switch
            {
                "Donation" => "Quyên góp",
                "Withdrawal" => "Rút tiền",
                "TopUp" => "Nạp tiền",
                _ => transaction.TransactionType
            };

            var statusDisplay = transaction.Status switch
            {
                "Completed" => "Hoàn thành",
                "Pending" => "Chờ xử lý",
                "Failed" => "Thất bại",
                _ => transaction.Status
            };

            var row = string.Format("{0,-15} {1,-12} {2,-15} {3,-20} {4,-15}",
                transaction.CreatedAt.ToString("dd/MM/yyyy"),
                typeDisplay,
                $"{transaction.Amount:N0} VND",
                transaction.Note?.Length > 20 ?
                    transaction.Note.Substring(0, 17) + "..." :
                    transaction.Note ?? "",
                statusDisplay);

            // Color code based on transaction type
            var color = transaction.TransactionType switch
            {
                "Donation" => ConsoleColor.Green,
                "Withdrawal" => ConsoleColor.Yellow,
                "TopUp" => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine(row);
            Console.ResetColor();
        }

        if (transactions.Count() > 10)
        {
            Console.WriteLine($"\n... và {transactions.Count() - 10} giao dịch khác");
        }
    }
}