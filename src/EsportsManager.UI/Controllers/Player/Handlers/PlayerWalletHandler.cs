using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

namespace EsportsManager.UI.Controllers.Player.Handlers
{
    /// <summary>
    /// Handler cho việc quản lý ví điện tử
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class PlayerWalletHandler : IPlayerWalletHandler
    {
        private readonly UserProfileDto _currentUser;

        public PlayerWalletHandler(UserProfileDto currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task HandleWalletManagementAsync()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("QUẢN LÝ VÍ ĐIỆN TỬ", 80, 15);

                    // Tính vị trí để hiển thị data bên trong border
                    int borderLeft = (Console.WindowWidth - 80) / 2;
                    int borderTop = (Console.WindowHeight - 15) / 4;
                    
                    // Mock wallet balance for demonstration
                    decimal mockBalance = 250000;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    Console.WriteLine($"💰 Số dư hiện tại: {mockBalance:N0} VND");

                    var walletOptions = new[]
                    {
                        "Xem lịch sử giao dịch",
                        "Nạp tiền vào ví (Mock)",
                        "Donation cho streamer (Mock)",
                        "Quay lại menu chính"
                    };

                    int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ VÍ", walletOptions);

                    switch (selection)
                    {
                        case 0:
                            await HandleTransactionHistoryAsync();
                            break;
                        case 1:
                            await HandleDepositAsync();
                            break;
                        case 2:
                            await HandleDonationAsync();
                            break;
                        case 3:
                        case -1:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
                }
            }
        }

        private async Task HandleDepositAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("NẠP TIỀN VÀO VÍ", 80, 10);

                Console.WriteLine("💳 Nhập số tiền muốn nạp:");
                Console.WriteLine("Số tiền tối thiểu: 10,000 VND");
                Console.WriteLine("Số tiền tối đa: 10,000,000 VND");
                Console.Write("Số tiền: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", false, 1500);
                    return;
                }

                if (amount < 10000 || amount > 10000000)
                {
                    ConsoleRenderingService.ShowMessageBox("Số tiền nạp phải từ 10,000 đến 10,000,000 VND!", false, 2000);
                    return;
                }

                Console.WriteLine($"\n💰 Xác nhận nạp {amount:N0} VND vào ví?");
                Console.Write("Nhập 'YES' để xác nhận: ");
                
                if (Console.ReadLine()?.ToUpper() == "YES")
                {
                    await Task.Delay(1000); // Simulate processing
                    
                    ConsoleRenderingService.ShowMessageBox($"✅ Nạp tiền thành công! Đã nạp {amount:N0} VND", true, 2500);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleTransactionHistoryAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ GIAO DỊCH", 80, 15);

                // Mock transaction history
                var mockTransactions = new[]
                {
                    "15/06/2024 09:30 - Nạp tiền: +100,000 VND",
                    "12/06/2024 14:15 - Phí giải đấu: -30,000 VND",
                    "10/06/2024 11:20 - Donation: -25,000 VND",
                    "08/06/2024 16:45 - Nạp tiền: +200,000 VND",
                    "05/06/2024 13:30 - Phí giải đấu: -50,000 VND"
                };

                Console.WriteLine("📊 Lịch sử giao dịch gần đây:");
                Console.WriteLine("─".PadRight(78, '─'));
                Console.WriteLine("Thời gian           | Loại giao dịch    | Số tiền        | Mô tả");
                Console.WriteLine("─".PadRight(78, '─'));

                foreach (var transaction in mockTransactions)
                {
                    Console.WriteLine(transaction);
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
                await Task.Delay(100); // Small delay to make it async
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleDonationAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DONATION CHO STREAMER", 80, 12);

                // Mock streamers list
                var mockStreamers = new[] { "Streamer1", "Streamer2", "Streamer3" };

                Console.WriteLine("🎮 Chọn streamer để donation:");
                for (int i = 0; i < mockStreamers.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {mockStreamers[i]}");
                }

                Console.Write($"\nNhập số thứ tự streamer (1-{mockStreamers.Length}): ");
                if (!int.TryParse(Console.ReadLine(), out int choice) || 
                    choice < 1 || choice > mockStreamers.Length)
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    return;
                }

                var selectedStreamer = mockStreamers[choice - 1];

                Console.Write("Nhập số tiền donation (tối thiểu 1,000 VND): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount < 1000)
                {
                    ConsoleRenderingService.ShowMessageBox("Số tiền donation không hợp lệ (tối thiểu 1,000 VND)!", false, 2000);
                    return;
                }

                Console.Write("Nhập lời nhắn (tùy chọn): ");
                string message = Console.ReadLine() ?? "";

                await Task.Delay(1000); // Simulate processing
                
                ConsoleRenderingService.ShowMessageBox($"✅ Donation thành công! Đã gửi {amount:N0} VND cho {selectedStreamer}", true, 2500);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }
    }
}
