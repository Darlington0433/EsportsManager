using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models = EsportsManager.BL.Models;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// Interface định nghĩa các thao tác quản lý người dùng
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Lấy danh sách tất cả người dùng
    /// </summary>
    Task<BusinessResult<IEnumerable<UserDto>>> GetAllUsersAsync();

    /// <summary>
    /// Xóa người dùng theo ID
    /// </summary>
    Task<BusinessResult> DeleteUserAsync(int userId);

    /// <summary>
    /// Cập nhật vai trò của người dùng
    /// </summary>
    Task<BusinessResult> UpdateUserRoleAsync(int userId, string newRole);

    /// <summary>
    /// Cấm người dùng
    /// </summary>
    Task<BusinessResult> BanUserAsync(int userId);

    /// <summary>
    /// Đăng ký người dùng mới
    /// </summary>
    Task<Models.AuthenticationResult> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Đăng nhập người dùng
    /// </summary>
    Task<Models.AuthenticationResult> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    Task<BusinessResult> UpdateUserAsync(int userId, UpdateUserDto updateDto);

    /// <summary>
    /// Đổi mật khẩu người dùng
    /// </summary>
    Task<BusinessResult> UpdatePasswordAsync(int userId, UpdatePasswordDto updateDto);

    /// <summary>
    /// Khôi phục mật khẩu
    /// </summary>
    Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetDto);

    /// <summary>
    /// Xác thực email người dùng
    /// </summary>
    Task<BusinessResult> VerifyEmailAsync(string token);

    /// <summary>
    /// Gửi lại email xác thực
    /// </summary>
    Task<BusinessResult> ResendVerificationEmailAsync(string email);

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    Task<BusinessResult<UserDto>> GetUserByIdAsync(int userId);

    /// <summary>
    /// Lấy thông tin người dùng theo email
    /// </summary>
    Task<BusinessResult<UserDto>> GetUserByEmailAsync(string email);

    /// <summary>
    /// Lấy thông tin người dùng theo username
    /// </summary>
    Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username);

    // User management methods
    Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role);
    Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync();

    // User profile methods
    Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId);
    Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto);

    // Admin methods
    Task<BusinessResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<BusinessResult> UpdateUserStatusAsync(int userId, string status);
    Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm);
    Task<bool> ToggleUserStatusAsync(int userId);
    Task<string> ResetPasswordAsync(int userId);
    Task<BusinessResult<bool>> DeleteUserWithPermissionCheckAsync(int userId, int requestUserId);

    // Validation methods
    Task<BusinessResult<bool>> IsUsernameAvailableAsync(string username);
    Task<BusinessResult<bool>> IsEmailAvailableAsync(string email);
    Models.ValidationResult ValidateUserData(CreateUserDto createUserDto);
    Models.ValidationResult ValidateLoginData(LoginDto loginDto);

    // Statistics methods
    Task<BusinessResult<int>> GetTotalUsersCountAsync();
    Task<BusinessResult<int>> GetUserCountByRoleAsync(string role);
    Task<BusinessResult<int>> GetActiveUsersCountAsync();

    // Pending account management methods
    Task<BusinessResult<IEnumerable<UserDto>>> GetPendingAccountsAsync();
    Task<BusinessResult> ApproveAccountAsync(int userId);
}
