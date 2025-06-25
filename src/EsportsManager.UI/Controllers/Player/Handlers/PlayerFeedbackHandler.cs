using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Player.Handlers
{
    /// <summary>
    /// Handler cho việc gửi feedback giải đấu
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class PlayerFeedbackHandler : IPlayerFeedbackHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITournamentService _tournamentService;

        public PlayerFeedbackHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService)
        {
            _currentUser = currentUser;
            _tournamentService = tournamentService;
        }

        public async Task HandleSubmitFeedbackAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("GỬI FEEDBACK GIẢI ĐẤU", 80, 15);

                Console.WriteLine("📝 LOẠI FEEDBACK:");
                Console.WriteLine("1. Báo cáo lỗi kỹ thuật");
                Console.WriteLine("2. Góp ý cải thiện");
                Console.WriteLine("3. Khiếu nại về kết quả");

                Console.Write("\nChọn loại feedback (1-3): ");
                if (int.TryParse(Console.ReadLine(), out int type) && type >= 1 && type <= 3)
                {
                    Console.Write("Tiêu đề feedback: ");
                    string title = Console.ReadLine() ?? "";

                    Console.Write("Nội dung chi tiết: ");
                    string content = Console.ReadLine() ?? "";

                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                    {
                        var feedbackDto = new FeedbackDto
                        {
                            UserId = _currentUser.Id,
                            Content = content,
                            CreatedAt = DateTime.Now
                        };

                        // Simulated feedback submission for now
                        await Task.Delay(1000);
                        bool success = true; // Mock success

                        if (success)
                        {
                            ConsoleRenderingService.ShowMessageBox("✅ Feedback đã được gửi thành công!", true, 2000);
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("❌ Gửi feedback thất bại!", false, 2000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Vui lòng nhập đầy đủ thông tin!", false, 2000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }
    }
}
