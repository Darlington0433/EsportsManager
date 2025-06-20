using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Models;

/// <summary>
/// Business result wrapper - áp dụng Single Responsibility Principle
/// Chỉ lo về việc wrap kết quả và error handling
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class BusinessResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static BusinessResult<T> Success(T data)
    {
        return new BusinessResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static BusinessResult<T> Failure(string errorMessage)
    {
        return new BusinessResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    public static BusinessResult<T> Failure(List<string> errors)
    {
        return new BusinessResult<T>
        {
            IsSuccess = false,
            Errors = errors,
            ErrorMessage = string.Join("; ", errors)
        };
    }
}

/// <summary>
/// Business result without data
/// </summary>
public class BusinessResult : BusinessResult<object>
{
    public static BusinessResult Success()
    {
        return new BusinessResult
        {
            IsSuccess = true
        };
    }

    public new static BusinessResult Failure(string errorMessage)
    {
        return new BusinessResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    public new static BusinessResult Failure(List<string> errors)
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
/// Authentication result
/// </summary>
public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }

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
/// Validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
    }
}
