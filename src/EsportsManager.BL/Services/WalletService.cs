using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Context;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services
{

    /// <summary>
    /// Wallet Service Implementation
    /// Cung cấp các chức năng quản lý ví điện tử và giao dịch
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly DataContext _dataContext;

        private bool _useDatabase = true;

        public WalletService(ILogger<WalletService> logger, DataContext dataContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

            // Kiểm tra nếu có thể kết nối đến database với error handling tốt hơn
            try
            {
                _useDatabase = _dataContext.TestConnectionAsync().Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed in WalletService constructor");
                _useDatabase = false;
            }

            if (!_useDatabase)
            {
                _logger.LogWarning("Database connection failed. Please check the connection settings.");
            }
            else
            {
                _logger.LogInformation("WalletService: Database connection established successfully");
            }
        }

        /// <summary>
        /// Lấy thông tin ví của người dùng
        /// </summary>
        public async Task<WalletInfoDto?> GetWalletByUserIdAsync(int userId)
        {
            if (_useDatabase)
            {
                try
                {
                    // Sử dụng view để lấy thông tin wallet
                    var sql = "SELECT * FROM v_user_wallet_summary WHERE UserID = @UserId";
                    using var connection = _dataContext.CreateConnection();
                    connection.Open(); // Use sync version for IDbConnection

                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    var parameter = _dataContext.CreateParameter("@UserId", userId);
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader(); // Use sync version for IDbCommand
                    if (reader.Read()) // Use sync version
                    {
                        return new WalletInfoDto
                        {
                            Id = Convert.ToInt32(reader["WalletID"]),
                            UserId = userId,
                            Username = reader["Username"]?.ToString() ?? string.Empty,
                            Balance = Convert.ToDecimal(reader["Balance"]),
                            TotalReceived = Convert.ToDecimal(reader["TotalReceived"]),
                            TotalWithdrawn = Convert.ToDecimal(reader["TotalWithdrawn"]),
                            CreatedAt = DateTime.Now, // Sẽ lấy từ database nếu có
                            Status = "Active",
                            IsLocked = false,
                            LastUpdatedAt = reader["LastUpdated"] != DBNull.Value
                                ? Convert.ToDateTime(reader["LastUpdated"])
                                : DateTime.Now
                        };
                    }

                    // Nếu không tìm thấy, trigger sẽ tự động tạo wallet khi tạo user mới
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving wallet for user {UserId}", userId);
                    _logger.LogError(ex, "Database connection error in GetWalletByUserIdAsync");
                    throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.", ex);
                }
            }

            // Nếu không có database connection
            _logger.LogError("Database connection required for wallet operations");
            throw new InvalidOperationException("Không thể thực hiện thao tác với ví khi không có kết nối cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.");
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

                // Create transaction record in database
                TransactionDto transaction;
                if (_useDatabase)
                {
                    try
                    {
                        var paramUserId = _dataContext.CreateParameter("@UserId", userId);
                        var paramAmount = _dataContext.CreateParameter("@Amount", depositDto.Amount);
                        var paramRefCode = _dataContext.CreateParameter("@ReferenceCode",
                            depositDto.ReferenceCode ?? GenerateReferenceCode());
                        var paramNote = _dataContext.CreateParameter("@Note", depositDto.Note ?? string.Empty);

                        var result = _dataContext.ExecuteStoredProcedure(
                            "sp_CreateDeposit", paramUserId, paramAmount, paramRefCode, paramNote);

                        if (result.Rows.Count > 0)
                        {
                            var row = result.Rows[0];
                            transaction = MapTransactionFromDataRow(row);
                        }
                        else
                        {
                            throw new Exception("Không thể tạo giao dịch nạp tiền");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating deposit transaction in database");
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Không thể thực hiện giao dịch khi không có kết nối cơ sở dữ liệu");
                }
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

                // Create withdrawal record in database
                TransactionDto transaction;
                if (_useDatabase)
                {
                    try
                    {
                        string note = $"Rút tiền đến {withdrawalDto.BankName} - {withdrawalDto.AccountName} - {withdrawalDto.BankAccount}" +
                           (string.IsNullOrEmpty(withdrawalDto.Note) ? "" : $" - {withdrawalDto.Note}");

                        var paramUserId = _dataContext.CreateParameter("@UserId", userId);
                        var paramAmount = _dataContext.CreateParameter("@Amount", withdrawalDto.Amount);
                        var paramRefCode = _dataContext.CreateParameter("@ReferenceCode", GenerateReferenceCode());
                        var paramNote = _dataContext.CreateParameter("@Note", note);
                        var paramBankName = _dataContext.CreateParameter("@BankName", withdrawalDto.BankName);
                        var paramAccountName = _dataContext.CreateParameter("@AccountName", withdrawalDto.AccountName);
                        var paramBankAccount = _dataContext.CreateParameter("@BankAccount", withdrawalDto.BankAccount);

                        var result = _dataContext.ExecuteStoredProcedure(
                            "sp_CreateWithdrawal",
                            paramUserId, paramAmount, paramRefCode, paramNote,
                            paramBankName, paramAccountName, paramBankAccount);

                        if (result.Rows.Count > 0)
                        {
                            var row = result.Rows[0];
                            transaction = MapTransactionFromDataRow(row);
                        }
                        else
                        {
                            throw new Exception("Không thể tạo giao dịch rút tiền");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating withdrawal transaction in database");
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Không thể thực hiện giao dịch khi không có kết nối cơ sở dữ liệu");
                }
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

                // Create transfer record in database
                TransactionDto senderTransaction;
                if (_useDatabase)
                {
                    try
                    {
                        string refCode = GenerateReferenceCode();
                        string senderNote = $"Chuyển tiền đến {transferDto.ToUsername}" +
                                          (string.IsNullOrEmpty(transferDto.Note) ? "" : $" - {transferDto.Note}");

                        var paramFromUserId = _dataContext.CreateParameter("@FromUserId", fromUserId);
                        var paramToUserId = _dataContext.CreateParameter("@ToUserId", transferDto.ToUserId);
                        var paramAmount = _dataContext.CreateParameter("@Amount", transferDto.Amount);
                        var paramRefCode = _dataContext.CreateParameter("@ReferenceCode", refCode);
                        var paramNote = _dataContext.CreateParameter("@Note", transferDto.Note ?? string.Empty);

                        var result = _dataContext.ExecuteStoredProcedure(
                            "sp_CreateTransfer",
                            paramFromUserId, paramToUserId, paramAmount, paramRefCode, paramNote);

                        if (result.Rows.Count > 0)
                        {
                            var row = result.Rows[0];
                            senderTransaction = MapTransactionFromDataRow(row);
                        }
                        else
                        {
                            throw new Exception("Không thể tạo giao dịch chuyển tiền");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating transfer transaction in database");
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Không thể thực hiện giao dịch khi không có kết nối cơ sở dữ liệu");
                }
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
                int? targetId = donationDto.DonationType switch
                {
                    "Tournament" => donationDto.TournamentId,
                    "Team" => donationDto.TeamId,
                    "Player" => donationDto.PlayerId,
                    _ => null
                };

                string targetName = donationDto.DonationType switch
                {
                    "Tournament" => $"Giải đấu #{donationDto.TournamentId}",
                    "Team" => $"Team #{donationDto.TeamId}",
                    "Player" => $"Player #{donationDto.PlayerId}",
                    _ => "Unknown"
                };

                if (targetId == null)
                    return new TransactionResultDto
                    {
                        Success = false,
                        Message = "Đối tượng donate không hợp lệ",
                        Errors = new List<string> { "Vui lòng chọn giải đấu, team hoặc player để donate" }
                    };

                // Process donation
                decimal newBalance = wallet.Balance - donationDto.Amount;
                wallet.Balance = newBalance;
                wallet.LastUpdatedAt = DateTime.Now;

                // Create donation record in database
                TransactionDto transaction;
                if (_useDatabase)
                {
                    try
                    {
                        string note = $"Donate cho {targetName}: {donationDto.Message}";

                        var paramUserId = _dataContext.CreateParameter("@UserId", userId);
                        var paramAmount = _dataContext.CreateParameter("@Amount", donationDto.Amount);
                        var paramRefCode = _dataContext.CreateParameter("@ReferenceCode", GenerateReferenceCode());
                        var paramNote = _dataContext.CreateParameter("@Note", note);
                        var paramMessage = _dataContext.CreateParameter("@Message", donationDto.Message ?? string.Empty);
                        var paramEntityType = _dataContext.CreateParameter("@EntityType", targetType);
                        var paramEntityId = _dataContext.CreateParameter("@EntityId", targetId.Value);

                        var result = _dataContext.ExecuteStoredProcedure(
                            "sp_CreateDonation",
                            paramUserId, paramAmount, paramRefCode, paramNote,
                            paramMessage, paramEntityType, paramEntityId);

                        if (result.Rows.Count > 0)
                        {
                            var row = result.Rows[0];
                            transaction = MapTransactionFromDataRow(row);
                        }
                        else
                        {
                            throw new Exception("Không thể tạo giao dịch donation");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating donation transaction in database");
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Không thể thực hiện giao dịch khi không có kết nối cơ sở dữ liệu");
                }
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
            if (_useDatabase)
            {
                try
                {
                    // Tạo tham số cho stored procedure
                    var parameters = new List<IDbDataParameter>
                    {
                        _dataContext.CreateParameter("@UserId", userId),
                        _dataContext.CreateParameter("@PageNumber", pageNumber),
                        _dataContext.CreateParameter("@PageSize", pageSize)
                    };

                    // Thêm các tham số lọc tùy chọn
                    if (fromDate.HasValue)
                        parameters.Add(_dataContext.CreateParameter("@FromDate", fromDate));

                    if (toDate.HasValue)
                        parameters.Add(_dataContext.CreateParameter("@ToDate", toDate));

                    if (!string.IsNullOrWhiteSpace(transactionType))
                        parameters.Add(_dataContext.CreateParameter("@TransactionType", transactionType));

                    var result = _dataContext.ExecuteStoredProcedure("sp_GetUserTransactionHistory", parameters.ToArray());

                    var transactions = new List<TransactionDto>();
                    foreach (DataRow row in result.Rows)
                    {
                        transactions.Add(MapTransactionFromDataRow(row));
                    }

                    return transactions;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving transaction history for user {UserId}", userId);
                    throw new InvalidOperationException("Không thể truy vấn lịch sử giao dịch từ database", ex);
                }
            }

            // Nếu không có kết nối database
            throw new InvalidOperationException("Không thể lấy lịch sử giao dịch khi không có kết nối cơ sở dữ liệu");
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
            if (_useDatabase)
            {
                try
                {
                    // Lấy thống kê tổng quan
                    var paramUserId = _dataContext.CreateParameter("@UserId", userId);
                    var result = _dataContext.ExecuteStoredProcedure("sp_GetUserWalletStats", paramUserId);

                    if (result.Rows.Count == 0)
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

                    var row = result.Rows[0];
                    var stats = new WalletStatsDto
                    {
                        TotalTransactions = Convert.ToInt32(row["TotalTransactions"]),
                        TotalIncome = Convert.ToDecimal(row["TotalIncome"]),
                        TotalExpense = Convert.ToDecimal(row["TotalExpense"]),
                        CurrentBalance = Convert.ToDecimal(row["CurrentBalance"]),
                        MonthlyStats = new List<MonthlyTransactionDto>(),
                        RecentTransactions = new List<TransactionDto>()
                    };

                    // Lấy thống kê theo nguồn thu
                    var incomeSourceResult = _dataContext.ExecuteStoredProcedure("sp_GetUserIncomeBySource", paramUserId);
                    stats.IncomeBySource = new Dictionary<string, decimal>();
                    foreach (DataRow sourceRow in incomeSourceResult.Rows)
                    {
                        string source = sourceRow["TransactionType"].ToString() ?? "Unknown";
                        decimal amount = Convert.ToDecimal(sourceRow["Amount"]);
                        stats.IncomeBySource.Add(source, amount);
                    }

                    // Lấy thống kê theo loại chi tiêu
                    var expenseCategoryResult = _dataContext.ExecuteStoredProcedure("sp_GetUserExpenseByCategory", paramUserId);
                    stats.ExpenseByCategory = new Dictionary<string, decimal>();
                    foreach (DataRow categoryRow in expenseCategoryResult.Rows)
                    {
                        string category = categoryRow["TransactionType"].ToString() ?? "Unknown";
                        decimal amount = Convert.ToDecimal(categoryRow["Amount"]);
                        stats.ExpenseByCategory.Add(category, amount);
                    }

                    // Lấy thống kê theo tháng
                    var monthlyStatsResult = _dataContext.ExecuteStoredProcedure("sp_GetUserMonthlyStats", paramUserId);
                    foreach (DataRow monthRow in monthlyStatsResult.Rows)
                    {
                        stats.MonthlyStats.Add(new MonthlyTransactionDto
                        {
                            Year = Convert.ToInt32(monthRow["Year"]),
                            Month = Convert.ToInt32(monthRow["Month"]),
                            TotalIncome = Convert.ToDecimal(monthRow["TotalIncome"]),
                            TotalExpense = Convert.ToDecimal(monthRow["TotalExpense"]),
                            TransactionCount = Convert.ToInt32(monthRow["TransactionCount"])
                        });
                    }

                    // Lấy các giao dịch gần đây
                    var recentTransactionsResult = _dataContext.ExecuteStoredProcedure(
                        "sp_GetUserRecentTransactions", paramUserId);

                    foreach (DataRow transactionRow in recentTransactionsResult.Rows)
                    {
                        stats.RecentTransactions.Add(MapTransactionFromDataRow(transactionRow));
                    }

                    return stats;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving wallet stats for user {UserId}", userId);
                    throw new InvalidOperationException("Không thể lấy thống kê ví từ database", ex);
                }
            }

            // Nếu không có kết nối database
            throw new InvalidOperationException("Không thể lấy thống kê ví khi không có kết nối cơ sở dữ liệu");
        }

        /// <summary>
        /// Lấy tổng quan báo cáo donation
        /// </summary>
        public async Task<DonationOverviewDto> GetDonationOverviewAsync()
        {
            if (_useDatabase)
            {
                try
                {
                    // Sử dụng stored procedure để lấy tổng quan donation
                    var result = _dataContext.ExecuteStoredProcedure("sp_GetDonationOverview");

                    if (result.Rows.Count > 0)
                    {
                        var row = result.Rows[0];
                        var overview = new DonationOverviewDto
                        {
                            TotalDonations = row["TotalDonations"] != DBNull.Value ? Convert.ToInt32(row["TotalDonations"]) : 0,
                            TotalDonators = row["TotalDonators"] != DBNull.Value ? Convert.ToInt32(row["TotalDonators"]) : 0,
                            TotalReceivers = row["TotalReceivers"] != DBNull.Value ? Convert.ToInt32(row["TotalReceivers"]) : 0,
                            TotalDonationAmount = row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]) : 0m,
                            LastUpdated = DateTime.Now
                        };

                        // Lấy thông kê theo loại
                        try
                        {
                            var byTypeTable = _dataContext.ExecuteStoredProcedure("sp_GetDonationsByType");
                            foreach (DataRow typeRow in byTypeTable.Rows)
                            {
                                string donationType = typeRow["DonationType"]?.ToString() ?? "Unknown";
                                decimal amount = typeRow["Amount"] != DBNull.Value ? Convert.ToDecimal(typeRow["Amount"]) : 0m;
                                overview.DonationByType[donationType] = amount;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Could not load donation by type data, continuing with basic overview");
                            // Thêm một số dữ liệu mặc định nếu không thể lấy được
                            if (overview.DonationByType.Count == 0)
                            {
                                overview.DonationByType["Tournament"] = 0m;
                                overview.DonationByType["Team"] = 0m;
                                overview.DonationByType["Player"] = 0m;
                            }
                        }

                        return overview;
                    }

                    // Nếu không có kết quả, trả về giá trị mặc định với cấu trúc hoàn chỉnh
                    _logger.LogInformation("No donation data found, returning default overview");
                    return new DonationOverviewDto
                    {
                        TotalDonations = 0,
                        TotalDonators = 0,
                        TotalReceivers = 0,
                        TotalDonationAmount = 0m,
                        LastUpdated = DateTime.Now,
                        DonationByType = new Dictionary<string, decimal>
                        {
                            {"Tournament", 0m},
                            {"Team", 0m},
                            {"Player", 0m}
                        }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving donation overview: {ErrorMessage}", ex.Message);

                    // Kiểm tra xem có phải lỗi stored procedure không tồn tại
                    if (ex.Message.Contains("doesn't exist") || ex.Message.Contains("does not exist") || ex.Message.Contains("PROCEDURE") || ex.Message.Contains("procedure"))
                    {
                        throw new InvalidOperationException("Stored procedure 'sp_GetDonationOverview' chưa được tạo hoặc bị lỗi. Vui lòng chạy lại file database/esportsmanager.sql để tạo đầy đủ các stored procedures.", ex);
                    }

                    // Lỗi kết nối database
                    if (ex.Message.Contains("connection") || ex.Message.Contains("server") || ex.Message.Contains("timeout"))
                    {
                        throw new InvalidOperationException("Lỗi kết nối cơ sở dữ liệu. Vui lòng kiểm tra MySQL server đang chạy và thông tin kết nối đúng.", ex);
                    }

                    throw new InvalidOperationException("Không thể lấy tổng quan donation. Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.", ex);
                }
            }

            // Nếu không có kết nối database
            _logger.LogError("Database connection required for donation overview operations");
            throw new InvalidOperationException("Không thể lấy tổng quan donation khi không có kết nối cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.");
        }

        /// <summary>
        /// Lấy danh sách top người nhận donation
        /// </summary>
        public async Task<List<TopDonationUserDto>> GetTopDonationReceiversAsync(int limit = 10)
        {
            if (_useDatabase)
            {
                try
                {
                    // Use raw SQL instead of stored procedure to avoid parameter issues
                    var sql = @"
                        SELECT 
                            d.TargetID as EntityId,
                            d.TargetType as EntityType,
                            COALESCE(u.Username, CONCAT(d.TargetType, ' #', d.TargetID)) as EntityName,
                            COUNT(*) as DonationCount,
                            SUM(d.Amount) as TotalAmount,
                            MIN(d.DonationDate) as FirstDonation,
                            MAX(d.DonationDate) as LastDonation
                        FROM Donations d
                        LEFT JOIN Users u ON (d.TargetType = 'Player' AND d.TargetID = u.UserID)
                        WHERE d.Status = 'Completed'
                        GROUP BY d.TargetID, d.TargetType
                        ORDER BY TotalAmount DESC
                        LIMIT @Limit";

                    using var connection = _dataContext.CreateConnection();
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    var parameter = _dataContext.CreateParameter("@Limit", limit);
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader();
                    var topReceivers = new List<TopDonationUserDto>();

                    while (reader.Read())
                    {
                        topReceivers.Add(new TopDonationUserDto
                        {
                            UserId = reader["EntityId"] != DBNull.Value ? Convert.ToInt32(reader["EntityId"]) : 0,
                            Username = Convert.ToString(reader["EntityName"]) ?? "Unknown",
                            UserType = Convert.ToString(reader["EntityType"]) ?? "Unknown",
                            DonationCount = Convert.ToInt32(reader["DonationCount"]),
                            TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                            FirstDonation = Convert.ToDateTime(reader["FirstDonation"]),
                            LastDonation = Convert.ToDateTime(reader["LastDonation"])
                        });
                    }

                    return topReceivers;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving top donation receivers");
                    throw new InvalidOperationException("Không thể lấy danh sách top người nhận donation. Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.", ex);
                }
            }

            // Nếu không có kết nối database
            _logger.LogError("Database connection required for getting top donation receivers");
            throw new InvalidOperationException("Không thể lấy danh sách top người nhận donation khi không có kết nối cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.");
        }

        /// <summary>
        /// Lấy danh sách top người gửi donation
        /// </summary>
        public async Task<List<TopDonationUserDto>> GetTopDonatorsAsync(int limit = 10)
        {
            if (_useDatabase)
            {
                try
                {
                    // Use raw SQL instead of stored procedure to avoid parameter issues
                    var sql = @"
                        SELECT 
                            d.UserID,
                            u.Username,
                            COUNT(*) as DonationCount,
                            SUM(d.Amount) as TotalAmount,
                            MIN(d.DonationDate) as FirstDonation,
                            MAX(d.DonationDate) as LastDonation
                        FROM Donations d
                        INNER JOIN Users u ON d.UserID = u.UserID
                        WHERE d.Status = 'Completed'
                        GROUP BY d.UserID, u.Username
                        ORDER BY TotalAmount DESC
                        LIMIT @Limit";

                    using var connection = _dataContext.CreateConnection();
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    var parameter = _dataContext.CreateParameter("@Limit", limit);
                    command.Parameters.Add(parameter);

                    using var reader = command.ExecuteReader();
                    var topDonators = new List<TopDonationUserDto>();

                    while (reader.Read())
                    {
                        topDonators.Add(new TopDonationUserDto
                        {
                            UserId = Convert.ToInt32(reader["UserID"]),
                            Username = Convert.ToString(reader["Username"]) ?? "Unknown",
                            UserType = "User",
                            DonationCount = Convert.ToInt32(reader["DonationCount"]),
                            TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                            FirstDonation = Convert.ToDateTime(reader["FirstDonation"]),
                            LastDonation = Convert.ToDateTime(reader["LastDonation"])
                        });
                    }

                    return topDonators;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving top donators");
                    throw new InvalidOperationException("Không thể lấy danh sách top người gửi donation. Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.", ex);
                }
            }

            // Nếu không có kết nối database
            _logger.LogError("Database connection required for getting top donators");
            throw new InvalidOperationException("Không thể lấy danh sách top người gửi donation khi không có kết nối cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.");
        }

        /// <summary>
        /// Lấy danh sách lịch sử donation với bộ lọc
        /// </summary>
        public async Task<List<TransactionDto>> GetDonationHistoryAsync(DonationSearchFilterDto filter)
        {
            if (_useDatabase)
            {
                try
                {
                    // Use raw SQL instead of stored procedure to avoid parameter issues
                    var sql = @"
                        SELECT 
                            wt.TransactionID as Id,
                            wt.UserID,
                            u.Username,
                            'Donation' as TransactionType,
                            ABS(wt.Amount) as Amount,
                            wt.Status,
                            wt.CreatedAt,
                            wt.ReferenceCode,
                            wt.Note,
                            wt.RelatedEntityType,
                            wt.RelatedEntityID
                        FROM WalletTransactions wt
                        JOIN Users u ON wt.UserID = u.UserID
                        WHERE wt.TransactionType = 'Donation'";

                    var parameters = new List<IDbDataParameter>();
                    var whereConditions = new List<string>();

                    // Add optional filter conditions
                    if (filter.FromDate.HasValue)
                    {
                        whereConditions.Add("wt.CreatedAt >= @FromDate");
                        parameters.Add(_dataContext.CreateParameter("@FromDate", filter.FromDate.Value));
                    }

                    if (filter.ToDate.HasValue)
                    {
                        whereConditions.Add("wt.CreatedAt <= @ToDate");
                        parameters.Add(_dataContext.CreateParameter("@ToDate", filter.ToDate.Value));
                    }

                    if (filter.MinAmount.HasValue)
                    {
                        whereConditions.Add("ABS(wt.Amount) >= @MinAmount");
                        parameters.Add(_dataContext.CreateParameter("@MinAmount", filter.MinAmount.Value));
                    }

                    if (filter.MaxAmount.HasValue)
                    {
                        whereConditions.Add("ABS(wt.Amount) <= @MaxAmount");
                        parameters.Add(_dataContext.CreateParameter("@MaxAmount", filter.MaxAmount.Value));
                    }

                    // Add WHERE conditions if any
                    if (whereConditions.Count > 0)
                    {
                        sql += " AND " + string.Join(" AND ", whereConditions);
                    }

                    // Add ORDER BY and LIMIT
                    sql += " ORDER BY wt.CreatedAt DESC";

                    var offset = (filter.PageNumber - 1) * filter.PageSize;
                    sql += $" LIMIT {filter.PageSize} OFFSET {offset}";

                    using var connection = _dataContext.CreateConnection();
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using var reader = command.ExecuteReader();
                    var transactions = new List<TransactionDto>();

                    while (reader.Read())
                    {
                        transactions.Add(new TransactionDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = Convert.ToInt32(reader["UserID"]),
                            Username = Convert.ToString(reader["Username"]) ?? "",
                            TransactionType = Convert.ToString(reader["TransactionType"]) ?? "",
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            Status = Convert.ToString(reader["Status"]) ?? "",
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            ReferenceCode = Convert.ToString(reader["ReferenceCode"]) ?? "",
                            Note = Convert.ToString(reader["Note"]) ?? "",
                            RelatedEntityType = Convert.ToString(reader["RelatedEntityType"]),
                            RelatedEntityId = reader["RelatedEntityID"] != DBNull.Value ? Convert.ToInt32(reader["RelatedEntityID"]) : null
                        });
                    }

                    return transactions;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving donation history");
                    throw new InvalidOperationException("Không thể lấy lịch sử donation. Vui lòng kiểm tra cài đặt cơ sở dữ liệu và thử lại.", ex);
                }
            }

            // Nếu không có kết nối database
            _logger.LogError("Database connection required for donation history");
            throw new InvalidOperationException("Không thể lấy lịch sử donation khi không có kết nối cơ sở dữ liệu. Vui lòng kiểm tra cài đặt kết nối và thử lại.");
        }

        /// <summary>
        /// Tìm kiếm donation theo các tiêu chí khác nhau
        /// </summary>
        public async Task<List<TransactionDto>> SearchDonationsAsync(DonationSearchFilterDto filter)
        {
            // Tìm kiếm donation sử dụng hàm GetDonationHistoryAsync nhưng với các điều kiện tìm kiếm khác nhau
            return await GetDonationHistoryAsync(filter);
        }

        // Helper method để map từ DataRow sang TransactionDto
        private TransactionDto MapTransactionFromDataRow(DataRow row)
        {
            return new TransactionDto
            {
                Id = Convert.ToInt32(row["TransactionID"]),
                UserId = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString() ?? string.Empty,
                TransactionType = row["TransactionType"].ToString() ?? string.Empty,
                Amount = Convert.ToDecimal(row["Amount"]),
                BalanceAfter = Convert.ToDecimal(row["BalanceAfter"]),
                Status = row["Status"].ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                ReferenceCode = row["ReferenceCode"].ToString(),
                Note = row["Note"].ToString(),
                RelatedUserId = row["RelatedUserID"] != DBNull.Value ? Convert.ToInt32(row["RelatedUserID"]) : null,
                RelatedUsername = row["RelatedUsername"].ToString(),
                RelatedEntityId = row["RelatedEntityID"] != DBNull.Value ? Convert.ToInt32(row["RelatedEntityID"]) : null,
                RelatedEntityType = row["RelatedEntityType"].ToString()
            };
        }

        // Helper methods
        private string GenerateReferenceCode()
        {
            return $"TRX{DateTime.Now:yyyyMMdd}{DateTime.Now.Ticks % 1000000:D6}";
        }
    }
}
