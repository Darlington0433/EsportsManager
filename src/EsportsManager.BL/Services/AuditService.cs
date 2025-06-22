using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

/// <summary>
/// Audit Service Implementation - Mock implementation for development
/// Production: Replace with real database logging and security tracking
/// </summary>
public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private static readonly List<AuditLogDto> _auditLogs = new();
    private static int _nextLogId = 1;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task LogAdminActionAsync(int adminUserId, string action, string? details = null, object? additionalData = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty", nameof(action));

        try
        {
            var logEntry = new AuditLogDto
            {
                Id = _nextLogId++,
                UserId = adminUserId,
                Username = $"admin_{adminUserId}", // Mock username
                Action = action,
                Details = details,
                Timestamp = DateTime.Now,
                Level = AuditLogLevel.Info,
                Success = true,
                IpAddress = "127.0.0.1", // Mock IP
                UserAgent = "AdminController"
            };

            _auditLogs.Add(logEntry);
            
            _logger.LogInformation("Admin action logged: {Action} by user {UserId}", action, adminUserId);
            await Task.Delay(10); // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log admin action: {Action} for user {UserId}", action, adminUserId);
            throw;
        }
    }

    public async Task LogUserActionAsync(int userId, string action, string? details = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty", nameof(action));

        try
        {
            var logEntry = new AuditLogDto
            {
                Id = _nextLogId++,
                UserId = userId,
                Username = $"user_{userId}", // Mock username
                Action = action,
                Details = details,
                Timestamp = DateTime.Now,
                Level = AuditLogLevel.Info,
                Success = true,
                IpAddress = "192.168.1.100", // Mock IP
                UserAgent = "UserController"
            };

            _auditLogs.Add(logEntry);
            
            _logger.LogInformation("User action logged: {Action} by user {UserId}", action, userId);
            await Task.Delay(5);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log user action: {Action} for user {UserId}", action, userId);
            // Don't throw for user actions to avoid breaking user experience
        }
    }

    public async Task LogSystemErrorAsync(string source, Exception exception, string? additionalContext = null)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        try
        {
            var logEntry = new AuditLogDto
            {
                Id = _nextLogId++,
                UserId = 0, // System user
                Username = "SYSTEM",
                Action = "SYSTEM_ERROR",
                Details = $"Source: {source}, Error: {exception.Message}",
                Timestamp = DateTime.Now,
                Level = AuditLogLevel.Error,
                Success = false,
                ErrorMessage = exception.ToString(),
                IpAddress = "localhost"
            };

            _auditLogs.Add(logEntry);
            
            _logger.LogError(exception, "System error logged from {Source}: {Message}", source, exception.Message);
            await Task.Delay(15);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to log system error from {Source}", source);
        }
    }

    public async Task LogAuthenticationAsync(int userId, string action, bool success, string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty", nameof(action));

        try
        {
            var logEntry = new AuditLogDto
            {
                Id = _nextLogId++,
                UserId = userId,
                Username = $"user_{userId}",
                Action = action,
                Details = success ? "Thành công" : "Thất bại",
                Timestamp = DateTime.Now,
                Level = success ? AuditLogLevel.Info : AuditLogLevel.Warning,
                Success = success,
                IpAddress = ipAddress ?? "Unknown",
                UserAgent = "AuthService"
            };

            _auditLogs.Add(logEntry);
            
            _logger.LogInformation("Authentication logged: {Action} for user {UserId}, Success: {Success}", 
                action, userId, success);
            await Task.Delay(8);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log authentication: {Action} for user {UserId}", action, userId);
        }
    }

    public async Task<List<AuditLogDto>> GetUserAuditLogsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        await Task.Delay(30);

        var query = _auditLogs.Where(log => log.UserId == userId);

        if (fromDate.HasValue)
            query = query.Where(log => log.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(log => log.Timestamp <= toDate.Value);

        return query.OrderByDescending(log => log.Timestamp).ToList();
    }

    public async Task<List<AuditLogDto>> GetAuditLogsByActionAsync(string action, DateTime? fromDate = null, DateTime? toDate = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be null or empty", nameof(action));

        await Task.Delay(40);

        var query = _auditLogs.Where(log => log.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

        if (fromDate.HasValue)
            query = query.Where(log => log.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(log => log.Timestamp <= toDate.Value);

        return query.OrderByDescending(log => log.Timestamp).ToList();
    }    public async Task<PagedResult<AuditLogDto>> GetAllAuditLogsAsync(int pageNumber = 1, int pageSize = 50)
    {
        if (pageNumber < 1)
            throw new ArgumentException("Page number must be >= 1", nameof(pageNumber));

        if (pageSize < 1 || pageSize > 1000)
            throw new ArgumentException("Page size must be between 1 and 1000", nameof(pageSize));

        await Task.Delay(50);

        var skip = (pageNumber - 1) * pageSize;
        var totalCount = _auditLogs.Count;
        
        var pagedLogs = _auditLogs
            .OrderByDescending(log => log.Timestamp)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return new PagedResult<AuditLogDto>
        {
            Items = pagedLogs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task CleanupOldLogsAsync(TimeSpan retentionPeriod)
    {
        var cutoffDate = DateTime.Now - retentionPeriod;
        
        try
        {
            var logsToRemove = _auditLogs
                .Where(log => log.Timestamp < cutoffDate)
                .ToList();

            foreach (var log in logsToRemove)
            {
                _auditLogs.Remove(log);
            }

            _logger.LogInformation("Cleaned up {Count} old audit logs older than {CutoffDate}", 
                logsToRemove.Count, cutoffDate);
            
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old audit logs");
            throw;
        }
    }

    // Helper method to get audit statistics
    public async Task<Dictionary<string, object>> GetAuditStatsAsync()
    {
        await Task.Delay(30);

        var stats = new Dictionary<string, object>
        {
            { "TotalLogs", _auditLogs.Count },
            { "LogsToday", _auditLogs.Count(l => l.Timestamp.Date == DateTime.Today) },
            { "ErrorLogsCount", _auditLogs.Count(l => l.Level == AuditLogLevel.Error || l.Level == AuditLogLevel.Critical) },
            { "UniqueUsers", _auditLogs.Select(l => l.UserId).Distinct().Count() },
            { "TopActions", _auditLogs.GroupBy(l => l.Action)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count()) }
        };

        return stats;
    }

    // Initialize with some mock data
    static AuditService()
    {
        var mockActions = new[]
        {
            ("LOGIN", "Đăng nhập hệ thống"),
            ("CREATE_TOURNAMENT", "Tạo giải đấu mới"),
            ("UPDATE_USER", "Cập nhật thông tin người dùng"),
            ("DELETE_USER", "Xóa người dùng"),
            ("VIEW_STATS", "Xem thống kê hệ thống")
        };

        var random = new Random();
        
        for (int i = 0; i < 100; i++)
        {
            var action = mockActions[random.Next(mockActions.Length)];
            _auditLogs.Add(new AuditLogDto
            {
                Id = _nextLogId++,
                UserId = random.Next(1, 20),
                Username = $"user_{random.Next(1, 20)}",
                Action = action.Item1,
                Details = action.Item2,
                Timestamp = DateTime.Now.AddDays(-random.Next(0, 30)).AddHours(-random.Next(0, 24)),
                Level = AuditLogLevel.Info,
                Success = random.Next(100) > 5, // 95% success rate
                IpAddress = $"192.168.1.{random.Next(1, 255)}",
                UserAgent = "MockData"
            });
        }
    }
}
