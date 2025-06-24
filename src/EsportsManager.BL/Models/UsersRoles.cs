// Enum và constants định nghĩa các role trong hệ thống
// Chuẩn hóa theo tài liệu nghiệp vụ

namespace EsportsManager.BL.Models;

/// <summary>
/// Enum định nghĩa các role (deprecated - sử dụng UsersRoles constants)
/// </summary>
public enum UserRoleType
{
    Viewer = 1,
    Player = 2,
    Admin = 3
}

/// <summary>
/// Users roles constants - theo tài liệu nghiệp vụ
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
