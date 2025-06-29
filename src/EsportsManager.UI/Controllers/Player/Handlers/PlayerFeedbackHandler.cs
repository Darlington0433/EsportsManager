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
                int borderWidth = 80;
                int borderHeight = 18;
                ConsoleRenderingService.DrawBorder("GỬI FEEDBACK GIẢI ĐẤU", borderWidth, borderHeight);
                int borderLeft = (Console.WindowWidth - borderWidth) / 2;
                int borderTop = (Console.WindowHeight - borderHeight) / 4;
                int cursorY = borderTop + 2;

                // Lấy danh sách tournaments trước
                var tournaments = await _tournamentService.GetAllTournamentsAsync();
                if (tournaments.Count == 0)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("❌ Không có giải đấu nào để gửi feedback!", false, 2000);
                    return;
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("🏆 CHỌN GIẢI ĐẤU ĐỂ GỬI FEEDBACK:");
                for (int i = 0; i < tournaments.Count; i++)
                {
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"{i + 1}. {tournaments[i].TournamentName} - Status: {tournaments[i].Status}");
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write($"Chọn giải đấu (1-{tournaments.Count}): ");
                Console.SetCursorPosition(borderLeft + 28, cursorY - 1);
                if (!int.TryParse(Console.ReadLine(), out int tournamentChoice) || tournamentChoice < 1 || tournamentChoice > tournaments.Count)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn giải đấu không hợp lệ!", false, 2000);
                    return;
                }

                var selectedTournament = tournaments[tournamentChoice - 1];
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"✅ Đã chọn: {selectedTournament.TournamentName}");

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("📝 LOẠI FEEDBACK:");
                Console.SetCursorPosition(borderLeft + 4, cursorY++);
                Console.WriteLine("1. Báo cáo lỗi kỹ thuật");
                Console.SetCursorPosition(borderLeft + 4, cursorY++);
                Console.WriteLine("2. Góp ý cải thiện");
                Console.SetCursorPosition(borderLeft + 4, cursorY++);
                Console.WriteLine("3. Khiếu nại về kết quả");

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Chọn loại feedback (1-3): ");
                Console.SetCursorPosition(borderLeft + 28, cursorY - 1);
                if (int.TryParse(Console.ReadLine(), out int type) && type >= 1 && type <= 3)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Tiêu đề feedback: ");
                    Console.SetCursorPosition(borderLeft + 22, cursorY - 1);
                    string title = Console.ReadLine() ?? "";

                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Nội dung chi tiết: ");
                    Console.SetCursorPosition(borderLeft + 22, cursorY - 1);
                    string content = Console.ReadLine() ?? "";

                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Đánh giá từ 1-5 sao (1=Rất tệ, 5=Rất tốt): ");
                    Console.SetCursorPosition(borderLeft + 44, cursorY - 1);
                    if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
                    {
                        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                        {
                            var feedbackDto = new FeedbackDto
                            {
                                TournamentId = selectedTournament.TournamentId,
                                UserId = _currentUser.Id,
                                Content = $"[{GetFeedbackTypeName(type)}] {title}\n\n{content}",
                                Rating = rating,
                                CreatedAt = DateTime.Now
                            };

                            // Submit feedback through tournament service
                            var result = await _tournamentService.SubmitFeedbackAsync(_currentUser.Id, feedbackDto);

                            Console.SetCursorPosition(borderLeft + 2, cursorY++);
                            if (result)
                            {
                                ConsoleRenderingService.ShowMessageBox($"✅ Feedback cho '{selectedTournament.TournamentName}' đã được gửi thành công!", true, 3000);
                            }
                            else
                            {
                                ConsoleRenderingService.ShowMessageBox("❌ Gửi feedback thất bại! Có thể bạn đã gửi feedback cho giải đấu này rồi.", false, 3000);
                            }
                        }
                        else
                        {
                            Console.SetCursorPosition(borderLeft + 2, cursorY++);
                            ConsoleRenderingService.ShowMessageBox("Vui lòng nhập đầy đủ thông tin!", false, 2000);
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        ConsoleRenderingService.ShowMessageBox("Đánh giá phải từ 1-5 sao!", false, 2000);
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                int borderWidth = 80;
                int borderHeight = 18;
                int borderLeft = (Console.WindowWidth - borderWidth) / 2;
                int borderTop = (Console.WindowHeight - borderHeight) / 4;
                int cursorY = borderTop + borderHeight - 2;
                Console.SetCursorPosition(borderLeft + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }

        private static string GetFeedbackTypeName(int type)
        {
            return type switch
            {
                1 => "BÁO CÁO LỖI KỸ THUẬT",
                2 => "GÓP Ý CẢI THIỆN",
                3 => "KHIẾU NẠI KẾT QUẢ",
                _ => "FEEDBACK"
            };
        }
    }
}
