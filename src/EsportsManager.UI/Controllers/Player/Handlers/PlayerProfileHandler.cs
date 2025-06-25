using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;

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

                Console.WriteLine($"👤 ID: {userInfo.Id}");
                Console.WriteLine($"📧 Username: {userInfo.Username}");
                Console.WriteLine($"✉️ Email: {userInfo.Email ?? "Chưa cập nhật"}");
                Console.WriteLine($"🎭 Role: {userInfo.Role}");
                Console.WriteLine($"📅 Ngày tạo: {userInfo.CreatedAt:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"🕐 Lần đăng nhập cuối: {userInfo.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");

                ConsoleRenderingService.PauseWithMessage();
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
                    ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin hiện tại!", true, 2000);
                    return;
                }

                Console.WriteLine("Thông tin hiện tại:");
                Console.WriteLine($"Email: {currentInfo.Email ?? "Chưa có"}");
                Console.WriteLine($"Họ tên: {currentInfo.FullName ?? "Chưa có"}");
                Console.WriteLine();

                Console.Write("Email mới (Enter để bỏ qua): ");
                string newEmail = Console.ReadLine()?.Trim();

                Console.Write("Họ tên mới (Enter để bỏ qua): ");
                string newFullName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newFullName))
                {
                    var updateDto = new UpdateUserDto
                    {
                        Email = !string.IsNullOrEmpty(newEmail) ? newEmail : currentInfo?.Email,
                        FullName = !string.IsNullOrEmpty(newFullName) ? newFullName : currentInfo?.FullName
                    };

                    bool success = await UpdatePersonalInfoAsync(updateDto);

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
                    ConsoleRenderingService.ShowMessageBox("Không có thông tin nào được thay đổi!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
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
