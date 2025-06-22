using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho thống kê tổng quan hệ thống
/// </summary>
public class SystemStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalTournaments { get; set; }
    public int ActiveTournaments { get; set; }
    public int TotalTeams { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime LastUpdated { get; set; }
    public int OnlineUsers { get; set; }
    public int TodayRegistrations { get; set; }
    public int CompletedTournaments { get; set; }
    public decimal TodayRevenue { get; set; }
}

/// <summary>
/// DTO cho thống kê người dùng
/// </summary>
public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int OnlineUsers { get; set; }
    public double AverageSessionTime { get; set; }
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
    public List<UserActivityDto> RecentActivity { get; set; } = new();
}

/// <summary>
/// DTO cho thống kê giải đấu
/// </summary>
public class TournamentStatsDto
{
    public int TotalTournaments { get; set; }
    public int ActiveTournaments { get; set; }
    public int CompletedTournaments { get; set; }
    public int UpcomingTournaments { get; set; }
    public int TotalParticipants { get; set; }
    public decimal TotalPrizePool { get; set; }
    public double AverageParticipants { get; set; }
    public Dictionary<string, int> TournamentsByGame { get; set; } = new();
    public Dictionary<string, int> TournamentsByFormat { get; set; } = new();
    public List<PopularTournamentDto> PopularTournaments { get; set; } = new();
}

/// <summary>
/// DTO cho thống kê doanh thu
/// </summary>
public class RevenueStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisWeek { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueThisYear { get; set; }
    public decimal AverageRevenuePerTournament { get; set; }
    public decimal TotalEntryFees { get; set; }
    public decimal TotalPrizePayout { get; set; }
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<RevenueSourceDto> RevenueSources { get; set; } = new();
}

/// <summary>
/// DTO cho thống kê player
/// </summary>
public class PlayerStatDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public string? FullName { get; set; }
    public int TournamentsPlayed { get; set; }
    public int TournamentsWon { get; set; }
    public decimal TotalWinnings { get; set; }
    public double WinRate { get; set; }
    public int CurrentRank { get; set; }
    public TimeSpan TotalPlayTime { get; set; }
    public DateTime LastActive { get; set; }
    public string? FavoriteGame { get; set; }
}

/// <summary>
/// DTO cho hoạt động hàng ngày
/// </summary>
public class DailyActivityDto
{
    public DateTime Date { get; set; }
    public int ActiveUsers { get; set; }
    public int NewRegistrations { get; set; }
    public int TournamentsCreated { get; set; }
    public int TournamentsCompleted { get; set; }
    public decimal Revenue { get; set; }
    public TimeSpan AverageSessionTime { get; set; }
    public int LoginCount { get; set; }
    public int PeakOnlineUsers { get; set; }
}

/// <summary>
/// DTO cho doanh thu theo tháng
/// </summary>
public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public decimal Revenue { get; set; }
    public int TournamentCount { get; set; }
    public int ParticipantCount { get; set; }
    public decimal AverageRevenuePerTournament => TournamentCount > 0 ? Revenue / TournamentCount : 0;
}

/// <summary>
/// DTO cho hoạt động người dùng
/// </summary>
public class UserActivityDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}

/// <summary>
/// DTO cho giải đấu phổ biến
/// </summary>
public class PopularTournamentDto
{
    public int TournamentId { get; set; }
    public required string Name { get; set; }
    public required string Game { get; set; }
    public int ParticipantCount { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime StartDate { get; set; }
    public TournamentStatus Status { get; set; }
}

/// <summary>
/// DTO cho nguồn doanh thu
/// </summary>
public class RevenueSourceDto
{
    public required string Source { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
    public int TransactionCount { get; set; }
}
