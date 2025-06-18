using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// Console utilities - áp dụng Single Responsibility Principle
/// Chỉ lo về việc vẽ và hiển thị trên console
/// </summary>
public static class ConsoleHelper
{
    private static IConfiguration? _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Clear console and show header
    /// </summary>
    public static void ShowHeader(string title = "")
    {
        Console.Clear();
        Console.ForegroundColor = GetColor("Primary");
        
        var appName = _configuration?["AppSettings:ApplicationName"] ?? "Esports Manager";
        var version = _configuration?["AppSettings:Version"] ?? "1.0.0";
        
        Console.WriteLine("═".PadRight(80, '═'));
        Console.WriteLine($"  {appName} v{version}".PadRight(78) + "  ");
        
        if (!string.IsNullOrEmpty(title))
        {
            Console.WriteLine($"  {title}".PadRight(78) + "  ");
        }
        
        Console.WriteLine("═".PadRight(80, '═'));
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Display welcome screen
    /// </summary>
    public static void ShowWelcomeScreen()
    {
        Console.Clear();
        Console.ForegroundColor = GetColor("Primary");
        
        var welcomeMessage = _configuration?["UI:WelcomeMessage"] ?? "Welcome to Esports Manager System!";
        
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + " ".PadRight(78) + "║");
        Console.WriteLine("║" + welcomeMessage.PadLeft((78 + welcomeMessage.Length) / 2).PadRight(78) + "║");
        Console.WriteLine("║" + " ".PadRight(78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// Display success message
    /// </summary>
    public static void ShowSuccess(string message)
    {
        Console.ForegroundColor = GetColor("Success");
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Display error message
    /// </summary>
    public static void ShowError(string message)
    {
        Console.ForegroundColor = GetColor("Error");
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Display warning message
    /// </summary>
    public static void ShowWarning(string message)
    {
        Console.ForegroundColor = GetColor("Warning");
        Console.WriteLine($"⚠ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Display info message
    /// </summary>
    public static void ShowInfo(string message)
    {
        Console.ForegroundColor = GetColor("Info");
        Console.WriteLine($"ℹ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Display multiple errors
    /// </summary>
    public static void ShowErrors(IEnumerable<string> errors)
    {
        Console.ForegroundColor = GetColor("Error");
        Console.WriteLine("✗ Errors occurred:");
        foreach (var error in errors)
        {
            Console.WriteLine($"  • {error}");
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Display loading animation
    /// </summary>
    public static void ShowLoading(string message, int durationMs = 2000)
    {
        Console.Write($"{message} ");
        var chars = new[] { '|', '/', '-', '\\' };
        var start = DateTime.Now;
        
        while ((DateTime.Now - start).TotalMilliseconds < durationMs)
        {
            foreach (var c in chars)
            {
                Console.Write(c);
                Thread.Sleep(100);
                Console.Write("\b");
                
                if ((DateTime.Now - start).TotalMilliseconds >= durationMs)
                    break;
            }
        }
        
        Console.WriteLine("Done!");
    }

    /// <summary>
    /// Display table header
    /// </summary>
    public static void ShowTableHeader(params string[] headers)
    {
        Console.ForegroundColor = GetColor("Secondary");
        
        var columnWidth = 20;
        var separator = "─".PadRight(columnWidth * headers.Length + headers.Length - 1, '─');
        
        Console.WriteLine(separator);
        
        for (int i = 0; i < headers.Length; i++)
        {
            Console.Write(headers[i].PadRight(columnWidth));
            if (i < headers.Length - 1)
                Console.Write("│");
        }
        
        Console.WriteLine();
        Console.WriteLine(separator);
        Console.ResetColor();
    }

    /// <summary>
    /// Display table row
    /// </summary>
    public static void ShowTableRow(params string[] values)
    {
        var columnWidth = 20;
        
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i].Length > columnWidth - 1 
                ? values[i].Substring(0, columnWidth - 4) + "..." 
                : values[i];
            
            Console.Write(value.PadRight(columnWidth));
            if (i < values.Length - 1)
                Console.Write("│");
        }
        
        Console.WriteLine();
    }

    /// <summary>
    /// Display progress bar
    /// </summary>
    public static void ShowProgressBar(int current, int total, string label = "")
    {
        var progress = (double)current / total;
        var barWidth = 50;
        var filledWidth = (int)(progress * barWidth);
        
        Console.Write($"\r{label} [");
        Console.ForegroundColor = GetColor("Success");
        Console.Write("█".PadRight(filledWidth, '█'));
        Console.ResetColor();
        Console.Write("░".PadRight(barWidth - filledWidth, '░'));
        Console.Write($"] {progress:P0} ({current}/{total})");
        
        if (current == total)
            Console.WriteLine();
    }

    /// <summary>
    /// Wait for user input
    /// </summary>
    public static void WaitForKey(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.ForegroundColor = GetColor("Info");
        Console.WriteLine(message);
        Console.ResetColor();
        Console.ReadKey(true);
    }

    /// <summary>
    /// Display separator line
    /// </summary>
    public static void ShowSeparator(char character = '-', int length = 80)
    {
        Console.ForegroundColor = GetColor("Secondary");
        Console.WriteLine(new string(character, length));
        Console.ResetColor();
    }

    /// <summary>
    /// Center text in console
    /// </summary>
    public static void ShowCenteredText(string text, ConsoleColor? color = null)
    {
        if (color.HasValue)
            Console.ForegroundColor = color.Value;
        
        var windowWidth = Console.WindowWidth;
        var padding = (windowWidth - text.Length) / 2;
        Console.WriteLine(text.PadLeft(padding + text.Length));
        
        if (color.HasValue)
            Console.ResetColor();
    }

    /// <summary>
    /// Get color from configuration
    /// </summary>
    private static ConsoleColor GetColor(string colorName)
    {
        var colorString = _configuration?[$"UI:Colors:{colorName}"];
        
        if (Enum.TryParse<ConsoleColor>(colorString, out var color))
            return color;
        
        return colorName switch
        {
            "Primary" => ConsoleColor.Cyan,
            "Secondary" => ConsoleColor.Yellow,
            "Success" => ConsoleColor.Green,
            "Error" => ConsoleColor.Red,
            "Warning" => ConsoleColor.DarkYellow,
            "Info" => ConsoleColor.Blue,
            _ => ConsoleColor.White
        };
    }
}
