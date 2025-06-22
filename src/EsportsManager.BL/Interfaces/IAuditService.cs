using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// Audit Service Interface - Quản lý audit log và security
/// Áp dụng Single Responsibility Principle cho logging và tracking
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Log hành động của Admin
    /// </summary>
    Task LogAdminActionAsync(int adminUserId, string action, string? details = null, object? additionalData = null);

    /// <summary>
    /// Log hành động của User
    /// </summary>
    Task LogUserActionAsync(int userId, string action, string? details = null);

    /// <summary>
    /// Log lỗi hệ thống
    /// </summary>
    Task LogSystemErrorAsync(string source, Exception exception, string? additionalContext = null);

    /// <summary>
    /// Log đăng nhập/đăng xuất
    /// </summary>
    Task LogAuthenticationAsync(int userId, string action, bool success, string? ipAddress = null);

    /// <summary>
    /// Lấy audit logs theo user
    /// </summary>
    Task<List<AuditLogDto>> GetUserAuditLogsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// Lấy audit logs theo action type
    /// </summary>
    Task<List<AuditLogDto>> GetAuditLogsByActionAsync(string action, DateTime? fromDate = null, DateTime? toDate = null);    /// <summary>
    /// Lấy tất cả audit logs (chỉ Admin)
    /// </summary>
    Task<PagedResult<AuditLogDto>> GetAllAuditLogsAsync(int pageNumber = 1, int pageSize = 50);

    /// <summary>
    /// Xóa audit logs cũ (theo policy)
    /// </summary>
    Task CleanupOldLogsAsync(TimeSpan retentionPeriod);
}
