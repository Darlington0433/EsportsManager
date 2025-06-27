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
                    // Set cursor vào giữa border để hiển thị thông báo
                    int centerX = (Console.WindowWidth - 30) / 2;
                    int centerY = Console.WindowHeight / 2;
                    Console.SetCursorPosition(centerX, centerY);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Hiện tại không có giải đấu nào!");
                    Console.ResetColor();
                    Console.SetCursorPosition(centerX - 10, centerY + 2);
                    Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey(true);
                    return;
                }

                // Tính vị trí để hiển thị data bên trong border
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;

                // Header
                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-25} {"Trạng thái",-12} {"Phí tham gia",-15} {"Ngày bắt đầu",-12}");
                
                // Separator line
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine(new string('─', 70));

                // Data rows
                int currentRow = borderTop + 4;
                int maxRows = 12; // Giới hạn số dòng hiển thị để vừa trong border
                int displayedRows = 0;

                for (int i = 0; i < tournaments.Count && displayedRows < maxRows; i++)
                {
                    var tournament = tournaments[i];
                    Console.SetCursorPosition(borderLeft + 2, currentRow);
                    
                    // Set color based on tournament status
                    Console.ForegroundColor = tournament.Status switch
                    {
                        "Active" => ConsoleColor.Green,
                        "Completed" => ConsoleColor.Blue,
                        "Draft" => ConsoleColor.Yellow,
                        _ => ConsoleColor.Gray
                    };

                    var row = string.Format("{0,-5} {1,-25} {2,-12} {3,-15} {4,-12}",
                        i + 1,
                        tournament.TournamentName.Length > 24 ? tournament.TournamentName.Substring(0, 24) : tournament.TournamentName,
                        tournament.Status,
                        $"{tournament.EntryFee:N0} VND",
                        tournament.StartDate.ToString("dd/MM/yyyy"));

                    Console.WriteLine(row);
                    currentRow++;
                    displayedRows++;
                }

                // Nếu có nhiều dữ liệu hơn, hiển thị thông báo
                if (tournaments.Count > maxRows)
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow + 1);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"... và {tournaments.Count - maxRows} giải đấu khác");
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                Console.WriteLine($"Tổng cộng: {tournaments.Count} giải đấu");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 3000);
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
                    // Set cursor vào giữa border để hiển thị thông báo
                    int centerX = (Console.WindowWidth - 30) / 2;
                    int centerY = Console.WindowHeight / 2;
                    Console.SetCursorPosition(centerX, centerY);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Hiện tại không có giải đấu nào!");
                    Console.ResetColor();
                    Console.SetCursorPosition(centerX - 10, centerY + 2);
                    Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey(true);
                    return;
                }

                // Tính vị trí để hiển thị data bên trong border
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("🏆 Chọn giải đấu để xem bảng xếp hạng:");

                int currentRow = borderTop + 4;
                for (int i = 0; i < tournaments.Count && i < 10; i++)
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{i + 1}. {tournaments[i].TournamentName}");
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                Console.Write($"Nhập số thứ tự giải đấu (1-{tournaments.Count}): ");
                
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
                {
                    var selectedTournament = tournaments[choice - 1];
                    await DisplayTournamentStandingsAsync(selectedTournament.TournamentId);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 1500);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 2000);
            }
        }

        private async Task DisplayTournamentStandingsAsync(int tournamentId)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("BẢNG XẾP HẠNG", 80, 20);

                var standings = await _tournamentService.GetTournamentLeaderboardAsync(tournamentId);

                // Tính vị trí để hiển thị data bên trong border
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;

                if (standings != null && standings.Count > 0)
                {
                    // Header
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{"Hạng",-6} {"Tên đội",-20} {"Vị trí",-8} {"Tiền thưởng",-15} {"Thành viên",-10}");
                    
                    // Separator line
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                    Console.WriteLine(new string('─', 65));

                    // Data rows
                    int currentRow = borderTop + 4;
                    int maxRows = 12;
                    int displayedRows = 0;

                    for (int i = 0; i < standings.Count && displayedRows < maxRows; i++)
                    {
                        var team = standings[i];
                        Console.SetCursorPosition(borderLeft + 2, currentRow);
                        
                        // Set color based on rank
                        Console.ForegroundColor = team.Rank switch
                        {
                            1 => ConsoleColor.Yellow,
                            2 => ConsoleColor.Gray,
                            3 => ConsoleColor.DarkYellow,
                            _ => ConsoleColor.Green
                        };

                        string rank = GetRankIcon(team.Rank);
                        var row = string.Format("{0,-6} {1,-20} {2,-8} {3,-15} {4,-10}",
                            rank,
                            team.TeamName.Length > 19 ? team.TeamName.Substring(0, 19) : team.TeamName,
                            team.Position,
                            $"{team.PrizeMoney:N0} VND",
                            team.TeamSize);

                        Console.WriteLine(row);
                        currentRow++;
                        displayedRows++;
                    }

                    Console.ResetColor();
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                    Console.WriteLine($"Tổng cộng: {standings.Count} đội thi đấu");
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 8);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("📝 Chưa có dữ liệu xếp hạng cho giải đấu này");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 2000);
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
