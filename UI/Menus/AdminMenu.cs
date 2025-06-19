using System;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace EsportManager.UI.Menus
{
    public static class AdminMenu
    {
        public static void Show()
        {
            System.Console.Clear();

            // FIGlet title
            var figlet = FigletFont.Default;
            var figletText = new Figlet(figlet).ToAscii("ESPORTS MANAGER");
            int maxFigletWidth = 0;
            foreach (var line in figletText.ToString().Split('\n'))
                if (line.Length > maxFigletWidth) maxFigletWidth = line.Length;

            string menuTitle = "[MENU ADMIN]";
            int contentWidth = Math.Max(50, Math.Max(maxFigletWidth, menuTitle.Length + 4));
            string horizontal = new string('═', contentWidth);

            string[] options = {
                "1. Quản lý người dùng",
                "2. Quản lý giải đấu",
                "3. Quản lý đội",
                "4. Thống kê",
                "0. Đăng xuất"
            };
            int selected = 0;
            ConsoleKeyInfo key;
            while (true)
            {
                System.Console.Clear();
                // Draw top border
                System.Console.WriteLine("╔" + horizontal + "╗");
                // Empty line
                System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

                // FIGlet title centered với màu #8AFFEF
                Color figletColor = ColorTranslator.FromHtml("#8AFFEF");
                foreach (var line in figletText.ToString().Split('\n'))
                {
                    string trimmed = line.TrimEnd('\r');
                    int pad = Math.Max(0, (contentWidth - trimmed.Length) / 2);
                    if (trimmed.Length > 0)
                    {
                        System.Console.Write("║" + new string(' ', pad));
                        Console.Write(trimmed, figletColor);
                        System.Console.WriteLine(new string(' ', contentWidth - pad - trimmed.Length) + "║");
                    }
                }
                // Empty line
                System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

                // [MENU ADMIN] centered, yellow
                int menuPad = (contentWidth - menuTitle.Length) / 2;
                System.Console.Write("║" + new string(' ', menuPad));
                Console.Write(menuTitle, Color.Yellow);
                System.Console.WriteLine(new string(' ', contentWidth - menuPad - menuTitle.Length) + "║");

                // Empty line
                System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

                // Vẽ lại menu options
                for (int i = 0; i < options.Length; i++)
                {
                    int pad = (contentWidth - options[i].Length) / 2;
                    System.Console.Write("║" + new string(' ', pad));
                    if (i == selected)
                    {
                        Console.Write(options[i], Color.LimeGreen);
                        System.Console.Write(" ");
                        Console.Write("▶", Color.Yellow);
                        System.Console.Write(new string(' ', contentWidth - pad - options[i].Length - 2));
                    }
                    else
                    {
                        Console.Write(options[i], Color.White);
                        System.Console.Write(new string(' ', contentWidth - pad - options[i].Length));
                    }
                    System.Console.WriteLine("║");
                }
                // Draw bottom border
                System.Console.WriteLine("╚" + horizontal + "╝");
                // Prompt
                Console.Write("→", Color.Cyan);
                System.Console.Write(" Dùng ↑/↓ để chọn, Enter để xác nhận. ");
                key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selected = (selected - 1 + options.Length) % options.Length;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selected = (selected + 1) % options.Length;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (selected == options.Length - 1) // Đăng xuất
                        return;
                    // Xử lý các chức năng khác ở đây nếu muốn
                }
            }
        }
    }
} 