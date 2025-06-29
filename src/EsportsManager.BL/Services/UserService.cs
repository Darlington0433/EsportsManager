using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.BL.Utilities;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Services;

/// <summary>
/// Service xử lý logic người dùng
/// 
/// SECURITY FEATURES:
/// - Password hashing sử dụng BCrypt
/// - Input validation comprehensive
/// - Không log sensitive data (passwords)
/// - Status checking (Active/Inactive users)
/// </summary>
public class UserService : IUserService
{
    private readonly IUsersRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Constructor với dependency injection
    /// </summary>
    /// <param name="userRepository">IUsersRepository</param>
    /// <param name="logger">Logger</param>
    public UserService(IUsersRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Phương thức được gọi từ bên ngoài để xác thực người dùng
    /// Thực hiện validation và kiểm tra thông tin đăng nhập
    /// 
    /// SECURITY FEATURES:
    /// - Không log password
    /// - Chỉ log failed login attempts
    /// - Kiểm tra status của user
    /// - Trả về các error message chung chung
    /// </summary>
    /// <param name="loginDto">Thông tin đăng nhập (username, password)</param>
    /// <returns>AuthenticationResult chứa thông tin user nếu thành công</returns>
    public async Task<Models.AuthenticationResult> AuthenticateAsync(LoginDto loginDto)
    {
        try
        {
            // Validate input data first
            var validation = ValidateLoginData(loginDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Login attempt failed during validation: {Username}", loginDto.Username);
                return Models.AuthenticationResult.Failure(string.Join("; ", validation.Errors));
            }

            // Get user from repository
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            // Check if user exists
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed for non-existent user: {Username}", loginDto.Username);
                return Models.AuthenticationResult.Failure("Invalid username or password");
            }

            // Check if user account is active
            if (user.Status != EsportsManager.DAL.Models.UsersStatus.Active)
            {
                _logger.LogWarning("Login attempt for inactive account: {Username}", loginDto.Username);
                return Models.AuthenticationResult.Failure("Account is not active");
            }            // Xác thực mật khẩu
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login attempt with invalid password: {Username}", loginDto.Username);
                return Models.AuthenticationResult.Failure("Invalid username or password");
            }

            // Update last login time
            await _userRepository.UpdateLastLoginAsync(user.UserID);

            _logger.LogInformation("User logged in: {Username}", user.Username);

            return Models.AuthenticationResult.Success(user.UserID, user.Username, user.Role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user authentication: {Username}", loginDto.Username);
            return Models.AuthenticationResult.Failure("An error occurred during authentication");
        }
    }

    /// <summary>
    /// Đăng ký người dùng mới (thông thường)
    /// </summary>
    /// <param name="registerDto">Thông tin đăng ký</param>
    /// <returns>Kết quả đăng ký và thông tin user mới</returns>
    public async Task<BusinessResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Validate registration data
            var validation = ValidateRegisterData(registerDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Registration failed during validation for: {Username}", registerDto.Username);
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            // Check if username already exists
            bool usernameExists = await _userRepository.IsUsernameExistsAsync(registerDto.Username);
            if (usernameExists)
            {
                _logger.LogWarning("Registration failed - username already exists: {Username}", registerDto.Username);
                return BusinessResult<UserDto>.Failure("Username already exists");
            }

            // Check if email already exists
            bool emailExists = await _userRepository.IsEmailExistsAsync(registerDto.Email);
            if (emailExists)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                return BusinessResult<UserDto>.Failure("Email address already exists");
            }

            // Hash password
            string passwordHash = PasswordHasher.HashPassword(registerDto.Password);

            // Hash security answer
            string hashedSecurityAnswer = !string.IsNullOrEmpty(registerDto.SecurityAnswer)
                ? PasswordHasher.HashPassword(registerDto.SecurityAnswer)
                : string.Empty;

            // Create verification token
            string emailVerificationToken = Guid.NewGuid().ToString("N");

