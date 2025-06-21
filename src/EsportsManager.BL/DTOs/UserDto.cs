using System;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// User Data Transfer Object - áp dụng Single Responsibility Principle
/// Chỉ chứa dữ liệu cần thiết để transfer giữa các layers
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public required string Role { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// DTO cho user profile
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public required string Role { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int TotalLogins { get; set; }
    public TimeSpan TotalTimeOnline { get; set; }
}
