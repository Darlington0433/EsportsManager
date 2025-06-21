using System;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// ConsoleFormatter - Utility class để format và tùy chỉnh console window
/// </summary>
public static class ConsoleFormatter
{
    // Console dimensions
    public const int PREFERRED_WIDTH = 120;
    public const int PREFERRED_HEIGHT = 30;
    public const int BUFFER_HEIGHT = 3000;
    
    // Console colors theme
    public static class Theme
    {
        public static readonly ConsoleColor Background = ConsoleColor.Black;
        public static readonly ConsoleColor Primary = ConsoleColor.White;
        public static readonly ConsoleColor Secondary = ConsoleColor.Gray;
        public static readonly ConsoleColor Accent = ConsoleColor.Cyan;
        public static readonly ConsoleColor Success = ConsoleColor.Green;
        public static readonly ConsoleColor Warning = ConsoleColor.Yellow;
        public static readonly ConsoleColor Error = ConsoleColor.Red;
        public static readonly ConsoleColor Info = ConsoleColor.Blue;
    }

    /// <summary>
    /// Thiết lập encoding UTF-8
    /// </summary>
    private static void SetEncoding()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
    }
    
    /// <summary>
    /// Thiết lập title cho console
    /// </summary>
    private static void SetTitle()
    {
        Console.Title = "🎮 ESPORTS MANAGER - Professional Console Application v1.0";
    }
    
    /// <summary>
    /// Thiết lập kích thước console
    /// </summary>
    private static void SetDimensions()
    {
        try
        {
            // Thử thiết lập kích thước lý tưởng
            Console.SetBufferSize(PREFERRED_WIDTH, BUFFER_HEIGHT);
            Console.SetWindowSize(PREFERRED_WIDTH, PREFERRED_HEIGHT);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Fallback với kích thước nhỏ hơn
            try
            {
                Console.SetBufferSize(100, 2000);
                Console.SetWindowSize(100, 25);
            }
            catch
            {
                // Nếu vẫn lỗi thì dùng kích thước mặc định
            }
        }
        catch (PlatformNotSupportedException)
        {
            // Platform không hỗ trợ - bỏ qua
        }
    }
    
    /// <summary>
    /// Thiết lập màu sắc mặc định
    /// </summary>
    private static void SetColors()
    {
        Console.BackgroundColor = Theme.Background;
        Console.ForegroundColor = Theme.Primary;
        Console.Clear();
    }
    
    /// <summary>
    /// Kích hoạt chế độ full screen (Windows only)
    /// </summary>
    private static void EnableFullScreenMode()
    {
        try
        {
            // Chỉ áp dụng trên Windows
            if (OperatingSystem.IsWindows())
            {
                // Maximzie console window
                ConsoleHelper.MaximizeConsoleWindow();
            }
        }
        catch
        {
            // Bỏ qua nếu không thể maximize
        }
    }
    
    /// <summary>
    /// Thiết lập màu cho console
    /// </summary>
    public static void SetColor(ConsoleColor color)
    {
        Console.ForegroundColor = color;
    }
    
    /// <summary>
    /// Reset màu về mặc định
    /// </summary>
    public static void ResetColor()
    {
        Console.ForegroundColor = Theme.Primary;
        Console.BackgroundColor = Theme.Background;
    }
    
    /// <summary>
    /// Tạo separator line
    /// </summary>
    public static void DrawSeparator(char character = '═', int width = 100)
    {
        SetColor(Theme.Secondary);
        Console.WriteLine(new string(character, Math.Min(width, Console.WindowWidth - 1)));
        ResetColor();
    }
    
    /// <summary>
    /// Pause với message đẹp
    /// </summary>
    public static void PauseWithMessage(string message = "Nhấn phím bất kỳ để tiếp tục...")
    {
        SetColor(Theme.Warning);
        Console.Write($"\n{message}");
        ResetColor();
        Console.ReadKey(true);
    }
}

/// <summary>
/// Helper class cho các thao tác Windows-specific
/// </summary>
internal static class ConsoleHelper
{
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();
    
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    private const int SW_MAXIMIZE = 3;
    
    /// <summary>
    /// Maximize console window (Windows only)
    /// </summary>
    public static void MaximizeConsoleWindow()
    {
        try
        {
            IntPtr consoleWindow = GetConsoleWindow();
            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, SW_MAXIMIZE);
            }
        }
        catch
        {
            // Ignore if fails
        }
    }
}
