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
/// </summary>
/// 
/// SECURITY FEATURES:
/// - Password hashing sử dụng BCrypt
/// - Input validation comprehensive
/// - Không log sensitive data (passwords)
/// - Status checking (Active/Inactive users)
/// </summary>
public class UserService : IUserService
{
    #region Private Fields - Các trường riêng tư
    
    /// <summary>
    /// Repository để truy cập dữ liệu User từ database
    /// Sử dụng interface để đảm bảo loose coupling
    /// </summary>
    private readonly IUserRepository _userRepository;
    
    /// <summary>
    /// Logger để ghi log các hoạt động quan trọng
    /// Giúp debug và monitor hệ thống
    /// </summary>
    private readonly ILogger<UserService> _logger;
    
    #endregion
    
    #region Constructor - Hàm khởi tạo
    
    /// <summary>
    /// Khởi tạo UserService với dependency injection
    /// </summary>
    /// <param name="userRepository">Repository để truy cập User data</param>
    /// <param name="logger">Logger để ghi log</param>
    /// <exception cref="ArgumentNullException">Ném ra khi có dependency null</exception>
    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        // Validate dependencies không được null (Defensive Programming)
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    #endregion

    #region Authentication Methods - Các phương thức xác thực
    
    /// <summary>
    /// Xác thực người dùng đăng nhập
    /// 
    /// BUSINESS RULES:
    /// - Validate input data trước khi xử lý
    /// - Kiểm tra user có tồn tại không
    /// - Kiểm tra user có active không
    /// - Verify password với hash trong database
    /// - Cập nhật thời gian đăng nhập cuối
    /// - Log tất cả attempt (success/failure)
    /// </summary>
    /// <param name="loginDto">Thông tin đăng nhập (username, password)</param>
    /// <returns>AuthenticationResult chứa thông tin user nếu thành công</returns>
    public async Task<AuthenticationResult> AuthenticateAsync(LoginDto loginDto)
    {
        try
        {
            // STEP 1: Validate input data
            var validation = ValidateLoginData(loginDto);
            if (!validation.IsValid)
            {
                return AuthenticationResult.Failure(string.Join("; ", validation.Errors));
            }

            // STEP 2: Tìm user theo username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                // Log warning nhưng không tiết lộ thông tin sensitive
                _logger.LogWarning("Login attempt with non-existent username: {Username}", loginDto.Username);
                return AuthenticationResult.Failure("Invalid username or password");
            }

            // STEP 3: Kiểm tra user status (phải Active mới được đăng nhập)
            if (user.Status != UserStatus.Active)
            {
                _logger.LogWarning("Login attempt with inactive user: {Username}", loginDto.Username);
                return AuthenticationResult.Failure("Account is not active");
            }

            // STEP 4: Verify password với BCrypt
            if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login attempt with wrong password for user: {Username}", loginDto.Username);
                return AuthenticationResult.Failure("Invalid username or password");
            }