            // Create new user
            var user = new EsportsManager.DAL.Models.Users
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                FullName = registerDto.FullName,
                Role = !string.IsNullOrEmpty(registerDto.Role) ? registerDto.Role : EsportsManager.DAL.Models.UsersRoles.Viewer, // Use provided role or default to Viewer
                Status = EsportsManager.DAL.Models.UsersStatus.Pending, // Start as Pending for Admin approval
                EmailVerificationToken = emailVerificationToken,
                IsEmailVerified = false,
                SecurityQuestion = registerDto.SecurityQuestion,
                SecurityAnswer = hashedSecurityAnswer,
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            var createdUser = await _userRepository.AddAsync(user);

            // TODO: Send verification email (implement in email service)

            // Map to UserDTO
            var userDto = MapUserToDto(createdUser);

            _logger.LogInformation("New user registered: {Username}", registerDto.Username);

            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration: {Username}", registerDto.Username);
            return BusinessResult<UserDto>.Failure("An error occurred during registration");
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    public async Task<BusinessResult> LogoutAsync(int userId)
    {
        try
        {
            // Add any logout logic here (e.g. invalidate tokens, update last activity)
            await Task.CompletedTask; // Add this to avoid compiler warning about lack of await
            _logger.LogInformation("User logged out: {UserId}", userId);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user logout: {UserId}", userId);
            return BusinessResult.Failure("An error occurred during logout");
        }
    }

    /// <summary>
    /// Lấy thông tin user theo ID
    /// </summary>
    /// <param name="userId">ID của user cần tìm</param>
    /// <returns>UserDTO</returns>
    public async Task<BusinessResult<UserDto>> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            var userDto = MapUserToDto(user);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return BusinessResult<UserDto>.Failure("An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Lấy thông tin user theo username
    /// </summary>
    /// <param name="username">Username của user</param>
    /// <returns>UserDTO</returns>
    public async Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            var userDto = MapUserToDto(user);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username: {Username}", username);
            return BusinessResult<UserDto>.Failure("An error occurred while retrieving the user");
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả user
    /// </summary>
    /// <returns>Danh sách UserDTO</returns>
    public async Task<List<UserProfileDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var userProfiles = users.Select(u => new UserProfileDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                TotalLogins = 0, // TODO: Implement method to get login count
                IsEmailVerified = u.IsEmailVerified,
                AvatarUrl = null, // Not in the current schema
                Bio = null // Not in the current schema
            }).ToList();

