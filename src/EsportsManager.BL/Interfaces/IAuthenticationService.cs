using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using EsportsManager.BL.Services;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Authentication Service Interface - Định nghĩa các phương thức liên quan đến 
    /// việc xác thực và quản lý người dùng
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Đăng nhập với username/password
        /// </summary>
        Task<LoginResult> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Đăng ký tài khoản người dùng mới
        /// </summary>
        Task<RegisterResult> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Kiểm tra xem người dùng có quyền truy cập chức năng hay không
        /// </summary>
        bool HasPermission(string userRole, string requiredRole);

        /// <summary>
        /// Đăng xuất người dùng
        /// </summary>
        Task<bool> LogoutAsync(int userId);

        /// <summary>
        /// Xác thực email người dùng
        /// </summary>
        Task<bool> VerifyEmailAsync(string token);

        /// <summary>
        /// Gửi email xác thực lại
        /// </summary>
        Task<bool> ResendVerificationEmailAsync(string email);

        /// <summary>
        /// Khôi phục mật khẩu
        /// </summary>
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
