using System;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.MenuServices;

namespace EsportsManager.UI.Services;

/// <summary>
/// ServiceManager - Tích hợp UI và BL layers
/// Đảm bảo UI có thể truy cập vào business logic một cách clean
/// </summary>
public class ServiceManager
{
    private readonly IUserService _userService;

    public ServiceManager(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }    /// <summary>
    /// Tạo AdminController và AdminMenuService
    /// </summary>
    public AdminMenuService CreateAdminMenuService(UserProfileDto adminUser)
    {
        // Temporary implementation - just use simplified constructor
        var adminController = new AdminController(_userService, adminUser);
        return new AdminMenuService(adminController);
    }

    /// <summary>
    /// Tạo PlayerController và PlayerMenuService  
    /// </summary>
    public PlayerMenuService CreatePlayerMenuService(UserProfileDto playerUser)
    {
        var playerController = new PlayerController(_userService, playerUser);
        return new PlayerMenuService(playerController);
    }

    /// <summary>
    /// Tạo ViewerController và ViewerMenuService
    /// </summary>
    public ViewerMenuService CreateViewerMenuService(UserProfileDto viewerUser)
    {
        var viewerController = new ViewerController(_userService, viewerUser);
        return new ViewerMenuService(viewerController);
    }
}
