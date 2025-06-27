using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thống kê giải đấu
    /// </summary>
    public class TournamentStatsDto
    {
        public int TotalTournaments { get; set; }
        public int ActiveTournaments { get; set; }
        public int CompletedTournaments { get; set; }
        public decimal TotalPrizePool { get; set; }
        public double AvgTeamsPerTournament { get; set; }
        
        // Legacy properties for backward compatibility
        public int OngoingTournaments { get; set; }
        public int CancelledTournaments { get; set; }
        public int TotalParticipants { get; set; }
        public int AverageParticipantsPerTournament { get; set; }
    }
}
