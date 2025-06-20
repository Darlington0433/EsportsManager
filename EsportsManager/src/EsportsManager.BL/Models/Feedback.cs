using System;

namespace EsportsManager.BL.Models;

public class Feedback
{
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public int? TournamentId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
