using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface for Wallet Repository
    /// </summary>
    public interface IWalletRepository : IRepository<Models.Wallet, int>
    {
        Task<Models.Wallet?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Models.WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId);
        Task<Models.WalletTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<IEnumerable<Models.WalletTransaction>> GetTransactionsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate);
        Task<Models.WalletTransaction> AddTransactionAsync(Models.WalletTransaction transaction);
    }
}
