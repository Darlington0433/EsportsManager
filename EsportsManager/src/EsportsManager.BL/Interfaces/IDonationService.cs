using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using System.Collections.Generic;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Interface cho Donation Service
    /// </summary>
    public interface IDonationService
    {
        // Donation operations
        Task<BusinessResult<Donation>> MakeDonationAsync(int fromUserId, int toUserId, decimal amount, string message);
        Task<BusinessResult<Donation>> GetDonationByIdAsync(int donationId);
        Task<BusinessResult<IEnumerable<Donation>>> GetDonationsByUserIdAsync(int userId, bool received = false);
        Task<BusinessResult<IEnumerable<Donation>>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<BusinessResult<DonationStatistics>> GetDonationStatisticsAsync(int userId);

        // Additional methods needed by UI
        Task<BusinessResult<Donation>> CreateAsync(Donation donation);
    }

    /// <summary>
    /// Statistics for donations
    /// </summary>
    public class DonationStatistics
    {
        public int TotalDonationsMade { get; set; }
        public int TotalDonationsReceived { get; set; }
        public decimal TotalAmountDonated { get; set; }
        public decimal TotalAmountReceived { get; set; }
        public List<UserDonationSummary> TopDonors { get; set; } = new();
        public List<UserDonationSummary> TopRecipients { get; set; } = new();
    }

    /// <summary>
    /// Summary for user donations
    /// </summary>
    public class UserDonationSummary
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
    }
}
