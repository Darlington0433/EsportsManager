using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho đăng ký tham gia giải đấu
    /// </summary>
    public class TournamentRegistrationDto
    {
        /// <summary>
        /// ID của registration
        /// </summary>
        public int RegistrationId { get; set; }

        /// <summary>
        /// ID của tournament
        /// </summary>
        public int TournamentId { get; set; }

        /// <summary>
        /// Tên của tournament
        /// </summary>
        public string TournamentName { get; set; } = string.Empty;

        /// <summary>
        /// ID của team
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Tên của team
        /// </summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// ID của người đăng ký
        /// </summary>
        public int RegisteredBy { get; set; }

        /// <summary>
        /// Tên của người đăng ký
        /// </summary>
        public string RegisteredByName { get; set; } = string.Empty;

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Trạng thái đăng ký
        /// </summary>
        public string Status { get; set; } = "Registered";

        /// <summary>
        /// Tên game
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// Số thành viên trong team
        /// </summary>
        public int TeamMemberCount { get; set; }
    }
}
