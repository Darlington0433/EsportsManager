using EsportsManager.DAL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace EsportsManager.DAL.Interfaces;

/// <summary>
/// Users repository interface - áp dụng Interface Segregation Principle
/// Chỉ chứa các phương thức liên quan đến Users
/// Theo tài liệu nghiệp vụ
/// </summary>
public interface IUsersRepository : IRepository<Users, int>
{
    /// <summary>
    /// Lấy user theo username
    /// </summary>
    Task<Users?> GetByUsernameAsync(string username);

    /// <summary>
    /// Lấy user theo email
    /// </summary>
    Task<Users?> GetByEmailAsync(string email);

    /// <summary>
    /// Lấy danh sách user theo role
    /// </summary>
    Task<IEnumerable<Users>> GetByRoleAsync(string role);

    /// <summary>
    /// Kiểm tra username có tồn tại không
    /// </summary>
    Task<bool> IsUsernameExistsAsync(string username);

    /// <summary>
    /// Kiểm tra email có tồn tại không
    /// </summary>
    Task<bool> IsEmailExistsAsync(string email);

    /// <summary>
    /// Lấy danh sách user đang active
    /// </summary>
    Task<IEnumerable<Users>> GetActiveUsersAsync();

    /// <summary>
    /// Cập nhật mật khẩu user
    /// </summary>
    Task<bool> UpdatePasswordAsync(int userId, string passwordHash);

    /// <summary>
    /// Cập nhật trạng thái user
    /// </summary>
    Task<bool> UpdateStatusAsync(int userId, string status);

    /// <summary>
    /// Cập nhật token xác minh email
    /// </summary>
    Task<bool> UpdateEmailVerificationTokenAsync(int userId, string token);

    /// <summary>
    /// Cập nhật token reset mật khẩu
    /// </summary>
    Task<bool> UpdatePasswordResetTokenAsync(int userId, string token, DateTime expiry);

    /// <summary>
    /// Xác minh email
    /// </summary>
    Task<bool> VerifyEmailAsync(string token);

    /// <summary>
    /// Cập nhật thời gian đăng nhập cuối
    /// </summary>
    Task<bool> UpdateLastLoginAsync(int userId);

    /// <summary>
    /// Lấy danh sách user cần duyệt (Pending)
    /// </summary>
    Task<IEnumerable<Users>> GetPendingUsersAsync();

    /// <summary>
    /// Lấy user theo token xác thực email
    /// </summary>
    Task<Users?> GetByEmailVerificationTokenAsync(string token);

    /// <summary>
    /// Lấy user theo token reset mật khẩu
    /// </summary>
    Task<Users?> GetByPasswordResetTokenAsync(string token);

    /// <summary>
    /// Lấy danh sách user theo trạng thái
    /// </summary>
    Task<IEnumerable<Users>> GetByStatusAsync(string status);

    /// <summary>
    /// Lấy số lượng user theo trạng thái
    /// </summary>
    Task<int> GetCountByStatusAsync(string status);

    /// <summary>
    /// Lấy số lượng user theo vai trò
    /// </summary>
    Task<int> GetCountByRoleAsync(string role);

    /// <summary>
    /// Tìm kiếm user theo từ khóa
    /// </summary>
    Task<IEnumerable<Users>> SearchAsync(string searchTerm);
}
