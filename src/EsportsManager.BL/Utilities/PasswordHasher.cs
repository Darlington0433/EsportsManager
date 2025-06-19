using BCrypt.Net;
using System.Linq;
using System;

namespace EsportsManager.BL.Utilities;

/// <summary>
/// PasswordHasher - Utility class để hash và verify mật khẩu
/// Áp dụng Single Responsibility Principle - chỉ lo về việc hash và verify password
/// Sử dụng BCrypt để đảm bảo security cao (salt + hash)
/// BCrypt tự động generate salt và resist rainbow table attacks
/// </summary>
public static class PasswordHasher
{
    #region Password Hashing - Mã hóa mật khẩu
    
    /// <summary>
    /// Hash mật khẩu sử dụng BCrypt algorithm
    /// BCrypt tự động generate salt và combine với password để tạo hash an toàn
    /// Mỗi lần hash cùng một password sẽ cho kết quả khác nhau (do salt khác nhau)
    /// </summary>
    /// <param name="password">Mật khẩu dạng plain text cần hash</param>
    /// <returns>Mật khẩu đã được hash (bao gồm cả salt)</returns>
    /// <exception cref="ArgumentException">Ném ra khi password null hoặc empty</exception>
    public static string HashPassword(string password)
    {
        // Validate input: password không được null hoặc empty
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        // Sử dụng BCrypt để hash password với salt được generate tự động
        // GenerateSalt() tạo salt ngẫu nhiên để đảm bảo mỗi hash là unique
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }
    
    #endregion

    #region Password Verification - Xác minh mật khẩu
    
    /// <summary>
    /// Verify mật khẩu plain text với hash đã lưu trong database
    /// BCrypt sẽ extract salt từ hash và dùng nó để hash password input
    /// Sau đó so sánh kết quả với hash gốc
    /// </summary>
    /// <param name="password">Mật khẩu plain text người dùng nhập</param>
    /// <param name="hash">Hash mật khẩu đã lưu trong database</param>
    /// <returns>True nếu password khớp với hash, False nếu không khớp hoặc có lỗi</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        // Validate input: cả password và hash đều phải có giá trị
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            // Sử dụng BCrypt.Verify để so sánh password với hash
            // BCrypt sẽ tự động extract salt từ hash và thực hiện verification
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Trả về false nếu có bất kỳ exception nào (hash không hợp lệ, etc.)
            return false;
        }
    }
    
    #endregion

    #region Password Strength Validation - Kiểm tra độ mạnh mật khẩu
    
    /// <summary>
    /// Kiểm tra mật khẩu có đủ mạnh theo các tiêu chí security
    /// Yêu cầu mật khẩu phải có:
    /// - Ít nhất 8 ký tự
    /// - Ít nhất 1 chữ hoa
    /// - Ít nhất 1 chữ thường  
    /// - Ít nhất 1 số
    /// - Ít nhất 1 ký tự đặc biệt
    /// </summary>
    /// <param name="password">Mật khẩu cần kiểm tra</param>
    /// <returns>True nếu mật khẩu đủ mạnh, False nếu không đủ mạnh</returns>
    public static bool IsPasswordStrong(string password)
    {
        // Validate input: password không được null hoặc empty
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Kiểm tra độ dài tối thiểu (8 ký tự)
        if (password.Length < 8)
            return false;

        // Kiểm tra phải có ít nhất 1 chữ hoa (A-Z)
        if (!password.Any(char.IsUpper))
            return false;

        // Kiểm tra phải có ít nhất 1 chữ thường (a-z)
        if (!password.Any(char.IsLower))
            return false;

        // Kiểm tra phải có ít nhất 1 số (0-9)
        if (!password.Any(char.IsDigit))
            return false;

        // Định nghĩa các ký tự đặc biệt được chấp nhận
        var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        
        // Kiểm tra phải có ít nhất 1 ký tự đặc biệt
        if (!password.Any(c => specialChars.Contains(c)))
            return false;

        // Tất cả điều kiện đều thỏa mãn
        return true;
    }

    /// <summary>
    /// Lấy thông báo yêu cầu về độ mạnh mật khẩu
    /// Sử dụng để hiển thị cho người dùng khi mật khẩu không đủ mạnh
    /// </summary>
    /// <returns>Chuỗi mô tả các yêu cầu về mật khẩu</returns>
    public static string GetPasswordRequirements()
    {
        return "Password must be at least 8 characters long and contain at least:\n" +
               "- One uppercase letter (A-Z)\n" +
               "- One lowercase letter (a-z)\n" +
               "- One digit (0-9)\n" +
               "- One special character (!@#$%^&*()_+-=[]{}|;:,.<>?)";
    }
    
    #endregion
}
