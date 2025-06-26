using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.BL.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

namespace EsportsManager.UI.Controllers.Viewer.Handlers
{
    /// <summary>
    /// Handler cho chức năng donation của Viewer
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class ViewerDonationHandler : IViewerDonationHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly IWalletService _walletService;
        private readonly IUserService _userService;
        private readonly WalletValidationService _validationService;

        public ViewerDonationHandler(UserProfileDto currentUser, IWalletService walletService, IUserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _validationService = new WalletValidationService();
        }

        public async Task HandleDonateToPlayerAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 15);

                // Hiển thị danh sách player và cho phép người dùng donate
                Console.WriteLine("\nĐang tải danh sách người chơi...");

                var playerResult = await _userService.GetUsersByRoleAsync("Player");

                if (playerResult.IsSuccess && playerResult.Data != null && playerResult.Data.Any())
                {
                    // Hiển thị danh sách player và chức năng donate
                    await HandleDonationAsync();
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy Player nào để donate!", false, 2000);
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi donate: {ex.Message}", false, 3000);
            }
        }

        // Updated version to use services
        public async Task HandleDonationAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 15);

                // Get actual wallet balance from service
                var wallet = await _walletService.GetWalletByUserIdAsync(_currentUser.Id);
                if (wallet == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin ví!", false, 2000);
                    return;
                }

                Console.WriteLine($"💰 Số dư ví hiện tại: {wallet.Balance:N0} VND");
                Console.WriteLine();

                // Get players from service
                var players = await _userService.GetUsersByRoleAsync("Player");

                List<UserDto> playersList = new List<UserDto>();
                if (players.IsSuccess && players.Data != null)
                {
                    playersList = new List<UserDto>(players.Data);
                }

                if (playersList.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy player nào!", false, 2000);
                    return;
                }

                Console.WriteLine("👥 Chọn player để donation:");
                for (int i = 0; i < playersList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {playersList[i].Username} - {playersList[i].Email ?? "N/A"}");
                }

                Console.Write($"\nNhập số thứ tự player (1-{playersList.Count}): ");
                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    choice < 1 || choice > playersList.Count)
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    return;
                }

                var selectedPlayer = playersList[choice - 1];

                // Nhập số tiền donation
                Console.Write($"Nhập số tiền donation (tối thiểu {WalletConstants.MIN_DONATION_AMOUNT:N0} VND): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    ConsoleRenderingService.ShowMessageBox("Số tiền donation không hợp lệ!", false, 2000);
                    return;
                }

                // Use validation service from BL layer
                var validationResult = _validationService.ValidateDonationRequest(amount, wallet?.Balance);
                if (!validationResult.IsValid)
                {
                    ConsoleRenderingService.ShowMessageBox(validationResult.ErrorMessage, false, 2000);
                    return;
                }

                // Nhập lời nhắn
                Console.Write("Nhập lời nhắn (tùy chọn): ");
                string message = Console.ReadLine() ?? "";

                // Xác nhận donation
                Console.WriteLine($"\n💸 Xác nhận donation {amount:N0} VND cho {selectedPlayer.Username}?");
                Console.Write("Nhập 'YES' để xác nhận: ");

                if (Console.ReadLine()?.ToUpper() == "YES")
                {
                    // Create donation DTO
                    var donationDto = new DonationDto
                    {
                        Amount = amount,
                        Message = message.Length > 0 ? message : "No message",
                        DonationType = "Player", // Custom donation type for player
                        TeamId = null
                    };

                    // Call service to process donation
                    var result = await _walletService.DonateAsync(_currentUser.Id, donationDto);

                    if (result.Success)
                    {
                        Console.Clear();
                        ConsoleRenderingService.DrawBorder("DONATION THÀNH CÔNG", 80, 12);

                        var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 12);

                        string[] successInfo = {
                            $"✅ Đã donation {amount:N0} VND cho {selectedPlayer.Username}!",
                            $"💌 Lời nhắn: {(string.IsNullOrEmpty(message) ? "Không có" : message)}",
                            $"🏷️ Mã giao dịch: {result.Transaction?.Id ?? 0}",
                            $"⏰ Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}",
                            $"💰 Số dư ví còn lại: {result.NewBalance:N0} VND",
                            "",
                            "Cảm ơn bạn đã ủng hộ player!",
                            "Nhấn Enter để tiếp tục..."
                        };

                        ConsoleRenderingService.WriteMultipleInBorder(successInfo, left, top, 0);
                        Console.ReadLine();
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Đã hủy donation", false, 1500);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }
    }
}
