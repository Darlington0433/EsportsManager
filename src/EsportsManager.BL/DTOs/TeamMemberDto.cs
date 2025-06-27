using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thành viên của team
    /// </summary>
    public class TeamMemberDto
    {
        /// <summary>
        /// ID của thành viên
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Tên đầy đủ của thành viên
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Username của thành viên
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email của thành viên
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Vai trò trong team
        /// </summary>
        public string Role { get; set; } = "Member";

        /// <summary>
        /// Ngày tham gia team
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Có phải là captain không
        /// </summary>
        public bool IsCaptain { get; set; } = false;

        /// <summary>
        /// Trạng thái trong team
        /// </summary>
        public string Status { get; set; } = "Active";
    }
}
