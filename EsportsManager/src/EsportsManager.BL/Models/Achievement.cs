using System;

namespace EsportsManager.BL.Models;

public class Achievement
{
    public int AchievementId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AchievementDate { get; set; } = DateTime.UtcNow;
    public int? AwardedBy { get; set; }
    public string Status { get; set; } = "Active";
}
