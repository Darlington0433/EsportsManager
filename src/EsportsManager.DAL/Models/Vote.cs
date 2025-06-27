using System;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Entity class cho bảng Votes
    /// </summary>
    public class Vote
    {
        /// <summary>
        /// ID của vote
        /// </summary>
        public int VoteID { get; set; }

        /// <summary>
        /// ID của người vote
        /// </summary>
        public int VoterID { get; set; }

        /// <summary>
        /// Loại vote (Player hoặc Tournament)
        /// </summary>
        public string VoteType { get; set; } = string.Empty;

        /// <summary>
        /// ID của đối tượng được vote (PlayerID hoặc TournamentID)
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// Điểm vote (1-5)
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Nhận xét kèm theo vote
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian vote
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
