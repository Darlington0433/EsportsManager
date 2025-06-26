using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface for system settings repository
    /// </summary>
    public interface ISystemSettingsRepository
    {
        // System Settings
        Task<List<SystemSetting>> GetAllSettingsAsync();
        Task<SystemSetting?> GetSettingAsync(string key);
        Task<bool> UpdateSettingAsync(string key, string value);
        Task<bool> CreateSettingAsync(SystemSetting setting);
        Task<bool> DeleteSettingAsync(string key);

        // Games
        Task<List<Game>> GetAllGamesAsync();
        Task<Game?> GetGameByIdAsync(int gameId);
        Task<bool> AddGameAsync(Game game);
        Task<bool> UpdateGameAsync(Game game);
        Task<bool> DeleteGameAsync(int gameId);

        // System Logs
        Task<List<SystemLog>> GetSystemLogsAsync(int maxCount = 100, string? logLevel = null);
        Task<bool> AddSystemLogAsync(SystemLog log);
        Task<bool> ClearOldLogsAsync(DateTime cutoffDate);
    }
}
