// Controller xử lý chức năng Player - Enterprise Architecture  
// Áp dụng SOLID principles và Handler Pattern

using System;
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
    private readonly TournamentRegistrationHandler _tournamentRegistrationHandler;
    private readonly PlayerTeamManagementHandler _teamManagementHandler;
    private readonly PlayerProfileHandler _profileHandler;
    private readonly TournamentViewHandler _tournamentViewHandler;
    private readonly PlayerFeedbackHandler _feedbackHandler;
    private readonly PlayerWalletHandler _walletHandler;
    private readonly PlayerAchievementHandler _achievementHandler;

    public PlayerController(
        UserProfileDto currentUser,
        TournamentRegistrationHandler tournamentRegistrationHandler,
        PlayerTeamManagementHandler teamManagementHandler,
        PlayerProfileHandler profileHandler,
        TournamentViewHandler tournamentViewHandler,
        PlayerFeedbackHandler feedbackHandler,
        PlayerWalletHandler walletHandler,
        PlayerAchievementHandler achievementHandler) : base(currentUser)
    {
        _tournamentRegistrationHandler = tournamentRegistrationHandler;
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
        ShowPlayerMenu();
    }

    /// <summary>
    /// Main menu for Player role
    /// Delegates all functionality to specialized handlers
    /// </summary>
    public void ShowPlayerMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Quản lý team (Tạo/Tham gia/Rời)",
                "Đăng ký tham gia giải đấu", 
                "Xem danh sách giải đấu",
                "Quản lý ví điện tử (Rút tiền)",
                "Gửi feedback giải đấu",
                "Xem thành tích cá nhân",
                "Cập nhật thông tin cá nhân",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU PLAYER - {_currentUser.Username}", menuOptions);
            
            switch (selection)
            {
                case 0:
                    _teamManagementHandler.HandleTeamManagementAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    _tournamentRegistrationHandler.HandleTournamentRegistrationAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    _tournamentViewHandler.HandleViewTournamentListAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    _walletHandler.HandleWalletManagementAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    _feedbackHandler.HandleSubmitFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    _achievementHandler.HandleViewAchievementsAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    _profileHandler.HandleUpdatePersonalInfoAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                case -1:
                    return; // Đăng xuất
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }
}
