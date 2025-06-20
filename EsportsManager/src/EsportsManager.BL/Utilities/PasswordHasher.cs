using BCrypt.Net;
using System.Linq;
using System;

namespace EsportsManager.BL.Utilities;

/// <summary>
/// Password hasher utility - áp dụng Single Responsibility Principle
/// Chỉ lo về việc hash và verify password
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hash password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    /// <summary>
    /// Verify password against hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Hashed password</param>
    /// <returns>True if password matches</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if password meets complexity requirements
    /// </summary>
    /// <param name="password">Password to check</param>
    /// <returns>True if password is strong enough</returns>
    public static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // Minimum 8 characters
        if (password.Length < 8)
            return false;

        // Must contain at least one uppercase letter
        if (!password.Any(char.IsUpper))
            return false;

        // Must contain at least one lowercase letter
        if (!password.Any(char.IsLower))
            return false;

        // Must contain at least one digit
        if (!password.Any(char.IsDigit))
            return false;

        // Must contain at least one special character
        var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        if (!password.Any(c => specialChars.Contains(c)))
            return false;

        return true;
    }

    /// <summary>
    /// Get password strength requirements message
    /// </summary>
    /// <returns>Password requirements</returns>
    public static string GetPasswordRequirements()
    {
        return "Password must be at least 8 characters long and contain at least:\n" +
               "- One uppercase letter (A-Z)\n" +
               "- One lowercase letter (a-z)\n" +
               "- One digit (0-9)\n" +
               "- One special character (!@#$%^&*()_+-=[]{}|;:,.<>?)";
    }
}
