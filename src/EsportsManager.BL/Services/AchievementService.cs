using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services
{
    /// <summary>
    /// Service xử lý thành tích và thống kê của người chơi
    /// </summary>
    public class AchievementService : IAchievementService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly ILogger<AchievementService> _logger;

        public AchievementService(
            IUsersRepository userRepository,
            IAchievementRepository achievementRepository,
            ILogger<AchievementService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _achievementRepository = achievementRepository ?? throw new ArgumentNullException(nameof(achievementRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy thống kê tổng quan của người chơi - Simplified version
        /// </summary>
        public async Task<PlayerStatsDto> GetPlayerStatsAsync(int userId)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return new PlayerStatsDto();
                }

                // Return basic stats since we don't have tournament data
                return await Task.FromResult(new PlayerStatsDto
                {
                    UserId = userId,
                    Username = player.Username,
                    TotalTournaments = 0, // Would need tournament repository
                    TournamentsWon = 0,
                    FinalsAppearances = 0,
                    TotalPrizeMoney = 0,
                    AverageRating = 0.0,
                    CurrentRanking = 0,
                    WinRate = 0.0,
                    SkillLevel = "Beginner"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stats for user {userId}");
                return new PlayerStatsDto();
            }
        }

        /// <summary>
        /// Lấy lịch sử tham gia giải đấu của người chơi - Simplified version
        /// </summary>
        public async Task<List<PlayerTournamentHistoryDto>> GetPlayerTournamentHistoryAsync(int userId)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return new List<PlayerTournamentHistoryDto>();
                }

                // Return empty history since we don't have tournament repository
                return await Task.FromResult(new List<PlayerTournamentHistoryDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting tournament history for user {userId}");
                return new List<PlayerTournamentHistoryDto>();
            }
        }

        /// <summary>
        /// Lấy danh hiệu và thành tích của người chơi - từ database thật
        /// </summary>
        public async Task<List<PlayerAchievementDto>> GetPlayerAchievementsAsync(int userId)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return new List<PlayerAchievementDto>();
                }

                // Lấy thành tích từ database
                var achievements = await _achievementRepository.GetByUserIdAsync(userId);

                var result = achievements.Select(a => new PlayerAchievementDto
                {
                    AchievementId = a.AchievementID,
                    Title = a.Title,
                    Description = a.Description ?? "",
                    DateAchieved = a.DateAchieved,
                    AchievementType = a.AchievementType
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting achievements for user {userId}");
                return new List<PlayerAchievementDto>();
            }
        }

        /// <summary>
        /// Gán thành tích cho người chơi
        /// </summary>
        public async Task<bool> AssignAchievementAsync(int userId, string achievementType, string description, int assignedBy, int? tournamentId = null, int? teamId = null)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return false;
                }

                var achievement = new Achievement
                {
                    UserID = userId,
                    Title = achievementType,
                    Description = description,
                    AchievementType = achievementType,
                    DateAchieved = DateTime.Now,
                    AssignedBy = assignedBy,
                    TournamentID = tournamentId,
                    TeamID = teamId
                };

                await _achievementRepository.CreateAsync(achievement);

                _logger.LogInformation($"Đã gán thành tích '{achievementType}' cho user ID {userId} bởi admin ID {assignedBy}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning achievement to user {userId}");
                return false;
            }
        }

        /// <summary>
        /// Xóa thành tích của người chơi
        /// </summary>
        public async Task<bool> RemoveAchievementAsync(int achievementId)
        {
            try
            {
                var success = await _achievementRepository.DeleteAsync(achievementId);

                if (success)
                {
                    _logger.LogInformation($"Đã xóa thành tích ID {achievementId}");
                }
                else
                {
                    _logger.LogWarning($"Không thể xóa thành tích ID {achievementId}");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing achievement {achievementId}");
                return false;
            }
        }

        /// <summary>
        /// Lấy điểm nổi bật của người chơi - Simplified version
        /// </summary>
        public async Task<List<string>> GetPlayerHighlightsAsync(int userId)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return new List<string>();
                }

                var highlights = new List<string>
                {
                    $"Thành viên từ {player.CreatedAt:dd/MM/yyyy}",
                    "Tài khoản đã được xác thực"
                };

                if (player.Status == "Active")
                {
                    highlights.Add("Tài khoản đang hoạt động");
                }

                return await Task.FromResult(highlights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting highlights for user {userId}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Lấy bảng xếp hạng top players - Simplified version
        /// </summary>
        public async Task<List<PlayerRankingDto>> GetTopPlayersAsync(int topCount = 10)
        {
            try
            {
                var allUsers = await _userRepository.GetAllAsync();
                var activeUsers = allUsers.Where(u => u.Status == "Active").Take(topCount);

                var rankings = activeUsers.Select((user, index) => new PlayerRankingDto
                {
                    UserId = user.UserID,
                    Username = user.Username,
                    Ranking = index + 1,
                    TotalScore = 100 - (index * 5), // Mock scoring
                    TournamentsWon = 0,
                    TotalPrizeMoney = 0
                }).ToList();

                return await Task.FromResult(rankings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top players");
                return new List<PlayerRankingDto>();
            }
        }

        /// <summary>
        /// Lấy xếp hạng của một người chơi cụ thể - Simplified version
        /// </summary>
        public async Task<int> GetPlayerRankingAsync(int userId)
        {
            try
            {
                var player = await _userRepository.GetByIdAsync(userId);
                if (player == null)
                {
                    _logger.LogWarning($"Không tìm thấy người chơi với ID: {userId}");
                    return 0;
                }

                // Return a basic ranking since we don't have tournament data
                return await Task.FromResult(player.UserID % 100); // Mock ranking based on user ID
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting ranking for user {userId}");
                return 0;
            }
        }

        /// <summary>
        /// So sánh thành tích giữa hai người chơi - Simplified version
        /// </summary>
        public async Task<PlayerComparisonDto> ComparePlayersAsync(int userId1, int userId2)
        {
            try
            {
                var player1Stats = await GetPlayerStatsAsync(userId1);
                var player2Stats = await GetPlayerStatsAsync(userId2);

                return new PlayerComparisonDto
                {
                    Player1 = player1Stats,
                    Player2 = player2Stats,
                    Comparison = new Dictionary<string, string>
                    {
                        { "Username", $"{player1Stats.Username} vs {player2Stats.Username}" },
                        { "Status", "Both players are active" }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error comparing players {userId1} and {userId2}");
                return new PlayerComparisonDto();
            }
        }

        #region Helper Methods

        private string GetPositionText(int position)
        {
            return position switch
            {
                1 => "🥇 Nhất",
                2 => "🥈 Nhì",
                3 => "🥉 Ba",
                _ => position <= 10 ? $"Top {position}" : $"#{position}"
            };
        }

        private string DetermineSkillLevel(double averageRating, int tournamentsWon)
        {
            if (tournamentsWon >= 5 && averageRating >= 4.5) return "Expert";
            if (tournamentsWon >= 2 && averageRating >= 4.0) return "Advanced";
            if (averageRating >= 3.5) return "Intermediate";
            return "Beginner";
        }

        #endregion

        #region Admin Methods

        /// <summary>
        /// Admin: Gán achievement/danh hiệu cho người chơi
        /// </summary>
        public async Task<bool> AssignAchievementToPlayerAsync(int userId, string achievementName, string description)
        {
            try
            {
                // TODO: Implement actual database operations to assign achievements
                // For now, return true as a placeholder
                _logger.LogInformation($"Assigning achievement '{achievementName}' to user {userId}: {description}");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning achievement to player {UserId}: {AchievementName}", userId, achievementName);
                return false;
            }
        }

        /// <summary>
        /// Admin: Lấy danh sách tất cả achievements có sẵn
        /// </summary>
        public async Task<List<string>> GetAvailableAchievementsAsync()
        {
            try
            {
                // TODO: Implement actual database query to get available achievements
                // For now, return a predefined list
                var achievements = new List<string>
                {
                    "Tournament Champion",
                    "Team Leader",
                    "MVP Player",
                    "Rising Star",
                    "Veteran Player",
                    "Fair Play Award",
                    "Community Contributor",
                    "Strategic Mastermind",
                    "Consistent Performer",
                    "Breakthrough Player"
                };

                return await Task.FromResult(achievements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available achievements");
                throw;
            }
        }

        /// <summary>
        /// Admin: Xóa achievement khỏi người chơi
        /// </summary>
        public async Task<bool> RemoveAchievementFromPlayerAsync(int userId, string achievementName)
        {
            try
            {
                // TODO: Implement actual database operations to remove achievements
                // For now, return true as a placeholder
                _logger.LogInformation($"Removing achievement '{achievementName}' from user {userId}");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing achievement from player {UserId}: {AchievementName}", userId, achievementName);
                return false;
            }
        }

        #endregion
    }
}
