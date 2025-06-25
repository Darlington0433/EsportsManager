using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
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

        public ViewerDonationHandler(UserProfileDto currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task HandleDonateToPlayerAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 15);

                // Implement donation logic here
                Console.WriteLine("💰 Chức năng donate cho player đang được phát triển...");
                
                await Task.Delay(100); // Placeholder async operation
                
                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi donate: {ex.Message}", false, 3000);
            }
        }

        // Keep the old method for backward compatibility during transition
        public async Task HandleDonationAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 15);

                // Mock wallet balance
                decimal mockBalance = 500000;
                Console.WriteLine($"💰 Số dư ví hiện tại: {mockBalance:N0} VND");
                Console.WriteLine();

                // Mock players list
                var mockPlayers = new[]
                {
                    ("Player1", "player1@email.com"),
                    ("Player2", "player2@email.com"),
                    ("Player3", "player3@email.com"),
                    ("Player4", "player4@email.com")
                };

                Console.WriteLine("👥 Chọn player để donation:");
                for (int i = 0; i < mockPlayers.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {mockPlayers[i].Item1} - {mockPlayers[i].Item2}");
                }

                Console.Write($"\nNhập số thứ tự player (1-{mockPlayers.Length}): ");
                if (!int.TryParse(Console.ReadLine(), out int choice) || 
                    choice < 1 || choice > mockPlayers.Length)
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    return;
                }

                var selectedPlayer = mockPlayers[choice - 1];

                // Nhập số tiền donation
                Console.Write("Nhập số tiền donation (tối thiểu 1,000 VND): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount < 1000)
                {
                    ConsoleRenderingService.ShowMessageBox("Số tiền donation không hợp lệ (tối thiểu 1,000 VND)!", false, 2000);
                    return;
                }

                if (amount > mockBalance)
                {
                    ConsoleRenderingService.ShowMessageBox("Số dư ví không đủ!", false, 2000);
                    return;
                }

                // Nhập lời nhắn
                Console.Write("Nhập lời nhắn (tùy chọn): ");
                string message = Console.ReadLine() ?? "";

                // Xác nhận donation
                Console.WriteLine($"\n💸 Xác nhận donation {amount:N0} VND cho {selectedPlayer.Item1}?");
                Console.Write("Nhập 'YES' để xác nhận: ");
                
                if (Console.ReadLine()?.ToUpper() == "YES")
                {
                    await Task.Delay(1000); // Simulate processing
                    
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("DONATION THÀNH CÔNG", 80, 12);
                    
                    var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 12);
                    
                    // Hiển thị số dư mới
                    var newBalance = mockBalance - amount;
                    
                    string[] successInfo = {
                        $"✅ Đã donation {amount:N0} VND cho {selectedPlayer.Item1}!",
                        $"💌 Lời nhắn: {(string.IsNullOrEmpty(message) ? "Không có" : message)}",
                        $"🏷️ Mã giao dịch: DN{DateTime.Now:yyyyMMddHHmm}",
                        $"⏰ Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}",
                        $"💰 Số dư ví còn lại: {newBalance:N0} VND",
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
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }
    }
}
