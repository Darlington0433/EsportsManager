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
using EsportsManager.DAL.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Team Repository implementation - áp dụng Single Responsibility Principle
    /// Chỉ lo về data access cho Teams entity với database thực tế
    /// </summary>
    public class TeamRepository : BaseRepository<Team, int>, ITeamRepository
    {
        private readonly ILogger<TeamRepository> _logger;

        public TeamRepository(DataContext context, ILogger<TeamRepository> logger) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Team CRUD Operations

        public async Task<Team> CreateAsync(Team team)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    INSERT INTO Teams (TeamName, Description, GameID, CreatedBy, LogoURL, MaxMembers, CreatedAt, IsActive, Status)
                    VALUES (@TeamName, @Description, @GameID, @CreatedBy, @LogoURL, @MaxMembers, @CreatedAt, @IsActive, @Status);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var parameters = new
                {
                    team.TeamName,
                    team.Description,
                    team.GameID,
                    team.CreatedBy,
                    team.LogoURL,
                    team.MaxMembers,
                    team.CreatedAt,
                    team.IsActive,
                    team.Status
                };

                var teamId = await connection.QuerySingleAsync<int>(sql, parameters);
                team.TeamID = teamId;
                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team: {TeamName}", team.TeamName);
                throw;
            }
        }

        public override async Task<Team?> GetByIdAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT t.*, COALESCE(COUNT(tm.UserID), 0) as MemberCount 
                    FROM Teams t
                    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
                    WHERE t.TeamID = @TeamID AND t.IsActive = 1
                    GROUP BY t.TeamID, t.TeamName, t.Description, t.GameID, t.CreatedBy, 
                             t.LogoURL, t.MaxMembers, t.CreatedAt, t.IsActive, t.Status";

                return await connection.QuerySingleOrDefaultAsync<Team>(sql, new { TeamID = teamId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team by ID: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<Team?> GetByNameAsync(string teamName)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT * FROM Teams WHERE TeamName = @TeamName AND IsActive = 1";
                return await connection.QuerySingleOrDefaultAsync<Team>(sql, new { TeamName = teamName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team by name: {TeamName}", teamName);
                throw;
            }
        }

        public async Task<List<Team>> GetAllActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT t.*, COALESCE(COUNT(tm.UserID), 0) as MemberCount 
                    FROM Teams t
                    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
                    WHERE t.IsActive = 1
                    GROUP BY t.TeamID, t.TeamName, t.Description, t.GameID, t.CreatedBy, 
                             t.LogoURL, t.MaxMembers, t.CreatedAt, t.IsActive, t.Status
                    ORDER BY t.TeamName";
                var result = await connection.QueryAsync<Team>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active teams");
                throw;
            }
        }

        public new async Task<bool> UpdateAsync(Team team)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Teams 
                    SET TeamName = @TeamName, Description = @Description, LogoURL = @LogoURL, 
                        MaxMembers = @MaxMembers, Status = @Status
                    WHERE TeamID = @TeamID";

                var parameters = new
                {
                    team.TeamName,
                    team.Description,
                    team.LogoURL,
                    team.MaxMembers,
                    team.Status,
                    team.TeamID
                };

                var affectedRows = await connection.ExecuteAsync(sql, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team: {TeamId}", team.TeamID);
                throw;
            }
        }

        public override async Task<bool> DeleteAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "UPDATE Teams SET IsActive = 0 WHERE TeamID = @TeamID";
                var affectedRows = await connection.ExecuteAsync(sql, new { TeamID = teamId });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<bool> DisbandAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                using var transaction = connection.BeginTransaction();
                try
                {
                    // Remove all members first
                    const string removeMembersSql = "DELETE FROM TeamMembers WHERE TeamID = @TeamID";
                    await connection.ExecuteAsync(removeMembersSql, new { TeamID = teamId }, transaction);

                    // Mark team as inactive
                    const string disbandSql = "UPDATE Teams SET IsActive = 0, Status = 'Disbanded' WHERE TeamID = @TeamID";
                    var result = await connection.ExecuteAsync(disbandSql, new { TeamID = teamId }, transaction);

                    transaction.Commit();
                    return result > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disbanding team: {TeamId}", teamId);
                throw;
            }
        }

        #endregion

        #region Team Search and Filter

        public async Task<List<Team>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT t.*, COALESCE(COUNT(tm.UserID), 0) as MemberCount 
                    FROM Teams t
                    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
                    WHERE t.TeamName LIKE @SearchTerm AND t.IsActive = 1 
                    GROUP BY t.TeamID, t.TeamName, t.Description, t.GameID, t.CreatedBy, 
                             t.LogoURL, t.MaxMembers, t.CreatedAt, t.IsActive, t.Status
                    ORDER BY t.TeamName";

                var result = await connection.QueryAsync<Team>(sql, new { SearchTerm = $"%{searchTerm}%" });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching teams by name: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<List<Team>> GetByGameIdAsync(int gameId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT * FROM Teams WHERE GameID = @GameID AND IsActive = 1 ORDER BY TeamName";
                var result = await connection.QueryAsync<Team>(sql, new { GameID = gameId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams by game ID: {GameId}", gameId);
                throw;
            }
        }

        public async Task<List<Team>> GetByCreatorAsync(int creatorId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT * FROM Teams WHERE CreatedBy = @CreatedBy AND IsActive = 1 ORDER BY TeamName";
                var result = await connection.QueryAsync<Team>(sql, new { CreatedBy = creatorId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams by creator: {CreatorId}", creatorId);
                throw;
            }
        }

        #endregion

        #region Team Member Operations

        public async Task<bool> AddMemberAsync(int teamId, int userId, bool isLeader = false, string? position = null)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    INSERT INTO TeamMembers (TeamID, UserID, JoinDate, IsLeader, Position, Status)
                    VALUES (@TeamID, @UserID, @JoinDate, @IsLeader, @Position, @Status)";

                var parameters = new
                {
                    TeamID = teamId,
                    UserID = userId,
                    JoinDate = DateTime.UtcNow,
                    IsLeader = isLeader,
                    Position = position,
                    Status = "Active"
                };

                var affectedRows = await connection.ExecuteAsync(sql, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to team: {TeamId}, {UserId}", teamId, userId);
                throw;
            }
        }

        public async Task<bool> RemoveMemberAsync(int teamId, int userId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "DELETE FROM TeamMembers WHERE TeamID = @TeamID AND UserID = @UserID";
                var affectedRows = await connection.ExecuteAsync(sql, new { TeamID = teamId, UserID = userId });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing member from team: {TeamId}, {UserId}", teamId, userId);
                throw;
            }
        }

        public async Task<bool> UpdateMemberAsync(int teamId, int userId, bool? isLeader = null, string? position = null, string? status = null)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var setParts = new List<string>();
                var parameters = new Dictionary<string, object>
                {
                    ["TeamID"] = teamId,
                    ["UserID"] = userId
                };

                if (isLeader.HasValue)
                {
                    setParts.Add("IsLeader = @IsLeader");
                    parameters["IsLeader"] = isLeader.Value;
                }

                if (position != null)
                {
                    setParts.Add("Position = @Position");
                    parameters["Position"] = position;
                }

                if (status != null)
                {
                    setParts.Add("Status = @Status");
                    parameters["Status"] = status;
                }

                if (setParts.Count == 0) return false;

                var sql = $"UPDATE TeamMembers SET {string.Join(", ", setParts)} WHERE TeamID = @TeamID AND UserID = @UserID";
                var affectedRows = await connection.ExecuteAsync(sql, parameters);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member: {TeamId}, {UserId}", teamId, userId);
                throw;
            }
        }

        public async Task<List<TeamMember>> GetTeamMembersAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT tm.*, u.Username 
                    FROM TeamMembers tm
                    INNER JOIN Users u ON tm.UserID = u.UserID
                    WHERE tm.TeamID = @TeamID AND tm.Status = 'Active'
                    ORDER BY tm.IsLeader DESC, tm.JoinDate ASC";
                var result = await connection.QueryAsync<TeamMember>(sql, new { TeamID = teamId });
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team members: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT * FROM TeamMembers WHERE TeamID = @TeamID AND UserID = @UserID";
                return await connection.QuerySingleOrDefaultAsync<TeamMember>(sql, new { TeamID = teamId, UserID = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team member: {TeamId}, {UserId}", teamId, userId);
                throw;
            }
        }

        public async Task<Team?> GetPlayerTeamAsync(int playerId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT t.*, COALESCE(COUNT(tm2.UserID), 0) as MemberCount 
                    FROM Teams t
                    INNER JOIN TeamMembers tm ON t.TeamID = tm.TeamID
                    LEFT JOIN TeamMembers tm2 ON t.TeamID = tm2.TeamID AND tm2.Status = 'Active'
                    WHERE tm.UserID = @UserID AND tm.Status = 'Active' AND t.IsActive = 1
                    GROUP BY t.TeamID, t.TeamName, t.Description, t.GameID, t.CreatedBy, 
                             t.LogoURL, t.MaxMembers, t.CreatedAt, t.IsActive, t.Status";

                return await connection.QuerySingleOrDefaultAsync<Team>(sql, new { UserID = playerId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player team: {PlayerId}", playerId);
                throw;
            }
        }

        public async Task<bool> IsPlayerInTeamAsync(int playerId, int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) FROM TeamMembers 
                    WHERE UserID = @UserID AND TeamID = @TeamID AND Status = 'Active'";

                var count = await connection.QuerySingleAsync<int>(sql, new { UserID = playerId, TeamID = teamId });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if player is in team: {PlayerId}, {TeamId}", playerId, teamId);
                throw;
            }
        }

        public async Task<bool> IsTeamLeaderAsync(int playerId, int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) FROM TeamMembers 
                    WHERE UserID = @UserID AND TeamID = @TeamID AND IsLeader = 1 AND Status = 'Active'";

                var count = await connection.QuerySingleAsync<int>(sql, new { UserID = playerId, TeamID = teamId });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if player is team leader: {PlayerId}, {TeamId}", playerId, teamId);
                throw;
            }
        }

        #endregion

        #region Team Statistics

        public async Task<int> GetMemberCountAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(*) FROM TeamMembers WHERE TeamID = @TeamID AND Status = 'Active'";
                return await connection.QuerySingleAsync<int>(sql, new { TeamID = teamId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting member count: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<bool> CanAddMemberAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT CASE WHEN t.MaxMembers > (SELECT COUNT(*) FROM TeamMembers WHERE TeamID = @TeamID AND Status = 'Active')
                           THEN 1 ELSE 0 END
                    FROM Teams t WHERE t.TeamID = @TeamID";

                var canAdd = await connection.QuerySingleAsync<bool>(sql, new { TeamID = teamId });
                return canAdd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if can add member: {TeamId}", teamId);
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(int teamId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Teams WHERE TeamID = @TeamID AND IsActive = 1";
                var count = await connection.QuerySingleAsync<int>(sql, new { TeamID = teamId });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if team exists: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<bool> IsTeamNameExistsAsync(string teamName)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Teams WHERE TeamName = @TeamName AND IsActive = 1";
                var count = await connection.QuerySingleAsync<int>(sql, new { TeamName = teamName });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if team name exists: {TeamName}", teamName);
                throw;
            }
        }

        #endregion

        #region Advanced Operations

        public async Task<bool> TransferLeadershipAsync(int teamId, int currentLeaderId, int newLeaderId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                using var transaction = connection.BeginTransaction();
                try
                {
                    // Remove leadership from current leader
                    const string removeLeaderSql = @"
                        UPDATE TeamMembers SET IsLeader = 0 
                        WHERE TeamID = @TeamID AND UserID = @CurrentLeaderID";

                    await connection.ExecuteAsync(removeLeaderSql,
                        new { TeamID = teamId, CurrentLeaderID = currentLeaderId }, transaction);

                    // Give leadership to new leader
                    const string setLeaderSql = @"
                        UPDATE TeamMembers SET IsLeader = 1 
                        WHERE TeamID = @TeamID AND UserID = @NewLeaderID";

                    var result = await connection.ExecuteAsync(setLeaderSql,
                        new { TeamID = teamId, NewLeaderID = newLeaderId }, transaction);

                    transaction.Commit();
                    return result > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring leadership: {TeamId}, {CurrentLeaderId}, {NewLeaderId}",
                    teamId, currentLeaderId, newLeaderId);
                throw;
            }
        }

        public async Task<List<Team>> GetTeamsWithMemberCountAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT t.*, COUNT(tm.UserID) as MemberCount
                    FROM Teams t
                    LEFT JOIN TeamMembers tm ON t.TeamID = tm.TeamID AND tm.Status = 'Active'
                    WHERE t.IsActive = 1
                    GROUP BY t.TeamID, t.TeamName, t.Description, t.GameID, t.CreatedBy, 
                             t.LogoURL, t.MaxMembers, t.CreatedAt, t.IsActive, t.Status
                    ORDER BY t.TeamName";

                var result = await connection.QueryAsync<Team>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams with member count");
                throw;
            }
        }

        public async Task<bool> IsPlayerLeaderOfAnyTeamAsync(int playerId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT COUNT(1) FROM TeamMembers 
                    WHERE UserID = @UserID AND IsLeader = 1 AND Status = 'Active'";

                var count = await connection.QuerySingleAsync<int>(sql, new { UserID = playerId });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if player is leader of any team: {PlayerId}", playerId);
                throw;
            }
        }

        #endregion
    }
}