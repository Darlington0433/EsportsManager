using System;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace EsportManager.UI.Menus
{
    public static class AdminMenu
    {
        private static readonly string TitleArt = @"
███████╗███████╗██████╗  ██████╗ ██████╗ ████████╗███████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ 
██╔════╝██╔════╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗
█████╗  ███████╗██████╔╝██║   ██║██████╔╝   ██║   ███████╗    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝
██╔══╝  ╚════██║██╔═══╝ ██║   ██║██╔══██╗   ██║   ╚════██║    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗
███████╗███████║██║     ╚██████╔╝██║  ██║   ██║   ███████║    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║
╚══════╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝";

        private static readonly Color[] TitleGradient = new[]
        {
            ColorTranslator.FromHtml("#87CEEB"),  // Sky Blue
            ColorTranslator.FromHtml("#98FB98"),  // Pale Green
            ColorTranslator.FromHtml("#DDA0DD"),  // Plum
            ColorTranslator.FromHtml("#FFB6C1")   // Light Pink
        };

        public static void Show()
        {
            System.Console.Clear();

            string[] artLines = TitleArt.Split('\n');
            int maxArtWidth = 0;
            foreach (var line in artLines)
                if (line.Length > maxArtWidth) maxArtWidth = line.Length;

            string menuTitle = "[MENU ADMIN]";
            int contentWidth = Math.Max(50, Math.Max(maxArtWidth, menuTitle.Length + 4));
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

                // Title Art với gradient màu
                int currentLine = Console.CursorTop;
                foreach (var line in artLines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        System.Console.Write("║");
                        int pad = (contentWidth - line.Length) / 2;
                        System.Console.Write(new string(' ', pad));
                        Color gradientColor = TitleGradient[(Console.CursorTop - currentLine) % TitleGradient.Length];
                        Console.Write(line, gradientColor);
                        System.Console.WriteLine(new string(' ', contentWidth - pad - line.Length) + "║");
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