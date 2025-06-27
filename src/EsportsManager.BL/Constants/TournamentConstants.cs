namespace EsportsManager.BL.Constants;

/// <summary>
/// Business constants for tournament operations
/// </summary>
public static class TournamentConstants
{
    // Default values
    public const int DEFAULT_MAX_TEAMS = 16;
    public const decimal DEFAULT_ENTRY_FEE = 0;
    public const int DEFAULT_REGISTRATION_DAYS = 5;
    public const int DEFAULT_START_DAYS = 7;
    public const int DEFAULT_END_DAYS = 14;
    
    // Validation constraints
    public const int MIN_GAME_ID = 1;
    public const int MIN_MAX_TEAMS = 4;
    public const int MAX_MAX_TEAMS = 64;
    public const decimal MIN_ENTRY_FEE = 0;
    public const decimal MAX_ENTRY_FEE = 10000000; // 10M VND
    
    // UI Display lengths
    public const int MAX_TOURNAMENT_NAME_DISPLAY = 28;
    public const int MAX_TOURNAMENT_NAME_SHORT = 19;
    
    // Game types
    public static readonly Dictionary<int, string> GAME_TYPES = new()
    {
        { 1, "League of Legends" },
        { 2, "Counter-Strike 2" },
        { 3, "Valorant" }
    };
    
    // Tournament status values
    public static readonly HashSet<string> ACTIVE_STATUSES = new() { "Ongoing", "Registration" };
    public static readonly HashSet<string> COMPLETED_STATUSES = new() { "Completed" };
}
