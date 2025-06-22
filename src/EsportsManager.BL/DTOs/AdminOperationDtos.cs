using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// Generic paged result cho pagination
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Result cho thay đổi trạng thái user
/// </summary>
public class UserStatusChangeResult
{
    public bool Success { get; set; }
    public string OldStatus { get; set; } = "";
    public string NewStatus { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime ChangedAt { get; set; }
}

/// <summary>
/// Result cho reset password
/// </summary>
public class PasswordResetResult
{
    public bool Success { get; set; }
    public string? NewPassword { get; set; }
    public bool EmailSent { get; set; }
    public string Message { get; set; } = "";
    public DateTime ResetAt { get; set; }
}

/// <summary>
/// Result cho xóa user
/// </summary>
public class UserDeletionResult
{
    public bool Success { get; set; }
    public bool RelatedDataCleaned { get; set; }
    public List<string> CleanedDataTypes { get; set; } = new();
    public string Message { get; set; } = "";
    public DateTime DeletedAt { get; set; }
}

/// <summary>
/// Result cho tạo tournament
/// </summary>
public class TournamentCreationResult
{
    public bool Success { get; set; }
    public TournamentDto? Tournament { get; set; }
    public string Message { get; set; } = "";
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Result cho bulk operations
/// </summary>
public class BulkOperationResult
{
    public bool Success { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkOperationError> Errors { get; set; } = new();
    public string Message { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Error details cho bulk operations
/// </summary>
public class BulkOperationError
{
    public int UserId { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string ErrorCode { get; set; } = "";
}

/// <summary>
/// Result cho export operations
/// </summary>
public class ExportResult
{
    public bool Success { get; set; }
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public int RecordCount { get; set; }
    public string Message { get; set; } = "";
    public DateTime ExportedAt { get; set; }
    public long FileSizeBytes { get; set; }
}

/// <summary>
/// DTO cho activity statistics
/// </summary>
public class ActivityStatsDto
{
    public int TotalActivities { get; set; }
    public int ActiveUsersInPeriod { get; set; }
    public int NewRegistrations { get; set; }
    public int TournamentsCreated { get; set; }
    public int TournamentsCompleted { get; set; }
    public decimal TotalRevenue { get; set; }
    public Dictionary<string, int> ActivityByType { get; set; } = new();
    public List<DailyActivityDto> DailyBreakdown { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

/// <summary>
/// Enum cho search types
/// </summary>
public enum SearchType
{
    Contains = 1,
    Exact = 2,
    StartsWith = 3,
    EndsWith = 4
}

/// <summary>
/// Enum cho bulk user operations
/// </summary>
public enum BulkUserOperation
{
    Activate = 1,
    Deactivate = 2,
    Ban = 3,
    Unban = 4,
    ChangeRole = 5,
    ResetPassword = 6,
    Delete = 7
}

/// <summary>
/// Enum cho export types
/// </summary>
public enum ExportType
{
    Users = 1,
    Tournaments = 2,
    AuditLogs = 3,
    Statistics = 4,
    Revenue = 5,
    PlayerStats = 6
}

/// <summary>
/// Enum cho export formats
/// </summary>
public enum ExportFormat
{
    Excel = 1,
    CSV = 2,
    PDF = 3,
    JSON = 4
}
