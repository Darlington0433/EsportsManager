using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

public class WalletService : IWalletService
{
    private static readonly List<Wallet> _wallets = new();
    private static readonly List<WalletTransaction> _transactions = new();
    private static int _nextWalletId = 1;
    private static int _nextTransactionId = 1;

    // Implement IWalletService methods
    public async Task<BusinessResult<Wallet>> GetWalletByUserIdAsync(int userId)
    {
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId); if (wallet == null)
        {
            return BusinessResult<Wallet>.Failure($"Không tìm thấy ví cho người dùng {userId}");
        }
        return BusinessResult<Wallet>.Success(wallet);
    }

    public async Task<BusinessResult<decimal>> GetBalanceAsync(int userId)
    {
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            return BusinessResult<decimal>.Failure($"Không tìm thấy ví cho người dùng {userId}");
        }
        return BusinessResult<decimal>.Success(wallet.Balance);
    }

    public async Task<BusinessResult<Wallet>> CreateWalletAsync(int userId)
    {
        if (_wallets.Any(w => w.UserId == userId))
        {
            return BusinessResult<Wallet>.Failure("Ví đã tồn tại cho người dùng này");
        }

        var wallet = new Wallet
        {
            WalletId = _nextWalletId++,
            UserId = userId,
            Balance = 0,
            LastUpdated = DateTime.UtcNow
        };

        _wallets.Add(wallet);
        return BusinessResult<Wallet>.Success(wallet);
    }

    public async Task<BusinessResult<Wallet>> UpdateBalanceAsync(int userId, decimal amount)
    {
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            return BusinessResult<Wallet>.Failure($"No wallet found for user {userId}");
        }

        wallet.Balance = amount;
        wallet.LastUpdated = DateTime.UtcNow;

        return BusinessResult<Wallet>.Success(wallet);
    }

    public async Task<BusinessResult<Wallet>> DepositAsync(int userId, decimal amount)
    {
        if (amount <= 0)
        {
            return BusinessResult<Wallet>.Failure("Số tiền phải lớn hơn không");
        }
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            var result = await CreateWalletAsync(userId);
            if (!result.IsSuccess)
            {
                return result;
            }
            wallet = result.Data;
        }

        if (wallet != null)
        {
            wallet.Balance += amount;
            wallet.LastUpdated = DateTime.UtcNow;

            // Add transaction
            _transactions.Add(new WalletTransaction
            {
                TransactionId = _nextTransactionId++,
                WalletId = wallet.WalletId,
                Amount = amount,
                Balance = wallet.Balance,
                Description = "Nạp tiền",
                TransactionDate = DateTime.UtcNow,
                TransactionType = "Deposit"
            });
        }
        return wallet != null
            ? BusinessResult<Wallet>.Success(wallet)
            : BusinessResult<Wallet>.Failure("Không thể tạo hoặc cập nhật ví");
    }
    public async Task<BusinessResult<Wallet>> WithdrawAsync(int userId, decimal amount)
    {
        if (amount <= 0)
        {
            return BusinessResult<Wallet>.Failure("Số tiền phải lớn hơn không");
        }

        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            return BusinessResult<Wallet>.Failure($"Không tìm thấy ví cho người dùng {userId}");
        }

        if (wallet.Balance < amount)
        {
            return BusinessResult<Wallet>.Failure("Số dư không đủ");
        }

        wallet.Balance -= amount;
        wallet.LastUpdated = DateTime.UtcNow;

        // Add transaction
        _transactions.Add(new WalletTransaction
        {
            TransactionId = _nextTransactionId++,
            WalletId = wallet.WalletId,
            Amount = -amount,
            Balance = wallet.Balance,
            Description = "Rút tiền",
            TransactionDate = DateTime.UtcNow,
            TransactionType = "Withdraw"
        });

        return BusinessResult<Wallet>.Success(wallet);
    }

    /// <summary>
    /// Withdraw funds from user wallet with a reason
    /// </summary>
    public async Task<BusinessResult<Wallet>> WithdrawAsync(int userId, decimal amount, string reason)
    {
        // Reuse existing withdraw logic
        var result = await WithdrawAsync(userId, amount);

        // If successful, add the reason to the transaction
        if (result.IsSuccess)
        {
            // Find the last transaction for this user
            var transaction = _transactions
                .Where(t => t.UserId == userId && t.Type == "Withdrawal")
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefault();

            if (transaction != null)
            {
                transaction.Description = reason;
            }
        }

        return result;
    }

    public async Task<BusinessResult<Wallet>> TransferAsync(int fromUserId, int toUserId, decimal amount, string message)
    {
        if (amount <= 0)
        {
            return BusinessResult<Wallet>.Failure("Số tiền phải lớn hơn không");
        }

        if (fromUserId == toUserId)
        {
            return BusinessResult<Wallet>.Failure("Không thể chuyển tiền cho chính mình");
        }

        // Withdraw from source
        var withdrawResult = await WithdrawAsync(fromUserId, amount);
        if (!withdrawResult.IsSuccess)
        {
            return withdrawResult;
        }

        // Deposit to target
        var depositResult = await DepositAsync(toUserId, amount);
        if (!depositResult.IsSuccess)
        {
            // Rollback withdrawal
            await DepositAsync(fromUserId, amount);
            return BusinessResult<Wallet>.Failure($"Chuyển tiền thất bại: {depositResult.ErrorMessage}");
        }        // Add transaction records
        var sourceWallet = withdrawResult.Data;
        var targetWallet = depositResult.Data;        // Transfer out transaction
        if (sourceWallet != null)
        {
            _transactions.Add(new WalletTransaction
            {
                TransactionId = _nextTransactionId++,
                WalletId = sourceWallet.WalletId,
                Amount = -amount,
                Balance = sourceWallet.Balance,
                Description = $"Chuyển tiền đến người dùng {toUserId}: {message}",
                TransactionDate = DateTime.UtcNow,
                TransactionType = "TransferOut",
                Reference = $"TO-{toUserId}"
            });
        }

        // Transfer in transaction
        if (targetWallet != null)
        {
            _transactions.Add(new WalletTransaction
            {
                TransactionId = _nextTransactionId++,
                WalletId = targetWallet.WalletId,
                Amount = amount,
                Balance = targetWallet.Balance,
                Description = $"Nhận tiền từ người dùng {fromUserId}: {message}",
                TransactionDate = DateTime.UtcNow,
                TransactionType = "TransferIn",
                Reference = $"FROM-{fromUserId}"
            });
        }

        return withdrawResult;
    }
    public async Task<BusinessResult<IEnumerable<WalletTransaction>>> GetTransactionHistoryAsync(int userId)
    {
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            return BusinessResult<IEnumerable<WalletTransaction>>.Failure($"Không tìm thấy ví cho người dùng {userId}");
        }

        var transactions = _transactions.Where(t => t.WalletId == wallet.WalletId).ToList();
        return BusinessResult<IEnumerable<WalletTransaction>>.Success(transactions);
    }

    public async Task<BusinessResult<WalletTransaction>> GetTransactionByIdAsync(int transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
        {
            return BusinessResult<WalletTransaction>.Failure($"Transaction {transactionId} not found");
        }

        return BusinessResult<WalletTransaction>.Success(transaction);
    }

    public async Task<BusinessResult<IEnumerable<WalletTransaction>>> GetTransactionsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var wallet = _wallets.FirstOrDefault(w => w.UserId == userId);
        if (wallet == null)
        {
            return BusinessResult<IEnumerable<WalletTransaction>>.Failure($"No wallet found for user {userId}");
        }

        var transactions = _transactions
            .Where(t => t.WalletId == wallet.WalletId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .ToList();

        return BusinessResult<IEnumerable<WalletTransaction>>.Success(transactions);
    }

    // Legacy methods
    public List<Wallet> GetAll() => _wallets.ToList();
    public Wallet? GetByUserId(int userId) => _wallets.FirstOrDefault(w => w.UserId == userId);
    public void Add(Wallet w)
    {
        w.WalletId = _nextWalletId++;
        _wallets.Add(w);
    }
    public void Update(Wallet w)
    {
        var idx = _wallets.FindIndex(x => x.WalletId == w.WalletId);
        if (idx >= 0) _wallets[idx] = w;
    }
    public void Delete(int id)
    {
        var w = _wallets.FirstOrDefault(x => x.WalletId == id);
        if (w != null) _wallets.Remove(w);
    }
}
