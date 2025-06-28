using EsportsManager.BL.Constants;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Utilities;
using System;
using System.Collections.Generic;

namespace EsportsManager.BL.Services;

/// <summary>
/// Tournament validation and calculation service for business logic
/// </summary>
public class TournamentValidationService
{
    /// <summary>
    /// Validates tournament name
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateTournamentName(string? tournamentName)
    {
        var errors = new List<string>();
        if (ValidationHelper.IsNullOrEmpty(tournamentName, "Tên giải đấu", errors) ||
            !ValidationHelper.ValidateLength(tournamentName!, "Tên giải đấu", 3, 100, errors))
        {
            return (false, string.Join("; ", errors));
        }
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates game ID
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateGameId(int gameId)
    {
        var errors = new List<string>();
        if (!ValidationHelper.IsValidValue(gameId, TournamentConstants.GAME_TYPES.Keys, "Game ID", errors))
        {
            return (false, string.Join("; ", errors));
        }
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates max teams with business rules
    /// </summary>
    public static (bool IsValid, string ErrorMessage, int ValidatedValue) ValidateMaxTeams(string? input)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(input?.Trim()))
        {
            return (true, string.Empty, TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        if (!int.TryParse(input, out int maxTeams))
        {
            return (false, "Số team tối đa phải là số nguyên!", TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        if (maxTeams < TournamentConstants.MIN_MAX_TEAMS || maxTeams > TournamentConstants.MAX_MAX_TEAMS)
        {
            return (false, $"Số team tối đa phải từ {TournamentConstants.MIN_MAX_TEAMS} đến {TournamentConstants.MAX_MAX_TEAMS}!", TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        return (true, string.Empty, maxTeams);
    }
    
    /// <summary>
    /// Validates entry fee with business rules
    /// </summary>
    public static (bool IsValid, string ErrorMessage, decimal ValidatedValue) ValidateEntryFee(string? input)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(input?.Trim()))
        {
            return (true, string.Empty, TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        if (!decimal.TryParse(input, out decimal entryFee))
        {
            return (false, "Phí tham gia phải là số!", TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        if (!ValidationHelper.IsValidAmount(entryFee, TournamentConstants.MIN_ENTRY_FEE, TournamentConstants.MAX_ENTRY_FEE, errors))
        {
            return (false, string.Join("; ", errors), TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        return (true, string.Empty, entryFee);
    }
    
    /// <summary>
    /// Creates default tournament dates based on business rules
    /// </summary>
    public static (DateTime RegistrationDeadline, DateTime StartDate, DateTime EndDate) GetDefaultTournamentDates()
    {
        var now = DateTime.Now;
        return (
            RegistrationDeadline: now.AddDays(TournamentConstants.DEFAULT_REGISTRATION_DAYS),
            StartDate: now.AddDays(TournamentConstants.DEFAULT_START_DAYS),
            EndDate: now.AddDays(TournamentConstants.DEFAULT_END_DAYS)
        );
    }

    /// <summary>
    /// Validates tournament dates
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateTournamentDates(
        DateTime registrationDeadline,
        DateTime startDate,
        DateTime endDate)
    {
        var errors = new List<string>();
        var now = DateTime.Now;

        if (!ValidationHelper.IsValidDate(registrationDeadline, now, startDate, errors))
        {
            return (false, string.Join("; ", errors));
        }

        if (!ValidationHelper.IsValidDate(startDate, registrationDeadline, endDate, errors))
        {
            return (false, string.Join("; ", errors));
        }

        if (!ValidationHelper.IsValidDate(endDate, startDate, now.AddYears(1), errors))
        {
            return (false, string.Join("; ", errors));
        }

        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates complete tournament create data
    /// </summary>
    public static (bool IsValid, List<string> ErrorMessages) ValidateTournamentCreateDto(TournamentCreateDto dto)
    {
        var errors = new List<string>();
        
        var nameValidation = ValidateTournamentName(dto.TournamentName);
        if (!nameValidation.IsValid)
            errors.Add(nameValidation.ErrorMessage);
            
        var gameValidation = ValidateGameId(dto.GameId);
        if (!gameValidation.IsValid)
            errors.Add(gameValidation.ErrorMessage);
            
        if (dto.MaxTeams < TournamentConstants.MIN_MAX_TEAMS || dto.MaxTeams > TournamentConstants.MAX_MAX_TEAMS)
            errors.Add($"Số team tối đa phải từ {TournamentConstants.MIN_MAX_TEAMS} đến {TournamentConstants.MAX_MAX_TEAMS}!");
            
        if (!ValidationHelper.IsValidAmount(dto.EntryFee, TournamentConstants.MIN_ENTRY_FEE, TournamentConstants.MAX_ENTRY_FEE, errors))
            errors.Add(string.Join("; ", errors));

        var datesValidation = ValidateTournamentDates(dto.RegistrationDeadline, dto.StartDate, dto.EndDate);
        if (!datesValidation.IsValid)
            errors.Add(datesValidation.ErrorMessage);
        
        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates tournament banner image
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateTournamentBanner(string? fileName, long fileSize)
    {
        var errors = new List<string>();
        var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
        
        if (!string.IsNullOrEmpty(fileName))
        {
            ValidationHelper.IsValidFileExtension(fileName, validExtensions, errors);
            ValidationHelper.IsValidFileSize(fileSize, 10 * 1024 * 1024, errors); // 10MB max
        }
        
        return errors.Count > 0 ? (false, string.Join("; ", errors)) : (true, string.Empty);
    }
}
