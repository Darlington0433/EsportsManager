using EsportsManager.BL.Interfaces;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
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
                ConsoleHelper.ShowHeader("Menu Chính");

                Console.WriteLine("Vui lòng chọn một tùy chọn:");
                Console.WriteLine();
                Console.WriteLine("1. Đăng nhập");
                Console.WriteLine("2. Đăng ký");
                Console.WriteLine("3. Giới thiệu");
                Console.WriteLine("0. Thoát");
                Console.WriteLine();

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 3);

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
                        if (ConsoleInput.GetConfirmation("Bạn có chắc muốn thoát không?"))
                        {
                            ConsoleHelper.ShowInfo("Cảm ơn bạn đã sử dụng Esports Manager!");
                            return;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong menu chính");
                ConsoleHelper.ShowError("Đã xảy ra lỗi. Vui lòng thử lại.");
                ConsoleHelper.WaitForKey();
            }
        }
    }

    /// <summary>
    /// Show login form
    /// </summary>
    private async Task ShowLoginAsync()
    {
        ConsoleHelper.ShowHeader("Đăng nhập");

        try
        {
            var username = ConsoleInput.GetUsername("Tên đăng nhập");
            var password = ConsoleInput.GetPassword("Mật khẩu");

            ConsoleHelper.ShowLoading("Đang xác thực", 1500);

            var loginDto = new EsportsManager.BL.DTOs.LoginDto
            {
                Username = username,
                Password = password
            };

            var result = await _userService.AuthenticateAsync(loginDto);

            if (result.IsAuthenticated)
            {
                ConsoleHelper.ShowSuccess($"Chào mừng trở lại, {result.Username}!");
                _logger.LogInformation("Người dùng đã đăng nhập: {Username}", result.Username);

                // Hiển thị menu dựa trên vai trò của người dùng
                await ShowRoleMenuAsync(result.UserId!.Value, result.Username!, result.Role!);
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Đăng nhập thất bại");
                _logger.LogWarning("Nỗ lực đăng nhập thất bại cho tên đăng nhập: {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong quá trình đăng nhập");
            ConsoleHelper.ShowError("Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại.");
        }

        ConsoleHelper.WaitForKey();
    }

    /// <summary>
    /// Show registration form
    /// </summary>
    private async Task ShowRegisterAsync()
    {
        ConsoleHelper.ShowHeader("Đăng Ký Tài Khoản Mới");

        try
        {
            var username = ConsoleInput.GetUsername("Tên đăng nhập");
            var email = ConsoleInput.GetEmail("Email (không bắt buộc)", false);
            var password = ConsoleInput.GetPassword("Mật khẩu");
            var confirmPassword = ConsoleInput.GetPassword("Xác nhận mật khẩu");

            ConsoleHelper.ShowLoading("Đang tạo tài khoản", 1500);

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
                ConsoleHelper.ShowSuccess("Tài khoản đã được tạo thành công!");
                ConsoleHelper.ShowInfo($"Chào mừng, {result.Data!.Username}! Bạn có thể đăng nhập với thông tin đăng nhập của mình.");
                _logger.LogInformation("Người dùng mới đã đăng ký: {Username}", result.Data.Username);
            }
            else
            {
                if (result.Errors.Any())
                {
                    ConsoleHelper.ShowErrors(result.Errors);
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Đăng ký thất bại");
                }
                _logger.LogWarning("Nỗ lực đăng ký thất bại cho tên đăng nhập: {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi trong quá trình đăng ký");
            ConsoleHelper.ShowError("Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.");
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
                ConsoleHelper.ShowWarning($"Vai trò không xác định: {role}. Hiển thị menu người xem.");
                await ShowViewerMenuAsync(userId, username);
                break;
        }
    }

    /// <summary>
    /// Show admin menu
    /// </summary>
    private async Task ShowAdminMenuAsync(int userId, string username)
    {
        try
        {
            var serviceProvider = Program.ServiceProvider;

            // Use GetRequiredService to get the services
            var tournamentService = serviceProvider.GetRequiredService<ITournamentService>();
            var teamService = serviceProvider.GetRequiredService<ITeamService>();
            var logger = serviceProvider.GetRequiredService<ILogger<AdminMenu>>();

            var adminMenu = new AdminMenu(
                _userService,
                tournamentService,
                teamService,
                logger,
                userId,
                username);

            await adminMenu.ShowAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing admin menu");
            ConsoleHelper.ShowError("An error occurred loading the admin menu. Please try again.");
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// Show player menu
    /// </summary>
    private async Task ShowPlayerMenuAsync(int userId, string username)
    {
        try
        {
            var serviceProvider = Program.ServiceProvider;

            // Use GetRequiredService to get the services
            var tournamentService = serviceProvider.GetRequiredService<ITournamentService>();
            var teamService = serviceProvider.GetRequiredService<ITeamService>();
            var achievementService = serviceProvider.GetRequiredService<IAchievementService>();
            var walletService = serviceProvider.GetRequiredService<IWalletService>();
            var feedbackService = serviceProvider.GetRequiredService<IFeedbackService>();
            var logger = serviceProvider.GetRequiredService<ILogger<PlayerMenu>>();

            var playerMenu = new PlayerMenu(
                _userService,
                tournamentService,
                teamService,
                achievementService,
                walletService,
                feedbackService,
                logger,
                userId,
                username);

            await playerMenu.ShowAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khởi tạo menu người chơi");
            ConsoleHelper.ShowError("Đã xảy ra lỗi khi tải menu người chơi. Vui lòng thử lại.");
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// Show viewer menu
    /// </summary>
    private async Task ShowViewerMenuAsync(int userId, string username)
    {
        try
        {
            var serviceProvider = Program.ServiceProvider;

            // Use GetRequiredService to get the services
            var tournamentService = serviceProvider.GetRequiredService<ITournamentService>();
            var teamService = serviceProvider.GetRequiredService<ITeamService>();
            var donationService = serviceProvider.GetRequiredService<IDonationService>();
            var walletService = serviceProvider.GetRequiredService<IWalletService>();
            var feedbackService = serviceProvider.GetRequiredService<IFeedbackService>();
            var voteService = serviceProvider.GetRequiredService<IVoteService>();
            var logger = serviceProvider.GetRequiredService<ILogger<ViewerMenu>>();

            var viewerMenu = new ViewerMenu(
                _userService,
                tournamentService,
                teamService,
                donationService,
                walletService,
                feedbackService,
                voteService,
                logger,
                userId,
                username);

            await viewerMenu.ShowAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khởi tạo menu người xem");
            ConsoleHelper.ShowError("Đã xảy ra lỗi khi tải menu người xem. Vui lòng thử lại.");
            ConsoleHelper.PressAnyKeyToContinue();
        }
    }

    /// <summary>
    /// Show about information
    /// </summary>
    private void ShowAbout()
    {
        ConsoleHelper.ShowHeader("Giới thiệu về Esports Manager");

        Console.WriteLine("Hệ thống Quản lý Esports v1.0.0");
        Console.WriteLine();
        Console.WriteLine("Một hệ thống toàn diện để quản lý các giải đấu esports,");
        Console.WriteLine("đội tuyển, người chơi, và các cuộc thi đấu.");
        Console.WriteLine();
        Console.WriteLine("Tính năng:");
        Console.WriteLine("• Quản lý người dùng (vai trò Admin, Player, Viewer)");
        Console.WriteLine("• Quản lý giải đấu");
        Console.WriteLine("• Quản lý đội tuyển");
        Console.WriteLine("• Theo dõi thành tích");
        Console.WriteLine("• Thống kê và báo cáo");
        Console.WriteLine("• Xác thực an toàn");
        Console.WriteLine();
        Console.WriteLine("Phát triển với:");
        Console.WriteLine("• C# .NET 9.0");
        Console.WriteLine("• Kiến trúc 3 tầng (UI, BL, DAL)");
        Console.WriteLine("• Nguyên tắc SOLID");
        Console.WriteLine("• Dependency Injection");
        Console.WriteLine();

        ConsoleHelper.WaitForKey();
    }

    // Placeholder methods for features to be implemented
    private async Task ShowUserManagementAsync()
    {
        ConsoleHelper.ShowInfo("Tính năng Quản lý Người dùng sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowSystemStatisticsAsync()
    {
        try
        {
            ConsoleHelper.ShowHeader("Thống kê Hệ thống");

            var totalUsersResult = await _userService.GetTotalUsersCountAsync();
            var activeUsersResult = await _userService.GetActiveUsersCountAsync();
            var adminCountResult = await _userService.GetUserCountByRoleAsync("Admin");
            var playerCountResult = await _userService.GetUserCountByRoleAsync("Player");
            var viewerCountResult = await _userService.GetUserCountByRoleAsync("Viewer");

            if (totalUsersResult.IsSuccess)
            {
                Console.WriteLine($"Tổng số người dùng: {totalUsersResult.Data}");
            }

            if (activeUsersResult.IsSuccess)
            {
                Console.WriteLine($"Người dùng đang hoạt động: {activeUsersResult.Data}");
            }

            if (adminCountResult.IsSuccess)
            {
                Console.WriteLine($"Quản trị viên: {adminCountResult.Data}");
            }

            if (playerCountResult.IsSuccess)
            {
                Console.WriteLine($"Người chơi: {playerCountResult.Data}");
            }

            if (viewerCountResult.IsSuccess)
            {
                Console.WriteLine($"Người xem: {viewerCountResult.Data}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hiển thị thống kê hệ thống");
            ConsoleHelper.ShowError("Không thể tải thống kê hệ thống.");
        }

        ConsoleHelper.WaitForKey();
    }

    private void ShowSettings()
    {
        ConsoleHelper.ShowInfo("Tính năng Cài đặt sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private async Task ShowUserProfileAsync(int userId)
    {
        try
        {
            ConsoleHelper.ShowHeader("Hồ sơ Người dùng");
            ConsoleHelper.ShowLoading("Đang tải hồ sơ", 1000);

            var result = await _userService.GetUserProfileAsync(userId);

            if (result.IsSuccess && result.Data != null)
            {
                var profile = result.Data;
                Console.WriteLine($"Tên đăng nhập: {profile.Username}");
                Console.WriteLine($"Email: {profile.Email ?? "Chưa đặt"}");
                Console.WriteLine($"Vai trò: {profile.Role}");
                Console.WriteLine($"Trạng thái: {profile.Status}");
                Console.WriteLine($"Thành viên từ: {profile.CreatedAt:yyyy-MM-dd}");
                Console.WriteLine($"Đăng nhập lần cuối: {profile.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Chưa bao giờ"}");
            }
            else
            {
                ConsoleHelper.ShowError("Không thể tải hồ sơ người dùng.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hiển thị hồ sơ người dùng");
            ConsoleHelper.ShowError("Không thể tải hồ sơ người dùng.");
        }

        ConsoleHelper.WaitForKey();
    }

    private async Task ShowChangePasswordAsync(int userId)
    {
        ConsoleHelper.ShowHeader("Đổi Mật Khẩu");

        try
        {
            var currentPassword = ConsoleInput.GetPassword("Mật khẩu hiện tại");
            var newPassword = ConsoleInput.GetPassword("Mật khẩu mới");
            var confirmPassword = ConsoleInput.GetPassword("Xác nhận mật khẩu mới");

            var updatePasswordDto = new EsportsManager.BL.DTOs.UpdatePasswordDto
            {
                UserId = userId,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmPassword
            };

            ConsoleHelper.ShowLoading("Đang cập nhật mật khẩu", 1000);

            var result = await _userService.UpdatePasswordAsync(updatePasswordDto);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Cập nhật mật khẩu thành công!");
            }
            else
            {
                if (result.Errors.Any())
                {
                    ConsoleHelper.ShowErrors(result.Errors);
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Cập nhật mật khẩu thất bại");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thay đổi mật khẩu");
            ConsoleHelper.ShowError("Đã xảy ra lỗi khi thay đổi mật khẩu.");
        }

        ConsoleHelper.WaitForKey();
    }

    private void ShowTournaments()
    {
        ConsoleHelper.ShowInfo("Tính năng Giải đấu sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowMyTeam()
    {
        ConsoleHelper.ShowInfo("Tính năng Đội của tôi sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowMyAchievements()
    {
        ConsoleHelper.ShowInfo("Tính năng Thành tích của tôi sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowTeams()
    {
        ConsoleHelper.ShowInfo("Tính năng Đội tuyển sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }

    private void ShowLeaderboards()
    {
        ConsoleHelper.ShowInfo("Tính năng Bảng xếp hạng sẽ được triển khai trong phiên bản tiếp theo.");
        ConsoleHelper.WaitForKey();
    }
}
