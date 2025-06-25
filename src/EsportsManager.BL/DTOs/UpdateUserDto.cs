using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho cập nhật thông tin user
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Email mới (optional)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại mới (optional)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Họ tên mới (optional)
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Tiểu sử/bio mới (optional)
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// Avatar URL mới (optional)
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Ngày sinh mới (optional)
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính mới (optional)
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Địa chỉ mới (optional)
        /// </summary>
        public string? Address { get; set; }
    }
}
