using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class UserManagementHandler
{
    private readonly IUserService _userService;

    public UserManagementHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task ShowAllUsersAsync()
    {
        try
        {
            var result = await _userService.GetActiveUsersAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH NGƯỜI DÙNG", 80, 20);

            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                // Set cursor vào giữa border để hiển thị thông báo
                int centerX = (Console.WindowWidth - 30) / 2;
                int centerY = Console.WindowHeight / 2;
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có người dùng nào.");
                Console.ResetColor();
                Console.SetCursorPosition(centerX - 10, centerY + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Tính vị trí để hiển thị data bên trong border
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;
            
            // Set cursor vào bên trong border (cách border 2 ký tự từ trái và 2 dòng từ trên)
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            
            DisplayUsersTableInBorder(result.Data, borderLeft + 2, borderTop + 2, 76);

            // Hiển thị tổng số và thông báo ở cuối border
            Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
            Console.WriteLine($"Tổng cộng: {result.Data.Count()} người dùng");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải danh sách người dùng: {ex.Message}", true, 3000);
        }
    }

    public async Task SearchUsersAsync()
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
            if (!result.IsSuccess || result.Data == null)
            {
                ConsoleRenderingService.ShowNotification("Không thể tải danh sách người dùng", ConsoleColor.Red);
                return;
            }

            var filteredUsers = FilterUsers(result.Data, searchTerm);

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 80, 20);

            if (!filteredUsers.Any())
            {
                // Set cursor vào giữa border để hiển thị thông báo
                int centerX = (Console.WindowWidth - 25) / 2;
                int centerY = Console.WindowHeight / 2;
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không tìm thấy kết quả nào");
                Console.ResetColor();
                Console.SetCursorPosition(centerX - 10, centerY + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Tính vị trí để hiển thị data bên trong border
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;
            
            // Set cursor vào bên trong border
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            
            DisplayUsersTableInBorder(filteredUsers, borderLeft + 2, borderTop + 2, 76);

            // Hiển thị tổng số và thông báo ở cuối border
            Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
            Console.WriteLine($"Tìm thấy: {filteredUsers.Count()} kết quả");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm người dùng: {ex.Message}", true, 3000);
        }
    }

    public async Task ToggleUserStatusAsync()
    {
        try
        {
            Console.Write("\nNhập User ID cần thay đổi trạng thái: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("User ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng!", ConsoleColor.Red);
                return;
            }

            var user = userResult.Data;
            var newStatus = user.Status == "Active" ? "Inactive" : "Active";
            var actionText = newStatus == "Active" ? "kích hoạt" : "vô hiệu hóa";

            Console.WriteLine($"\nThông tin người dùng:");
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Trạng thái hiện tại: {user.Status}");
            Console.WriteLine($"Trạng thái mới: {newStatus}");

            Console.Write($"\nXác nhận {actionText} người dùng này? (y/n): ");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                var result = await _userService.UpdateUserStatusAsync(userId, newStatus);
                if (result.IsSuccess)
                {
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã {actionText} người dùng thành công!", false, 2000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Thay đổi trạng thái thất bại!", true, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác", false, 1000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    public async Task ResetUserPasswordAsync()
    {
        try
        {
            Console.Write("\nNhập User ID cần reset mật khẩu: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("User ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng!", ConsoleColor.Red);
                return;
            }

            var user = userResult.Data;
            Console.WriteLine($"\nThông tin người dùng:");
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Email: {user.Email}");

            Console.Write("\nXác nhận reset mật khẩu? (y/n): ");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                var resetResult = await _userService.ResetPasswordAsync(userId);
                if (!string.IsNullOrEmpty(resetResult))
                {
                    ConsoleRenderingService.ShowNotification($"✅ Đã reset mật khẩu thành công. Mật khẩu mới: {resetResult}", ConsoleColor.Green);
                }
                else
                {
                    ConsoleRenderingService.ShowNotification("❌ Lỗi khi reset mật khẩu", ConsoleColor.Red);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác", false, 1000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    public async Task DeleteUsersAsync()
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
                    var result = await _userService.DeleteUserAsync(userId);
                    if (result)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã xóa thành công user ID: {userId}", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Xóa thất bại", true, 3000);
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
    }

    private void DisplayUsersTable(IEnumerable<UserDto> users)
    {
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
    }

    private IEnumerable<UserDto> FilterUsers(IEnumerable<UserDto> users, string searchTerm)
    {
        return users.Where(u =>
            u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
        );
    }

    private void DisplayUsersTableInBorder(IEnumerable<UserDto> users, int startX, int startY, int maxWidth)
    {
        // Header
        Console.SetCursorPosition(startX, startY);
        var header = string.Format("{0,-5} {1,-15} {2,-25} {3,-10} {4,-10}",
            "ID", "Username", "Email", "Role", "Status");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(header);
        
        // Separator line
        Console.SetCursorPosition(startX, startY + 1);
        Console.WriteLine(new string('─', Math.Min(70, maxWidth - 4)));
        
        // Data rows
        int currentRow = startY + 2;
        int maxRows = 12; // Giới hạn số dòng hiển thị để vừa trong border
        int displayedRows = 0;
        
        foreach (var user in users)
        {
            if (displayedRows >= maxRows) break;
            
            Console.SetCursorPosition(startX, currentRow);
            var row = string.Format("{0,-5} {1,-15} {2,-25} {3,-10} {4,-10}",
                user.Id,
                user.Username.Length > 14 ? user.Username.Substring(0, 14) : user.Username,
                (user.Email?.Length > 24 ? user.Email.Substring(0, 24) : user.Email) ?? "N/A",
                user.Role,
                user.Status);
            
            Console.ForegroundColor = user.Status == "Active" ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(row);
            
            currentRow++;
            displayedRows++;
        }
        
        // Nếu có nhiều dữ liệu hơn, hiển thị thông báo
        if (users.Count() > maxRows)
        {
            Console.SetCursorPosition(startX, currentRow + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"... và {users.Count() - maxRows} người dùng khác");
        }
        
        Console.ResetColor();
    }
}
