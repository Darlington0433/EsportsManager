using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class AchievementService : IAchievementService
{
    private static readonly List<Achievement> _achievements = new();
    private static int _nextId = 1;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(ILogger<AchievementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Add some sample achievements if the list is empty
        if (_achievements.Count == 0)
        {
            _achievements.Add(new Achievement
            {
                AchievementId = _nextId++,
                Title = "First Victory",
                Description = "Win your first tournament match",
                UserId = 1,
                AchievementDate = DateTime.UtcNow.AddDays(-30)
            });

            _achievements.Add(new Achievement
            {
                AchievementId = _nextId++,
                Title = "Tournament Champion",
                Description = "Win a tournament",
                UserId = 1,
                AchievementDate = DateTime.UtcNow.AddDays(-15)
            });

            _achievements.Add(new Achievement
            {
                AchievementId = _nextId++,
                Title = "Team Captain",
                Description = "Create a team",
                UserId = 2,
                AchievementDate = DateTime.UtcNow.AddDays(-45)
            });
        }
    }

    public async Task<ServiceResult<List<Achievement>>> GetAllAsync()
    {
        try
        {
            return ServiceResult<List<Achievement>>.Success(_achievements.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all achievements");
            return ServiceResult<List<Achievement>>.Failure("Failed to retrieve achievements.");
        }
    }

    public async Task<ServiceResult<Achievement>> GetByIdAsync(int id)
    {
        try
        {
            var achievement = _achievements.FirstOrDefault(a => a.AchievementId == id);
            if (achievement == null)
                return ServiceResult<Achievement>.Failure($"Achievement with ID {id} not found.");

            return ServiceResult<Achievement>.Success(achievement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting achievement by ID {AchievementId}", id);
            return ServiceResult<Achievement>.Failure("Failed to retrieve achievement.");
        }
    }

    public async Task<ServiceResult> CreateAsync(Achievement achievement)
    {
        try
        {
            achievement.AchievementId = _nextId++;
            _achievements.Add(achievement);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating achievement");
            return ServiceResult.Failure("Failed to create achievement.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Achievement achievement)
    {
        try
        {
            var idx = _achievements.FindIndex(a => a.AchievementId == achievement.AchievementId);
            if (idx < 0)
                return ServiceResult.Failure($"Achievement with ID {achievement.AchievementId} not found.");

            _achievements[idx] = achievement;
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating achievement {AchievementId}", achievement.AchievementId);
            return ServiceResult.Failure("Failed to update achievement.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var achievement = _achievements.FirstOrDefault(a => a.AchievementId == id);
            if (achievement == null)
                return ServiceResult.Failure($"Achievement with ID {id} not found.");

            _achievements.Remove(achievement);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting achievement {AchievementId}", id);
            return ServiceResult.Failure("Failed to delete achievement.");
        }
    }

    public async Task<ServiceResult<List<Achievement>>> GetAchievementsByUserIdAsync(int userId)
    {
        try
        {
            var userAchievements = _achievements
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AchievementDate)
                .ToList();

            return ServiceResult<List<Achievement>>.Success(userAchievements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting achievements for user {UserId}", userId);
            return ServiceResult<List<Achievement>>.Failure("Failed to retrieve user achievements.");
        }
    }

    public async Task<ServiceResult<List<Achievement>>> GetRecentAchievementsAsync(int count = 10)
    {
        try
        {
            var recentAchievements = _achievements
                .OrderByDescending(a => a.AchievementDate)
                .Take(count)
                .ToList();

            return ServiceResult<List<Achievement>>.Success(recentAchievements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent achievements");
            return ServiceResult<List<Achievement>>.Failure("Failed to retrieve recent achievements.");
        }
    }
}
