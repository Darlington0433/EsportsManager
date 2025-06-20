// Unified Input Service - Thay thế tất cả duplicate input logic
using System;
using System.Text;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// Service thống nhất cho tất cả input operations
/// </summary>
public static class UnifiedInputService
{
    /// <summary>
    /// Đọc text input với validation tùy chỉnh
    /// </summary>
    public static string? ReadText(int maxLength = 100, Func<char, bool>? validator = null)
    {
        var input = new StringBuilder();
        
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.Enter)
            {
                return input.ToString();
            }
            
            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }
            
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Length--;
                Console.Write("\b \b");
                continue;
            }
            
            if (input.Length >= maxLength) continue;
            
            if (validator == null || validator(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }
    
    /// <summary>
    /// Đọc password (hiển thị dấu *)
    /// </summary>
    public static string? ReadPassword(int maxLength = 100)
    {
        var password = new StringBuilder();
        
        while (true)
        {
            var key = Console.ReadKey(true);
            
            if (key.Key == ConsoleKey.Enter)
            {
                return password.ToString();
            }
            
            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }
            
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
                continue;
            }
            
            if (password.Length >= maxLength) continue;
            
            if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
    }
    
    /// <summary>
    /// Đọc username (chỉ chữ, số, _)
    /// </summary>
    public static string? ReadUsername(int maxLength = 50)
    {
        return ReadText(maxLength, c => char.IsLetterOrDigit(c) || c == '_');
    }
    
    /// <summary>
    /// Đọc email
    /// </summary>
    public static string? ReadEmail(int maxLength = 255)
    {
        return ReadText(maxLength, c => char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-');
    }
}
