// Controller xử lý chức năng Player - Enterprise Architecture  
// Áp dụng SOLID principles và Handler Pattern

using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Base;
using EsportsManager.UI.Controllers.Player.Handlers;
using EsportsManager.UI.Controllers.Shared;
using EsportsManager.UI.Controllers.Shared.Handlers;

namespace EsportsManager.UI.Controllers.Player;

/// <summary>
/// Player Controller - Enterprise-grade implementation
/// Uses Handler Pattern for single responsibility
/// </summary>
public class PlayerController : BaseController
{
    private readonly TournamentManagementHandler _tournamentManagementHandler;
    private readonly PlayerTeamManagementHandler _teamManagementHandler;
    private readonly PlayerProfileHandler _profileHandler;
    private readonly TournamentViewHandler _tournamentViewHandler;
    private readonly PlayerFeedbackHandler _feedbackHandler;
    private readonly PlayerWalletHandler _walletHandler;
    private readonly PlayerAchievementHandler _achievementHandler;

    public PlayerController(
        UserProfileDto currentUser,
        TournamentManagementHandler tournamentManagementHandler,
        PlayerTeamManagementHandler teamManagementHandler,
        PlayerProfileHandler profileHandler,
        TournamentViewHandler tournamentViewHandler,
        PlayerFeedbackHandler feedbackHandler,
        PlayerWalletHandler walletHandler,
        PlayerAchievementHandler achievementHandler) : base(currentUser)
    {
        _tournamentManagementHandler = tournamentManagementHandler;
        _teamManagementHandler = teamManagementHandler;
        _profileHandler = profileHandler;
        _tournamentViewHandler = tournamentViewHandler;
        _feedbackHandler = feedbackHandler;
        _walletHandler = walletHandler;
        _achievementHandler = achievementHandler;
    }

    /// <summary>
    /// Implements abstract DisplayMenu method from BaseController
    /// </summary>
    protected override void DisplayMenu()
    {
        ShowPlayerMenuAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Main menu for Player role
    /// Delegates all functionality to specialized handlers
    /// </summary>
    public async Task ShowPlayerMenuAsync()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Quản lý team (Tạo/Tham gia/Rời)",
                "Quản lý giải đấu",
                "Xem danh sách giải đấu",
                "Quản lý ví điện tử (Rút tiền)",
                "Gửi feedback giải đấu",
                "Xem thành tích cá nhân",
                "Cập nhật thông tin cá nhân",
                "Đổi mật khẩu",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU PLAYER - {_currentUser.Username}", menuOptions);

            switch (selection)
            {
                case 0:
                    await _teamManagementHandler.HandleTeamManagementAsync();
                    break;
                case 1:
                    await _tournamentManagementHandler.HandleTournamentManagementAsync();
                    break;
                case 2:
                    await _tournamentViewHandler.HandleViewTournamentListAsync();
                    break;
                case 3:
                    await _walletHandler.HandleWalletManagementAsync();
                    break;
                case 4:
                    await _feedbackHandler.HandleSubmitFeedbackAsync();
                    break;
                case 5:
                    await _achievementHandler.HandleViewAchievementsAsync();
                    break;
                case 6:
                    await _profileHandler.HandleUpdatePersonalInfoAsync();
                    break;
                case 7:
                    await _profileHandler.HandleChangePasswordAsync();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }
}
