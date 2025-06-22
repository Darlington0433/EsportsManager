using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

/// <summary>
/// Tournament Service Implementation - Mock data for development
/// Production: Replace with real database operations
/// </summary>
public class TournamentService : ITournamentService
{
    private readonly ILogger<TournamentService> _logger;
    private static readonly List<TournamentDto> _mockTournaments = new();
    private static int _nextId = 1;

    public TournamentService(ILogger<TournamentService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeMockData();
    }

    public async Task<TournamentDto> CreateTournamentAsync(TournamentCreateDto createDto)
    {
        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto));

        // Validate dates
        if (createDto.StartDate >= createDto.EndDate)
            throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc");

        if (createDto.StartDate < DateTime.Now.Date)
            throw new ArgumentException("Ngày bắt đầu không được trong quá khứ");

        try
        {
            var tournament = new TournamentDto
            {
                Id = _nextId++,
                Name = createDto.Name,
                Description = createDto.Description,
                Game = createDto.Game,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                MaxParticipants = createDto.MaxParticipants,
                CurrentParticipants = 0,
                EntryFee = createDto.EntryFee,
                PrizePool = createDto.PrizePool,
                Format = createDto.Format,
                Type = createDto.Type,
                Status = TournamentStatus.Draft,
                Rules = createDto.Rules,
                IsPublic = createDto.IsPublic,
                CreatedAt = DateTime.Now,
                CreatedBy = 1, // Mock admin ID
                CreatedByUsername = "admin"
            };

            _mockTournaments.Add(tournament);
            _logger.LogInformation("Created tournament {TournamentId}: {Name}", tournament.Id, tournament.Name);
            
            await Task.Delay(50); // Simulate async operation
            return tournament;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tournament {Name}", createDto.Name);
            throw;
        }
    }

    public async Task<List<TournamentDto>> GetAllTournamentsAsync()
    {
        await Task.Delay(30); // Simulate async operation
        return _mockTournaments.OrderByDescending(t => t.CreatedAt).ToList();
    }

    public async Task<List<TournamentDto>> GetActiveTournamentsAsync()
    {
        await Task.Delay(30);
        return _mockTournaments
            .Where(t => t.Status == TournamentStatus.InProgress || t.Status == TournamentStatus.RegistrationOpen)
            .OrderBy(t => t.StartDate)
            .ToList();
    }

    public async Task<TournamentDto?> GetTournamentByIdAsync(int tournamentId)
    {
        await Task.Delay(20);
        return _mockTournaments.FirstOrDefault(t => t.Id == tournamentId);
    }

    public async Task<bool> UpdateTournamentAsync(int tournamentId, TournamentUpdateDto updateDto)
    {
        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto));

        var tournament = _mockTournaments.FirstOrDefault(t => t.Id == tournamentId);
        if (tournament == null)
            return false;

        try
        {
            // Update only non-null properties
            if (!string.IsNullOrEmpty(updateDto.Name))
                tournament.Name = updateDto.Name;
            if (!string.IsNullOrEmpty(updateDto.Description))
                tournament.Description = updateDto.Description;
            if (updateDto.StartDate.HasValue)
                tournament.StartDate = updateDto.StartDate.Value;
            if (updateDto.EndDate.HasValue)
                tournament.EndDate = updateDto.EndDate.Value;
            if (updateDto.MaxParticipants.HasValue)
                tournament.MaxParticipants = updateDto.MaxParticipants.Value;
            if (updateDto.EntryFee.HasValue)
                tournament.EntryFee = updateDto.EntryFee.Value;
            if (updateDto.PrizePool.HasValue)
                tournament.PrizePool = updateDto.PrizePool.Value;
            if (!string.IsNullOrEmpty(updateDto.Rules))
                tournament.Rules = updateDto.Rules;
            if (updateDto.IsPublic.HasValue)
                tournament.IsPublic = updateDto.IsPublic.Value;
            if (updateDto.Status.HasValue)
                tournament.Status = updateDto.Status.Value;

            await Task.Delay(40);
            _logger.LogInformation("Updated tournament {TournamentId}", tournamentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tournament {TournamentId}", tournamentId);
            throw;
        }
    }

    public async Task<bool> DeleteTournamentAsync(int tournamentId)
    {
        var tournament = _mockTournaments.FirstOrDefault(t => t.Id == tournamentId);
        if (tournament == null)
            return false;

        // Don't allow deletion of active tournaments
        if (tournament.Status == TournamentStatus.InProgress)
            throw new InvalidOperationException("Không thể xóa giải đấu đang diễn ra");

        _mockTournaments.Remove(tournament);
        await Task.Delay(30);
        _logger.LogInformation("Deleted tournament {TournamentId}: {Name}", tournamentId, tournament.Name);
        return true;
    }

    public async Task<List<TournamentDto>> SearchTournamentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllTournamentsAsync();

        await Task.Delay(40);
        return _mockTournaments
            .Where(t => t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       t.Game.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
    }

    public async Task<bool> ToggleTournamentStatusAsync(int tournamentId)
    {
        var tournament = _mockTournaments.FirstOrDefault(t => t.Id == tournamentId);
        if (tournament == null)
            return false;

        // Simple status toggle logic
        tournament.Status = tournament.Status switch
        {
            TournamentStatus.Draft => TournamentStatus.Published,
            TournamentStatus.Published => TournamentStatus.RegistrationOpen,
            TournamentStatus.RegistrationOpen => TournamentStatus.RegistrationClosed,
            TournamentStatus.RegistrationClosed => TournamentStatus.InProgress,
            TournamentStatus.InProgress => TournamentStatus.Completed,
            _ => tournament.Status
        };

        await Task.Delay(30);
        _logger.LogInformation("Toggled tournament {TournamentId} status to {Status}", tournamentId, tournament.Status);
        return true;
    }

    private static void InitializeMockData()
    {
        if (_mockTournaments.Any())
            return;

        _mockTournaments.AddRange(new[]
        {
            new TournamentDto
            {
                Id = _nextId++,
                Name = "VTC Esports Championship 2025",
                Description = "Giải đấu esports lớn nhất Việt Nam năm 2025",
                Game = "League of Legends",
                StartDate = DateTime.Now.AddDays(30),
                EndDate = DateTime.Now.AddDays(45),
                MaxParticipants = 64,
                CurrentParticipants = 32,
                EntryFee = 500000,
                PrizePool = 1000000000,
                Format = TournamentFormat.SingleElimination,
                Type = TournamentType.Team,
                Status = TournamentStatus.RegistrationOpen,
                Rules = "Tuân thủ luật chơi quốc tế",
                IsPublic = true,
                CreatedAt = DateTime.Now.AddDays(-7),
                CreatedBy = 1,
                CreatedByUsername = "admin"
            },
            new TournamentDto
            {
                Id = _nextId++,
                Name = "Vietnam Mobile Legends Cup",
                Description = "Giải đấu Mobile Legends cho game thủ Việt Nam",
                Game = "Mobile Legends",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(20),
                MaxParticipants = 32,
                CurrentParticipants = 28,
                EntryFee = 200000,
                PrizePool = 50000000,
                Format = TournamentFormat.DoubleElimination,
                Type = TournamentType.Team,
                Status = TournamentStatus.RegistrationOpen,
                Rules = "5v5, Best of 3",
                IsPublic = true,
                CreatedAt = DateTime.Now.AddDays(-3),
                CreatedBy = 1,
                CreatedByUsername = "admin"
            }
        });
    }
}
