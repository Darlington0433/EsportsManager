using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class FeedbackService : IFeedbackService
{
    private static readonly List<Feedback> _feedbacks = new();
    private static int _nextId = 1;
    private readonly ILogger<FeedbackService> _logger;

    public FeedbackService(ILogger<FeedbackService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Add some sample feedback if the list is empty
        if (_feedbacks.Count == 0)
        {
            _feedbacks.Add(new Feedback
            {
                FeedbackId = _nextId++,
                UserId = 1,
                TournamentId = 1,
                Rating = 5,
                Comment = "Great tournament, well organized!",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            });

            _feedbacks.Add(new Feedback
            {
                FeedbackId = _nextId++,
                UserId = 2,
                TournamentId = 1,
                Rating = 4,
                Comment = "Good experience, but schedule could be better.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });
        }
    }

    public async Task<ServiceResult<List<Feedback>>> GetAllAsync()
    {
        try
        {
            return ServiceResult<List<Feedback>>.Success(_feedbacks.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all feedback");
            return ServiceResult<List<Feedback>>.Failure("Failed to retrieve feedback.");
        }
    }

    public async Task<ServiceResult<Feedback>> GetByIdAsync(int id)
    {
        try
        {
            var feedback = _feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
                return ServiceResult<Feedback>.Failure($"Feedback with ID {id} not found.");

            return ServiceResult<Feedback>.Success(feedback);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feedback by ID {FeedbackId}", id);
            return ServiceResult<Feedback>.Failure("Failed to retrieve feedback.");
        }
    }

    public async Task<ServiceResult> CreateAsync(Feedback feedback)
    {
        try
        {
            feedback.FeedbackId = _nextId++;
            _feedbacks.Add(feedback);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating feedback");
            return ServiceResult.Failure("Failed to create feedback.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Feedback feedback)
    {
        try
        {
            var idx = _feedbacks.FindIndex(f => f.FeedbackId == feedback.FeedbackId);
            if (idx < 0)
                return ServiceResult.Failure($"Feedback with ID {feedback.FeedbackId} not found.");

            _feedbacks[idx] = feedback;
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating feedback {FeedbackId}", feedback.FeedbackId);
            return ServiceResult.Failure("Failed to update feedback.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var feedback = _feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
                return ServiceResult.Failure($"Feedback with ID {id} not found.");

            _feedbacks.Remove(feedback);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting feedback {FeedbackId}", id);
            return ServiceResult.Failure("Failed to delete feedback.");
        }
    }

    public async Task<ServiceResult<List<Feedback>>> GetFeedbackByUserIdAsync(int userId)
    {
        try
        {
            var userFeedback = _feedbacks
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            return ServiceResult<List<Feedback>>.Success(userFeedback);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feedback for user {UserId}", userId);
            return ServiceResult<List<Feedback>>.Failure("Failed to retrieve user feedback.");
        }
    }

    public async Task<ServiceResult<List<Feedback>>> GetRecentFeedbackAsync(int count = 10)
    {
        try
        {
            var recentFeedback = _feedbacks
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToList();

            return ServiceResult<List<Feedback>>.Success(recentFeedback);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent feedback");
            return ServiceResult<List<Feedback>>.Failure("Failed to retrieve recent feedback.");
        }
    }
}
