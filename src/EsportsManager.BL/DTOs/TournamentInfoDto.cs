using System;

namespace EsportsManager.BL.DTOs
{
    public class TournamentInfoDto
    {
        public int TournamentId { get; set; }
        public required string TournamentName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public int MaxTeams { get; set; }
        public decimal EntryFee { get; set; }
        public decimal PrizePool { get; set; }
        public string Status { get; set; } = "Draft";
        public int RegisteredTeams { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // Backward compatibility properties
        public string Name => TournamentName;
        public int Id => TournamentId;
        public int CurrentParticipants => RegisteredTeams;
        public int MaxParticipants => MaxTeams;
        public string Location { get; set; } = string.Empty;
        public string Rules { get; set; } = string.Empty;
    }
}
