using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho game
    /// </summary>
    public class GameDto
    {
        public int GameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO cho cài đặt tournament
    /// </summary>
    public class TournamentSettingsDto
    {
        public int DefaultMaxTeams { get; set; } = 16;
        public int DefaultMaxPlayersPerTeam { get; set; } = 5;
        public bool RequireTeamApproval { get; set; } = true;
        public bool AllowPublicVoting { get; set; } = true;
        public int DefaultMinPrizePool { get; set; } = 1000;
        public string DefaultCurrency { get; set; } = "USD";
        public int MinimumTeamsForStart { get; set; } = 4;
        public bool EnableAutoScheduling { get; set; } = false;
    }

    /// <summary>
    /// DTO cho cài đặt wallet
    /// </summary>
    public class WalletSettingsDto
    {
        public decimal MinimumDepositAmount { get; set; } = 10;
        public decimal MaximumDepositAmount { get; set; } = 10000;
        public decimal MinimumWithdrawalAmount { get; set; } = 20;
        public decimal TransactionFeePercent { get; set; } = 2.5m;
        public int WithdrawalProcessingTimeHours { get; set; } = 48;
        public bool EnableDonations { get; set; } = true;
        public int MaxDailyTransactions { get; set; } = 5;
        public bool RequireEmailVerificationForWithdrawals { get; set; } = true;
    }

    /// <summary>
    /// DTO cho backup
    /// </summary>
    public class BackupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long SizeBytes { get; set; }
        public string Type { get; set; } = "Full"; // Full, Differential, Log
    }

    /// <summary>
    /// DTO cho system log
    /// </summary>
    public class SystemLogDto
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty; // Info, Warning, Error, Critical
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho sức khỏe hệ thống
    /// </summary>
    public class SystemHealthDto
    {
        public bool DatabaseConnected { get; set; }
        public string DatabaseVersion { get; set; } = string.Empty;
        public long DatabaseSizeBytes { get; set; }
        public int ActiveConnections { get; set; }
        public List<string> MissingTables { get; set; } = new List<string>();
        public Dictionary<string, bool> ServiceStatus { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, long> TableRowCounts { get; set; } = new Dictionary<string, long>();
        public long FreeStorageSpaceBytes { get; set; }
    }
}
