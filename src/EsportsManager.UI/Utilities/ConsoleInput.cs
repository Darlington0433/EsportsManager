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
                ConsoleHelper.ShowError("This field is required. Please try again.");
                continue;
            }

            if (!string.IsNullOrEmpty(input) && input.Length > maxLength)
            {
                ConsoleHelper.ShowError($"Input cannot exceed {maxLength} characters. Please try again.");
                continue;
            }

            return input ?? string.Empty;
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
                    ConsoleHelper.ShowError($"Please enter a number between {min} and {max}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError("Please enter a valid number.");
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
                    ConsoleHelper.ShowError("Please press 'y' for yes or 'n' for no.");
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
                    ConsoleHelper.ShowError($"Please enter a number between {minChoice} and {maxChoice}.");
                }
            }
            else
            {
                ConsoleHelper.ShowError("Please enter a valid number.");
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

            var choice = GetChoice("Enter your choice", 1, options.Length);
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

            ConsoleHelper.ShowError("Please enter a valid email address.");
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
            Console.Write($"{prompt} (yyyy-mm-dd): ");
            var input = Console.ReadLine()?.Trim();

            if (DateTime.TryParse(input, out var date))
            {
                if (minDate.HasValue && date < minDate.Value)
                {
                    ConsoleHelper.ShowError($"Date cannot be earlier than {minDate.Value:yyyy-MM-dd}.");
                    continue;
                }

                if (maxDate.HasValue && date > maxDate.Value)
                {
                    ConsoleHelper.ShowError($"Date cannot be later than {maxDate.Value:yyyy-MM-dd}.");
                    continue;
                }

                return date;
            }

            ConsoleHelper.ShowError("Please enter a valid date in yyyy-mm-dd format.");
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
