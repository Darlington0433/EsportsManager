using System;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO cho việc cập nhật mật khẩu
    /// </summary>
    public class UpdatePasswordDto
    {
        /// <summary>
        /// ID người dùng
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Mật khẩu hiện tại
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Xác nhận mật khẩu mới
        /// </summary>
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }


}
