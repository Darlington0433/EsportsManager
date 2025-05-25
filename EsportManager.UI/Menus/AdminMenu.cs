using System;
using System.Collections.Generic;
using EsportManager.UI.Interfaces;
using EsportManager.BLL.Interfaces;
using EsportManager.Models;
using System.Linq;
using EsportManager.Utils;

namespace EsportManager.UI.Screens
{
    public class AdminMenuScreen : IScreen
    {
        private readonly IUserService _userService;

        public AdminMenuScreen(IUserService userService)
        {
            _userService = userService;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                DrawSpecialHeader("ESPORT MANAGER", "[ADMIN PANEL]");

                try
                {
                    string[] options = {
                        "1. Quản lý người dùng",
                        "2. Quản lý giải đấu",
                        "3. Quản lý đội tuyển",
                        "4. Thống kê hệ thống",
                        "5. Cấu hình hệ thống",
                        "6. Đăng xuất"
                    };

                    DrawSimpleMenu(options);

                    int selectedIndex = HandleMenuSelection(options);

                    switch (selectedIndex)
                    {
                        case 0:
                            ManageUsers();
                            break;
                        case 1:
                            ShowInfoMessage("Chức năng quản lý giải đấu đang được phát triển...");
                            break;
                        case 2:
                            ShowInfoMessage("Chức năng quản lý đội tuyển đang được phát triển...");
                            break;
                        case 3:
                            ShowStatistics();
                            break;
                        case 4:
                            ShowInfoMessage("Chức năng cấu hình hệ thống đang được phát triển...");
                            break;
                        case 5:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("Lỗi hiển thị menu: " + ex.Message);
                    Console.WriteLine("Vui lòng mở rộng cửa sổ console và nhấn phím bất kỳ để thử lại...");
                    Console.ReadKey(true);
                }
            }
        }

        private void ManageUsers()
        {
            Console.Clear();
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                DrawSpecialHeader("ESPORT MANAGER", "[QUẢN LÝ NGƯỜI DÙNG]");

                List<User> users = _userService.GetAllUsers();

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition((Console.WindowWidth - 25) / 2, 16);
                Console.WriteLine($"Tổng số người dùng: {users.Count}");

                DrawSimpleTable(users);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 2);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi: " + ex.Message);
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

        private void DrawSimpleMenu(string[] options)
        {
            int startY = 18;

            for (int i = 0; i < options.Length; i++)
            {
                SafeSetCursorPosition(Math.Max(0, (Console.WindowWidth - options[i].Length) / 2), startY + i);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(options[i]);
            }

            SafeSetCursorPosition(Math.Max(0, (Console.WindowWidth - 38) / 2), startY + options.Length + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("↑↓: Di chuyển   Enter: Chọn");
        }

        private int HandleMenuSelection(string[] options)
        {
            int selectedIndex = 0;
            ConsoleKey key;
            int startY = 18;

            do
            {
                for (int i = 0; i < options.Length; i++)
                {
                    SafeSetCursorPosition(Math.Max(0, (Console.WindowWidth - options[i].Length) / 2), startY + i);

                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(options[i]);
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(options[i]);
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0;
                }

            } while (key != ConsoleKey.Enter);

            return selectedIndex;
        }

        private void DrawSimpleTable(List<User> users)
        {
            int startY = 18;
            int maxUsers = Math.Min(users.Count, 10);

            SafeSetCursorPosition(10, startY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ID   Tên người dùng      Email                 Vai trò    Trạng thái");
            Console.WriteLine(new string('-', 80).PadLeft(10 + 80));

            for (int i = 0; i < maxUsers; i++)
            {
                User user = users[i];
                SafeSetCursorPosition(10, startY + 2 + i);
                Console.ForegroundColor = ConsoleColor.White;

                string id = user.UserID.ToString().PadRight(5);
                string name = (user.DisplayName?.Length > 18 ? user.DisplayName.Substring(0, 15) + "..." : user.DisplayName?.PadRight(18) ?? "".PadRight(18));
                string email = (user.Email?.Length > 20 ? user.Email.Substring(0, 17) + "..." : user.Email?.PadRight(20) ?? "".PadRight(20));
                string role = (user.Role?.PadRight(10) ?? "".PadRight(10));
                string status = user.Status ?? "";

                Console.Write($"{id}{name}{email}{role}{status}");
            }

            if (users.Count > maxUsers)
            {
                SafeSetCursorPosition(10, startY + 2 + maxUsers);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"... và {users.Count - maxUsers} người dùng khác");
            }
        }

        private void ShowStatistics()
        {
            Console.Clear();
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                DrawSpecialHeader("ESPORT MANAGER", "[THỐNG KÊ HỆ THỐNG]");

                List<User> users = _userService.GetAllUsers();

                int totalUsers = users.Count;
                int activeUsers = users.Count(u => u.Status == "Active");
                int pendingUsers = users.Count(u => u.Status == "Pending");
                int blockedUsers = users.Count(u => u.Status == "Blocked");

                int startY = 18;
                DrawSimpleBar("Tổng số người dùng", totalUsers, totalUsers > 0 ? totalUsers : 1, ConsoleColor.Cyan, startY);
                DrawSimpleBar("Tài khoản đang hoạt động", activeUsers, totalUsers > 0 ? totalUsers : 1, ConsoleColor.Green, startY + 3);
                DrawSimpleBar("Tài khoản chờ duyệt", pendingUsers, totalUsers > 0 ? totalUsers : 1, ConsoleColor.Yellow, startY + 6);
                DrawSimpleBar("Tài khoản bị khóa", blockedUsers, totalUsers > 0 ? totalUsers : 1, ConsoleColor.Red, startY + 9);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition((Console.WindowWidth - 40) / 2, startY + 12);
                Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Lỗi: " + ex.Message);
            }
        }

        private void DrawSimpleBar(string label, int value, int maxValue, ConsoleColor color, int y)
        {
            int barLength = 40;
            int x = 20;

            SafeSetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(label.PadRight(30));

            SafeSetCursorPosition(x + 30, y);
            Console.Write("[");

            int filledLength = (int)Math.Ceiling((double)value / maxValue * barLength);
            filledLength = Math.Min(filledLength, barLength);

            Console.ForegroundColor = color;
            Console.Write(new string('#', filledLength));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('-', barLength - filledLength));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"] {value}/{maxValue}");
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

        private void ShowInfoMessage(string message)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("\n\n");
            Console.WriteLine(CenterText("=== THÔNG BÁO ==="));
            Console.WriteLine();
            Console.WriteLine(CenterText(message));
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n" + CenterText("Nhấn phím bất kỳ để quay lại..."));
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