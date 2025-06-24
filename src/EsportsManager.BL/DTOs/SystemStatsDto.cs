using System;

namespace EsportsManager.BL.DTOs
{    /// <summary>
    /// DTO cho thống kê hệ thống
    /// </summary>
    public class SystemStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalTournaments { get; set; }
        public int ActiveTournaments { get; set; }
        public int TotalTeams { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OngoingTournaments { get; set; }
        public decimal TotalDonations { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TournamentsThisMonth { get; set; }
    }


}
