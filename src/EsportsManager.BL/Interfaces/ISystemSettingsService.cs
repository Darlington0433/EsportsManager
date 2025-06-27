using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Interface cho các dịch vụ liên quan đến cài đặt hệ thống
    /// </summary>
    public interface ISystemSettingsService
    {
        /// <summary>
        /// Lấy tất cả cài đặt hệ thống
        /// </summary>
        Task<Dictionary<string, string>> GetAllSettingsAsync();

        /// <summary>
        /// Cập nhật một cài đặt hệ thống
        /// </summary>
        Task<bool> UpdateSettingAsync(string key, string value);

        /// <summary>
        /// Lấy danh sách games
        /// </summary>
        Task<List<GameDto>> GetAllGamesAsync();

        /// <summary>
        /// Thêm game mới
        /// </summary>
        Task<bool> AddGameAsync(GameDto game);

        /// <summary>
        /// Cập nhật thông tin game
        /// </summary>
        Task<bool> UpdateGameAsync(GameDto game);

        /// <summary>
        /// Xóa game
        /// </summary>
        Task<bool> DeleteGameAsync(int gameId);

        /// <summary>
        /// Lấy cài đặt tournament
        /// </summary>
        Task<TournamentSettingsDto> GetTournamentSettingsAsync();

        /// <summary>
        /// Cập nhật cài đặt tournament
        /// </summary>
        Task<bool> UpdateTournamentSettingsAsync(TournamentSettingsDto settings);

        /// <summary>
        /// Lấy cài đặt wallet
        /// </summary>
        Task<WalletSettingsDto> GetWalletSettingsAsync();

        /// <summary>
        /// Cập nhật cài đặt wallet
        /// </summary>
        Task<bool> UpdateWalletSettingsAsync(WalletSettingsDto settings);

        /// <summary>
        /// Tạo backup database
        /// </summary>
        Task<string> CreateBackupAsync(string? backupName = null);

        /// <summary>
        /// Lấy danh sách các backups
        /// </summary>
        Task<List<BackupDto>> GetBackupsAsync();

        /// <summary>
        /// Phục hồi database từ backup
        /// </summary>
        Task<bool> RestoreFromBackupAsync(string backupPath);

        /// <summary>
        /// Lấy system logs
        /// </summary>
        Task<List<SystemLogDto>> GetSystemLogsAsync(int maxCount = 100, string? logLevel = null);

        /// <summary>
        /// Kiểm tra sức khỏe hệ thống
        /// </summary>
        Task<SystemHealthDto> CheckSystemHealthAsync();
    }
}
