using System;
using System.Linq;
using EsportsManager.UI.Legacy.Utilities;

namespace EsportsManager.UI.Legacy.Forms
{
    /// <summary>
    /// Xử lý hiển thị và tương tác với form quên mật khẩu
    /// </summary>
    public class ForgotPasswordForm : BaseForm
    {
        private static int selectedField = 0;
        private readonly string[] fields = { "Tên đăng nhập", "Email", "Câu trả lời bảo mật" };
        
        /// <summary>
        /// Khởi tạo form quên mật khẩu
        /// </summary>
        public ForgotPasswordForm() : base("[QUÊN MẬT KHẨU]", 80, 18)  // Tăng chiều cao để đủ chỗ cho 3 trường
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
        /// Hiển thị form quên mật khẩu với giao diện riêng
        /// </summary>
        protected override void DrawForm()
        {
            Console.Clear();
            
            // Tính toán vị trí an toàn
            var (left, top) = GetSafePosition();
            
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleDrawing.DrawBoxWithTitle(Width, Height, left, top, "[QUÊN MẬT KHẨU]");
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
                    Console.Write(values[i]);
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
        /// Xử lý logic form quên mật khẩu
        /// </summary>
        /// <returns>Thông tin đặt lại mật khẩu hoặc null nếu hủy</returns>
        protected override object? ProcessForm()
        {
            string[] values = new string[3];
            
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
                        Console.Write(new string(' ', 40)); // Xóa nội dung cũ
                        Console.SetCursorPosition(inputLeft, inputTop);
                        
                        string? inputValue = null;
                        
                        if (selectedField == 0) // Tên đăng nhập
                        {
                            inputValue = ConsoleInput.ReadAlphaNumeric(32);
                        }
                        else if (selectedField == 1) // Email
                        {
                            inputValue = ConsoleInput.ReadAnyString(64);
                            if (inputValue != null && !ConsoleInput.IsValidEmail(inputValue))
                            {
                                ShowMessage("Email không hợp lệ!", true);
                                inputValue = null;
                            }
                        }
                        else // Câu trả lời bảo mật
                        {
                            inputValue = ConsoleInput.ReadAnyString(64);
                        }
                        
                        if (inputValue != null && !string.IsNullOrEmpty(inputValue))
                        {
                            values[selectedField] = inputValue;
                            
                            // Tự động chuyển sang trường tiếp theo
                            if (selectedField < fields.Length - 1)
                            {
                                selectedField++;
                            }
                            
                            // Kiểm tra nếu đã nhập đủ thông tin
                            if (!string.IsNullOrEmpty(values[0]) && !string.IsNullOrEmpty(values[1]) && !string.IsNullOrEmpty(values[2]))
                            {
                                string successMessage = "Yêu cầu đặt lại mật khẩu đã được gửi!";
                                ShowMessage(successMessage, false);
                                
                                System.Threading.Thread.Sleep(1000);
                                return (values[0], values[1], values[2]);
                            }
                        }
                        break;
                        
                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }
        
        /// <summary>
        /// Hiển thị form quên mật khẩu và trả về thông tin đặt lại mật khẩu
        /// </summary>
        /// <returns>Thông tin đặt lại mật khẩu hoặc null nếu hủy</returns>
        public new (string Username, string Email, string SecurityAnswer)? Show()
        {
            return (base.Show() as (string, string, string)?);
        }
        
        public (string Username, string Email, string SecurityAnswer)? ShowForgotPasswordForm()
        {
            return (ProcessForm() as (string, string, string)?);
        }
    }
}
