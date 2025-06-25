using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class VotingResultsHandler
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;

    public VotingResultsHandler(IUserService userService, ITournamentService tournamentService)
    {
        _userService = userService;
        _tournamentService = tournamentService;
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING PLAYER", 80, 20);
            
            Console.WriteLine("🏆 KẾT QUẢ VOTING PLAYER:");
            Console.WriteLine("Chức năng đang được phát triển...");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING TOURNAMENT", 80, 20);
            
            Console.WriteLine("🎖️ KẾT QUẢ VOTING TOURNAMENT:");
            Console.WriteLine("Chức năng đang được phát triển...");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TÌM KIẾM VOTE THEO USER", 80, 20);
            
            Console.WriteLine("🔍 TÌM KIẾM VOTE THEO USER:");
            Console.WriteLine("Chức năng đang được phát triển...");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
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
            ConsoleRenderingService.DrawBorder("THỐNG KÊ VOTING", 80, 20);
            
            Console.WriteLine("📊 THỐNG KÊ VOTING:");
            Console.WriteLine("Chức năng đang được phát triển...");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê voting: {ex.Message}", true, 3000);
        }
    }
}
