using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EsportsManager.BL.Constants;

namespace EsportsManager.BL.Utilities;

/// <summary>
/// Helper class chứa các phương thức validation chung
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Regex patterns cho validation
    /// </summary>
    private static class Patterns
    {
        public const string EMAIL = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public const string USERNAME = @"^[a-zA-Z0-9_-]+$";
        public const string PHONE = @"^(0|\+84)[0-9]{9}$";
        public const string PASSWORD = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        public const string URL = @"^(http|https)://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(?:/[^/]*)*$";
    }

    /// <summary>
    /// Kiểm tra chuỗi có null hoặc empty không
    /// </summary>
    public static bool IsNullOrEmpty(string? value, string fieldName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(string.Format(AppConstants.Validation.REQUIRED_FIELD, fieldName));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Kiểm tra độ dài chuỗi
    /// </summary>
    public static bool ValidateLength(string value, string fieldName, int minLength, int maxLength, List<string> errors)
    {
        if (value.Length < minLength || value.Length > maxLength)
        {
            errors.Add(string.Format(AppConstants.Validation.INVALID_LENGTH, fieldName, minLength, maxLength));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra email hợp lệ
    /// </summary>
    public static bool IsValidEmail(string email, List<string> errors)
    {
        if (!Regex.IsMatch(email, Patterns.EMAIL))
        {
            errors.Add(string.Format(AppConstants.Validation.INVALID_FORMAT, "Email"));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra username hợp lệ
    /// </summary>
    public static bool IsValidUsername(string username, List<string> errors)
    {
        if (!Regex.IsMatch(username, Patterns.USERNAME))
        {
            errors.Add(string.Format(AppConstants.Validation.INVALID_FORMAT, "Username"));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra số điện thoại hợp lệ
    /// </summary>
    public static bool IsValidPhone(string phone, List<string> errors)
    {
        if (!Regex.IsMatch(phone, Patterns.PHONE))
        {
            errors.Add(string.Format(AppConstants.Validation.INVALID_FORMAT, "Số điện thoại"));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra số tiền hợp lệ
    /// </summary>
    public static bool IsValidAmount(decimal amount, decimal minAmount, decimal maxAmount, List<string> errors)
    {
        if (amount < minAmount || amount > maxAmount)
        {
            errors.Add($"Số tiền phải từ {minAmount.ToString(AppConstants.Currency.CURRENCY_FORMAT)} " +
                      $"đến {maxAmount.ToString(AppConstants.Currency.CURRENCY_FORMAT)} " +
                      AppConstants.Currency.CURRENCY_CODE);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra mật khẩu hợp lệ
    /// </summary>
    public static bool IsValidPassword(string password, List<string> errors)
    {
        if (!Regex.IsMatch(password, Patterns.PASSWORD))
        {
            errors.Add("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra URL hợp lệ
    /// </summary>
    public static bool IsValidUrl(string url, List<string> errors)
    {
        if (!Regex.IsMatch(url, Patterns.URL))
        {
            errors.Add(string.Format(AppConstants.Validation.INVALID_FORMAT, "URL"));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra ngày hợp lệ
    /// </summary>
    public static bool IsValidDate(DateTime date, DateTime? minDate, DateTime? maxDate, List<string> errors)
    {
        if (minDate.HasValue && date < minDate.Value)
        {
            errors.Add($"Ngày phải sau {minDate.Value:dd/MM/yyyy}");
            return false;
        }

        if (maxDate.HasValue && date > maxDate.Value)
        {
            errors.Add($"Ngày phải trước {maxDate.Value:dd/MM/yyyy}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Kiểm tra giá trị enum hợp lệ
    /// </summary>
    public static bool IsValidEnum<T>(string value, string fieldName, List<string> errors) where T : struct
    {
        if (!Enum.TryParse<T>(value, true, out _))
        {
            errors.Add($"{fieldName} không hợp lệ. Giá trị cho phép: {string.Join(", ", Enum.GetNames(typeof(T)))}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra giá trị nằm trong danh sách cho phép
    /// </summary>
    public static bool IsValidValue<T>(T value, IEnumerable<T> validValues, string fieldName, List<string> errors)
    {
        if (!validValues.Contains(value))
        {
            errors.Add($"{fieldName} không hợp lệ. Giá trị cho phép: {string.Join(", ", validValues)}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra file extension hợp lệ
    /// </summary>
    public static bool IsValidFileExtension(string fileName, string[] validExtensions, List<string> errors)
    {
        var extension = System.IO.Path.GetExtension(fileName)?.ToLower();
        if (string.IsNullOrEmpty(extension) || !validExtensions.Contains(extension))
        {
            errors.Add($"File không hợp lệ. Định dạng cho phép: {string.Join(", ", validExtensions)}");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra kích thước file hợp lệ (bytes)
    /// </summary>
    public static bool IsValidFileSize(long fileSize, long maxSize, List<string> errors)
    {
        if (fileSize > maxSize)
        {
            errors.Add($"Kích thước file không được vượt quá {maxSize / 1024 / 1024}MB");
            return false;
        }
        return true;
    }
} 