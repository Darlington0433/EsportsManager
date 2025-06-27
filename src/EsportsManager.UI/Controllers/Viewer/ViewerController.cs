// Controller xử lý chức năng Viewer - Refactored using Handler Pattern
// Áp dụng SOLID principles và Single Responsibility Principle

using System;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Base;
using EsportsManager.UI.Controllers.Viewer.Handlers;

namespace EsportsManager.UI.Controllers.Viewer;

/// <summary>
/// Viewer Controller - Refactored to follow SOLID principles
/// Uses handler pattern to delegate specific responsibilities
/// </summary>
public class ViewerController : BaseController
{
    private readonly ViewerTournamentHandler _tournamentHandler;
    private readonly ViewerVotingHandler _votingHandler;
    private readonly ViewerDonationHandler _donationHandler;
    private readonly ViewerProfileHandler _profileHandler;
    private readonly ViewerWalletHandler _walletHandler;

    public ViewerController(
        UserProfileDto currentUser,
        ViewerTournamentHandler tournamentHandler,
        ViewerVotingHandler votingHandler,
        ViewerDonationHandler donationHandler,
        ViewerProfileHandler profileHandler,
        ViewerWalletHandler walletHandler) : base(currentUser)
    {
        _tournamentHandler = tournamentHandler;
        _votingHandler = votingHandler;
        _donationHandler = donationHandler;
        _profileHandler = profileHandler;
        _walletHandler = walletHandler;
    }

    /// <summary>
    /// Implements abstract DisplayMenu method from BaseController
    /// </summary>
    protected override void DisplayMenu()
    {
        ShowViewerMenu();
    }

    /// <summary>
    /// Main menu for Viewer role
    /// Delegates all functionality to specialized handlers
    /// </summary>
    public void ShowViewerMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Xem danh sách giải đấu",
                "Xem bảng xếp hạng giải đấu", 
                "Donate cho Player",
                "Vote (Player/Tournament/Sport)",
                "Quản lý ví điện tử (Nạp tiền)",
                "Xem thông tin cá nhân",
                "Cập nhật thông tin cá nhân",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU VIEWER - {_currentUser.Username}", menuOptions);
            
            switch (selection)
            {
                case 0:
                    _tournamentHandler.HandleViewTournamentListAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    _tournamentHandler.HandleViewTournamentStandingsAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    _donationHandler.HandleDonateToPlayerAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    _votingHandler.HandleVotingAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    _walletHandler.HandleWalletManagementAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    _profileHandler.HandleViewProfileAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    _profileHandler.HandleUpdateProfileAsync().GetAwaiter().GetResult();
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
