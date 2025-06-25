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
    private readonly TournamentManagementHandler _tournamentManagementHandler;
    private readonly SystemStatsHandler _systemStatsHandler;
    private readonly DonationReportHandler _donationReportHandler;
    private readonly VotingResultsHandler _votingResultsHandler;
    private readonly FeedbackManagementHandler _feedbackManagementHandler;
    private readonly SystemSettingsHandler _systemSettingsHandler;

    public AdminUIController(
        UserProfileDto currentUser,
        UserManagementHandler userManagementHandler,
        TournamentManagementHandler tournamentManagementHandler,
        SystemStatsHandler systemStatsHandler,
        DonationReportHandler donationReportHandler,
        VotingResultsHandler votingResultsHandler,
        FeedbackManagementHandler feedbackManagementHandler,
        SystemSettingsHandler systemSettingsHandler) : base(currentUser)
    {
        _userManagementHandler = userManagementHandler;
        _tournamentManagementHandler = tournamentManagementHandler;
        _systemStatsHandler = systemStatsHandler;
        _donationReportHandler = donationReportHandler;
        _votingResultsHandler = votingResultsHandler;
        _feedbackManagementHandler = feedbackManagementHandler;
        _systemSettingsHandler = systemSettingsHandler;
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
                "Xem thống kê hệ thống",
                "Xem báo cáo donation",
                "Xem kết quả voting",
                "Quản lý feedback",
                "Cài đặt hệ thống",
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
                    _systemStatsHandler.ViewSystemStatsAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    _donationReportHandler.ViewDonationReportsAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    _votingResultsHandler.ViewVotingResultsAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    _feedbackManagementHandler.ManageFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    _systemSettingsHandler.SystemSettingsAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                    _userManagementHandler.DeleteUsersAsync().GetAwaiter().GetResult();
                    break;
                case 8:
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
