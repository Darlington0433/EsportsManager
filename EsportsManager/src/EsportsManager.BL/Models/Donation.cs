using System;

namespace EsportsManager.BL.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public int UserId { get; set; }  // User who made the donation
        public int FromUserId { get; set; }  // Original property - kept for compatibility
        public int ToUserId { get; set; }  // Original property - kept for compatibility
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Completed";

        // Added properties needed by the UI
        public string RecipientType { get; set; } = "User";  // User, Team, Tournament
        public int RecipientId { get; set; }  // UserId, TeamId, or TournamentId
    }
}
