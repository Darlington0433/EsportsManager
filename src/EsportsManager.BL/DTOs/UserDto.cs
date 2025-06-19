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
    public required string Role { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// DTO cho việc tạo user mới
/// </summary>
public class CreateUserDto
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}

/// <summary>
/// DTO cho việc đăng nhập
/// </summary>
public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

/// <summary>
/// DTO cho việc đăng ký
/// </summary>
public class RegisterDto
{
    public required string Username { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}

/// <summary>
/// DTO cho việc cập nhật password
/// </summary>
public class UpdatePasswordDto
{
    public int UserId { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}

/// <summary>
/// DTO cho việc reset password
/// </summary>
public class ResetPasswordDto
{
    public required string Username { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// DTO cho user profile
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public required string Role { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int TotalLogins { get; set; }
    public TimeSpan TotalTimeOnline { get; set; }
}
