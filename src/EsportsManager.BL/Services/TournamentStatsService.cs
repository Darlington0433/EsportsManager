using EsportsManager.BL.Constants;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Services;

/// <summary>
/// Tournament statistics calculation service for business logic
/// </summary>
public class TournamentStatsService
{
    /// <summary>
    /// Calculates comprehensive tournament statistics
    /// </summary>
    public static TournamentStatsDto CalculateTournamentStats(IEnumerable<TournamentInfoDto> tournaments)
    {
        if (tournaments == null || !tournaments.Any())
        {
            return new TournamentStatsDto
            {
                TotalTournaments = 0,
                ActiveTournaments = 0,
                CompletedTournaments = 0,
                TotalPrizePool = 0,
                AvgTeamsPerTournament = 0
            };
        }

        return new TournamentStatsDto
        {
            TotalTournaments = tournaments.Count(),
            ActiveTournaments = tournaments.Count(t => TournamentConstants.ACTIVE_STATUSES.Contains(t.Status)),
            CompletedTournaments = tournaments.Count(t => TournamentConstants.COMPLETED_STATUSES.Contains(t.Status)),
            TotalPrizePool = tournaments.Sum(t => t.PrizePool),
            AvgTeamsPerTournament = tournaments.Average(t => t.RegisteredTeams)
        };
    }
    
    /// <summary>
    /// Formats tournament name for display based on max length
    /// </summary>
    public static string FormatTournamentNameForDisplay(string tournamentName, int maxLength = TournamentConstants.MAX_TOURNAMENT_NAME_DISPLAY)
    {
        if (string.IsNullOrEmpty(tournamentName))
            return string.Empty;
            
        return tournamentName.Length > maxLength 
            ? tournamentName.Substring(0, maxLength)
            : tournamentName;
    }
    
    /// <summary>
    /// Formats tournament name for short display
    /// </summary>
    public static string FormatTournamentNameShort(string tournamentName)
    {
        return FormatTournamentNameForDisplay(tournamentName, TournamentConstants.MAX_TOURNAMENT_NAME_SHORT);
    }
    
    /// <summary>
    /// Gets tournament status display information
    /// </summary>
    public static (bool IsActive, bool IsCompleted, string DisplayStatus) GetTournamentStatusInfo(string status)
    {
        var isActive = TournamentConstants.ACTIVE_STATUSES.Contains(status);
        var isCompleted = TournamentConstants.COMPLETED_STATUSES.Contains(status);
        
        var displayStatus = status switch
        {
            "Ongoing" => "Đang diễn ra",
            "Registration" => "Đang mở đăng ký",
            "Completed" => "Đã kết thúc",
            _ => status
        };
        
        return (isActive, isCompleted, displayStatus);
    }
}
