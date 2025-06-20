using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class TeamService : ITeamService
{
    private static readonly List<Team> _teams = new();
    private static int _nextId = 1;
    private readonly ILogger<TeamService> _logger;

    public TeamService(ILogger<TeamService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<List<Team>>> GetAllAsync()
    {
        try
        {
            return ServiceResult<List<Team>>.Success(_teams.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all teams");
            return ServiceResult<List<Team>>.Failure("Failed to retrieve teams.");
        }
    }

    public async Task<ServiceResult<Team>> GetByIdAsync(int id)
    {
        try
        {
            var team = _teams.FirstOrDefault(t => t.TeamId == id);
            if (team == null)
                return ServiceResult<Team>.Failure($"Team with ID {id} not found.");

            return ServiceResult<Team>.Success(team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team by ID {TeamId}", id);
            return ServiceResult<Team>.Failure("Failed to retrieve team.");
        }
    }

    public async Task<ServiceResult> CreateAsync(Team team)
    {
        try
        {
            team.TeamId = _nextId++;
            _teams.Add(team);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            return ServiceResult.Failure("Failed to create team.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Team team)
    {
        try
        {
            var idx = _teams.FindIndex(t => t.TeamId == team.TeamId);
            if (idx < 0)
                return ServiceResult.Failure($"Team with ID {team.TeamId} not found.");

            _teams[idx] = team;
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId}", team.TeamId);
            return ServiceResult.Failure("Failed to update team.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var team = _teams.FirstOrDefault(t => t.TeamId == id);
            if (team == null)
                return ServiceResult.Failure($"Team with ID {id} not found.");

            _teams.Remove(team);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team {TeamId}", id);
            return ServiceResult.Failure("Failed to delete team.");
        }
    }

    public async Task<ServiceResult<Team>> GetTeamByUserIdAsync(int userId)
    {
        try
        {
            var team = _teams.FirstOrDefault(t => t.CaptainId == userId || t.MemberIds.Contains(userId));
            if (team == null)
                return ServiceResult<Team>.Failure($"No team found for user ID {userId}.");

            return ServiceResult<Team>.Success(team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team by user ID {UserId}", userId);
            return ServiceResult<Team>.Failure("Failed to retrieve team.");
        }
    }

    public async Task<ServiceResult> AddMemberAsync(int teamId, int userId)
    {
        try
        {
            var team = _teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team == null)
                return ServiceResult.Failure($"Team with ID {teamId} not found.");

            if (team.MemberIds.Contains(userId))
                return ServiceResult.Failure($"User with ID {userId} is already a member of this team.");

            team.MemberIds.Add(userId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member {UserId} to team {TeamId}", userId, teamId);
            return ServiceResult.Failure("Failed to add team member.");
        }
    }

    public async Task<ServiceResult> RemoveMemberAsync(int teamId, int userId)
    {
        try
        {
            var team = _teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team == null)
                return ServiceResult.Failure($"Team with ID {teamId} not found.");

            if (team.CaptainId == userId)
                return ServiceResult.Failure("Cannot remove team captain from team.");

            if (!team.MemberIds.Contains(userId))
                return ServiceResult.Failure($"User with ID {userId} is not a member of this team.");

            team.MemberIds.Remove(userId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member {UserId} from team {TeamId}", userId, teamId);
            return ServiceResult.Failure("Failed to remove team member.");
        }
    }

    public async Task<ServiceResult<bool>> IsUserTeamCaptainAsync(int userId)
    {
        try
        {
            var isCaptain = _teams.Any(t => t.CaptainId == userId);
            return ServiceResult<bool>.Success(isCaptain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is team captain", userId);
            return ServiceResult<bool>.Failure("Failed to check team captain status.");
        }
    }

    public async Task<ServiceResult<List<Team>>> GetTeamsForTournamentAsync(int tournamentId)
    {
        try
        {
            // In a real application, this would query a tournament registration table
            // For this demo, we'll just return all teams as if they're registered
            return ServiceResult<List<Team>>.Success(_teams.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teams for tournament {TournamentId}", tournamentId);
            return ServiceResult<List<Team>>.Failure("Failed to retrieve tournament teams.");
        }
    }
}
