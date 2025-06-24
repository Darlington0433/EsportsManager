using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

/// <summary>
/// Wallet Service Implementation - Mock data for development
/// Production: Replace with real database operations
/// </summary>
public class WalletService : IWalletService
{
    private readonly ILogger<WalletService> _logger;
    private static readonly Dictionary<int, WalletInfoDto> _mockWallets = new();
    private static readonly List<TransactionDto> _mockTransactions = new();
    private static int _nextWalletId = 1;
    private static int _nextTransactionId = 1;
    private static readonly Random _random = new();

    public WalletService(ILogger<WalletService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeMockData();
    }

    /// <summary>
    /// Lấy thông tin ví của người dùng
    /// </summary>
    public async Task<WalletInfoDto?> GetWalletByUserIdAsync(int userId)
    {
        await Task.Delay(30); // Simulate db call

        if (_mockWallets.TryGetValue(userId, out var wallet))
        {
            return wallet;
        }

        // Auto create wallet if not exists
        var newWallet = new WalletInfoDto
        {
            Id = _nextWalletId++,
            UserId = userId,
            Username = $"user_{userId}",
            Balance = 0,
            CreatedAt = DateTime.Now,
            Status = "Active",
            IsLocked = false
        };

        _mockWallets[userId] = newWallet;
        _logger.LogInformation("Created new wallet for user {UserId}", userId);

        return newWallet;
    }

    /// <summary>
    /// Nạp tiền vào ví
    /// </summary>
    public async Task<TransactionResultDto> DepositAsync(int userId, DepositDto depositDto)
    {
        if (depositDto == null)
            throw new ArgumentNullException(nameof(depositDto));

        if (depositDto.Amount <= 0)
            return new TransactionResultDto
            {
                Success = false,
                Message = "Số tiền không hợp lệ",
                Errors = new List<string> { "Số tiền phải lớn hơn 0" }
            };

        try
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            if (wallet == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví không tồn tại",
                    Errors = new List<string> { "Không tìm thấy ví của người dùng" }
                };

            if (wallet.IsLocked)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví đang bị khóa",
                    Errors = new List<string> { "Không thể thực hiện giao dịch khi ví đang bị khóa" }
                };

            // Process deposit
            decimal newBalance = wallet.Balance + depositDto.Amount;
            wallet.Balance = newBalance;
            wallet.LastUpdatedAt = DateTime.Now;

