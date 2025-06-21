using System;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.MenuServices;

/// <summary>
/// AdminMenuService - Xử lý UI menu cho Admin, delegate business logic cho AdminController
/// Tách biệt UI concerns khỏi business logic
/// </summary>
public class AdminMenuService
{
    private readonly AdminController _adminController;

    public AdminMenuService(AdminController adminController)
    {
        _adminController = adminController ?? throw new ArgumentNullException(nameof(adminController));
    }    /// <summary>
    /// Hiển thị menu chính của Admin
    /// </summary>
    public void ShowAdminMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Hiển thị menu chính của Admin
    /// </summary>
    public void ShowMainMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "👥 Quản lý người dùng",
                "🏆 Quản lý giải đấu/trận đấu", 
                "📊 Xem thống kê hệ thống",
                "💰 Xem báo cáo donation",
                "🗳️ Xem kết quả voting",
                "📝 Quản lý feedback",
                "⚙️ Cài đặt hệ thống",
                "🗑️ Xóa người dùng",
                "🚪 Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("ADMIN CONTROL PANEL", menuOptions);

            switch (selection)
            {
                case 0:
                    ShowUserManagementMenu();
                    break;
                case 1:
                    ShowTournamentManagementMenu();
                    break;
                case 2:
                    ShowSystemStatsMenu();
                    break;
                case 3:
                    ShowDonationReportsMenu();
                    break;
                case 4:
                    ShowVotingResultsMenu();
                    break;
                case 5:
                    ShowFeedbackManagementMenu();
                    break;
                case 6:
                    ShowSystemSettingsMenu();
                    break;
                case 7:
                    ShowUserDeletionMenu();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    /// <summary>
    /// Menu quản lý người dùng
    /// </summary>
    private void ShowUserManagementMenu()
    {
        var userOptions = new[]
        {
            "📋 Xem danh sách người dùng",
            "🔍 Tìm kiếm người dùng",
            "⚡ Thay đổi trạng thái người dùng",
            "👤 Xem thông tin chi tiết người dùng",
            "🔑 Reset mật khẩu người dùng",
            "⬅️ Quay lại"
        };

        while (true)
        {
            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ NGƯỜI DÙNG", userOptions);
            
            switch (selection)
            {
                case 0:
                    ShowAllUsers();
                    break;
                case 1:
                    SearchUsers();
                    break;
                case 2:
                    ToggleUserStatus();
                    break;
                case 3:
                    ShowUserDetails();
                    break;
                case 4:
                    ResetUserPassword();
                    break;
                case 5:
                case -1:
                    return; // Quay lại menu chính
            }
        }
    }

    /// <summary>
    /// Hiển thị danh sách tất cả người dùng
    /// </summary>
    private void ShowAllUsers()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách người dùng...");
            
            var users = _adminController.GetAllUsersAsync().GetAwaiter().GetResult();
            
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH NGƯỜI DÙNG", 100, 20);
            
            if (users.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Không có người dùng nào trong hệ thống.", false, 2000);
                return;
            }            // Display users in a table format
            Console.WriteLine($"{"ID",-5} {"Username",-20} {"Email",-30} {"Role",-10} {"Status",-10}");
            Console.WriteLine(new string('=', 80));
            
            foreach (var user in users)
            {
                string status = "Active"; // TODO: Get actual status
                Console.WriteLine($"{user.Id,-5} {user.Username,-20} {user.Email,-30} {user.Role,-10} {status,-10}");
            }
            
            ConsoleRenderingService.PauseWithMessage("\nNhấn phím bất kỳ để tiếp tục...");
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Tìm kiếm người dùng
    /// </summary>
    private void SearchUsers()
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
            
            var results = _adminController.SearchUsersAsync(searchTerm).GetAwaiter().GetResult();
            
            Console.Clear();
            if (results.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox($"Không tìm thấy người dùng nào với từ khóa: {searchTerm}", false, 2000);
            }
            else
            {                ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 100, 15);
                // Display search results similar to ShowAllUsers
                foreach (var user in results)
                {
                    Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Email: {user.Email}, Role: {user.Role}");
                }
                ConsoleRenderingService.PauseWithMessage("\nNhấn phím bất kỳ để tiếp tục...");
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi tìm kiếm: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Thay đổi trạng thái người dùng
    /// </summary>
    private void ToggleUserStatus()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THAY ĐỔI TRẠNG THÁI NGƯỜI DÙNG", 80, 10);
            
            Console.Write("Nhập User ID: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật trạng thái...");
                
                bool success = _adminController.ToggleUserStatusAsync(userId).GetAwaiter().GetResult();
                
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

    /// <summary>
    /// Reset mật khẩu người dùng
    /// </summary>
    private void ResetUserPassword()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("RESET MẬT KHẨU NGƯỜI DÙNG", 80, 10);
            
            Console.Write("Nhập User ID: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang reset mật khẩu...");
                
                string newPassword = _adminController.ResetUserPasswordAsync(userId).GetAwaiter().GetResult();
                
                ConsoleRenderingService.ShowMessageBox($"Reset thành công! Mật khẩu mới: {newPassword}", false, 5000);
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

    /// <summary>
    /// Hiển thị thông tin chi tiết người dùng
    /// </summary>
    private void ShowUserDetails()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem chi tiết đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu quản lý giải đấu
    /// </summary>
    private void ShowTournamentManagementMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Menu quản lý giải đấu đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu thống kê hệ thống
    /// </summary>
    private void ShowSystemStatsMenu()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thống kê hệ thống...");
            
            var stats = _adminController.GetSystemStatsAsync().GetAwaiter().GetResult();
            
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

    /// <summary>
    /// Menu báo cáo donation
    /// </summary>
    private void ShowDonationReportsMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng báo cáo donation đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu kết quả voting
    /// </summary>
    private void ShowVotingResultsMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem kết quả voting đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu quản lý feedback
    /// </summary>
    private void ShowFeedbackManagementMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý feedback đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu cài đặt hệ thống
    /// </summary>
    private void ShowSystemSettingsMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cài đặt hệ thống đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Menu xóa người dùng
    /// </summary>
    private void ShowUserDeletionMenu()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xóa người dùng đang được phát triển", false, 2000);
    }
}
