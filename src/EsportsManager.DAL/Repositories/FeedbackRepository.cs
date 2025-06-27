using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Repository cho bảng Feedback
    /// </summary>
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<FeedbackRepository> _logger;

        public FeedbackRepository(DataContext context, ILogger<FeedbackRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Thêm feedback mới
        /// </summary>
        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            try
            {
                using var connection = _context.CreateConnection();

                // Kiểm tra xem user đã gửi feedback cho tournament này chưa
                if (await HasFeedbackAsync(feedback.UserID, feedback.TournamentID))
                {
                    // Cập nhật feedback cũ
                    const string updateSql = @"
                        UPDATE Feedback 
                        SET Content = @Content, Rating = @Rating, UpdatedAt = @UpdatedAt
                        WHERE UserID = @UserID AND TournamentID = @TournamentID;
                        
                        SELECT * FROM Feedback 
                        WHERE UserID = @UserID AND TournamentID = @TournamentID";

                    feedback.UpdatedAt = DateTime.UtcNow;
                    var updatedFeedback = await connection.QuerySingleAsync<Feedback>(updateSql, new
                    {
                        feedback.Content,
                        feedback.Rating,
                        feedback.UpdatedAt,
                        feedback.UserID,
                        feedback.TournamentID
                    });

                    return updatedFeedback;
                }
                else
                {
                    // Thêm feedback mới
                    const string insertSql = @"
                        INSERT INTO Feedback (TournamentID, UserID, Content, Rating, CreatedAt, UpdatedAt, Status)
                        VALUES (@TournamentID, @UserID, @Content, @Rating, @CreatedAt, @UpdatedAt, @Status);
                        SELECT LAST_INSERT_ID();";

                    feedback.CreatedAt = DateTime.UtcNow;
                    feedback.UpdatedAt = DateTime.UtcNow;
                    feedback.Status = "Active";

                    var feedbackId = await connection.ExecuteScalarAsync<int>(insertSql, feedback);
                    feedback.FeedbackID = feedbackId;

                    _logger.LogDebug("Added new feedback: ID {FeedbackID}, Tournament {TournamentID}",
                        feedback.FeedbackID, feedback.TournamentID);

                    return feedback;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding feedback for tournament {TournamentID}",
                    feedback.TournamentID);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách feedback theo tournament
        /// </summary>
        public async Task<List<Feedback>> GetFeedbackByTournamentAsync(int tournamentId, int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT * FROM Feedback 
                    WHERE TournamentID = @TournamentID AND Status = 'Active'
                    ORDER BY CreatedAt DESC
                    LIMIT @Offset, @PageSize";

                var result = await connection.QueryAsync<Feedback>(sql, new
                {
                    TournamentID = tournamentId,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                });

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback for tournament {TournamentID}", tournamentId);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách feedback theo user
        /// </summary>
        public async Task<List<Feedback>> GetFeedbackByUserAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT * FROM Feedback 
                    WHERE UserID = @UserID AND Status = 'Active'
                    ORDER BY CreatedAt DESC
                    LIMIT @Offset, @PageSize";

                var result = await connection.QueryAsync<Feedback>(sql, new
                {
                    UserID = userId,
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                });

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback for user {UserID}", userId);
                throw;
            }
        }

        /// <summary>
        /// Lấy tất cả feedback
        /// </summary>
        public async Task<List<Feedback>> GetAllFeedbackAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT * FROM Feedback 
                    WHERE Status = 'Active'
                    ORDER BY CreatedAt DESC
                    LIMIT @Offset, @PageSize";

                var result = await connection.QueryAsync<Feedback>(sql, new
                {
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                });

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all feedback");
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem user đã gửi feedback cho tournament này chưa
        /// </summary>
        public async Task<bool> HasFeedbackAsync(int userId, int tournamentId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) FROM Feedback 
                    WHERE UserID = @UserID AND TournamentID = @TournamentID";

                var count = await connection.QuerySingleAsync<int>(sql, new
                {
                    UserID = userId,
                    TournamentID = tournamentId
                });

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserID} has feedback for tournament {TournamentID}",
                    userId, tournamentId);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái feedback
        /// </summary>
        public async Task<bool> UpdateFeedbackStatusAsync(int feedbackId, string status)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Feedback 
                    SET Status = @Status, UpdatedAt = @UpdatedAt
                    WHERE FeedbackID = @FeedbackID";

                var result = await connection.ExecuteAsync(sql, new
                {
                    FeedbackID = feedbackId,
                    Status = status,
                    UpdatedAt = DateTime.UtcNow
                });

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for feedback {FeedbackID}", feedbackId);
                throw;
            }
        }

        /// <summary>
        /// Đếm tổng số feedback
        /// </summary>
        public async Task<int> CountFeedbackAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(*) FROM Feedback 
                    WHERE Status = 'Active'";

                return await connection.QuerySingleAsync<int>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting feedback");
                throw;
            }
        }

        /// <summary>
        /// Lấy rating trung bình của tournament
        /// </summary>
        public async Task<double> GetAverageRatingAsync(int tournamentId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT AVG(Rating) FROM Feedback 
                    WHERE TournamentID = @TournamentID AND Status = 'Active'";

                var result = await connection.QuerySingleAsync<double?>(sql, new { TournamentID = tournamentId });
                return result ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average rating for tournament {TournamentID}", tournamentId);
                throw;
            }
        }

        /// <summary>
        /// Lấy phân bố rating của tournament
        /// </summary>
        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int tournamentId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT Rating, COUNT(*) as Count
                    FROM Feedback 
                    WHERE TournamentID = @TournamentID AND Status = 'Active'
                    GROUP BY Rating
                    ORDER BY Rating";

                var result = await connection.QueryAsync<dynamic>(sql, new { TournamentID = tournamentId });

                var distribution = new Dictionary<int, int>();
                foreach (var item in result)
                {
                    distribution[(int)item.Rating] = (int)item.Count;
                }

                // Đảm bảo có đủ các mức rating từ 1-5
                for (int i = 1; i <= 5; i++)
                {
                    if (!distribution.ContainsKey(i))
                    {
                        distribution[i] = 0;
                    }
                }

                return distribution;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rating distribution for tournament {TournamentID}", tournamentId);
                throw;
            }
        }
    }
}
