using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Wallet Repository implementation
    /// </summary>
    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<WalletRepository> _logger;

        // In-memory storage for demo
        private static readonly List<Wallet> _wallets = new();
        private static readonly List<WalletTransaction> _transactions = new();
        private static int _nextWalletId = 1;
        private static int _nextTransactionId = 1;

        public WalletRepository(DataContext context, ILogger<WalletRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get wallet by id
        /// </summary>
        public async Task<Wallet?> GetByIdAsync(int id)
        {
            // In-memory implementation
            return await Task.FromResult(_wallets.FirstOrDefault(w => w.Id == id));
        }

        /// <summary>
        /// Get wallet by user id
        /// </summary>
        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            // In-memory implementation
            return await Task.FromResult(_wallets.FirstOrDefault(w => w.UserId == userId));
        }

        /// <summary>
        /// Get all wallets
        /// </summary>
        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            // In-memory implementation
            return await Task.FromResult(_wallets);
        }

        /// <summary>
        /// Add a new wallet
        /// </summary>
        public async Task<Wallet> AddAsync(Wallet entity)
        {
            // Set id for new wallet
            entity.Id = _nextWalletId++;

            // Add to in-memory storage
            _wallets.Add(entity);

            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Update wallet
        /// </summary>
        public async Task<Wallet> UpdateAsync(Wallet entity)
        {
            // Find the wallet to update
            var index = _wallets.FindIndex(w => w.Id == entity.Id);

            if (index != -1)
            {
                // Update wallet
                _wallets[index] = entity;
                return await Task.FromResult(entity);
            }

            // Wallet not found
            throw new KeyNotFoundException($"Wallet with ID {entity.Id} not found");
        }

        /// <summary>
        /// Delete wallet
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            // Find the wallet to delete
            var index = _wallets.FindIndex(w => w.Id == id);

            if (index != -1)
            {
                // Delete wallet
                _wallets.RemoveAt(index);
                return await Task.FromResult(true);
            }

            // Wallet not found
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Check if wallet exists
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            // Check if wallet exists
            return await Task.FromResult(_wallets.Any(w => w.Id == id));
        }

        /// <summary>
        /// Get total wallets count
        /// </summary>
        public async Task<int> CountAsync()
        {
            // Get total wallets count
            return await Task.FromResult(_wallets.Count);
        }

        /// <summary>
        /// Get transactions by wallet id
        /// </summary>
        public async Task<IEnumerable<WalletTransaction>> GetTransactionsByWalletIdAsync(int walletId)
        {
            // Get transactions for wallet
            return await Task.FromResult(_transactions.Where(t => t.WalletId == walletId).ToList());
        }

        /// <summary>
        /// Get transaction by id
        /// </summary>
        public async Task<WalletTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            // Get transaction by id
            return await Task.FromResult(_transactions.FirstOrDefault(t => t.Id == transactionId));
        }

        /// <summary>
        /// Get transactions by date range
        /// </summary>
        public async Task<IEnumerable<WalletTransaction>> GetTransactionsByDateRangeAsync(int walletId, DateTime startDate, DateTime endDate)
        {
            // Get transactions for wallet within date range
            return await Task.FromResult(
                _transactions.Where(t =>
                    t.WalletId == walletId &&
                    t.TransactionDate >= startDate &&
                    t.TransactionDate <= endDate)
                .ToList());
        }

        /// <summary>
        /// Add transaction
        /// </summary>
        public async Task<WalletTransaction> AddTransactionAsync(WalletTransaction transaction)
        {
            // Set id for new transaction
            transaction.Id = _nextTransactionId++;

            // Add to in-memory storage
            _transactions.Add(transaction);

            return await Task.FromResult(transaction);
        }
    }
}
