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

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;
            int borderBottom = borderTop + 20;

            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có người dùng nào.");
                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderBottom + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);

            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.WriteLine($"Tổng cộng: {result.Data.Count()} người dùng");
            // Đã bỏ gọi PrintUserListShortcuts ở đây để tránh trùng lặp

            int selectedIndex = 0;
            var users = result.Data.ToList();
            int maxRows = 12;
            int page = 0;
            int totalPages = (int)Math.Ceiling(users.Count / (double)maxRows);

            void RenderPage()
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH NGƯỜI DÙNG", 80, 20);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                int borderBottom = borderTop + 20;
                var pageUsers = users.Skip(page * maxRows).Take(maxRows).ToList();
                int tableStartY = borderTop + 3;
                // Header
                Console.SetCursorPosition(borderLeft + 2, tableStartY);
                var header = string.Format("{0,-5} {1,-15} {2,-25} {3,-10} {4,-10}",
                    "ID", "Username", "Email", "Role", "Status");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(header);
                Console.SetCursorPosition(borderLeft + 2, tableStartY + 1);
                Console.WriteLine(new string('─', 70));
                // Data rows
                for (int i = 0; i < pageUsers.Count; i++)
                {
                    Console.SetCursorPosition(borderLeft + 2, tableStartY + 2 + i);
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = pageUsers[i].Status == "Active" ? ConsoleColor.Green : ConsoleColor.Red;
                    }
                    var email = pageUsers[i].Email;
                    var emailDisplay = string.IsNullOrEmpty(email)
                        ? "N/A"
                        : (email.Length > 24 ? email.Substring(0, 24) : email);
                    var row = string.Format("{0,-5} {1,-15} {2,-25} {3,-10} {4,-10}",
                        pageUsers[i].Id,
                        pageUsers[i].Username.Length > 14 ? pageUsers[i].Username.Substring(0, 14) : pageUsers[i].Username,
                        $"{emailDisplay}",
                        pageUsers[i].Role,
                        pageUsers[i].Status);
                    Console.WriteLine(row);
                    Console.ResetColor();
                }
                // Footer
                int footerY = borderBottom;
                int footerX = borderLeft + (80 - 15) / 2; // căn giữa chuỗi [Trang x/y]
                Console.SetCursorPosition(footerX, footerY);
                Console.WriteLine($"[Trang {page + 1}/{totalPages}]");
                // Luôn hiển thị chỉ dẫn phím tắt ở dưới border, căn giữa
                int shortcutY = borderBottom + 1;
                int shortcutX = borderLeft + (80 - 60) / 2; // căn giữa chỉ dẫn (giả sử chỉ dẫn dài 60 ký tự)
                PrintUserListShortcuts(shortcutX, shortcutY);
            }

            RenderPage();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.F)
                {
                    await SearchUsersAsync();
                    break;
                }
                else if (key.Key == ConsoleKey.LeftArrow && page > 0)
                {
                    page--;
                    selectedIndex = 0;
                    RenderPage();
                }
                else if (key.Key == ConsoleKey.RightArrow && page < totalPages - 1)
                {
                    page++;
                    selectedIndex = 0;
                    RenderPage();
                }
                else if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
                {
                    selectedIndex--;
                    RenderPage();
                }
                else if (key.Key == ConsoleKey.DownArrow && selectedIndex < Math.Min(maxRows, users.Count - page * maxRows) - 1)
                {
                    selectedIndex++;
                    RenderPage();
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var pageUsers = users.Skip(page * maxRows).Take(maxRows).ToList();
                    if (selectedIndex < pageUsers.Count)
                    {
                        var selectedUser = pageUsers[selectedIndex];
                        // Lấy thông tin profile chi tiết từ service
                        var profileResult = await _userService.GetUserProfileAsync(selectedUser.Id);
                        if (profileResult.IsSuccess && profileResult.Data != null)
                        {
                            ShowUserProfile(profileResult.Data);
                            // Làm mới danh sách sau khi xem profile
                            Console.Clear();
                            var refreshResult = await _userService.GetActiveUsersAsync();
                            if (refreshResult.IsSuccess && refreshResult.Data != null)
                            {
                                users = refreshResult.Data.ToList();
                                totalPages = (int)Math.Ceiling(users.Count / (double)maxRows);
                                if (page >= totalPages) page = Math.Max(0, totalPages - 1);
                                selectedIndex = 0;
                            }
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Không lấy được thông tin chi tiết người dùng!", true, 2000);
                        }
                        RenderPage();
                    }
                }
                else if (key.Key == ConsoleKey.C)
                {
                    // Gọi hàm tạo mới hoặc xử lý tạo mới ở đây
                    // await CreateUserAsync();
                    break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    // Thoát menu
                    break;
                }
            }
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TÌM KIẾM NGƯỜI DÙNG", 80, 20);
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;
            int borderBottom = borderTop + 20;
            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.Write("Nhập từ khóa tìm kiếm: ");
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
            borderLeft = (Console.WindowWidth - 80) / 2;
            borderTop = (Console.WindowHeight - 20) / 4;
            borderBottom = borderTop + 20;

            if (!filteredUsers.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không tìm thấy kết quả nào");
                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderBottom + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            DisplayUsersTableInBorder(filteredUsers, borderLeft + 2, borderTop + 2, 76);

            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.WriteLine($"Tìm thấy: {filteredUsers.Count()} kết quả");
            PrintUserListShortcuts(borderLeft + 2, borderBottom + 2);
            Console.SetCursorPosition(borderLeft + 2, borderBottom + 3);
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THAY ĐỔI TRẠNG THÁI NGƯỜI DÙNG", 80, 12);
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 12) / 4;
            int borderBottom = borderTop + 12;
            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.Write("Nhập User ID cần thay đổi trạng thái: ");
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

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine($"ID: {user.Id} | Username: {user.Username} | Email: {user.Email}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine($"Trạng thái hiện tại: {user.Status} → Trạng thái mới: {newStatus}");

            Console.SetCursorPosition(borderLeft + 2, borderBottom + 2);
            Console.Write($"Xác nhận {actionText} người dùng này? (y/n): ");
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
            Console.Clear();
            ConsoleRenderingService.DrawBorder("RESET MẬT KHẨU NGƯỜI DÙNG", 80, 12);
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 12) / 4;
            int borderBottom = borderTop + 12;
            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.Write("Nhập User ID cần reset mật khẩu: ");
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
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine($"Username: {user.Username} | Email: {user.Email}");

            Console.SetCursorPosition(borderLeft + 2, borderBottom + 2);
            Console.Write("Xác nhận reset mật khẩu? (y/n): ");
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
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 15) / 4;
            int borderBottom = borderTop + 15;
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("⚠️  CẢNH BÁO: Thao tác này sẽ xóa vĩnh viễn người dùng!");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine("📋 Dữ liệu sẽ bị xóa:");
            Console.SetCursorPosition(borderLeft + 4, borderTop + 4);
            Console.WriteLine("• Thông tin tài khoản");
            Console.SetCursorPosition(borderLeft + 4, borderTop + 5);
            Console.WriteLine("• Lịch sử tham gia giải đấu");
            Console.SetCursorPosition(borderLeft + 4, borderTop + 6);
            Console.WriteLine("• Dữ liệu team");
            Console.SetCursorPosition(borderLeft + 4, borderTop + 7);
            Console.WriteLine("• Lịch sử giao dịch");

            Console.SetCursorPosition(borderLeft + 2, borderBottom + 1);
            Console.Write("Nhập User ID cần xóa: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                var currentUser = EsportsManager.UI.Services.UserSessionManager.CurrentUser;
                if (currentUser != null && currentUser.Id == userId && currentUser.Role == "Admin")
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Admin không thể xóa tài khoản của chính mình!", true, 3000);
                    return;
                }

                var userResult = await _userService.GetUserByIdAsync(userId);
                if (!userResult.IsSuccess || userResult.Data == null)
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Không tìm thấy người dùng!", true, 2000);
                    return;
                }

                var targetUser = userResult.Data;
                if (targetUser.Role == "Admin")
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Admin không thể xóa tài khoản Admin khác!", true, 3000);
                    return;
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
                Console.WriteLine($"Username: {targetUser.Username} | Email: {targetUser.Email} | Role: {targetUser.Role}");

                Console.SetCursorPosition(borderLeft + 2, borderBottom + 2);
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

    /// Gán thành tích cho người chơi
    public async Task AssignAchievementsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GÁN THÀNH TÍCH", 80, 20);

            // Get all players
            var playersResult = await _userService.GetUsersByRoleAsync("Player");
            if (!playersResult.IsSuccess || playersResult.Data == null || !playersResult.Data.Any())
            {
                int centerX = (Console.WindowWidth - 30) / 2;
                int centerY = Console.WindowHeight / 2;
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có Player nào trong hệ thống.");
                Console.ResetColor();
                Console.SetCursorPosition(centerX - 10, centerY + 2);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Display players list
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("👤 Danh sách Players:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 4;
            var players = playersResult.Data.ToList();

            for (int i = 0; i < players.Count && i < 8; i++)
            {
                var player = players[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                Console.WriteLine($"{i + 1}. ID: {player.Id} | {player.Username} | {player.FullName ?? "N/A"}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 1);
            Console.Write("Chọn Player (nhập số thứ tự, 0 để thoát): ");

            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= players.Count)
            {
                var selectedPlayer = players[selection - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 3);
                Console.WriteLine($"Đã chọn Player: {selectedPlayer.Username}");

                // Get available achievements from service
                var availableAchievements = await _achievementService.GetAvailableAchievementsAsync();

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 5);
                Console.WriteLine("🏆 Chọn loại thành tích:");

                for (int i = 0; i < availableAchievements.Count && i < 10; i++)
                {
                    Console.SetCursorPosition(borderLeft + 4, currentRow + Math.Min(players.Count, 8) + 6 + i);
                    Console.WriteLine($"{i + 1}. {availableAchievements[i]}");
                }

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 17);
                Console.Write($"Chọn thành tích (1-{availableAchievements.Count}): ");

                if (int.TryParse(Console.ReadLine(), out int achievementChoice) && achievementChoice > 0 && achievementChoice <= availableAchievements.Count)
                {
                    var selectedAchievement = availableAchievements[achievementChoice - 1];

                    Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 16);
                    Console.Write("Nhập mô tả thành tích: ");
                    var description = Console.ReadLine() ?? "";

                    Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(players.Count, 8) + 17);
                    Console.Write($"Xác nhận gán thành tích '{selectedAchievement}' cho {selectedPlayer.Username}? (y/n): ");

                    var confirmation = Console.ReadLine()?.ToLower();
                    if (confirmation == "y" || confirmation == "yes")
                    {
                        // Use IAchievementService to assign achievement
                        var success = await _achievementService.AssignAchievementToPlayerAsync(selectedPlayer.Id, selectedAchievement, description);
                        
                        if (success)
                        {
                            ConsoleRenderingService.ShowMessageBox($"✅ Đã gán thành tích '{selectedAchievement}' cho {selectedPlayer.Username}!\n📝 Mô tả: {description}", false, 3000);

                            // Log the action
                            Console.WriteLine($"\n📊 Achievement Assignment:");
                            Console.WriteLine($"   Player ID: {selectedPlayer.Id}");
                            Console.WriteLine($"   Achievement: {selectedAchievement}");
                            Console.WriteLine($"   Description: {description}");
                            Console.WriteLine($"   Assigned by: Admin");
                            Console.WriteLine($"   Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("❌ Lỗi khi gán thành tích! Vui lòng thử lại.", true, 2000);
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
            else if (selection != 0)
            {
                ConsoleRenderingService.ShowMessageBox("❌ Lựa chọn Player không hợp lệ!", true, 2000);
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

    // Thêm hàm in hướng dẫn/phím tắt dưới border
    private void PrintUserListShortcuts(int left, int y)
    {
        Console.SetCursorPosition(left, y);
        Console.Write("• Nhấn ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("F");
        Console.ResetColor();
        Console.Write(" để tìm kiếm, ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Enter");
        Console.ResetColor();
        Console.Write(" xem chi tiết, ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("C");
        Console.ResetColor();
        Console.Write(" tạo mới, ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("ESC");
        Console.ResetColor();
        Console.WriteLine(" để thoát.");
    }

    // Thêm hàm hiển thị chi tiết user
    private void ShowUserDetail(UserDto user)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder($"THÔNG TIN NGƯỜI DÙNG: {user.Username}", 60, 10);
        int left = (Console.WindowWidth - 60) / 2 + 2;
        int top = (Console.WindowHeight - 10) / 4 + 2;
        Console.SetCursorPosition(left, top);
        Console.WriteLine($"ID: {user.Id}");
        Console.SetCursorPosition(left, top + 1);
        Console.WriteLine($"Username: {user.Username}");
        Console.SetCursorPosition(left, top + 2);
        Console.WriteLine($"Email: {user.Email}");
        Console.SetCursorPosition(left, top + 3);
        Console.WriteLine($"Role: {user.Role}");
        Console.SetCursorPosition(left, top + 4);
        Console.WriteLine($"Status: {user.Status}");
        Console.SetCursorPosition(left, top + 6);
        Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
        Console.ReadKey(true);
        Console.Clear();
        // Sau khi xem chi tiết, vẽ lại trang danh sách
        // (Có thể gọi lại RenderPage nếu cần)
    }

    // Thêm hàm hiển thị chi tiết user profile
    private void ShowUserProfile(UserProfileDto user)
    {
        // Chuẩn bị dữ liệu hiển thị
        var infoLines = new List<string>
        {
            $"ID: {user.Id}",
            $"Username: {user.Username}",
            $"Email: {user.Email ?? "N/A"}",
            $"Họ tên: {user.FullName ?? "N/A"}",
            $"Số điện thoại: {user.PhoneNumber ?? "N/A"}",
            $"Role: {user.Role}",
            $"Status: {user.Status}",
            $"Ngày tạo: {user.CreatedAt:dd/MM/yyyy HH:mm}",
            $"Lần đăng nhập cuối: {(user.LastLoginAt.HasValue ? user.LastLoginAt.Value.ToString("dd/MM/yyyy HH:mm") : "N/A")}"
        };
        // Tính toán kích thước box phù hợp
        int boxWidth = Math.Max(infoLines.Max(l => l.Length) + 8, 50);
        int boxHeight = infoLines.Count + 5;
        // Vẽ border căn giữa
        Console.Clear();
        ConsoleRenderingService.DrawBorder($"THÔNG TIN NGƯỜI DÙNG: {user.Username}", boxWidth, boxHeight);
        // Lấy vị trí content bên trong border
        var (contentLeft, contentTop, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(boxWidth, boxHeight);
        // Hiển thị từng dòng thông tin
        for (int i = 0; i < infoLines.Count; i++)
        {
            Console.SetCursorPosition(contentLeft, contentTop + i);
            Console.WriteLine(infoLines[i].PadRight(contentWidth));
        }
        Console.SetCursorPosition(contentLeft, contentTop + infoLines.Count + 1);
        Console.Write("Nhấn phím bất kỳ để quay lại...".PadRight(contentWidth));
        Console.ReadKey(true);
    }
}
