using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// Statistics Service Interface - Quản lý thống kê hệ thống
/// Áp dụng Interface Segregation Principle
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Lấy thống kê tổng quan hệ thống
    /// </summary>
    Task<SystemStatsDto> GetSystemStatsAsync();

    /// <summary>
    /// Lấy thống kê người dùng
    /// </summary>
    Task<UserStatsDto> GetUserStatsAsync();

    /// <summary>
    /// Lấy thống kê giải đấu
    /// </summary>
    Task<TournamentStatsDto> GetTournamentStatsAsync();

    /// <summary>
    /// Lấy thống kê doanh thu
    /// </summary>
    Task<RevenueStatsDto> GetRevenueStatsAsync();    /// <summary>
    /// Lấy thống kê hoạt động trong khoảng thời gian
    /// </summary>
    Task<ActivityStatsDto> GetActivityStatsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Lấy thống kê theo khoảng thời gian
    /// </summary>
    Task<SystemStatsDto> GetStatsByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Lấy top players theo thống kê
    /// </summary>
    Task<List<PlayerStatDto>> GetTopPlayersAsync(int count = 10);

    /// <summary>
    /// Lấy thống kê hoạt động hàng ngày
    /// </summary>
    Task<List<DailyActivityDto>> GetDailyActivityAsync(int days = 30);
}
