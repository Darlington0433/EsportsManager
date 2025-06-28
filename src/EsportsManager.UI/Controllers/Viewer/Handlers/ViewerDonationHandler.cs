using System;
using System.Linq;
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

                // Hiển thị bảng danh sách player
                Console.WriteLine("👥 DANH SÁCH PLAYER:");
                Console.WriteLine("┌─────────────────────────────────────────────────────────────────────┐");
                Console.WriteLine("│ STT │      USERNAME       │            EMAIL            │    ROLE    │");
                Console.WriteLine("├─────┼─────────────────────┼─────────────────────────────┼────────────┤");

                for (int i = 0; i < playersList.Count; i++)
                {
                    string username = playersList[i].Username?.PadRight(19) ?? "N/A".PadRight(19);
                    string email = (playersList[i].Email ?? "N/A").PadRight(27);
                    string role = "Player".PadRight(10);

                    if (username.Length > 19) username = username.Substring(0, 16) + "...";
                    if (email.Length > 27) email = email.Substring(0, 24) + "...";

                    Console.WriteLine($"│ {(i + 1).ToString().PadLeft(3)} │ {username} │ {email} │ {role} │");
                }

                Console.WriteLine("└─────────────────────────────────────────────────────────────────────┘");

                Console.Write("\n💡 Nhập USERNAME của player bạn muốn donate: ");
                string? inputUsername = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(inputUsername))
                {
                    ConsoleRenderingService.ShowMessageBox("Username không được để trống!", false, 1500);
                    return;
                }

                // Tìm player theo username (không phân biệt hoa thường)
                var selectedPlayer = playersList.FirstOrDefault(p =>
                    string.Equals(p.Username, inputUsername, StringComparison.OrdinalIgnoreCase));

                if (selectedPlayer == null)
                {
                    ConsoleRenderingService.ShowMessageBox($"Không tìm thấy player với username '{inputUsername}'!", false, 2000);
                    return;
                }

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
                Console.WriteLine($"\n💸 Xác nhận donation {amount:N0} VND cho player '{selectedPlayer.Username}'?");
                Console.WriteLine($"   📧 Email: {selectedPlayer.Email ?? "N/A"}");
                Console.Write("Nhập 'YES' để xác nhận: ");

                if (Console.ReadLine()?.ToUpper() == "YES")
                {
                    // Create donation DTO
                    var donationDto = new DonationDto
                    {
                        Amount = amount,
                        Message = message.Length > 0 ? message : "No message",
                        DonationType = "Player", // Custom donation type for player
                        PlayerId = selectedPlayer.Id, // Set the selected player ID
                        TeamId = null,
                        TournamentId = null
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
                        ConsoleRenderingService.ShowMessageBox("❌ Donation thất bại! Vui lòng thử lại.", false, 2000);
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
