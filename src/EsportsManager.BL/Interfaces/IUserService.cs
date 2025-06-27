using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models = EsportsManager.BL.Models;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// User service interface - áp dụng Interface Segregation Principle
/// Chỉ chứa các phương thức liên quan đến User business logic
/// </summary>
public interface IUserService
{
    // Authentication methods
    Task<Models.AuthenticationResult> AuthenticateAsync(LoginDto loginDto);
    Task<BusinessResult<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<BusinessResult> LogoutAsync(int userId);

    // User management methods
    Task<BusinessResult<UserDto>> GetUserByIdAsync(int userId);
    Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username);
    Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role);
    Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync();

    // User profile methods
    Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId);
    Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto);
    Task<BusinessResult> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
    Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    // Admin methods
    Task<BusinessResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<BusinessResult> UpdateUserStatusAsync(int userId, string status);
    Task<List<UserProfileDto>> GetAllUsersAsync();
    Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm);
    Task<bool> ToggleUserStatusAsync(int userId);
    Task<string> ResetPasswordAsync(int userId);
    Task<bool> DeleteUserAsync(int userId, int currentUserId = 0);
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
