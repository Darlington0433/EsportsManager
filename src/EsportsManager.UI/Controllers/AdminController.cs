// Controller xử lý chức năng Admin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;

namespace EsportsManager.UI.Controllers;

public class AdminUIController
{
    private readonly UserDto _currentUser;
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public AdminUIController(UserDto currentUser, IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _currentUser = currentUser;
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
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

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU ADMIN - {_currentUser.Username}", menuOptions); switch (selection)
            {
                case 0:
                    ManageUsersAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    ManageTournamentsAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    ViewSystemStatsAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    ViewDonationReportsAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    ViewVotingResultsAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    ManageFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    SystemSettingsAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                    DeleteUsersAsync().GetAwaiter().GetResult();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
            }
        }    private async Task ManageUsersAsync()
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
                    await ShowAllUsersAsync();
                    break;
                case 1:
                    await SearchUsersAsync();
                    break;
                case 2:
                    await ToggleUserStatusAsync();
                    break;
                case 3:
                    await ResetUserPasswordAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    private async Task ShowAllUsersAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải danh sách người dùng...");

            var result = await _userService.GetActiveUsersAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH NGƯỜI DÙNG", 80, 20);

            if (!result.IsSuccess || !result.Data.Any())
            {
                ConsoleRenderingService.ShowNotification("Không có người dùng nào.", ConsoleColor.Yellow);
                return;
            }

