using System.Text.RegularExpressions;
using EsportsManager.BL.Models;
using System.Linq;
using System.Collections.Generic;

namespace EsportsManager.BL.Utilities;

/// <summary>
/// InputValidator - Utility class để validation các dữ liệu đầu vào
/// Áp dụng Single Responsibility Principle - chỉ lo về việc validate input data
/// Sử dụng static methods để có thể gọi trực tiếp mà không cần tạo instance
/// Trả về ValidationResult để có thể handle multiple errors
/// </summary>
public static class InputValidator
{
    #region Username Validation - Validation tên đăng nhập
    
    /// <summary>
    /// Validate username theo các quy tắc:
    /// - Bắt buộc phải có
    /// - Ít nhất 3 ký tự, tối đa 50 ký tự
    /// - Chỉ được chứa chữ cái, số và dấu gạch dưới
    /// - Không được bắt đầu hoặc kết thúc bằng dấu gạch dưới
    /// </summary>
    /// <param name="username">Tên đăng nhập cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateUsername(string username)
    {
        var errors = new List<string>();

        // Kiểm tra null hoặc empty
        if (string.IsNullOrWhiteSpace(username))
        {
            errors.Add("Username is required");
        }
        else
        {
            // Kiểm tra độ dài tối thiểu
            if (username.Length < 3)
                errors.Add("Username must be at least 3 characters long");

            // Kiểm tra độ dài tối đa
            if (username.Length > 50)
                errors.Add("Username cannot exceed 50 characters");

            // Kiểm tra format: chỉ cho phép chữ cái, số và dấu gạch dưới
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                errors.Add("Username can only contain letters, numbers, and underscores");

            // Kiểm tra không được bắt đầu hoặc kết thúc bằng dấu gạch dưới
            if (username.StartsWith("_") || username.EndsWith("_"))
                errors.Add("Username cannot start or end with underscore");
        }

        // Trả về kết quả: thành công nếu không có lỗi, thất bại nếu có lỗi
        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
    
    #endregion

    #region Email Validation - Validation email
    
    /// <summary>
    /// Validate email theo các quy tắc:
    /// - Không bắt buộc (có thể null/empty)
    /// - Nếu có thì phải đúng format email
    /// - Tối đa 255 ký tự
    /// </summary>
    /// <param name="email">Email cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateEmail(string? email)
    {
        var errors = new List<string>();

        // Email không bắt buộc, chỉ validate nếu có giá trị
        if (!string.IsNullOrWhiteSpace(email))
        {
            // Kiểm tra format email cơ bản: phải có @ và domain
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email format");

            // Kiểm tra độ dài tối đa
            if (email.Length > 255)
                errors.Add("Email cannot exceed 255 characters");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
    
    #endregion

    #region Password Validation - Validation mật khẩu
    
    /// <summary>
    /// Validate password sử dụng PasswordHasher để kiểm tra độ mạnh
    /// Các quy tắc được định nghĩa trong PasswordHasher.IsPasswordStrong()
    /// </summary>
    /// <param name="password">Mật khẩu cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidatePassword(string password)
    {
        var errors = new List<string>();

        // Kiểm tra null hoặc empty
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
        }
        else
        {
            // Sử dụng PasswordHasher để kiểm tra độ mạnh của password
            if (!PasswordHasher.IsPasswordStrong(password))
            {
                // Lấy yêu cầu mật khẩu từ PasswordHasher
                errors.Add(PasswordHasher.GetPasswordRequirements());
            }
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate password confirmation (xác nhận mật khẩu)
    /// Kiểm tra password confirmation có khớp với password gốc không
    /// </summary>
    /// <param name="password">Mật khẩu gốc</param>
    /// <param name="confirmPassword">Mật khẩu xác nhận</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidatePasswordConfirmation(string password, string confirmPassword)
    {
        var errors = new List<string>();

        // Kiểm tra confirmation không được để trống
        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            errors.Add("Password confirmation is required");
        }
        // Kiểm tra confirmation có khớp với password gốc không
        else if (password != confirmPassword)
        {
            errors.Add("Passwords do not match");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
    
    #endregion

    #region Role and Status Validation - Validation vai trò và trạng thái
    
    /// <summary>
    /// Validate user role theo danh sách các role hợp lệ
    /// Các role hợp lệ: Admin, Player, Viewer
    /// </summary>
    /// <param name="role">Vai trò cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateRole(string role)
    {
        var errors = new List<string>();
        
        // Danh sách các role hợp lệ
        var validRoles = new[] { "Admin", "Player", "Viewer" };

        // Kiểm tra role không được để trống
        if (string.IsNullOrWhiteSpace(role))
        {
            errors.Add("Role is required");
        }
        // Kiểm tra role có nằm trong danh sách hợp lệ không
        else if (!validRoles.Contains(role))
        {
            errors.Add($"Invalid role. Valid roles are: {string.Join(", ", validRoles)}");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate user status theo danh sách các status hợp lệ
    /// Các status hợp lệ: Active, Suspended, Inactive
    /// </summary>
    /// <param name="status">Trạng thái cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateStatus(string status)
    {
        var errors = new List<string>();
        
        // Danh sách các status hợp lệ
        var validStatuses = new[] { "Active", "Suspended", "Inactive" };

        // Kiểm tra status không được để trống
        if (string.IsNullOrWhiteSpace(status))
        {
            errors.Add("Status is required");
        }
        // Kiểm tra status có nằm trong danh sách hợp lệ không
        else if (!validStatuses.Contains(status))
        {
            errors.Add($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
    
    #endregion

    #region General Validation - Validation chung
    
    /// <summary>
    /// Validate User ID - phải là số nguyên dương
    /// </summary>
    /// <param name="userId">User ID cần validate</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateUserId(int userId)
    {
        var errors = new List<string>();

        // User ID phải là số nguyên dương
        if (userId <= 0)
        {
            errors.Add("User ID must be a positive integer");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Validate độ dài chuỗi theo min/max length
    /// Có thể sử dụng cho bất kỳ trường nào cần kiểm tra độ dài
    /// </summary>
    /// <param name="value">Giá trị cần validate</param>
    /// <param name="fieldName">Tên trường (để hiển thị trong thông báo lỗi)</param>
    /// <param name="minLength">Độ dài tối thiểu (mặc định 0)</param>
    /// <param name="maxLength">Độ dài tối đa (mặc định không giới hạn)</param>
    /// <returns>ValidationResult chứa kết quả và danh sách lỗi (nếu có)</returns>
    public static ValidationResult ValidateStringLength(string value, string fieldName, int minLength = 0, int maxLength = int.MaxValue)
    {
        var errors = new List<string>();

        // Kiểm tra null hoặc empty
        if (string.IsNullOrWhiteSpace(value))
        {
            // Chỉ báo lỗi required nếu minLength > 0
            if (minLength > 0)
                errors.Add($"{fieldName} is required");
        }
        else
        {
            // Kiểm tra độ dài tối thiểu
            if (value.Length < minLength)
                errors.Add($"{fieldName} must be at least {minLength} characters long");

            // Kiểm tra độ dài tối đa
            if (value.Length > maxLength)
                errors.Add($"{fieldName} cannot exceed {maxLength} characters");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    /// <summary>
    /// Combine nhiều ValidationResult thành một kết quả duy nhất
    /// Sử dụng để validate nhiều trường cùng lúc và gom tất cả lỗi lại
    /// </summary>
    /// <param name="results">Danh sách các ValidationResult cần combine</param>
    /// <returns>ValidationResult tổng hợp tất cả lỗi</returns>
    public static ValidationResult CombineResults(params ValidationResult[] results)
    {
        var allErrors = new List<string>();

        // Duyệt qua tất cả các kết quả validation
        foreach (var result in results)
        {
            // Nếu kết quả không hợp lệ, thêm lỗi vào danh sách tổng
            if (!result.IsValid)
            {
                allErrors.AddRange(result.Errors);
            }
        }

        // Trả về kết quả tổng hợp
        return allErrors.Any() ? ValidationResult.Failure(allErrors) : ValidationResult.Success();
    }
    
    #endregion
}
