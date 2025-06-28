using System;
using EsportsManager.BL.Constants;

namespace EsportsManager.UI.Utilities;

/// <summary>
/// ConsoleFormatter - Utility class để format và tùy chỉnh console window
/// </summary>
public static class ConsoleFormatter
{
    /// <summary>
    /// Kích thước mặc định cho console window
    /// </summary>
    public static class WindowSize
    {
        public static int Width => AppConstants.UI.CONSOLE_WIDTH;
        public static int Height => AppConstants.UI.CONSOLE_HEIGHT;
        public static int BufferHeight => AppConstants.UI.CONSOLE_BUFFER;
    }
    
    /// <summary>
    /// Theme màu sắc cho console
    /// </summary>
    public static class Theme
    {
        /// <summary>
        /// Màu nền mặc định
        /// </summary>
        public static readonly ConsoleColor Background = ConsoleColor.Black;

        /// <summary>
        /// Màu chữ chính
        /// </summary>
        public static readonly ConsoleColor Primary = ConsoleColor.White;

        /// <summary>
        /// Màu chữ phụ
        /// </summary>
        public static readonly ConsoleColor Secondary = ConsoleColor.Gray;

        /// <summary>
        /// Màu nhấn mạnh
        /// </summary>
        public static readonly ConsoleColor Accent = ConsoleColor.Cyan;

        /// <summary>
        /// Màu thông báo thành công
        /// </summary>
        public static readonly ConsoleColor Success = ConsoleColor.Green;

        /// <summary>
        /// Màu cảnh báo
        /// </summary>
        public static readonly ConsoleColor Warning = ConsoleColor.Yellow;

        /// <summary>
        /// Màu lỗi
        /// </summary>
        public static readonly ConsoleColor Error = ConsoleColor.Red;

        /// <summary>
        /// Màu thông tin
        /// </summary>
        public static readonly ConsoleColor Info = ConsoleColor.Blue;
    }

    /// <summary>
    /// Khởi tạo console window với kích thước và màu sắc mặc định
    /// </summary>
    public static void InitializeConsole()
    {
        try
        {
            Console.SetWindowSize(WindowSize.Width, WindowSize.Height);
            Console.SetBufferSize(WindowSize.Width, WindowSize.BufferHeight);
            Console.BackgroundColor = Theme.Background;
            Console.ForegroundColor = Theme.Primary;
            Console.Clear();
        }
        catch (Exception)
        {
            // Ignore errors when running in environments that don't support window size changes
        }
    }

    /// <summary>
    /// Reset màu console về mặc định
    /// </summary>
    public static void ResetColors()
    {
        Console.BackgroundColor = Theme.Background;
        Console.ForegroundColor = Theme.Primary;
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
            Console.SetBufferSize(AppConstants.UI.CONSOLE_WIDTH, AppConstants.UI.CONSOLE_BUFFER);
            Console.SetWindowSize(AppConstants.UI.CONSOLE_WIDTH, AppConstants.UI.CONSOLE_HEIGHT);
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
    /// Tạo separator line
    /// </summary>
    public static void DrawSeparator(char character = '═', int width = 100)
    {
        SetColor(Theme.Secondary);
        Console.WriteLine(new string(character, Math.Min(width, Console.WindowWidth - 1)));
        ResetColors();
    }
    
    /// <summary>
    /// Pause với message đẹp
    /// </summary>
    public static void PauseWithMessage(string message = "Nhấn phím bất kỳ để tiếp tục...")
    {
        SetColor(Theme.Warning);
        Console.Write($"\n{message}");
        ResetColors();
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
