using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace EsportsManager.UI.Menus;

/// <summary>
/// Main menu - áp dụng Single Responsibility Principle
/// Chỉ lo về việc hiển thị và xử lý main menu
/// </summary>
public class MainMenu
{
    private readonly IUserService _userService;
    private readonly ILogger<MainMenu> _logger;
    
    public MainMenu(IUserService userService, ILogger<MainMenu> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Show main menu
    /// </summary>
    public async Task ShowAsync()
    {
        while (true)
        {
            try
            {
                ConsoleHelper.ShowHeader("Main Menu");
                
                Console.WriteLine("Please select an option:");
                Console.WriteLine();
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. About");
                Console.WriteLine("0. Exit");
                Console.WriteLine();

                var choice = ConsoleInput.GetChoice("Enter your choice", 0, 3);

                switch (choice)
                {
                    case 1:
                        await ShowLoginAsync();
                        break;
                    case 2:
                        await ShowRegisterAsync();
                        break;
                    case 3:
                        ShowAbout();
                        break;
                    case 0:
                        if (ConsoleInput.GetConfirmation("Are you sure you want to exit?"))
                        {
                            ConsoleHelper.ShowInfo("Thank you for using Esports Manager!");
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in main menu");
                ConsoleHelper.ShowError("An error occurred. Please try again.");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    /// <summary>
    /// Show login form
    /// </summary>
    private async Task ShowLoginAsync()
    {
        ConsoleHelper.ShowHeader("Login");

        try
        {
            var username = ConsoleInput.GetUsername("Username");
            var password = ConsoleInput.GetPassword("Password");

            ConsoleHelper.ShowLoading("Authenticating", 1500);

            var loginDto = new EsportsManager.BL.DTOs.LoginDto
            {
                Username = username,
                Password = password
            };

            var result = await _userService.AuthenticateAsync(loginDto);

            if (result.IsAuthenticated)
            {
                ConsoleHelper.ShowSuccess($"Welcome back, {result.Username}!");
                _logger.LogInformation("User logged in: {Username}", result.Username);
                
                // Show role-specific menu based on user role
                await ShowRoleMenuAsync(result.UserId!.Value, result.Username!, result.Role!);
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Login failed");
                _logger.LogWarning("Failed login attempt for username: {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ConsoleHelper.ShowError("An error occurred during login. Please try again.");
        }

        ConsoleHelper.WaitForKey();
    }

    /// <summary>
    /// Show registration form
    /// </summary>
    private async Task ShowRegisterAsync()
    {
        ConsoleHelper.ShowHeader("Register New Account");

        try
        {
            var username = ConsoleInput.GetUsername("Username");
            var email = ConsoleInput.GetEmail("Email (optional)", false);
            var password = ConsoleInput.GetPassword("Password");
            var confirmPassword = ConsoleInput.GetPassword("Confirm Password");

            ConsoleHelper.ShowLoading("Creating account", 1500);

            var registerDto = new EsportsManager.BL.DTOs.RegisterDto
            {
                Username = username,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var result = await _userService.RegisterAsync(registerDto);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Account created successfully!");
                ConsoleHelper.ShowInfo($"Welcome, {result.Data!.Username}! You can now log in with your credentials.");
                _logger.LogInformation("New user registered: {Username}", result.Data.Username);
            }
            else
            {
                if (result.Errors.Any())
                {
                    ConsoleHelper.ShowErrors(result.Errors);
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Registration failed");
                }
                _logger.LogWarning("Failed registration attempt for username: {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            ConsoleHelper.ShowError("An error occurred during registration. Please try again.");
        }

        ConsoleHelper.WaitForKey();
    }

    /// <summary>
    /// Show role-specific menu
    /// </summary>
    private async Task ShowRoleMenuAsync(int userId, string username, string role)
    {
        switch (role.ToLower())
        {
            case "admin":
                await ShowAdminMenuAsync(userId, username);
                break;
            case "player":
                await ShowPlayerMenuAsync(userId, username);
                break;
            case "viewer":
                await ShowViewerMenuAsync(userId, username);
                break;
            default:
                ConsoleHelper.ShowWarning($"Unknown role: {role}. Showing viewer menu.");
                await ShowViewerMenuAsync(userId, username);
                break;
        }
    }

    /// <summary>
    /// Show admin menu
    /// </summary>
    private async Task ShowAdminMenuAsync(int userId, string username)
    {
        while (true)
        {
            try
            {
                ConsoleHelper.ShowHeader($"Admin Panel - Welcome, {username}!");
                
                Console.WriteLine("Admin Options:");
                Console.WriteLine();
                Console.WriteLine("1. User Management");
                Console.WriteLine("2. System Statistics");
                Console.WriteLine("3. Settings");
                Console.WriteLine("4. View Profile");
                Console.WriteLine("5. Change Password");
                Console.WriteLine("0. Logout");
                Console.WriteLine();

                var choice = ConsoleInput.GetChoice("Enter your choice", 0, 5);

                switch (choice)
                {
                    case 1:
                        await ShowUserManagementAsync();
                        break;
                    case 2:
                        await ShowSystemStatisticsAsync();
                        break;
                    case 3:
                        ShowSettings();
                        break;
                    case 4:
                        await ShowUserProfileAsync(userId);
                        break;
                    case 5:
                        await ShowChangePasswordAsync(userId);
                        break;
                    case 0:
                        if (ConsoleInput.GetConfirmation("Are you sure you want to logout?"))
                        {
                            ConsoleHelper.ShowInfo("Logged out successfully!");
                            _logger.LogInformation("User logged out: {Username}", username);
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in admin menu");
                ConsoleHelper.ShowError("An error occurred. Please try again.");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    /// <summary>
    /// Show player menu
    /// </summary>
    private async Task ShowPlayerMenuAsync(int userId, string username)
    {
        while (true)
        {
            try
            {
                ConsoleHelper.ShowHeader($"Player Dashboard - Welcome, {username}!");
                
                Console.WriteLine("Player Options:");
                Console.WriteLine();
                Console.WriteLine("1. View Tournaments");
                Console.WriteLine("2. My Team");
                Console.WriteLine("3. My Achievements");
                Console.WriteLine("4. View Profile");
                Console.WriteLine("5. Change Password");
                Console.WriteLine("0. Logout");
                Console.WriteLine();

                var choice = ConsoleInput.GetChoice("Enter your choice", 0, 5);

                switch (choice)
                {
                    case 1:
                        ShowTournaments();
                        break;
                    case 2:
                        ShowMyTeam();
                        break;
                    case 3:
                        ShowMyAchievements();
                        break;
                    case 4:
                        await ShowUserProfileAsync(userId);
                        break;
                    case 5:
                        await ShowChangePasswordAsync(userId);
                        break;
                    case 0:
                        if (ConsoleInput.GetConfirmation("Are you sure you want to logout?"))
                        {
                            ConsoleHelper.ShowInfo("Logged out successfully!");
                            _logger.LogInformation("User logged out: {Username}", username);
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in player menu");
                ConsoleHelper.ShowError("An error occurred. Please try again.");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    /// <summary>
    /// Show viewer menu
    /// </summary>
    private async Task ShowViewerMenuAsync(int userId, string username)
    {
        while (true)
        {
            try
            {
                ConsoleHelper.ShowHeader($"Viewer Dashboard - Welcome, {username}!");
                
                Console.WriteLine("Viewer Options:");
                Console.WriteLine();
                Console.WriteLine("1. View Tournaments");
                Console.WriteLine("2. View Teams");
                Console.WriteLine("3. Leaderboards");
                Console.WriteLine("4. View Profile");
                Console.WriteLine("5. Change Password");
                Console.WriteLine("0. Logout");
                Console.WriteLine();

                var choice = ConsoleInput.GetChoice("Enter your choice", 0, 5);

                switch (choice)
                {
                    case 1:
                        ShowTournaments();
                        break;
                    case 2:
                        ShowTeams();
                        break;
                    case 3:
                        ShowLeaderboards();
                        break;
                    case 4:
                        await ShowUserProfileAsync(userId);
                        break;
                    case 5:
                        await ShowChangePasswordAsync(userId);
                        break;
                    case 0:
                        if (ConsoleInput.GetConfirmation("Are you sure you want to logout?"))
                        {
                            ConsoleHelper.ShowInfo("Logged out successfully!");
                            _logger.LogInformation("User logged out: {Username}", username);
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in viewer menu");
                ConsoleHelper.ShowError("An error occurred. Please try again.");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    /// <summary>
    /// Show about information
    /// </summary>
    private void ShowAbout()
    {
        ConsoleHelper.ShowHeader("About Esports Manager");
        
        Console.WriteLine("Esports Manager System v1.0.0");
        Console.WriteLine();
        Console.WriteLine("A comprehensive system for managing esports tournaments,");
        Console.WriteLine("teams, players, and competitions.");
        Console.WriteLine();
        Console.WriteLine("Features:");
        Console.WriteLine("• User Management (Admin, Player, Viewer roles)");
        Console.WriteLine("• Tournament Management");
        Console.WriteLine("• Team Management");
        Console.WriteLine("• Achievement Tracking");
        Console.WriteLine("• Statistics and Reporting");
        Console.WriteLine("• Secure Authentication");
        Console.WriteLine();
        Console.WriteLine("Developed with:");
        Console.WriteLine("• C# .NET 9.0");
        Console.WriteLine("• 3-Layer Architecture (UI, BL, DAL)");
        Console.WriteLine("• SOLID Principles");
        Console.WriteLine("• Dependency Injection");
        Console.WriteLine();
        
        ConsoleHelper.WaitForKey();
    }

    // Placeholder methods for features to be implemented
    private async Task ShowUserManagementAsync()
    {
        ConsoleHelper.ShowInfo("User Management feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowSystemStatisticsAsync()
    {
        try
        {
            ConsoleHelper.ShowHeader("System Statistics");
            
            var totalUsersResult = await _userService.GetTotalUsersCountAsync();
            var activeUsersResult = await _userService.GetActiveUsersCountAsync();
            var adminCountResult = await _userService.GetUserCountByRoleAsync("Admin");
            var playerCountResult = await _userService.GetUserCountByRoleAsync("Player");
            var viewerCountResult = await _userService.GetUserCountByRoleAsync("Viewer");

            if (totalUsersResult.IsSuccess)
            {
                Console.WriteLine($"Total Users: {totalUsersResult.Data}");
            }
            
            if (activeUsersResult.IsSuccess)
            {
                Console.WriteLine($"Active Users: {activeUsersResult.Data}");
            }
            
            if (adminCountResult.IsSuccess)
            {
                Console.WriteLine($"Admins: {adminCountResult.Data}");
            }
            
            if (playerCountResult.IsSuccess)
            {
                Console.WriteLine($"Players: {playerCountResult.Data}");
            }
            
            if (viewerCountResult.IsSuccess)
            {
                Console.WriteLine($"Viewers: {viewerCountResult.Data}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing system statistics");
            ConsoleHelper.ShowError("Could not load system statistics.");
        }
        
        ConsoleHelper.WaitForKey();
    }

    private void ShowSettings()
    {
        ConsoleHelper.ShowInfo("Settings feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowUserProfileAsync(int userId)
    {
        try
        {
            ConsoleHelper.ShowHeader("User Profile");
            ConsoleHelper.ShowLoading("Loading profile", 1000);
            
            var result = await _userService.GetUserProfileAsync(userId);
            
            if (result.IsSuccess && result.Data != null)
            {
                var profile = result.Data;
                Console.WriteLine($"Username: {profile.Username}");
                Console.WriteLine($"Email: {profile.Email ?? "Not set"}");
                Console.WriteLine($"Role: {profile.Role}");
                Console.WriteLine($"Status: {profile.Status}");
                Console.WriteLine($"Member Since: {profile.CreatedAt:yyyy-MM-dd}");
                Console.WriteLine($"Last Login: {profile.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Never"}");
            }
            else
            {
                ConsoleHelper.ShowError("Could not load user profile.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing user profile");
            ConsoleHelper.ShowError("Could not load user profile.");
        }
        
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowChangePasswordAsync(int userId)
    {
        ConsoleHelper.ShowHeader("Change Password");
        
        try
        {
            var currentPassword = ConsoleInput.GetPassword("Current Password");
            var newPassword = ConsoleInput.GetPassword("New Password");
            var confirmPassword = ConsoleInput.GetPassword("Confirm New Password");
            
            var updatePasswordDto = new EsportsManager.BL.DTOs.UpdatePasswordDto
            {
                UserId = userId,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmPassword
            };
            
            ConsoleHelper.ShowLoading("Updating password", 1000);
            
            var result = await _userService.UpdatePasswordAsync(updatePasswordDto);
            
            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Password updated successfully!");
            }
            else
            {
                if (result.Errors.Any())
                {
                    ConsoleHelper.ShowErrors(result.Errors);
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Failed to update password");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            ConsoleHelper.ShowError("An error occurred while changing password.");
        }
        
        ConsoleHelper.WaitForKey();
    }

    private void ShowTournaments()
    {
        ConsoleHelper.ShowInfo("Tournaments feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowMyTeam()
    {
        ConsoleHelper.ShowInfo("My Team feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowMyAchievements()
    {
        ConsoleHelper.ShowInfo("My Achievements feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowTeams()
    {
        ConsoleHelper.ShowInfo("Teams feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowLeaderboards()
    {
        ConsoleHelper.ShowInfo("Leaderboards feature will be implemented in next version.");
        ConsoleHelper.WaitForKey();
    }
}
