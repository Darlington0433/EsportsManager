using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho báo cáo donation
    /// </summary>
    public class DonationReportDto
    {
        public decimal TotalDonations { get; set; }
        public decimal DonationsThisMonth { get; set; }
        public List<string> TopDonors { get; set; } = new List<string>();
        public List<string> TopRecipients { get; set; } = new List<string>();
        public decimal AverageDonationAmount { get; set; }
    }

    /// <summary>
    /// DTO cho kết quả voting
    /// </summary>
    public class VotingResultsDto
    {
        public int TotalVotes { get; set; }
        public int VotesThisMonth { get; set; }
        public List<string> TopVotedPlayers { get; set; } = new List<string>();
        public List<string> TopVotedTeams { get; set; } = new List<string>();
        public int ActivePolls { get; set; }
    }
}
