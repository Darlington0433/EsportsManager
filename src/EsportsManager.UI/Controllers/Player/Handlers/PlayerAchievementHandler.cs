using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Player.Handlers
{
    /// <summary>
    /// Handler cho việc xem thành tích cá nhân
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class PlayerAchievementHandler : IPlayerAchievementHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITournamentService _tournamentService;
        private readonly IUserService _userService;
        private readonly IAchievementService _achievementService;

        public PlayerAchievementHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            IUserService userService,
            IAchievementService achievementService)
        {
            _currentUser = currentUser;
            _tournamentService = tournamentService;
            _userService = userService;
            _achievementService = achievementService;
        }

        public async Task HandleViewAchievementsAsync()
        {
            try
            {
                Console.Clear();
                int borderWidth = 80;
                int borderHeight = 20;
                ConsoleRenderingService.DrawBorder("THÀNH TÍCH CÁ NHÂN", borderWidth, borderHeight);
                int borderLeft = (Console.WindowWidth - borderWidth) / 2;
                int borderTop = (Console.WindowHeight - borderHeight) / 4;

                // Ví dụ: In bảng lịch sử giải đấu
                var tournamentHistory = await _achievementService.GetPlayerTournamentHistoryAsync(_currentUser.Id);
                if (tournamentHistory != null && tournamentHistory.Count > 0)
                {
                    var headers = new[] { "Tên giải đấu", "Kết quả", "Vị trí", "Tiền thưởng" };
                    var rows = tournamentHistory.Select(t => new[] {
                        t.TournamentName.Length > 24 ? t.TournamentName.Substring(0, 24) : t.TournamentName,
                        t.Result,
                        $"#{t.Position}",
                        t.PrizeMoney > 0 ? $"{t.PrizeMoney:N0} VND" : "-"
                    }).ToList();
                    int[] colWidths = { 26, 12, 8, 18 }; // Tổng + phân cách <= borderWidth - 4
                    UIHelper.PrintTableInBorder(headers, rows, borderWidth, borderHeight, borderLeft, borderTop, colWidths);
                    int infoY = borderTop + 2 + rows.Count + 2;
                    UIHelper.PrintPromptInBorder($"Tổng cộng: {tournamentHistory.Count} giải đấu", borderLeft, infoY, borderWidth - 4);
                }
                // Dòng tiếp tục để ngoài border
                Console.SetCursorPosition(0, borderTop + borderHeight + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }
        private async Task<int> DisplayPlayerStatsAsync(int borderLeft, int cursorY)
        {
            try
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("📊 THỐNG KÊ TỔNG QUAN");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("─".PadRight(78, '─'));

                // Lấy thống kê người chơi từ IAchievementService
                var playerStats = await _achievementService.GetPlayerStatsAsync(_currentUser.Id);

                if (playerStats != null)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"🏆 Tổng số giải đấu tham gia: {playerStats.TotalTournaments}");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"🥇 Số giải đấu đã thắng: {playerStats.TournamentsWon}");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"🥈 Số lần vào chung kết: {playerStats.FinalsAppearances}");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"🥉 Số lần vào bán kết: {playerStats.SemiFinalsAppearances}");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"💰 Tổng tiền thưởng đã nhận: {playerStats.TotalPrizeMoney:N0} VND");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"📈 Điểm số trung bình: {playerStats.AverageRating:F1}");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"⭐ Xếp hạng hiện tại: #{playerStats.CurrentRanking} toàn quốc");
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("❌ Không thể tải thông tin thống kê.");
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"⚠️ Lỗi khi tải thống kê: {ex.Message}");
            }
            return cursorY;
        }
        private async Task<int> DisplayTournamentHistoryAsync(int borderLeft, int cursorY)
        {
            try
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("\n🏅 LỊCH SỬ GIẢI ĐẤU");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("─".PadRight(78, '─'));

                // Lấy lịch sử giải đấu từ IAchievementService
                var tournamentHistory = await _achievementService.GetPlayerTournamentHistoryAsync(_currentUser.Id);

                if (tournamentHistory != null && tournamentHistory.Count > 0)
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("Tên giải đấu                | Kết quả      | Vị trí | Tiền thưởng");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("─".PadRight(78, '─'));

                    foreach (var tournament in tournamentHistory)
                    {
                        string position = $"#{tournament.Position}";
                        string prize = tournament.PrizeMoney > 0 ? $"{tournament.PrizeMoney:N0} VND" : "-";
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine($"{tournament.TournamentName,-25} | {tournament.Result,-10} | {position,-6} | {prize}");
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("📝 Chưa tham gia giải đấu nào");
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"⚠️ Lỗi khi tải lịch sử giải đấu: {ex.Message}");
            }
            return cursorY;
        }
        private async Task<int> DisplayAwardsAndRankingsAsync(int borderLeft, int cursorY)
        {
            try
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("\n🏆 DANH HIỆU VÀ THÀNH TÍCH");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("─".PadRight(78, '─'));

                // Lấy danh sách thành tích từ IAchievementService
                var achievements = await _achievementService.GetPlayerAchievementsAsync(_currentUser.Id);

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("Danh hiệu                   | Ngày đạt được | Mô tả");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("─".PadRight(78, '─'));

                if (achievements != null && achievements.Count > 0)
                {
                    foreach (var achievement in achievements)
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine($"{achievement.Title,-25} | {achievement.DateAchieved:dd/MM/yyyy} | {achievement.Description}");
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("Chưa có thành tích nào.");
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("\n🌟 ĐIỂM NỔI BẬT");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("─".PadRight(78, '─'));

                var highlights = await _achievementService.GetPlayerHighlightsAsync(_currentUser.Id);

                if (highlights != null && highlights.Count > 0)
                {
                    foreach (var highlight in highlights)
                    {
                        Console.SetCursorPosition(borderLeft + 2, cursorY++);
                        Console.WriteLine($"• {highlight}");
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("• Chưa có điểm nổi bật nào.");
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"⚠️ Lỗi khi tải thành tích: {ex.Message}");
            }
            return cursorY;
        }
    }
}