            // STEP 5: Cập nhật thời gian đăng nhập cuối
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // STEP 6: Log success và trả về kết quả thành công
            _logger.LogInformation("User successfully authenticated: {Username}", user.Username);
            return AuthenticationResult.Success(user.Id, user.Username, user.Role);
        }
        catch (Exception ex)
        {
            // Log exception với đầy đủ thông tin để debug
            _logger.LogError(ex, "Error during authentication for user: {Username}", loginDto.Username);
            return AuthenticationResult.Failure("An error occurred during authentication");
        }
    }

    /// <summary>
    /// Đăng xuất người dùng
    /// 
    /// BUSINESS RULES:
    /// - Validate userId
    /// - Kiểm tra user có tồn tại không
    /// - Log hoạt động đăng xuất
    /// - Trong thực tế có thể invalidate session/token ở đây
    /// </summary>
    /// <param name="userId">ID của user cần đăng xuất</param>
    /// <returns>BusinessResult cho biết kết quả</returns>
    public async Task<BusinessResult> LogoutAsync(int userId)
    {
        try
        {
            // Tìm user để lấy thông tin log
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("User not found");
            }

            // Log hoạt động đăng xuất
            _logger.LogInformation("User logged out: {Username}", user.Username);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user ID: {UserId}", userId);
            return BusinessResult.Failure("An error occurred during logout");
        }
    }
    
    #endregion

    #region Registration Methods - Các phương thức đăng ký
    
    /// <summary>
    /// Đăng ký người dùng mới
    /// 
    /// BUSINESS RULES:
    /// - Validate tất cả input data
    /// - Kiểm tra username đã tồn tại chưa
    /// - Kiểm tra email đã tồn tại chưa (nếu có)
    /// - Hash password trước khi lưu
    /// - Tạo user với role mặc định là Viewer
    /// - Log hoạt động đăng ký
    /// </summary>
    /// <param name="registerDto">Thông tin đăng ký user mới</param>
    /// <returns>BusinessResult chứa UserDto nếu thành công</returns>
    public async Task<BusinessResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Validate input
            var validation = ValidateRegisterData(registerDto);
            if (!validation.IsValid)
            {
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            // Check if username already exists
            if (await _userRepository.IsUsernameExistsAsync(registerDto.Username))
            {
                return BusinessResult<UserDto>.Failure("Username already exists");
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(registerDto.Email) && await _userRepository.IsEmailExistsAsync(registerDto.Email))
            {
                return BusinessResult<UserDto>.Failure("Email already exists");
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                Role = UserRoles.Viewer, // Default role
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            var userDto = MapToDto(createdUser);

            _logger.LogInformation("User successfully registered: {Username}", createdUser.Username);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration: {Username}", registerDto.Username);
            return BusinessResult<UserDto>.Failure("An error occurred during registration");
        }
    }


    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<BusinessResult<UserDto>> GetUserByIdAsync(int userId)
    {
        try
        {
            var validation = InputValidator.ValidateUserId(userId);
            if (!validation.IsValid)
            {
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            var userDto = MapToDto(user);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return BusinessResult<UserDto>.Failure("An error occurred while retrieving user");
        }
    }

    /// <summary>
    /// Get user by username
    /// </summary>
    public async Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username)
    {
        try
        {
            var validation = InputValidator.ValidateUsername(username);
            if (!validation.IsValid)
            {
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            var userDto = MapToDto(user);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username: {Username}", username);
            return BusinessResult<UserDto>.Failure("An error occurred while retrieving user");
        }
    }

    /// <summary>
    /// Get all users
    /// </summary>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MapToDto);
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return BusinessResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get users by role
    /// </summary>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role)
    {
        try
        {
            var validation = InputValidator.ValidateRole(role);
            if (!validation.IsValid)
            {
                return BusinessResult<IEnumerable<UserDto>>.Failure(validation.Errors);
            }

            var users = await _userRepository.GetByRoleAsync(role);
            var userDtos = users.Select(MapToDto);
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role: {Role}", role);
            return BusinessResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users");
        }
    }

    /// <summary>
    /// Get active users
    /// </summary>
    public async Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync();
            var userDtos = users.Select(MapToDto);
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users");
            return BusinessResult<IEnumerable<UserDto>>.Failure("An error occurred while retrieving active users");
        }
    }

    /// <summary>
    /// Get user profile
    /// </summary>
    public async Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId)
    {
        try
        {
            var validation = InputValidator.ValidateUserId(userId);
            if (!validation.IsValid)
            {
                return BusinessResult<UserProfileDto>.Failure(validation.Errors);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserProfileDto>.Failure("User not found");
            }

            var profileDto = MapToProfileDto(user);
            return BusinessResult<UserProfileDto>.Success(profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile: {UserId}", userId);
            return BusinessResult<UserProfileDto>.Failure("An error occurred while retrieving user profile");
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    public async Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto)
    {
        try
        {
            var validation = InputValidator.ValidateUserId(userId);
            if (!validation.IsValid)
            {
                return BusinessResult<UserDto>.Failure(validation.Errors);
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("User not found");
            }

            // Update user properties
            user.Email = userDto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedUserDto = MapToDto(updatedUser);

            _logger.LogInformation("User profile updated: {Username}", user.Username);
            return BusinessResult<UserDto>.Success(updatedUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
            return BusinessResult<UserDto>.Failure("An error occurred while updating user profile");
        }
    }

    /// <summary>
    /// Update password
    /// </summary>
    public async Task<BusinessResult> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
    {
        try
        {
            var validation = ValidateUpdatePasswordData(updatePasswordDto);
            if (!validation.IsValid)
            {
                return BusinessResult.Failure(validation.Errors);
            }

            var user = await _userRepository.GetByIdAsync(updatePasswordDto.UserId);
            if (user == null)
            {
                return BusinessResult.Failure("User not found");
            }

            // Verify current password
            if (!PasswordHasher.VerifyPassword(updatePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return BusinessResult.Failure("Current password is incorrect");
            }

            // Update password
            var newPasswordHash = PasswordHasher.HashPassword(updatePasswordDto.NewPassword);
            var success = await _userRepository.UpdatePasswordAsync(updatePasswordDto.UserId, newPasswordHash);

            if (success)
            {
                _logger.LogInformation("Password updated for user: {Username}", user.Username);
                return BusinessResult.Success();
            }

            return BusinessResult.Failure("Failed to update password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for user ID: {UserId}", updatePasswordDto.UserId);
            return BusinessResult.Failure("An error occurred while updating password");
        }
    }

    /// <summary>
    /// Reset password
    /// </summary>
    public async Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var validation = ValidateResetPasswordData(resetPasswordDto);
            if (!validation.IsValid)
            {
                return BusinessResult.Failure(validation.Errors);
            }

            // Find user by username or email
            User? user = null;
            if (!string.IsNullOrEmpty(resetPasswordDto.Username))
            {
                user = await _userRepository.GetByUsernameAsync(resetPasswordDto.Username);
            }
            else if (!string.IsNullOrEmpty(resetPasswordDto.Email))
            {
                user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
            }

            if (user == null)
            {
                return BusinessResult.Failure("User not found");
            }

            // Generate temporary password
            var tempPassword = GenerateTemporaryPassword();
            var tempPasswordHash = PasswordHasher.HashPassword(tempPassword);

            // Update password
            var success = await _userRepository.UpdatePasswordAsync(user.Id, tempPasswordHash);

            if (success)
            {
                _logger.LogInformation("Password reset for user: {Username}", user.Username);
                // In real application, send email with temporary password
                return BusinessResult.Success();
            }

            return BusinessResult.Failure("Failed to reset password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user: {Username}", resetPasswordDto.Username);
            return BusinessResult.Failure("An error occurred while resetting password");
        }
    }

    /// <summary>
    /// Create user (Admin only)
    /// </summary>
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
            if (await _userRepository.IsUsernameExistsAsync(createUserDto.Username))
            {
                return BusinessResult<UserDto>.Failure("Username already exists");
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(createUserDto.Email) && await _userRepository.IsEmailExistsAsync(createUserDto.Email))
            {
                return BusinessResult<UserDto>.Failure("Email already exists");
            }

            // Create new user
            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = PasswordHasher.HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            var userDto = MapToDto(createdUser);

            _logger.LogInformation("User created by admin: {Username}", createdUser.Username);
            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", createUserDto.Username);
            return BusinessResult<UserDto>.Failure("An error occurred while creating user");
        }
    }

    /// <summary>
    /// Update user status
    /// </summary>
    public async Task<BusinessResult> UpdateUserStatusAsync(int userId, string status)
    {
        try
        {
            var userValidation = InputValidator.ValidateUserId(userId);
            var statusValidation = InputValidator.ValidateStatus(status);
            var combinedValidation = InputValidator.CombineResults(userValidation, statusValidation);

            if (!combinedValidation.IsValid)
            {
                return BusinessResult.Failure(combinedValidation.Errors);
            }

            var success = await _userRepository.UpdateStatusAsync(userId, status);

            if (success)
            {
                _logger.LogInformation("User status updated: ID {UserId} to {Status}", userId, status);
                return BusinessResult.Success();
            }

            return BusinessResult.Failure("Failed to update user status");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status: ID {UserId}", userId);
            return BusinessResult.Failure("An error occurred while updating user status");
        }
    }

    /// <summary>
    /// Delete user
    /// </summary>
    public async Task<BusinessResult> DeleteUserAsync(int userId)
    {
        try
        {
            var validation = InputValidator.ValidateUserId(userId);
            if (!validation.IsValid)
            {
                return BusinessResult.Failure(validation.Errors);
            }

            var success = await _userRepository.DeleteAsync(userId);

            if (success)
            {
                _logger.LogInformation("User deleted: ID {UserId}", userId);
                return BusinessResult.Success();
            }

            return BusinessResult.Failure("Failed to delete user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: ID {UserId}", userId);
            return BusinessResult.Failure("An error occurred while deleting user");
        }
    }

    /// <summary>
    /// Check if username is available
    /// </summary>
    public async Task<BusinessResult<bool>> IsUsernameAvailableAsync(string username)
    {
        try
        {
            var validation = InputValidator.ValidateUsername(username);
            if (!validation.IsValid)
            {
                return BusinessResult<bool>.Failure(validation.Errors);
            }

            var exists = await _userRepository.IsUsernameExistsAsync(username);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username availability: {Username}", username);
            return BusinessResult<bool>.Failure("An error occurred while checking username availability");
        }
    }

    /// <summary>
    /// Check if email is available
    /// </summary>
    public async Task<BusinessResult<bool>> IsEmailAvailableAsync(string email)
    {
        try
        {
            var validation = InputValidator.ValidateEmail(email);
            if (!validation.IsValid)
            {
                return BusinessResult<bool>.Failure(validation.Errors);
            }

            var exists = await _userRepository.IsEmailExistsAsync(email);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email availability: {Email}", email);
            return BusinessResult<bool>.Failure("An error occurred while checking email availability");
        }
    }

    /// <summary>
    /// Get total users count
    /// </summary>
    public async Task<BusinessResult<int>> GetTotalUsersCountAsync()
    {
        try
        {
            var count = await _userRepository.CountAsync();
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total users count");
            return BusinessResult<int>.Failure("An error occurred while getting users count");
        }
    }

    /// <summary>
    /// Get user count by role
    /// </summary>
    public async Task<BusinessResult<int>> GetUserCountByRoleAsync(string role)
    {
        try
        {
            var validation = InputValidator.ValidateRole(role);
            if (!validation.IsValid)
            {
                return BusinessResult<int>.Failure(validation.Errors);
            }

            var users = await _userRepository.GetByRoleAsync(role);
            return BusinessResult<int>.Success(users.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user count by role: {Role}", role);
            return BusinessResult<int>.Failure("An error occurred while getting user count");
        }
    }

    /// <summary>
    /// Get active users count
    /// </summary>
    public async Task<BusinessResult<int>> GetActiveUsersCountAsync()
    {
        try
        {
            var users = await _userRepository.GetActiveUsersAsync();
            return BusinessResult<int>.Success(users.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users count");
            return BusinessResult<int>.Failure("An error occurred while getting active users count");
        }
    }

    /// <summary>
    /// Validate user data
    /// </summary>
    public ValidationResult ValidateUserData(CreateUserDto createUserDto)
    {
        var usernameValidation = InputValidator.ValidateUsername(createUserDto.Username);
        var emailValidation = InputValidator.ValidateEmail(createUserDto.Email);
        var passwordValidation = InputValidator.ValidatePassword(createUserDto.Password);
        var roleValidation = InputValidator.ValidateRole(createUserDto.Role);

        return InputValidator.CombineResults(usernameValidation, emailValidation, passwordValidation, roleValidation);
    }

    /// <summary>
    /// Validate login data
    /// </summary>
    public ValidationResult ValidateLoginData(LoginDto loginDto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(loginDto.Username))
            errors.Add("Username is required");

        if (string.IsNullOrWhiteSpace(loginDto.Password))
            errors.Add("Password is required");

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    // Private helper methods
    private ValidationResult ValidateRegisterData(RegisterDto registerDto)
    {
        var usernameValidation = InputValidator.ValidateUsername(registerDto.Username);
        var emailValidation = InputValidator.ValidateEmail(registerDto.Email);
        var passwordValidation = InputValidator.ValidatePassword(registerDto.Password);
        var confirmPasswordValidation = InputValidator.ValidatePasswordConfirmation(registerDto.Password, registerDto.ConfirmPassword);

        return InputValidator.CombineResults(usernameValidation, emailValidation, passwordValidation, confirmPasswordValidation);
    }

    private ValidationResult ValidateUpdatePasswordData(UpdatePasswordDto updatePasswordDto)
    {
        var userIdValidation = InputValidator.ValidateUserId(updatePasswordDto.UserId);
        var currentPasswordValidation = string.IsNullOrWhiteSpace(updatePasswordDto.CurrentPassword) 
            ? ValidationResult.Failure("Current password is required") 
            : ValidationResult.Success();
        var newPasswordValidation = InputValidator.ValidatePassword(updatePasswordDto.NewPassword);
        var confirmPasswordValidation = InputValidator.ValidatePasswordConfirmation(updatePasswordDto.NewPassword, updatePasswordDto.ConfirmNewPassword);

        return InputValidator.CombineResults(userIdValidation, currentPasswordValidation, newPasswordValidation, confirmPasswordValidation);
    }

    private ValidationResult ValidateResetPasswordData(ResetPasswordDto resetPasswordDto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(resetPasswordDto.Username) && string.IsNullOrWhiteSpace(resetPasswordDto.Email))
        {
            errors.Add("Either username or email is required");
        }

        if (!string.IsNullOrWhiteSpace(resetPasswordDto.Username))
        {
            var usernameValidation = InputValidator.ValidateUsername(resetPasswordDto.Username);
            if (!usernameValidation.IsValid)
            {
                errors.AddRange(usernameValidation.Errors);
            }
        }

        if (!string.IsNullOrWhiteSpace(resetPasswordDto.Email))
        {
            var emailValidation = InputValidator.ValidateEmail(resetPasswordDto.Email);
            if (!emailValidation.IsValid)
            {
                errors.AddRange(emailValidation.Errors);
            }
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    private UserDto MapToDto(EsportsManager.DAL.Models.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    private UserProfileDto MapToProfileDto(EsportsManager.DAL.Models.User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            // These would be calculated from other data sources in a real app
            TotalLogins = 0,
            TotalTimeOnline = TimeSpan.Zero
        };
    }

    private string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    #endregion

    // ═══════════════════════════════════════════════════════════════
    // ENHANCED ADMIN METHODS - Vietnamese Esports Focus
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Lấy danh sách tất cả người dùng với phân trang và filtering
    /// </summary>
    public async Task<PagedResult<UserProfileDto>> GetAllUsersAsync(
        int pageNumber = 1, 
        int pageSize = 20,
        string? roleFilter = null,
        string? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Mock implementation - Production: Replace with real database queries
            await Task.Delay(50, cancellationToken);

            var allUsers = GetMockUsers();

            // Apply filters
            var filteredUsers = allUsers.AsEnumerable();
            
            if (!string.IsNullOrEmpty(roleFilter))
                filteredUsers = filteredUsers.Where(u => u.Role.Equals(roleFilter, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(statusFilter))
                filteredUsers = filteredUsers.Where(u => u.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));

            var totalCount = filteredUsers.Count();
            var pagedUsers = filteredUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<UserProfileDto>
            {
                Items = pagedUsers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged users list");
            throw;
        }
    }

    /// <summary>
    /// Tìm kiếm người dùng với enhanced search types
    /// </summary>
    public async Task<List<UserProfileDto>> SearchUsersAsync(
        string searchTerm, 
        SearchType searchType = SearchType.Contains,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(30, cancellationToken);

            var allUsers = GetMockUsers();
            var results = searchType switch
            {
                SearchType.Exact => allUsers.Where(u => 
                    u.Username.Equals(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email?.Equals(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.FullName?.Equals(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList(),
                
                SearchType.StartsWith => allUsers.Where(u => 
                    u.Username.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email?.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.FullName?.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList(),
                
                SearchType.EndsWith => allUsers.Where(u => 
                    u.Username.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email?.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.FullName?.EndsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList(),
                
                _ => allUsers.Where(u => 
                    u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList()
            };

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Lấy user by ID trả về UserProfileDto
    /// </summary>
    public async Task<UserProfileDto?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(20, cancellationToken);
            var allUsers = GetMockUsers();
            return allUsers.FirstOrDefault(u => u.Id == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Thay đổi trạng thái người dùng
    /// </summary>
    public async Task<UserStatusChangeResult> ToggleUserStatusAsync(
        int userId, 
        string newStatus = "Toggle",
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(40, cancellationToken);

            var user = GetMockUsers().FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new UserStatusChangeResult { Success = false, Message = "Không tìm thấy người dùng" };

            var oldStatus = user.Status;
            var targetStatus = newStatus == "Toggle" ? (oldStatus == "Active" ? "Inactive" : "Active") : newStatus;

            // Mock status change
            user.Status = targetStatus;

            return new UserStatusChangeResult
            {
                Success = true,
                OldStatus = oldStatus,
                NewStatus = targetStatus,
                Message = $"Đã thay đổi trạng thái từ {oldStatus} thành {targetStatus}",
                ChangedAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status for userId: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Reset mật khẩu người dùng
    /// </summary>
    public async Task<PasswordResetResult> ResetPasswordAsync(
        int userId, 
        bool sendEmail = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(60, cancellationToken);

            var user = GetMockUsers().FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new PasswordResetResult { Success = false, Message = "Không tìm thấy người dùng" };

            var newPassword = GenerateRandomPassword();

            return new PasswordResetResult
            {
                Success = true,
                NewPassword = newPassword,
                EmailSent = sendEmail,
                Message = sendEmail ? "Mật khẩu mới đã được gửi qua email" : "Mật khẩu đã được reset",
                ResetAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for userId: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Xóa người dùng
    /// </summary>
    public async Task<UserDeletionResult> DeleteUserAsync(
        int userId, 
        string confirmationCode,
        bool hardDelete = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(80, cancellationToken);

            // Validate confirmation code
            if (confirmationCode != "CONFIRM_DELETE")
                return new UserDeletionResult { Success = false, Message = "Mã xác nhận không đúng" };

            var user = GetMockUsers().FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new UserDeletionResult { Success = false, Message = "Không tìm thấy người dùng" };

            return new UserDeletionResult
            {
                Success = true,
                RelatedDataCleaned = true,
                CleanedDataTypes = new List<string> { "Tournaments", "Teams", "Audit Logs" },
                Message = hardDelete ? "Người dùng đã được xóa vĩnh viễn" : "Người dùng đã được đánh dấu xóa",
                DeletedAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Bulk operations cho nhiều users
    /// </summary>
    public async Task<BulkOperationResult> BulkUpdateUsersAsync(
        List<int> userIds,
        BulkUserOperation operation,
        object? operationData = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(100, cancellationToken);

            var result = new BulkOperationResult
            {
                Success = true,
                SuccessCount = userIds.Count - 1, // Mock 1 failure
                FailedCount = 1,
                ProcessedAt = DateTime.Now,
                Message = $"Đã xử lý {userIds.Count} người dùng"
            };

            // Mock 1 error
            if (userIds.Count > 0)
            {
                result.Errors.Add(new BulkOperationError
                {
                    UserId = userIds[0],
                    ErrorMessage = "Người dùng này không thể cập nhật",
                    ErrorCode = "USER_LOCKED"
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk operation: {Operation}", operation);
            throw;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════════

    private static List<UserProfileDto> GetMockUsers()
    {
        return new List<UserProfileDto>
        {
            new UserProfileDto 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@vtc.vn", 
                FullName = "Quản trị viên",
                Role = "Admin", 
                Status = "Active",
                CreatedAt = DateTime.Now.AddDays(-30),
                LastLoginAt = DateTime.Now.AddHours(-1),
                TotalLogins = 250,
                TotalTimeOnline = TimeSpan.FromHours(120)
            },
            new UserProfileDto 
            { 
                Id = 2, 
                Username = "pro_gamer_vn", 
                Email = "progamer@esports.vn", 
                FullName = "Nguyễn Văn An",
                Role = "Player", 
                Status = "Active",
                CreatedAt = DateTime.Now.AddDays(-15),
                LastLoginAt = DateTime.Now.AddMinutes(-30),
                TotalLogins = 89,
                TotalTimeOnline = TimeSpan.FromHours(45)
            },
            new UserProfileDto 
            { 
                Id = 3, 
                Username = "esports_fan", 
                Email = "fan@viewer.vn", 
                FullName = "Trần Thị Bình",
                Role = "Viewer", 
                Status = "Active",
                CreatedAt = DateTime.Now.AddDays(-10),
                LastLoginAt = DateTime.Now.AddDays(-2),
                TotalLogins = 42,
                TotalTimeOnline = TimeSpan.FromHours(20)
            },
            new UserProfileDto 
            { 
                Id = 4, 
                Username = "mobile_legends_pro", 
                Email = "mlpro@esports.vn", 
                FullName = "Lê Văn Cường",
                Role = "Player", 
                Status = "Inactive",
                CreatedAt = DateTime.Now.AddDays(-20),
                LastLoginAt = DateTime.Now.AddDays(-5),
                TotalLogins = 156,
                TotalTimeOnline = TimeSpan.FromHours(78)
            }
        };
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}