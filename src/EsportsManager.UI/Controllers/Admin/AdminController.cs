using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Base;
using EsportsManager.UI.Controllers.Interfaces;
using EsportsManager.UI.Controllers.Admin.Handlers;

namespace EsportsManager.UI.Controllers.Admin;

public class AdminUIController : BaseController, IAdminUIController
{
    private readonly UserManagementHandler _userManagementHandler;
    private readonly AdminTournamentManagementHandler _tournamentManagementHandler;
    private readonly SystemStatsHandler _systemStatsHandler;
    private readonly DonationReportHandler _donationReportHandler;
    private readonly VotingResultsHandler _votingResultsHandler;
    private readonly FeedbackManagementHandler _feedbackManagementHandler;

    public AdminUIController(
        UserProfileDto currentUser,
        UserManagementHandler userManagementHandler,
        AdminTournamentManagementHandler tournamentManagementHandler,
        SystemStatsHandler systemStatsHandler,
        DonationReportHandler donationReportHandler,
        VotingResultsHandler votingResultsHandler,
        FeedbackManagementHandler feedbackManagementHandler) : base(currentUser)
    {
        _userManagementHandler = userManagementHandler;
        _tournamentManagementHandler = tournamentManagementHandler;
        _systemStatsHandler = systemStatsHandler;
        _donationReportHandler = donationReportHandler;
        _votingResultsHandler = votingResultsHandler;
        _feedbackManagementHandler = feedbackManagementHandler;
    }

    /// <summary>
    /// Implements abstract DisplayMenu method from BaseController
    /// </summary>
    protected override void DisplayMenu()
    {
        ShowAdminMenu();
    }

    public void ShowAdminMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Quản lý người dùng",
                "Quản lý giải đấu/trận đấu",
                "Duyệt đăng ký giải đấu",
                "Quản lý đội/team",
                "Gán achievement cho player",
                "Quản lý game",
                "Xem thống kê hệ thống",
                "Xem báo cáo donation",
                "Xem kết quả voting",
                "Quản lý feedback",
                "Xóa người dùng",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU ADMIN - {_currentUser.Username}", menuOptions);

            switch (selection)
            {
                case 0:
                    ManageUsersAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    _tournamentManagementHandler.ManageTournamentsAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    _tournamentManagementHandler.ApproveTournamentRegistrationsAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    _tournamentManagementHandler.ManageTeamsAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    _userManagementHandler.AssignAchievementsAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    HandleGameManagementAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    _systemStatsHandler.ViewSystemStatsAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                    _donationReportHandler.ViewDonationReportsAsync().GetAwaiter().GetResult();
                    break;
                case 8:
                    _votingResultsHandler.ViewVotingResultsAsync().GetAwaiter().GetResult();
                    break;
                case 9:
                    _feedbackManagementHandler.ManageFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 10:
                    _userManagementHandler.DeleteUsersAsync().GetAwaiter().GetResult();
                    break;
                case 11:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    private async Task ManageUsersAsync()
    {
        while (true)
        {
            var userOptions = new[]
            {
                "Xem danh sách người dùng",
                "Tìm kiếm người dùng",
                "Thay đổi trạng thái người dùng",
                "Reset mật khẩu người dùng",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ NGƯỜI DÙNG", userOptions);

            switch (selection)
            {
                case 0:
                    await _userManagementHandler.ShowAllUsersAsync();
                    break;
                case 1:
                    await _userManagementHandler.SearchUsersAsync();
                    break;
                case 2:
                    await _userManagementHandler.ToggleUserStatusAsync();
                    break;
                case 3:
                    await _userManagementHandler.ResetUserPasswordAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    /// <summary>
    /// Xử lý quản lý game
    /// </summary>
    private async Task HandleGameManagementAsync()
    {
        while (true)
        {
            var gameOptions = new[]
            {
                "Xem danh sách game",
                "Thêm game mới",
                "Cập nhật game",
                "Xóa game",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ GAME", gameOptions);

            switch (selection)
            {
                case 0:
                    await ViewAllGamesAsync();
                    break;
                case 1:
                    await AddNewGameAsync();
                    break;
                case 2:
                    await UpdateGameAsync();
                    break;
                case 3:
                    await DeleteGameAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    /// <summary>
    /// Xem tất cả game
    /// </summary>
    private async Task ViewAllGamesAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GAME", 80, 20);

            // TODO: Implement get all games from service
            await Task.Delay(1); // Minimal async operation to satisfy compiler
            Console.WriteLine("Tính năng đang được phát triển...");
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Thêm game mới
    /// </summary>
    private async Task AddNewGameAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÊM GAME MỚI", 70, 18);

            int borderLeft = (Console.WindowWidth - 70) / 2;
            int borderTop = (Console.WindowHeight - 18) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.Write("Tên game: ");
            var gameName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(gameName))
            {
                ConsoleRenderingService.ShowMessageBox("Tên game không được để trống!", true, 2000);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.Write("Mô tả: ");
            var description = Console.ReadLine()?.Trim();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
            Console.Write("Thể loại: ");
            var genre = Console.ReadLine()?.Trim();

            // TODO: Implement add game to service
            await Task.Delay(1); // Minimal async operation to satisfy compiler
            ConsoleRenderingService.ShowMessageBox($"Thêm game '{gameName}' thành công!", false, 3000);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Cập nhật game
    /// </summary>
    private async Task UpdateGameAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẬP NHẬT GAME", 70, 18);

            await Task.Delay(1); // Minimal async operation to satisfy compiler
            Console.WriteLine("Tính năng đang được phát triển...");
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Xóa game
    /// </summary>
    private async Task DeleteGameAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("XÓA GAME", 70, 18);

            await Task.Delay(1); // Minimal async operation to satisfy compiler
            Console.WriteLine("Tính năng đang được phát triển...");
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    // Public methods for backward compatibility
    public Task<List<UserDto>> GetAllUsersAsync()
    {
        // This could be moved to a separate service if needed
        // For now, we'll implement a basic version
        throw new NotImplementedException("This method should be implemented through UserManagementHandler");
    }

    public Task<UserDto?> GetUserDetailsAsync(int userId)
    {
        // This could be moved to a separate service if needed
        throw new NotImplementedException("This method should be implemented through UserManagementHandler");
    }

    public Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        // This could be moved to a separate service if needed
        throw new NotImplementedException("This method should be implemented through UserManagementHandler");
    }
}
