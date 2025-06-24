using System;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Controllers;
using EsportsManager.UI.MenuServices;

namespace EsportsManager.UI.Services;

/// <summary>
/// ServiceManager - Tích hợp UI và BL layers
/// Đảm bảo UI có thể truy cập vào business logic một cách clean
/// </summary>
public class ServiceManager
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public ServiceManager(IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
    }    /// <summary>
         /// Tạo AdminUIController và AdminMenuService
         /// </summary>
    public AdminMenuService CreateAdminMenuService(UserProfileDto adminUser)
    {
        var adminController = new AdminUIController(adminUser, _userService, _tournamentService, _teamService);
        return new AdminMenuService(adminController);
    }

    /// <summary>
    /// Tạo PlayerController và PlayerMenuService  
    /// </summary>
    public PlayerMenuService CreatePlayerMenuService(UserProfileDto playerUser)
    {
        var playerController = new PlayerController(playerUser, _userService, _tournamentService, _teamService);
        return new PlayerMenuService(playerController);
    }

    /// <summary>
    /// Tạo ViewerController và ViewerMenuService
    /// </summary>
    public ViewerMenuService CreateViewerMenuService(UserProfileDto viewerUser)
    {
        var viewerController = new ViewerController(viewerUser, _userService, _tournamentService);
        return new ViewerMenuService(viewerController);
    }
}
