using System;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO cho đăng nhập
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Tên đăng nhập hoặc email
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Ghi nhớ đăng nhập
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }


}
