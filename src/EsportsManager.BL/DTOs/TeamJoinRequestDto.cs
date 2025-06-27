using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tham gia team
    /// </summary>
    public class TeamJoinRequestDto
    {
        /// <summary>
        /// ID của yêu cầu
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// ID của team
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Tên của team
        /// </summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// ID của player gửi yêu cầu
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// Tên của player gửi yêu cầu
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// Email của player
        /// </summary>
        public string PlayerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Tin nhắn kèm theo yêu cầu
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Ngày gửi yêu cầu
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Trạng thái yêu cầu
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Tên game của team
        /// </summary>
        public string GameName { get; set; } = string.Empty;
    }
}
