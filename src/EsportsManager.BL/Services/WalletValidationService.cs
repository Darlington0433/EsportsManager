using EsportsManager.BL.Constants;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Models;
using EsportsManager.BL.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Services;

/// <summary>
/// Service for wallet-related business logic validation
/// </summary>
public class WalletValidationService
{
    /// <summary>
    /// Validates withdrawal amount from string input
    /// </summary>
    public static (bool IsValid, string ErrorMessage, decimal ValidatedAmount) ValidateWithdrawalAmount(string? input, decimal currentBalance)
    {
        var errors = new List<string>();
        
        if (ValidationHelper.IsNullOrEmpty(input, "Số tiền", errors))
        {
            return (false, string.Join("; ", errors), 0);
        }

        if (!decimal.TryParse(input, out decimal amount))
        {
            return (false, "Số tiền phải là số hợp lệ!", 0);
        }

        if (!ValidationHelper.IsValidAmount(amount, WalletConstants.MIN_WITHDRAWAL_AMOUNT, currentBalance, errors))
        {
            return (false, string.Join("; ", errors), 0);
        }

        return (true, string.Empty, amount);
    }

    /// <summary>
    /// Validates withdrawal request parameters
    /// </summary>
    public ValidationResult ValidateWithdrawalRequest(decimal amount, decimal currentBalance)
    {
        var errors = new List<string>();

        if (!ValidationHelper.IsValidAmount(amount, WalletConstants.MIN_WITHDRAWAL_AMOUNT, currentBalance, errors))
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Validates top-up request parameters
    /// </summary>
    public ValidationResult ValidateTopUpRequest(decimal amount)
    {
        var errors = new List<string>();

        if (!ValidationHelper.IsValidAmount(amount, WalletConstants.MIN_TOP_UP_AMOUNT, WalletConstants.MAX_TOP_UP_AMOUNT, errors))
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Validates donation request parameters
    /// </summary>
    public ValidationResult ValidateDonationRequest(decimal amount, decimal? currentBalance)
    {
        var errors = new List<string>();

        if (!ValidationHelper.IsValidAmount(amount, WalletConstants.MIN_DONATION_AMOUNT, 
            currentBalance ?? decimal.MaxValue, errors))
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Calculates withdrawal fee and net amount
    /// </summary>
    public (decimal Fee, decimal NetAmount) CalculateWithdrawalAmounts(decimal amount)
    {
        var fee = WalletConstants.WITHDRAWAL_FEE;
        var netAmount = amount - fee;
        return (fee, netAmount);
    }

    /// <summary>
    /// Validates bank account information
    /// </summary>
    public ValidationResult ValidateBankInfo(string? bankName, string? accountNumber, string? accountName)
    {
        var errors = new List<string>();

        if (ValidationHelper.IsNullOrEmpty(bankName, "Tên ngân hàng", errors) ||
            ValidationHelper.IsNullOrEmpty(accountNumber, "Số tài khoản", errors) ||
            ValidationHelper.IsNullOrEmpty(accountName, "Tên chủ tài khoản", errors))
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        if (!ValidationHelper.ValidateLength(accountNumber!, "Số tài khoản", 8, 20, errors))
        {
            return new ValidationResult { IsValid = false, Errors = errors };
        }

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Kiểm tra số tiền có hợp lệ không
    /// </summary>
    /// <param name="amount">Số tiền cần kiểm tra</param>
    /// <returns>Kết quả validation</returns>
    public ValidationResult ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Failure("Số tiền phải lớn hơn 0");
        }

        if (amount > WalletConstants.MAX_TRANSACTION_AMOUNT)
        {
            return ValidationResult.Failure($"Số tiền không được vượt quá {WalletConstants.MAX_TRANSACTION_AMOUNT:N0} VND");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Kiểm tra phương thức thanh toán có hợp lệ không
    /// </summary>
    /// <param name="paymentMethod">Phương thức thanh toán cần kiểm tra</param>
    /// <returns>Kết quả validation</returns>
    public ValidationResult ValidatePaymentMethod(string paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            return ValidationResult.Failure("Phương thức thanh toán không được để trống");
        }

        if (!WalletConstants.VALID_PAYMENT_METHODS.Contains(paymentMethod.ToUpper()))
        {
            return ValidationResult.Failure($"Phương thức thanh toán không hợp lệ. Các phương thức hợp lệ: {string.Join(", ", WalletConstants.VALID_PAYMENT_METHODS)}");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Kiểm tra mã giao dịch có hợp lệ không
    /// </summary>
    /// <param name="transactionId">Mã giao dịch cần kiểm tra</param>
    /// <returns>Kết quả validation</returns>
    public ValidationResult ValidateTransactionId(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return ValidationResult.Failure("Mã giao dịch không được để trống");
        }

        if (transactionId.Length < WalletConstants.MIN_TRANSACTION_ID_LENGTH)
        {
            return ValidationResult.Failure($"Mã giao dịch phải có ít nhất {WalletConstants.MIN_TRANSACTION_ID_LENGTH} ký tự");
        }

        if (transactionId.Length > WalletConstants.MAX_TRANSACTION_ID_LENGTH)
        {
            return ValidationResult.Failure($"Mã giao dịch không được vượt quá {WalletConstants.MAX_TRANSACTION_ID_LENGTH} ký tự");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Kiểm tra mô tả giao dịch có hợp lệ không
    /// </summary>
    /// <param name="description">Mô tả cần kiểm tra</param>
    /// <returns>Kết quả validation</returns>
    public ValidationResult ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return ValidationResult.Failure("Mô tả giao dịch không được để trống");
        }

        if (description.Length > WalletConstants.MAX_DESCRIPTION_LENGTH)
        {
            return ValidationResult.Failure($"Mô tả giao dịch không được vượt quá {WalletConstants.MAX_DESCRIPTION_LENGTH} ký tự");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Kiểm tra thông tin giao dịch có hợp lệ không
    /// </summary>
    /// <param name="amount">Số tiền</param>
    /// <param name="paymentMethod">Phương thức thanh toán</param>
    /// <param name="transactionId">Mã giao dịch</param>
    /// <param name="description">Mô tả giao dịch</param>
    /// <returns>Kết quả validation</returns>
    public ValidationResult ValidateTransaction(decimal amount, string paymentMethod, string transactionId, string description)
    {
        var results = new List<ValidationResult>
        {
            ValidateAmount(amount),
            ValidatePaymentMethod(paymentMethod),
            ValidateTransactionId(transactionId),
            ValidateDescription(description)
        };

        var failedResults = results.Where(r => !r.IsValid).ToList();
        if (failedResults.Any())
        {
            var errors = failedResults.SelectMany(r => r.Errors).ToList();
            return ValidationResult.Failure(errors);
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Validation result for business logic operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new List<string>();

    private ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);

    public void Combine(ValidationResult other)
    {
        if (!other.IsValid)
        {
            IsValid = false;
            Errors.AddRange(other.Errors);
        }
    }
}
