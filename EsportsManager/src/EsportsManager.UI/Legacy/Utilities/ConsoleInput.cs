using System;
using System.Text.RegularExpressions;

namespace EsportsManager.UI.Legacy.Utilities
{
    /// <summary>
    /// Cung cấp các phương thức xử lý input từ người dùng
    /// </summary>
    public static class ConsoleInput
    {
        /// <summary>
        /// Đọc chuỗi chỉ bao gồm chữ và số
        /// </summary>
        /// <param name="maxLen">Độ dài tối đa của chuỗi</param>
        /// <returns>Chuỗi được nhập hoặc null nếu người dùng nhấn ESC</returns>
        public static string? ReadAlphaNumeric(int maxLen)
        {
            return ReadWithValidation(maxLen, ch => char.IsLetterOrDigit(ch) || ch == '_');
        }

        /// <summary>
        /// Đọc mật khẩu, hiển thị dưới dạng dấu *
        /// </summary>
        /// <param name="maxLen">Độ dài tối đa của mật khẩu</param>
        /// <returns>Mật khẩu được nhập hoặc null nếu người dùng nhấn ESC</returns>
        public static string? ReadPassword(int maxLen)
        {
            return ReadWithValidation(maxLen, _ => true, true);
        }

        /// <summary>
        /// Đọc chuỗi bất kỳ
        /// </summary>
        /// <param name="maxLen">Độ dài tối đa của chuỗi</param>
        /// <returns>Chuỗi được nhập hoặc null nếu người dùng nhấn ESC</returns>
        public static string? ReadAnyString(int maxLen)
        {
            return ReadWithValidation(maxLen, _ => true);
        }

        /// <summary>
        /// Đọc chuỗi với điều kiện xác thực
        /// </summary>
        /// <param name="maxLen">Độ dài tối đa</param>
        /// <param name="validator">Hàm xác thực ký tự</param>
        /// <param name="isPassword">Có phải nhập mật khẩu không</param>
        /// <returns>Chuỗi được nhập hoặc null nếu người dùng nhấn ESC</returns>
        private static string? ReadWithValidation(int maxLen, Func<char, bool> validator, bool isPassword = false)
        {
            string result = "";
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return result.Length > 0 ? result : null;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return null;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                    // Xóa ký tự cuối trên màn hình
                    if (Console.CursorLeft > cursorLeft)
                    {
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Backspace && result.Length < maxLen && validator(keyInfo.KeyChar))
                {
                    result += keyInfo.KeyChar;
                    Console.Write(isPassword ? "*" : keyInfo.KeyChar.ToString());
                }
            }
        }

        /// <summary>
        /// Kiểm tra email có hợp lệ không
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }
}
