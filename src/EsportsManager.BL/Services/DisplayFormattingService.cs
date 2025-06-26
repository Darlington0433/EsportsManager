using EsportsManager.BL.Constants;

namespace EsportsManager.BL.Services;

/// <summary>
/// Display formatting service for business logic
/// </summary>
public class DisplayFormattingService
{
    /// <summary>
    /// Formats username for display with length constraints
    /// </summary>
    public static string FormatUsernameForDisplay(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return string.Empty;
            
        return username.Length > UserConstants.MAX_USERNAME_DISPLAY 
            ? username.Substring(0, UserConstants.MAX_USERNAME_DISPLAY)
            : username;
    }
    
    /// <summary>
    /// Formats email for display with length constraints
    /// </summary>
    public static string FormatEmailForDisplay(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return UserConstants.DEFAULT_EMAIL_DISPLAY;
            
        return email.Length > UserConstants.MAX_EMAIL_DISPLAY 
            ? email.Substring(0, UserConstants.MAX_EMAIL_DISPLAY)
            : email;
    }
    
    /// <summary>
    /// Formats team name for display with length constraints
    /// </summary>
    public static string FormatTeamNameForDisplay(string? teamName)
    {
        if (string.IsNullOrEmpty(teamName))
            return string.Empty;
            
        return teamName.Length > UserConstants.MAX_TEAM_NAME_DISPLAY 
            ? teamName.Substring(0, UserConstants.MAX_TEAM_NAME_DISPLAY)
            : teamName;
    }
    
    /// <summary>
    /// Formats team description for display with length constraints
    /// </summary>
    public static string FormatTeamDescriptionForDisplay(string? description)
    {
        if (string.IsNullOrEmpty(description))
            return string.Empty;
            
        return description.Length > UserConstants.MAX_TEAM_DESCRIPTION_DISPLAY 
            ? description.Substring(0, UserConstants.MAX_TEAM_DESCRIPTION_DISPLAY)
            : description;
    }
    
    /// <summary>
    /// Formats date for display with null safety
    /// </summary>
    public static string FormatDateForDisplay(DateTime? date, string format = "dd/MM/yyyy")
    {
        return date?.ToString(format) ?? UserConstants.DEFAULT_DATE_DISPLAY;
    }
    
    /// <summary>
    /// Formats date and time for display with null safety
    /// </summary>
    public static string FormatDateTimeForDisplay(DateTime? dateTime, string format = "dd/MM/yyyy HH:mm")
    {
        return dateTime?.ToString(format) ?? UserConstants.DEFAULT_DATE_DISPLAY;
    }
}
