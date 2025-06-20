using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using System.Collections.Generic;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Interface cho Wallet Service
    /// </summary>
    public interface IWalletService
    {
        // Wallet management
        Task<BusinessResult<Wallet>> GetWalletByUserIdAsync(int userId);
        Task<BusinessResult<decimal>> GetBalanceAsync(int userId);
        Task<BusinessResult<Wallet>> CreateWalletAsync(int userId);
        Task<BusinessResult<Wallet>> UpdateBalanceAsync(int userId, decimal amount);        // Transaction operations
        Task<BusinessResult<Wallet>> DepositAsync(int userId, decimal amount);
        Task<BusinessResult<Wallet>> WithdrawAsync(int userId, decimal amount);
        Task<BusinessResult<Wallet>> WithdrawAsync(int userId, decimal amount, string reason);
        Task<BusinessResult<Wallet>> TransferAsync(int fromUserId, int toUserId, decimal amount, string message);

        // History operations
        Task<BusinessResult<IEnumerable<WalletTransaction>>> GetTransactionHistoryAsync(int userId);
        Task<BusinessResult<WalletTransaction>> GetTransactionByIdAsync(int transactionId);
        Task<BusinessResult<IEnumerable<WalletTransaction>>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    }
}
