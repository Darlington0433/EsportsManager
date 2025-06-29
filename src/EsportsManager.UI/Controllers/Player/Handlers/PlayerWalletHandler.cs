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
            int borderWidth = 60;
            int borderHeight = 12;
            ConsoleRenderingService.DrawBorder("SỐ DƯ VÍ QUYÊN GÓP", borderWidth, borderHeight);
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            int cursorY = borderTop + 2;

            var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);

            if (wallet != null)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"💰 Số dư hiện tại: {wallet.Balance:N0} VND");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"📅 Cập nhật lần cuối: {wallet.LastUpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");

                var summary = await _walletService.GetWalletStatsAsync(_currentUser.Id);
                if (summary != null)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"📊 Thống kê giao dịch:");
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"- Tổng thu nhập: {summary.TotalIncome:N0} VND");
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"- Tổng chi tiêu: {summary.TotalExpense:N0} VND");
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"- Số giao dịch: {summary.TotalTransactions}");
                }
            }
            else
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowNotification(WalletConstants.Messages.WALLET_NOT_FOUND, ConsoleColor.Yellow);
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            Console.WriteLine(WalletConstants.Messages.PRESS_ANY_KEY);
            Console.SetCursorPosition(borderLeft + 30, borderTop + borderHeight - 2);
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            int borderWidth = 60;
            int borderHeight = 12;
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            ConsoleRenderingService.ShowMessageBox($"Không thể tải thông tin ví. Lỗi: {ex.Message}", true, 5000);
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
            int borderWidth = 80;
            int borderHeight = 20;
            ConsoleRenderingService.DrawBorder("LỊCH SỬ GIAO DỊCH", borderWidth, borderHeight);
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            int cursorY = borderTop + 2;

            var transactions = await _walletService.GetTransactionHistoryAsync(_currentUser.Id);

            if (transactions == null || !transactions.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowNotification("Không có giao dịch nào!", ConsoleColor.Yellow);
            }
            else
            {
                DisplayTransactionHistoryInBorder(transactions, borderLeft, cursorY, borderWidth - 4);
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            Console.WriteLine(WalletConstants.Messages.PRESS_ANY_KEY);
            Console.SetCursorPosition(borderLeft + 30, borderTop + borderHeight - 2);
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            int borderWidth = 80;
            int borderHeight = 20;
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            ConsoleRenderingService.ShowMessageBox($"Không thể tải lịch sử giao dịch. Lỗi: {ex.Message}", true, 3000);
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
            int borderWidth = 60;
            int borderHeight = 15;
            ConsoleRenderingService.DrawBorder("RÚT TIỀN", borderWidth, borderHeight);
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            int cursorY = borderTop + 2;

            // Get current balance using BL service
            var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
            if (wallet == null)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowNotification(
                    WalletConstants.Messages.WALLET_NOT_FOUND, ConsoleColor.Red);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.WriteLine($"💰 Số dư hiện tại: {wallet.Balance:N0} VND");
            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.WriteLine($"💡 Số tiền tối thiểu để rút: {WalletConstants.MIN_WITHDRAWAL_AMOUNT:N0} VND");

            // Get withdrawal amount with BL validation
            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.Write("Nhập số tiền muốn rút (VND): ");
            Console.SetCursorPosition(borderLeft + 32, cursorY - 1);
            var amountInput = Console.ReadLine();

            var validation = WalletValidationService.ValidateWithdrawalAmount(amountInput, wallet.Balance);
            if (!validation.IsValid)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowNotification(validation.ErrorMessage, ConsoleColor.Red);
                Thread.Sleep(2000);
                return;
            }

            // Get withdrawal method
            var withdrawalMethod = GetWithdrawalMethod(borderLeft, ref cursorY);
            if (string.IsNullOrEmpty(withdrawalMethod))
            {
                return; // User cancelled
            }

            // Get withdrawal details
            var withdrawalDetails = GetWithdrawalDetails(withdrawalMethod, borderLeft, ref cursorY);
            if (string.IsNullOrEmpty(withdrawalDetails))
            {
                return; // User cancelled or invalid input
            }

            // Confirm withdrawal
            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.WriteLine($"\n📋 Xác nhận thông tin rút tiền:");
            Console.SetCursorPosition(borderLeft + 4, cursorY++);
            Console.WriteLine($"Số tiền: {validation.ValidatedAmount:N0} VND");
            Console.SetCursorPosition(borderLeft + 4, cursorY++);
            Console.WriteLine($"Phương thức: {withdrawalMethod}");
            Console.SetCursorPosition(borderLeft + 4, cursorY++);
            Console.WriteLine($"Chi tiết: {withdrawalDetails}");
            Console.SetCursorPosition(borderLeft + 4, cursorY++);
            Console.WriteLine($"Phí rút tiền: {WalletConstants.WITHDRAWAL_FEE:N0} VND");
            Console.SetCursorPosition(borderLeft + 4, cursorY++);
            Console.WriteLine($"Số tiền thực nhận: {validation.ValidatedAmount - WalletConstants.WITHDRAWAL_FEE:N0} VND");

            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.Write("Xác nhận rút tiền? (y/n): ");
            Console.SetCursorPosition(borderLeft + 28, cursorY - 1);
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
            int borderWidth = 60;
            int borderHeight = 15;
            int borderLeft = (Console.WindowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            ConsoleRenderingService.ShowMessageBox(
                $"Không thể thực hiện rút tiền. Lỗi: {ex.Message}", true, 3000);
        }
    }

    // Overload for border and cursorY
    private string GetWithdrawalMethod(int borderLeft, ref int cursorY)
    {
        var methods = WalletConstants.WithdrawalMethods.OPTIONS;
        var methodOptions = methods.Select(m => m.Value).Concat(new[] { "❌ Hủy" }).ToArray();

        int methodSelection = InteractiveMenuService.DisplayInteractiveMenu(
            "CHỌN PHƯƠNG THỨC RÚT TIỀN", methodOptions);

        if (methodSelection == -1 || methodSelection == methods.Count)
            return string.Empty;

        cursorY += 2; // Tăng dòng cho giao diện
        return methods.ElementAt(methodSelection).Key;
    }

    // Overload for border and cursorY
    private string GetWithdrawalDetails(string method, int borderLeft, ref int cursorY)
    {
        try
        {
            switch (method)
            {
                case "BankTransfer":
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Số tài khoản ngân hàng: ");
                    Console.SetCursorPosition(borderLeft + 28, cursorY - 1);
                    var bankAccount = Console.ReadLine()?.Trim();
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Tên ngân hàng: ");
                    Console.SetCursorPosition(borderLeft + 18, cursorY - 1);
                    var bankName = Console.ReadLine()?.Trim();
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Chủ tài khoản: ");
                    Console.SetCursorPosition(borderLeft + 18, cursorY - 1);
                    var accountHolder = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(bankAccount) || string.IsNullOrEmpty(bankName) ||
                        string.IsNullOrEmpty(accountHolder))
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
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
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write($"Số điện thoại {selectedEwallet.Value}: ");
                    Console.SetCursorPosition(borderLeft + 24, cursorY - 1);
                    var phone = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(phone))
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
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
    private void DisplayTransactionHistoryInBorder(IEnumerable<TransactionDto> transactions, int startX, int startY, int maxWidth)
    {
        var headers = new[] { "ID", "Loại", "Số tiền", "Thời gian", "Trạng thái" };
        var rows = transactions.Select(t => new[] {
            t.Id.ToString(),
            t.TransactionType,
            t.Amount.ToString("N0"),
            t.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            t.Status
        }).ToList();
        int borderWidth = maxWidth;
        int borderHeight = 16;
        int[] colWidths = { 5, 12, 14, 20, 12 }; // Tổng + phân cách <= borderWidth - 4
        UIHelper.PrintTableInBorder(headers, rows, borderWidth, borderHeight, startX, startY, colWidths);
        int infoY = startY + 2 + rows.Count + 2;
        UIHelper.PrintPromptInBorder($"Tổng cộng: {transactions.Count()} giao dịch", startX, infoY, borderWidth - 4);
        Console.SetCursorPosition(0, startY + borderHeight + 1);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
    }
}