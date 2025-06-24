namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho kết quả tournament
    /// </summary>
    public class TournamentResultDto
    {
        /// <summary>
        /// Vị trí xếp hạng
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Tên đội
        /// </summary>
        public required string TeamName { get; set; }

        /// <summary>
        /// Số tiền thưởng
        /// </summary>
        public decimal PrizeMoney { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Người dẫn đầu đội
        /// </summary>
        public required string TeamLeader { get; set; }

        /// <summary>
        /// ID của đội
        /// </summary>
        public int TeamId { get; set; }
    }
}

