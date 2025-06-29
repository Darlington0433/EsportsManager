using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Player.Handlers
{
    /// <summary>
    /// Handler cho quản lý thông tin cá nhân của player
    /// Single Responsibility: Chỉ lo việc xem/cập nhật profile
    /// </summary>
    public class PlayerProfileHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly IUserService _userService;

        public PlayerProfileHandler(
            UserProfileDto currentUser,
            IUserService userService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task HandleViewPersonalInfoAsync()
        {
            try
            {
                var userInfo = await GetPersonalInfoAsync();

                if (userInfo == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin cá nhân!", true, 2000);
                    return;
                }

                Console.Clear();
                ConsoleRenderingService.DrawBorder("THÔNG TIN CÁ NHÂN", 80, 15);

                // Tính vị trí để hiển thị data bên trong border
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.WriteLine($"👤 ID: {userInfo.Id}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine($"📧 Username: {userInfo.Username}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.WriteLine($"✉️ Email: {userInfo.Email ?? "Chưa cập nhật"}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                Console.WriteLine($"🎭 Role: {userInfo.Role}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine($"📅 Ngày tạo: {userInfo.CreatedAt:dd/MM/yyyy HH:mm}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.WriteLine($"🕐 Lần đăng nhập cuối: {userInfo.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");

                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        public async Task HandleUpdatePersonalInfoAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 15);

                var currentInfo = await GetPersonalInfoAsync();
                if (currentInfo == null)
                {
                    int borderLeft = (Console.WindowWidth - 80) / 2;
                    int borderTop = (Console.WindowHeight - 15) / 4;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin hiện tại!", true, 2000);
                    return;
                }

                int borderLeft2 = (Console.WindowWidth - 80) / 2;
                int borderTop2 = (Console.WindowHeight - 15) / 4;
                int cursorY = borderTop2 + 2;
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine("Thông tin hiện tại:");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine($"Email: {currentInfo.Email ?? "Chưa có"}");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine($"Họ tên: {currentInfo.FullName ?? "Chưa có"}");
                cursorY++;
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.Write("Email mới (Enter để bỏ qua): ");
                string newEmail = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Họ tên mới (Enter để bỏ qua): ");
                string newFullName = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newFullName))
                {
                    var updateDto = new UpdateUserDto
                    {
                        Email = !string.IsNullOrEmpty(newEmail) ? newEmail : currentInfo?.Email,
                        FullName = !string.IsNullOrEmpty(newFullName) ? newFullName : currentInfo?.FullName
                    };

                    bool success = await UpdatePersonalInfoAsync(updateDto);

                    Console.SetCursorPosition(borderLeft2 + 2, borderTop2 + 12);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox("Cập nhật thông tin thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Cập nhật thất bại!", true, 2000);
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft2 + 2, borderTop2 + 12);
                    ConsoleRenderingService.ShowMessageBox("Không có thông tin nào được thay đổi!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        /// <summary>
        /// Xử lý thay đổi mật khẩu của player
        /// </summary>
        public async Task HandleChangePasswordAsync()
        {
            try
            {
                Console.WriteLine("\n=== ĐỔI MẬT KHẨU ===");

                // Nhập mật khẩu hiện tại
                Console.Write("Mật khẩu hiện tại: ");
                string currentPassword = UnifiedInputService.ReadPassword() ?? "";

                if (string.IsNullOrEmpty(currentPassword))
                {
                    Console.WriteLine("Mật khẩu hiện tại không được để trống!");
                    return;
                }

                // Nhập mật khẩu mới
                Console.Write("Mật khẩu mới: ");
                string newPassword = UnifiedInputService.ReadPassword() ?? "";

                if (string.IsNullOrEmpty(newPassword))
                {
                    Console.WriteLine("Mật khẩu mới không được để trống!");
                    return;
                }

                // Xác nhận mật khẩu mới
                Console.Write("Xác nhận mật khẩu mới: ");
                string confirmPassword = UnifiedInputService.ReadPassword() ?? "";

                if (newPassword != confirmPassword)
                {
                    Console.WriteLine("Mật khẩu xác nhận không khớp!");
                    return;
                }

                // Tạo DTO để đổi mật khẩu
                var updatePasswordDto = new UpdatePasswordDto
                {
                    UserId = _currentUser.Id,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                // Gọi service để đổi mật khẩu
                var result = await _userService.UpdatePasswordAsync(_currentUser.Id, updatePasswordDto);

                if (result.IsSuccess)
                {
                    Console.WriteLine("Đổi mật khẩu thành công!");
                }
                else
                {
                    Console.WriteLine($"Đổi mật khẩu thất bại: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }

            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        private async Task<UserDto?> GetPersonalInfoAsync()
        {
            var result = await _userService.GetUserByIdAsync(_currentUser.Id);
            return result.IsSuccess ? result.Data : null;
        }

        private async Task<bool> UpdatePersonalInfoAsync(UpdateUserDto updateDto)
        {
            // Convert UpdateUserDto to UserDto for the service call
            var userDto = new UserDto
            {
                Id = _currentUser.Id,
                Username = _currentUser.Username,
                Email = updateDto.Email ?? _currentUser.Email,
                FullName = updateDto.FullName,
                Role = _currentUser.Role
            };

            var result = await _userService.UpdateUserProfileAsync(_currentUser.Id, userDto);
            return result.IsSuccess;
        }
    }
}
