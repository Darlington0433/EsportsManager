using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using EsportsManager.DAL.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Users Repository implementation - áp dụng Single Responsibility Principle
    /// Chỉ lo về data access cho Users entity với database thực tế
    /// </summary>
    public class UsersRepository : BaseRepository<Users, int>, IUsersRepository
    {
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(DataContext context, ILogger<UsersRepository> logger) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        public async Task<Users?> GetByUsernameAsync(string username)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Username = @Username AND Status != 'Deleted'";

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Username = username });

                _logger.LogDebug("Retrieved user by username: {Username}, Found: {Found}", username, user != null);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        /// <summary>
        /// Lấy user theo email
        /// </summary>
        public async Task<Users?> GetByEmailAsync(string email)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Email = @Email AND Status != 'Deleted'";

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Email = email });

                _logger.LogDebug("Retrieved user by email: {Email}, Found: {Found}", email, user != null);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra username có tồn tại không
        /// </summary>
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND Status != 'Deleted'";

                var count = await connection.QuerySingleAsync<int>(sql, new { Username = username });

                _logger.LogDebug("Username exists check: {Username}, Exists: {Exists}", username, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username exists: {Username}", username);
                throw;
            }
        }

        /// <summary>
        /// Kiểm tra email có tồn tại không
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Status != 'Deleted'";

                var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });

                _logger.LogDebug("Email exists check: {Email}, Exists: {Exists}", email, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email exists: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách user theo role
        /// </summary>
        public async Task<IEnumerable<Users>> GetByRoleAsync(string role)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Role = @Role AND Status != 'Deleted'
                    ORDER BY CreatedAt DESC";

                var users = await connection.QueryAsync<Users>(sql, new { Role = role });

                _logger.LogDebug("Retrieved users by role: {Role}, Count: {Count}", role, users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                throw;
            }
        }

        /// <summary>
        /// Lấy tất cả users đang active
        /// </summary>
        public async Task<IEnumerable<Users>> GetActiveUsersAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Status = 'Active'
                    ORDER BY LastLoginAt DESC";

                var users = await connection.QueryAsync<Users>(sql);

                _logger.LogDebug("Retrieved active users, Count: {Count}", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật password của user
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt 
                    WHERE UserID = @UserId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    PasswordHash = passwordHash,
                    UpdatedAt = DateTime.UtcNow
                });

                _logger.LogDebug("Updated password for user: {UserId}, Success: {Success}", userId, rowsAffected > 0);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật status của user
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int userId, string status)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET Status = @Status, UpdatedAt = @UpdatedAt 
                    WHERE UserID = @UserId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    Status = status,
                    UpdatedAt = DateTime.UtcNow
                });

                _logger.LogDebug("Updated status for user: {UserId} to {Status}, Success: {Success}", userId, status, rowsAffected > 0);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật token xác minh email
        /// </summary>
        public async Task<bool> UpdateEmailVerificationTokenAsync(int userId, string token)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET EmailVerificationToken = @Token, UpdatedAt = @UpdatedAt 
                    WHERE UserID = @UserId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    Token = token,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email verification token for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật token reset mật khẩu
        /// </summary>
        public async Task<bool> UpdatePasswordResetTokenAsync(int userId, string token, DateTime expiry)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET PasswordResetToken = @Token, PasswordResetExpiry = @Expiry, UpdatedAt = @UpdatedAt 
                    WHERE UserID = @UserId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    Token = token,
                    Expiry = expiry,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password reset token for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Xác minh email
        /// </summary>
        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET IsEmailVerified = 1, EmailVerificationToken = NULL, UpdatedAt = @UpdatedAt 
                    WHERE EmailVerificationToken = @Token";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    Token = token,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email with token: {Token}", token);
                throw;
            }
        }

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET LastLoginAt = @LastLoginAt, UpdatedAt = @UpdatedAt 
                    WHERE UserID = @UserId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    UserId = userId,
                    LastLoginAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách user cần duyệt (Pending)
        /// </summary>
        public async Task<IEnumerable<Users>> GetPendingUsersAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Status = 'Pending'
                    ORDER BY CreatedAt ASC";

                var users = await connection.QueryAsync<Users>(sql);

                _logger.LogDebug("Retrieved pending users, Count: {Count}", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending users");
                throw;
            }
        }

        /// <summary>
        /// Lấy user theo token xác thực email
        /// </summary>
        public async Task<Users?> GetByEmailVerificationTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE EmailVerificationToken = @Token AND Status != 'Deleted'";

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Token = token });

                _logger.LogDebug("Retrieved user by email verification token, Found: {Found}", user != null);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email verification token");
                throw;
            }
        }

        /// <summary>
        /// Lấy user theo token reset mật khẩu
        /// </summary>
        public async Task<Users?> GetByPasswordResetTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE PasswordResetToken = @Token AND Status != 'Deleted'";

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Token = token });

                _logger.LogDebug("Retrieved user by password reset token, Found: {Found}", user != null);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by password reset token");
                throw;
            }
        }

        // Override methods from BaseRepository
        public override async Task<Users?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE UserID = @Id AND Status != 'Deleted'";

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Id = id });

                _logger.LogDebug("Retrieved user by ID: {Id}, Found: {Found}", id, user != null);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                throw;
            }
        }

        public override async Task<IEnumerable<Users>> GetAllAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Status != 'Deleted'
                    ORDER BY CreatedAt DESC";

                var users = await connection.QueryAsync<Users>(sql);

                _logger.LogDebug("Retrieved all users, Count: {Count}", users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public override async Task<Users> AddAsync(Users entity)
        {
            try
            {
                using var connection = _context.CreateConnection(); const string sql = @"
                    INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, Status, 
                                     IsEmailVerified, EmailVerificationToken, PasswordResetToken,
                                     PasswordResetExpiry, CreatedAt)
                    VALUES (@Username, @Email, @PasswordHash, @FullName, @Role, @Status, 
                           @IsEmailVerified, @EmailVerificationToken, @PasswordResetToken,
                           @PasswordResetExpiry, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var userId = await connection.QuerySingleAsync<int>(sql, entity);
                entity.UserID = userId;

                _logger.LogDebug("Added new user: {Username}, ID: {UserId}", entity.Username, userId);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Username}", entity.Username);
                throw;
            }
        }

        public override async Task<Users> UpdateAsync(Users entity)
        {
            try
            {
                using var connection = _context.CreateConnection(); const string sql = @"
                    UPDATE Users 
                    SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, 
                        FullName = @FullName, Role = @Role, Status = @Status, 
                        IsEmailVerified = @IsEmailVerified, EmailVerificationToken = @EmailVerificationToken,
                        PasswordResetToken = @PasswordResetToken, PasswordResetExpiry = @PasswordResetExpiry,
                        UpdatedAt = @UpdatedAt
                    WHERE UserID = @UserID";

                entity.UpdatedAt = DateTime.UtcNow;
                var rowsAffected = await connection.ExecuteAsync(sql, entity);

                if (rowsAffected == 0)
                    throw new InvalidOperationException($"User with ID {entity.UserID} not found");

                _logger.LogDebug("Updated user: {Username}, ID: {UserId}", entity.Username, entity.UserID);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", entity.UserID);
                throw;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    UPDATE Users 
                    SET Status = 'Deleted', UpdatedAt = @UpdatedAt 
                    WHERE UserID = @Id";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    UpdatedAt = DateTime.UtcNow
                });

                _logger.LogDebug("Soft deleted user: {UserId}, Success: {Success}", id, rowsAffected > 0);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Users WHERE UserID = @Id AND Status != 'Deleted'";

                var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user exists: {UserId}", id);
                throw;
            }
        }

        public override async Task<int> CountAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = "SELECT COUNT(1) FROM Users WHERE Status != 'Deleted'";

                var count = await connection.QuerySingleAsync<int>(sql);

                _logger.LogDebug("Total users count: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users count");
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng theo trạng thái
        /// </summary>
        /// <param name="status">Trạng thái người dùng (Active, Inactive, etc.)</param>
        /// <returns>Danh sách người dùng</returns>
        public async Task<IEnumerable<Users>> GetByStatusAsync(string status)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, PasswordHash, FullName, Role, Status, 
                           IsEmailVerified, EmailVerificationToken, PasswordResetToken, 
                           PasswordResetExpiry, LastLoginAt, CreatedAt, UpdatedAt
                    FROM Users 
                    WHERE Status = @Status";

                var users = await connection.QueryAsync<Users>(sql, new { Status = status });

                _logger.LogDebug("Retrieved users by status: {Status}, Count: {Count}", status, users.Count());
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by status: {Status}", status);
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng người dùng theo trạng thái
        /// </summary>
        /// <param name="status">Trạng thái người dùng (Active, Inactive, etc.)</param>
        /// <returns>Số lượng người dùng</returns>
        public async Task<int> GetCountByStatusAsync(string status)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"SELECT COUNT(*) FROM Users WHERE Status = @Status";

                var count = await connection.QuerySingleAsync<int>(sql, new { Status = status });

                _logger.LogDebug("Users count by status {Status}: {Count}", status, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users count by status: {Status}", status);
                throw;
            }
        }

        /// <summary>
        /// Lấy số lượng người dùng theo vai trò
        /// </summary>
        /// <param name="role">Vai trò người dùng (Admin, Player, Viewer)</param>
        /// <returns>Số lượng người dùng</returns>
        public async Task<int> GetCountByRoleAsync(string role)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"SELECT COUNT(*) FROM Users WHERE Role = @Role AND Status != 'Deleted'";

                var count = await connection.QuerySingleAsync<int>(sql, new { Role = role });

                _logger.LogDebug("Users count by role {Role}: {Count}", role, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users count by role: {Role}", role);
                throw;
            }
        }

        /// <summary>
        /// Tìm kiếm user theo từ khóa
        /// </summary>
        public async Task<IEnumerable<Users>> SearchAsync(string searchTerm)
        {
            try
            {
                using var connection = _context.CreateConnection();
                const string sql = @"
                    SELECT UserID, Username, Email, FullName, Role, Status
                    FROM Users
                    WHERE (Username LIKE @SearchTerm OR Email LIKE @SearchTerm OR FullName LIKE @SearchTerm)
                      AND Status != 'Deleted'";

                var users = await connection.QueryAsync<Users>(sql, new { SearchTerm = $"%{searchTerm}%" });
                _logger.LogDebug("Search users with term: {SearchTerm}, Found: {Count}", searchTerm, users.AsList().Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}
