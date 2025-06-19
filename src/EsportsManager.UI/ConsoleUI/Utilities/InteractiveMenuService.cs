// Menu tương tác console
using System;
using System.Linq;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.ConsoleUI.Utilities
{    /// <summary>
    /// Menu tương tác
    /// </summary>
    public static class InteractiveMenuService
    {        /// <summary>
        /// ASCII art title
        /// </summary>
        public static readonly string ApplicationTitleArt = @"
███████╗███████╗██████╗  ██████╗ ██████╗ ████████╗███████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ 
██╔════╝██╔════╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗
█████╗  ███████╗██████╔╝██║   ██║██████╔╝   ██║   ███████╗    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝
██╔══╝  ╚════██║██╔═══╝ ██║   ██║██╔══██╗   ██║   ╚════██║    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗
███████╗███████║██║     ╚██████╔╝██║  ██║   ██║   ███████║    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║
╚══════╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝
                                                                                                                            
";/// <summary>
        /// Hiển thị menu tương tác với giao diện giống hệt ảnh mẫu
        /// Bao gồm: border xanh lá, ASCII art lớn, menu option highlight, hướng dẫn phím
        /// </summary>
        /// <param name="menuTitle">Tiêu đề của menu (hiển thị trong border)</param>
        /// <param name="menuOptions">Mảng các lựa chọn menu</param>
        /// <param name="defaultSelectedIndex">Index mặc định được chọn (0 = đầu tiên)</param>        /// <returns>Index của lựa chọn được chọn, hoặc -1 nếu ESC</returns>
        public static int DisplayInteractiveMenu(string menuTitle, string[] menuOptions, int defaultSelectedIndex = 0)
        {
            // Xóa màn hình và đặt nền đen
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;            // Tính toán kích thước và vị trí
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            
            // Tính kích thước khung chứa - ưu tiên chiều rộng để border không bị "ngang ngắn hơn dọc"
            // Đảm bảo đủ rộng cho ASCII art ESPORTS MANAGER (khoảng 130+ ký tự)
            int minContentWidth = Math.Max(windowWidth * 3 / 4, 140); // Ít nhất 3/4 màn hình hoặc 140 ký tự
            int contentWidth = Math.Min(windowWidth - 2, Math.Max(minContentWidth, 140)); // Giới hạn độ rộng, để 2 ký tự margin
            
            // Tính chiều cao dựa trên chiều rộng để đảm bảo tỷ lệ đẹp (rộng > cao)
            int maxContentHeight = Math.Min(windowHeight - 4, contentWidth * 2 / 3); // Chiều cao tối đa = 2/3 chiều rộng
            int contentHeight = Math.Min(maxContentHeight, 25); // Giới hạn chiều cao tối đa là 25
            
            // Đảm bảo chiều rộng luôn lớn hơn chiều cao
            if (contentWidth <= contentHeight)
            {
                contentWidth = contentHeight + 20; // Thêm 20 để đảm bảo rộng hơn cao
                contentWidth = Math.Min(contentWidth, windowWidth - 2); // Không vượt biên màn hình
            }
            
            // Căn giữa khung trong màn hình
            int left = Math.Max(0, (windowWidth - contentWidth) / 2);
            int top = Math.Max(0, (windowHeight - contentHeight) / 2);
            
            // Vẽ khung viền
            ConsoleRenderingService.DrawBorder(left, top, contentWidth, contentHeight, menuTitle, true);            // Hiển thị ASCII art logo - căn giữa trong khung content, không căn giữa toàn màn hình
            string[] artLines = ApplicationTitleArt.Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
              
            // Tính vị trí để căn giữa ASCII art trong khung content
            int artTop = top + 2;
            int leftPadding = left + 2; // Padding từ border trái
            int availableWidth = contentWidth - 4; // Chiều rộng khả dụng trong khung (trừ 2 border + 2 padding)
            
            for (int i = 0; i < artLines.Length && artTop + i < top + contentHeight - 2; i++)
            {
                string line = artLines[i];
                // KHÔNG cắt dòng để đảm bảo không mất chữ R cuối cùng
                // Chỉ căn giữa dòng trong khung content
                int artLeft = leftPadding + Math.Max(0, (availableWidth - line.Length) / 2);
                
                // Đảm bảo không vượt biên trái
                if (artLeft < leftPadding) artLeft = leftPadding;
                
                SafeConsole.SetCursorPosition(artLeft, artTop + i);
                Console.ForegroundColor = ConsoleColor.Cyan; // Màu xanh dương cho ASCII art
                Console.Write(line);
                Console.ResetColor();
            }// Hiển thị menu options - căn giữa trong khung content
            int menuStartY = artTop + artLines.Length + 2; // Cách logo 2 dòng
            for (int i = 0; i < menuOptions.Length; i++)
            {
                string optionText = $"> {menuOptions[i]}";
                // Căn giữa option trong khung content
                int optionLeft = leftPadding + Math.Max(0, (availableWidth - optionText.Length) / 2);
                SafeConsole.SetCursorPosition(optionLeft, menuStartY + i);
                
                if (i == defaultSelectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"> {menuOptions[i]}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"  {menuOptions[i]}");
                }
                Console.ResetColor();
            }
              // Hiển thị hướng dẫn phím ở cuối khung - căn giữa trong khung content
            string helpText = "↑↓: Di chuyển | Enter: Chọn | Esc: Thoát";
            // Cắt text nếu quá dài
            if (helpText.Length > availableWidth)
            {
                helpText = helpText.Substring(0, availableWidth);
            }
            int helpLeft = leftPadding + Math.Max(0, (availableWidth - helpText.Length) / 2);
            SafeConsole.SetCursorPosition(helpLeft, top + contentHeight - 3);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(helpText);
            Console.ResetColor();
            
            // Xử lý input
            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        defaultSelectedIndex = (defaultSelectedIndex > 0) ? defaultSelectedIndex - 1 : menuOptions.Length - 1;
                        break;
                        
                    case ConsoleKey.DownArrow:
                        defaultSelectedIndex = (defaultSelectedIndex < menuOptions.Length - 1) ? defaultSelectedIndex + 1 : 0;
                        break;
                        
                    case ConsoleKey.Enter:
                        return defaultSelectedIndex;
                        
                    case ConsoleKey.Escape:
                        return -1;
                }                  // Cập nhật menu selection - căn giữa trong khung content
                for (int i = 0; i < menuOptions.Length; i++)
                {
                    string optionText = $"> {menuOptions[i]}";
                    // Căn giữa option trong khung content
                    int optionLeft = leftPadding + Math.Max(0, (availableWidth - optionText.Length) / 2);
                    SafeConsole.SetCursorPosition(optionLeft, menuStartY + i);
                    
                    if (i == defaultSelectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"> {menuOptions[i]}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"  {menuOptions[i]}");
                    }
                    Console.ResetColor();
                }
            }
        }        /// <summary>
        /// Chờ người dùng nhấn phím bất kỳ để tiếp tục
        /// Utility method đơn giản để pause execution
        /// </summary>
        public static void WaitForAnyKey()
        {
            // Hiển thị message ở gần cuối màn hình
            ConsoleRenderingService.WriteLineCenter(
                "Nhấn phím bất kỳ để tiếp tục...",    // Text hướng dẫn
                Console.WindowHeight - 2,             // Vị trí: 2 dòng từ cuối màn hình
                ConsoleColor.Gray                      // Màu xám nhạt
            );
            // Chờ input từ người dùng
            Console.ReadKey(true); // true = không hiển thị ký tự nhấn
        }

        /// <summary>
        /// Căn giữa text trong một field có độ rộng cho trước
        /// Utility method để format text
        /// </summary>
        /// <param name="text">Text cần căn giữa</param>
        /// <param name="width">Độ rộng của field</param>
        /// <returns>String đã được căn giữa với padding</returns>
        public static string CenterText(string text, int width)
        {
            // Delegate việc căn giữa cho ConsoleRenderingService
            return ConsoleRenderingService.CenterText(text, width);
        }
        
        /// <summary>
        /// Lấy xác nhận Yes/No từ người dùng với giao diện đẹp
        /// Method này hiển thị prompt và chờ user nhấn Y hoặc N
        /// </summary>
        /// <param name="message">Câu hỏi cần xác nhận</param>
        /// <returns>true nếu user chọn Y, false nếu chọn N</returns>
        public static bool GetYesNoConfirmation(string message)
        {
            // Hiển thị câu hỏi với format: "Message (Y/N): "
            Console.Write($"{message} (Y/N): ");
            // Đổi màu chữ thành vàng để nổi bật
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            // Vòng lặp chờ input hợp lệ (Y hoặc N)
            while (true)
            {
                // Đọc phím nhấn (không hiển thị ký tự)
                var key = Console.ReadKey(true);
                
                if (key.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Yes");    // Hiển thị "Yes" để confirm
                    Console.ResetColor();        // Reset màu về mặc định
                    return true;                 // Trả về true (đồng ý)
                }
                if (key.Key == ConsoleKey.N)
                {
                    Console.WriteLine("No");     // Hiển thị "No" để confirm  
                    Console.ResetColor();        // Reset màu về mặc định
                    return false;                // Trả về false (từ chối)
                }
                // Nếu nhấn phím khác Y/N, tiếp tục vòng lặp (không làm gì cả)
            }
        } // Kết thúc method GetYesNoConfirmation
    } // Kết thúc class InteractiveMenuService
} // Kết thúc namespace
