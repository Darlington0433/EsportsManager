using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Repositories
{
    /// <summary>
    /// Donation Repository implementation
    /// </summary>
    public class DonationRepository : IDonationRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<DonationRepository> _logger;

        // In-memory storage for demo
        private static readonly List<Donation> _donations = new();
        private static int _nextDonationId = 1;

        public DonationRepository(DataContext context, ILogger<DonationRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get donation by id
        /// </summary>
        public async Task<Donation?> GetByIdAsync(int id)
        {
            // In-memory implementation
            return await Task.FromResult(_donations.FirstOrDefault(d => d.Id == id));
        }

        /// <summary>
        /// Get all donations
        /// </summary>
        public async Task<IEnumerable<Donation>> GetAllAsync()
        {
            // In-memory implementation
            return await Task.FromResult(_donations);
        }

        /// <summary>
        /// Add a new donation
        /// </summary>
        public async Task<Donation> AddAsync(Donation entity)
        {
            // Set id for new donation
            entity.Id = _nextDonationId++;

            // Add to in-memory storage
            _donations.Add(entity);

            return await Task.FromResult(entity);
        }

        /// <summary>
        /// Update donation
        /// </summary>
        public async Task<Donation> UpdateAsync(Donation entity)
        {
            // Find the donation to update
            var index = _donations.FindIndex(d => d.Id == entity.Id);

            if (index != -1)
            {
                // Update donation
                _donations[index] = entity;
                return await Task.FromResult(entity);
            }

            // Donation not found
            throw new KeyNotFoundException($"Donation with ID {entity.Id} not found");
        }

        /// <summary>
        /// Delete donation
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            // Find the donation to delete
            var index = _donations.FindIndex(d => d.Id == id);

            if (index != -1)
            {
                // Delete donation
                _donations.RemoveAt(index);
                return await Task.FromResult(true);
            }

            // Donation not found
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Check if donation exists
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            // Check if donation exists
            return await Task.FromResult(_donations.Any(d => d.Id == id));
        }

        /// <summary>
        /// Get total donations count
        /// </summary>
        public async Task<int> CountAsync()
        {
            // Get total donations count
            return await Task.FromResult(_donations.Count);
        }

        /// <summary>
        /// Get donations by from user id
        /// </summary>
        public async Task<IEnumerable<Donation>> GetByFromUserIdAsync(int fromUserId)
        {
            // Get donations from user
            return await Task.FromResult(_donations.Where(d => d.FromUserId == fromUserId).ToList());
        }

        /// <summary>
        /// Get donations by to user id
        /// </summary>
        public async Task<IEnumerable<Donation>> GetByToUserIdAsync(int toUserId)
        {
            // Get donations to user
            return await Task.FromResult(_donations.Where(d => d.ToUserId == toUserId).ToList());
        }

        /// <summary>
        /// Get donations by date range
        /// </summary>
        public async Task<IEnumerable<Donation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Get donations within date range
            return await Task.FromResult(
                _donations.Where(d =>
                    d.DonationDate >= startDate &&
                    d.DonationDate <= endDate)
                .ToList());
        }

        /// <summary>
        /// Get total donated by user
        /// </summary>
        public async Task<decimal> GetTotalDonatedByUserAsync(int userId)
        {
            // Get total donated by user
            return await Task.FromResult(
                _donations.Where(d => d.FromUserId == userId)
                .Sum(d => d.Amount));
        }

        /// <summary>
        /// Get total received by user
        /// </summary>
        public async Task<decimal> GetTotalReceivedByUserAsync(int userId)
        {
            // Get total received by user
            return await Task.FromResult(
                _donations.Where(d => d.ToUserId == userId)
                .Sum(d => d.Amount));
        }
    }
}
