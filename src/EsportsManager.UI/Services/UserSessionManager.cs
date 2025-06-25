using System;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Services
{
    /// <summary>
    /// Quản lý thông tin phiên đăng nhập của người dùng
    /// </summary>
    public static class UserSessionManager
    {
        /// <summary>
        /// Thông tin người dùng hiện tại đang đăng nhập
        /// </summary>
        public static UserProfileDto? CurrentUser { get; set; }

        /// <summary>
        /// Trạng thái đăng nhập
        /// </summary>
        public static bool IsLoggedIn { get; set; } = false;

        /// <summary>
        /// Thông tin token (nếu cần)
        /// </summary>
        public static string? AuthToken { get; set; }

        /// <summary>
        /// Reset thông tin phiên hiện tại (đăng xuất)
        /// </summary>
        public static void ClearSession()
        {
            CurrentUser = null;
            IsLoggedIn = false;
            AuthToken = null;
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền truy cập một chức năng hay không
        /// </summary>
        public static bool HasAccess(string requiredRole)
        {
            if (!IsLoggedIn || CurrentUser == null)
                return false;

            // Admin có quyền truy cập mọi chức năng
            if (CurrentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            // Kiểm tra role cụ thể
            return CurrentUser.Role.Equals(requiredRole, StringComparison.OrdinalIgnoreCase);
        }
    }
}
