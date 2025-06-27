using EsportsManager.BL.Constants;
using EsportsManager.BL.DTOs;

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
        if (string.IsNullOrEmpty(input?.Trim()))
        {
            return (false, "Số tiền không được để trống!", 0);
        }

        if (!decimal.TryParse(input, out decimal amount))
        {
            return (false, "Số tiền phải là số hợp lệ!", 0);
        }

        if (amount <= 0)
        {
            return (false, "Số tiền rút phải lớn hơn 0!", 0);
        }

        if (amount < WalletConstants.MIN_WITHDRAWAL_AMOUNT)
        {
            return (false, $"Số tiền rút tối thiểu là {WalletConstants.MIN_WITHDRAWAL_AMOUNT:N0} VND!", 0);
        }

        if (amount > currentBalance)
        {
            return (false, "Số dư không đủ để thực hiện giao dịch!", 0);
        }

        return (true, string.Empty, amount);
    }

    /// <summary>
    /// Validates withdrawal request parameters
    /// </summary>
    public ValidationResult ValidateWithdrawalRequest(decimal amount, decimal currentBalance)
    {
        if (amount <= 0)
        {
            return ValidationResult.Failure("Số tiền rút phải lớn hơn 0.");
        }

        if (amount < WalletConstants.MIN_WITHDRAWAL_AMOUNT)
        {
            return ValidationResult.Failure($"Số tiền rút tối thiểu là {WalletConstants.MIN_WITHDRAWAL_AMOUNT:N0} VND.");
        }

        if (amount > currentBalance)
        {
            return ValidationResult.Failure("Số dư không đủ để thực hiện giao dịch.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates top-up request parameters
    /// </summary>
    public ValidationResult ValidateTopUpRequest(decimal amount)
    {
        if (amount <= 0)
        {
            return ValidationResult.Failure("Số tiền nạp phải lớn hơn 0.");
        }

        if (amount < WalletConstants.MIN_TOP_UP_AMOUNT || amount > WalletConstants.MAX_TOP_UP_AMOUNT)
        {
            return ValidationResult.Failure($"Số tiền nạp phải từ {WalletConstants.MIN_TOP_UP_AMOUNT:N0} đến {WalletConstants.MAX_TOP_UP_AMOUNT:N0} VND.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates donation request parameters
    /// </summary>
    public ValidationResult ValidateDonationRequest(decimal amount, decimal? currentBalance)
    {
        if (amount <= 0)
        {
            return ValidationResult.Failure("Số tiền donation phải lớn hơn 0.");
        }

        if (amount < WalletConstants.MIN_DONATION_AMOUNT)
        {
            return ValidationResult.Failure($"Số tiền donation tối thiểu là {WalletConstants.MIN_DONATION_AMOUNT:N0} VND.");
        }

        if (currentBalance.HasValue && amount > currentBalance.Value)
        {
            return ValidationResult.Failure("Số dư ví không đủ.");
        }

        return ValidationResult.Success();
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
}

/// <summary>
/// Validation result for business logic operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}
