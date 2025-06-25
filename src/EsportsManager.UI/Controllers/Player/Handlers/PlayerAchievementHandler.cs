using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

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

        public PlayerAchievementHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            IUserService userService)
        {
            _currentUser = currentUser;
            _tournamentService = tournamentService;
            _userService = userService;
        }

        public async Task HandleViewAchievementsAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THÀNH TÍCH CÁ NHÂN", 80, 20);

                await DisplayPlayerStatsAsync();
                await DisplayTournamentHistoryAsync();
                await DisplayAwardsAndRankingsAsync();

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi hệ thống: {ex.Message}", false, 2000);
            }
        }

        private async Task DisplayPlayerStatsAsync()
        {
            try
            {
                Console.WriteLine("📊 THỐNG KÊ TỔNG QUAN");
                Console.WriteLine("─".PadRight(78, '─'));

                // Mock player stats for demonstration
                await Task.Delay(100); // Small delay to make it async
                
                Console.WriteLine($"🏆 Tổng số giải đấu tham gia: 12");
                Console.WriteLine($"🥇 Số giải đầu đã thắng: 3");
                Console.WriteLine($"🥈 Số lần vào chung kết: 5");
                Console.WriteLine($"🥉 Số lần vào bán kết: 8");
                Console.WriteLine($"💰 Tổng tiền thưởng đã nhận: 2,500,000 VND");
                Console.WriteLine($"📈 Điểm số trung bình: 8.5");
                Console.WriteLine($"⭐ Xếp hạng hiện tại: #15 toàn quốc");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi tải thống kê: {ex.Message}");
            }
        }

        private async Task DisplayTournamentHistoryAsync()
        {
            try
            {
                Console.WriteLine("\n🏅 LỊCH SỬ GIẢI ĐẤU");
                Console.WriteLine("─".PadRight(78, '─'));

                var tournaments = await _tournamentService.GetAllTournamentsAsync();
                
                if (tournaments.Count > 0)
                {
                    Console.WriteLine("Tên giải đấu                | Kết quả      | Vị trí | Tiền thưởng");
                    Console.WriteLine("─".PadRight(78, '─'));

                    // Mock tournament history
                    var mockHistory = new[]
                    {
                        ("LOL Championship 2024", "Vô địch", "#1", "1,000,000 VND"),
                        ("CS:GO Masters", "Á quân", "#2", "500,000 VND"),
                        ("PUBG Mobile Cup", "Bán kết", "#4", "200,000 VND"),
                        ("FIFA Online League", "Vòng loại", "#8", "-")
                    };

                    foreach (var (name, result, position, prize) in mockHistory)
                    {
                        Console.WriteLine($"{name,-25} | {result,-10} | {position,-6} | {prize}");
                    }
                }
                else
                {
                    Console.WriteLine("📝 Chưa tham gia giải đấu nào");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi tải lịch sử giải đấu: {ex.Message}");
            }
        }

        private async Task DisplayAwardsAndRankingsAsync()
        {
            try
            {
                Console.WriteLine("\n🏆 DANH HIỆU VÀ THÀNH TÍCH");
                Console.WriteLine("─".PadRight(78, '─'));

                // Mock achievements
                await Task.Delay(100); // Small delay to make it async
                
                var mockAchievements = new[]
                {
                    ("Vô địch mùa đầu", "15/06/2024", "Giành chiến thắng giải đấu đầu tiên"),
                    ("Top Player", "20/07/2024", "Vào top 10 bảng xếp hạng"),
                    ("Chiến binh bất bại", "05/08/2024", "Thắng 10 trận liên tiếp")
                };

                Console.WriteLine("Danh hiệu                   | Ngày đạt được | Mô tả");
                Console.WriteLine("─".PadRight(78, '─'));

                foreach (var (title, date, description) in mockAchievements)
                {
                    Console.WriteLine($"{title,-25} | {date} | {description}");
                }

                // Hiển thị điểm nổi bật
                Console.WriteLine("\n🌟 ĐIỂM NỔI BẬT");
                Console.WriteLine("─".PadRight(78, '─'));

                var highlights = new[]
                {
                    "Đã giành chiến thắng trong 3 giải đấu",
                    "Tham gia hơn 10 giải đấu - Player tích cực",
                    "Kiếm được hơn 2 triệu VND tiền thưởng",
                    "Điểm số trung bình cao (>= 8.0)",
                    "Top 15 trong bảng xếp hạng toàn quốc"
                };

                foreach (var highlight in highlights)
                {
                    Console.WriteLine($"• {highlight}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi tải thành tích: {ex.Message}");
            }
        }
    }
}
