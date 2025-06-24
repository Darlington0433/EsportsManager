using System;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO cho việc tạo user mới (bởi Admin)
    /// </summary>
    public class CreateUserDto
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
        /// Họ tên đầy đủ
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public string Role { get; set; } = "Viewer";

        /// <summary>
        /// Trạng thái
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Câu trả lời bảo mật
        /// </summary>
        public string? SecurityAnswer { get; set; }
    }


}
