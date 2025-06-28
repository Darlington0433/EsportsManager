// Models cho Business Logic Layer

using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Models;

/// <summary>
/// BusinessResult - Lớp wrapper cho kết quả xử lý business logic có dữ liệu trả về
/// Sử dụng generic để có thể trả về bất kỳ kiểu dữ liệu nào
/// </summary>
public class BusinessResult<T>
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Tạo kết quả thành công với dữ liệu
    /// </summary>
    public static BusinessResult<T> Success(T data)
    {
        return new BusinessResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại với thông báo lỗi
    /// </summary>
    public static BusinessResult<T> Failure(string errorMessage)
    {
        return new BusinessResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại với danh sách lỗi
    /// </summary>
    public static BusinessResult<T> Failure(List<string> errors)
    {
        return new BusinessResult<T>
        {
            IsSuccess = false,
            Errors = errors,
            ErrorMessage = string.Join("; ", errors)
        };
    }

    /// <summary>
    /// Thêm chi tiết database error cho kết quả lỗi
    /// </summary>
    public BusinessResult<T> WithDatabaseError(string details)
    {
        if (!IsSuccess)
        {
            ErrorMessage = $"Database Error: {ErrorMessage}. Details: {details}";
            ErrorCode = "DB_ERROR";
        }
        return this;
    }

    /// <summary>
    /// Thêm chi tiết validation error cho kết quả lỗi
    /// </summary>
    public BusinessResult<T> WithValidationError(string field, string details)
    {
        if (!IsSuccess)
        {
            ErrorMessage = $"Validation Error in {field}: {details}";
            ErrorCode = "VALIDATION_ERROR";
        }
        return this;
    }

    /// <summary>
    /// Thêm chi tiết security error cho kết quả lỗi
    /// </summary> 
    public BusinessResult<T> WithSecurityError()
    {
        if (!IsSuccess)
        {
            ErrorMessage = $"Security Error: {ErrorMessage}";
            ErrorCode = "SECURITY_ERROR";
        }
        return this;
    }
}

/// <summary>
/// BusinessResult - Lớp wrapper cho kết quả xử lý business logic không có dữ liệu trả về
/// Standalone class để tránh circular dependency
/// Sử dụng cho các operation chỉ cần biết thành công/thất bại
/// </summary>
public class BusinessResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Tạo kết quả thành công không có dữ liệu
    /// </summary>
    public static BusinessResult Success()
    {
        return new BusinessResult
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại với thông báo lỗi
    /// </summary>
    public static BusinessResult Failure(string errorMessage)
    {
        return new BusinessResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại với danh sách lỗi
    /// </summary>
    public static BusinessResult Failure(List<string> errors)
    {
        return new BusinessResult
        {
            IsSuccess = false,
            Errors = errors,
            ErrorMessage = string.Join("; ", errors)
        };
    }
}

/// <summary>
/// AuthenticationResult - Kết quả xác thực người dùng
/// Chứa thông tin cơ bản của user sau khi xác thực thành công
/// </summary>
public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tạo kết quả xác thực thành công
    /// </summary>
    public static AuthenticationResult Success(int userId, string username, string role)
    {
        return new AuthenticationResult
        {
            IsAuthenticated = true,
            UserId = userId,
            Username = username,
            Role = role
        };
    }

    /// <summary>
    /// Tạo kết quả xác thực thất bại
    /// </summary>
    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsAuthenticated = false,
            ErrorMessage = errorMessage
        };
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
    public bool IsValid { get; set; }

    /// <summary>
    /// Danh sách các lỗi validation
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Thông báo lỗi tổng hợp
    /// </summary>
    public string ErrorMessage => string.Join("; ", Errors);

    /// <summary>
    /// Tạo kết quả validation thành công
    /// </summary>
    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Tạo kết quả validation thất bại với một lỗi
    /// </summary>
    public static ValidationResult Failure(string error)
    {
        return new ValidationResult 
        { 
            IsValid = false,
            Errors = new List<string> { error }
        };
    }

    /// <summary>
    /// Tạo kết quả validation thất bại với nhiều lỗi
    /// </summary>
    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = new List<string>(errors)
        };
    }

    /// <summary>
    /// Thêm lỗi vào kết quả validation
    /// </summary>
    public ValidationResult AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
        return this;
    }

    /// <summary>
    /// Thêm nhiều lỗi vào kết quả validation
    /// </summary>
    public ValidationResult AddErrors(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        IsValid = false;
        return this;
    }

    /// <summary>
    /// Kết hợp hai kết quả validation
    /// </summary>
    public ValidationResult Combine(ValidationResult other)
    {
        if (!other.IsValid)
        {
            IsValid = false;
            Errors.AddRange(other.Errors);
        }
        return this;
    }
}
