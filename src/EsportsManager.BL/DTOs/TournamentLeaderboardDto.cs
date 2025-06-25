namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho bảng xếp hạng tournament
    /// </summary>
    public class TournamentLeaderboardDto
    {
        /// <summary>
        /// Thứ hạng
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Tên đội
        /// </summary>
        public required string TeamName { get; set; }

        /// <summary>
        /// Vị trí trong tournament
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Số tiền thưởng
        /// </summary>
        public decimal PrizeMoney { get; set; }

        /// <summary>
        /// Số thành viên trong đội
        /// </summary>
        public int TeamSize { get; set; }

        /// <summary>
        /// Danh sách thành viên
        /// </summary>
        public string? TeamMembers { get; set; }
    }
}

