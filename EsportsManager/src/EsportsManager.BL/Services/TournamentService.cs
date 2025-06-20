using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.BL.Services;

public class TournamentService : ITournamentService
{
    private static readonly List<Tournament> _tournaments = new();
    private static int _nextId = 1;
    private readonly ILogger<TournamentService> _logger;

    public TournamentService(ILogger<TournamentService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<List<Tournament>>> GetAllAsync()
    {
        try
        {
            return ServiceResult<List<Tournament>>.Success(_tournaments.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tournaments");
            return ServiceResult<List<Tournament>>.Failure("Failed to retrieve tournaments.");
        }
    }

    public async Task<ServiceResult<Tournament>> GetByIdAsync(int id)
    {
        try
        {
            var tournament = _tournaments.FirstOrDefault(t => t.TournamentId == id);
            if (tournament == null)
                return ServiceResult<Tournament>.Failure($"Tournament with ID {id} not found.");

            return ServiceResult<Tournament>.Success(tournament);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tournament by ID {TournamentId}", id);
            return ServiceResult<Tournament>.Failure("Failed to retrieve tournament.");
        }
    }

    public async Task<ServiceResult> CreateAsync(Tournament tournament)
    {
        try
        {
            tournament.TournamentId = _nextId++;
            _tournaments.Add(tournament);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tournament");
            return ServiceResult.Failure("Failed to create tournament.");
        }
    }

    public async Task<ServiceResult> UpdateAsync(Tournament tournament)
    {
        try
        {
            var idx = _tournaments.FindIndex(x => x.TournamentId == tournament.TournamentId);
            if (idx < 0)
                return ServiceResult.Failure($"Tournament with ID {tournament.TournamentId} not found.");

            _tournaments[idx] = tournament;
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tournament {TournamentId}", tournament.TournamentId);
            return ServiceResult.Failure("Failed to update tournament.");
        }
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        try
        {
            var tournament = _tournaments.FirstOrDefault(t => t.TournamentId == id);
            if (tournament == null)
                return ServiceResult.Failure($"Tournament with ID {id} not found.");

            _tournaments.Remove(tournament);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tournament {TournamentId}", id);
            return ServiceResult.Failure("Failed to delete tournament.");
        }
    }

    public async Task<ServiceResult<List<Tournament>>> GetUpcomingTournamentsAsync()
    {
        try
        {
            var upcoming = _tournaments
                .Where(t => t.StartDate > DateTime.UtcNow && t.Status == "Upcoming")
                .OrderBy(t => t.StartDate)
                .ToList();

            return ServiceResult<List<Tournament>>.Success(upcoming);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming tournaments");
            return ServiceResult<List<Tournament>>.Failure("Failed to retrieve upcoming tournaments.");
        }
    }

    public async Task<ServiceResult<List<Tournament>>> GetActiveTournamentsAsync()
    {
        try
        {
            var active = _tournaments
                .Where(t => t.StartDate <= DateTime.UtcNow && t.EndDate >= DateTime.UtcNow)
                .OrderBy(t => t.EndDate)
                .ToList();

            return ServiceResult<List<Tournament>>.Success(active);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tournaments");
            return ServiceResult<List<Tournament>>.Failure("Failed to retrieve active tournaments.");
        }
    }

    public async Task<ServiceResult<List<Tournament>>> GetCompletedTournamentsAsync()
    {
        try
        {
            var completed = _tournaments
                .Where(t => t.EndDate < DateTime.UtcNow || t.Status == "Completed")
                .OrderByDescending(t => t.EndDate)
                .ToList();

            return ServiceResult<List<Tournament>>.Success(completed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed tournaments");
            return ServiceResult<List<Tournament>>.Failure("Failed to retrieve completed tournaments.");
        }
    }
}
