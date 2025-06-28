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

                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;

                if (userInfo.IsSuccess && userInfo.Data != null)
                {
                    var user = userInfo.Data;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    Console.WriteLine($"👤 Tên đăng nhập: {user?.Username ?? "Chưa cập nhật"}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                    Console.WriteLine($"📧 Email: {user?.Email ?? "Chưa cập nhật"}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                    Console.WriteLine($"👨 Họ tên: {user?.FullName ?? "Chưa cập nhật"}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                    Console.WriteLine($"📱 Số điện thoại: {user?.PhoneNumber ?? "Chưa cập nhật"}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                    Console.WriteLine($"🎭 Vai trò: {user?.Role ?? "Chưa cập nhật"}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                    Console.WriteLine($"📅 Ngày tạo tài khoản: {(user?.CreatedAt != null ? user.CreatedAt.ToString("dd/MM/yyyy HH:mm") : "Chưa có")}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 8);
                    Console.WriteLine($"🕐 Lần đăng nhập cuối: {(user?.LastLoginAt != null ? user.LastLoginAt.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa có")}");
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
                    Console.WriteLine($"⭐ Trạng thái: {user?.Status ?? "Chưa cập nhật"}");
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Không thể tải thông tin người dùng!");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", true, 2000);
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
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;
                if (!currentInfo.IsSuccess || currentInfo.Data == null)
                {
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Không thể tải thông tin hiện tại!");
                    Console.ResetColor();
                    return;
                }

                var user = currentInfo.Data;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.WriteLine("Thông tin hiện tại:");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine($"📧 Email: {user?.Email ?? "Chưa có"}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.WriteLine($"👨 Họ tên: {user?.FullName ?? "Chưa có"}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                Console.WriteLine($"📱 Số điện thoại: {user?.PhoneNumber ?? "Chưa có"}");

                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.WriteLine("Nhập thông tin mới (Enter để bỏ qua):");

                Console.SetCursorPosition(borderLeft + 2, borderTop + 8);
                Console.Write("Email mới: ");
                string? newEmail = Console.ReadLine()?.Trim();

                Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
                Console.Write("Họ tên mới: ");
                string? newFullName = Console.ReadLine()?.Trim();

                Console.SetCursorPosition(borderLeft + 2, borderTop + 10);
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
                    ConsoleRenderingService.ShowMessageBox($"❌ Cập nhật thất bại: {result.Message}", false, 2000);
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

                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.WriteLine("🔐 Thay đổi mật khẩu");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine("─".PadRight(50, '─'));

                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.Write("Mật khẩu hiện tại: ");
                string currentPassword = ReadPassword();

                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                Console.Write("Mật khẩu mới: ");
                string newPassword = ReadPassword();

                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                {
                    ConsoleRenderingService.ShowMessageBox("Mật khẩu mới phải có ít nhất 6 ký tự!", false, 2000);
                    return;
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
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

                var result = await _userService.UpdatePasswordAsync(_currentUser.Id, passwordDto);

                if (result.IsSuccess)
                {
                    ConsoleRenderingService.ShowMessageBox("✅ Đổi mật khẩu thành công!", true, 2500);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox($"❌ Đổi mật khẩu thất bại: {result.Message}", false, 2000);
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

        public void ShowSimpleInfo(UserDto user)
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÔNG TIN NGƯỜI DÙNG", 80, 15);
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 15) / 4;
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine($"👤 Tên đăng nhập: {user?.Username ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine($"📧 Email: {user?.Email ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine($"👨 Họ tên: {user?.FullName ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine($"📱 Số điện thoại: {user?.PhoneNumber ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
            Console.WriteLine($"🎭 Vai trò: {user?.Role ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
            Console.WriteLine($"📅 Ngày tạo tài khoản: {(user?.CreatedAt != null ? user.CreatedAt.ToString("dd/MM/yyyy HH:mm") : "Chưa có")}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 8);
            Console.WriteLine($"🕐 Lần đăng nhập cuối: {(user?.LastLoginAt != null ? user.LastLoginAt.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa có")}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
            Console.WriteLine($"⭐ Trạng thái: {user?.Status ?? "Chưa cập nhật"}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }
}
