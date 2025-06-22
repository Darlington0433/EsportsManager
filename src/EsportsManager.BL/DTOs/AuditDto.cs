using System;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho audit log
/// </summary>
public class AuditLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Action { get; set; }
    public string? Details { get; set; }
    public string? TargetType { get; set; }
    public int? TargetId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public AuditLogLevel Level { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DTO cho security log
/// </summary>
public class SecurityLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public required string EventType { get; set; }
    public required string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public SecurityEventLevel Severity { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
    public int AttemptCount { get; set; }
}

/// <summary>
/// DTO cho system log
/// </summary>
public class SystemLogDto
{
    public int Id { get; set; }
    public required string EventType { get; set; }
    public required string Description { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
    public SystemLogLevel Level { get; set; }
    public string? Source { get; set; }
    public string? StackTrace { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Enum cho mức độ audit log
/// </summary>
public enum AuditLogLevel
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

/// <summary>
/// Enum cho mức độ security event
/// </summary>
public enum SecurityEventLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Enum cho mức độ system log
/// </summary>
public enum SystemLogLevel
{
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}
