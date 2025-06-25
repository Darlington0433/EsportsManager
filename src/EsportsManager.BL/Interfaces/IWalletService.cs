using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces
{

    /// <summary>
    /// Wallet Service Interface - Quản lý các thao tác liên quan đến ví điện tử
    /// Áp dụng Interface Segregation Principle
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Lấy thông tin ví của người dùng
        /// </summary>
        Task<WalletInfoDto?> GetWalletByUserIdAsync(int userId);

        /// <summary>
        /// Nạp tiền vào ví
        /// </summary>
        Task<TransactionResultDto> DepositAsync(int userId, DepositDto depositDto);

        /// <summary>
        /// Rút tiền từ ví
        /// </summary>
        Task<TransactionResultDto> WithdrawAsync(int userId, WithdrawalDto withdrawalDto);

        /// <summary>
        /// Chuyển tiền cho người dùng khác
        /// </summary>
        Task<TransactionResultDto> TransferAsync(int fromUserId, TransferDto transferDto);

        /// <summary>
        /// Donate cho team hoặc giải đấu
        /// </summary>
        Task<TransactionResultDto> DonateAsync(int userId, DonationDto donationDto);

        /// <summary>
        /// Lấy lịch sử giao dịch của người dùng
        /// </summary>
        Task<List<TransactionDto>> GetTransactionHistoryAsync(
            int userId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? transactionType = null,
            int pageNumber = 1,
            int pageSize = 20);

        /// <summary>
        /// Lấy số dư của người dùng
        /// </summary>
        Task<decimal> GetBalanceAsync(int userId);

        /// <summary>
        /// Kiểm tra xem người dùng có đủ số dư không
        /// </summary>
        Task<bool> HasSufficientBalanceAsync(int userId, decimal amount);    /// <summary>
                                                                             /// Lấy thống kê giao dịch
                                                                             /// </summary>
        Task<WalletStatsDto> GetWalletStatsAsync(int userId);
    }
}
