using System;
using EsportManager.UI.Interfaces;
using EsportManager.BLL.Interfaces;
using System.Collections.Generic;

namespace EsportManager.UI.Screens
{
    public class ForgotPasswordScreen : IScreen
    {
        private readonly IUserService _userService;

        public ForgotPasswordScreen(IUserService userService)
        {
            _userService = userService;
        }

        public void Show()
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            DrawSpecialHeader("ESPORT MANAGER", "[QUÊN MẬT KHẨU]");

            try
            {
                DrawForgotPasswordForm();

                int formStartLine = 18;
                int centerX = Math.Max(0, (Console.WindowWidth - 40) / 2);
                SafeSetCursorPosition(centerX, formStartLine + 2);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("→ Tên đăng nhập (@TênID): ");
                Console.ForegroundColor = ConsoleColor.White;
                string username = Console.ReadLine();

                string securityQuestion = _userService.GetSecurityQuestion(username);
                if (string.IsNullOrEmpty(securityQuestion))
                {
                    ShowErrorMessage("Không tìm thấy tài khoản với tên đăng nhập này!");
                    return;
                }

                SafeSetCursorPosition(centerX, formStartLine + 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("→ Câu hỏi bảo mật: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(securityQuestion);
                Console.ForegroundColor = ConsoleColor.White;

                SafeSetCursorPosition(centerX, formStartLine + 4);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("→ Câu trả lời: ");
                Console.ForegroundColor = ConsoleColor.White;
                string answer = Console.ReadLine();

                bool success = _userService.ResetPassword(username, answer);
                if (success)
                {
                    ShowSuccessMessage("Mật khẩu đã được đặt lại thành 'player123'.\nVui lòng đăng nhập với mật khẩu mới và đổi mật khẩu ngay lập tức.");
                    DrawTransitionEffect();
                    return;
                }
                else
                {
                    ShowErrorMessage("Câu trả lời không đúng! Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Lỗi hiển thị form quên mật khẩu: " + ex.Message);
                Console.WriteLine("Vui lòng mở rộng cửa sổ console và nhấn phím bất kỳ để thử lại...");
                Console.ReadKey(true);
                Show();
            }
        }

        private void DrawSpecialHeader(string title, string subtitle)
        {
            Console.Clear();
            Console.CursorVisible = false;

            int windowWidth = Console.WindowWidth;
            int startY = 5;

            int borderWidth = Math.Max(10, windowWidth - 4);

            string topBorder = "╔" + new string('═', borderWidth - 2) + "╗";
            string bottomBorder = "╚" + new string('═', borderWidth - 2) + "╝";
            string sideBorder = "║" + new string(' ', borderWidth - 2) + "║";

            int leftPosition = Math.Max(0, (windowWidth - borderWidth) / 2);

            Console.ForegroundColor = ConsoleColor.DarkGray;

            SafeSetCursorPosition(leftPosition, startY);
            Console.Write(topBorder);

            for (int i = 1; i <= 8; i++)
            {
                SafeSetCursorPosition(leftPosition, startY + i);
                Console.Write(sideBorder);
            }

            SafeSetCursorPosition(leftPosition, startY + 9);
            Console.Write(bottomBorder);

            Console.ForegroundColor = ConsoleColor.Cyan;

            string[] pixelFont = CreatePixelFont("ESPORT MANAGER");
            int titleWidth = pixelFont[0].Length;
            int centerX = Math.Max(0, (windowWidth - titleWidth) / 2);

            for (int i = 0; i < pixelFont.Length; i++)
            {
                SafeSetCursorPosition(centerX, startY + 2 + i);
                Console.Write(pixelFont[i]);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            SafeSetCursorPosition(Math.Max(0, (windowWidth - subtitle.Length) / 2), startY + 7);
            Console.Write(subtitle);

            Console.ForegroundColor = ConsoleColor.White;
        }

        private string[] CreatePixelFont(string text)
        {
            Dictionary<char, string[]> pixelLetters = new Dictionary<char, string[]>
            {
                {'E', new string[] {"█████", "█    ", "████ ", "█    ", "█████"}},
                {'S', new string[] {"█████", "█    ", "█████", "    █", "█████"}},
                {'P', new string[] {"█████", "█   █", "█████", "█    ", "█    "}},
                {'O', new string[] {"█████", "█   █", "█   █", "█   █", "█████"}},
                {'R', new string[] {"█████", "█   █", "████ ", "█  █ ", "█   █"}},
                {'T', new string[] {"█████", "  █  ", "  █  ", "  █  ", "  █  "}},
                {'M', new string[] {"█   █", "██ ██", "█ █ █", "█   █", "█   █"}},
                {'A', new string[] {"█████", "█   █", "█████", "█   █", "█   █"}},
                {'N', new string[] {"█   █", "██  █", "█ █ █", "█  ██", "█   █"}},
                {'G', new string[] {"█████", "█    ", "█  ██", "█   █", "█████"}},
                {' ', new string[] {"     ", "     ", "     ", "     ", "     "}}
            };

            string[] result = new string[5];
            for (int i = 0; i < 5; i++)
                result[i] = "";

            foreach (char c in text)
            {
                char upperC = char.ToUpper(c);
                if (pixelLetters.ContainsKey(upperC))
                {
                    string[] letterPattern = pixelLetters[upperC];
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += letterPattern[i] + " ";
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] += "      ";
                    }
                }
            }

            return result;
        }

        private void SafeSetCursorPosition(int left, int top)
        {
            try
            {
                int safeLeft = Math.Max(0, Math.Min(left, Console.WindowWidth - 1));
                int safeTop = Math.Max(0, Math.Min(top, Console.WindowHeight - 1));

                Console.SetCursorPosition(safeLeft, safeTop);
            }
            catch
            {
            }
        }

        private void DrawForgotPasswordForm()
        {
            int windowWidth = Console.WindowWidth;
            int startY = 18;

            int formWidth = 50;
            int leftPosition = Math.Max(0, (windowWidth - formWidth) / 2);

            Console.ForegroundColor = ConsoleColor.DarkCyan;

            string topBorder = "┌" + new string('─', formWidth - 2) + "┐";
            string bottomBorder = "└" + new string('─', formWidth - 2) + "┘";
            string sideBorder = "│" + new string(' ', formWidth - 2) + "│";
            string titleBar = "│" + "QUÊN MẬT KHẨU".PadLeft((formWidth + "QUÊN MẬT KHẨU".Length) / 2 - 1).PadRight(formWidth - 2) + "│";
            string separator = "├" + new string('─', formWidth - 2) + "┤";

            SafeSetCursorPosition(leftPosition, startY - 2);
            Console.Write(topBorder);

            SafeSetCursorPosition(leftPosition, startY - 1);
            Console.Write(titleBar);

            SafeSetCursorPosition(leftPosition, startY);
            Console.Write(separator);

            for (int i = 1; i <= 5; i++)
            {
                SafeSetCursorPosition(leftPosition, startY + i);
                Console.Write(sideBorder);
            }

            SafeSetCursorPosition(leftPosition, startY + 6);
            Console.Write(separator);

            SafeSetCursorPosition(leftPosition, startY + 7);
            Console.Write("│" + "ESC: Hủy bỏ   ENTER: Xác nhận".PadLeft((formWidth + "ESC: Hủy bỏ   ENTER: Xác nhận".Length) / 2 - 1).PadRight(formWidth - 2) + "│");

            SafeSetCursorPosition(leftPosition, startY + 8);
            Console.Write(bottomBorder);

            Console.ForegroundColor = ConsoleColor.White;
        }

        private void DrawTransitionEffect()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            int width = 40;
            string loading = "ĐANG CHUYỂN TRANG";

            for (int i = 0; i < 3; i++)
            {
                Console.Clear();
                Console.WriteLine("\n\n");

                string topBorder = "┌" + new string('─', width) + "┐";
                string bottomBorder = "└" + new string('─', width) + "┘";
                string emptyBorder = "│" + new string(' ', width) + "│";

                Console.WriteLine(CenterText(topBorder));
                Console.WriteLine(CenterText(emptyBorder));

                string dots = new string('.', i + 1);
                string paddedText = loading + dots + new string(' ', width - loading.Length - dots.Length);

                Console.WriteLine(CenterText("│" + paddedText + "│"));
                Console.WriteLine(CenterText(emptyBorder));
                Console.WriteLine(CenterText(bottomBorder));

                System.Threading.Thread.Sleep(300);
            }
        }

        private void ShowSuccessMessage(string message)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("\n\n");
            Console.WriteLine(CenterText("=== THÔNG BÁO ==="));
            Console.WriteLine();
            Console.WriteLine(CenterText(message));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n" + CenterText("Nhấn phím bất kỳ để tiếp tục..."));
            Console.ReadKey(true);
        }

        private void ShowErrorMessage(string message)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("\n\n");
            Console.WriteLine(CenterText("=== THÔNG BÁO ==="));
            Console.WriteLine();
            Console.WriteLine(CenterText(message));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n" + CenterText("Nhấn phím bất kỳ để thử lại..."));
            Console.ReadKey(true);
        }

        private string CenterText(string text)
        {
            int screenWidth = Console.WindowWidth;

            if (text.Length >= screenWidth)
            {
                return text.Substring(0, screenWidth - 3) + "...";
            }

            int leftPadding = (screenWidth - text.Length) / 2;
            return new string(' ', Math.Max(0, leftPadding)) + text;
        }
    }
}