            var header = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                "ID", "Username", "Email", "Role", "Status");
            Console.WriteLine("\n" + header);
            Console.WriteLine(new string('─', 75));

            foreach (var user in result.Data)
            {
                var row = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                    user.Id,
                    user.Username,
                    user.Email ?? "",
                    user.Role,
                    user.Status);
                Console.WriteLine(row);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLỗi: {ex.Message}");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }

    private async Task SearchUsersAsync()
    {
        try
        {
            Console.Write("\nNhập từ khóa tìm kiếm: ");
            var searchTerm = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowNotification("Từ khóa tìm kiếm không được rỗng", ConsoleColor.Yellow);
                return;
            }

            var result = await _userService.GetActiveUsersAsync();
            if (!result.IsSuccess)
            {
                ConsoleRenderingService.ShowNotification("Không thể tải danh sách người dùng", ConsoleColor.Red);
                return;
            }

            var users = result.Data.Where(u =>
                u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 80, 20);

            if (!users.Any())
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy kết quả nào", ConsoleColor.Yellow);
                return;
            }

            var header = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                "ID", "Username", "Email", "Role", "Status");
            Console.WriteLine("\n" + header);
            Console.WriteLine(new string('─', 75));

            foreach (var user in users)
            {
                var row = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                    user.Id,
                    user.Username,
                    user.Email ?? "",
                    user.Role,
                    user.Status);
                Console.WriteLine(row);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLỗi: {ex.Message}");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }

    private async Task ToggleUserStatusAsync()
    {
        try
        {
            Console.Write("\nNhập ID người dùng: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ", ConsoleColor.Yellow);
                return;
            }

            var result = await _userService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng", ConsoleColor.Yellow);
                return;
            }

            var user = result.Data;
            var newStatus = user.Status == "Active" ? "Inactive" : "Active";

            var confirmPrompt = $"Xác nhận thay đổi trạng thái người dùng {user.Username} từ {user.Status} sang {newStatus}? (Y/N): ";
            Console.Write("\n" + confirmPrompt);

            if (Console.ReadKey(true).Key != ConsoleKey.Y)
            {
                ConsoleRenderingService.ShowNotification("Đã hủy thao tác", ConsoleColor.Blue);
                return;
            }

            var updatedUser = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                Status = newStatus
            };
            var updateResult = await _userService.UpdateUserProfileAsync(userId, updatedUser);

            if (updateResult.IsSuccess)
                ConsoleRenderingService.ShowNotification($"Đã cập nhật trạng thái thành công", ConsoleColor.Green);
            else
                ConsoleRenderingService.ShowNotification($"Lỗi khi cập nhật trạng thái", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowNotification($"Lỗi: {ex.Message}", ConsoleColor.Red);
        }
    }

    private async Task ResetUserPasswordAsync()
    {
        try
        {
            Console.Write("\nNhập ID người dùng: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ", ConsoleColor.Yellow);
                return;
            }

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng", ConsoleColor.Yellow);
                return;
            }

            var confirmPrompt = $"Xác nhận reset mật khẩu cho người dùng {userResult.Data.Username}? (Y/N): ";
            Console.Write("\n" + confirmPrompt);

            if (Console.ReadKey(true).Key != ConsoleKey.Y)
            {
                ConsoleRenderingService.ShowNotification("Đã hủy thao tác", ConsoleColor.Blue);
                return;
            }

            var resetResult = await _userService.ResetPasswordAsync(new ResetPasswordDto
            {
                Id = userId,
                AdminId = _currentUser.Id
            });

            if (resetResult.IsSuccess)
                ConsoleRenderingService.ShowNotification($"Đã reset mật khẩu thành công", ConsoleColor.Green);
            else
                ConsoleRenderingService.ShowNotification($"Lỗi khi reset mật khẩu", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowNotification($"Lỗi: {ex.Message}", ConsoleColor.Red);
        }
    }

    private async Task ManageTournamentsAsync()
    {
        ConsoleRenderingService.ShowMessageBox("🏆 Chức năng quản lý giải đấu sẽ được kết nối với BL TournamentService", false, 2000);
    }

    private async Task ViewSystemStatsAsync()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thống kê hệ thống...");

            var stats = await GetSystemStatsAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", 80, 15);

            Console.WriteLine($"📊 Tổng số người dùng: {stats.TotalUsers}");
            Console.WriteLine($"✅ Số người dùng hoạt động: {stats.ActiveUsers}");
            Console.WriteLine($"🏆 Tổng số giải đấu: {stats.TotalTournaments}");
            Console.WriteLine($"🔥 Số giải đấu đang diễn ra: {stats.ActiveTournaments}");
            Console.WriteLine($"👥 Tổng số team: {stats.TotalTeams}");
            Console.WriteLine($"💰 Tổng doanh thu: {stats.TotalRevenue:N0} VND");

            ConsoleRenderingService.PauseWithMessage("\nNhấn phím bất kỳ để tiếp tục...");
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }

    private async Task ViewDonationReportsAsync()
    {
        ConsoleRenderingService.ShowMessageBox("💰 Chức năng báo cáo donation sẽ được kết nối với BL WalletService", false, 2000);
    }

    private async Task ViewVotingResultsAsync()
    {
        ConsoleRenderingService.ShowMessageBox("🗳️ Chức năng kết quả voting sẽ được kết nối với BL VotingService", false, 2000);
    }

    private async Task ManageFeedbackAsync()
    {
        ConsoleRenderingService.ShowMessageBox("📝 Chức năng quản lý feedback sẽ được kết nối với BL TournamentService", false, 2000);
    }

    private async Task SystemSettingsAsync()
    {
        ConsoleRenderingService.ShowMessageBox("⚙️ Chức năng cài đặt hệ thống sẽ được kết nối với BL ConfigService", false, 2000);
    }

    private async Task DeleteUsersAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("XÓA NGƯỜI DÙNG", 80, 15);

            Console.WriteLine("⚠️  CẢNH BÁO: Thao tác này sẽ xóa vĩnh viễn người dùng!");
            Console.WriteLine("📋 Dữ liệu sẽ bị xóa:");
            Console.WriteLine("   • Thông tin tài khoản");
            Console.WriteLine("   • Lịch sử tham gia giải đấu");
            Console.WriteLine("   • Dữ liệu team");
            Console.WriteLine("   • Lịch sử giao dịch");

            Console.Write("\nNhập User ID cần xóa: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.Write($"Xác nhận xóa user ID {userId}? (YES để xác nhận): ");
                string confirmation = Console.ReadLine() ?? "";

                if (confirmation.ToUpper() == "YES")
                {
                    ConsoleRenderingService.ShowLoadingMessage("Đang xóa người dùng...");

                    var result = await _userService.DeleteUserAsync(userId);

                    if (result.IsSuccess)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã xóa thành công user ID: {userId}", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Xóa thất bại: {result.Message}", true, 3000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác xóa", false, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("ID không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    // Public async methods for AdminMenuService to call    
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var result = await _userService.GetActiveUsersAsync();
        return result.IsSuccess ? result.Data.ToList() : new List<UserDto>();
    }

    public async Task<UserDto?> GetUserDetailsAsync(int userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);
        return result.IsSuccess ? result.Data : null;
    }

    // Async methods calling BL Services
    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        var result = await _userService.GetActiveUsersAsync();
        if (!result.IsSuccess) return new List<UserDto>();

        // Simple search implementation - could be enhanced
        return result.Data.Where(u =>
            u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
        ).ToList();
    }

    // Interactive methods that call BL Services    private async Task ShowAllUsersAsync()
    {
        try
        {
            Console.WriteLine("Đang tải danh sách người dùng...");
            
            var result = await _userService.GetActiveUsersAsync();

    Console.Clear();
            Console.WriteLine("═══════════════ DANH SÁCH NGƯỜI DÙNG ═══════════════");
            Console.WriteLine();
            
            if (!result.IsSuccess || !result.Data.Any())
            {
                Console.WriteLine("Không có người dùng nào.");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

Console.WriteLine(string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                "ID", "Username", "Email", "Role", "Status"));
            Console.WriteLine(new string ('─', 75));

            foreach (var user in result.Data)
            {
                Console.WriteLine(string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                    user.Id,
                    user.Username,
                    user.Email ?? "",
                    user.Role,
                    user.Status));
            }
            
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLỗi: {ex.Message}");
Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
Console.ReadKey(true);
        }
    }

    private async Task SearchUsersInteractive()
{
    try
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("TÌM KIẾM NGƯỜI DÙNG", 80, 10);

        Console.Write("Nhập từ khóa tìm kiếm (username hoặc email): ");
        string searchTerm = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            ConsoleRenderingService.ShowMessageBox("Từ khóa tìm kiếm không được rỗng!", true, 2000);
            return;
        }

        ConsoleRenderingService.ShowLoadingMessage("Đang tìm kiếm...");

        var results = await SearchUsersAsync(searchTerm);

        Console.Clear();
        if (!results.Any())
        {
            ConsoleRenderingService.ShowMessageBox($"Không tìm thấy người dùng nào với từ khóa: {searchTerm}", false, 2000);
        }
        else
        {
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 100, 15);
            Console.WriteLine($"{"ID",-5} {"Username",-20} {"Email",-30} {"Role",-10}");
            Console.WriteLine(new string('=', 80));

            foreach (var user in results)
            {
                Console.WriteLine($"{user.Id,-5} {user.Username,-20} {user.Email,-30} {user.Role,-10}");
            }
            ConsoleRenderingService.PauseWithMessage("\nNhấn phím bất kỳ để tiếp tục...");
        }
    }
    catch (Exception ex)
    {
        ConsoleRenderingService.ShowMessageBox($"Lỗi tìm kiếm: {ex.Message}", true, 3000);
    }
}

private async Task ToggleUserStatusInteractive()
{
    try
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("THAY ĐỔI TRẠNG THÁI NGƯỜI DÙNG", 80, 10);

        Console.Write("Nhập User ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật trạng thái...");

            bool success = await ToggleUserStatusAsync(userId);

            if (success)
            {
                ConsoleRenderingService.ShowMessageBox("Thay đổi trạng thái thành công!", false, 2000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Không thể thay đổi trạng thái!", true, 2000);
            }
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("User ID không hợp lệ!", true, 2000);
        }
    }
    catch (Exception ex)
    {
        ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
    }
}

private async Task ResetUserPasswordInteractive()
{
    try
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("RESET MẬT KHẨU NGƯỜI DÙNG", 80, 10);

        Console.Write("Nhập User ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang reset mật khẩu...");

            string newPassword = await ResetUserPasswordAsync(userId);

            if (!string.IsNullOrEmpty(newPassword))
            {
                ConsoleRenderingService.ShowMessageBox($"Reset thành công! Mật khẩu mới: {newPassword}", false, 5000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Reset mật khẩu thất bại!", true, 2000);
            }
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("User ID không hợp lệ!", true, 2000);
        }
    }
    catch (Exception ex)
    {
        ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
    }
}
}
