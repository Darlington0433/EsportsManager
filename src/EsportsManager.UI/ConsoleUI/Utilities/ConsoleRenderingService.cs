// Render UI console

using System;
using System.Text;
using System.Linq;
using System.Threading;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.ConsoleUI.Utilities
{    /// <summary>
     /// Render UI console
     /// </summary>
     /// </summary>
    public static class ConsoleRenderingService
    {
        // Các ký tự để vẽ border đơn (single line border)
        // Format: [TopLeft, Horizontal, TopRight, Vertical, BottomLeft, BottomRight]
        public static readonly char[] SingleBorder = { '┌', '─', '┐', '│', '└', '┘' };

        // Các ký tự để vẽ border đôi (double line border) - trông đậm và nổi bật hơn
        public static readonly char[] DoubleBorder = { '╔', '═', '╗', '║', '╚', '╝' };

        /// <summary>
        /// Vẽ khung border xung quanh một vùng console với title tùy chọn
        /// Được sử dụng để tạo các hộp thoại, form, menu có viền đẹp mắt
        /// </summary>
        /// <param name="left">Vị trí cột bắt đầu của border</param>
        /// <param name="top">Vị trí hàng bắt đầu của border</param>
        /// <param name="width">Chiều rộng của border</param>
        /// <param name="height">Chiều cao của border</param>
        /// <param name="title">Tiêu đề hiển thị ở giữa border trên (optional)</param>
        /// <param name="useDoubleBorder">true = dùng border đôi, false = border đơn</param>
        public static void DrawBorder(int left, int top, int width, int height, string? title = null, bool useDoubleBorder = true)
        {
            // Validate và điều chỉnh parameters để tránh vị trí âm hoặc vượt biên console
            left = Math.Max(0, left); // Không cho phép vị trí âm
            top = Math.Max(0, top);
            width = Math.Min(width, Console.WindowWidth - left); // Không vượt quá chiều rộng console
            height = Math.Min(height, Console.WindowHeight - top); // Không vượt quá chiều cao console

            // Nếu kích thước quá nhỏ (<=2) thì không thể vẽ border đầy đủ
            if (width <= 2 || height <= 2) return;

            // Chọn bộ ký tự border (đơn hoặc đôi)
            var border = useDoubleBorder ? DoubleBorder : SingleBorder;
            Console.ForegroundColor = ConsoleColor.Green; // Màu xanh cho border            // Vẽ hàng trên của border
            SafeConsole.SetCursorPosition(left, top);
            Console.Write(border[0]); // Góc trên trái

            // Nếu có title, đặt title ở giữa hàng trên
            if (!string.IsNullOrEmpty(title))
            {
                int titleSpace = width - 2; // Không gian available cho title (trừ 2 góc)
                int padding = Math.Max(0, (titleSpace - title.Length) / 2); // Tính padding để center title

                // Vẽ đường kẻ ngang trước title
                Console.Write(new string(border[1], Math.Max(0, padding - 1)));
                Console.Write(' '); // Khoảng trắng trước title

                // Đổi màu title sang trắng để nổi bật
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(title);
                Console.ForegroundColor = ConsoleColor.Green; // Đổi lại màu border

                Console.Write(' '); // Khoảng trắng sau title
                // Vẽ đường kẻ ngang sau title
                Console.Write(new string(border[1], Math.Max(0, titleSpace - title.Length - padding - 1)));
            }
            else
            {
                // Không có title, vẽ đường kẻ ngang liên tục
                Console.Write(new string(border[1], width - 2));
            }
            Console.Write(border[2]); // Góc trên phải            // Vẽ các hàng giữa (body của border)
            for (int i = 1; i < height - 1; i++)
            {
                SafeConsole.SetCursorPosition(left, top + i);
                Console.Write(border[3]); // Đường dọc trái
                Console.Write(new string(' ', width - 2)); // Khoảng trắng ở giữa
                Console.Write(border[3]); // Đường dọc phải
            }            // Vẽ hàng dưới của border
            SafeConsole.SetCursorPosition(left, top + height - 1);
            Console.Write(border[4]); // Góc dưới trái
            Console.Write(new string(border[1], width - 2)); // Đường kẻ ngang dưới
            Console.Write(border[5]); // Góc dưới phải

            Console.ResetColor(); // Reset màu về mặc định
        }        /// <summary>
                 /// Viết text căn giữa màn hình tại vị trí dọc chỉ định với positioning an toàn
                 /// Tự động tính toán vị trí ngang để căn giữa text
                 /// </summary>
                 /// <param name="text">Nội dung text cần hiển thị</param>
                 /// <param name="y">Vị trí hàng để hiển thị (optional, null = vị trí hiện tại)</param>
                 /// <param name="color">Màu chữ (optional, null = màu hiện tại)</param>
        public static void WriteLineCenter(string text, int? y = null, ConsoleColor? color = null)
        {
            int windowWidth = Console.WindowWidth; // Lấy chiều rộng console hiện tại
            // Tính vị trí left để căn giữa text, đảm bảo không âm
            int left = Math.Max(0, (windowWidth - text.Length) / 2);
            // Nếu có chỉ định vị trí y và y hợp lệ (trong bounds console)
            if (y.HasValue && y.Value >= 0 && y.Value < Console.WindowHeight)
            {
                SafeConsole.SetCursorPosition(left, y.Value);
            }

            // Đổi màu nếu được chỉ định
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            Console.Write(text); // Viết text

            // Reset màu nếu đã thay đổi
            if (color.HasValue)
            {
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Hiển thị message box với border, có thể là thông báo thường hoặc lỗi
        /// Tự động căn giữa màn hình và có timeout tự động đóng
        /// </summary>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="isError">true = thông báo lỗi (màu đỏ), false = thông báo thường (màu xanh)</param>
        /// <param name="timeout">Thời gian hiển thị (milliseconds), 0 = không tự đóng</param>
        public static void ShowMessageBox(string message, bool isError = false, int timeout = 2000)
        {
            // Lưu màu hiện tại để restore sau này
            var oldForeground = Console.ForegroundColor;
            var oldBackground = Console.BackgroundColor;

            try
            {
                // Tính kích thước message box
                // Chiều rộng: tối thiểu 40, tối đa là chiều rộng console - 4 (để có margin)
                int width = Math.Min(Console.WindowWidth - 4, Math.Max(message.Length + 4, 40));
                int height = 3; // Chiều cao cố định cho message box đơn giản

                // Tính vị trí để căn giữa màn hình
                int left = (Console.WindowWidth - width) / 2;
                int top = (Console.WindowHeight - height) / 2;                // Xóa vùng sẽ vẽ message box (clear background)
                for (int i = 0; i < height; i++)
                {
                    SafeConsole.SetCursorPosition(left, top + i);
                    Console.Write(new string(' ', width)); // Điền khoảng trắng
                }

                // Vẽ border với màu tương ứng
                Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
                DrawBorder(left, top, width, height);
                // Viết message ở giữa message box
                int messageLeft = Math.Max(0, left + (width - message.Length) / 2);
                SafeConsole.SetCursorPosition(messageLeft, top + 1); // Hàng giữa của box
                Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write(message);

                // Chờ theo timeout (nếu > 0)
                if (timeout > 0)
                {
                    Thread.Sleep(timeout);
                }
            }
            finally
            {
                // Luôn restore màu ban đầu dù có lỗi hay không
                Console.ForegroundColor = oldForeground;
                Console.BackgroundColor = oldBackground;
            }
        }        /// <summary>
                 /// Render ASCII art căn giữa trong khung với màu cyan như ảnh mẫu
                 /// Đảm bảo không bị cắt và hiển thị đầy đủ, tự động điều chỉnh kích thước khung nếu cần
                 /// </summary>
                 /// <param name="art">Chuỗi ASCII art (có thể chứa nhiều dòng, ngăn cách bởi \n)</param>
                 /// <param name="top">Vị trí hàng bắt đầu để vẽ art</param>
                 /// <param name="leftPadding">Padding trái của khung border</param>
                 /// <param name="maxWidth">Chiều rộng khung có sẵn (trừ border)</param>
        public static void RenderAsciiArt(string art, int top = 0, int leftPadding = 0, int maxWidth = 0)
        {
            // Tách art thành các dòng riêng biệt, bỏ qua dòng trống
            var lines = art.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            // Vẽ từng dòng của ASCII art
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd(); // Bỏ space cuối dòng

                // KHÔNG cắt dòng để đảm bảo hiển thị đầy đủ
                // Chỉ kiểm tra nếu line quá dài so với console width
                if (line.Length > Console.WindowWidth - 4)
                {
                    // Nếu thực sự quá dài, mới cắt để tránh lỗi
                    line = line.Substring(0, Console.WindowWidth - 4);
                }

                // Tính vị trí left để căn giữa trong khung
                int left;
                if (maxWidth > 0 && leftPadding > 0)
                {
                    // Căn giữa trong khung với padding - ưu tiên hiển thị đầy đủ
                    int availableSpace = maxWidth - 4; // Trừ margin
                    if (line.Length <= availableSpace)
                    {
                        // Nếu vừa, căn giữa bình thường
                        int centerOffset = (maxWidth - line.Length) / 2;
                        left = leftPadding + centerOffset;
                    }
                    else
                    {
                        // Nếu không vừa, đặt sát biên trái để hiển thị tối đa
                        left = leftPadding + 2;
                    }
                }
                else
                {
                    // Căn giữa toàn màn hình (fallback)
                    left = Math.Max(2, (Console.WindowWidth - line.Length) / 2);
                }

                // Đảm bảo không vượt quá biên màn hình
                left = Math.Max(1, left); // Tối thiểu cách biên trái 1
                left = Math.Min(left, Console.WindowWidth - line.Length - 1); // Đảm bảo không lòi phải
                                                                              // Đảm bảo vị trí y hợp lệ
                int currentY = top + i;
                if (currentY >= 0 && currentY < Console.WindowHeight && left >= 0)
                {
                    SafeConsole.SetCursorPosition(left, currentY);
                    Console.ForegroundColor = ConsoleColor.Cyan; // Màu cyan như trong ảnh mẫu
                    Console.Write(line); // Viết dòng hiện tại
                }
            }
            Console.ResetColor(); // Reset màu
        }

        /// <summary>
        /// Render một input field với label, có thể là field thường hoặc password
        /// Hỗ trợ highlight khi field được chọn (focus)
        /// </summary>
        /// <param name="label">Nhãn của field (ví dụ: "Username", "Password")</param>
        /// <param name="value">Giá trị hiện tại của field</param>
        /// <param name="left">Vị trí cột bắt đầu</param>
        /// <param name="top">Vị trí hàng</param>
        /// <param name="width">Chiều rộng tổng của field (bao gồm label)</param>
        /// <param name="isSelected">true = field đang được chọn (highlight)</param>        /// <param name="isPassword">true = field password (hiển thị dấu *)</param>
        public static void RenderField(string label, string value, int left, int top, int width, bool isSelected, bool isPassword = false)
        {
            SafeConsole.SetCursorPosition(left, top);

            // Hiển thị label với màu trắng
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{label}: ");

            // Tính chiều rộng của vùng input (trừ đi label và ": ")
            int fieldWidth = width - label.Length - 2;

            // Đổi background tùy theo trạng thái selected
            Console.BackgroundColor = isSelected ? ConsoleColor.DarkGray : ConsoleColor.Black;

            // Hiển thị value
            if (isPassword && !string.IsNullOrEmpty(value))
            {
                // Password field: hiển thị dấu * thay vì text thực
                Console.Write(new string('*', value.Length).PadRight(fieldWidth));
            }
            else
            {
                // Field thường: hiển thị value và pad để đủ chiều rộng
                Console.Write(value.PadRight(fieldWidth));
            }

            Console.ResetColor(); // Reset màu và background
        }

        /// <summary>
        /// Render hướng dẫn navigation ở dưới form
        /// Thường hiển thị các phím tắt như "Enter: Submit, Esc: Cancel, Tab: Next field"
        /// </summary>
        /// <param name="navigation">Text hướng dẫn navigation</param>
        /// <param name="left">Vị trí cột</param>        /// <param name="top">Vị trí hàng</param>
        public static void RenderNavigation(string navigation, int left, int top)
        {
            SafeConsole.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.DarkGray; // Màu xám nhạt cho navigation
            Console.Write(navigation);
            Console.ResetColor();
        }

        /// <summary>
        /// Căn giữa text trong một field có chiều rộng cố định
        /// Utility method để format text hiển thị đẹp mắt
        /// </summary>
        /// <param name="text">Text cần căn giữa</param>
        /// <param name="width">Chiều rộng field</param>
        /// <returns>Text đã được căn giữa và pad đủ chiều rộng</returns>
        public static string CenterText(string text, int width)
        {
            // Nếu text null/empty, trả về chuỗi khoảng trắng đủ width
            if (string.IsNullOrEmpty(text)) return new string(' ', width);

            // PadLeft: thêm space vào trước để căn giữa
            // PadRight: thêm space vào sau để đủ width
            return text.PadLeft((width + text.Length) / 2).PadRight(width);
        }

        /// <summary>
        /// Hiển thị loading message với animation
        /// </summary>
        public static void ShowLoadingMessage(string message)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"⚡ {message}");

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(300);
                Console.Write(".");
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Pause với message và chờ user nhấn phím
        /// </summary>
        public static void PauseWithMessage(string message = "\nNhấn phím bất kỳ để tiếp tục...")
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);
            Console.ResetColor();
            Console.ReadKey(true);
        }        /// <summary>
                 /// Vẽ border đơn giản với title (overload cho compatibility)
                 /// </summary>
        public static void DrawBorder(string title, int width, int height)
        {
            int left = (Console.WindowWidth - width) / 2;
            int top = (Console.WindowHeight - height) / 2;
            DrawBorder(left, top, width, height, title, true); // true = use double border
        }

        /// <summary>
        /// Hiển thị thông báo cho người dùng với màu sắc tương ứng
        /// </summary>
        /// <param name="message">Nội dung thông báo</param>
        /// <param name="color">Màu sắc thông báo</param>
        public static void ShowNotification(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = originalColor;
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Vẽ khung border với title và căn chỉnh tự động
        /// </summary>
        /// <param name="title">Tiêu đề của khung</param>
        /// <param name="width">Chiều rộng khung</param>
        /// <param name="height">Chiều cao khung</param>
        public static void DrawBorder(string title, int width, int height)
        {
            // Tính toán vị trí để căn giữa border
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int left = Math.Max(0, (windowWidth - width) / 2);
            int top = Math.Max(0, (windowHeight - height) / 4);

            DrawBorder(left, top, width, height, title, true);
        }

        /// <summary>
        /// Đợi người dùng nhấn phím để tiếp tục
        /// </summary>
        public static void WaitForKeyPress()
        {
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }
}
