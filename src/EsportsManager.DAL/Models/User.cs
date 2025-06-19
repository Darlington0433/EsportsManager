using System;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.DAL.Models;

/// <summary>
/// User entity - đại diện cho bảng Users trong database
/// Áp dụng Single Responsibility Principle - chỉ chứa dữ liệu
/// </summary>
public class User
{
    /// <summary>
    /// ID duy nhất của người dùng (Primary Key)
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Tên đăng nhập của người dùng (Unique)
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// Email của người dùng (Unique)
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Mật khẩu đã được hash của người dùng
    /// </summary>
    public required string PasswordHash { get; set; }
    
    /// <summary>
    /// Vai trò của người dùng (Admin, Player, Viewer)
    /// </summary>
    public required string Role { get; set; }
    
    /// <summary>
    /// Trạng thái của tài khoản (Active, Suspended, Inactive)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    /// <summary>
    /// Ngày tạo tài khoản
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Ngày cập nhật lần cuối
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Ngày đăng nhập lần cuối
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// User roles constants - áp dụng Open/Closed Principle
/// </summary>
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Player = "Player";
    public const string Viewer = "Viewer";
}

/// <summary>
/// User status constants
/// </summary>
public static class UserStatus
{
    public const string Active = "Active";
    public const string Suspended = "Suspended";
    public const string Inactive = "Inactive";
}
