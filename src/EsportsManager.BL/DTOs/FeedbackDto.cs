using System;

namespace EsportsManager.BL.DTOs
{
    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        public int TournamentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
