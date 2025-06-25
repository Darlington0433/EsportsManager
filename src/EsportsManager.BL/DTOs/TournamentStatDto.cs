using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thống kê tournament dựa trên game
    /// </summary>
    public class TournamentStatDto
    {
        /// <summary>
        /// ID của tournament
        /// </summary>
        public int TournamentId { get; set; }

        /// <summary>
        /// Tên của tournament
        /// </summary>
        public required string TournamentName { get; set; }

        /// <summary>
        /// Trạng thái của tournament
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Số đội tham gia
        /// </summary>
        public int TeamsParticipating { get; set; }

        /// <summary>
        /// Tổng giá trị giải thưởng
        /// </summary>
        public decimal PrizePool { get; set; }

        /// <summary>
        /// Ngày bắt đầu tournament
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc tournament
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}

