using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.BL.Utilities;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersRepository usersRepository, ILogger<UserService> logger)
    {
        _usersRepository = usersRepository;
        _logger = logger;
    }

    public async Task<BusinessResult<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await _usersRepository.GetAllAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role
            });
            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng");
            return BusinessResult<IEnumerable<UserDto>>.Failure($"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
    }

    public async Task<BusinessResult> DeleteUserAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            await _usersRepository.DeleteAsync(userId);
            return BusinessResult.Success();
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Lỗi khi xóa người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi xóa người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult> UpdateUserRoleAsync(int userId, string newRole)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            if (newRole != DAL.Models.UsersRoles.Admin && newRole != DAL.Models.UsersRoles.Player && newRole != DAL.Models.UsersRoles.Viewer)
            {
                return BusinessResult.Failure("Vai trò không hợp lệ");
            }

            user.Role = newRole;
            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật vai trò cho người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi cập nhật vai trò: {ex.Message}");
        }
    }

    public async Task<BusinessResult> BanUserAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            user.Status = DAL.Models.UsersStatus.Suspended;
            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Lỗi khi cấm người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi cấm người dùng: {ex.Message}");
            }
    }

    public async Task<Models.AuthenticationResult> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            if (await _usersRepository.GetByUsernameAsync(registerDto.Username) != null)
            {
                return Models.AuthenticationResult.Failure("Tên đăng nhập đã tồn tại");
            }

            if (await _usersRepository.GetByEmailAsync(registerDto.Email) != null)
            {
                return Models.AuthenticationResult.Failure("Email đã được sử dụng");
            }

            var user = new Users
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = DAL.Models.UsersRoles.Viewer,
                Status = DAL.Models.UsersStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _usersRepository.AddAsync(user);
            return Models.AuthenticationResult.Success(user.UserID, user.Username, user.Role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký người dùng");
            return Models.AuthenticationResult.Failure($"Lỗi khi đăng ký: {ex.Message}");
        }
    }

    public async Task<Models.AuthenticationResult> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _usersRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return Models.AuthenticationResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Models.AuthenticationResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng");
            }

            if (user.Status == DAL.Models.UsersStatus.Suspended)
            {
                return Models.AuthenticationResult.Failure("Tài khoản đã bị khóa");
            }

            return Models.AuthenticationResult.Success(user.UserID, user.Username, user.Role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng nhập");
            return Models.AuthenticationResult.Failure($"Lỗi khi đăng nhập: {ex.Message}");
        }
    }

    public async Task<BusinessResult> UpdateUserAsync(int userId, UpdateUserDto updateDto)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != user.Email)
            {
                if (await _usersRepository.GetByEmailAsync(updateDto.Email) != null)
                {
                    return BusinessResult.Failure("Email đã được sử dụng");
                }
                user.Email = updateDto.Email;
            }

            if (!string.IsNullOrEmpty(updateDto.FullName))
                user.FullName = updateDto.FullName;

            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi cập nhật thông tin: {ex.Message}");
        }
    }

    public async Task<BusinessResult> UpdatePasswordAsync(int userId, UpdatePasswordDto updateDto)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            if (!BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, user.PasswordHash))
            {
                return BusinessResult.Failure("Mật khẩu hiện tại không đúng");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật mật khẩu cho người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi cập nhật mật khẩu: {ex.Message}");
        }
    }

    public async Task<string> ResetPasswordAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Không tìm thấy người dùng");
            }

            // Tạo mật khẩu mới ngẫu nhiên
            var newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Cập nhật mật khẩu mới
            await _usersRepository.UpdatePasswordAsync(userId, hashedPassword);

            return newPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi reset mật khẩu cho người dùng {UserId}", userId);
            throw;
        }
    }

    public async Task<BusinessResult> VerifyEmailAsync(string token)
    {
        try
        {
            var user = await _usersRepository.GetByEmailVerificationTokenAsync(token);
            if (user == null)
            {
                return BusinessResult.Failure("Token không hợp lệ hoặc đã hết hạn");
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            await _usersRepository.UpdateAsync(user);

            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xác thực email");
            return BusinessResult.Failure($"Lỗi khi xác thực email: {ex.Message}");
        }
    }

    public async Task<BusinessResult> ResendVerificationEmailAsync(string email)
    {
        try
        {
            var user = await _usersRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng với email này");
            }

            if (user.IsEmailVerified)
            {
                return BusinessResult.Failure("Email đã được xác thực");
            }

            // Tạo token mới
            user.EmailVerificationToken = Guid.NewGuid().ToString("N");
            await _usersRepository.UpdateAsync(user);

            // TODO: Gửi email xác thực
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi lại email xác thực");
            return BusinessResult.Failure($"Lỗi khi gửi lại email xác thực: {ex.Message}");
        }
    }

    public async Task<BusinessResult<UserDto>> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("Không tìm thấy người dùng");
            }

            var userDto = new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng {UserId}", userId);
            return BusinessResult<UserDto>.Failure($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<UserDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _usersRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("Không tìm thấy người dùng");
            }

            var userDto = new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng theo email {Email}", email);
            return BusinessResult<UserDto>.Failure($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<UserDto>> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await _usersRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("Không tìm thấy người dùng");
            }

            var userDto = new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng theo username {Username}", username);
            return BusinessResult<UserDto>.Failure($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<IEnumerable<UserDto>>> GetUsersByRoleAsync(string role)
    {
        try
        {
            var users = await _usersRepository.GetByRoleAsync(role);
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role
            });

            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
            }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng theo vai trò {Role}", role);
            return BusinessResult<IEnumerable<UserDto>>.Failure($"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
    }

    public async Task<BusinessResult<IEnumerable<UserDto>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _usersRepository.GetActiveUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role
            });

            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng đang hoạt động");
            return BusinessResult<IEnumerable<UserDto>>.Failure($"Lỗi khi lấy danh sách người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<UserProfileDto>> GetUserProfileAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserProfileDto>.Failure("Không tìm thấy người dùng");
            }

            var profile = new UserProfileDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Status = user.Status
            };

            return BusinessResult<UserProfileDto>.Success(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy thông tin profile người dùng {UserId}", userId);
            return BusinessResult<UserProfileDto>.Failure($"Lỗi khi lấy thông tin profile: {ex.Message}");
            }
    }

    public async Task<BusinessResult<UserDto>> UpdateUserProfileAsync(int userId, UserDto userDto)
            {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult<UserDto>.Failure("Không tìm thấy người dùng");
            }

            // Kiểm tra email trùng lặp nếu email thay đổi
            if (!string.IsNullOrEmpty(userDto.Email) && userDto.Email != user.Email)
            {
                if (await _usersRepository.GetByEmailAsync(userDto.Email) != null)
            {
                    return BusinessResult<UserDto>.Failure("Email đã được sử dụng");
            }
                user.Email = userDto.Email;
            }

            // Cập nhật thông tin
            user.FullName = userDto.FullName ?? user.FullName;
            user.Username = userDto.Username ?? user.Username;

            await _usersRepository.UpdateAsync(user);

            var updatedDto = new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            return BusinessResult<UserDto>.Success(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật profile người dùng {UserId}", userId);
            return BusinessResult<UserDto>.Failure($"Lỗi khi cập nhật profile: {ex.Message}");
        }
    }

    public async Task<BusinessResult<UserDto>> CreateUserAsync(CreateUserDto createDto)
    {
        try
        {
            var validationResult = ValidateUserData(createDto);
            if (!validationResult.IsValid)
            {
                return BusinessResult<UserDto>.Failure(string.Join(", ", validationResult.Errors));
            }

            if (await _usersRepository.GetByUsernameAsync(createDto.Username) != null)
            {
                return BusinessResult<UserDto>.Failure("Tên đăng nhập đã tồn tại");
            }

            if (await _usersRepository.GetByEmailAsync(createDto.Email) != null)
            {
                return BusinessResult<UserDto>.Failure("Email đã được sử dụng");
            }

            var user = new Users
            {
                Username = createDto.Username,
                Email = createDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                FullName = createDto.FullName,
                Role = createDto.Role ?? DAL.Models.UsersRoles.Viewer,
                Status = DAL.Models.UsersStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _usersRepository.AddAsync(user);

            var userDto = new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };

            return BusinessResult<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo người dùng mới");
            return BusinessResult<UserDto>.Failure($"Lỗi khi tạo người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult> UpdateUserStatusAsync(int userId, string status)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            if (status != DAL.Models.UsersStatus.Active && status != DAL.Models.UsersStatus.Pending && status != DAL.Models.UsersStatus.Suspended)
            {
                return BusinessResult.Failure("Trạng thái không hợp lệ");
            }

            user.Status = status;
            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật trạng thái người dùng {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi cập nhật trạng thái: {ex.Message}");
        }
    }

    public async Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            var users = await _usersRepository.SearchAsync(searchTerm);
            return users.Select(u => new UserProfileDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Status = u.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tìm kiếm người dùng với từ khóa {SearchTerm}", searchTerm);
            return new List<UserProfileDto>();
        }
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.Status == DAL.Models.UsersStatus.Active)
            {
                user.Status = DAL.Models.UsersStatus.Inactive;
            }
            else if (user.Status == DAL.Models.UsersStatus.Inactive)
            {
                user.Status = DAL.Models.UsersStatus.Active;
            }
            await _usersRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thay đổi trạng thái người dùng {UserId}", userId);
            return false;
        }
    }

    public async Task<BusinessResult<bool>> DeleteUserWithPermissionCheckAsync(int userId, int requestUserId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            var requestUser = await _usersRepository.GetByIdAsync(requestUserId);

            if (user == null)
            {
                return BusinessResult<bool>.Failure("Không tìm thấy người dùng cần xóa");
            }

            if (requestUser == null)
            {
                return BusinessResult<bool>.Failure("Không tìm thấy người dùng yêu cầu");
            }

            // Kiểm tra quyền: chỉ admin mới có thể xóa user khác
            if (requestUser.Role != DAL.Models.UsersRoles.Admin && requestUserId != userId)
            {
                return BusinessResult<bool>.Failure("Không có quyền xóa người dùng khác");
            }

            await _usersRepository.DeleteAsync(userId);
            return BusinessResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa người dùng {UserId}", userId);
            return BusinessResult<bool>.Failure($"Lỗi khi xóa người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<bool>> IsUsernameAvailableAsync(string username)
    {
        try
        {
            var exists = await _usersRepository.IsUsernameExistsAsync(username);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra tên đăng nhập {Username}", username);
            return BusinessResult<bool>.Failure($"Lỗi khi kiểm tra tên đăng nhập: {ex.Message}");
        }
    }

    public async Task<BusinessResult<bool>> IsEmailAvailableAsync(string email)
    {
        try
        {
            var exists = await _usersRepository.IsEmailExistsAsync(email);
            return BusinessResult<bool>.Success(!exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra email {Email}", email);
            return BusinessResult<bool>.Failure($"Lỗi khi kiểm tra email: {ex.Message}");
        }
    }

    public Models.ValidationResult ValidateUserData(CreateUserDto createUserDto)
    {
        if (createUserDto == null)
        {
            return Models.ValidationResult.Failure("Dữ liệu người dùng không được để trống");
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(createUserDto.Username))
        {
            errors.Add("Tên đăng nhập không được để trống");
        }
        else if (createUserDto.Username.Length < 3 || createUserDto.Username.Length > 50)
        {
            errors.Add("Tên đăng nhập phải từ 3-50 ký tự");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Email))
    {
            errors.Add("Email không được để trống");
        }
        else if (!InputValidator.IsValidEmail(createUserDto.Email))
        {
            errors.Add("Email không hợp lệ");
        }

        if (string.IsNullOrWhiteSpace(createUserDto.Password))
        {
            errors.Add("Mật khẩu không được để trống");
        }
        else
        {
            var passwordValidation = ValidatePassword(createUserDto.Password);
            if (!passwordValidation.IsValid)
            {
                errors.AddRange(passwordValidation.Errors);
            }
        }

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    public Models.ValidationResult ValidateLoginData(LoginDto loginDto)
    {
        if (loginDto == null)
        {
            return Models.ValidationResult.Failure("Dữ liệu đăng nhập không được để trống");
        }

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(loginDto.Username))
        {
            errors.Add("Tên đăng nhập không được để trống");
        }

        if (string.IsNullOrWhiteSpace(loginDto.Password))
        {
            errors.Add("Mật khẩu không được để trống");
        }

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    private Models.ValidationResult ValidateUserDataInternal(string username, string email)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add("Tên người dùng không được để trống");
        }
        else if (username.Length < 3 || username.Length > 50)
        {
            errors.Add("Tên người dùng phải từ 3-50 ký tự");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("Email không được để trống");
    }
        else if (!InputValidator.IsValidEmail(email))
        {
            errors.Add("Email không hợp lệ");
        }

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    private Models.ValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Mật khẩu không được để trống");
        }
        else
        {
            if (password.Length < 8)
                errors.Add("Mật khẩu phải có ít nhất 8 ký tự");
            if (!password.Any(char.IsUpper))
                errors.Add("Mật khẩu phải chứa ít nhất 1 chữ hoa");
            if (!password.Any(char.IsLower))
                errors.Add("Mật khẩu phải chứa ít nhất 1 chữ thường");
            if (!password.Any(char.IsDigit))
                errors.Add("Mật khẩu phải chứa ít nhất 1 chữ số");
        }

        return errors.Any() ? Models.ValidationResult.Failure(errors) : Models.ValidationResult.Success();
    }

    public async Task<BusinessResult<int>> GetTotalUsersCountAsync()
    {
        try
        {
            var count = await _usersRepository.CountAsync();
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy tổng số người dùng");
            return BusinessResult<int>.Failure($"Lỗi khi lấy tổng số người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<int>> GetUserCountByRoleAsync(string role)
    {
        try
        {
            var count = await _usersRepository.GetCountByRoleAsync(role);
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy số lượng người dùng theo vai trò {Role}", role);
            return BusinessResult<int>.Failure($"Lỗi khi lấy số lượng người dùng: {ex.Message}");
        }
    }

    public async Task<BusinessResult<int>> GetActiveUsersCountAsync()
    {
        try
        {
            var count = await _usersRepository.GetCountByStatusAsync(DAL.Models.UsersStatus.Active);
            return BusinessResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy số lượng người dùng đang hoạt động");
            return BusinessResult<int>.Failure($"Lỗi khi lấy số lượng người dùng: {ex.Message}");
    }
    }

    public async Task<BusinessResult<IEnumerable<UserDto>>> GetPendingAccountsAsync()
    {
        try
        {
            var users = await _usersRepository.GetPendingUsersAsync();
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.UserID,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role
            });

            return BusinessResult<IEnumerable<UserDto>>.Success(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách tài khoản chờ duyệt");
            return BusinessResult<IEnumerable<UserDto>>.Failure($"Lỗi khi lấy danh sách tài khoản chờ duyệt: {ex.Message}");
        }
    }

    public async Task<BusinessResult> ApproveAccountAsync(int userId)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng");
            }

            if (user.Status != DAL.Models.UsersStatus.Pending)
            {
                return BusinessResult.Failure("Tài khoản không ở trạng thái chờ duyệt");
            }

            user.Status = DAL.Models.UsersStatus.Active;
            await _usersRepository.UpdateAsync(user);
            return BusinessResult.Success();
        }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Lỗi khi duyệt tài khoản {UserId}", userId);
            return BusinessResult.Failure($"Lỗi khi duyệt tài khoản: {ex.Message}");
        }
    }

    public async Task<BusinessResult> ResetPasswordAsync(ResetPasswordDto resetDto)
    {
        try
        {
            var user = await _usersRepository.GetByEmailAsync(resetDto.Email);
            if (user == null)
            {
                return BusinessResult.Failure("Không tìm thấy người dùng với email này");
            }

            // Tạo mật khẩu mới ngẫu nhiên
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            // Cập nhật mật khẩu mới
            await _usersRepository.UpdatePasswordAsync(user.UserID, hashedPassword);

            // TODO: Gửi email chứa mật khẩu tạm thời cho người dùng
            _logger.LogInformation("Đã tạo mật khẩu tạm thời cho user {UserId}", user.UserID);

            return BusinessResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi reset mật khẩu cho email {Email}", resetDto.Email);
            return BusinessResult.Failure($"Lỗi khi reset mật khẩu: {ex.Message}");
        }
    }
} 