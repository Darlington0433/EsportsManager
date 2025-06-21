// Models cho Business Logic Layer

using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.BL.Models;

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

    /// <summary>
    /// Tạo kết quả thất bại với một thông báo lỗi
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
    /// Tạo kết quả thất bại với danh sách các lỗi
    /// </summary>
    public static BusinessResult<T> Failure(List<string> errors)
    {
        return new BusinessResult<T>
        {
            IsSuccess = false,
            Errors = errors,
            ErrorMessage = string.Join("; ", errors) // Gộp tất cả lỗi thành một chuỗi
        };
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
    /// Tạo kết quả thất bại không có dữ liệu
    /// </summary>
    public static BusinessResult Failure(string errorMessage)
    {
        return new BusinessResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Errors = new List<string> { errorMessage }
        };
    }

    /// <summary>    /// Tạo kết quả thất bại với danh sách các lỗi
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
/// Chứa thông tin về việc đăng nhập thành công hay thất bại
/// Nếu thành công sẽ có thông tin user, nếu thất bại sẽ có thông báo lỗi
/// </summary>
public class AuthenticationResult
{
    
    
    /// <summary>
    /// Cho biết có xác thực thành công hay không
    /// </summary>
    public bool IsAuthenticated { get; set; }
    
    /// <summary>
    /// ID của user nếu xác thực thành công
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// Tên đăng nhập nếu xác thực thành công
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Vai trò của user nếu xác thực thành công
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Thông báo lỗi nếu xác thực thất bại
    /// </summary>
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
/// Sử dụng để kiểm tra tính hợp lệ của dữ liệu đầu vào
/// Có thể chứa nhiều lỗi validation cùng lúc
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
    /// Tạo kết quả validation thành công
    /// </summary>

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Tạo kết quả validation thất bại với nhiều lỗi
    /// </summary>
    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList() // Chuyển array thành List
        };
    }

    /// <summary>
    /// Tạo kết quả validation thất bại với danh sách lỗi
    /// </summary>

    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
    }
    
    
}
