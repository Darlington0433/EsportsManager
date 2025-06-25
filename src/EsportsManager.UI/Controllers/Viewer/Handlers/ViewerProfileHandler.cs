using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

namespace EsportsManager.UI.Controllers.Viewer.Handlers
{
    /// <summary>
    /// Handler cho quản lý thông tin cá nhân của Viewer
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class ViewerProfileHandler : IViewerProfileHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly IUserService _userService;

        public ViewerProfileHandler(
            UserProfileDto currentUser,
            IUserService userService)
        {
            _currentUser = currentUser;
            _userService = userService;
        }

        public async Task HandleViewProfileAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THÔNG TIN CÁ NHÂN", 80, 15);

                var userInfo = await _userService.GetUserByIdAsync(_currentUser.Id);

                if (userInfo.IsSuccess)
                {
                    var user = userInfo.Data;
                    Console.WriteLine($"👤 Tên đăng nhập: {user.Username}");
                    Console.WriteLine($"📧 Email: {user.Email ?? "Chưa cập nhật"}");
                    Console.WriteLine($"👨 Họ tên: {user.FullName ?? "Chưa cập nhật"}");
                    Console.WriteLine($"📱 Số điện thoại: {user.PhoneNumber ?? "Chưa cập nhật"}");
                    Console.WriteLine($"🎭 Vai trò: {user.Role}");
                    Console.WriteLine($"📅 Ngày tạo tài khoản: {user.CreatedAt:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"🕐 Lần đăng nhập cuối: {user.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");
                    Console.WriteLine($"⭐ Trạng thái: {user.Status}");
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin người dùng!", false, 2000);
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        public async Task HandleUpdateProfileAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 15);

                // Lấy thông tin hiện tại
                var currentInfo = await _userService.GetUserByIdAsync(_currentUser.Id);
                if (!currentInfo.IsSuccess)
                {
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin hiện tại!", false, 2000);
                    return;
                }

                var user = currentInfo.Data;
                Console.WriteLine("Thông tin hiện tại:");
                Console.WriteLine($"📧 Email: {user.Email ?? "Chưa có"}");
                Console.WriteLine($"👨 Họ tên: {user.FullName ?? "Chưa có"}");
                Console.WriteLine($"📱 Số điện thoại: {user.PhoneNumber ?? "Chưa có"}");
                Console.WriteLine();

                Console.WriteLine("Nhập thông tin mới (Enter để bỏ qua):");

                Console.Write("Email mới: ");
                string? newEmail = Console.ReadLine()?.Trim();

                Console.Write("Họ tên mới: ");
                string? newFullName = Console.ReadLine()?.Trim();

                Console.Write("Số điện thoại mới: ");
                string? newPhoneNumber = Console.ReadLine()?.Trim();

                // Kiểm tra xem có thông tin nào thay đổi không
                if (string.IsNullOrEmpty(newEmail) && string.IsNullOrEmpty(newFullName) && string.IsNullOrEmpty(newPhoneNumber))
                {
                    ConsoleRenderingService.ShowMessageBox("Không có thông tin nào được thay đổi!", false, 2000);
                    return;
                }

                // Tạo DTO để cập nhật
                var userDto = new UserDto
                {
                    Id = _currentUser.Id,
                    Username = _currentUser.Username,
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : user?.Email,
                    FullName = !string.IsNullOrEmpty(newFullName) ? newFullName : user?.FullName,
                    Role = _currentUser.Role,
                    PhoneNumber = !string.IsNullOrEmpty(newPhoneNumber) ? newPhoneNumber : user?.PhoneNumber
                };

                var result = await _userService.UpdateUserProfileAsync(_currentUser.Id, userDto);

                if (result.IsSuccess)
                {
                    ConsoleRenderingService.ShowMessageBox("✅ Cập nhật thông tin thành công!", true, 2500);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox($"❌ Cập nhật thất bại: {result.ErrorMessage}", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        public async Task HandleForgotPasswordAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("ĐỔI MẬT KHẨU", 80, 15);

                Console.WriteLine("🔐 Thay đổi mật khẩu");
                Console.WriteLine("─".PadRight(50, '─'));

                Console.Write("Mật khẩu hiện tại: ");
                string currentPassword = ReadPassword();

                Console.Write("Mật khẩu mới: ");
                string newPassword = ReadPassword();

                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                {
                    ConsoleRenderingService.ShowMessageBox("Mật khẩu mới phải có ít nhất 6 ký tự!", false, 2000);
                    return;
                }

                Console.Write("Xác nhận mật khẩu mới: ");
                string confirmPassword = ReadPassword();

                if (newPassword != confirmPassword)
                {
                    ConsoleRenderingService.ShowMessageBox("Mật khẩu xác nhận không khớp!", false, 2000);
                    return;
                }

                var passwordDto = new UpdatePasswordDto
                {
                    UserId = _currentUser.Id,
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var result = await _userService.UpdatePasswordAsync(passwordDto);

                if (result.IsSuccess)
                {
                    ConsoleRenderingService.ShowMessageBox("✅ Đổi mật khẩu thành công!", true, 2500);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox($"❌ Đổi mật khẩu thất bại: {result.ErrorMessage}", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