            // Create transaction record
            var transaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                UserId = userId,
                Username = wallet.Username,
                TransactionType = "Deposit",
                Amount = depositDto.Amount,
                BalanceAfter = newBalance,
                Status = "Completed",
                CreatedAt = DateTime.Now,
                ReferenceCode = depositDto.ReferenceCode ?? GenerateReferenceCode(),
                Note = depositDto.Note
            };

            _mockTransactions.Add(transaction);

            await Task.Delay(50); // Simulate db operation
            _logger.LogInformation("Deposit successful for user {UserId}, Amount: {Amount}", userId, depositDto.Amount);

            return new TransactionResultDto
            {
                Success = true,
                Message = "Nạp tiền thành công",
                Transaction = transaction,
                NewBalance = newBalance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing deposit for user {UserId}", userId);
            return new TransactionResultDto
            {
                Success = false,
                Message = "Lỗi khi xử lý giao dịch",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Rút tiền từ ví
    /// </summary>
    public async Task<TransactionResultDto> WithdrawAsync(int userId, WithdrawalDto withdrawalDto)
    {
        if (withdrawalDto == null)
            throw new ArgumentNullException(nameof(withdrawalDto));

        if (withdrawalDto.Amount <= 0)
            return new TransactionResultDto
            {
                Success = false,
                Message = "Số tiền không hợp lệ",
                Errors = new List<string> { "Số tiền phải lớn hơn 0" }
            };

        try
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            if (wallet == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví không tồn tại",
                    Errors = new List<string> { "Không tìm thấy ví của người dùng" }
                };

            if (wallet.IsLocked)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví đang bị khóa",
                    Errors = new List<string> { "Không thể thực hiện giao dịch khi ví đang bị khóa" }
                };

            // Check balance
            if (wallet.Balance < withdrawalDto.Amount)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Số dư không đủ",
                    Errors = new List<string> { "Số dư trong ví không đủ để thực hiện giao dịch này" },
                    NewBalance = wallet.Balance
                };

            // Process withdrawal
            decimal newBalance = wallet.Balance - withdrawalDto.Amount;
            wallet.Balance = newBalance;
            wallet.LastUpdatedAt = DateTime.Now;

            // Create transaction record
            var transaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                UserId = userId,
                Username = wallet.Username,
                TransactionType = "Withdrawal",
                Amount = -withdrawalDto.Amount, // Negative for withdrawal
                BalanceAfter = newBalance,
                Status = "Completed",
                CreatedAt = DateTime.Now,
                ReferenceCode = GenerateReferenceCode(),
                Note = $"Rút tiền đến {withdrawalDto.BankName} - {withdrawalDto.AccountName} - {withdrawalDto.BankAccount}" +
                       (string.IsNullOrEmpty(withdrawalDto.Note) ? "" : $" - {withdrawalDto.Note}")
            };

            _mockTransactions.Add(transaction);

            await Task.Delay(50); // Simulate db operation
            _logger.LogInformation("Withdrawal successful for user {UserId}, Amount: {Amount}", userId, withdrawalDto.Amount);

            return new TransactionResultDto
            {
                Success = true,
                Message = "Rút tiền thành công",
                Transaction = transaction,
                NewBalance = newBalance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing withdrawal for user {UserId}", userId);
            return new TransactionResultDto
            {
                Success = false,
                Message = "Lỗi khi xử lý giao dịch",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Chuyển tiền cho người dùng khác
    /// </summary>
    public async Task<TransactionResultDto> TransferAsync(int fromUserId, TransferDto transferDto)
    {
        if (transferDto == null)
            throw new ArgumentNullException(nameof(transferDto));

        if (transferDto.Amount <= 0)
            return new TransactionResultDto
            {
                Success = false,
                Message = "Số tiền không hợp lệ",
                Errors = new List<string> { "Số tiền phải lớn hơn 0" }
            };

        if (fromUserId == transferDto.ToUserId)
            return new TransactionResultDto
            {
                Success = false,
                Message = "Không thể chuyển tiền cho chính mình",
                Errors = new List<string> { "Người nhận không thể là chính bạn" }
            };

        try
        {
            // Get sender's wallet
            var senderWallet = await GetWalletByUserIdAsync(fromUserId);
            if (senderWallet == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví người gửi không tồn tại",
                    Errors = new List<string> { "Không tìm thấy ví của bạn" }
                };

            if (senderWallet.IsLocked)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví người gửi đang bị khóa",
                    Errors = new List<string> { "Không thể thực hiện giao dịch khi ví đang bị khóa" }
                };

            // Check sender's balance
            if (senderWallet.Balance < transferDto.Amount)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Số dư không đủ",
                    Errors = new List<string> { "Số dư trong ví không đủ để thực hiện giao dịch này" },
                    NewBalance = senderWallet.Balance
                };

            // Get or create receiver's wallet
            var receiverWallet = await GetWalletByUserIdAsync(transferDto.ToUserId);
            if (receiverWallet == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví người nhận không tồn tại",
                    Errors = new List<string> { "Không tìm thấy ví của người nhận" }
                };

            if (receiverWallet.IsLocked)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví người nhận đang bị khóa",
                    Errors = new List<string> { "Không thể chuyển tiền đến ví đang bị khóa" }
                };

            // Process transfer
            decimal senderNewBalance = senderWallet.Balance - transferDto.Amount;
            decimal receiverNewBalance = receiverWallet.Balance + transferDto.Amount;

            senderWallet.Balance = senderNewBalance;
            senderWallet.LastUpdatedAt = DateTime.Now;

            receiverWallet.Balance = receiverNewBalance;
            receiverWallet.LastUpdatedAt = DateTime.Now;

            // Create transaction records
            var senderTransaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                UserId = fromUserId,
                Username = senderWallet.Username,
                TransactionType = "TransferOut",
                Amount = -transferDto.Amount, // Negative for sender
                BalanceAfter = senderNewBalance,
                Status = "Completed",
                CreatedAt = DateTime.Now,
                ReferenceCode = GenerateReferenceCode(),
                Note = $"Chuyển tiền đến {transferDto.ToUsername}" +
                       (string.IsNullOrEmpty(transferDto.Note) ? "" : $" - {transferDto.Note}"),
                RelatedUserId = transferDto.ToUserId,
                RelatedUsername = transferDto.ToUsername
            };

            var receiverTransaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                UserId = transferDto.ToUserId,
                Username = transferDto.ToUsername,
                TransactionType = "TransferIn",
                Amount = transferDto.Amount, // Positive for receiver
                BalanceAfter = receiverNewBalance,
                Status = "Completed",
                CreatedAt = DateTime.Now,
                ReferenceCode = senderTransaction.ReferenceCode,
                Note = $"Nhận tiền từ {senderWallet.Username}" +
                       (string.IsNullOrEmpty(transferDto.Note) ? "" : $" - {transferDto.Note}"),
                RelatedUserId = fromUserId,
                RelatedUsername = senderWallet.Username
            };

            _mockTransactions.Add(senderTransaction);
            _mockTransactions.Add(receiverTransaction);

            await Task.Delay(50); // Simulate db operation
            _logger.LogInformation("Transfer successful from user {FromUserId} to user {ToUserId}, Amount: {Amount}",
                fromUserId, transferDto.ToUserId, transferDto.Amount);

            return new TransactionResultDto
            {
                Success = true,
                Message = "Chuyển tiền thành công",
                Transaction = senderTransaction,
                NewBalance = senderNewBalance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transfer from user {FromUserId} to user {ToUserId}",
                fromUserId, transferDto.ToUserId);
            return new TransactionResultDto
            {
                Success = false,
                Message = "Lỗi khi xử lý giao dịch",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Donate cho team hoặc giải đấu
    /// </summary>
    public async Task<TransactionResultDto> DonateAsync(int userId, DonationDto donationDto)
    {
        if (donationDto == null)
            throw new ArgumentNullException(nameof(donationDto));

        if (donationDto.Amount <= 0)
            return new TransactionResultDto
            {
                Success = false,
                Message = "Số tiền không hợp lệ",
                Errors = new List<string> { "Số tiền phải lớn hơn 0" }
            };

        try
        {
            // Get donor's wallet
            var wallet = await GetWalletByUserIdAsync(userId);
            if (wallet == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví không tồn tại",
                    Errors = new List<string> { "Không tìm thấy ví của người dùng" }
                };

            if (wallet.IsLocked)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Ví đang bị khóa",
                    Errors = new List<string> { "Không thể thực hiện giao dịch khi ví đang bị khóa" }
                };

            // Check balance
            if (wallet.Balance < donationDto.Amount)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Số dư không đủ",
                    Errors = new List<string> { "Số dư trong ví không đủ để thực hiện giao dịch này" },
                    NewBalance = wallet.Balance
                };

            // Determine donation target
            string targetType = donationDto.DonationType;
            int? targetId = donationDto.DonationType == "Tournament" ? donationDto.TournamentId : donationDto.TeamId;
            string targetName = donationDto.DonationType == "Tournament"
                ? $"Giải đấu #{donationDto.TournamentId}"
                : $"Team #{donationDto.TeamId}";

            if (targetId == null)
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Đối tượng donate không hợp lệ",
                    Errors = new List<string> { "Vui lòng chọn giải đấu hoặc team để donate" }
                };

            // Process donation
            decimal newBalance = wallet.Balance - donationDto.Amount;
            wallet.Balance = newBalance;
            wallet.LastUpdatedAt = DateTime.Now;

            // Create transaction record
            var transaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                UserId = userId,
                Username = wallet.Username,
                TransactionType = "Donation",
                Amount = -donationDto.Amount, // Negative for donor
                BalanceAfter = newBalance,
                Status = "Completed",
                CreatedAt = DateTime.Now,
                ReferenceCode = GenerateReferenceCode(),
                Note = $"Donate cho {targetName}: {donationDto.Message}",
                RelatedEntityId = targetId,
                RelatedEntityType = targetType
            };

            _mockTransactions.Add(transaction);

            await Task.Delay(50); // Simulate db operation
            _logger.LogInformation("Donation successful from user {UserId} to {TargetType} {TargetId}, Amount: {Amount}",
                userId, targetType, targetId, donationDto.Amount);

            return new TransactionResultDto
            {
                Success = true,
                Message = "Donate thành công",
                Transaction = transaction,
                NewBalance = newBalance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing donation for user {UserId}", userId);
            return new TransactionResultDto
            {
                Success = false,
                Message = "Lỗi khi xử lý giao dịch",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Lấy lịch sử giao dịch của người dùng
    /// </summary>
    public async Task<List<TransactionDto>> GetTransactionHistoryAsync(
        int userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? transactionType = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        await Task.Delay(40); // Simulate db query

        var query = _mockTransactions.Where(t => t.UserId == userId);

        if (fromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.CreatedAt <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(transactionType))
            query = query.Where(t => t.TransactionType.Equals(transactionType, StringComparison.OrdinalIgnoreCase));

        return query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Lấy số dư của người dùng
    /// </summary>
    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var wallet = await GetWalletByUserIdAsync(userId);
        return wallet?.Balance ?? 0;
    }

    /// <summary>
    /// Kiểm tra xem người dùng có đủ số dư không
    /// </summary>
    public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
    {
        decimal balance = await GetBalanceAsync(userId);
        return balance >= amount;
    }

    /// <summary>
    /// Lấy thống kê giao dịch
    /// </summary>
    public async Task<WalletStatsDto> GetWalletStatsAsync(int userId)
    {
        await Task.Delay(50); // Simulate complex calculation

        var userTransactions = _mockTransactions.Where(t => t.UserId == userId).ToList();

        if (!userTransactions.Any())
        {
            return new WalletStatsDto
            {
                TotalTransactions = 0,
                TotalIncome = 0,
                TotalExpense = 0,
                CurrentBalance = await GetBalanceAsync(userId),
                MonthlyStats = new List<MonthlyTransactionDto>(),
                RecentTransactions = new List<TransactionDto>()
            };
        }

        var incomeTransactions = userTransactions.Where(t => t.Amount > 0).ToList();
        var expenseTransactions = userTransactions.Where(t => t.Amount < 0).ToList();

        var incomeBySource = incomeTransactions
            .GroupBy(t => t.TransactionType)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        var expenseByCategory = expenseTransactions
            .GroupBy(t => t.TransactionType)
            .ToDictionary(g => g.Key, g => Math.Abs(g.Sum(t => t.Amount)));

        // Calculate monthly stats for the last 6 months
        var monthlyStats = new List<MonthlyTransactionDto>();
        var today = DateTime.Today;

        for (int i = 0; i < 6; i++)
        {
            var date = today.AddMonths(-i);
            var startOfMonth = new DateTime(date.Year, date.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthlyTransactions = userTransactions.Where(t =>
                t.CreatedAt >= startOfMonth && t.CreatedAt <= endOfMonth).ToList();

            monthlyStats.Add(new MonthlyTransactionDto
            {
                Year = date.Year,
                Month = date.Month,
                TotalIncome = monthlyTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount),
                TotalExpense = Math.Abs(monthlyTransactions.Where(t => t.Amount < 0).Sum(t => t.Amount)),
                TransactionCount = monthlyTransactions.Count
            });
        }

        var stats = new WalletStatsDto
        {
            TotalTransactions = userTransactions.Count,
            TotalIncome = incomeTransactions.Sum(t => t.Amount),
            TotalExpense = Math.Abs(expenseTransactions.Sum(t => t.Amount)),
            CurrentBalance = await GetBalanceAsync(userId),
            IncomeBySource = incomeBySource,
            ExpenseByCategory = expenseByCategory,
            MonthlyStats = monthlyStats,
            RecentTransactions = userTransactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToList()
        };

        return stats;
    }

    // Helper methods
    private string GenerateReferenceCode()
    {
        return $"TRX{DateTime.Now:yyyyMMdd}{_nextTransactionId:D6}";
    }

    private static void InitializeMockData()
    {
        if (_mockWallets.Any())
            return;

        // Create sample wallets
        _mockWallets.Add(1, new WalletInfoDto
        {
            Id = _nextWalletId++,
            UserId = 1,
            Username = "admin",
            Balance = 5000000,
            CreatedAt = DateTime.Now.AddMonths(-6),
            LastUpdatedAt = DateTime.Now.AddDays(-2),
            Status = "Active",
            IsLocked = false
        });

        _mockWallets.Add(2, new WalletInfoDto
        {
            Id = _nextWalletId++,
            UserId = 2,
            Username = "pro_gamer_vn",
            Balance = 3500000,
            CreatedAt = DateTime.Now.AddMonths(-4),
            LastUpdatedAt = DateTime.Now.AddDays(-5),
            Status = "Active",
            IsLocked = false
        });

        _mockWallets.Add(3, new WalletInfoDto
        {
            Id = _nextWalletId++,
            UserId = 3,
            Username = "esports_fan",
            Balance = 1200000,
            CreatedAt = DateTime.Now.AddMonths(-3),
            LastUpdatedAt = DateTime.Now.AddDays(-1),
            Status = "Active",
            IsLocked = false
        });

        _mockWallets.Add(4, new WalletInfoDto
        {
            Id = _nextWalletId++,
            UserId = 4,
            Username = "mobile_legends_pro",
            Balance = 2800000,
            CreatedAt = DateTime.Now.AddMonths(-5),
            LastUpdatedAt = DateTime.Now.AddDays(-7),
            Status = "Active",
            IsLocked = false
        });

        // Create sample transactions
        CreateSampleTransactions();
    }

    private static void CreateSampleTransactions()
    {
        // Transaction types: Deposit, Withdrawal, TransferIn, TransferOut, Donation, EntryFee, PrizeMoney

        // Deposits
        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 1,
            Username = "admin",
            TransactionType = "Deposit",
            Amount = 2000000,
            BalanceAfter = 2000000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddMonths(-6),
            ReferenceCode = $"TRX{DateTime.Now.AddMonths(-6):yyyyMMdd}000001",
            Note = "Nạp tiền ban đầu"
        });

        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 1,
            Username = "admin",
            TransactionType = "Deposit",
            Amount = 3000000,
            BalanceAfter = 5000000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddMonths(-3),
            ReferenceCode = $"TRX{DateTime.Now.AddMonths(-3):yyyyMMdd}000002",
            Note = "Nạp tiền qua bank transfer"
        });

        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 2,
            Username = "pro_gamer_vn",
            TransactionType = "Deposit",
            Amount = 1500000,
            BalanceAfter = 1500000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddMonths(-4),
            ReferenceCode = $"TRX{DateTime.Now.AddMonths(-4):yyyyMMdd}000003",
            Note = "Nạp tiền ban đầu"
        });

        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 2,
            Username = "pro_gamer_vn",
            TransactionType = "Deposit",
            Amount = 2000000,
            BalanceAfter = 3500000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddMonths(-2),
            ReferenceCode = $"TRX{DateTime.Now.AddMonths(-2):yyyyMMdd}000004",
            Note = "Nạp tiền qua Momo"
        });

        // Withdrawals
        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 4,
            Username = "mobile_legends_pro",
            TransactionType = "Withdrawal",
            Amount = -1000000,
            BalanceAfter = 1800000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-14),
            ReferenceCode = $"TRX{DateTime.Now.AddDays(-14):yyyyMMdd}000005",
            Note = "Rút tiền đến TPBank - Nguyen Van An - 0123456789"
        });

        // Transfers
        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 1,
            Username = "admin",
            TransactionType = "TransferOut",
            Amount = -500000,
            BalanceAfter = 4500000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-10),
            ReferenceCode = $"TRX{DateTime.Now.AddDays(-10):yyyyMMdd}000006",
            Note = "Chuyển tiền đến pro_gamer_vn - Prize money for winning",
            RelatedUserId = 2,
            RelatedUsername = "pro_gamer_vn"
        });

        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 2,
            Username = "pro_gamer_vn",
            TransactionType = "TransferIn",
            Amount = 500000,
            BalanceAfter = 4000000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-10),
            ReferenceCode = $"TRX{DateTime.Now.AddDays(-10):yyyyMMdd}000006",
            Note = "Nhận tiền từ admin - Prize money for winning",
            RelatedUserId = 1,
            RelatedUsername = "admin"
        });

        // Donations
        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 3,
            Username = "esports_fan",
            TransactionType = "Donation",
            Amount = -200000,
            BalanceAfter = 1000000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-3),
            ReferenceCode = $"TRX{DateTime.Now.AddDays(-3):yyyyMMdd}000007",
            Note = "Donate cho Team #2: Ủng hộ team vô địch!",
            RelatedEntityId = 2,
            RelatedEntityType = "Team"
        });

        _mockTransactions.Add(new TransactionDto
        {
            Id = _nextTransactionId++,
            UserId = 3,
            Username = "esports_fan",
            TransactionType = "Donation",
            Amount = -300000,
            BalanceAfter = 700000,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-1),
            ReferenceCode = $"TRX{DateTime.Now.AddDays(-1):yyyyMMdd}000008",
            Note = "Donate cho Giải đấu #1: Giải đấu tuyệt vời!",
            RelatedEntityId = 1,
            RelatedEntityType = "Tournament"
        });

        // Add more sample transactions with different dates
        AddRandomTransactions();
    }

    private static void AddRandomTransactions()
    {
        var transactionTypes = new[] { "Deposit", "Withdrawal", "Donation", "EntryFee", "PrizeMoney" };
        var targetTypes = new[] { "Tournament", "Team" };

        foreach (var userId in _mockWallets.Keys)
        {
            // Add 5-10 random transactions for each user
            int numTransactions = _random.Next(5, 11);

            for (int i = 0; i < numTransactions; i++)
            {
                var transactionType = transactionTypes[_random.Next(transactionTypes.Length)];
                var amount = _random.Next(10, 101) * 10000; // 100k to 1000k

                if (transactionType == "Withdrawal" || transactionType == "Donation" || transactionType == "EntryFee")
                    amount = -amount; // Make negative for outgoing transactions

                var daysAgo = _random.Next(1, 120); // Random date within the last 4 months
                var createdAt = DateTime.Now.AddDays(-daysAgo);

                // Skip creating transaction record if it would result in negative balance
                // This is just for mock data generation logic
                var wallet = _mockWallets[userId];
                var projectedBalance = wallet.Balance;
                if (projectedBalance + amount < 0)
                    continue;

                string note = "";
                int? relatedEntityId = null;
                string? relatedEntityType = null;

                switch (transactionType)
                {
                    case "Deposit":
                        note = "Nạp tiền qua " + new[] { "Bank transfer", "Momo", "ZaloPay", "Credit card" }[_random.Next(4)];
                        break;
                    case "Withdrawal":
                        note = "Rút tiền đến " + new[] { "TPBank", "Vietcombank", "Agribank", "BIDV" }[_random.Next(4)] +
                               " - " + $"Account #{_random.Next(1000000, 9999999)}";
                        break;
                    case "Donation":
                        relatedEntityType = targetTypes[_random.Next(targetTypes.Length)];
                        relatedEntityId = _random.Next(1, 5);
                        note = $"Donate cho {relatedEntityType} #{relatedEntityId}: " +
                               new[] { "Cố lên!", "Chúc may mắn!", "Team yêu thích!", "Vô địch!" }[_random.Next(4)];
                        break;
                    case "EntryFee":
                        relatedEntityType = "Tournament";
                        relatedEntityId = _random.Next(1, 5);
                        note = $"Phí tham gia giải đấu #{relatedEntityId}";
                        break;
                    case "PrizeMoney":
                        relatedEntityType = "Tournament";
                        relatedEntityId = _random.Next(1, 5);
                        note = $"Giải thưởng từ giải đấu #{relatedEntityId}";
                        break;
                }

                var transaction = new TransactionDto
                {
                    Id = _nextTransactionId++,
                    UserId = userId,
                    Username = wallet.Username,
                    TransactionType = transactionType,
                    Amount = amount,
                    BalanceAfter = projectedBalance + amount,
                    Status = "Completed",
                    CreatedAt = createdAt,
                    ReferenceCode = $"TRX{createdAt:yyyyMMdd}{_random.Next(100000, 999999)}",
                    Note = note,
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType
                };

                _mockTransactions.Add(transaction);
            }
        }
    }
}
