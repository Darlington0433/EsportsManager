using System;

namespace EsportsManager.UI.Legacy.Utilities
{
    /// <summary>
    /// Cung cấp các phương thức để vẽ giao diện console
    /// </summary>
    public static class ConsoleDrawing
    {
        /// <summary>
        /// Vẽ một hộp với viền
        /// </summary>
        public static void DrawBox(int width, int height, int left, int top)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            // Top border
            Console.SetCursorPosition(left, top);
            Console.Write("╔" + new string('═', width - 2) + "╗");

            // Sides
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write("║" + new string(' ', width - 2) + "║");
            }

            // Bottom border
            Console.SetCursorPosition(left, top + height - 1);
            Console.Write("╚" + new string('═', width - 2) + "╝");
            Console.ResetColor();
        }
        
        /// <summary>
        /// Vẽ một hộp với viền và tiêu đề ở giữa thanh ngang trên cùng
        /// </summary>
        public static void DrawBoxWithTitle(int width, int height, int left, int top, string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            
            // Tính toán vị trí tiêu đề
            int titleWithSpaces = title.Length + 2; // +2 cho khoảng trắng hai bên
            int titlePosition = Math.Max(1, (width - titleWithSpaces) / 2);
            
            // Top border với tiêu đề
            Console.SetCursorPosition(left, top);
            Console.Write("╔");
            
            // Phần trước tiêu đề
            if (titlePosition > 1)
            {
                Console.Write(new string('═', titlePosition - 1));
            }
            
            // Tiêu đề
            Console.Write(" " + title + " ");
            
            // Phần sau tiêu đề
            int remainingWidth = width - 2 - titlePosition - titleWithSpaces + 1;
            if (remainingWidth > 0)
            {
                Console.Write(new string('═', remainingWidth));
            }
            
            Console.Write("╗");

            // Sides
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write("║" + new string(' ', width - 2) + "║");
            }
            
            // Bottom border
            Console.SetCursorPosition(left, top + height - 1);
            Console.Write("╚" + new string('═', width - 2) + "╝");
            Console.ResetColor();
        }

        /// <summary>
        /// Vẽ ASCII art căn giữa
        /// </summary>
        public static void DrawTitleArt(string[] artLines, int contentWidth)
        {
            foreach (var line in artLines)
            {
                string trimmed = line.TrimEnd('\r', '\n');
                if (!string.IsNullOrEmpty(trimmed))
                {
                    // Nếu art dài hơn contentWidth thì cắt bớt
                    string artLine = trimmed.Length > contentWidth ? trimmed.Substring(0, contentWidth) : trimmed;
                    string centered = CenterText(artLine, contentWidth);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("║");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(centered);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("║");
                    Console.ResetColor();
                }
            }
        }
        
        /// <summary>
        /// Căn giữa văn bản trong một chiều rộng nhất định
        /// </summary>
        public static string CenterText(string text, int width)
        {
            if (width <= text.Length) return text;
            int pad = (width - text.Length) / 2;
            return new string(' ', pad) + text + new string(' ', width - pad - text.Length);
        }
        
        /// <summary>
        /// Cập nhật một dòng menu mà không redraw toàn bộ
        /// </summary>
        public static void UpdateMenuLine(int leftPad, int line, string text, bool isSelected, int contentWidth)
        {
            Console.SetCursorPosition(leftPad, line);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("║");
            string lineText = isSelected ? $"> {text} ▶" : $"  {text}";
            string centered = CenterText(lineText, contentWidth);
            if (isSelected)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write(centered);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("║");
        }

        /// <summary>
        /// Vẽ nội dung form với tô sáng vị trí được chọn
        /// </summary>
        public static void DrawFormContent(string[] fields, int selectedField, string[] values, int width, int left, int top)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Console.SetCursorPosition(left + 4, top + i);
                string value = values[i] ?? "";
                string line = $"{fields[i]}: {value}";
                if (i == selectedField)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("▶ ");
                    Console.Write(line);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ");
                    Console.Write(line);
                }
                Console.ResetColor();
            }
        }
        
        /// <summary>
        /// Trả về chiều rộng tối đa của các dòng trong văn bản
        /// </summary>
        public static int GetMaxLineWidth(string text)
        {
            int max = 0;
            foreach (var line in text.Split('\n'))
                if (line.Length > max) max = line.Length;
            return max;
        }
        
        /// <summary>
        /// Tính toán độ rộng thực tế của chuỗi Unicode khi hiển thị trên console
        /// </summary>
        /// <param name="text">Chuỗi cần tính độ rộng</param>
        /// <returns>Độ rộng thực tế (số cột) khi hiển thị trên console</returns>
        public static int GetVisualWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
                
            int width = 0;
            foreach (char c in text)
            {
                // Các ký tự điều khiển không chiếm không gian
                if (char.IsControl(c))
                    continue;
                    
                // Các ký tự surrogate được tính là một ký tự
                if (char.IsHighSurrogate(c) || char.IsLowSurrogate(c))
                    continue;
                    
                // Một số ký tự CJK và emoji có độ rộng 2 cột
                if ((c >= 0x1100 && c <= 0x11FF) || // Hangul Jamo
                    (c >= 0x3000 && c <= 0x303F) || // CJK Punctuation
                    (c >= 0x3040 && c <= 0x309F) || // Hiragana
                    (c >= 0x30A0 && c <= 0x30FF) || // Katakana
                    (c >= 0x3130 && c <= 0x318F) || // Hangul Compatibility Jamo
                    (c >= 0xAC00 && c <= 0xD7AF) || // Hangul Syllables
                    (c >= 0x4E00 && c <= 0x9FFF) || // CJK Unified Ideographs
                    (c >= 0xFF00 && c <= 0xFFEF))   // Fullwidth Forms
                {
                    width += 2;
                }
                else
                {
                    width += 1;
                }
            }
            
            return width;
        }
        
        /// <summary>
        /// Thay đổi kích thước ASCII art để phù hợp với độ rộng có sẵn
        /// </summary>
        /// <param name="asciiArt">ASCII art gốc</param>
        /// <param name="availableWidth">Độ rộng tối đa có thể hiển thị</param>
        /// <returns>ASCII art đã điều chỉnh kích thước hoặc null nếu không cần điều chỉnh</returns>
        public static string[] ScaleAsciiArt(string asciiArt, int availableWidth)
        {
            string[] lines = asciiArt.Split('\n');
            int maxWidth = 0;
            foreach (var line in lines)
                if (line.Length > maxWidth) maxWidth = line.Length;
            
            // Nếu độ rộng của ASCII art nhỏ hơn độ rộng có sẵn, không cần điều chỉnh
            if (maxWidth <= availableWidth - 4) // 4 là padding cho viền
                return lines;
            
            string[] scaledLines = new string[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd('\r');
                if (line.Length > availableWidth - 4)
                {
                    // Cắt bớt hoặc thu nhỏ dòng để vừa với độ rộng có sẵn
                    scaledLines[i] = line.Substring(0, Math.Max(0, availableWidth - 4));
                }
                else
                {
                    scaledLines[i] = line;
                }
            }
            return scaledLines;
        }
    }
}
