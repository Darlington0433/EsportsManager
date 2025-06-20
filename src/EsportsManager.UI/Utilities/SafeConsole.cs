// =============================================================================
// SAFECONSOLE.CS - UTILITY CHO CÁC THAO TÁC CONSOLE AN TOÀN
// =============================================================================
// 
// CHỨC NĂNG:
// - Cung cấp wrapper an toàn cho Console.SetCursorPosition
// - Tự động validate và điều chỉnh tọa độ để tránh lỗi
// - Xử lý các edge cases khi console window thay đổi kích thước
// - Backup/fallback methods cho compatibility
// 
// VẤN ĐỀ GIẢI QUYẾT:
// - ArgumentOutOfRangeException khi set cursor position
// - Console buffer vs window size conflicts
// - Cross-platform console compatibility issues
// - Terminal resize handling
// 
// DESIGN PATTERNS:
// - Static Utility: Không cần state, chỉ cung cấp helper methods
// - Defensive Programming: Validate input và handle errors gracefully
// - Fail-Safe: Fallback strategies khi operations fail
// 
// =============================================================================

using System;

namespace EsportsManager.UI.Utilities
{
    /// <summary>
    /// SafeConsole - Utility class cung cấp các thao tác console an toàn
    /// 
    /// TÍNH NĂNG:
    /// - SetCursorPosition: Đặt vị trí cursor với validation
    /// - IsValidPosition: Kiểm tra tọa độ có hợp lệ không
    /// - GetSafePosition: Điều chỉnh tọa độ về khoảng hợp lệ
    /// - GetConsoleSize: Lấy kích thước console an toàn
    /// 
    /// SỬ DỤNG:
    /// - Thay thế Console.SetCursorPosition với SafeConsole.SetCursorPosition
    /// - Luôn validate positions trước khi render text
    /// - Handle console resize events gracefully
    /// 
    /// COMPATIBILITY:
    /// - Windows Console Host
    /// - Windows Terminal
    /// - PowerShell Console
    /// - CMD.exe
    /// - VS Code Terminal
    /// </summary>
    public static class SafeConsole
    {
        /// <summary>
        /// Đặt vị trí cursor một cách an toàn, tự động validate và điều chỉnh tọa độ
        /// Tránh lỗi khi left/top âm hoặc vượt quá kích thước console window
        /// </summary>
        /// <param name="left">Vị trí cột (x), sẽ được điều chỉnh trong khoảng hợp lệ</param>
        /// <param name="top">Vị trí hàng (y), sẽ được điều chỉnh trong khoảng hợp lệ</param>
        public static void SetCursorPosition(int left, int top)
        {
            try
            {
                // Đảm bảo left nằm trong khoảng [0, WindowWidth-1]
                // Math.Max(0, left): không cho phép giá trị âm
                // Math.Min(left, Console.WindowWidth - 1): không cho phép vượt quá chiều rộng
                left = Math.Max(0, Math.Min(left, Console.WindowWidth - 1));
                
                // Đảm bảo top nằm trong khoảng [0, WindowHeight-1]  
                // Tương tự như left nhưng cho chiều cao
                top = Math.Max(0, Math.Min(top, Console.WindowHeight - 1));
                
                // Gọi SetCursorPosition với tọa độ đã được validate
                Console.SetCursorPosition(left, top);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Nếu vẫn có lỗi (trường hợp hiếm), đặt cursor về góc trên trái (0,0)
                // Đây là vị trí an toàn nhất
                Console.SetCursorPosition(0, 0);
            }
        }

        /// <summary>
        /// Viết text tại vị trí chỉ định một cách an toàn, có thể đổi màu
        /// Kết hợp SetCursorPosition an toàn với việc viết text và quản lý màu sắc
        /// </summary>
        /// <param name="left">Vị trí cột để bắt đầu viết</param>
        /// <param name="top">Vị trí hàng để bắt đầu viết</param>
        /// <param name="text">Nội dung text cần viết</param>
        /// <param name="color">Màu chữ (optional), nếu null thì giữ màu hiện tại</param>
        public static void WriteAt(int left, int top, string text, ConsoleColor? color = null)
        {
            // Đặt cursor về vị trí mong muốn (đã được validate an toàn)
            SetCursorPosition(left, top);
            
            // Nếu có chỉ định màu, thay đổi màu foreground
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }
            
            // Viết text ra console
            Console.Write(text);
            
            // Nếu đã thay đổi màu, reset về màu mặc định để không ảnh hưởng text khác
            if (color.HasValue)
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Xóa một vùng hình chữ nhật trên console một cách an toàn
        /// Điền khoảng trắng vào vùng được chỉ định để "xóa" nội dung cũ
        /// </summary>
        /// <param name="left">Vị trí cột bắt đầu của vùng cần xóa</param>
        /// <param name="top">Vị trí hàng bắt đầu của vùng cần xóa</param>
        /// <param name="width">Chiều rộng của vùng cần xóa</param>
        /// <param name="height">Chiều cao của vùng cần xóa</param>
        public static void ClearArea(int left, int top, int width, int height)
        {
            // Lặp qua từng hàng trong vùng cần xóa
            for (int i = 0; i < height && top + i < Console.WindowHeight; i++)
            {
                // Đặt cursor ở đầu hàng hiện tại
                SetCursorPosition(left, top + i);
                
                // Tính chiều rộng an toàn (không vượt quá biên console)
                int safeWidth = Math.Min(width, Console.WindowWidth - left);
                
                // Chỉ viết khoảng trắng nếu còn chỗ để viết
                if (safeWidth > 0)
                {
                    // Tạo chuỗi khoảng trắng có độ dài safeWidth và viết ra
                    Console.Write(new string(' ', safeWidth));
                }
            }
        }

        /// <summary>
        /// Lấy kích thước console window một cách an toàn
        /// Trả về kích thước thực hoặc giá trị mặc định nếu có lỗi
        /// </summary>
        /// <returns>Tuple chứa (Width, Height) của console window</returns>
        public static (int Width, int Height) GetSafeDimensions()
        {
            try
            {
                // Cố gắng lấy kích thước thực của console window
                return (Console.WindowWidth, Console.WindowHeight);
            }
            catch
            {
                // Nếu có lỗi (console không hỗ trợ hoặc lỗi hệ thống), 
                // trả về kích thước mặc định 80x25 (kích thước console chuẩn)
                return (80, 25);
            }
        }
    }
}
