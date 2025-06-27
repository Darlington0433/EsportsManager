namespace EsportsManager.BL.Constants;

/// <summary>
/// Business constants for user and display operations
/// </summary>
public static class UserConstants
{
    // Display length constraints
    public const int MAX_USERNAME_DISPLAY = 14;
    public const int MAX_EMAIL_DISPLAY = 24;
    public const int MAX_TEAM_NAME_DISPLAY = 24;
    public const int MAX_TEAM_DESCRIPTION_DISPLAY = 29;
    
    // Validation constants
    public const int MIN_USERNAME_LENGTH = 3;
    public const int MAX_USERNAME_LENGTH = 50;
    public const int MIN_PASSWORD_LENGTH = 6;
    public const int MAX_EMAIL_LENGTH = 100;
    
    // User status values
    public static readonly HashSet<string> VALID_USER_STATUSES = new() { "Active", "Inactive", "Pending", "Rejected" };
    public static readonly HashSet<string> VALID_USER_ROLES = new() { "Player", "Viewer", "Admin" };
    
    // Default values
    public const string DEFAULT_EMAIL_DISPLAY = "N/A";
    public const string DEFAULT_DATE_DISPLAY = "N/A";
    
    // Confirmation texts
    public const string CONFIRM_YES = "y";
    public const string CONFIRM_YES_FULL = "yes";
    public const string CONFIRM_DELETE = "YES";
}
