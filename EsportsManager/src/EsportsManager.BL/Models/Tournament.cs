using System;
namespace EsportsManager.BL.Models;

public class Tournament
{
    public int TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int GameId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public int MaxTeams { get; set; }
    public int MinTeamSize { get; set; }
    public int MaxTeamSize { get; set; }
    public string Status { get; set; } = "Upcoming";
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
