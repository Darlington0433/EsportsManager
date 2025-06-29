using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;
using EsportsManager.UI.Utilities;

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
                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count == 0)
                {
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 80, 20);
                    
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

                // DisplayTournamentsTableInBorder sẽ tự vẽ border với kích thước phù hợp
                DisplayTournamentsTableInBorder(tournaments, 0, 0, 0);
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

        private void DisplayTournamentsTableInBorder(IEnumerable<TournamentInfoDto> tournaments, int startX, int startY, int maxWidth)
        {
            Console.Clear();
            
            // Giảm số lượng cột hiển thị để tránh tràn ra ngoài border
            var headers = new[] { "ID", "Tên giải đấu", "Trạng thái", "Phí tham gia", "Ngày bắt đầu", "Ngày kết thúc", "Tổng thưởng" };
            
            // Tính toán kích thước border phù hợp với console
            int windowWidth = Console.WindowWidth;
            int borderWidth = Math.Min(windowWidth - 4, 110); // Đảm bảo border không vượt quá chiều rộng console
            
            // Tính toán độ rộng các cột
            int usableWidth = borderWidth - 10; // Tăng padding để đảm bảo không tràn ra ngoài
            int numCols = headers.Length;
            int numSeparators = numCols - 1;
            int separatorWidth = 3; // " | "
            int totalSeparator = numSeparators * separatorWidth;
            int totalColWidth = usableWidth - totalSeparator;
            
            // Phân bổ độ rộng cho các cột một cách hợp lý
            int[] colWidths = { 3, 20, 10, 10, 10, 10, 10 }; // Giảm độ rộng các cột
            
            // Đảm bảo tổng độ rộng không vượt quá không gian có sẵn
            int sumCol = colWidths.Sum();
            if (sumCol > totalColWidth)
            {
                // Giảm độ rộng các cột theo tỷ lệ
                double ratio = (double)totalColWidth / sumCol;
                for (int i = 0; i < colWidths.Length; i++)
                {
                    colWidths[i] = Math.Max(3, (int)(colWidths[i] * ratio));
                }
            }
            
            // Tạo dữ liệu hàng với đầy đủ thông tin
            var rows = tournaments.Select(t => new[] {
                t.TournamentId.ToString(),
                t.TournamentName.Length > colWidths[1] - 1 ? t.TournamentName.Substring(0, colWidths[1] - 3) + "..." : t.TournamentName,
                t.Status.Length > colWidths[2] - 1 ? t.Status.Substring(0, colWidths[2] - 3) + "..." : t.Status,
                t.EntryFee.ToString("N0"),
                t.StartDate.ToString("dd/MM/yyyy"),
                t.EndDate.ToString("dd/MM/yyyy"),
                t.PrizePool.ToString("N0")
            }).ToList();
            
            // Tính toán chiều cao border
            int borderHeight = Math.Min(rows.Count + 8, 20); // Đảm bảo đủ chỗ cho nội dung và không quá cao
            
            // Vẽ border
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", borderWidth, borderHeight);
            
            // Tính vị trí để căn giữa
            int borderLeft = (windowWidth - borderWidth) / 2;
            int borderTop = (Console.WindowHeight - borderHeight) / 4;
            
            // Hiển thị bảng
            UIHelper.PrintTableInBorder(headers, rows, borderWidth, borderHeight, borderLeft, borderTop, colWidths);
            
            // Hiển thị thông tin tổng số giải đấu
            int infoY = borderTop + Math.Min(rows.Count + 4, borderHeight - 4);
            UIHelper.PrintPromptInBorder($"Tổng cộng: {tournaments.Count()} giải đấu", borderLeft, infoY, borderWidth - 10);
            
            // Hiển thị thông báo nhấn phím
            Console.SetCursorPosition(borderLeft + 2, borderTop + borderHeight - 2);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }
}
