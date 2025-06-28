// Models cho Business Logic Layer

using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Models;

/// <summary>
/// Kết quả trả về từ business layer
/// </summary>
public class BusinessResult
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public string? ErrorCode { get; }

    protected BusinessResult(bool isSuccess, string? message = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorCode = errorCode;
    }

    public static BusinessResult Success(string? message = null)
    {
        return new BusinessResult(true, message);
    }

    public static BusinessResult Failure(string message, string? errorCode = null)
    {
        return new BusinessResult(false, message, errorCode);
    }

    public static BusinessResult WithDatabaseError(string message)
    {
        return new BusinessResult(false, message, "DB_ERROR");
    }

    public static BusinessResult WithValidationError(string message)
    {
        return new BusinessResult(false, message, "VALIDATION_ERROR");
    }
}

/// <summary>
/// Kết quả trả về từ business layer với dữ liệu
/// </summary>
public class BusinessResult<T> : BusinessResult
{
    public T? Data { get; }

    private BusinessResult(bool isSuccess, T? data, string? message = null, string? errorCode = null)
        : base(isSuccess, message, errorCode)
    {
        Data = data;
    }

    public static BusinessResult<T> Success(T data, string? message = null)
    {
        return new BusinessResult<T>(true, data, message);
    }

    public new static BusinessResult<T> Failure(string message, string? errorCode = null)
    {
        return new BusinessResult<T>(false, default, message, errorCode);
    }

    public new static BusinessResult<T> WithDatabaseError(string message)
    {
        return new BusinessResult<T>(false, default, message, "DB_ERROR");
    }

    public new static BusinessResult<T> WithValidationError(string message)
    {
        return new BusinessResult<T>(false, default, message, "VALIDATION_ERROR");
    }
}

/// <summary>
/// Kết quả trả về từ quá trình xác thực
/// </summary>
public class AuthenticationResult : BusinessResult
{
    public int? UserId { get; }
    public string? Username { get; }
    public string? Role { get; }

    private AuthenticationResult(bool isSuccess, int? userId = null, string? username = null, string? role = null, string? message = null)
        : base(isSuccess, message)
    {
        UserId = userId;
        Username = username;
        Role = role;
    }

    public static AuthenticationResult Success(int userId, string username, string role)
    {
        return new AuthenticationResult(true, userId, username, role);
    }

    public new static AuthenticationResult Failure(string message)
    {
        return new AuthenticationResult(false, message: message);
    }
}

/// <summary>
/// ValidationResult - Kết quả validation dữ liệu
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Cho biết dữ liệu có hợp lệ hay không
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Danh sách các lỗi validation
    /// </summary>
    private readonly List<string> _errors;
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Thông báo lỗi tổng hợp
    /// </summary>
    public string ErrorMessage => string.Join("; ", _errors);

    /// <summary>
    /// Constructor với trạng thái và danh sách lỗi
    /// </summary>
    private ValidationResult(bool isValid, IEnumerable<string> errors = null)
    {
        IsValid = isValid;
        _errors = errors?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Tạo kết quả validation thành công
    /// </summary>
    public static ValidationResult Success() => new ValidationResult(true);

    /// <summary>
    /// Tạo kết quả validation thất bại với một lỗi
    /// </summary>
    public static ValidationResult Failure(string error) 
        => new ValidationResult(false, new[] { error });

    /// <summary>
    /// Tạo kết quả validation thất bại với nhiều lỗi
    /// </summary>
    public static ValidationResult Failure(IEnumerable<string> errors) 
        => new ValidationResult(false, errors);

    /// <summary>
    /// Kết hợp với một kết quả validation khác
    /// </summary>
    public ValidationResult CombineWith(ValidationResult other)
    {
        if (other == null) return this;

        var combinedErrors = new List<string>(_errors);
        if (!other.IsValid)
        {
            combinedErrors.AddRange(other.Errors);
        }

        return new ValidationResult(IsValid && other.IsValid, combinedErrors);
    }
}
