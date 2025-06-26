using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thống kê người chơi
    /// </summary>
    public class PlayerStatsDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TotalTournaments { get; set; }
        public int TournamentsWon { get; set; }
        public int FinalsAppearances { get; set; }
        public int SemiFinalsAppearances { get; set; }
        public decimal TotalPrizeMoney { get; set; }
        public double AverageRating { get; set; }
        public int CurrentRanking { get; set; }
        public double WinRate { get; set; }
        public string SkillLevel { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho lịch sử tham gia giải đấu của người chơi
    /// </summary>
    public class PlayerTournamentHistoryDto
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public int Position { get; set; }
        public decimal PrizeMoney { get; set; }
        public DateTime ParticipationDate { get; set; }
    }

    /// <summary>
    /// DTO cho thành tích và danh hiệu của người chơi
    /// </summary>
    public class PlayerAchievementDto
    {
        public int AchievementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateAchieved { get; set; }
        public string AchievementType { get; set; } = string.Empty; // Tournament, Personal, Team, etc.
    }

    /// <summary>
    /// DTO cho bảng xếp hạng người chơi
    /// </summary>
    public class PlayerRankingDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Ranking { get; set; }
        public int TotalScore { get; set; }
        public int TournamentsWon { get; set; }
        public decimal TotalPrizeMoney { get; set; }
    }

    /// <summary>
    /// DTO cho so sánh hai người chơi
    /// </summary>
    public class PlayerComparisonDto
    {
        public PlayerStatsDto Player1 { get; set; } = new PlayerStatsDto();
        public PlayerStatsDto Player2 { get; set; } = new PlayerStatsDto();
        public Dictionary<string, string> Comparison { get; set; } = new Dictionary<string, string>();
    }
}
