using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services
{
    /// <summary>
    /// Dịch vụ quản lý tournament sử dụng stored procedures
    /// </summary>
    public class TournamentService : ITournamentService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<TournamentService> _logger;

        public TournamentService(DataContext dataContext, ILogger<TournamentService> logger)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Helper methods for safe data type conversion
        private static DateTime SafeGetDateTime(object dbValue, DateTime defaultValue = default)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToDateTime(dbValue);
        }

        private static int SafeGetInt32(object dbValue, int defaultValue = 0)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToInt32(dbValue);
        }

        private static decimal SafeGetDecimal(object dbValue, decimal defaultValue = 0)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToDecimal(dbValue);
        }        /// <summary>
                 /// Lấy danh sách tất cả giải đấu
                 /// </summary>
        public async Task<List<TournamentInfoDto>> GetAllTournamentsAsync()
        {
            try
            {
                var result = new List<TournamentInfoDto>();

                // TODO: Triển khai stored procedure sp_GetAllTournaments
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetAllTournaments");

                foreach (DataRow row in dataTable.Rows)
                {
                    var tournament = new TournamentInfoDto
                    {
                        TournamentId = SafeGetInt32(row["TournamentID"]),
                        TournamentName = row["TournamentName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        GameId = SafeGetInt32(row["GameID"]),
                        GameName = row["GameName"]?.ToString() ?? string.Empty,
                        StartDate = SafeGetDateTime(row["StartDate"]),
                        EndDate = SafeGetDateTime(row["EndDate"]),
                        RegistrationDeadline = SafeGetDateTime(row["RegistrationDeadline"]),
                        MaxTeams = SafeGetInt32(row["MaxTeams"]),
                        EntryFee = SafeGetDecimal(row["EntryFee"]),
                        PrizePool = SafeGetDecimal(row["PrizePool"]),
                        Status = row["Status"]?.ToString() ?? string.Empty,
                        RegisteredTeams = SafeGetInt32(row["RegisteredTeams"]),
                        CreatedBy = SafeGetInt32(row["CreatedBy"]),
                        CreatedAt = SafeGetDateTime(row["CreatedAt"])
                    };

                    result.Add(tournament);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tournaments");
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách giải đấu có thể đăng ký tham gia
        /// </summary>
        public async Task<List<TournamentInfoDto>> GetAvailableTournamentsAsync()
        {
            try
            {
                var result = new List<TournamentInfoDto>();

                // TODO: Triển khai stored procedure sp_GetAvailableTournaments
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetAvailableTournaments");

                foreach (DataRow row in dataTable.Rows)
                {
                    var tournament = new TournamentInfoDto
                    {
                        TournamentId = SafeGetInt32(row["TournamentID"]),
                        TournamentName = row["TournamentName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        GameId = SafeGetInt32(row["GameID"]),
                        GameName = row["GameName"]?.ToString() ?? string.Empty,
                        StartDate = SafeGetDateTime(row["StartDate"]),
                        EndDate = SafeGetDateTime(row["EndDate"]),
                        RegistrationDeadline = SafeGetDateTime(row["RegistrationDeadline"]),
                        MaxTeams = SafeGetInt32(row["MaxTeams"]),
                        EntryFee = SafeGetDecimal(row["EntryFee"]),
                        PrizePool = SafeGetDecimal(row["PrizePool"]),
                        Status = row["Status"]?.ToString() ?? string.Empty,
                        RegisteredTeams = SafeGetInt32(row["RegisteredTeams"]),
                        CreatedBy = SafeGetInt32(row["CreatedBy"]),
                        CreatedAt = SafeGetDateTime(row["CreatedAt"])
                    };

                    result.Add(tournament);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available tournaments");
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin giải đấu theo ID
        /// </summary>
        public async Task<TournamentInfoDto?> GetTournamentByIdAsync(int tournamentId)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_GetTournamentById
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentById",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                if (dataTable.Rows.Count == 0)
                {
                    return null;
                }

                DataRow row = dataTable.Rows[0];
                var tournament = new TournamentInfoDto
                {
                    TournamentId = SafeGetInt32(row["TournamentID"]),
                    TournamentName = row["TournamentName"]?.ToString() ?? string.Empty,
                    Description = row["Description"]?.ToString() ?? string.Empty,
                    GameId = SafeGetInt32(row["GameID"]),
                    GameName = row["GameName"]?.ToString() ?? string.Empty,
                    StartDate = SafeGetDateTime(row["StartDate"]),
                    EndDate = SafeGetDateTime(row["EndDate"]),
                    RegistrationDeadline = SafeGetDateTime(row["RegistrationDeadline"]),
                    MaxTeams = SafeGetInt32(row["MaxTeams"]),
                    EntryFee = SafeGetDecimal(row["EntryFee"]),
                    PrizePool = SafeGetDecimal(row["PrizePool"]),
                    Status = row["Status"]?.ToString() ?? string.Empty,
                    RegisteredTeams = SafeGetInt32(row["RegisteredTeams"]),
                    CreatedBy = SafeGetInt32(row["CreatedBy"]),
                    CreatedAt = SafeGetDateTime(row["CreatedAt"])
                };

                return await Task.FromResult(tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tournament with ID {TournamentId}", tournamentId);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách giải đấu mà team đã đăng ký
        /// </summary>
        public async Task<List<TournamentInfoDto>> GetTeamTournamentsAsync(int teamId)
        {
            try
            {
                var result = new List<TournamentInfoDto>();

                // TODO: Triển khai stored procedure sp_GetTeamTournaments
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTeamTournaments",
                    _dataContext.CreateParameter("p_TeamID", teamId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var tournament = new TournamentInfoDto
                    {
                        TournamentId = SafeGetInt32(row["TournamentID"]),
                        TournamentName = row["TournamentName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString() ?? string.Empty,
                        GameId = SafeGetInt32(row["GameID"]),
                        GameName = row["GameName"]?.ToString() ?? string.Empty,
                        StartDate = SafeGetDateTime(row["StartDate"]),
                        EndDate = SafeGetDateTime(row["EndDate"]),
                        RegistrationDeadline = SafeGetDateTime(row["RegistrationDeadline"]),
                        MaxTeams = SafeGetInt32(row["MaxTeams"]),
                        EntryFee = SafeGetDecimal(row["EntryFee"]),
                        PrizePool = SafeGetDecimal(row["PrizePool"]),
                        Status = row["Status"]?.ToString() ?? string.Empty,
                        RegisteredTeams = SafeGetInt32(row["RegisteredTeams"]),
                        CreatedBy = SafeGetInt32(row["CreatedBy"]),
                        CreatedAt = SafeGetDateTime(row["CreatedAt"])
                    };

                    result.Add(tournament);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tournaments for team {TeamId}", teamId);
                throw;
            }
        }

        /// <summary>
        /// Đăng ký team tham gia giải đấu
        /// </summary>
        public async Task<bool> RegisterTeamForTournamentAsync(int tournamentId, int teamId)
        {
            try
            {
                _logger.LogInformation("Attempting to register team {TeamId} for tournament {TournamentId}", teamId, tournamentId);

                // Gọi stored procedure sp_RegisterTeamForTournament
                _dataContext.ExecuteNonQueryStoredProcedure("sp_RegisterTeamForTournament",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId),
                    _dataContext.CreateParameter("p_TeamID", teamId));

                _logger.LogInformation("Successfully registered team {TeamId} for tournament {TournamentId}", teamId, tournamentId);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering team {TeamId} for tournament {TournamentId}: {Message}", teamId, tournamentId, ex.Message);

                // Log chi tiết về loại lỗi
                if (ex.Message.Contains("already registered"))
                {
                    _logger.LogWarning("Team {TeamId} is already registered for tournament {TournamentId}", teamId, tournamentId);
                }
                else if (ex.Message.Contains("Registration is closed"))
                {
                    _logger.LogWarning("Registration is closed for tournament {TournamentId}", tournamentId);
                }
                else if (ex.Message.Contains("maximum number of teams"))
                {
                    _logger.LogWarning("Tournament {TournamentId} has reached maximum number of teams", tournamentId);
                }

                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Hủy đăng ký tham gia giải đấu
        /// </summary>
        public async Task<bool> UnregisterTeamFromTournamentAsync(int tournamentId, int teamId)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_UnregisterTeamFromTournament
                _dataContext.ExecuteNonQueryStoredProcedure("sp_UnregisterTeamFromTournament",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId),
                    _dataContext.CreateParameter("p_TeamID", teamId));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering team {TeamId} from tournament {TournamentId}", teamId, tournamentId);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Tạo giải đấu mới (Admin function)
        /// </summary>
        public async Task<TournamentInfoDto> CreateTournamentAsync(TournamentCreateDto tournamentDto)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_CreateTournament
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_CreateTournament",
                    _dataContext.CreateParameter("p_TournamentName", tournamentDto.TournamentName),
                    _dataContext.CreateParameter("p_Description", tournamentDto.Description),
                    _dataContext.CreateParameter("p_GameID", tournamentDto.GameId),
                    _dataContext.CreateParameter("p_StartDate", tournamentDto.StartDate),
                    _dataContext.CreateParameter("p_EndDate", tournamentDto.EndDate),
                    _dataContext.CreateParameter("p_RegistrationDeadline", tournamentDto.RegistrationDeadline),
                    _dataContext.CreateParameter("p_MaxTeams", tournamentDto.MaxTeams),
                    _dataContext.CreateParameter("p_EntryFee", tournamentDto.EntryFee),
                    _dataContext.CreateParameter("p_PrizePool", tournamentDto.PrizePool),
                    _dataContext.CreateParameter("p_CreatedBy", tournamentDto.CreatedBy));

                if (dataTable.Rows.Count == 0)
                {
                    throw new InvalidOperationException("Failed to create tournament");
                }

                DataRow row = dataTable.Rows[0];
                var tournament = new TournamentInfoDto
                {
                    TournamentId = SafeGetInt32(row["TournamentID"]),
                    TournamentName = tournamentDto.TournamentName,
                    Description = tournamentDto.Description,
                    GameId = tournamentDto.GameId,
                    GameName = row["GameName"]?.ToString() ?? string.Empty,
                    StartDate = tournamentDto.StartDate,
                    EndDate = tournamentDto.EndDate,
                    RegistrationDeadline = tournamentDto.RegistrationDeadline,
                    MaxTeams = tournamentDto.MaxTeams,
                    EntryFee = tournamentDto.EntryFee,
                    PrizePool = tournamentDto.PrizePool,
                    Status = "Draft",
                    RegisteredTeams = 0,
                    CreatedBy = tournamentDto.CreatedBy,
                    CreatedAt = SafeGetDateTime(row["CreatedAt"])
                };

                return await Task.FromResult(tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tournament");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thông tin giải đấu (Admin function)
        /// </summary>
        public async Task<bool> UpdateTournamentAsync(int tournamentId, TournamentUpdateDto tournamentDto)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_UpdateTournament
                _dataContext.ExecuteNonQueryStoredProcedure("sp_UpdateTournament",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId),
                    _dataContext.CreateParameter("p_TournamentName", tournamentDto.TournamentName),
                    _dataContext.CreateParameter("p_Description", tournamentDto.Description),
                    _dataContext.CreateParameter("p_StartDate", tournamentDto.StartDate),
                    _dataContext.CreateParameter("p_EndDate", tournamentDto.EndDate),
                    _dataContext.CreateParameter("p_RegistrationDeadline", tournamentDto.RegistrationDeadline),
                    _dataContext.CreateParameter("p_MaxTeams", tournamentDto.MaxTeams),
                    _dataContext.CreateParameter("p_EntryFee", tournamentDto.EntryFee),
                    _dataContext.CreateParameter("p_PrizePool", tournamentDto.PrizePool),
                    _dataContext.CreateParameter("p_Status", tournamentDto.Status));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tournament with ID {TournamentId}", tournamentId);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Xóa giải đấu (Admin function)
        /// </summary>
        public async Task<bool> DeleteTournamentAsync(int tournamentId)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_DeleteTournament
                _dataContext.ExecuteNonQueryStoredProcedure("sp_DeleteTournament",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tournament with ID {TournamentId}", tournamentId);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Lấy danh sách team tham gia giải đấu
        /// </summary>
        public async Task<List<TeamInfoDto>> GetTournamentTeamsAsync(int tournamentId)
        {
            try
            {
                var result = new List<TeamInfoDto>();

                // TODO: Triển khai stored procedure sp_GetTournamentTeams
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentTeams",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var team = new TeamInfoDto
                    {
                        Id = SafeGetInt32(row["TeamID"]),
                        Name = row["TeamName"]?.ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString(),
                        Logo = row["LogoURL"]?.ToString(),
                        LeaderId = SafeGetInt32(row["TeamLeaderID"]),
                        LeaderName = row["TeamLeaderName"]?.ToString() ?? string.Empty,
                        MemberCount = SafeGetInt32(row["MemberCount"])
                        // Note: RegistrationStatus is not a property of TeamInfoDto
                    };

                    result.Add(team);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams for tournament {TournamentId}", tournamentId);
                throw;
            }
        }

        /// <summary>
        /// Gửi feedback về giải đấu
        /// </summary>
        public async Task<bool> SubmitFeedbackAsync(int userId, FeedbackDto feedbackDto)
        {
            try
            {
                // TODO: Triển khai stored procedure sp_SubmitFeedback
                _dataContext.ExecuteNonQueryStoredProcedure("sp_SubmitFeedback",
                    _dataContext.CreateParameter("p_TournamentID", feedbackDto.TournamentId),
                    _dataContext.CreateParameter("p_UserID", userId),
                    _dataContext.CreateParameter("p_Content", feedbackDto.Content),
                    _dataContext.CreateParameter("p_Rating", feedbackDto.Rating));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback for tournament {TournamentId} by user {UserId}", feedbackDto.TournamentId, userId);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Lấy danh sách feedback của giải đấu
        /// </summary>
        public async Task<List<FeedbackDto>> GetTournamentFeedbackAsync(int tournamentId)
        {
            try
            {
                var result = new List<FeedbackDto>();

                // TODO: Triển khai stored procedure sp_GetTournamentFeedback
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentFeedback",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var feedback = new FeedbackDto
                    {
                        FeedbackId = SafeGetInt32(row["FeedbackID"]),
                        TournamentId = SafeGetInt32(row["TournamentID"]),
                        UserId = SafeGetInt32(row["UserID"]),
                        UserName = row["UserName"]?.ToString() ?? string.Empty,
                        Content = row["Content"]?.ToString() ?? string.Empty,
                        Rating = SafeGetInt32(row["Rating"]),
                        CreatedAt = SafeGetDateTime(row["CreatedAt"]),
                        Status = row["Status"]?.ToString() ?? string.Empty
                    };

                    result.Add(feedback);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback for tournament {TournamentId}", tournamentId);
                throw;
            }
        }        /// <summary>
                 /// Lấy thống kê tournament dựa trên game
                 /// </summary>
                 /// <param name="gameId">ID của game cần lấy thống kê</param>
                 /// <returns>Danh sách thống kê tournament của game</returns>
        public async Task<List<TournamentStatDto>> GetTournamentStatsByGameAsync(int gameId)
        {
            try
            {
                var result = new List<TournamentStatDto>();

                // Sử dụng stored procedure sp_GetTournamentStatsByGame
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentStatsByGame",
                    _dataContext.CreateParameter("p_GameID", gameId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var tournamentStat = new TournamentStatDto
                    {
                        TournamentId = SafeGetInt32(row["TournamentID"]),
                        TournamentName = row["TournamentName"]?.ToString() ?? string.Empty,
                        Status = row["Status"]?.ToString() ?? string.Empty,
                        TeamsParticipating = SafeGetInt32(row["TeamsParticipating"]),
                        PrizePool = SafeGetDecimal(row["PrizePool"]),
                        StartDate = SafeGetDateTime(row["StartDate"]),
                        EndDate = SafeGetDateTime(row["EndDate"])
                    };

                    result.Add(tournamentStat);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tournament stats for game {GameId}", gameId);
                throw;
            }
        }        /// <summary>
                 /// Lấy kết quả chi tiết của tournament
                 /// </summary>
                 /// <param name="tournamentId">ID của tournament</param>
                 /// <returns>Danh sách kết quả của từng đội trong tournament</returns>
        public async Task<List<TournamentResultDto>> GetTournamentResultsAsync(int tournamentId)
        {
            try
            {
                var result = new List<TournamentResultDto>();

                // Sử dụng stored procedure sp_GetTournamentResults
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentResults",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var tournamentResult = new TournamentResultDto
                    {
                        Position = SafeGetInt32(row["Position"]),
                        TeamName = row["TeamName"]?.ToString() ?? string.Empty,
                        PrizeMoney = SafeGetDecimal(row["PrizeMoney"]),
                        Notes = row["Notes"]?.ToString(),
                        TeamLeader = row["TeamLeader"]?.ToString() ?? string.Empty,
                        TeamId = SafeGetInt32(row["TeamID"])
                    };

                    result.Add(tournamentResult);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting results for tournament {TournamentId}", tournamentId);
                throw;
            }
        }        /// <summary>
                 /// Thêm kết quả mới cho tournament
                 /// </summary>
                 /// <param name="tournamentId">ID của tournament</param>
                 /// <param name="teamId">ID của team</param>
                 /// <param name="position">Vị trí xếp hạng</param>
                 /// <param name="prizeMoney">Tiền thưởng</param>
                 /// <param name="notes">Ghi chú</param>
                 /// <returns>true nếu thêm thành công</returns>
        public async Task<bool> AddTournamentResultAsync(int tournamentId, int teamId, int position, decimal prizeMoney, string notes)
        {
            try
            {
                // Sử dụng stored procedure sp_AddTournamentResult
                _dataContext.ExecuteNonQueryStoredProcedure("sp_AddTournamentResult",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId),
                    _dataContext.CreateParameter("p_TeamID", teamId),
                    _dataContext.CreateParameter("p_Position", position),
                    _dataContext.CreateParameter("p_PrizeMoney", prizeMoney),
                    _dataContext.CreateParameter("p_Notes", notes));

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding result for tournament {TournamentId}, team {TeamId}", tournamentId, teamId);
                return await Task.FromResult(false);
            }
        }        /// <summary>
                 /// Lấy bảng xếp hạng đầy đủ của tournament
                 /// </summary>
                 /// <param name="tournamentId">ID của tournament</param>
                 /// <returns>Bảng xếp hạng chi tiết</returns>
        public async Task<List<TournamentLeaderboardDto>> GetTournamentLeaderboardAsync(int tournamentId)
        {
            try
            {
                var result = new List<TournamentLeaderboardDto>();

                // Sử dụng stored procedure sp_GetTournamentLeaderboard
                var dataTable = _dataContext.ExecuteStoredProcedure("sp_GetTournamentLeaderboard",
                    _dataContext.CreateParameter("p_TournamentID", tournamentId));

                foreach (DataRow row in dataTable.Rows)
                {
                    var leaderboardItem = new TournamentLeaderboardDto
                    {
                        Rank = SafeGetInt32(row["RankPosition"]),
                        TeamName = row["TeamName"]?.ToString() ?? string.Empty,
                        Position = SafeGetInt32(row["Position"]),
                        PrizeMoney = SafeGetDecimal(row["PrizeMoney"]),
                        TeamSize = SafeGetInt32(row["TeamSize"]),
                        TeamMembers = row["TeamMembers"]?.ToString()
                    };

                    result.Add(leaderboardItem);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard for tournament {TournamentId}", tournamentId);
                throw;
            }
        }
    }
}
