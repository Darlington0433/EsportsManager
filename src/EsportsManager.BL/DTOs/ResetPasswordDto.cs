using System;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO cho việc reset mật khẩu
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Email (alternative to username)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Câu trả lời bảo mật
        /// </summary>
        public string SecurityAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        public string ConfirmNewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Token xác thực reset mật khẩu
        /// </summary>
        public string? Token { get; set; }
    }


}
