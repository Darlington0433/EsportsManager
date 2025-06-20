using System;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Wallet entity
    /// </summary>
    public class Wallet
    {
        /// <summary>
        /// Wallet ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Wallet balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Last update time
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Wallet transaction entity
    /// </summary>
    public class WalletTransaction
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Wallet ID
        /// </summary>
        public int WalletId { get; set; }

        /// <summary>
        /// Transaction type (Deposit, Withdraw, Transfer, etc.)
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;

        /// <summary>
        /// Transaction amount (positive for deposit, negative for withdraw)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Wallet balance after transaction
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Transaction date
        /// </summary>
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Reference (optional - for linking to other entities)
        /// </summary>
        public string? Reference { get; set; }
    }
}
