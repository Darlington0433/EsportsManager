using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Repository cho Achievement
    /// </summary>
    public class AchievementRepository : IAchievementRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<AchievementRepository> _logger;

        public AchievementRepository(DataContext context, ILogger<AchievementRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy danh sách thành tích của user
        /// </summary>
        public async Task<List<Achievement>> GetByUserIdAsync(int userId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    SELECT AchievementID, UserID, Title, Description, AchievementType, 
                           DateAchieved, AssignedBy, TournamentID, TeamID, CreatedAt, UpdatedAt
                    FROM Achievements 
                    WHERE UserID = @UserId
                    ORDER BY DateAchieved DESC";

                var achievements = await connection.QueryAsync<Achievement>(sql, new { UserId = userId });
                return achievements.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting achievements for UserID: {userId}");
                return new List<Achievement>();
            }
        }

        /// <summary>
        /// Tạo thành tích mới
        /// </summary>
        public async Task<Achievement> CreateAsync(Achievement achievement)
        {
            if (achievement == null)
                throw new ArgumentNullException(nameof(achievement));

            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    INSERT INTO Achievements (UserID, Title, Description, AchievementType, DateAchieved, AssignedBy, TournamentID, TeamID, CreatedAt, UpdatedAt)
                    VALUES (@UserID, @Title, @Description, @AchievementType, @DateAchieved, @AssignedBy, @TournamentID, @TeamID, @CreatedAt, @UpdatedAt);
                    SELECT LAST_INSERT_ID();";

                var achievementId = await connection.QuerySingleAsync<int>(sql, new
                {
                    achievement.UserID,
                    achievement.Title,
                    achievement.Description,
                    achievement.AchievementType,
                    achievement.DateAchieved,
                    achievement.AssignedBy,
                    achievement.TournamentID,
                    achievement.TeamID,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

                achievement.AchievementID = achievementId;
                achievement.CreatedAt = DateTime.Now;
                achievement.UpdatedAt = DateTime.Now;

                return achievement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating achievement for UserID: {achievement.UserID}");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thành tích
        /// </summary>
        public async Task<Achievement> UpdateAsync(Achievement achievement)
        {
            if (achievement == null)
                throw new ArgumentNullException(nameof(achievement));

            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    UPDATE Achievements 
                    SET Title = @Title, Description = @Description, AchievementType = @AchievementType, 
                        DateAchieved = @DateAchieved, TournamentID = @TournamentID, TeamID = @TeamID, 
                        UpdatedAt = @UpdatedAt
                    WHERE AchievementID = @AchievementID";

                await connection.ExecuteAsync(sql, new
                {
                    achievement.AchievementID,
                    achievement.Title,
                    achievement.Description,
                    achievement.AchievementType,
                    achievement.DateAchieved,
                    achievement.TournamentID,
                    achievement.TeamID,
                    UpdatedAt = DateTime.Now
                });

                achievement.UpdatedAt = DateTime.Now;
                return achievement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating achievement ID: {achievement.AchievementID}");
                throw;
            }
        }

        /// <summary>
        /// Xóa thành tích
        /// </summary>
        public async Task<bool> DeleteAsync(int achievementId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                const string sql = "DELETE FROM Achievements WHERE AchievementID = @AchievementId";

                var rowsAffected = await connection.ExecuteAsync(sql, new { AchievementId = achievementId });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting achievement ID: {achievementId}");
                return false;
            }
        }

        /// <summary>
        /// Lấy thành tích theo ID
        /// </summary>
        public async Task<Achievement?> GetByIdAsync(int achievementId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    SELECT AchievementID, UserID, Title, Description, AchievementType, 
                           DateAchieved, AssignedBy, TournamentID, TeamID, CreatedAt, UpdatedAt
                    FROM Achievements 
                    WHERE AchievementID = @AchievementId";

                var achievement = await connection.QueryFirstOrDefaultAsync<Achievement>(sql, new { AchievementId = achievementId });
                return achievement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting achievement by ID: {achievementId}");
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách thành tích theo loại
        /// </summary>
        public async Task<List<Achievement>> GetByTypeAsync(string achievementType)
        {
            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    SELECT AchievementID, UserID, Title, Description, AchievementType, 
                           DateAchieved, AssignedBy, TournamentID, TeamID, CreatedAt, UpdatedAt
                    FROM Achievements 
                    WHERE AchievementType = @AchievementType
                    ORDER BY DateAchieved DESC";

                var achievements = await connection.QueryAsync<Achievement>(sql, new { AchievementType = achievementType });
                return achievements.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting achievements by type: {achievementType}");
                return new List<Achievement>();
            }
        }
    }
}
