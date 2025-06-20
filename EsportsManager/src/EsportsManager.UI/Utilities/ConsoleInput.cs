using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// Console input helper - áp dụng Single Responsibility Principle
/// Chỉ lo về việc nhận input từ user
/// </summary>
public static class ConsoleInput
{
    /// <summary>
    /// Get string input with validation
    /// </summary>
    public static string GetString(string prompt, bool required = true, int maxLength = 255)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();

            if (required && string.IsNullOrWhiteSpace(input))
            {
                ConsoleHelper.ShowError("Trường này là bắt buộc. Vui lòng thử lại.");
                continue;
            }

            if (!string.IsNullOrEmpty(input) && input.Length > maxLength)
            {
                ConsoleHelper.ShowError($"Độ dài không được vượt quá {maxLength} ký tự. Vui lòng thử lại.");
                continue;
            }

            return input ?? string.Empty;
        }
    }

    /// <summary>
    /// Get string input with validation and default value
    /// </summary>
    public static string GetString(string prompt, string defaultValue, bool required = true, int maxLength = 255)
    {
        while (true)
        {
            Console.Write($"{prompt} [Mặc định: {defaultValue}]: ");
            var input = Console.ReadLine()?.Trim();

            // Use default value if input is empty
            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultValue ?? string.Empty;
            }

            if (required && string.IsNullOrWhiteSpace(input))
            {
                ConsoleHelper.ShowError("This field is required. Please try again.");
                continue;
            }

            if (!string.IsNullOrEmpty(input) && input.Length > maxLength)
            {
                ConsoleHelper.ShowError($"Input cannot exceed {maxLength} characters. Please try again.");
                continue;
            }

            return input;
        }
    }

    /// <summary>
    /// Get password input (hidden)
    /// </summary>
    public static string GetPassword(string prompt = "Password")
    {
        Console.Write($"{prompt}: ");
        var password = new StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }

        return password.ToString();
    }

    /// <summary>
    /// Get integer input with validation
    /// </summary>
    public static int GetInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out var value))
            {
                if (value >= min && value <= max)
                {
                    return value;
                }
                else
                {
                    ConsoleHelper.ShowError($"Vui lòng nhập số từ {min} đến {max}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError("Vui lòng nhập một số hợp lệ.");
            }
        }
    }

    /// <summary>
    /// Get yes/no confirmation
    /// </summary>
    public static bool GetConfirmation(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = Console.ReadKey(true);
            Console.WriteLine();

            switch (input.Key)
            {
                case ConsoleKey.Y:
                    return true;
                case ConsoleKey.N:
                    return false;
                default:
                    ConsoleHelper.ShowError("Vui lòng nhấn 'y' cho có hoặc 'n' cho không.");
                    break;
            }
        }
    }

    /// <summary>
    /// Get choice from menu options
    /// </summary>
    public static int GetChoice(string prompt, int minChoice, int maxChoice)
    {
        while (true)
        {
            Console.Write($"{prompt} ({minChoice}-{maxChoice}): ");
            var input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out var choice))
            {
                if (choice >= minChoice && choice <= maxChoice)
                {
                    return choice;
                }
                else
                {
                    ConsoleHelper.ShowError($"Vui lòng nhập số từ {minChoice} đến {maxChoice}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError("Vui lòng nhập một số hợp lệ.");
            }
        }
    }

    /// <summary>
    /// Get choice from string options
    /// </summary>
    public static string GetChoice(string prompt, params string[] options)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            var choice = GetChoice("Nhập lựa chọn của bạn", 1, options.Length);
            return options[choice - 1];
        }
    }

    /// <summary>
    /// Get email input with basic validation
    /// </summary>
    public static string GetEmail(string prompt = "Email", bool required = false)
    {
        while (true)
        {
            var email = GetString(prompt, required);

            if (string.IsNullOrWhiteSpace(email) && !required)
            {
                return email;
            }

            if (IsValidEmail(email))
            {
                return email;
            }

            ConsoleHelper.ShowError("Vui lòng nhập địa chỉ email hợp lệ.");
        }
    }

    /// <summary>
    /// Get username input with validation
    /// </summary>
    public static string GetUsername(string prompt = "Username")
    {
        while (true)
        {
            var username = GetString(prompt, true, 50);

            if (IsValidUsername(username))
            {
                return username;
            }

            ConsoleHelper.ShowError("Username can only contain letters, numbers, and underscores (3-50 characters).");
        }
    }

    /// <summary>
    /// Get multi-line input
    /// </summary>
    public static string GetMultilineString(string prompt, string endMarker = "END")
    {
        Console.WriteLine($"{prompt} (type '{endMarker}' on a new line to finish):");
        var lines = new List<string>();

        while (true)
        {
            var line = Console.ReadLine();
            if (line?.Trim().ToUpper() == endMarker.ToUpper())
            {
                break;
            }
            lines.Add(line ?? string.Empty);
        }

        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Wait for specific key
    /// </summary>
    public static void WaitForKey(ConsoleKey key, string message = "")
    {
        if (!string.IsNullOrEmpty(message))
        {
            Console.WriteLine(message);
        }

        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
        } while (keyInfo.Key != key);
    }

    /// <summary>
    /// Clear current line
    /// </summary>
    public static void ClearCurrentLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    /// <summary>
    /// Get date input
    /// </summary>
    public static DateTime GetDate(string prompt, DateTime? minDate = null, DateTime? maxDate = null)
    {
        while (true)
        {
            Console.Write($"{prompt} (dd/MM/yyyy): ");
            var input = Console.ReadLine()?.Trim();

            if (DateTime.TryParse(input, out var date))
            {
                if (minDate.HasValue && date < minDate.Value)
                {
                    ConsoleHelper.ShowError($"Ngày không được sớm hơn {minDate.Value:dd/MM/yyyy}.");
                    continue;
                }

                if (maxDate.HasValue && date > maxDate.Value)
                {
                    ConsoleHelper.ShowError($"Ngày không được muộn hơn {maxDate.Value:dd/MM/yyyy}.");
                    continue;
                }

                return date;
            }

            ConsoleHelper.ShowError("Vui lòng nhập ngày hợp lệ theo định dạng dd/MM/yyyy.");
        }
    }

    /// <summary>
    /// Get decimal input with validation
    /// </summary>
    public static decimal GetDecimal(string prompt, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                ConsoleHelper.ShowError("Trường này là bắt buộc. Vui lòng thử lại.");
                continue;
            }

            if (!decimal.TryParse(input, out decimal value))
            {
                ConsoleHelper.ShowError("Vui lòng nhập một số thập phân hợp lệ.");
                continue;
            }

            if (value < min)
            {
                ConsoleHelper.ShowError($"Giá trị phải ít nhất là {min}.");
                continue;
            }

            if (value > max)
            {
                ConsoleHelper.ShowError($"Giá trị không được vượt quá {max}.");
                continue;
            }

            return value;
        }
    }

    /// <summary>
    /// Get DateTime input with validation
    /// </summary>
    public static DateTime GetDateTime(string prompt, DateTime? min = null, DateTime? max = null)
    {
        min ??= DateTime.MinValue;
        max ??= DateTime.MaxValue;

        while (true)
        {
            Console.Write($"{prompt} (dd/MM/yyyy HH:mm): ");
            var input = Console.ReadLine()?.Trim();

            if (DateTime.TryParse(input, out var value))
            {
                if (value >= min && value <= max)
                {
                    return value;
                }
                else
                {
                    ConsoleHelper.ShowError($"Vui lòng nhập ngày giờ trong khoảng từ {min:dd/MM/yyyy HH:mm} đến {max:dd/MM/yyyy HH:mm}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError("Vui lòng nhập ngày giờ hợp lệ theo định dạng dd/MM/yyyy HH:mm.");
            }
        }
    }

    /// <summary>
    /// Get DateTime input with validation and custom format
    /// </summary>
    public static DateTime GetDateTime(string prompt, string format, DateTime? min = null, DateTime? max = null)
    {
        min ??= DateTime.MinValue;
        max ??= DateTime.MaxValue;

        while (true)
        {
            Console.Write($"{prompt} ({format}): ");
            var input = Console.ReadLine()?.Trim();

            if (DateTime.TryParse(input, out var value))
            {
                if (value >= min && value <= max)
                {
                    return value;
                }
                else
                {
                    ConsoleHelper.ShowError($"Please enter a date between {min.Value.ToString(format)} and {max.Value.ToString(format)}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError($"Please enter a valid date in format {format}.");
            }
        }
    }

    // Private helper methods
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        if (username.Length < 3 || username.Length > 50)
            return false;

        return username.All(c => char.IsLetterOrDigit(c) || c == '_') &&
               !username.StartsWith("_") && !username.EndsWith("_");
    }
}
