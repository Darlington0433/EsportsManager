using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EsportsManager.BL.Services
{
    /// <summary>
    /// Authentication Service - Xử lý đăng nhập và xác thực người dùng
    /// Theo tài liệu nghiệp vụ: Role-based Authentication
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IUsersRepository usersRepository,
            ILogger<AuthenticationService> logger)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Đăng nhập với username/password
        /// Theo logic tài liệu: kiểm tra account approval, role-based access
        /// </summary>
        public async Task<LoginResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                if (loginDto == null)
                    throw new ArgumentNullException(nameof(loginDto));

                _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);

                // 1. Tìm user theo username
                var user = await _usersRepository.GetByUsernameAsync(loginDto.Username);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Username}", loginDto.Username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                        ErrorCode = "USER_NOT_FOUND"
                    };
                }

                // 2. Kiểm tra trạng thái tài khoản
                if (user.Status == "Deleted")
                {
                    _logger.LogWarning("Login failed: User deleted - {Username}", loginDto.Username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tài khoản đã bị xóa",
                        ErrorCode = "USER_DELETED"
                    };
                }

                if (user.Status == "Suspended")
                {
                    _logger.LogWarning("Login failed: User suspended - {Username}", loginDto.Username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tài khoản đã bị tạm khóa",
                        ErrorCode = "USER_SUSPENDED"
                    };
                }

                if (user.Status == "Pending")
                {
                    _logger.LogWarning("Login failed: User pending approval - {Username}", loginDto.Username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tài khoản đang chờ duyệt từ Admin",
                        ErrorCode = "USER_PENDING"
                    };
                }

                // 3. Xác minh mật khẩu
                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password - {Username}", loginDto.Username);
                    return new LoginResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                        ErrorCode = "INVALID_PASSWORD"
                    };
                }

                // 4. Cập nhật thời gian đăng nhập cuối
                await _usersRepository.UpdateLastLoginAsync(user.UserID);

                // 5. Tạo UserDto theo role
                var userDto = CreateUserDtoByRole(user);

                _logger.LogInformation("Login successful for user: {Username}, Role: {Role}", user.Username, user.Role);

                return new LoginResult
                {
                    IsSuccess = true,
                    Message = $"Đăng nhập thành công với vai trò {GetRoleDisplayName(user.Role)}",
                    User = userDto,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", loginDto?.Username);
                return new LoginResult
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống. Vui lòng thử lại sau.",
                    ErrorCode = "SYSTEM_ERROR"
                };
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// Theo tài liệu: Account cần được Admin duyệt
        /// </summary>
        public async Task<RegisterResult> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                    throw new ArgumentNullException(nameof(registerDto));

                _logger.LogInformation("Registration attempt for user: {Username}", registerDto.Username);

                // 1. Validate input
                var validationResult = ValidateRegistration(registerDto);
                if (!validationResult.IsValid)
                {
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = validationResult.ErrorMessage,
                        ErrorCode = "VALIDATION_ERROR"
                    };
                }

                // 2. Kiểm tra username đã tồn tại
                if (await _usersRepository.IsUsernameExistsAsync(registerDto.Username))
                {
                    _logger.LogWarning("Registration failed: Username exists - {Username}", registerDto.Username);
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập đã tồn tại",
                        ErrorCode = "USERNAME_EXISTS"
                    };
                }

                // 3. Kiểm tra email đã tồn tại
                if (await _usersRepository.IsEmailExistsAsync(registerDto.Email))
                {
                    _logger.LogWarning("Registration failed: Email exists - {Email}", registerDto.Email);
                    return new RegisterResult
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng",
                        ErrorCode = "EMAIL_EXISTS"
                    };
                }

                // 4. Tạo user mới với trạng thái Pending
                var newUser = new DAL.Models.Users
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = HashPassword(registerDto.Password),
                    FullName = registerDto.FullName,
                    Role = registerDto.Role,
                    Status = "Pending", // Theo tài liệu: cần Admin duyệt
                    IsEmailVerified = false,
                    EmailVerificationToken = GenerateToken(),
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _usersRepository.AddAsync(newUser);

                _logger.LogInformation("User registered successfully: {Username}, Role: {Role}, Status: Pending",
                    createdUser.Username, createdUser.Role);

                return new RegisterResult
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công. Tài khoản đang chờ duyệt từ Admin.",
                    UserId = createdUser.UserID
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", registerDto?.Username);
                return new RegisterResult
                {
                    IsSuccess = false,
                    Message = "Lỗi hệ thống. Vui lòng thử lại sau.",
                    ErrorCode = "SYSTEM_ERROR"
                };
            }
        }

        /// <summary>
        /// Đăng xuất người dùng
        /// </summary>
        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Logout attempt for user ID: {UserId}", userId);

                // Kiểm tra người dùng tồn tại
                var user = await _usersRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Logout failed: User not found - ID: {UserId}", userId);
                    return false;
                }

                // Ghi log hoạt động đăng xuất (có thể thêm bảng ActivityLog trong tương lai)
                _logger.LogInformation("User logged out successfully: {Username}, ID: {UserId}",
                    user.Username, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user ID: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Xác thực email người dùng
        /// </summary>
        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Email verification failed: Empty token");
                    return false;
                }

                _logger.LogInformation("Email verification attempt with token: {Token}", token);

                // Tìm người dùng với token xác thực
                var user = await _usersRepository.GetByEmailVerificationTokenAsync(token);
                if (user == null)
                {
                    _logger.LogWarning("Email verification failed: Invalid token - {Token}", token);
                    return false;
                }

                if (user.IsEmailVerified)
                {
                    _logger.LogInformation("Email already verified for user: {Username}", user.Username);
                    return true;
                }                // Cập nhật trạng thái xác thực email qua repository method chuyên biệt
                var result = await _usersRepository.VerifyEmailAsync(token); if (result)
                {
                    _logger.LogInformation("Email verified successfully for user: {Username}", user.Username);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Email verification failed for user: {Username}", user.Username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification with token: {Token}", token);
                return false;
            }
        }

        /// <summary>
        /// Gửi email xác thực lại
        /// </summary>
        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Resend verification email failed: Empty email");
                    return false;
                }

                _logger.LogInformation("Resend verification email attempt for: {Email}", email);

                // Tìm người dùng theo email
                var user = await _usersRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Resend verification email failed: Email not found - {Email}", email);
                    return false;
                }

                if (user.IsEmailVerified)
                {
                    _logger.LogInformation("Email already verified for user: {Username}", user.Username);
                    return false;
                }

                // Tạo token xác thực mới
                user.EmailVerificationToken = GenerateToken();
                await _usersRepository.UpdateAsync(user);

                // Tại đây sẽ gửi email xác thực (cần implement email service)
                // SendVerificationEmail(user.Email, user.EmailVerificationToken);

                _logger.LogInformation("Verification email resent successfully for user: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resending verification email for: {Email}", email);
                return false;
            }
        }

        /// <summary>
        /// Khôi phục mật khẩu
        /// </summary>
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null)
                {
                    _logger.LogWarning("Reset password failed: Invalid input");
                    return false;
                }

                _logger.LogInformation("Reset password attempt with token");

                if (string.IsNullOrEmpty(resetPasswordDto.Token))
                {
                    _logger.LogWarning("Reset password failed: Token is null or empty");
                    return false;
                }
                // Tìm người dùng với token reset password
                var user = await _usersRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token);
                if (user == null)
                {
                    _logger.LogWarning("Reset password failed: Invalid token");
                    return false;
                }

                // Kiểm tra token có hết hạn không
                if (user.PasswordResetExpiry.HasValue && user.PasswordResetExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning("Reset password failed: Token expired for user: {Username}", user.Username);
                    return false;
                }

                // Cập nhật mật khẩu mới
                user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetExpiry = null;

                await _usersRepository.UpdateAsync(user);

                _logger.LogInformation("Password reset successfully for user: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                {
                    _logger.LogWarning("Change password failed: Invalid input for user ID: {UserId}", userId);
                    return false;
                }

                _logger.LogInformation("Change password attempt for user ID: {UserId}", userId);

                // Tìm người dùng theo ID
                var user = await _usersRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Change password failed: User not found - ID: {UserId}", userId);
                    return false;
                }

                // Kiểm tra mật khẩu hiện tại
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Change password failed: Invalid current password for user: {Username}", user.Username);
                    return false;
                }                // Cập nhật mật khẩu mới sử dụng phương thức chuyên biệt
                string newPasswordHash = HashPassword(newPassword);
                var result = await _usersRepository.UpdatePasswordAsync(userId, newPasswordHash); if (result)
                {
                    _logger.LogInformation("Password changed successfully for user: {Username}", user.Username);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Password change failed for user: {Username}", user.Username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user ID: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo role
        /// </summary>
        public bool HasPermission(string userRole, string requiredRole)
        {
            // Admin có tất cả quyền
            if (userRole == "Admin")
                return true;

            // Player có quyền Player và Viewer
            if (userRole == "Player" && (requiredRole == "Player" || requiredRole == "Viewer"))
                return true;

            // Viewer chỉ có quyền Viewer
            if (userRole == "Viewer" && requiredRole == "Viewer")
                return true;

            return false;
        }

        #region Private Methods

        private UserDto CreateUserDtoByRole(DAL.Models.Users user)
        {
            return new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Status = user.Status,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        private string GetRoleDisplayName(string role)
        {
            return role switch
            {
                "Admin" => "Quản trị viên",
                "Player" => "Người chơi",
                "Viewer" => "Người xem",
                _ => role
            };
        }

        private (bool IsValid, string ErrorMessage) ValidateRegistration(RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username))
                return (false, "Tên đăng nhập không được để trống");

            if (registerDto.Username.Length < 3 || registerDto.Username.Length > 50)
                return (false, "Tên đăng nhập phải từ 3-50 ký tự");

            if (string.IsNullOrWhiteSpace(registerDto.Email))
                return (false, "Email không được để trống");

            if (string.IsNullOrWhiteSpace(registerDto.Password))
                return (false, "Mật khẩu không được để trống");

            if (registerDto.Password.Length < 8)
                return (false, "Mật khẩu phải có ít nhất 8 ký tự");

            if (!IsValidRole(registerDto.Role))
                return (false, "Vai trò không hợp lệ");

            return (true, string.Empty);
        }

        private bool IsValidRole(string role)
        {
            return role == "Player" || role == "Viewer"; // Admin không cho phép đăng ký trực tiếp
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "EsportsManager_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var computedHash = HashPassword(password);
            return computedHash == hashedPassword;
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion
    }

    /// <summary>
    /// Kết quả đăng nhập
    /// </summary>
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string? Role { get; set; }
        public string? ErrorCode { get; set; }
    }

    /// <summary>
    /// Kết quả đăng ký
    /// </summary>
    public class RegisterResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? ErrorCode { get; set; }
    }
}
