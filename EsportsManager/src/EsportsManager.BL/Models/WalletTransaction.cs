using System;

namespace EsportsManager.BL.Models
{
    /// <summary>
    /// Wallet Transaction model
    /// </summary>
    public class WalletTransaction
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Wallet ID
        /// </summary>
        public int WalletId { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Transaction type (Deposit, Withdraw, Transfer, etc.)
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;

        /// <summary>
        /// Type property (alias for TransactionType for backward compatibility)
        /// </summary>
        public string Type { get => TransactionType; set => TransactionType = value; }

        /// <summary>
        /// Transaction amount
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
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Reference (optional)
        /// </summary>
        public string? Reference { get; set; }
    }
}
