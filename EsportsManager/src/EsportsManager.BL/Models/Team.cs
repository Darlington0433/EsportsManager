using System;
using System.Collections.Generic;

namespace EsportsManager.BL.Models;

public class Team
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int CaptainId { get; set; }
    public List<int> MemberIds { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}
