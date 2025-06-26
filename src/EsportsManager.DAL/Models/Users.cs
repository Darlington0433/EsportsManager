using System;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.DAL.Models;

/// <summary>
/// Users entity - đại diện cho bảng Users trong database
/// Áp dụng Single Responsibility Principle - chỉ chứa dữ liệu
/// Theo cấu trúc database trong tài liệu phân tích
/// </summary>
public class Users
{
    /// <summary>
    /// UserID - Primary Key (tương ứng UserID trong DB)
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// Tên đăng nhập của người dùng (Unique, NOT NULL)
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string Username { get; set; }

    /// <summary>
    /// Email của người dùng (Unique, NOT NULL)
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    /// <summary>
    /// Mật khẩu đã được hash của người dùng (NOT NULL)
    /// </summary>
    [Required]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Họ và tên đầy đủ của người dùng
    /// </summary>
    [StringLength(100)]
    public string? FullName { get; set; }

    /// <summary>
    /// Vai trò của người dùng (Admin, Player, Viewer)
    /// </summary>
    [Required]
    public required string Role { get; set; }

    /// <summary>
    /// Trạng thái của tài khoản (Active, Suspended, Inactive, Pending, Deleted)
    /// </summary>
    [Required]
    public string Status { get; set; } = UsersStatus.Pending;

    /// <summary>
    /// Email đã được xác minh chưa
    /// </summary>
    public bool IsEmailVerified { get; set; } = false;

    /// <summary>
    /// Token để xác minh email
    /// </summary>
    public string? EmailVerificationToken { get; set; }

    /// <summary>
    /// Token để reset mật khẩu
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Thời gian hết hạn của token reset mật khẩu
    /// </summary>
    public DateTime? PasswordResetExpiry { get; set; }

    /// <summary>
    /// Câu hỏi bảo mật để khôi phục mật khẩu
    /// </summary>
    [StringLength(200)]
    public string? SecurityQuestion { get; set; }

    /// <summary>
    /// Câu trả lời bảo mật (đã được hash)
    /// </summary>
    public string? SecurityAnswer { get; set; }

    /// <summary>
    /// Ngày đăng nhập lần cuối
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Ngày tạo tài khoản (timestamp, NOT NULL)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật lần cuối (timestamp)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Users roles constants - áp dụng Open/Closed Principle
/// Theo tài liệu: Admin, Player, Viewer
/// </summary>
public static class UsersRoles
{
    public const string Admin = "Admin";
    public const string Player = "Player";
    public const string Viewer = "Viewer";
}

/// <summary>
/// Users status constants - theo tài liệu nghiệp vụ
/// </summary>
public static class UsersStatus
{
    public const string Active = "Active";
    public const string Suspended = "Suspended";
    public const string Inactive = "Inactive";
    public const string Pending = "Pending";
    public const string Deleted = "Deleted";
}
