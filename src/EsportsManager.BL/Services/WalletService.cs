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
    /// Cung cấp các chức năng quản lý ví điện tử và giao dịch (Database only)
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly DataContext _dataContext;

        public WalletService(ILogger<WalletService> logger, DataContext dataContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        // Helper methods for safe data type conversion
        private static DateTime SafeGetDateTime(object dbValue, DateTime defaultValue = default)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToDateTime(dbValue);
        }

        private static int SafeGetInt32(object dbValue, int defaultValue = 0)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToInt32(dbValue);
        }

        private static decimal SafeGetDecimal(object dbValue, decimal defaultValue = 0)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToDecimal(dbValue);
        }

        private static bool SafeGetBoolean(object dbValue, bool defaultValue = false)
        {
            return dbValue == DBNull.Value ? defaultValue : Convert.ToBoolean(dbValue);
        }

        /// <summary>
        /// Lấy thông tin ví của người dùng
        /// </summary>
        public async Task<WalletInfoDto?> GetWalletByUserIdAsync(int userId)
        {
            await Task.CompletedTask; // To satisfy async signature

            try
            {
                // Sử dụng query trực tiếp từ bảng Wallets thay vì view
                var sql = @"SELECT 
                    w.WalletID, 
                    w.UserID, 
                    u.Username, 
                    w.Balance, 
                    w.TotalReceived, 
                    w.TotalWithdrawn, 
                    w.LastUpdated 
                FROM Wallets w 
                JOIN Users u ON w.UserID = u.UserID 
                WHERE w.UserID = ?";

                using var connection = _dataContext.CreateConnection();
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                var parameter = _dataContext.CreateParameter("p_UserID", userId);
                command.Parameters.Add(parameter);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new WalletInfoDto
                    {
                        Id = SafeGetInt32(reader["WalletID"]),
                        UserId = userId,
                        Username = reader["Username"]?.ToString() ?? string.Empty,
                        Balance = SafeGetDecimal(reader["Balance"]),
                        TotalReceived = SafeGetDecimal(reader["TotalReceived"]),
                        TotalWithdrawn = SafeGetDecimal(reader["TotalWithdrawn"]),
                        CreatedAt = DateTime.Now,
                        Status = "Active",
                        IsLocked = false,
                        LastUpdatedAt = SafeGetDateTime(reader["LastUpdated"], DateTime.Now)
                    };
                }

                // Nếu không tìm thấy, có thể user chưa có wallet hoặc không phải Player
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet for user {UserId}", userId);
                throw new InvalidOperationException($"Không thể tải thông tin ví cho user {userId}: {ex.Message}", ex);
            }
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

                // Create deposit transaction
                try
                {
                    var paramUserId = _dataContext.CreateParameter("p_UserID", userId);
                    var paramAmount = _dataContext.CreateParameter("p_Amount", depositDto.Amount);
                    var paramRefCode = _dataContext.CreateParameter("p_ReferenceCode",
                        depositDto.ReferenceCode ?? GenerateReferenceCode());
                    var paramNote = _dataContext.CreateParameter("p_Note", depositDto.Note ?? string.Empty);

                    var result = _dataContext.ExecuteStoredProcedure(
                        "sp_CreateDeposit", paramUserId, paramAmount, paramRefCode, paramNote);

                    TransactionDto transaction;
                    if (result.Rows.Count > 0)
                    {
                        var row = result.Rows[0];
                        transaction = MapTransactionFromDataRow(row);
                    }
                    else
                    {
                        throw new Exception("Không thể tạo giao dịch nạp tiền");
                    }

                    _logger.LogInformation("Deposit successful for user {UserId}, Amount: {Amount}", userId, depositDto.Amount);

                    decimal newBalance = wallet.Balance + depositDto.Amount;
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
                    _logger.LogError(ex, "Error creating deposit transaction in database");
                    throw new InvalidOperationException($"Không thể thực hiện nạp tiền: {ex.Message}", ex);
                }
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

                // Create withdrawal record in database (triggers will update wallet automatically)
                try
                {
                    string note = $"Rút tiền đến {withdrawalDto.BankName ?? "N/A"} - {withdrawalDto.AccountName ?? "N/A"} - {withdrawalDto.BankAccount ?? "N/A"}" +
                       (string.IsNullOrEmpty(withdrawalDto.Note) ? "" : $" - {withdrawalDto.Note}");

                    var paramUserId = _dataContext.CreateParameter("p_UserID", userId);
                    var paramAmount = _dataContext.CreateParameter("p_Amount", withdrawalDto.Amount);
                    var paramRefCode = _dataContext.CreateParameter("p_ReferenceCode", GenerateReferenceCode());
                    var paramNote = _dataContext.CreateParameter("p_Note", note);
                    var paramBankName = _dataContext.CreateParameter("p_BankName", withdrawalDto.BankName ?? "");
                    var paramAccountName = _dataContext.CreateParameter("p_AccountName", withdrawalDto.AccountName ?? "");
                    var paramBankAccount = _dataContext.CreateParameter("p_BankAccount", withdrawalDto.BankAccount ?? "");

                    var result = _dataContext.ExecuteStoredProcedure(
                        "sp_CreateWithdrawal",
                        paramUserId, paramAmount, paramRefCode, paramNote,
                        paramBankName, paramAccountName, paramBankAccount);

                    TransactionDto transaction;
                    if (result.Rows.Count > 0)
                    {
                        var row = result.Rows[0];
                        transaction = MapTransactionFromDataRow(row);
                    }
                    else
                    {
                        throw new Exception("Không thể tạo giao dịch rút tiền");
                    }

                    _logger.LogInformation("Withdrawal successful for user {UserId}, Amount: {Amount}", userId, withdrawalDto.Amount);

                    decimal newBalance = wallet.Balance - withdrawalDto.Amount;
                    return new TransactionResultDto
                    {
                        Success = true,
                        Message = "Rút tiền thành công! Số dư đã được cập nhật.",
                        Transaction = transaction,
                        NewBalance = newBalance
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating withdrawal transaction in database");
                    throw new InvalidOperationException($"Không thể thực hiện rút tiền: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal for user {UserId}", userId);
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xử lý rút tiền",
                    Errors = new List<string> { "Vui lòng thử lại sau" }
                };
            }
        }

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        public async Task<List<TransactionDto>> GetTransactionHistoryAsync(
            int userId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? transactionType = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            await Task.CompletedTask; // To satisfy async signature

            try
            {
                // Sử dụng stored procedure để lấy transaction history
                var paramUserId = _dataContext.CreateParameter("p_UserID", userId);
                var paramLimit = _dataContext.CreateParameter("p_Limit", pageSize);

                var result = _dataContext.ExecuteStoredProcedure("sp_GetWalletTransactionHistory", paramUserId, paramLimit);

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
                throw new InvalidOperationException($"Không thể tải lịch sử giao dịch cho user {userId}: {ex.Message}", ex);
            }
        }        /// <summary>
                 /// Lấy thống kê ví
                 /// </summary>
        public async Task<WalletStatsDto> GetWalletStatsAsync(int userId)
        {
            await Task.CompletedTask; // To satisfy async signature

            try
            {
                var wallet = await GetWalletByUserIdAsync(userId);
                if (wallet == null)
                    return new WalletStatsDto
                    {
                        TotalIncome = 0,
                        TotalExpense = 0,
                        TotalTransactions = 0,
                        CurrentBalance = 0
                    };

                // Calculate stats from transaction history
                var transactions = await GetTransactionHistoryAsync(userId, pageSize: 1000);

                var totalIncome = transactions
                    .Where(t => t.Amount > 0 && t.Status == "Completed")
                    .Sum(t => t.Amount);

                var totalExpense = transactions
                    .Where(t => t.Amount < 0 && t.Status == "Completed")
                    .Sum(t => Math.Abs(t.Amount));

                return new WalletStatsDto
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    TotalTransactions = transactions.Count,
                    CurrentBalance = wallet.Balance
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet stats for user {UserId}", userId);
                throw new InvalidOperationException($"Không thể tải thống kê ví cho user {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy số dư ví
        /// </summary>
        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            return wallet?.Balance ?? 0;
        }

        /// <summary>
        /// Kiểm tra số dư có đủ không
        /// </summary>
        public async Task<bool> HasSufficientBalanceAsync(int userId, decimal amount)
        {
            var balance = await GetBalanceAsync(userId);
            return balance >= amount;
        }

        /// <summary>
        /// Donate (donation features not implemented in database-only mode)
        /// </summary>
        public async Task<TransactionResultDto> DonateAsync(int fromUserId, DonationDto donationDto)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Donation feature not implemented in database-only mode");
        }

        /// <summary>
        /// Search donations (not implemented in database-only mode)
        /// </summary>
        public async Task<List<TransactionDto>> SearchDonationsAsync(DonationSearchFilterDto filter)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Search donations not implemented in database-only mode");
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

                // TODO: Implement transfer logic with database transaction
                // For now, throw not implemented
                throw new NotImplementedException("Transfer functionality not yet implemented in database-only mode");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transfer from user {FromUserId} to {ToUserId}", fromUserId, transferDto.ToUserId);
                return new TransactionResultDto
                {
                    Success = false,
                    Message = "Lỗi khi xử lý chuyển tiền",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        /// <summary>
        /// Test method to check database connection and basic queries
        /// </summary>
        public async Task<string> TestDatabaseConnectionAsync()
        {
            await Task.CompletedTask;

            try
            {
                // Test basic connection
                using var connection = _dataContext.CreateConnection();
                connection.Open();

                // Test simple query
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Wallets";
                command.CommandType = CommandType.Text;

                var result = command.ExecuteScalar();
                return $"✅ Database connection OK. Total wallets: {result}";
            }
            catch (Exception ex)
            {
                return $"❌ Database error: {ex.Message}";
            }
        }

        // Helper methods remain the same...
        private string GenerateReferenceCode()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private TransactionDto MapTransactionFromDataRow(DataRow row)
        {
            return new TransactionDto
            {
                Id = SafeGetInt32(row["TransactionID"]),
                UserId = SafeGetInt32(row["UserID"]),
                Amount = SafeGetDecimal(row["Amount"]),
                TransactionType = row["TransactionType"]?.ToString() ?? string.Empty,
                Status = row["Status"]?.ToString() ?? string.Empty,
                Note = row["Note"]?.ToString() ?? string.Empty,
                ReferenceCode = row["ReferenceCode"]?.ToString() ?? string.Empty,
                CreatedAt = SafeGetDateTime(row["CreatedAt"], DateTime.Now)
            };
        }

        // Not implemented methods for donation features
        public async Task<DonationOverviewDto> GetDonationOverviewAsync()
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Donation overview not implemented in database-only mode");
        }

        public async Task<List<TopDonationUserDto>> GetTopDonationReceiversAsync(int limit = 10)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Top donation receivers not implemented in database-only mode");
        }

        public async Task<List<TopDonationUserDto>> GetTopDonatorsAsync(int limit = 10)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Top donators not implemented in database-only mode");
        }

        public async Task<List<TransactionDto>> GetDonationHistoryAsync(DonationSearchFilterDto filter)
        {
            await Task.CompletedTask;
            throw new NotImplementedException("Donation history not implemented in database-only mode");
        }
    }
}
