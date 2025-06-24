using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho thông tin ví
/// </summary>
public class WalletInfoDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public string Status { get; set; } = "Active";
    public bool IsLocked { get; set; }
}

/// <summary>
/// DTO cho giao dịch nạp tiền
/// </summary>
public class DepositDto
{
    [Required(ErrorMessage = "Số tiền không được để trống")]
    [Range(10000, 100000000, ErrorMessage = "Số tiền nạp phải từ 10.000 đến 100.000.000 VND")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Phương thức thanh toán không được để trống")]
    public string PaymentMethod { get; set; } = "BankTransfer"; // BankTransfer, CreditCard, Momo, ZaloPay, etc.

    public string? ReferenceCode { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO cho giao dịch rút tiền
/// </summary>
public class WithdrawalDto
{
    [Required(ErrorMessage = "Số tiền không được để trống")]
    [Range(50000, 50000000, ErrorMessage = "Số tiền rút phải từ 50.000 đến 50.000.000 VND")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Thông tin tài khoản nhận không được để trống")]
    public string BankAccount { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên ngân hàng không được để trống")]
    public string BankName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên chủ tài khoản không được để trống")]
    public string AccountName { get; set; } = string.Empty;

    public string? Note { get; set; }
}

/// <summary>
/// DTO cho giao dịch chuyển khoản
/// </summary>
public class TransferDto
{
    [Required(ErrorMessage = "ID người nhận không được để trống")]
    public int ToUserId { get; set; }

    [Required(ErrorMessage = "Tên người nhận không được để trống")]
    public string ToUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số tiền không được để trống")]
    [Range(10000, 10000000, ErrorMessage = "Số tiền chuyển phải từ 10.000 đến 10.000.000 VND")]
    public decimal Amount { get; set; }

    [StringLength(200, ErrorMessage = "Ghi chú không được vượt quá 200 ký tự")]
    public string? Note { get; set; }
}

/// <summary>
/// DTO cho giao dịch donate
/// </summary>
public class DonationDto
{
    [Required(ErrorMessage = "Số tiền không được để trống")]
    [Range(10000, 10000000, ErrorMessage = "Số tiền donate phải từ 10.000 đến 10.000.000 VND")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Thông điệp không được để trống")]
    [StringLength(200, ErrorMessage = "Thông điệp không được vượt quá 200 ký tự")]
    public string Message { get; set; } = string.Empty;

    public int? TournamentId { get; set; }
    public int? TeamId { get; set; }

    [Required(ErrorMessage = "Loại donate không được để trống")]
    public string DonationType { get; set; } = "Tournament"; // Tournament, Team
}

/// <summary>
/// DTO cho kết quả giao dịch
/// </summary>
public class TransactionResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionDto? Transaction { get; set; }
    public decimal NewBalance { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// DTO cho thông tin giao dịch
/// </summary>
public class TransactionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer, Donation, EntryFee, PrizeMoney
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Completed, Failed, Cancelled
    public DateTime CreatedAt { get; set; }
    public string? ReferenceCode { get; set; }
    public string? Note { get; set; }

    // Thông tin đối tác giao dịch (nếu có)
    public int? RelatedUserId { get; set; }
    public string? RelatedUsername { get; set; }
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

/// <summary>
/// DTO cho thống kê ví
/// </summary>
public class WalletStatsDto
{
    public int TotalTransactions { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal CurrentBalance { get; set; }
    public Dictionary<string, decimal> IncomeBySource { get; set; } = new();
    public Dictionary<string, decimal> ExpenseByCategory { get; set; } = new();
    public List<MonthlyTransactionDto> MonthlyStats { get; set; } = new();
    public List<TransactionDto> RecentTransactions { get; set; } = new();
}

/// <summary>
/// DTO cho thống kê giao dịch theo tháng
/// </summary>
public class MonthlyTransactionDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public int TransactionCount { get; set; }
}
