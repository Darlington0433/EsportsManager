using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.MenuServices;

/// <summary>
/// AdminMenuService - Xử lý UI menu cho Admin, delegate business logic cho AdminController
/// Tách biệt UI concerns khỏi business logic với tối ưu hiệu suất
/// </summary>
public class AdminMenuService
{
    private readonly AdminController _adminController;
    
    // Caching để tối ưu hiệu suất
    private List<UserProfileDto>? _cachedUsers;
    private DateTime _lastUsersCacheTime = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public AdminMenuService(AdminController adminController)    {
        _adminController = adminController ?? throw new ArgumentNullException(nameof(adminController));
    }

    /// <summary>
    /// Hiển thị menu chính của Admin (synchronous wrapper for compatibility)
    /// </summary>
    public void ShowAdminMenu()
    {
        ShowAdminMenuAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Hiển thị menu chính của Admin
    /// </summary>
    public async Task ShowAdminMenuAsync()
    {
        await ShowMainMenuAsync();
    }

    /// <summary>
    /// Hiển thị menu chính của Admin
    /// </summary>
    public async Task ShowMainMenuAsync()
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
                    await ShowUserManagementMenuAsync();
                    break;
                case 1:
                    ShowTournamentManagementMenu();
                    break;
                case 2:
                    await ShowSystemStatsMenuAsync();
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
    }/// <summary>
    /// Menu quản lý người dùng
    /// </summary>
    private async Task ShowUserManagementMenuAsync()
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
                    await ShowAllUsersAsync();
                    break;
                case 1:
                    await SearchUsersAsync();
                    break;
                case 2:
                    await ToggleUserStatusAsync();
                    break;
                case 3:
                    await ShowUserDetailsAsync();
                    break;
                case 4:
                    await ResetUserPasswordAsync();
                    break;
                case 5:
                case -1:
                    return; // Quay lại menu chính
            }
        }
    }/// <summary>
    /// Hiển thị danh sách tất cả người dùng với phân trang
    /// </summary>
    private async Task ShowAllUsersAsync()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách người dùng...");
            
            var users = await GetUsersWithCacheAsync();
            
            if (users.Count == 0)
            {
                Console.Clear();
                ConsoleRenderingService.ShowMessageBox("Không có người dùng nào trong hệ thống.", false, 2000);
                return;
            }

            await ShowUsersWithPaginationAsync(users);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Hiển thị danh sách người dùng với phân trang và khung bao quanh
    /// </summary>
    private async Task ShowUsersWithPaginationAsync(List<UserProfileDto> users)
    {
        const int usersPerPage = 8;  // Số người dùng mỗi trang
        const int frameWidth = 100;  // Thu nhỏ khung cho vừa màn hình
        const int frameHeight = 16;  // Chiều cao vừa phải
        
        int currentPage = 0;
        int totalPages = (int)Math.Ceiling((double)users.Count / usersPerPage);

        while (true)
        {
            Console.Clear();
            
            // Tính vị trí để center khung
            int frameLeft = Math.Max(0, (Console.WindowWidth - frameWidth) / 2);
            int frameTop = Math.Max(0, (Console.WindowHeight - frameHeight) / 2);
            
            // Vẽ khung
            string title = $"DANH SÁCH NGƯỜI DÙNG - QUẢN LÝ HỆ THỐNG";
            ConsoleRenderingService.DrawBorder(title, frameWidth, frameHeight);
              
            // Vị trí nội dung bên trong khung
            int contentLeft = frameLeft + 2;  // Margin nhỏ hơn
            int contentTop = frameTop + 2;    // Margin nhỏ hơn
            int contentWidth = frameWidth - 4;
            
            // Table Header tiếng Việt - ESPORTS THEME (thu gọn)
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.ForegroundColor = ConsoleColor.Yellow; // Highlight header
            Console.Write($"{"ID",-4} {"Tên Người Dùng",-16} {"Email",-22} {"Vai Trò",-10} {"Trạng Thái",-10} {"Đội",-12} {"Ngày TG",-10}");
            Console.ResetColor();
              
            // Separator line
            Console.SetCursorPosition(contentLeft, contentTop + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('─', contentWidth - 5)); // Thu gọn separator
            Console.ResetColor();
            
            // Data rows - ESPORTS DATA
            var pageUsers = users.Skip(currentPage * usersPerPage).Take(usersPerPage);
            int rowIndex = 0;
            
            foreach (var user in pageUsers)
            {
                Console.SetCursorPosition(contentLeft, contentTop + 2 + rowIndex);
                
                // Dữ liệu Esports tiếng Việt (thu gọn)
                string trangThai = "Hoạt động";
                string doi = user.Role == "Admin" ? "N/A" : 
                            user.Role == "Player" ? $"Team{user.Id % 5 + 1}" : "Khán giả";
                string ngayThamGia = DateTime.Now.AddDays(-(user.Id * 30)).ToString("dd/MM/yy"); // Format ngắn
                
                // Format text vừa với cột (thu gọn)
                string tenNguoiDung = user.Username?.Length > 14 ? user.Username.Substring(0, 13) + "…" : user.Username ?? "";
                string emailRutGon = user.Email?.Length > 20 ? user.Email.Substring(0, 19) + "…" : user.Email ?? "";
                string vaiTro = user.Role == "Admin" ? "Quản trị" : user.Role == "Player" ? "Tuyển thủ" : "Khán giả";
                
                // Màu sắc theo vai trò
                if (user.Role == "Admin")
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (user.Role == "Player")
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.White;
                
                Console.Write($"{user.Id,-4} {tenNguoiDung,-16} {emailRutGon,-22} {vaiTro,-10} {trangThai,-10} {doi,-12} {ngayThamGia,-10}");
                Console.ResetColor();
                rowIndex++;
            }
            
            // Pagination info ở cuối giống mẫu
            Console.SetCursorPosition(contentLeft + contentWidth/2 - 10, frameTop + frameHeight - 5);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"● [{currentPage + 1}/{totalPages}] ●");
            Console.ResetColor();
            
            // Instructions tiếng Việt - ESPORTS THEME (thu gọn)
            Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("● Nhấn 'MŨI TÊN TRÁI/PHẢI' để chuyển trang");
            Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 2);
            Console.Write("● 'F' tìm kiếm, 'D' chi tiết, 'T' quản lý Đội, 'ESC' thoát");
            Console.ResetColor();
            
            // Xử lý input giống mẫu
            var key = Console.ReadKey(true);
            
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (currentPage > 0) currentPage--;
                    break;
                    
                case ConsoleKey.RightArrow:
                    if (currentPage < totalPages - 1) currentPage++;
                    break;
                    
                case ConsoleKey.F:
                    await SearchUsersAsync();
                    return;
                    
                case ConsoleKey.D:
                    await ShowUserDetailsAsync();
                    break;
                
                case ConsoleKey.T:
                    // Quản lý Team - Esports context
                    await ShowTeamManagementMenuAsync();
                    break;
                    
                case ConsoleKey.R:
                    // Refresh data
                    try
                    {
                        ConsoleRenderingService.ShowLoadingMessage("Đang tải lại dữ liệu...");
                        users = await GetUsersWithCacheAsync(true);
                        totalPages = (int)Math.Ceiling((double)users.Count / usersPerPage);
                        if (currentPage >= totalPages) currentPage = Math.Max(0, totalPages - 1);
                    }
                    catch (Exception ex)
                    {
                        ConsoleRenderingService.ShowMessageBox($"Lỗi refresh: {ex.Message}", true, 2000);
                    }
                    break;
                    
                case ConsoleKey.Escape:
                    return;
            }
        }
    }    /// <summary>
    /// Tìm kiếm người dùng với giao diện cải tiến
    /// </summary>
    private async Task SearchUsersAsync()
    {
        try
        {
            Console.Clear();
            
            const int frameWidth = 60;
            const int frameHeight = 8;
            
            // Tính vị trí khung TRƯỚC KHI vẽ
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung TRƯỚC
            int contentLeft = frameLeft + 2;
            int contentTop = frameTop + 2;
            
            // Vẽ khung SAU KHI tính toán vị trí
            ConsoleRenderingService.DrawBorder("TÌM KIẾM NGƯỜI DÙNG", frameWidth, frameHeight);
              
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.Write("Nhập từ khóa tìm kiếm (tên người dùng hoặc email):");
            
            Console.SetCursorPosition(contentLeft, contentTop + 2);
            Console.Write("Từ khóa: ");
            string searchTerm = Console.ReadLine() ?? "";
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ConsoleRenderingService.ShowMessageBox("Từ khóa tìm kiếm không được rỗng!", true, 2000);
                return;
            }

            ConsoleRenderingService.ShowLoadingMessage("Đang tìm kiếm...");
            
            var results = await _adminController.SearchUsersAsync(searchTerm);
            
            if (results.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox($"Không tìm thấy người dùng nào với từ khóa: '{searchTerm}'", false, 3000);
            }
            else
            {
                // Sử dụng lại hệ thống phân trang cho kết quả tìm kiếm
                await ShowSearchResultsWithPaginationAsync(results, searchTerm);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi tìm kiếm: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Hiển thị kết quả tìm kiếm với phân trang
    /// </summary>
    private async Task ShowSearchResultsWithPaginationAsync(List<UserProfileDto> searchResults, string searchTerm)
    {
        const int usersPerPage = 8;
        const int frameWidth = 100;  // Cùng kích thước với main table
        const int frameHeight = 16;
        
        int currentPage = 0;
        int totalPages = (int)Math.Ceiling((double)searchResults.Count / usersPerPage);
        
        while (true)
        {
            Console.Clear();
            
            // Tính vị trí khung TRƯỚC KHI vẽ
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung bên trong khung TRƯỚC
            int contentLeft = frameLeft + 2;
            int contentTop = frameTop + 2;
            int contentWidth = frameWidth - 4;
            
            // Vẽ khung với tiêu đề tìm kiếm SAU KHI tính toán
            string shortSearchTerm = searchTerm.Length > 15 ? searchTerm.Substring(0, 12) + "..." : searchTerm;
            string title = $"KẾT QUẢ TÌM KIẾM: '{shortSearchTerm}' - Trang {currentPage + 1}/{totalPages} ({searchResults.Count} kết quả)";
            ConsoleRenderingService.DrawBorder(title, frameWidth, frameHeight);
            
            // Header bảng tìm kiếm tiếng Việt (thu gọn)
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.Write($"{"ID",-4} {"Tên Người Dùng",-16} {"Email",-22} {"Vai Trò",-10} {"Trạng Thái",-10} {"Khớp",-8}");
            
            Console.SetCursorPosition(contentLeft, contentTop + 1);
            Console.Write(new string('─', contentWidth - 5));
            
            // Lấy users cho trang hiện tại
            var pageResults = searchResults.Skip(currentPage * usersPerPage).Take(usersPerPage);
            
            int rowIndex = 0;
            foreach (var user in pageResults)
            {
                Console.SetCursorPosition(contentLeft, contentTop + 2 + rowIndex);
                
                string trangThai = "Hoạt động";
                
                // Loại khớp tiếng Việt
                string loaiKhop = "";
                if (user.Username?.ToLower().Contains(searchTerm.ToLower()) == true)
                    loaiKhop = "Tên người dùng";
                else if (user.Email?.ToLower().Contains(searchTerm.ToLower()) == true)
                    loaiKhop = "Email";
                else
                    loaiKhop = "Khác";
                  
                // Format text (thu gọn)
                string tenNguoiDung = user.Username?.Length > 14 ? user.Username.Substring(0, 13) + "…" : user.Username ?? "";
                string emailRutGon = user.Email?.Length > 20 ? user.Email.Substring(0, 19) + "…" : user.Email ?? "";
                string vaiTro = user.Role == "Admin" ? "Quản trị" : user.Role == "Player" ? "Tuyển thủ" : "Khán giả";
                
                Console.Write($"{user.Id,-4} {tenNguoiDung,-16} {emailRutGon,-22} {vaiTro,-10} {trangThai,-10} {loaiKhop,-8}");
                rowIndex++;
            }
              
            // Hiển thị hướng dẫn điều hướng (thu gọn)
            Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 4);
            Console.Write(new string('─', contentWidth - 5));
            
            Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
            string navigation = "";
            if (totalPages > 1)
            {
                if (currentPage > 0) navigation += "[← Trước] ";
                if (currentPage < totalPages - 1) navigation += "[Sau →] ";
            }
            navigation += "[T]ìm kiếm mới [ESC] Quay lại";
            Console.Write(navigation);
            
            // Xử lý input
            var key = Console.ReadKey(true);
            
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.P:
                    if (currentPage > 0) currentPage--;
                    break;
                    
                case ConsoleKey.RightArrow:
                case ConsoleKey.PageDown:
                    if (currentPage < totalPages - 1) currentPage++;
                    break;
                      
                case ConsoleKey.T:
                    await SearchUsersAsync(); // Tìm kiếm mới
                    return;
                    
                case ConsoleKey.Escape:
                    return;
            }
        }
    }    /// <summary>
    /// Thay đổi trạng thái người dùng
    /// </summary>
    private async Task ToggleUserStatusAsync()
    {
        try
        {
            Console.Clear();
            
            const int frameWidth = 70;
            const int frameHeight = 10;
            
            // Tính vị trí khung
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung
            int contentLeft = frameLeft + 3;
            int contentTop = frameTop + 3;
            
            // Vẽ khung
            ConsoleRenderingService.DrawBorder("THAY ĐỔI TRẠNG THÁI NGƯỜI DÙNG", frameWidth, frameHeight);
            
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.Write("Nhập User ID cần thay đổi trạng thái: ");
            
            Console.SetCursorPosition(contentLeft, contentTop + 2);
            Console.Write("User ID: ");
              string input = Console.ReadLine() ?? "";
            
            if (TryGetValidUserId(input, out int userId, out string errorMessage))
            {                try
                {
                    var result = await ExecuteWithTimeoutAsync(
                        _adminController.ToggleUserStatusAsync(userId),
                        "Đang cập nhật trạng thái...");
                    
                    if (result.Success)
                    {
                        ConsoleRenderingService.ShowMessageBox("Thay đổi trạng thái thành công!", false, 2000);
                        
                        // Invalidate cache để đảm bảo dữ liệu mới nhất
                        InvalidateUserCache();
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Không thể thay đổi trạng thái! Có thể user không tồn tại.", true, 2000);
                    }
                }
                catch (TimeoutException ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"Timeout: {ex.Message}", true, 3000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox(errorMessage, true, 2000);
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
    private async Task ResetUserPasswordAsync()
    {
        try
        {
            Console.Clear();
            
            const int frameWidth = 70;
            const int frameHeight = 10;
            
            // Tính vị trí khung
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung
            int contentLeft = frameLeft + 3;
            int contentTop = frameTop + 3;
            
            // Vẽ khung
            ConsoleRenderingService.DrawBorder("RESET MẬT KHẨU NGƯỜI DÙNG", frameWidth, frameHeight);
            
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.Write("Nhập User ID cần reset mật khẩu: ");
            
            Console.SetCursorPosition(contentLeft, contentTop + 2);
            Console.Write("User ID: ");
              string input = Console.ReadLine() ?? "";
            
            if (TryGetValidUserId(input, out int userId, out string errorMessage))
            {                try
                {
                    var result = await ExecuteWithTimeoutAsync(
                        _adminController.ResetUserPasswordAsync(userId),
                        "Đang reset mật khẩu...");
                    
                    // Invalidate cache sau khi reset password
                    InvalidateUserCache();
                    
                    ConsoleRenderingService.ShowMessageBox($"Reset thành công! Mật khẩu mới: {result.NewPassword}", false, 5000);
                }
                catch (TimeoutException ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"Timeout: {ex.Message}", true, 3000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox(errorMessage, true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }/// <summary>
    /// Hiển thị thông tin chi tiết người dùng
    /// </summary>
    private async Task ShowUserDetailsAsync()
    {
        try
        {
            Console.Clear();
            
            const int frameWidth = 80;
            const int frameHeight = 12;
            
            // Tính vị trí khung
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung
            int contentLeft = frameLeft + 3;
            int contentTop = frameTop + 3;
            
            // Vẽ khung
            ConsoleRenderingService.DrawBorder("THÔNG TIN CHI TIẾT NGƯỜI DÙNG", frameWidth, frameHeight);
            
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.Write("Nhập User ID cần xem chi tiết: ");
            
            Console.SetCursorPosition(contentLeft, contentTop + 2);
            Console.Write("User ID: ");
              string input = Console.ReadLine() ?? "";
              if (TryGetValidUserId(input, out int userId, out string errorMessage))
            {
                try
                {                    // Sử dụng GetAllUsersAsync và tìm theo ID vì GetUserByIdAsync chưa có
                    var allUsersResult = await ExecuteWithTimeoutAsync(
                        _adminController.GetAllUsersAsync(),
                        "🔍 Đang tải thông tin chi tiết...");
                    
                    var userDetails = allUsersResult.Items.FirstOrDefault(u => u.Id == userId);
                    
                    if (userDetails != null)
                    {
                        DisplayUserDetails(userDetails, contentLeft, contentTop);
                    }
                    else
                    {
                        Console.SetCursorPosition(contentLeft, contentTop + 4);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("❌ Không tìm thấy người dùng với ID này");
                        Console.ResetColor();
                    }
                }
                catch (TimeoutException ex)
                {
                    Console.SetCursorPosition(contentLeft, contentTop + 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"⏰ Timeout: {ex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.SetCursorPosition(contentLeft, contentTop + 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"❌ Lỗi: {ex.Message}");
                    Console.ResetColor();
                }
                
                Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Nhấn phím bất kỳ để quay lại...");
                Console.ResetColor();
                
                Console.ReadKey(true);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox(errorMessage, true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }/// <summary>
    /// Menu quản lý giải đấu
    /// </summary>
    private void ShowTournamentManagementMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 10;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("QUẢN LÝ GIẢI ĐẤU/TRẬN ĐẤU", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("🏆 TOURNAMENT MANAGEMENT SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Các tính năng sẽ có:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Tạo và quản lý giải đấu");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Lập lịch trận đấu");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Quản lý kết quả");
        
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        
        Console.ReadKey(true);
    }    /// <summary>
    /// Menu thống kê hệ thống
    /// </summary>
    private async Task ShowSystemStatsMenuAsync()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thống kê hệ thống...");
            
            var stats = await _adminController.GetSystemStatsAsync();
            
            Console.Clear();
            
            const int frameWidth = 80;
            const int frameHeight = 15;
            
            // Tính vị trí khung
            int frameLeft = (Console.WindowWidth - frameWidth) / 2;
            int frameTop = (Console.WindowHeight - frameHeight) / 2;
            
            // Tính vị trí nội dung
            int contentLeft = frameLeft + 3;
            int contentTop = frameTop + 3;
            
            // Vẽ khung
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", frameWidth, frameHeight);
            
            // Hiển thị thống kê bên trong khung
            Console.SetCursorPosition(contentLeft, contentTop);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("📊 THỐNG KÊ TỔNG QUAN HỆ THỐNG ESPORTS");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 2);
            Console.Write($"� Tổng số người dùng: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{stats.TotalUsers}");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 3);
            Console.Write($"✅ Số người dùng hoạt động: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{stats.ActiveUsers}");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 4);
            Console.Write($"🏆 Tổng số giải đấu: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{stats.TotalTournaments}");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 5);
            Console.Write($"🔥 Số giải đấu đang diễn ra: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{stats.ActiveTournaments}");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 6);
            Console.Write($"👥 Tổng số team: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{stats.TotalTeams}");
            Console.ResetColor();
            
            Console.SetCursorPosition(contentLeft, contentTop + 7);
            Console.Write($"💰 Tổng doanh thu: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{stats.TotalRevenue:N0} VND");
            Console.ResetColor();
            
            // Instructions
            Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Nhấn phím bất kỳ để quay lại...");
            Console.ResetColor();
            
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Menu báo cáo donation
    /// </summary>
    private void ShowDonationReportsMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 10;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("BÁO CÁO DONATION", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("💰 DONATION REPORT SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Các tính năng sẽ có:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Theo dõi donation cho Player/Team");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Báo cáo doanh thu");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Thống kê donation theo thời gian");
        
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        
        Console.ReadKey(true);
    }    /// <summary>
    /// Menu kết quả voting
    /// </summary>
    private void ShowVotingResultsMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 10;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("🗳️ VOTING RESULTS SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Các tính năng sẽ có:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Xem kết quả vote cho Team/Player");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Thống kê popularity ranking");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Quản lý polls và surveys");
        
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        
        Console.ReadKey(true);
    }    /// <summary>
    /// Menu quản lý feedback
    /// </summary>
    private void ShowFeedbackManagementMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 10;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("QUẢN LÝ FEEDBACK", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("📝 FEEDBACK MANAGEMENT SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Các tính năng sẽ có:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Xem và trả lời feedback từ người dùng");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Phân loại feedback theo mức độ ưu tiên");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Thống kê satisfaction rating");
        
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");
        Console.ResetColor();
        
        Console.ReadKey(true);
    }    /// <summary>
    /// Menu cài đặt hệ thống
    /// </summary>
    private void ShowSystemSettingsMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 12;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("CÀI ĐẶT HỆ THỐNG", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("⚙️ SYSTEM SETTINGS PANEL");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Các tính năng sẽ có:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Cấu hình máy chủ game");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Thiết lập thông số esports");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Quản lý cơ sở dữ liệu");
        Console.SetCursorPosition(contentLeft, contentTop + 8);
        Console.Write("• Backup và restore");
        
        // Hiển thị phím điều hướng ở cuối khung
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("ESC: Quay lại   🎮 Gaming Settings Dashboard");
        Console.ResetColor();
        
        // Chờ phím ESC
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
        }        while (key.Key != ConsoleKey.Escape);
    }

    /// <summary>
    /// Menu xóa người dùng
    /// </summary>
    private void ShowUserDeletionMenu()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 12;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("XÓA NGƯỜI DÙNG", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("🗑️ USER DELETION SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Tính năng sẽ bao gồm:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Xác nhận xóa tài khoản");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Backup dữ liệu người dùng");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Xóa vĩnh viễn/tạm ngưng");
        Console.SetCursorPosition(contentLeft, contentTop + 8);
        Console.Write("• Bảo mật cao cấp");
        
        // Hiển thị phím điều hướng ở cuối khung
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("ESC: Quay lại   ⚡ Safety First Policy");
        Console.ResetColor();
        
        // Chờ phím ESC
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
        }
        while (key.Key != ConsoleKey.Escape);
    }    /// <summary>
    /// Menu quản lý đội (Team Management)
    /// </summary>
    private async Task ShowTeamManagementMenuAsync()
    {
        Console.Clear();
        
        const int frameWidth = 80;
        const int frameHeight = 12;
        
        // Tính vị trí khung
        int frameLeft = (Console.WindowWidth - frameWidth) / 2;
        int frameTop = (Console.WindowHeight - frameHeight) / 2;
        
        // Tính vị trí nội dung
        int contentLeft = frameLeft + 3;
        int contentTop = frameTop + 3;
        
        // Vẽ khung
        ConsoleRenderingService.DrawBorder("QUẢN LÝ ĐỘI/TEAM", frameWidth, frameHeight);
        
        Console.SetCursorPosition(contentLeft, contentTop);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("👥 TEAM MANAGEMENT SYSTEM");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("⚠️ Chức năng đang được phát triển");
        Console.ResetColor();
        
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write("Tính năng sẽ bao gồm:");
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write("• Tạo và quản lý team esports");
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        Console.Write("• Thêm/xóa thành viên team");
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        Console.Write("• Phân công vai trò (Captain, Support...)");
        Console.SetCursorPosition(contentLeft, contentTop + 8);
        Console.Write("• Thống kê thành tích team");
        
        // Hiển thị phím điều hướng ở cuối khung
        Console.SetCursorPosition(contentLeft, frameTop + frameHeight - 2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("ESC: Quay lại   🏆 Team Management Portal");
        Console.ResetColor();
        
        // Chờ phím ESC
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
        }
        while (key.Key != ConsoleKey.Escape);
        
        await Task.CompletedTask; // Async compliance
    }
    
    /// <summary>
    /// Lấy danh sách người dùng với caching để tối ưu hiệu suất
    /// </summary>
    private async Task<List<UserProfileDto>> GetUsersWithCacheAsync(bool forceRefresh = false)
    {        if (forceRefresh || _cachedUsers == null || DateTime.Now - _lastUsersCacheTime > _cacheExpiration)
        {
            try
            {
                var result = await _adminController.GetAllUsersAsync();
                _cachedUsers = result.Items.ToList();
                _lastUsersCacheTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                // Nếu có lỗi và có cache cũ, dùng cache cũ
                if (_cachedUsers != null)
                {
                    ConsoleRenderingService.ShowMessageBox($"Sử dụng dữ liệu cache do lỗi: {ex.Message}", true, 2000);
                    return _cachedUsers;
                }
                throw; // Re-throw nếu không có cache
            }
        }
        
        return _cachedUsers ?? new List<UserProfileDto>();
    }
    
    /// <summary>
    /// Invalidate cache khi có thay đổi dữ liệu
    /// </summary>
    private void InvalidateUserCache()
    {
        _cachedUsers = null;
        _lastUsersCacheTime = DateTime.MinValue;
    }
      /// <summary>
    /// Hiển thị loading với timeout để tránh treo ứng dụng
    /// </summary>
    private async Task<T> ExecuteWithTimeoutAsync<T>(Task<T> task, string loadingMessage, int timeoutMs = 30000)
    {
        ConsoleRenderingService.ShowLoadingMessage(loadingMessage);
        
        try
        {
            using var cts = new System.Threading.CancellationTokenSource(timeoutMs);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeoutMs, cts.Token));
            
            if (completedTask == task)
            {
                return await task; // Task hoàn thành thành công
            }
            else
            {
                throw new TimeoutException($"Thao tác quá thời gian chờ ({timeoutMs/1000}s)");
            }
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Thao tác quá thời gian chờ ({timeoutMs/1000}s)");
        }
    }
    
    /// <summary>
    /// Validate User ID input với error handling tốt hơn
    /// </summary>
    private bool TryGetValidUserId(string input, out int userId, out string errorMessage)
    {
        userId = 0;
        errorMessage = "";
        
        if (string.IsNullOrWhiteSpace(input))
        {
            errorMessage = "User ID không được để trống!";
            return false;
        }
        
        if (!int.TryParse(input.Trim(), out userId))
        {
            errorMessage = "User ID phải là số nguyên!";
            return false;
        }
        
        if (userId <= 0)
        {
            errorMessage = "User ID phải lớn hơn 0!";
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Helper method để hiển thị thông tin user một cách đẹp mắt
    /// </summary>
    private void DisplayUserDetails(UserProfileDto user, int contentLeft, int contentTop)
    {
        // Clear loading message
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.Write(new string(' ', 60));
        
        // Display user info with Vietnamese esports context
        Console.SetCursorPosition(contentLeft, contentTop + 4);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"👤 Tên: {user.Username}");
        
        Console.SetCursorPosition(contentLeft, contentTop + 5);
        Console.Write($"📧 Email: {user.Email}");
        
        Console.SetCursorPosition(contentLeft, contentTop + 6);
        var joinDate = DateTime.Now.AddDays(-(user.Id * 30));
        Console.Write($"📅 Ngày tham gia: {joinDate:dd/MM/yyyy}");
        
        Console.SetCursorPosition(contentLeft, contentTop + 7);
        string roleVietnamese = user.Role switch
        {
            "Admin" => "Quản trị viên",
            "Player" => "Tuyển thủ Esports",
            "Viewer" => "Khán giả",
            _ => "Không xác định"
        };
        Console.Write($"🎮 Vai trò: {roleVietnamese}");
        
        Console.SetCursorPosition(contentLeft, contentTop + 8);
        Console.ForegroundColor = ConsoleColor.Green; // Mock active status
        Console.Write($"🔄 Trạng thái: Hoạt động");
          // Additional esports info for players
        if (user.Role == "Player")
        {
            Console.SetCursorPosition(contentLeft, contentTop + 9);
            Console.ForegroundColor = ConsoleColor.Cyan;
            string team = $"Team{user.Id % 5 + 1}";
            Console.Write($"🏆 Team: {team}");
        }
        
        Console.ResetColor();
    }
}
