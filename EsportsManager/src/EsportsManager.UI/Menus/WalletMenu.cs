using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.UI.Menus
{
    /// <summary>
    /// Wallet Menu - handles wallet operations
    /// </summary>
    public class WalletMenu
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletMenu> _logger;
        private readonly int _currentUserId;

        public WalletMenu(IWalletService walletService, ILogger<WalletMenu> logger, int currentUserId)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserId = currentUserId;
        }

        /// <summary>
        /// Show wallet menu
        /// </summary>
        public async Task ShowAsync()
        {
            while (true)
            {
                try
                {
                    // Get current balance
                    var balanceResult = await _walletService.GetBalanceAsync(_currentUserId); ConsoleHelper.ShowHeader("Quản Lý Ví Điện Tử");

                    if (balanceResult.IsSuccess)
                    {
                        Console.WriteLine($"Số dư hiện tại: {balanceResult.Data:C2}");
                    }
                    else
                    {
                        Console.WriteLine("Không thể tải số dư. Bạn có thể cần tạo ví trước.");
                    }

                    Console.WriteLine();
                    Console.WriteLine("1. Nạp Tiền");
                    Console.WriteLine("2. Rút Tiền");
                    Console.WriteLine("3. Chuyển Tiền");
                    Console.WriteLine("4. Xem Lịch Sử Giao Dịch");
                    Console.WriteLine("0. Quay Lại");
                    Console.WriteLine();

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 4);

                    switch (choice)
                    {
                        case 1:
                            await DepositFundsAsync();
                            break;
                        case 2:
                            await WithdrawFundsAsync();
                            break;
                        case 3:
                            await TransferFundsAsync();
                            break;
                        case 4:
                            await ViewTransactionHistoryAsync();
                            break;
                        case 0:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong menu ví");
                    ConsoleHelper.ShowError($"Lỗi: {ex.Message}");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }

        /// <summary>
        /// Handle deposit funds
        /// </summary>
        private async Task DepositFundsAsync()
        {
            ConsoleHelper.ShowHeader("Nạp Tiền");

            try
            {
                var amount = ConsoleInput.GetDecimal("Nhập số tiền cần nạp", 1, 10000);

                Console.WriteLine($"Đang nạp {amount:C2}...");

                var result = await _walletService.DepositAsync(_currentUserId, amount); if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Nạp tiền thành công {amount:C2}. Số dư mới: {result.Data!.Balance:C2}");
                }
                else
                {
                    ConsoleHelper.ShowError($"Nạp tiền thất bại: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi nạp tiền");
                ConsoleHelper.ShowError($"Lỗi: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// Handle withdraw funds
        /// </summary>
        private async Task WithdrawFundsAsync()
        {
            ConsoleHelper.ShowHeader("Rút Tiền");

            try
            {
                // Get current balance
                var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);

                if (!balanceResult.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Không thể tải số dư: {balanceResult.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }
                Console.WriteLine($"Số dư hiện tại: {balanceResult.Data:C2}");
                Console.WriteLine();

                var amount = ConsoleInput.GetDecimal("Nhập số tiền cần rút", 1, (decimal)balanceResult.Data);

                Console.WriteLine($"Đang rút {amount:C2}...");

                var result = await _walletService.WithdrawAsync(_currentUserId, amount); if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Rút tiền thành công {amount:C2}. Số dư mới: {result.Data!.Balance:C2}");
                }
                else
                {
                    ConsoleHelper.ShowError($"Rút tiền thất bại: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi rút tiền");
                ConsoleHelper.ShowError($"Lỗi: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// Handle transfer funds
        /// </summary>
        private async Task TransferFundsAsync()
        {
            ConsoleHelper.ShowHeader("Chuyển Tiền");

            try
            {
                // Get current balance
                var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);

                if (!balanceResult.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Không thể tải số dư: {balanceResult.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"Số dư hiện tại: {balanceResult.Data:C2}");
                Console.WriteLine(); var toUserId = ConsoleInput.GetInt("Nhập ID người nhận", 1);

                if (toUserId == _currentUserId)
                {
                    ConsoleHelper.ShowError("Bạn không thể chuyển tiền cho chính mình");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var amount = ConsoleInput.GetDecimal("Nhập số tiền cần chuyển", 1, (decimal)balanceResult.Data);
                var message = ConsoleInput.GetString("Nhập lời nhắn (không bắt buộc)");

                Console.WriteLine($"Đang chuyển {amount:C2} đến người dùng {toUserId}...");

                var result = await _walletService.TransferAsync(_currentUserId, toUserId, amount, message); if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Chuyển tiền thành công {amount:C2} đến người dùng {toUserId}. Số dư mới: {result.Data!.Balance:C2}");
                }
                else
                {
                    ConsoleHelper.ShowError($"Chuyển tiền thất bại: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chuyển tiền");
                ConsoleHelper.ShowError($"Lỗi: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        /// <summary>
        /// View transaction history
        /// </summary>
        private async Task ViewTransactionHistoryAsync()
        {
            ConsoleHelper.ShowHeader("Lịch Sử Giao Dịch");

            try
            {
                var result = await _walletService.GetTransactionHistoryAsync(_currentUserId);

                if (!result.IsSuccess)
                {
                    ConsoleHelper.ShowError($"Không thể tải lịch sử giao dịch: {result.ErrorMessage}");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var transactions = result.Data!;

                if (!transactions.Any())
                {
                    Console.WriteLine("Không tìm thấy giao dịch nào");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine($"{"Ngày",-20} {"Loại",-15} {"Số tiền",-15} {"Số dư",-15} {"Mô tả",-40}");
                Console.WriteLine(new string('-', 105));

                foreach (var transaction in transactions.OrderByDescending(t => t.TransactionDate))
                {
                    Console.WriteLine($"{transaction.TransactionDate,-20} {transaction.TransactionType,-15} {transaction.Amount,-15:C2} {transaction.Balance,-15:C2} {transaction.Description,-40}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem lịch sử giao dịch");
                ConsoleHelper.ShowError($"Lỗi: {ex.Message}");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
}
