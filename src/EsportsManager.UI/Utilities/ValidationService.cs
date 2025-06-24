// Validation Service - Gom tất cả logic validation
using System;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// Service thống nhất cho tất cả validation operations
/// </summary>
public static class ValidationService
{
    /// <summary>
    /// Validate các field bắt buộc không được để trống
    /// </summary>
    /// <param name="fieldValues">Giá trị các field</param>
    /// <param name="fieldLabels">Tên các field</param>
    /// <param name="messageCallback">Callback để hiển thị lỗi</param>
    /// <returns>true nếu hợp lệ</returns>
    public static bool ValidateRequiredFields(string[] fieldValues, string[] fieldLabels, Action<string, bool> messageCallback)
    {
        for (int i = 0; i < fieldValues.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(fieldValues[i]))
            {
                messageCallback($"Vui lòng nhập {fieldLabels[i].ToLower()}", true);
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Validate password confirmation match
    /// </summary>
    /// <param name="password">Password chính</param>
    /// <param name="confirmPassword">Password xác nhận</param>
    /// <param name="messageCallback">Callback để hiển thị lỗi</param>
    /// <returns>true nếu khớp</returns>
    public static bool ValidatePasswordMatch(string password, string confirmPassword, Action<string, bool> messageCallback)
    {
        if (password != confirmPassword)
        {
            messageCallback("Mật khẩu xác nhận không khớp", true);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate email format
    /// </summary>
    /// <param name="email">Email cần validate</param>
    /// <param name="messageCallback">Callback để hiển thị lỗi</param>
    /// <returns>true nếu hợp lệ</returns>
    public static bool ValidateEmail(string email, Action<string, bool> messageCallback)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            messageCallback("Email không hợp lệ", true);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate username format (chỉ chữ, số, _)
    /// </summary>
    /// <param name="username">Username cần validate</param>
    /// <param name="messageCallback">Callback để hiển thị lỗi</param>
    /// <returns>true nếu hợp lệ</returns>
    public static bool ValidateUsername(string username, Action<string, bool> messageCallback)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            messageCallback("Tên đăng nhập không được để trống", true);
            return false;
        }

        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c) && c != '_')
            {
                messageCallback("Tên đăng nhập chỉ được chứa chữ, số và dấu _", true);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validate password strength
    /// </summary>
    /// <param name="password">Password cần validate</param>
    /// <param name="messageCallback">Callback để hiển thị lỗi</param>
    /// <returns>true nếu hợp lệ</returns>
    public static bool ValidatePasswordStrength(string password, Action<string, bool> messageCallback)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            messageCallback("Mật khẩu không được để trống", true);
            return false;
        }

        if (password.Length < 6)
        {
            messageCallback("Mật khẩu phải có ít nhất 6 ký tự", true);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của input để phòng chống SQL injection
    /// </summary>
    /// <param name="input">Chuỗi input cần kiểm tra</param>
    /// <returns>true nếu input không chứa ký tự đáng ngờ</returns>
    public static bool ValidateAgainstSqlInjection(string input)
    {
        if (string.IsNullOrEmpty(input))
            return true;

        // Kiểm tra các ký tự đáng ngờ có thể dùng để SQL injection
        string[] suspiciousPatterns = {
            "DROP TABLE",
            "DELETE FROM",
            "INSERT INTO",
            "UPDATE",
            "ALTER TABLE",
            "EXEC(",
            "EXECUTE(",
            "--",
            "/*",
            "*/",
            ";",
            "' OR '1'='1",
            "' OR 1=1",
            "1=1;--",
            "' OR ''='",
            "OR 1=1"
        };

        foreach (string pattern in suspiciousPatterns)
        {
            if (input.ToUpper().Contains(pattern.ToUpper()))
                return false;
        }

        return true;
    }
}
