using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class VoteService : IVoteService
{
    private static readonly List<Vote> _votes = new();
    private static int _nextId = 1;
    private readonly ILogger<VoteService> _logger;

    public VoteService(ILogger<VoteService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Add some sample votes if the list is empty
        if (_votes.Count == 0)
        {
            _votes.Add(new Vote
            {
                VoteId = _nextId++,
                UserId = 1,
                EntityType = "Team",
                EntityId = 1,
                IsUpvote = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Comment = "Great team!"
            });

            _votes.Add(new Vote
            {
                VoteId = _nextId++,
                UserId = 2,
                EntityType = "Tournament",
                EntityId = 1,
                IsUpvote = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            });
        }
    }

    public async Task<ServiceResult<List<Vote>>> GetAllAsync()
    {
        try
        {
            return ServiceResult<List<Vote>>.Success(_votes.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all votes");
            return ServiceResult<List<Vote>>.Failure("Failed to retrieve votes.");
        }
    }

    public async Task<ServiceResult<Vote>> GetByIdAsync(int id)
    {
        try
        {
            var vote = _votes.FirstOrDefault(v => v.VoteId == id);
            if (vote == null)
                return ServiceResult<Vote>.Failure($"Vote with ID {id} not found.");

            return ServiceResult<Vote>.Success(vote);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vote by ID {VoteId}", id);
            return ServiceResult<Vote>.Failure("Failed to retrieve vote.");
        }
    }

    public async Task<ServiceResult> CreateAsync(Vote vote)
    {
        try
        {
            // Check if user already voted for this entity
            var existingVote = _votes.FirstOrDefault(v =>
                v.UserId == vote.UserId &&
                v.EntityType == vote.EntityType &&
                v.EntityId == vote.EntityId);

            if (existingVote != null)
                return ServiceResult.Failure("You have already voted for this item.");

            vote.VoteId = _nextId++;
            _votes.Add(vote);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vote");
            return ServiceResult.Failure("Failed to create vote.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Vote vote)
    {
        try
        {
            var idx = _votes.FindIndex(v => v.VoteId == vote.VoteId);
            if (idx < 0)
                return ServiceResult.Failure($"Vote with ID {vote.VoteId} not found.");

            _votes[idx] = vote;
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vote {VoteId}", vote.VoteId);
            return ServiceResult.Failure("Failed to update vote.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var vote = _votes.FirstOrDefault(v => v.VoteId == id);
            if (vote == null)
                return ServiceResult.Failure($"Vote with ID {id} not found.");

            _votes.Remove(vote);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vote {VoteId}", id);
            return ServiceResult.Failure("Failed to delete vote.");
        }
    }

    public async Task<ServiceResult<List<Vote>>> GetVotesByUserIdAsync(int userId)
    {
        try
        {
            var userVotes = _votes
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();

            return ServiceResult<List<Vote>>.Success(userVotes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting votes for user {UserId}", userId);
            return ServiceResult<List<Vote>>.Failure("Failed to retrieve user votes.");
        }
    }

    public async Task<ServiceResult<List<Vote>>> GetVotesByEntityIdAsync(string entityType, int entityId)
    {
        try
        {
            var entityVotes = _votes
                .Where(v => v.EntityType == entityType && v.EntityId == entityId)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();

            return ServiceResult<List<Vote>>.Success(entityVotes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting votes for entity {EntityType}/{EntityId}", entityType, entityId);
            return ServiceResult<List<Vote>>.Failure("Failed to retrieve entity votes.");
        }
    }

    public async Task<ServiceResult<Dictionary<int, int>>> GetVoteCountsByEntityIdsAsync(string entityType, List<int> entityIds)
    {
        try
        {
            var voteCounts = _votes
                .Where(v => v.EntityType == entityType && entityIds.Contains(v.EntityId))
                .GroupBy(v => v.EntityId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count(v => v.IsUpvote) - g.Count(v => !v.IsUpvote)
                );

            // Add missing entities with zero votes
            foreach (var entityId in entityIds)
            {
                if (!voteCounts.ContainsKey(entityId))
                {
                    voteCounts[entityId] = 0;
                }
            }

            return ServiceResult<Dictionary<int, int>>.Success(voteCounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vote counts for entity {EntityType}", entityType);
            return ServiceResult<Dictionary<int, int>>.Failure("Failed to retrieve vote counts.");
        }
    }

    public async Task<ServiceResult<bool>> HasUserVotedForEntityAsync(int userId, string entityType, int entityId)
    {
        try
        {
            var hasVoted = _votes.Any(v =>
                v.UserId == userId &&
                v.EntityType == entityType &&
                v.EntityId == entityId);

            return ServiceResult<bool>.Success(hasVoted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} voted for entity {EntityType}/{EntityId}", userId, entityType, entityId);
            return ServiceResult<bool>.Failure("Failed to check vote status.");
        }
    }
}
