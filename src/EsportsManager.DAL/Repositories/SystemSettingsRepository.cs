using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Repository for system settings operations - Simplified version that uses stored procedures
    /// </summary>
    public class SystemSettingsRepository : ISystemSettingsRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<SystemSettingsRepository> _logger;

        public SystemSettingsRepository(DataContext context, ILogger<SystemSettingsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region System Settings

        public async Task<List<SystemSetting>> GetAllSettingsAsync()
        {
            try
            {
                // Use stored procedure to get system settings
                var parameters = new IDbDataParameter[] { };
                var dataTable = _context.ExecuteStoredProcedure("GetAllSystemSettings", parameters);

                var settings = new List<SystemSetting>();
                foreach (DataRow row in dataTable.Rows)
                {
                    settings.Add(new SystemSetting
                    {
                        Key = row["Key"].ToString() ?? string.Empty,
                        Value = row["Value"].ToString() ?? string.Empty,
                        Description = row["Description"].ToString() ?? string.Empty,
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }

                return await Task.FromResult(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all system settings");
                // Return empty list instead of throwing to prevent cascading failures
                return new List<SystemSetting>();
            }
        }

        public async Task<SystemSetting?> GetSettingAsync(string key)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@Key", key)
                };

                var dataTable = _context.ExecuteStoredProcedure("GetSystemSetting", parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    return await Task.FromResult(new SystemSetting
                    {
                        Key = row["Key"].ToString() ?? string.Empty,
                        Value = row["Value"].ToString() ?? string.Empty,
                        Description = row["Description"].ToString() ?? string.Empty,
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }

                return await Task.FromResult<SystemSetting?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system setting with key: {Key}", key);
                return null;
            }
        }

        public async Task<bool> UpdateSettingAsync(string key, string value)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@Key", key),
                    _context.CreateParameter("@Value", value),
                    _context.CreateParameter("@UpdatedAt", DateTime.UtcNow)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("UpdateSystemSetting", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating system setting {Key} with value {Value}", key, value);
                return false;
            }
        }

        public async Task<bool> CreateSettingAsync(SystemSetting setting)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@Key", setting.Key),
                    _context.CreateParameter("@Value", setting.Value),
                    _context.CreateParameter("@Description", setting.Description),
                    _context.CreateParameter("@CreatedAt", DateTime.UtcNow),
                    _context.CreateParameter("@UpdatedAt", DateTime.UtcNow)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("CreateSystemSetting", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating system setting with key: {Key}", setting.Key);
                return false;
            }
        }

        public async Task<bool> DeleteSettingAsync(string key)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@Key", key)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("DeleteSystemSetting", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting system setting with key: {Key}", key);
                return false;
            }
        }

        #endregion

        #region Games

        public async Task<List<Game>> GetAllGamesAsync()
        {
            try
            {
                var parameters = new IDbDataParameter[] { };
                var dataTable = _context.ExecuteStoredProcedure("GetAllGames", parameters);

                var games = new List<Game>();
                foreach (DataRow row in dataTable.Rows)
                {
                    games.Add(new Game
                    {
                        GameID = Convert.ToInt32(row["GameID"]),
                        GameName = row["GameName"].ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString(),
                        Genre = row["Genre"]?.ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }

                return await Task.FromResult(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all games");
                return new List<Game>();
            }
        }

        public async Task<Game?> GetGameByIdAsync(int gameId)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@GameID", gameId)
                };

                var dataTable = _context.ExecuteStoredProcedure("GetGameById", parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    return await Task.FromResult(new Game
                    {
                        GameID = Convert.ToInt32(row["GameID"]),
                        GameName = row["GameName"].ToString() ?? string.Empty,
                        Description = row["Description"]?.ToString(),
                        Genre = row["Genre"]?.ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
                    });
                }

                return await Task.FromResult<Game?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game with ID: {GameId}", gameId);
                return null;
            }
        }

        public async Task<bool> AddGameAsync(Game game)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@GameName", game.GameName),
                    _context.CreateParameter("@Description", game.Description ?? (object)DBNull.Value),
                    _context.CreateParameter("@Genre", game.Genre ?? (object)DBNull.Value),
                    _context.CreateParameter("@IsActive", game.IsActive),
                    _context.CreateParameter("@CreatedAt", DateTime.UtcNow),
                    _context.CreateParameter("@UpdatedAt", DateTime.UtcNow)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("AddGame", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding game: {GameName}", game.GameName);
                return false;
            }
        }

        public async Task<bool> UpdateGameAsync(Game game)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@GameID", game.GameID),
                    _context.CreateParameter("@GameName", game.GameName),
                    _context.CreateParameter("@Description", game.Description ?? (object)DBNull.Value),
                    _context.CreateParameter("@Genre", game.Genre ?? (object)DBNull.Value),
                    _context.CreateParameter("@IsActive", game.IsActive),
                    _context.CreateParameter("@UpdatedAt", DateTime.UtcNow)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("UpdateGame", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating game with ID: {GameId}", game.GameID);
                return false;
            }
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@GameID", gameId)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("DeleteGame", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting game with ID: {GameId}", gameId);
                return false;
            }
        }

        #endregion

        #region System Logs

        public async Task<List<SystemLog>> GetSystemLogsAsync(int maxCount = 100, string? logLevel = null)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@MaxCount", maxCount),
                    _context.CreateParameter("@LogLevel", logLevel ?? (object)DBNull.Value)
                };

                var dataTable = _context.ExecuteStoredProcedure("GetSystemLogs", parameters);

                var logs = new List<SystemLog>();
                foreach (DataRow row in dataTable.Rows)
                {
                    logs.Add(new SystemLog
                    {
                        LogID = Convert.ToInt32(row["LogID"]),
                        Timestamp = Convert.ToDateTime(row["Timestamp"]),
                        Level = row["Level"].ToString() ?? string.Empty,
                        Message = row["Message"].ToString() ?? string.Empty,
                        Source = row["Source"].ToString() ?? string.Empty,
                        Exception = row["Exception"]?.ToString() ?? string.Empty
                    });
                }

                return await Task.FromResult(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system logs");
                return new List<SystemLog>();
            }
        }

        public async Task<bool> AddSystemLogAsync(SystemLog log)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@Timestamp", DateTime.UtcNow),
                    _context.CreateParameter("@Level", log.Level),
                    _context.CreateParameter("@Message", log.Message),
                    _context.CreateParameter("@Source", log.Source),
                    _context.CreateParameter("@Exception", log.Exception ?? (object)DBNull.Value)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("AddSystemLog", parameters);
                return await Task.FromResult(rowsAffected > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding system log");
                return false;
            }
        }

        public async Task<bool> ClearOldLogsAsync(DateTime cutoffDate)
        {
            try
            {
                var parameters = new IDbDataParameter[]
                {
                    _context.CreateParameter("@CutoffDate", cutoffDate)
                };

                var rowsAffected = _context.ExecuteNonQueryStoredProcedure("ClearOldLogs", parameters);
                _logger.LogInformation("Cleared {RowsAffected} old log entries before {CutoffDate}", rowsAffected, cutoffDate);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing old logs before: {CutoffDate}", cutoffDate);
                return false;
            }
        }

        #endregion
    }
}
