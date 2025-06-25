using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

namespace EsportsManager.UI.Controllers.Viewer.Handlers
{
    /// <summary>
    /// Handler cho xem thông tin giải đấu của Viewer
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class ViewerTournamentHandler : IViewerTournamentHandler
    {
        private readonly ITournamentService _tournamentService;

        public ViewerTournamentHandler(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        public async Task HandleViewTournamentListAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 80, 20);

                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào!", false, 2000);
                    return;
                }

                Console.WriteLine("🏆 Danh sách tất cả giải đấu:");
                Console.WriteLine("─".PadRight(78, '─'));
                Console.WriteLine("STT | Tên giải đấu               | Trạng thái    | Phí tham gia");
                Console.WriteLine("─".PadRight(78, '─'));

                for (int i = 0; i < tournaments.Count; i++)
                {
                    var tournament = tournaments[i];
                    Console.WriteLine($"{i + 1,3} | {tournament.Name,-25} | {tournament.Status,-12} | {tournament.EntryFee,12:N0} VND");
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        public async Task HandleViewTournamentStandingsAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("BẢNG XẾP HẠNG GIẢI ĐẤU", 80, 20);

                // Lấy danh sách giải đấu để chọn
                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào!", false, 2000);
                    return;
                }

                Console.WriteLine("🏆 Chọn giải đấu để xem bảng xếp hạng:");
                for (int i = 0; i < tournaments.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tournaments[i].Name}");
                }

                Console.Write($"\nNhập số thứ tự giải đấu (1-{tournaments.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
                {
                    var selectedTournament = tournaments[choice - 1];
                    await DisplayTournamentStandingsAsync(selectedTournament.TournamentId);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task DisplayTournamentStandingsAsync(int tournamentId)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("BẢNG XẾP HẠNG", 80, 15);

                var standings = await _tournamentService.GetTournamentLeaderboardAsync(tournamentId);

                if (standings != null && standings.Count > 0)
                {
                    Console.WriteLine("📊 Bảng xếp hạng hiện tại:");
                    Console.WriteLine("─".PadRight(78, '─'));
                    Console.WriteLine("Hạng | Tên đội               | Vị trí | Tiền thưởng | Thành viên");
                    Console.WriteLine("─".PadRight(78, '─'));

                    for (int i = 0; i < standings.Count; i++)
                    {
                        var team = standings[i];
                        string rank = GetRankIcon(team.Rank);
                        Console.WriteLine($" {rank}   | {team.TeamName,-20} | {team.Position,6} | {team.PrizeMoney,11:C} | {team.TeamSize,9}");
                    }
                }
                else
                {
                    Console.WriteLine("📝 Chưa có dữ liệu xếp hạng cho giải đấu này");
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private string GetRankIcon(int rank)
        {
            return rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => $"{rank,2}"
            };
        }
    }
}
