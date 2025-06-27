using EsportsManager.BL.Constants;
using EsportsManager.BL.DTOs;

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
        if (string.IsNullOrEmpty(tournamentName?.Trim()))
        {
            return (false, "Tên giải đấu không được để trống!");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates game ID
    /// </summary>
    public static (bool IsValid, string ErrorMessage) ValidateGameId(int gameId)
    {
        if (gameId < TournamentConstants.MIN_GAME_ID || !TournamentConstants.GAME_TYPES.ContainsKey(gameId))
        {
            return (false, "Game ID không hợp lệ!");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates max teams with business rules
    /// </summary>
    public static (bool IsValid, string ErrorMessage, int ValidatedValue) ValidateMaxTeams(string? input)
    {
        if (string.IsNullOrEmpty(input?.Trim()))
        {
            return (true, string.Empty, TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        if (!int.TryParse(input, out int maxTeams))
        {
            return (false, "Số team tối đa phải là số nguyên!", TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        if (maxTeams < TournamentConstants.MIN_MAX_TEAMS)
        {
            return (false, $"Số team tối đa phải ít nhất {TournamentConstants.MIN_MAX_TEAMS}!", TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        if (maxTeams > TournamentConstants.MAX_MAX_TEAMS)
        {
            return (false, $"Số team tối đa không được vượt quá {TournamentConstants.MAX_MAX_TEAMS}!", TournamentConstants.DEFAULT_MAX_TEAMS);
        }
        
        return (true, string.Empty, maxTeams);
    }
    
    /// <summary>
    /// Validates entry fee with business rules
    /// </summary>
    public static (bool IsValid, string ErrorMessage, decimal ValidatedValue) ValidateEntryFee(string? input)
    {
        if (string.IsNullOrEmpty(input?.Trim()))
        {
            return (true, string.Empty, TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        if (!decimal.TryParse(input, out decimal entryFee))
        {
            return (false, "Phí tham gia phải là số!", TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        if (entryFee < TournamentConstants.MIN_ENTRY_FEE)
        {
            return (false, "Phí tham gia không được âm!", TournamentConstants.DEFAULT_ENTRY_FEE);
        }
        
        if (entryFee > TournamentConstants.MAX_ENTRY_FEE)
        {
            return (false, $"Phí tham gia không được vượt quá {TournamentConstants.MAX_ENTRY_FEE:N0} VND!", TournamentConstants.DEFAULT_ENTRY_FEE);
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
            
        if (dto.EntryFee < TournamentConstants.MIN_ENTRY_FEE || dto.EntryFee > TournamentConstants.MAX_ENTRY_FEE)
            errors.Add($"Phí tham gia phải từ {TournamentConstants.MIN_ENTRY_FEE:N0} đến {TournamentConstants.MAX_ENTRY_FEE:N0} VND!");
        
        return (errors.Count == 0, errors);
    }
}
