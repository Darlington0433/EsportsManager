using System;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Donation entity
    /// </summary>
    public class Donation
    {
        /// <summary>
        /// Donation ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID who made the donation
        /// </summary>
        public int FromUserId { get; set; }

        /// <summary>
        /// User ID who received the donation
        /// </summary>
        public int ToUserId { get; set; }

        /// <summary>
        /// Donation amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Donation message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Donation date
        /// </summary>
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Donation status
        /// </summary>
        public string Status { get; set; } = "Completed";
    }
}
