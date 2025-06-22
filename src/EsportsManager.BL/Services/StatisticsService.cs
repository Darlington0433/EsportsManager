using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

/// <summary>
/// Statistics Service Implementation - Mock data for development
/// Production: Replace with real database queries and analytics
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly ILogger<StatisticsService> _logger;
    private readonly Random _random = new();

    public StatisticsService(ILogger<StatisticsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SystemStatsDto> GetSystemStatsAsync()
    {
        await Task.Delay(100); // Simulate database query

        var stats = new SystemStatsDto
        {
            TotalUsers = 15420,
            ActiveUsers = 8930,
            TotalTournaments = 247,
            ActiveTournaments = 12,
            TotalTeams = 1840,
            TotalRevenue = 2485000000,
            LastUpdated = DateTime.Now,
            OnlineUsers = 342,
            TodayRegistrations = 28,
            CompletedTournaments = 235,
            TodayRevenue = 15500000
        };

        _logger.LogInformation("System stats retrieved: {TotalUsers} users, {TotalTournaments} tournaments", 
            stats.TotalUsers, stats.TotalTournaments);

        return stats;
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        await Task.Delay(80);

        var userStats = new UserStatsDto
        {
            TotalUsers = 15420,
            ActiveUsers = 8930,
            NewUsersToday = 28,
            NewUsersThisWeek = 156,
            NewUsersThisMonth = 642,
            OnlineUsers = 342,
            AverageSessionTime = 45.5,
            UsersByRole = new Dictionary<string, int>
            {
                { "Player", 12890 },
                { "Viewer", 2485 },
                { "Admin", 45 }
            },
            UsersByStatus = new Dictionary<string, int>
            {
                { "Active", 14870 },
                { "Inactive", 485 },
                { "Banned", 65 }
            },
            RecentActivity = await GetRecentUserActivity()
        };

        return userStats;
    }

    public async Task<TournamentStatsDto> GetTournamentStatsAsync()
    {
        await Task.Delay(90);

        var tournamentStats = new TournamentStatsDto
        {
            TotalTournaments = 247,
            ActiveTournaments = 12,
            CompletedTournaments = 235,
            UpcomingTournaments = 18,
            TotalParticipants = 28450,
            TotalPrizePool = 15840000000,
            AverageParticipants = 115.2,
            TournamentsByGame = new Dictionary<string, int>
            {
                { "League of Legends", 89 },
                { "Mobile Legends", 67 },
                { "Valorant", 45 },
                { "CS:GO", 28 },
                { "FIFA", 18 }
            },
            TournamentsByFormat = new Dictionary<string, int>
            {
                { "Single Elimination", 142 },
                { "Double Elimination", 78 },
                { "Round Robin", 18 },
                { "Swiss", 9 }
            },
            PopularTournaments = await GetPopularTournaments()
        };

        return tournamentStats;
    }

    public async Task<RevenueStatsDto> GetRevenueStatsAsync()
    {
        await Task.Delay(70);

        var revenueStats = new RevenueStatsDto
        {
            TotalRevenue = 2485000000,
            RevenueToday = 15500000,
            RevenueThisWeek = 89200000,
            RevenueThisMonth = 342800000,
            RevenueThisYear = 2485000000,
            AverageRevenuePerTournament = 10060729,
            TotalEntryFees = 1840000000,
            TotalPrizePayout = 1650000000,
            MonthlyRevenue = await GetMonthlyRevenue(),
            RevenueSources = await GetRevenueSources()
        };

        return revenueStats;
    }

    public async Task<SystemStatsDto> GetStatsByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        await Task.Delay(120);

        // Mock calculation based on date range
        var days = (toDate - fromDate).Days;
        var multiplier = Math.Max(0.1, days / 365.0);

        var stats = new SystemStatsDto
        {
            TotalUsers = (int)(15420 * multiplier),
            ActiveUsers = (int)(8930 * multiplier),
            TotalTournaments = (int)(247 * multiplier),
            ActiveTournaments = Math.Min(12, (int)(12 * multiplier)),
            TotalTeams = (int)(1840 * multiplier),
            TotalRevenue = (decimal)(2485000000 * multiplier),
            LastUpdated = DateTime.Now,
            OnlineUsers = Math.Min(342, (int)(342 * multiplier)),
            TodayRegistrations = (int)(28 * multiplier),
            CompletedTournaments = (int)(235 * multiplier),
            TodayRevenue = (decimal)(15500000 * multiplier)
        };

        return stats;
    }

    public async Task<List<PlayerStatDto>> GetTopPlayersAsync(int count = 10)
    {
        await Task.Delay(60);

        var topPlayers = new List<PlayerStatDto>();
        var playerNames = new[] { "ProGamer_VN", "EsportsKing", "VietnamChamp", "GameMaster", "SkillfulPlayer", 
                                 "TopNotch", "EliteGamer", "ChampionVN", "ProSkill", "MasterPlayer" };

        for (int i = 0; i < Math.Min(count, playerNames.Length); i++)
        {
            topPlayers.Add(new PlayerStatDto
            {
                UserId = i + 1,
                Username = playerNames[i],
                FullName = $"Nguyễn Văn {(char)('A' + i)}",
                TournamentsPlayed = _random.Next(50, 200),
                TournamentsWon = _random.Next(5, 50),
                TotalWinnings = _random.Next(10000000, 500000000),
                WinRate = _random.NextDouble() * 0.4 + 0.3, // 30-70%
                CurrentRank = i + 1,
                TotalPlayTime = TimeSpan.FromHours(_random.Next(500, 2000)),
                LastActive = DateTime.Now.AddDays(-_random.Next(0, 7)),
                FavoriteGame = new[] { "League of Legends", "Mobile Legends", "Valorant" }[_random.Next(3)]
            });
        }

        return topPlayers.OrderByDescending(p => p.TotalWinnings).ToList();
    }

    public async Task<List<DailyActivityDto>> GetDailyActivityAsync(int days = 30)
    {
        await Task.Delay(80);

        var activities = new List<DailyActivityDto>();
        
        for (int i = days - 1; i >= 0; i--)
        {
            var date = DateTime.Now.Date.AddDays(-i);
            activities.Add(new DailyActivityDto
            {
                Date = date,
                ActiveUsers = _random.Next(300, 500),
                NewRegistrations = _random.Next(10, 50),
                TournamentsCreated = _random.Next(0, 5),
                TournamentsCompleted = _random.Next(0, 3),
                Revenue = _random.Next(5000000, 25000000),
                AverageSessionTime = TimeSpan.FromMinutes(_random.Next(30, 90)),
                LoginCount = _random.Next(800, 1500),
                PeakOnlineUsers = _random.Next(400, 600)
            });
        }

        return activities;
    }

    public async Task<ActivityStatsDto> GetActivityStatsAsync(DateTime startDate, DateTime endDate)
    {
        await Task.Delay(90);

        var days = (endDate - startDate).Days;
        var dailyActivities = await GetDailyActivityAsync(days);

        var activityStats = new ActivityStatsDto
        {
            TotalActivities = dailyActivities.Sum(d => d.LoginCount + d.TournamentsCreated + d.TournamentsCompleted),
            ActiveUsersInPeriod = dailyActivities.Sum(d => d.ActiveUsers) / Math.Max(1, days),
            NewRegistrations = dailyActivities.Sum(d => d.NewRegistrations),
            TournamentsCreated = dailyActivities.Sum(d => d.TournamentsCreated),
            TournamentsCompleted = dailyActivities.Sum(d => d.TournamentsCompleted),
            TotalRevenue = dailyActivities.Sum(d => d.Revenue),
            ActivityByType = new Dictionary<string, int>
            {
                { "Đăng nhập", dailyActivities.Sum(d => d.LoginCount) },
                { "Đăng ký mới", dailyActivities.Sum(d => d.NewRegistrations) },
                { "Tạo giải đấu", dailyActivities.Sum(d => d.TournamentsCreated) },
                { "Hoàn thành giải đấu", dailyActivities.Sum(d => d.TournamentsCompleted) }
            },
            DailyBreakdown = dailyActivities,
            FromDate = startDate,
            ToDate = endDate
        };

        return activityStats;
    }

    private async Task<List<UserActivityDto>> GetRecentUserActivity()
    {
        await Task.Delay(20);

        var activities = new List<UserActivityDto>();
        var actions = new[] { "Đăng nhập", "Tạo giải đấu", "Tham gia giải đấu", "Cập nhật hồ sơ", "Đăng xuất" };
        var usernames = new[] { "player1", "gamer_vn", "esports_pro", "vietnam_champ", "top_player" };

        for (int i = 0; i < 10; i++)
        {
            activities.Add(new UserActivityDto
            {
                UserId = _random.Next(1, 100),
                Username = usernames[_random.Next(usernames.Length)],
                Action = actions[_random.Next(actions.Length)],
                Timestamp = DateTime.Now.AddMinutes(-_random.Next(1, 1440)),
                Details = "Chi tiết hoạt động",
                IpAddress = $"192.168.1.{_random.Next(1, 255)}"
            });
        }

        return activities.OrderByDescending(a => a.Timestamp).ToList();
    }

    private async Task<List<PopularTournamentDto>> GetPopularTournaments()
    {
        await Task.Delay(30);

        return new List<PopularTournamentDto>
        {
            new PopularTournamentDto
            {
                TournamentId = 1,
                Name = "VTC Esports Championship 2025",
                Game = "League of Legends",
                ParticipantCount = 64,
                PrizePool = 1000000000,
                StartDate = DateTime.Now.AddDays(30),
                Status = TournamentStatus.RegistrationOpen
            },
            new PopularTournamentDto
            {
                TournamentId = 2,
                Name = "Vietnam Mobile Legends Cup",
                Game = "Mobile Legends",
                ParticipantCount = 32,
                PrizePool = 50000000,
                StartDate = DateTime.Now.AddDays(15),
                Status = TournamentStatus.RegistrationOpen
            }
        };
    }

    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenue()
    {
        await Task.Delay(40);

        var monthlyRevenue = new List<MonthlyRevenueDto>();
        
        for (int i = 11; i >= 0; i--)
        {
            var date = DateTime.Now.AddMonths(-i);
            monthlyRevenue.Add(new MonthlyRevenueDto
            {
                Year = date.Year,
                Month = date.Month,
                Revenue = _random.Next(100000000, 400000000),
                TournamentCount = _random.Next(15, 35),
                ParticipantCount = _random.Next(1500, 3000)
            });
        }

        return monthlyRevenue;
    }

    private async Task<List<RevenueSourceDto>> GetRevenueSources()
    {
        await Task.Delay(25);

        return new List<RevenueSourceDto>
        {
            new RevenueSourceDto
            {
                Source = "Phí tham gia giải đấu",
                Amount = 1840000000,
                Percentage = 74.0,
                TransactionCount = 15420
            },
            new RevenueSourceDto
            {
                Source = "Tài trợ",
                Amount = 485000000,
                Percentage = 19.5,
                TransactionCount = 89
            },
            new RevenueSourceDto
            {
                Source = "Phí dịch vụ",
                Amount = 160000000,
                Percentage = 6.5,
                TransactionCount = 3240
            }
        };
    }
}
