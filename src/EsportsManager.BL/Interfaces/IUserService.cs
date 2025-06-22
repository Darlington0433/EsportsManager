using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

namespace EsportsManager.BL.Interfaces;

/// <summary>
/// User service interface - áp dụng Interface Segregation Principle
/// Enhanced with async/await patterns và Vietnamese esports support
/// </summary>
public interface IUserService
{
    // Authentication methods
    Task<AuthenticationResult> AuthenticateAsync(LoginDto loginDto);
    Task<BusinessResult<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<BusinessResult> LogoutAsync(int userId);

    // Enhanced user management methods with pagination and filtering
    Task<PagedResult<UserProfileDto>> GetAllUsersAsync(
        int pageNumber = 1, 
        int pageSize = 20,
        string? roleFilter = null,
        string? statusFilter = null,
        CancellationToken cancellationToken = default);

    Task<List<UserProfileDto>> SearchUsersAsync(
        string searchTerm, 
        SearchType searchType = SearchType.Contains,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username);
    Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role);
    Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync();

    // Enhanced user operations
    Task<UserStatusChangeResult> ToggleUserStatusAsync(
        int userId, 
        string newStatus = "Toggle",
        CancellationToken cancellationToken = default);

    Task<PasswordResetResult> ResetPasswordAsync(
        int userId, 
        bool sendEmail = true,
        CancellationToken cancellationToken = default);

    Task<UserDeletionResult> DeleteUserAsync(
        int userId, 
        string confirmationCode,
        bool hardDelete = false,
        CancellationToken cancellationToken = default);

    // Bulk operations
    Task<BulkOperationResult> BulkUpdateUsersAsync(
        List<int> userIds,
        BulkUserOperation operation,
        object? operationData = null,
        CancellationToken cancellationToken = default);

    // User profile methods
    Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId);
    Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto);
    Task<BusinessResult> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
    Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    // Admin methods
    Task<BusinessResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<BusinessResult> UpdateUserStatusAsync(int userId, string status);

    // Validation methods
    Task<BusinessResult<bool>> IsUsernameAvailableAsync(string username);
    Task<BusinessResult<bool>> IsEmailAvailableAsync(string email);
    ValidationResult ValidateUserData(CreateUserDto createUserDto);
    ValidationResult ValidateLoginData(LoginDto loginDto);

    // Statistics methods
    Task<BusinessResult<int>> GetTotalUsersCountAsync();
    Task<BusinessResult<int>> GetUserCountByRoleAsync(string role);
    Task<BusinessResult<int>> GetActiveUsersCountAsync();
}
