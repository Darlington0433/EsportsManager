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

                // Lấy danh sách tournaments trước
                var tournaments = await _tournamentService.GetAllTournamentsAsync();
                if (tournaments.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Không có giải đấu nào để gửi feedback!", false, 2000);
                    return;
                }

                // Hiển thị danh sách tournaments để chọn
                Console.WriteLine("🏆 CHỌN GIẢI ĐẤU ĐỂ GỬI FEEDBACK:");
                for (int i = 0; i < tournaments.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tournaments[i].TournamentName} - Status: {tournaments[i].Status}");
                }

                Console.Write($"\nChọn giải đấu (1-{tournaments.Count}): ");
                if (!int.TryParse(Console.ReadLine(), out int tournamentChoice) || tournamentChoice < 1 || tournamentChoice > tournaments.Count)
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn giải đấu không hợp lệ!", false, 2000);
                    return;
                }

                var selectedTournament = tournaments[tournamentChoice - 1];
                Console.WriteLine($"\n✅ Đã chọn: {selectedTournament.TournamentName}");

                Console.WriteLine("\n📝 LOẠI FEEDBACK:");
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

                    Console.Write("Đánh giá từ 1-5 sao (1=Rất tệ, 5=Rất tốt): ");
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
                            ConsoleRenderingService.ShowMessageBox("Vui lòng nhập đầy đủ thông tin!", false, 2000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Đánh giá phải từ 1-5 sao!", false, 2000);
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
