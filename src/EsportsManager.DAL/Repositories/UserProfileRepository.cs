using System;
using System.Threading.Tasks;
using Dapper;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Repository cho UserProfile
    /// </summary>
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<UserProfileRepository> _logger;

        public UserProfileRepository(DataContext context, ILogger<UserProfileRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lấy profile theo UserID
        /// </summary>
        public async Task<UserProfile?> GetByUserIdAsync(int userId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    SELECT ProfileID, UserID, Bio, DateOfBirth, Country, City, SocialLinks, Achievements, CreatedAt, UpdatedAt
                    FROM UserProfiles 
                    WHERE UserID = @UserId";

                var profile = await connection.QueryFirstOrDefaultAsync<UserProfile>(sql, new { UserId = userId });
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user profile for UserID: {userId}");
                return null;
            }
        }

        /// <summary>
        /// Tạo profile mới
        /// </summary>
        public async Task<UserProfile> CreateAsync(UserProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    INSERT INTO UserProfiles (UserID, Bio, DateOfBirth, Country, City, SocialLinks, Achievements, CreatedAt, UpdatedAt)
                    VALUES (@UserID, @Bio, @DateOfBirth, @Country, @City, @SocialLinks, @Achievements, @CreatedAt, @UpdatedAt);
                    SELECT LAST_INSERT_ID();";

                profile.CreatedAt = DateTime.Now;
                profile.UpdatedAt = DateTime.Now;

                var profileId = await connection.QuerySingleAsync<int>(sql, profile);
                profile.ProfileID = profileId;

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user profile for UserID: {profile.UserID}");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật profile
        /// </summary>
        public async Task<UserProfile> UpdateAsync(UserProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            try
            {
                using var connection = _context.CreateConnection();

                const string sql = @"
                    UPDATE UserProfiles 
                    SET Bio = @Bio, DateOfBirth = @DateOfBirth, Country = @Country, City = @City, 
                        SocialLinks = @SocialLinks, Achievements = @Achievements, UpdatedAt = @UpdatedAt
                    WHERE ProfileID = @ProfileID";

                profile.UpdatedAt = DateTime.Now;

                await connection.ExecuteAsync(sql, profile);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user profile for ProfileID: {profile.ProfileID}");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật achievements cho user
        /// </summary>
        public async Task<bool> UpdateAchievementsAsync(int userId, string achievementsJson)
        {
            try
            {
                var profile = await GetByUserIdAsync(userId);
                if (profile == null)
                {
                    // Tạo profile mới nếu chưa có
                    profile = new UserProfile
                    {
                        UserID = userId,
                        Achievements = achievementsJson,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    await CreateAsync(profile);
                }
                else
                {
                    profile.Achievements = achievementsJson;
                    profile.UpdatedAt = DateTime.Now;
                    await UpdateAsync(profile);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating achievements for UserID: {userId}");
                return false;
            }
        }
    }
}
