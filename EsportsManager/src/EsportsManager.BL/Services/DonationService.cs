using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

public class DonationService : IDonationService
{
    private static readonly List<Donation> _donations = new();
    private static int _nextId = 1;
    private readonly IWalletService? _walletService;

    public DonationService(IWalletService? walletService = null)
    {
        _walletService = walletService;
    }

    // Implement IDonationService methods
    public async Task<BusinessResult<Donation>> MakeDonationAsync(int fromUserId, int toUserId, decimal amount, string message)
    {
        if (fromUserId == toUserId)
        {
            return BusinessResult<Donation>.Failure("Cannot donate to yourself");
        }

        if (amount <= 0)
        {
            return BusinessResult<Donation>.Failure("Amount must be greater than zero");
        }

        // Transfer funds if wallet service is available
        if (_walletService != null)
        {
            var transferResult = await _walletService.TransferAsync(fromUserId, toUserId, amount, message);
            if (!transferResult.IsSuccess)
            {
                return BusinessResult<Donation>.Failure($"Transfer failed: {transferResult.ErrorMessage}");
            }
        }

        // Create donation
        var donation = new Donation
        {
            DonationId = _nextId++,
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Amount = amount,
            Message = message,
            DonationDate = DateTime.UtcNow,
            Status = "Completed"
        };

        _donations.Add(donation);
        return BusinessResult<Donation>.Success(donation);
    }

    public async Task<BusinessResult<Donation>> GetDonationByIdAsync(int donationId)
    {
        var donation = _donations.FirstOrDefault(d => d.DonationId == donationId);
        if (donation == null)
        {
            return BusinessResult<Donation>.Failure($"Donation {donationId} not found");
        }

        return BusinessResult<Donation>.Success(donation);
    }

    public async Task<BusinessResult<IEnumerable<Donation>>> GetDonationsByUserIdAsync(int userId, bool received = false)
    {
        List<Donation> donations;

        if (received)
        {
            donations = _donations.Where(d => d.ToUserId == userId).ToList();
        }
        else
        {
            donations = _donations.Where(d => d.FromUserId == userId).ToList();
        }

        return BusinessResult<IEnumerable<Donation>>.Success(donations);
    }

    public async Task<BusinessResult<IEnumerable<Donation>>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var donations = _donations
            .Where(d => d.DonationDate >= startDate && d.DonationDate <= endDate)
            .ToList();

        return BusinessResult<IEnumerable<Donation>>.Success(donations);
    }

    public async Task<BusinessResult<DonationStatistics>> GetDonationStatisticsAsync(int userId)
    {
        var stats = new DonationStatistics
        {
            TotalDonationsMade = _donations.Count(d => d.FromUserId == userId),
            TotalDonationsReceived = _donations.Count(d => d.ToUserId == userId),
            TotalAmountDonated = _donations.Where(d => d.FromUserId == userId).Sum(d => d.Amount),
            TotalAmountReceived = _donations.Where(d => d.ToUserId == userId).Sum(d => d.Amount)
        };

        // Top recipients from this user
        stats.TopRecipients = _donations
            .Where(d => d.FromUserId == userId)
            .GroupBy(d => d.ToUserId)
            .Select(g => new UserDonationSummary
            {
                UserId = g.Key,
                Username = $"User {g.Key}",
                TotalAmount = g.Sum(d => d.Amount),
                Count = g.Count()
            })
            .OrderByDescending(s => s.TotalAmount)
            .Take(5)
            .ToList();

        // Top donors to this user
        stats.TopDonors = _donations
            .Where(d => d.ToUserId == userId)
            .GroupBy(d => d.FromUserId)
            .Select(g => new UserDonationSummary
            {
                UserId = g.Key,
                Username = $"User {g.Key}",
                TotalAmount = g.Sum(d => d.Amount),
                Count = g.Count()
            })
            .OrderByDescending(s => s.TotalAmount)
            .Take(5)
            .ToList();

        return BusinessResult<DonationStatistics>.Success(stats);
    }

    /// <summary>
    /// Create a new donation
    /// </summary>
    public async Task<BusinessResult<Donation>> CreateAsync(Donation donation)
    {
        if (donation == null)
        {
            return BusinessResult<Donation>.Failure("Donation cannot be null");
        }

        if (donation.Amount <= 0)
        {
            return BusinessResult<Donation>.Failure("Amount must be greater than zero");
        }

        // Transfer funds if wallet service is available and recipient is a user
        if (_walletService != null && donation.RecipientType == "User")
        {
            var transferResult = await _walletService.TransferAsync(
                donation.UserId,
                donation.RecipientId,
                donation.Amount,
                donation.Message);

            if (!transferResult.IsSuccess)
            {
                return BusinessResult<Donation>.Failure($"Transfer failed: {transferResult.ErrorMessage}");
            }
        }

        // Set donation ID
        donation.DonationId = _nextId++;
        donation.DonationDate = DateTime.UtcNow;
        donation.CreatedAt = DateTime.UtcNow;

        // Add to in-memory storage
        _donations.Add(donation);

        return BusinessResult<Donation>.Success(donation);
    }

    // Legacy methods
    public List<Donation> GetAll() => _donations.ToList();
    public Donation? GetById(int id) => _donations.FirstOrDefault(d => d.DonationId == id);
    public void Add(Donation d)
    {
        d.DonationId = _nextId++;
        _donations.Add(d);
    }
    public void Update(Donation d)
    {
        var idx = _donations.FindIndex(x => x.DonationId == d.DonationId);
        if (idx >= 0) _donations[idx] = d;
    }
    public void Delete(int id)
    {
        var d = GetById(id);
        if (d != null) _donations.Remove(d);
    }
}
