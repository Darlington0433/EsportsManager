using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class UserManagementHandler
{
    private readonly IUserService _userService;
    private readonly IAchievementService _achievementService;

    public UserManagementHandler(IUserService userService, IAchievementService achievementService)
    {
        _userService = userService;
        _achievementService = achievementService;
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
                // Check if trying to delete their own account (Admin cannot delete themselves)
                var currentUser = EsportsManager.UI.Services.UserSessionManager.CurrentUser;
                if (currentUser != null && currentUser.Id == userId && currentUser.Role == "Admin")
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Admin không thể xóa tài khoản của chính mình!", true, 3000);
                    return;
                }

                // Get user details to check role
                var userResult = await _userService.GetUserByIdAsync(userId);
                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Không tìm thấy người dùng!", true, 2000);
                    return;
                }

                var targetUser = userResult.Data;

                // Admin can only delete Player/Viewer, not other Admins
                if (targetUser.Role == "Admin")
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Admin không thể xóa tài khoản Admin khác!", true, 3000);
                    return;
                }

                Console.WriteLine($"\nThông tin người dùng sẽ bị xóa:");
                Console.WriteLine($"Username: {targetUser.Username}");
                Console.WriteLine($"Email: {targetUser.Email}");
                Console.WriteLine($"Role: {targetUser.Role}");

                Console.Write($"\nXác nhận xóa user ID {userId}? (YES để xác nhận): ");
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

    /// <summary>
    /// Phê duyệt tài khoản đang chờ xử lý
    /// </summary>
    public async Task ApprovePendingAccountsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("PHÊ DUYỆT TÀI KHOẢN", 80, 20);

            // Get pending accounts
            var result = await _userService.GetPendingAccountsAsync();

            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                int centerX = (Console.WindowWidth - 30) / 2;
                int centerY = Console.WindowHeight / 2;
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có tài khoản nào đang chờ phê duyệt.");
                Console.ResetColor();
                Console.SetCursorPosition(centerX - 10, centerY + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Display pending accounts
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("📋 Danh sách tài khoản đang chờ phê duyệt:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 4;
            var pendingAccounts = result.Data.ToList();

            for (int i = 0; i < pendingAccounts.Count && i < 10; i++)
            {
                var user = pendingAccounts[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                Console.WriteLine($"{i + 1}. ID: {user.Id} | {user.Username} | {user.Email} | Role: {user.Role}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(pendingAccounts.Count, 10) + 1);
            Console.Write("Nhập số thứ tự tài khoản cần phê duyệt (0 để thoát): ");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= pendingAccounts.Count)
            {
                var selectedUser = pendingAccounts[selection - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(pendingAccounts.Count, 10) + 3);
                Console.WriteLine($"Phê duyệt tài khoản: {selectedUser.Username}");
                Console.Write("Xác nhận phê duyệt? (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    var approveResult = await _userService.ApproveAccountAsync(selectedUser.Id);
                    if (approveResult.IsSuccess)
                    {
                        ConsoleRenderingService.ShowMessageBox("✅ Đã phê duyệt tài khoản thành công!", false, 2000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Phê duyệt thất bại: {approveResult.ErrorMessage}", true, 3000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác", false, 1000);
                }
            }
            else if (selection != 0)
            {
                ConsoleRenderingService.ShowMessageBox("❌ Lựa chọn không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Gán thành tích cho người chơi
    /// </summary>
    public async Task AssignAchievementsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GÁN THÀNH TÍCH", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("🏆 GÁN THÀNH TÍCH CHO PLAYER");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine(new string('─', 70));

            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine("📝 Lưu ý: Chỉ có thể gán thành tích cho tài khoản có role 'Player'");
            Console.WriteLine();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
            Console.Write("👤 Nhập username của Player (0 để thoát): ");
            var username = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username) || username == "0")
            {
                return;
            }

            // Kiểm tra user tồn tại và là Player
            var userResult = await _userService.GetUserByUsernameAsync(username);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Không tìm thấy user với username: {username}", true, 3000);
                return;
            }

            var selectedUser = userResult.Data;

            // Kiểm tra role phải là Player
            if (selectedUser.Role != "Player")
            {
                string roleMessage = selectedUser.Role switch
                {
                    "Admin" => "❌ Không thể gán thành tích cho Admin!",
                    "Viewer" => "❌ Không thể gán thành tích cho Viewer!",
                    _ => $"❌ Không thể gán thành tích cho role '{selectedUser.Role}'!"
                };

                ConsoleRenderingService.ShowMessageBox($"{roleMessage}\n💡 Chỉ có thể gán thành tích cho tài khoản Player.", true, 3000);
                return;
            }

            // Hiển thị thông tin Player đã chọn
            Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
            Console.WriteLine($"✅ Đã tìm thấy Player: {selectedUser.Username}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 10);
            Console.WriteLine($"📄 Họ tên: {selectedUser.FullName ?? "N/A"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 11);
            Console.WriteLine($"📧 Email: {selectedUser.Email}");

            // Achievement types
            var achievementTypes = new string[]
            {
                "Tournament Winner",
                "Top 3 Finisher",
                "Most Valuable Player",
                "Best Team Player",
                "Rising Star",
                "Veteran Player",
                "Fair Play Award",
                "Community Champion"
            };

            Console.SetCursorPosition(borderLeft + 2, borderTop + 13);
            Console.WriteLine("🏆 Chọn loại thành tích:");

            for (int i = 0; i < achievementTypes.Length; i++)
            {
                Console.SetCursorPosition(borderLeft + 4, borderTop + 14 + i);
                Console.WriteLine($"{i + 1}. {achievementTypes[i]}");
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 14 + achievementTypes.Length + 1);
            Console.Write("Chọn thành tích (1-8): ");

            if (int.TryParse(Console.ReadLine(), out int achievementChoice) && achievementChoice > 0 && achievementChoice <= achievementTypes.Length)
            {
                var selectedAchievement = achievementTypes[achievementChoice - 1];

                Console.SetCursorPosition(borderLeft + 2, borderTop + 14 + achievementTypes.Length + 2);
                Console.Write("Nhập mô tả thành tích: ");
                var description = Console.ReadLine() ?? "";

                Console.SetCursorPosition(borderLeft + 2, borderTop + 14 + achievementTypes.Length + 3);
                Console.Write($"Xác nhận gán thành tích '{selectedAchievement}' cho {selectedUser.Username}? (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    // Gán thành tích thực sự vào database
                    var currentUser = EsportsManager.UI.Services.UserSessionManager.CurrentUser;
                    int adminId = currentUser?.Id ?? 1; // Fallback to admin ID 1

                    var success = await _achievementService.AssignAchievementAsync(
                        selectedUser.Id,
                        selectedAchievement,
                        description,
                        adminId);

                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã gán thành tích '{selectedAchievement}' cho Player {selectedUser.Username}!\n📝 Mô tả: {description}\n💾 Dữ liệu đã được lưu vào database", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Không thể gán thành tích cho {selectedUser.Username}. Vui lòng thử lại.", true, 3000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác", false, 1000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Lựa chọn thành tích không hợp lệ!", true, 2000);
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
