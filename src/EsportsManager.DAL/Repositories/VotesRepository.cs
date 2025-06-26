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
    /// Repository implementation cho bảng Votes
    /// </summary>
    public class VotesRepository : IVotesRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<VotesRepository> _logger;

        public VotesRepository(DataContext context, ILogger<VotesRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Thêm vote mới
        /// </summary>
        public async Task<Vote> AddVoteAsync(Vote vote)
        {
            try
            {
                using var connection = _context.CreateConnection();

                // Kiểm tra nếu user đã vote cho đối tượng này rồi thì update vote cũ
                if (await HasVotedAsync(vote.VoterID, vote.VoteType, vote.TargetID))
                {
                    const string updateSql = @"
                        UPDATE Votes 
                        SET Rating = @Rating, Comment = @Comment, CreatedAt = @CreatedAt
                        WHERE VoterID = @VoterID AND VoteType = @VoteType AND TargetID = @TargetID;
                        
                        SELECT * FROM Votes 
                        WHERE VoterID = @VoterID AND VoteType = @VoteType AND TargetID = @TargetID;";

                    vote.CreatedAt = DateTime.UtcNow;
                    var updatedVote = await connection.QuerySingleAsync<Vote>(updateSql, vote);
                    return updatedVote;
                }
                else
                {
                    // Thêm vote mới
                    const string insertSql = @"
                        INSERT INTO Votes (VoterID, VoteType, TargetID, Rating, Comment, CreatedAt)
                        VALUES (@VoterID, @VoteType, @TargetID, @Rating, @Comment, @CreatedAt);
                        SELECT LAST_INSERT_ID();";

                    vote.CreatedAt = DateTime.UtcNow;
                    var voteId = await connection.ExecuteScalarAsync<int>(insertSql, vote);
                    vote.VoteID = voteId;

                    _logger.LogDebug("Added new vote: ID {VoteID}, Type {VoteType}, Target {TargetID}",
                        vote.VoteID, vote.VoteType, vote.TargetID);

                    return vote;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vote: {VoteType}, {TargetID}", vote.VoteType, vote.TargetID);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách votes theo VoteType và TargetID
        /// </summary>
        public async Task<List<Vote>> GetVotesByTargetAsync(string voteType, int targetId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT * FROM Votes 
                    WHERE VoteType = @VoteType AND TargetID = @TargetID
                    ORDER BY CreatedAt DESC";

                var votes = await connection.QueryAsync<Vote>(sql, new { VoteType = voteType, TargetID = targetId });
                return votes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting votes by target: {VoteType}, {TargetID}", voteType, targetId);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách votes theo người vote
        /// </summary>
        public async Task<List<Vote>> GetVotesByVoterAsync(int voterId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT * FROM Votes 
                    WHERE VoterID = @VoterID
                    ORDER BY CreatedAt DESC";

                var votes = await connection.QueryAsync<Vote>(sql, new { VoterID = voterId });
                return votes.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting votes by voter: {VoterID}", voterId);
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra xem user đã vote cho đối tượng này chưa
        /// </summary>
        public async Task<bool> HasVotedAsync(int voterId, string voteType, int targetId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) FROM Votes 
                    WHERE VoterID = @VoterID AND VoteType = @VoteType AND TargetID = @TargetID";

                var count = await connection.QuerySingleAsync<int>(sql,
                    new { VoterID = voterId, VoteType = voteType, TargetID = targetId });

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user has voted: {VoterID}, {VoteType}, {TargetID}",
                    voterId, voteType, targetId);
                throw;
            }
        }

        /// <summary>
        /// Tìm kiếm votes theo nhiều tiêu chí
        /// </summary>
        public async Task<List<Vote>> SearchVotesAsync(
            int? voterId = null,
            string username = null,
            string voteType = null,
            int? targetId = null,
            int? minRating = null,
            int? maxRating = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var conditions = new List<string>();
                var parameters = new DynamicParameters();

                if (voterId.HasValue)
                {
                    conditions.Add("v.VoterID = @VoterID");
                    parameters.Add("VoterID", voterId.Value);
                }

                if (!string.IsNullOrEmpty(username))
                {
                    conditions.Add("u.Username LIKE @Username");
                    parameters.Add("Username", $"%{username}%");
                }

                if (!string.IsNullOrEmpty(voteType))
                {
                    conditions.Add("v.VoteType = @VoteType");
                    parameters.Add("VoteType", voteType);
                }

                if (targetId.HasValue)
                {
                    conditions.Add("v.TargetID = @TargetID");
                    parameters.Add("TargetID", targetId.Value);
                }

                if (minRating.HasValue)
                {
                    conditions.Add("v.Rating >= @MinRating");
                    parameters.Add("MinRating", minRating.Value);
                }

                if (maxRating.HasValue)
                {
                    conditions.Add("v.Rating <= @MaxRating");
                    parameters.Add("MaxRating", maxRating.Value);
                }

                if (fromDate.HasValue)
                {
                    conditions.Add("v.CreatedAt >= @FromDate");
                    parameters.Add("FromDate", fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    conditions.Add("v.CreatedAt <= @ToDate");
                    parameters.Add("ToDate", toDate.Value);
                }

                var whereClause = conditions.Count > 0
                    ? $"WHERE {string.Join(" AND ", conditions)}"
                    : string.Empty;

                var sql = $@"
                    SELECT v.*, u.Username 
                    FROM Votes v
                    INNER JOIN Users u ON v.VoterID = u.UserID
                    {whereClause}
                    ORDER BY v.CreatedAt DESC
                    LIMIT @Offset, @PageSize";

                parameters.Add("Offset", (page - 1) * pageSize);
                parameters.Add("PageSize", pageSize);

                var result = await connection.QueryAsync<dynamic>(sql, parameters);

                var votes = new List<Vote>();
                foreach (var item in result)
                {
                    votes.Add(new Vote
                    {
                        VoteID = item.VoteID,
                        VoterID = item.VoterID,
                        VoteType = item.VoteType,
                        TargetID = item.TargetID,
                        Rating = item.Rating,
                        Comment = item.Comment,
                        CreatedAt = item.CreatedAt
                    });
                }

                return votes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching votes");
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng votes theo loại
        /// </summary>
        public async Task<int> GetVoteCountByTypeAsync(string voteType)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(*) FROM Votes 
                    WHERE VoteType = @VoteType";

                return await connection.QuerySingleAsync<int>(sql, new { VoteType = voteType });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vote count by type: {VoteType}", voteType);
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng người vote độc nhất
        /// </summary>
        public async Task<int> GetUniqueVotersCountAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(DISTINCT VoterID) FROM Votes";

                return await connection.QuerySingleAsync<int>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unique voters count");
                throw;
            }
        }

        /// <summary>
        /// Lấy thống kê votes theo tháng
        /// </summary>
        public async Task<Dictionary<string, int>> GetVotesByMonthAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT 
                        DATE_FORMAT(CreatedAt, '%Y-%m') AS Month,
                        COUNT(*) AS Count
                    FROM Votes
                    GROUP BY DATE_FORMAT(CreatedAt, '%Y-%m')
                    ORDER BY Month";

                var result = await connection.QueryAsync<dynamic>(sql);
                return result.ToDictionary(
                    row => (string)row.Month,
                    row => (int)row.Count
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting votes by month");
                throw;
            }
        }
    }
}
