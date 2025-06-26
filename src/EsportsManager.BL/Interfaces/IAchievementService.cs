using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Achievement Service Interface - Quản lý thành tích người chơi
    /// Áp dụng Interface Segregation Principle
    /// </summary>
    public interface IAchievementService
    {
        /// <summary>
        /// Lấy thống kê tổng quan của người chơi
        /// </summary>
        Task<PlayerStatsDto> GetPlayerStatsAsync(int userId);

        /// <summary>
        /// Lấy lịch sử tham gia giải đấu của người chơi
        /// </summary>
        Task<List<PlayerTournamentHistoryDto>> GetPlayerTournamentHistoryAsync(int userId);

        /// <summary>
        /// Lấy danh hiệu và thành tích của người chơi
        /// </summary>
        Task<List<PlayerAchievementDto>> GetPlayerAchievementsAsync(int userId);

        /// <summary>
        /// Lấy điểm nổi bật của người chơi
        /// </summary>
        Task<List<string>> GetPlayerHighlightsAsync(int userId);

        /// <summary>
        /// Lấy xếp hạng hiện tại của người chơi
        /// </summary>
        Task<int> GetPlayerRankingAsync(int userId);
    }
}
