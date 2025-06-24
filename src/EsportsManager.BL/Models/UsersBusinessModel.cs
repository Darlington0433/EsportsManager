using System;

namespace EsportsManager.BL.Models;

/// <summary>
/// Business layer Users model - contains business logic specific properties
/// Renamed to UsersBusinessModel to avoid conflicts with DAL Users model
/// Chuẩn hóa theo tài liệu nghiệp vụ
/// </summary>
public class UsersBusinessModel
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? SecurityQuestion { get; set; }
    public string? SecurityAnswerHash { get; set; }

    // Business logic methods
    public bool IsValidForLogin() => Status == "Active" && IsEmailVerified && !string.IsNullOrEmpty(Username);
    public string GetDisplayName() => !string.IsNullOrEmpty(FullName) ? FullName : Username;
    public bool HasSecurityQuestionSetup() => !string.IsNullOrEmpty(SecurityQuestion) && !string.IsNullOrEmpty(SecurityAnswerHash);
    public bool IsActive() => Status == "Active";
    public bool IsPending() => Status == "Pending";
    public bool IsSuspended() => Status == "Suspended";
    public bool IsDeleted() => Status == "Deleted";
    public bool IsAdmin() => Role == "Admin";
    public bool IsPlayer() => Role == "Player";
    public bool IsViewer() => Role == "Viewer";
}

/// <summary>
/// User roles in the system - chuẩn hóa theo tài liệu
/// Deprecated: Sử dụng UsersRoles constants thay thế
/// </summary>
public enum UserRole
{
    Viewer = 1,
    Player = 2,
    Admin = 3
}
