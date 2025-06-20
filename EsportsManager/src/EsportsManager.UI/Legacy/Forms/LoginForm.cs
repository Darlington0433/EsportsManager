using System;
using EsportsManager.UI.Legacy.Utilities;

namespace EsportsManager.UI.Legacy.Forms
{
    /// <summary>
    /// Xử lý hiển thị và tương tác với form đăng nhập
    /// </summary>
    public class LoginForm : BaseForm
    {
        private static int selectedField = 0;
        private readonly string[] fields = { "Tên đăng nhập", "Mật khẩu" };

        /// <summary>
        /// Khởi tạo form đăng nhập mới
        /// </summary>
        public LoginForm() : base("[ĐĂNG NHẬP]", 70, 12)  // Tăng kích thước
        {
        }

        /// <summary>
        /// Tính toán vị trí an toàn cho form
        /// </summary>
        private (int left, int top) GetSafePosition()
        {
            // Đảm bảo có ít nhất chiều cao tối thiểu cho console
            int minConsoleHeight = Height + 6; // +6 để có thể hiển thị hướng dẫn
            int minConsoleWidth = Width + 4;

            try
            {
                if (Console.WindowHeight < minConsoleHeight)
                {
                    Console.SetWindowSize(Console.WindowWidth, minConsoleHeight);
                }
                if (Console.WindowWidth < minConsoleWidth)
                {
                    Console.SetWindowSize(minConsoleWidth, Console.WindowHeight);
                }
            }
            catch
            {
                // Nếu không thể resize, tiếp tục với kích thước hiện tại
            }

            int left = Math.Max(0, (Console.WindowWidth - Width) / 2);
            int top = Math.Max(1, Math.Min((Console.WindowHeight - Height) / 2, Console.WindowHeight - Height - 3));

            return (left, top);
        }

        /// <summary>
        /// Hiển thị form đăng nhập với giao diện riêng
        /// </summary>
        protected override void DrawForm()
        {
            Console.Clear();

            // Tính toán vị trí an toàn
            var (left, top) = GetSafePosition();

            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleDrawing.DrawBoxWithTitle(Width, Height, left, top, "[ĐĂNG NHẬP]");
            Console.ResetColor();
        }

        /// <summary>
        /// Vẽ các trường với giá trị hiện tại
        /// </summary>
        private void DrawFields(string[] values)
        {
            var (left, top) = GetSafePosition();

            // Vẽ các trường nhập liệu
            for (int i = 0; i < fields.Length; i++)
            {
                Console.SetCursorPosition(left + 4, top + 3 + i * 2); // Tăng khoảng cách giữa các dòng

                if (i == selectedField)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("> ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ");
                }

                Console.Write(fields[i] + ": ");

                // Hiển thị giá trị đã nhập ngay sau dấu ":"
                if (!string.IsNullOrEmpty(values[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string displayValue = (i == 1) ? new string('*', values[i].Length) : values[i];
                    Console.Write(displayValue);
                }
                else if (i == selectedField)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("_"); // Hiển thị cursor
                }

                Console.ResetColor();
            }

            // Vẽ hướng dẫn
            string instructions = "↑↓: Di chuyển | Enter: Nhập | Esc: Thoát";
            Console.SetCursorPosition(left + (Width - instructions.Length) / 2, top + Height + 1);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(instructions);
            Console.ResetColor();
        }

        /// <summary>
        /// Xử lý logic form đăng nhập
        /// </summary>
        /// <returns>Thông tin đăng nhập hoặc null nếu hủy</returns>
        protected override object? ProcessForm()
        {
            string[] values = new string[2];

            while (true)
            {
                DrawForm();
                DrawFields(values);

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedField = (selectedField > 0) ? selectedField - 1 : fields.Length - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedField = (selectedField < fields.Length - 1) ? selectedField + 1 : 0;
                        break;

                    case ConsoleKey.Enter:
                        // Nhập giá trị cho trường được chọn
                        var (left, top) = GetSafePosition();
                        int inputLeft = left + 4 + 2 + fields[selectedField].Length + 2; // Vị trí sau ": "
                        int inputTop = top + 3 + selectedField * 2;

                        Console.SetCursorPosition(inputLeft, inputTop);
                        Console.Write(new string(' ', 30)); // Xóa nội dung cũ
                        Console.SetCursorPosition(inputLeft, inputTop);

                        string? inputValue = selectedField == 1
                            ? ConsoleInput.ReadPassword(32)
                            : ConsoleInput.ReadAlphaNumeric(32);

                        if (inputValue != null && !string.IsNullOrEmpty(inputValue))
                        {
                            values[selectedField] = inputValue;

                            // Tự động chuyển sang trường tiếp theo
                            if (selectedField < fields.Length - 1)
                            {
                                selectedField++;
                            }

                            // Kiểm tra nếu đã nhập đủ thông tin
                            if (!string.IsNullOrEmpty(values[0]) && !string.IsNullOrEmpty(values[1]))
                            {
                                string successMessage = "Đăng nhập thành công!";
                                ShowMessage(successMessage, false);

                                System.Threading.Thread.Sleep(1000);
                                return (values[0], values[1]);
                            }
                        }
                        break;

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }

        /// <summary>
        /// Hiển thị form đăng nhập và trả về thông tin đăng nhập
        /// </summary>
        /// <returns>Thông tin đăng nhập hoặc null nếu hủy</returns>
        public new (string Username, string Password)? Show()
        {
            return (base.Show() as (string, string)?);
        }
    }
}
