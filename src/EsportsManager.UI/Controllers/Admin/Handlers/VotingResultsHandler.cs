using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class VotingResultsHandler : IVotingResultsHandler
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly IVotingService _votingService;

    public VotingResultsHandler(
        IUserService userService,
        ITournamentService tournamentService,
        IVotingService votingService)
    {
        _userService = userService;
        _tournamentService = tournamentService;
        _votingService = votingService;
    }

    public async Task ViewVotingResultsAsync()
    {
        while (true)
        {
            var options = new[]
            {
                "Kết quả voting player",
                "Kết quả voting tournament",
                "Tìm kiếm vote theo user",
                "Thống kê voting",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("KẾT QUẢ VOTING", options);

            switch (selection)
            {
                case 0:
                    await ShowPlayerVotingResultsAsync();
                    break;
                case 1:
                    await ShowTournamentVotingResultsAsync();
                    break;
                case 2:
                    await SearchVotesByUserAsync();
                    break;
                case 3:
                    await ShowVotingStatsAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    public async Task ShowPlayerVotingResultsAsync()
    {
        try
        {
            int borderWidth = 100;
            int borderHeight = 25;
            int maxRows = 15;
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING PLAYER", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Lấy dữ liệu từ service
            var playerResults = await _votingService.GetPlayerVotingResultsAsync();
            if (playerResults == null || !playerResults.Any())
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có dữ liệu voting cho players.".PadRight(width));
                Console.ResetColor();
                Console.SetCursorPosition(left, top + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
                Console.ReadKey(true);
                return;
            }
            // Header
            var header = string.Format("{0,-5}{1,-15}{2,-10}{3,-10}{4,-30}",
                "ID", "Player", "Điểm TB", "Số vote", "Phân bố điểm");
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine(new string('-', Math.Min(70, width)));
            // Data rows
            int displayCount = Math.Min(playerResults.Count, maxRows);
            foreach (var (result, i) in playerResults.OrderByDescending(r => r.AverageRating).Select((r, i) => (r, i)))
            {
                if (i >= maxRows) break;
                string distribution = string.Join(" ", Enumerable.Range(1, 5).Select(star => $"{star}★:{(result.RatingDistribution.ContainsKey(star) ? result.RatingDistribution[star] : 0)}"));
                var row = string.Format("{0,-5}{1,-15}{2,-10:F2}{3,-10}{4,-30}",
                    result.TargetId,
                    result.TargetName.Length > 14 ? result.TargetName.Substring(0, 14) : result.TargetName,
                    result.AverageRating,
                    result.TotalVotes,
                    distribution.Length > 29 ? distribution.Substring(0, 29) : distribution);
                Console.SetCursorPosition(left, top + 2 + i);
                Console.WriteLine(row.Length > width ? row.Substring(0, width) : row.PadRight(width));
            }
            Console.ResetColor();
            // Footer
            int footerY = top + 2 + maxRows;
            string totalInfo = $"Tổng số: {playerResults.Count} players có đánh giá";
            if (totalInfo.Length > width) totalInfo = totalInfo.Substring(0, width);
            Console.SetCursorPosition(left, footerY);
            Console.WriteLine(totalInfo.PadRight(width));
            Console.SetCursorPosition(left, footerY + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải kết quả voting player: {ex.Message}", true, 3000);
        }
    }

    public async Task ShowTournamentVotingResultsAsync()
    {
        try
        {
            int borderWidth = 100;
            int borderHeight = 25;
            int maxRows = 15;
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING TOURNAMENT", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Lấy dữ liệu từ service
            var tournamentResults = await _votingService.GetTournamentVotingResultsAsync();
            if (tournamentResults == null || !tournamentResults.Any())
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có dữ liệu voting cho tournaments.".PadRight(width));
                Console.ResetColor();
                Console.SetCursorPosition(left, top + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
                Console.ReadKey(true);
                return;
            }
            // Header
            var header = string.Format("{0,-5}{1,-25}{2,-10}{3,-10}{4,-30}",
                "ID", "Tournament", "Điểm TB", "Số vote", "Phân bố điểm");
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine(new string('-', Math.Min(80, width)));
            // Data rows
            int displayCount = Math.Min(tournamentResults.Count, maxRows);
            foreach (var (result, i) in tournamentResults.OrderByDescending(r => r.AverageRating).Select((r, i) => (r, i)))
            {
                if (i >= maxRows) break;
                string distribution = string.Join(" ", Enumerable.Range(1, 5).Select(star => $"{star}★:{(result.RatingDistribution.ContainsKey(star) ? result.RatingDistribution[star] : 0)}"));
                var row = string.Format("{0,-5}{1,-25}{2,-10:F2}{3,-10}{4,-30}",
                    result.TargetId,
                    result.TargetName.Length > 24 ? result.TargetName.Substring(0, 24) : result.TargetName,
                    result.AverageRating,
                    result.TotalVotes,
                    distribution.Length > 29 ? distribution.Substring(0, 29) : distribution);
                Console.SetCursorPosition(left, top + 2 + i);
                Console.WriteLine(row.Length > width ? row.Substring(0, width) : row.PadRight(width));
            }
            Console.ResetColor();
            // Footer
            int footerY = top + 2 + maxRows;
            string totalInfo = $"Tổng số: {tournamentResults.Count} tournaments có đánh giá";
            if (totalInfo.Length > width) totalInfo = totalInfo.Substring(0, width);
            Console.SetCursorPosition(left, footerY);
            Console.WriteLine(totalInfo.PadRight(width));
            Console.SetCursorPosition(left, footerY + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải kết quả voting tournament: {ex.Message}", true, 3000);
        }
    }

    public async Task SearchVotesByUserAsync()
    {
        try
        {
            bool searching = true;

            while (searching)
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("TÌM KIẾM VOTE THEO USER", 100, 25);

                Console.WriteLine("🔍 TÌM KIẾM VOTE THEO USER:");
                Console.WriteLine("--------------------------");

                // Nhập thông tin tìm kiếm
                Console.Write("Tên người dùng (để trống để bỏ qua): ");
                string username = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Tìm kiếm đã hủy!");
                    break;
                }

                Console.Write("Loại vote (Player/Tournament, để trống để tất cả): ");
                string voteType = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.Write("Từ ngày (yyyy-MM-dd, để trống để bỏ qua): ");
                string fromDateStr = Console.ReadLine()?.Trim() ?? string.Empty;
                DateTime? fromDate = !string.IsNullOrEmpty(fromDateStr) && DateTime.TryParse(fromDateStr, out var date)
                    ? date
                    : null;

                Console.Write("Đến ngày (yyyy-MM-dd, để trống để bỏ qua): ");
                string toDateStr = Console.ReadLine()?.Trim() ?? string.Empty;
                DateTime? toDate = !string.IsNullOrEmpty(toDateStr) && DateTime.TryParse(toDateStr, out var date2)
                    ? date2
                    : null;

                // Tạo đối tượng tìm kiếm
                var searchDto = new VotingSearchDto
                {
                    Username = username,
                    VoteType = string.IsNullOrEmpty(voteType) ? null : voteType,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Page = 1,
                    PageSize = 20
                };

                // Thực hiện tìm kiếm
                var results = await _votingService.SearchVotesAsync(searchDto);

                Console.WriteLine("\nKẾT QUẢ TÌM KIẾM:");
                Console.WriteLine("-----------------");

                if (results == null || !results.Any())
                {
                    Console.WriteLine("Không tìm thấy kết quả nào phù hợp.");
                }
                else
                {
                    // Header
                    Console.WriteLine($"{"Vote ID",-8}{"User",-15}{"Loại",-12}{"Đối tượng",-20}{"Điểm",-8}{"Ngày vote",-12}{"Bình luận",-30}");
                    Console.WriteLine(new string('-', 95));

                    // Data
                    foreach (var vote in results)
                    {
                        Console.WriteLine($"{vote.VotingId,-8}{vote.Username,-15}{vote.VoteType,-12}{vote.TargetName,-20}{vote.Rating,-8}{vote.VoteDate:yyyy-MM-dd,-12}{vote.Comment.Substring(0, Math.Min(30, vote.Comment.Length)),-30}");
                    }

                    Console.WriteLine($"\nTìm thấy {results.Count} kết quả");
                }

                Console.WriteLine("\nBạn có muốn tìm kiếm tiếp? (Y/N): ");
                var key = Console.ReadKey(true);
                searching = (key.Key == ConsoleKey.Y);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm vote: {ex.Message}", true, 3000);
        }
    }

    public async Task ShowVotingStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ VOTING", 100, 30);

            Console.WriteLine("📊 THỐNG KÊ VOTING:");
            Console.WriteLine("-----------------");

            // Lấy dữ liệu thống kê
            var stats = await _votingService.GetVotingStatsAsync();

            // Hiển thị các chỉ số tổng quan
            Console.WriteLine($"Tổng số votes: {stats.TotalVotes}");
            Console.WriteLine($"Votes cho players: {stats.TotalPlayerVotes} ({(double)stats.TotalPlayerVotes / stats.TotalVotes:P1})");
            Console.WriteLine($"Votes cho tournaments: {stats.TotalTournamentVotes} ({(double)stats.TotalTournamentVotes / stats.TotalVotes:P1})");
            Console.WriteLine($"Số người tham gia đánh giá: {stats.UniqueVoters}");

            // Hiển thị thống kê theo tháng
            Console.WriteLine("\nPHÂN BỐ THEO THÁNG:");
            Console.WriteLine("------------------");

            if (stats.VotesByMonth.Any())
            {
                int maxValue = stats.VotesByMonth.Values.Max();
                int barWidth = 40;

                foreach (var entry in stats.VotesByMonth.OrderBy(e => e.Key))
                {
                    int month = int.Parse(entry.Key.Split('-')[1]);
                    int year = int.Parse(entry.Key.Split('-')[0]);
                    string monthName = new DateTime(year, month, 1).ToString("MMM yyyy");
                    int count = entry.Value;

                    // Tính toán chiều dài thanh biểu đồ
                    int barLength = (int)Math.Round((double)count / maxValue * barWidth);
                    string bar = new string('█', barLength);

                    Console.WriteLine($"{monthName,-10}: {bar,-40} {count,4}");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu thống kê theo tháng");
            }

            // Hiển thị Top 5 players
            Console.WriteLine("\nTOP 5 PLAYERS:");
            Console.WriteLine("-------------");

            if (stats.TopPlayers.Any())
            {
                foreach (var player in stats.TopPlayers)
                {
                    string stars = new string('★', (int)Math.Round(player.AverageRating));
                    Console.WriteLine($"{player.TargetName,-15}: {stars,-5} ({player.AverageRating:F1}) - {player.TotalVotes} votes");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu");
            }

            // Hiển thị Top 5 tournaments
            Console.WriteLine("\nTOP 5 TOURNAMENTS:");
            Console.WriteLine("-----------------");

            if (stats.TopTournaments.Any())
            {
                foreach (var tournament in stats.TopTournaments)
                {
                    string stars = new string('★', (int)Math.Round(tournament.AverageRating));
                    Console.WriteLine($"{tournament.TargetName,-20}: {stars,-5} ({tournament.AverageRating:F1}) - {tournament.TotalVotes} votes");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê voting: {ex.Message}", true, 3000);
        }
    }

    #region Interface Implementation

    public async Task HandlePlayerVotingResultsAsync()
    {
        await ShowPlayerVotingResultsAsync();
    }

    public async Task HandleTournamentVotingResultsAsync()
    {
        await ShowTournamentVotingResultsAsync();
    }

    public async Task HandleVotingSearchAsync()
    {
        await SearchVotesByUserAsync();
    }

    public async Task HandleVotingStatisticsAsync()
    {
        await ShowVotingStatsAsync();
    }

    #endregion
}
