using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class SystemStatsHandler
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public SystemStatsHandler(IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
    }

    public async Task ViewSystemStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", 80, 20);

            // Get basic system statistics
            var users = await _userService.GetAllUsersAsync();
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var teams = await _teamService.GetAllTeamsAsync();

            Console.WriteLine("📊 TỔNG QUAN HỆ THỐNG:");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine($"👥 Tổng số người dùng: {users.Count()}");
            Console.WriteLine($"🏆 Tổng số giải đấu: {tournaments.Count()}");
            Console.WriteLine($"⚔️ Tổng số đội: {teams.Count()}");
            Console.WriteLine($"🎮 Giải đấu đang hoạt động: {tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration")}");
            Console.WriteLine($"✅ Giải đấu đã hoàn thành: {tournaments.Count(t => t.Status == "Completed")}");

            Console.WriteLine("\n💰 THỐNG KÊ TÀI CHÍNH:");
            Console.WriteLine(new string('─', 50));
            var totalPrizePool = tournaments.Sum(t => t.PrizePool);
            var totalEntryFees = tournaments.Sum(t => t.EntryFee * t.RegisteredTeams);
            Console.WriteLine($"💎 Tổng giải thưởng: {totalPrizePool:C0} VND");
            Console.WriteLine($"💳 Tổng phí tham gia: {totalEntryFees:C0} VND");

            Console.WriteLine("\n📈 THỐNG KÊ HOẠT ĐỘNG:");
            Console.WriteLine(new string('─', 50));
            var activeUsers = users.Count(u => u.Status == "Active");
            var avgTeamsPerTournament = tournaments.Any() ? tournaments.Average(t => t.RegisteredTeams) : 0;
            Console.WriteLine($"👤 Người dùng hoạt động: {activeUsers}");
            Console.WriteLine($"📊 Trung bình team/giải đấu: {avgTeamsPerTournament:F1}");

            Console.WriteLine("\n📅 HOẠT ĐỘNG GẦN ĐÂY:");
            Console.WriteLine(new string('─', 50));
            var recentTournaments = tournaments.Where(t => t.CreatedAt >= DateTime.Now.AddDays(-7)).Count();
            Console.WriteLine($"🆕 Giải đấu tạo trong 7 ngày: {recentTournaments}");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê hệ thống: {ex.Message}", true, 3000);
        }
    }
}
