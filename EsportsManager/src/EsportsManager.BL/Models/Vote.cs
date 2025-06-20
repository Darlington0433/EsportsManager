using System;

namespace EsportsManager.BL.Models
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int UserId { get; set; }
        public string EntityType { get; set; } = string.Empty; // Player/Tournament/Team
        public int EntityId { get; set; }
        public bool IsUpvote { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
