using System;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO cho đăng ký
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Xác nhận mật khẩu
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Họ tên đầy đủ
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Vai trò (mặc định là Viewer)
        /// </summary>
        public string Role { get; set; } = "Viewer";

        /// <summary>
        /// Câu trả lời bảo mật
        /// </summary>
        public string? SecurityAnswer { get; set; }
    }


}
