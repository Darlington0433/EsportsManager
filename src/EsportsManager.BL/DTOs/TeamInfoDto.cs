using System;

namespace EsportsManager.BL.DTOs
{
    public class TeamInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public int MemberCount { get; set; }
        public int LeaderId { get; set; }
        public string LeaderName { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public int Ranking { get; set; }
        public int Points { get; set; }
        public string Status { get; set; } = "Active";
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int MaxMembers { get; set; } = 10;
        public DateTime CreatedAt { get; set; }
        public string? Achievements { get; set; }
        public int? CurrentTournamentId { get; set; }
    }
}
