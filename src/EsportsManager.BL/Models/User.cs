using System;

namespace EsportsManager.BL.Models;

/// <summary>
/// Business layer User model - contains business logic specific properties
/// Renamed to UserBusinessModel to avoid conflicts with DAL User model
/// </summary>
public class UserBusinessModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SecurityQuestion { get; set; }
    public string? SecurityAnswerHash { get; set; }

    // Business logic methods
    public bool IsValidForLogin() => IsActive && !string.IsNullOrEmpty(Username);
    public string GetDisplayName() => string.IsNullOrEmpty(Username) ? Email : Username;
    public bool HasSecurityQuestionSetup() => !string.IsNullOrEmpty(SecurityQuestion) && !string.IsNullOrEmpty(SecurityAnswerHash);
}

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    Viewer = 1,
    Player = 2, 
    Admin = 3
}