            return userProfiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return new List<UserProfileDto>();
        }
    }

    /// <summary>
    /// Lấy danh sách user theo role
    /// </summary>
    /// <param name="role">Role của user</param>
    /// <returns>Danh sách UserDTO</returns>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role)
    {
        try
        {
            var users = await _userRepository.GetByRoleAsync(role);
            var userDtos = users.Select(u => MapUserToDto(u)).ToList();
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role: {Role}", role);
            return BusinessResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Lấy danh sách user đang active
    /// </summary>
    /// <returns>Danh sách UserDTO</returns>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync();
            var userDtos = users.Select(u => MapUserToDto(u)).ToList();
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return BusinessResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Lấy thông tin profile của user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>UserProfileDTO</returns>
    public async Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserProfileDto>.Failure("User not found");
            }

            // Map to UserProfileDTO with additional data
            var userProfileDto = new UserProfileDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                TotalLogins = 0, // TODO: Implement method to get login count
                IsEmailVerified = user.IsEmailVerified,
                AvatarUrl = null, // Not in the current schema
                Bio = null // Not in the current schema
            };

            return BusinessResult<UserProfileDto>.Success(userProfileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile: {UserId}", userId);
            return BusinessResult<UserProfileDto>.Failure("An error occurred while retrieving the user profile");
        }
    }

    /// <summary>
    /// Cập nhật profile user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <param name="userDto">UserDTO</param>
    /// <returns>UserDTO cập nhật</returns>
    public async Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto)
    {
        try
        {
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            // Check if email is changed and if it already exists
            if (!string.IsNullOrEmpty(userDto.Email) && userDto.Email != user.Email)
            {
                bool emailExists = await _userRepository.IsEmailExistsAsync(userDto.Email);
                if (emailExists)
                {
                    return BusinessResult<UserDto>.Failure("Email address already exists");
                }
            }

            // Update user properties
            // Only allow specific fields to be updated (not role or status)
            user.Email = userDto.Email ?? user.Email;
            user.FullName = userDto.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            // Save to database
            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedUserDto = MapUserToDto(updatedUser);

            return BusinessResult<UserDto>.Success(updatedUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
            return BusinessResult<UserDto>.Failure("An error occurred while updating the user profile");
        }
    }

    /// <summary>
    /// Cập nhật mật khẩu
    /// </summary>
    /// <param name="updatePasswordDto">DTO chứa thông tin cập nhật mật khẩu</param>
    /// <returns>Kết quả cập nhật</returns>
    public async Task<BusinessResult> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
    {
        try
        {
            var validation = ValidateUpdatePasswordData(updatePasswordDto);
            if (!validation.IsValid)
            {
                return BusinessResult.Failure(validation.Errors);
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(updatePasswordDto.UserId);
            if (user == null)
            {
                return BusinessResult.Failure("User not found");
            }

            // Verify current password
            bool isCurrentPasswordValid = PasswordHasher.VerifyPassword(
                updatePasswordDto.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                return BusinessResult.Failure("Current password is incorrect");
            }

            // Hash new password
            string newPasswordHash = PasswordHasher.HashPassword(updatePasswordDto.NewPassword);

            // Update password
            bool success = await _userRepository.UpdatePasswordAsync(user.UserID, newPasswordHash);
            if (!success)
            {
                return BusinessResult.Failure("Failed to update password");
            }

            _logger.LogInformation("Password updated for user: {Username}", user.Username);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password: {UserId}", updatePasswordDto.UserId);
            return BusinessResult.Failure("An error occurred while updating the password");
        }
    }

    /// <summary>
    /// Reset mật khẩu
    /// </summary>
    /// <param name="resetPasswordDto">DTO chứa thông tin reset mật khẩu</param>
    /// <returns>Kết quả reset</returns>
    public async Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var validation = ValidateResetPasswordData(resetPasswordDto);
            if (!validation.IsValid)
            {
                return BusinessResult.Failure(validation.Errors);
            }

            // Get user by reset token
            EsportsManager.DAL.Models.Users? user = null;
            if (!string.IsNullOrEmpty(resetPasswordDto.Token))
            {
                user = await _userRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token);
            }
            else if (!string.IsNullOrEmpty(resetPasswordDto.Email))
            {
                user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
            }

            if (user == null)
            {
                return BusinessResult.Failure("Invalid or expired token");
            }

            // Check if token is expired
            if (user.PasswordResetExpiry.HasValue && user.PasswordResetExpiry < DateTime.UtcNow)
            {
                return BusinessResult.Failure("Reset token has expired");
            }

            // Hash new password
            string tempPasswordHash = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);

            // Update password and clear reset token
            var success = await _userRepository.UpdatePasswordAsync(user.UserID, tempPasswordHash);
            if (success)
            {
                // Add method to clear password reset token
                await _userRepository.UpdatePasswordResetTokenAsync(user.UserID, string.Empty, DateTime.MinValue);
                _logger.LogInformation("Password reset for user: {Username}", user.Username);
                return BusinessResult.Success();
            }

            return BusinessResult.Failure("Failed to reset password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return BusinessResult.Failure("An error occurred while resetting the password");
        }
    }

    /// <summary>
    /// Tạo người dùng mới (admin)
    /// </summary>
    /// <param name="createUserDto">DTO chứa thông tin người dùng mới</param>
    /// <returns>UserDTO</returns>
    public async Task<BusinessResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            var validation = ValidateUserData(createUserDto);
            if (!validation.IsValid)
            {
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            // Check if username already exists
            bool usernameExists = await _userRepository.IsUsernameExistsAsync(createUserDto.Username);
            if (usernameExists)
            {
                return BusinessResult<UserDto>.Failure("Username already exists");
            }

            // Check if email already exists
            bool emailExists = await _userRepository.IsEmailExistsAsync(createUserDto.Email);
            if (emailExists)
            {
                return BusinessResult<UserDto>.Failure("Email address already exists");
            }

            // Hash password
            string passwordHash = PasswordHasher.HashPassword(createUserDto.Password);

            // Create new user
            var user = new EsportsManager.DAL.Models.Users
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = passwordHash,
                FullName = createUserDto.FullName,
                Role = createUserDto.Role ?? EsportsManager.DAL.Models.UsersRoles.Viewer, // Default to Viewer if not specified
                Status = createUserDto.Status ?? EsportsManager.DAL.Models.UsersStatus.Active, // Default to Active if not specified
                IsEmailVerified = true, // Admin-created accounts are verified by default
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            var createdUser = await _userRepository.AddAsync(user);

            // Map to DTO
            var userDto = MapUserToDto(createdUser);

            _logger.LogInformation("New user created by admin: {Username}", createUserDto.Username);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", createUserDto.Username);
            return BusinessResult<UserDto>.Failure("An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Cập nhật trạng thái user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <param name="status">Trạng thái mới</param>
    /// <returns>Kết quả cập nhật</returns>
    public async Task<BusinessResult> UpdateUserStatusAsync(int userId, string status)
    {
        try
        {
            // Validate status
            if (!IsValidStatus(status))
            {
                return BusinessResult.Failure("Invalid status value");
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("User not found");
            }

            // Update status
            user.Status = status;
            user.UpdatedAt = DateTime.UtcNow;

            // Save to database
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {Username} status updated to {Status}", user.Username, status);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status: {UserId}, Status: {Status}", userId, status);
            return BusinessResult.Failure("An error occurred while updating the user status");
        }
    }

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    /// <param name="userId">ID của user cần xóa</param>
    /// <param name="currentUserId">ID của user thực hiện yêu cầu (không sử dụng trong method này)</param>
    /// <returns>Kết quả xóa</returns>
    public async Task<bool> DeleteUserAsync(int userId, int currentUserId = 0)
    {
        try
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Soft delete by setting status to Deleted
            user.Status = EsportsManager.DAL.Models.UsersStatus.Deleted;
            user.UpdatedAt = DateTime.UtcNow;

            // Save to database
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User deleted (soft delete): {Username}", user.Username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Xóa người dùng với kiểm tra quyền (Admin không thể xóa chính mình hoặc Admin khác)
    /// </summary>
    /// <param name="userId">ID của user cần xóa</param>
    /// <param name="requestUserId">ID của user thực hiện yêu cầu</param>
    /// <returns>Kết quả xóa</returns>
    public async Task<BusinessResult<bool>> DeleteUserWithPermissionCheckAsync(int userId, int requestUserId)
    {
        try
        {
            // Get user to be deleted
            var targetUser = await _userRepository.GetByIdAsync(userId);
            if (targetUser == null)
            {
                return BusinessResult<bool>.Failure("User not found");
            }

            // Get requesting user
            var requestUser = await _userRepository.GetByIdAsync(requestUserId);
            if (requestUser == null)
            {
                return BusinessResult<bool>.Failure("Request user not found");
            }

            // Admin cannot delete themselves
            if (requestUser.Role == "Admin" && requestUserId == userId)
            {
                return BusinessResult<bool>.Failure("Admin cannot delete their own account");
            }

            // Admin can only delete Player/Viewer, not other Admins
            if (requestUser.Role == "Admin" && targetUser.Role == "Admin")
            {
                return BusinessResult<bool>.Failure("Admin cannot delete other Admin accounts");
            }

            // Only Admin can delete users
            if (requestUser.Role != "Admin")
            {
                return BusinessResult<bool>.Failure("Only Admin can delete users");
            }

            // Perform soft delete
            targetUser.Status = EsportsManager.DAL.Models.UsersStatus.Deleted;
            targetUser.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(targetUser);

            _logger.LogInformation("User deleted by admin: {Username} deleted by {AdminUsername}",
                targetUser.Username, requestUser.Username);

            return BusinessResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId} by user: {RequestUserId}", userId, requestUserId);
            return BusinessResult<bool>.Failure("An error occurred while deleting the user");
        }
    }

    /// <summary>
    /// Kiểm tra username có khả dụng không
    /// </summary>
    /// <param name="username">Username cần kiểm tra</param>
    /// <returns>true nếu username chưa tồn tại</returns>
    public async Task<BusinessResult<bool>> IsUsernameAvailableAsync(string username)
    {
        try
        {
            bool exists = await _userRepository.IsUsernameExistsAsync(username);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username availability: {Username}", username);
            return BusinessResult<bool>.Failure("An error occurred while checking username availability");
        }
    }

    /// <summary>
    /// Kiểm tra email có khả dụng không
    /// </summary>
    /// <param name="email">Email cần kiểm tra</param>
    /// <returns>true nếu email chưa tồn tại</returns>
    public async Task<BusinessResult<bool>> IsEmailAvailableAsync(string email)
    {
        try
        {
            bool exists = await _userRepository.IsEmailExistsAsync(email);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email availability: {Email}", email);
            return BusinessResult<bool>.Failure("An error occurred while checking email availability");
        }
    }

    /// <summary>
    /// Lấy tổng số người dùng
    /// </summary>
    /// <returns>Số lượng user</returns>
    public async Task<BusinessResult<int>> GetTotalUsersCountAsync()
    {
        try
        {
            int count = await _userRepository.CountAsync();
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total users count");
            return BusinessResult<int>.Failure("An error occurred while getting the total users count");
        }
    }

    /// <summary>
    /// Lấy số người dùng theo role
    /// </summary>
    /// <param name="role">Role cần đếm</param>
    /// <returns>Số lượng user theo role</returns>
    public async Task<BusinessResult<int>> GetUserCountByRoleAsync(string role)
    {
        try
        {
            // Implementation needed in the repository
            var users = await _userRepository.GetByRoleAsync(role);
            int count = users.Count();
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user count by role: {Role}", role);
            return BusinessResult<int>.Failure("An error occurred while getting the user count");
        }
    }

    /// <summary>
    /// Lấy số người dùng đang active
    /// </summary>
    /// <returns>Số lượng user active</returns>
    public async Task<BusinessResult<int>> GetActiveUsersCountAsync()
    {
        try
        {
            int count = await _userRepository.GetCountByStatusAsync(EsportsManager.DAL.Models.UsersStatus.Active);
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users count");
            return BusinessResult<int>.Failure("An error occurred while getting the active users count");
        }
    }

    /// <summary>
    /// Validate dữ liệu create user
    /// </summary>
    /// <param name="createUserDto">CreateUserDTO</param>
    /// <returns>ValidationResult</returns>
    public Models.ValidationResult ValidateUserData(CreateUserDto createUserDto)
    {
        var errors = new List<string>();

        // Validate username
        if (string.IsNullOrWhiteSpace(createUserDto.Username))
            errors.Add("Username is required");

        // Validate email
        if (string.IsNullOrWhiteSpace(createUserDto.Email))
            errors.Add("Email is required");

        // Validate password
        if (string.IsNullOrWhiteSpace(createUserDto.Password))
            errors.Add("Password is required");

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    /// <summary>
    /// Validate dữ liệu đăng nhập
    /// </summary>
    /// <param name="loginDto">LoginDTO</param>
    /// <returns>ValidationResult</returns>
    public Models.ValidationResult ValidateLoginData(LoginDto loginDto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(loginDto.Username))
            errors.Add("Username is required");

        if (string.IsNullOrWhiteSpace(loginDto.Password))
            errors.Add("Password is required");

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    /// <summary>
    /// Validate thông tin đăng ký
    /// </summary>
    /// <param name="registerDto">RegisterDTO</param>
    /// <returns>ValidationResult</returns>
    private Models.ValidationResult ValidateRegisterData(RegisterDto registerDto)
    {
        var errors = new List<string>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(registerDto.Username))
            errors.Add("Username is required");

        if (string.IsNullOrWhiteSpace(registerDto.Email))
            errors.Add("Email is required");

        if (string.IsNullOrWhiteSpace(registerDto.Password))
            errors.Add("Password is required");

        if (string.IsNullOrWhiteSpace(registerDto.ConfirmPassword))
            errors.Add("Confirm password is required");

        if (string.IsNullOrWhiteSpace(registerDto.FullName))
            errors.Add("Full name is required");

        if (string.IsNullOrWhiteSpace(registerDto.SecurityQuestion))
            errors.Add("Security question is required");

        if (string.IsNullOrWhiteSpace(registerDto.SecurityAnswer))
            errors.Add("Security answer is required");

        // Validate password match
        if (registerDto.Password != registerDto.ConfirmPassword)
            errors.Add("Password and confirm password do not match");

        // Validate email format
        if (!string.IsNullOrWhiteSpace(registerDto.Email) && !IsValidEmail(registerDto.Email))
            errors.Add("Invalid email format");

        // Validate username length
        if (!string.IsNullOrWhiteSpace(registerDto.Username) &&
            (registerDto.Username.Length < 3 || registerDto.Username.Length > 50))
            errors.Add("Username must be between 3 and 50 characters");

        // Validate password strength
        if (!string.IsNullOrWhiteSpace(registerDto.Password) && registerDto.Password.Length < 6)
            errors.Add("Password must be at least 6 characters long");

        // Validate role
        if (!string.IsNullOrWhiteSpace(registerDto.Role) &&
            registerDto.Role != "Player" && registerDto.Role != "Viewer")
            errors.Add("Invalid role. Only Player and Viewer roles are allowed for registration");

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    /// <summary>
    /// Validate email format
    /// </summary>
    /// <param name="email">Email to validate</param>
    /// <returns>True if valid email format</returns>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validate thông tin đổi mật khẩu
    /// </summary>
    /// <param name="updatePasswordDto">UpdatePasswordDTO</param>
    /// <returns>ValidationResult</returns>
    private Models.ValidationResult ValidateUpdatePasswordData(UpdatePasswordDto updatePasswordDto)
    {
        return string.IsNullOrWhiteSpace(updatePasswordDto.CurrentPassword)
            ? Models.ValidationResult.Failure("Current password is required")
            : Models.ValidationResult.Success();
    }

    /// <summary>
    /// Validate thông tin reset mật khẩu
    /// </summary>
    /// <param name="resetPasswordDto">ResetPasswordDTO</param>
    /// <returns>ValidationResult</returns>
    private Models.ValidationResult ValidateResetPasswordData(ResetPasswordDto resetPasswordDto)
    {
        var errors = new List<string>();

        // Validate token or email presence
        if (string.IsNullOrWhiteSpace(resetPasswordDto.Token) && string.IsNullOrWhiteSpace(resetPasswordDto.Email))
            errors.Add("Either reset token or email must be provided");

        // Validate new password
        if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
            errors.Add("New password is required");
        else if (resetPasswordDto.NewPassword.Length < 8)
            errors.Add("New password must be at least 8 characters long");

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    /// <summary>
    /// Check if the status is valid
    /// </summary>
    /// <param name="status">Status to validate</param>
    /// <returns>true if valid</returns>
    private bool IsValidStatus(string status)
    {
        // Check if status is one of the defined constants
        return status == EsportsManager.DAL.Models.UsersStatus.Active ||
               status == EsportsManager.DAL.Models.UsersStatus.Inactive ||
               status == EsportsManager.DAL.Models.UsersStatus.Suspended ||
               status == EsportsManager.DAL.Models.UsersStatus.Pending ||
               status == EsportsManager.DAL.Models.UsersStatus.Deleted;
    }

    /// <summary>
    /// Map Users entity to UserDTO
    /// </summary>
    /// <param name="user">Users entity</param>
    /// <returns>UserDTO</returns>
    private UserDto MapUserToDto(EsportsManager.DAL.Models.Users user)
    {
        return new UserDto
        {
            Id = user.UserID,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsEmailVerified = user.IsEmailVerified,
            EmailVerificationToken = user.EmailVerificationToken
        };
    }

    public async Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            var users = await _userRepository.SearchAsync(searchTerm);
            var userProfiles = users.Select(u => new UserProfileDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                TotalLogins = 0, // TODO: Implement method to get login count
                IsEmailVerified = u.IsEmailVerified,
                AvatarUrl = null, // Not in the current schema
                Bio = null // Not in the current schema
            }).ToList();

            return userProfiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users: {SearchTerm}", searchTerm);
            return new List<UserProfileDto>();
        }
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        try
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Toggle status
            user.Status = user.Status == EsportsManager.DAL.Models.UsersStatus.Active
                ? EsportsManager.DAL.Models.UsersStatus.Inactive
                : EsportsManager.DAL.Models.UsersStatus.Active;

            user.UpdatedAt = DateTime.UtcNow;

            // Save to database
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {Username} status toggled to {Status}", user.Username, user.Status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status: {UserId}", userId);
            return false;
        }
    }

    public async Task<string> ResetPasswordAsync(int userId)
    {
        try
        {
            // Get user first to get their role
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return "User not found";
            }

            // Generate a role-based password (role + "123")
            string newPassword = GenerateRoleBasedPassword(user.Role);

            // Hash the new password
            string passwordHash = PasswordHasher.HashPassword(newPassword);

            // Update password
            bool success = await _userRepository.UpdatePasswordAsync(user.UserID, passwordHash);
            if (!success)
            {
                return "Failed to reset password";
            }

            // TODO: Send new password via email or other means

            _logger.LogInformation("Password reset for user: {Username}", user.Username);
            return newPassword; // Return the actual new password for display
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user: {UserId}", userId);
            return "An error occurred while resetting the password";
        }
    }

    /// <summary>
    /// Generate a role-based password (role + "123")
    /// </summary>
    /// <param name="role">User role</param>
    /// <returns>Role-based password</returns>
    private string GenerateRoleBasedPassword(string role)
    {
        return role.ToLower() + "123";
    }

    /// <summary>
    /// Generate a random password (deprecated - keeping for backward compatibility)
    /// </summary>
    /// <returns>Randomly generated password</returns>
    private string GenerateRandomPassword()
    {
        const int length = 8;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Get all accounts with Pending status
    /// </summary>
    /// <returns>List of pending accounts</returns>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetPendingAccountsAsync()
    {
        try
        {
            var users = await _userRepository.GetByStatusAsync("Pending");
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                TotalLogins = 0, // TODO: Implement method to get login count
                LastLoginAt = u.LastLoginAt
            });

            return new BusinessResult<IEnumerable<UserDto>>
            {
                IsSuccess = true,
                Data = userDtos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending accounts");
            return new BusinessResult<IEnumerable<UserDto>>
            {
                IsSuccess = false,
                ErrorMessage = "Failed to get pending accounts"
            };
        }
    }

    /// <summary>
    /// Approve a pending account by changing status to Active
    /// </summary>
    /// <param name="userId">User ID to approve</param>
    /// <returns>Success or failure result</returns>
    public async Task<BusinessResult> ApproveAccountAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new BusinessResult
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found"
                };
            }

            if (user.Status != "Pending")
            {
                return new BusinessResult
                {
                    IsSuccess = false,
                    ErrorMessage = "User is not in pending status"
                };
            }

            user.Status = "Active";
            user.UpdatedAt = DateTime.Now;

            var updatedUser = await _userRepository.UpdateAsync(user);
            if (updatedUser != null)
            {
                _logger.LogInformation("Account approved for user: {Username}", user.Username);
                return new BusinessResult { IsSuccess = true };
            }
            else
            {
                return new BusinessResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to update user status"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving account for user: {UserId}", userId);
            return new BusinessResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while approving the account"
            };
        }
    }
}
