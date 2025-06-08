using System;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;
using System.Linq;

namespace EsportManager.UI
{
    public static class UIHelper
    {
        private static readonly string[] SecurityQuestions = new[]
        {
            "Tên trường tiểu học đầu tiên của bạn là gì?",
            "Họ và tên đệm của mẹ bạn là gì?",
            "Thú cưng đầu tiên của bạn tên gì?",
            "Bạn sinh ra ở thành phố nào?",
            "Môn thể thao yêu thích của bạn là gì?"
        };
        
        private static int selectedSecurityQuestion = 0;

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

        private static void DrawBox(int width, int height, int left, int top)
        {
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
        }

        private static void DrawFormContent(string[] fields, int selectedField, string[] values, int width, int left, int top)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Console.SetCursorPosition(left + 4, top + i);
                string value = values[i];
                string line = $"{fields[i]}: {value}";

                if (i == selectedField)
                {
                    Console.Write("▶ ", Color.Yellow);
                    Console.Write(line, Color.Yellow);
                }
                else
                {
                    Console.Write("  ");
                    Console.Write(line, Color.DarkGray);
                }
            }
        }

        public static void ShowWelcomeScreen()
        {
            while (true)
            {
                ShowFullScreen(
                    figletTitle: "ESPORTS MANAGER",
                    subTitle: "[MENU CHÍNH]",
                    contentLines: GetMainMenuContent(),
                    menuSelect: true
                );
            }
        }

        // Hiển thị border lớn, FIGlet, tiêu đề nhỏ, nội dung (menu hoặc form)
        private static void ShowFullScreen(string figletTitle, string subTitle, string[] contentLines, bool menuSelect = false, int selected = 0)
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;
            string[] esportsArt = new[]
            {
                "███████╗███████╗██████╗  ██████╗ ██████╗ ████████╗███████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ ",
                "██╔════╝██╔════╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗",
                "█████╗  ███████╗██████╔╝██║   ██║██████╔╝   ██║   ███████╗    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝",
                "██╔══╝  ╚════██║██╔═══╝ ██║   ██║██╔══██╗   ██║   ╚════██║    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗",
                "███████╗███████║██║     ╚██████╔╝██║  ██║   ██║   ███████║    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║",
                "╚══════╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝"
            };
            Color[] gradient = new[]
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.LimeGreen,
                Color.DodgerBlue,
                Color.MediumPurple
            };
            int figletWidth = GetMaxLineWidth(figletTitle);
            int artMaxWidth = esportsArt.Max(line => line.Length);
            int contentWidth = Math.Max(figletWidth, 50);
            foreach (var line in contentLines) contentWidth = Math.Max(contentWidth, line.Length + 4);
            contentWidth = Math.Min(contentWidth, windowWidth - 4);
            if (contentWidth < artMaxWidth) contentWidth = artMaxWidth;
            int leftPad = Math.Max(0, (windowWidth - contentWidth - 2) / 2);
            int totalHeight = 7 + contentLines.Length + esportsArt.Length;
            int topPad = Math.Max(0, (Console.WindowHeight - totalHeight) / 2);

            // Padding trên
            for (int i = 0; i < topPad; i++) Console.WriteLine();
            // Border top
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine("╔" + new string('═', contentWidth) + "╗");
            // FIGlet
            int artPad = leftPad;
            for (int i = 0; i < esportsArt.Length; i++)
            {
                string line = esportsArt[i];
                string centered = CenterText(line, contentWidth);
                Console.SetCursorPosition(artPad, Console.CursorTop);
                Console.WriteLine("║" + centered + "║", gradient[i % gradient.Length]);
            }
            // SubTitle
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine("║" + CenterText(subTitle, contentWidth) + "║", Color.Khaki);
            Console.SetCursorPosition(leftPad, Console.CursorTop);
            Console.WriteLine("║" + new string(' ', contentWidth) + "║");
            // Content
            if (menuSelect)
            {
                int menuSelected = 0;
                while (true)
                {
                    for (int i = 0; i < contentLines.Length; i++)
                    {
                        Console.SetCursorPosition(leftPad, Console.CursorTop);
                        Console.Write("║");
                        string lineText;
                        if (i == menuSelected)
                            lineText = $"> {contentLines[i]} ▶";
                        else
                            lineText = $"  {contentLines[i]}";
                        int pad = (contentWidth - lineText.Length) / 2;
                        if (pad < 0) pad = 0;
                        Console.Write(new string(' ', pad));
                        if (i == menuSelected)
                            Console.Write(lineText, Color.Aqua);
                        else
                            Console.Write(lineText, Color.White);
                        Console.Write(new string(' ', contentWidth - pad - lineText.Length));
                        Console.WriteLine("║");
                    }
                    // Empty line
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("║" + new string(' ', contentWidth) + "║");
                    // Border bottom
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("╚" + new string('═', contentWidth) + "╝");
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("Dùng ↑/↓ để chọn, Enter để xác nhận.");
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.UpArrow)
                        menuSelected = (menuSelected - 1 + contentLines.Length) % contentLines.Length;
                    else if (key.Key == ConsoleKey.DownArrow)
                        menuSelected = (menuSelected + 1) % contentLines.Length;
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        switch (menuSelected)
                        {
                            case 0:
                                ShowFullScreen(figletTitle, "[ĐĂNG NHẬP]", GetLoginFormContent(), false);
                                return;
                            case 1:
                                ShowFullScreen(figletTitle, "[ĐĂNG KÝ]", GetRegisterFormContent(), false);
                                return;
                            case 2:
                                ShowFullScreen(figletTitle, "[QUÊN MẬT KHẨU]", GetForgotFormContent(), false);
                                return;
                            case 3:
                                Environment.Exit(0);
                                return;
                        }
                    }
                    // Redraw border and menu
                    Console.SetCursorPosition(0, Console.CursorTop - contentLines.Length - 3);
                }
            }
            else
            {
                // Xác định loại form dựa vào subTitle
                if (subTitle == "[ĐĂNG NHẬP]")
                {
                    string username = "";
                    string password = "";
                    int field = 0;
                    string[] fields = { "Tên đăng nhập", "Mật khẩu" };
                    while (true)
                    {
                        Console.Clear();
                        // Vẽ lại border, Title Art, subTitle
                        int winW = Console.WindowWidth;
                        string[] artLines = TitleArt.Split('\n');
                        int artWidth = 0;
                        foreach (var line in artLines)
                        {
                            if (line.Length > artWidth) artWidth = line.Length;
                        }
                        int cWidth = Math.Max(artWidth, 50);
                        cWidth = Math.Min(cWidth, winW - 4);
                        int lPad = Math.Max(0, (winW - cWidth - 2) / 2);
                        int tHeight = 7 + artLines.Length;
                        int tPad = Math.Max(0, (Console.WindowHeight - tHeight) / 2);
                        
                        for (int i = 0; i < tPad; i++) Console.WriteLine();
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╔" + new string('═', cWidth) + "╗");

                        // Title Art với gradient
                        int currentLine = Console.CursorTop;
                        foreach (var line in artLines)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                Console.SetCursorPosition(lPad, Console.CursorTop);
                                Color gradientColor = TitleGradient[(Console.CursorTop - currentLine) % TitleGradient.Length];
                                string centered = CenterText(line.TrimEnd('\r'), cWidth);
                                Console.WriteLine($"║{centered}║", gradientColor);
                            }
                        }

                        // SubTitle
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{CenterText(subTitle, cWidth)}║", Color.Khaki);
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");

                        // Form fields
                        for (int i = 0; i < fields.Length; i++)
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            string label = $"→ {fields[i]}: ";
                            string value = i == 1 ? new string('*', password.Length) : username;
                            string line = label + value;
                            string content = i == field ? $"▶ {line}" : $"  {line}";
                            string centered = CenterText(content, cWidth);
                            Console.WriteLine($"║{centered}║", i == field ? Color.Yellow : Color.DarkGray);
                        }

                        // Bottom border
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╚" + new string('═', cWidth) + "╝");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.");
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Tab || (key.Key == ConsoleKey.DownArrow))
                            field = (field + 1) % fields.Length;
                        else if (key.Key == ConsoleKey.UpArrow || (key.Modifiers.HasFlag(ConsoleModifiers.Shift) && key.Key == ConsoleKey.Tab))
                            field = (field - 1 + fields.Length) % fields.Length;
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            Console.SetCursorPosition(lPad + (cWidth - 30) / 2, Console.CursorTop);
                            if (field == 0)
                            {
                                Console.Write("Nhập Tên đăng nhập: ");
                                username = Console.ReadLine();
                            }
                            else if (field == 1)
                            {
                                Console.Write("Nhập Mật khẩu: ");
                                password = ReadPassword(20);
                            }
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                        // Nếu đã nhập xong cả 2 trường, cho xác nhận đăng nhập
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            Console.WriteLine(CenterText("[Giả lập] Đăng nhập thành công! Nhấn phím bất kỳ để quay lại menu chính.", cWidth));
                            Console.ReadKey();
                            return;
                        }
                    }
                }
                else if (subTitle == "[ĐĂNG KÝ]")
                {
                    string username = "";
                    string email = "";
                    string password = "";
                    string confirm = "";
                    string answer = "";
                    int field = 0;
                    string[] fields = { "Tên đăng nhập", "Email", "Mật khẩu", "Xác nhận mật khẩu", "Câu trả lời bảo mật" };
                    while (true)
                    {
                        Console.Clear();
                        int winW = Console.WindowWidth;
                        string[] artLines = TitleArt.Split('\n');
                        int artWidth = 0;
                        foreach (var line in artLines)
                        {
                            if (line.Length > artWidth) artWidth = line.Length;
                        }
                        int cWidth = Math.Max(artWidth, 50);
                        cWidth = Math.Min(cWidth, winW - 4);
                        int lPad = Math.Max(0, (winW - cWidth - 2) / 2);
                        int tHeight = 7 + artLines.Length;
                        int tPad = Math.Max(0, (Console.WindowHeight - tHeight) / 2);
                        
                        for (int i = 0; i < tPad; i++) Console.WriteLine();
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╔" + new string('═', cWidth) + "╗");

                        // Title Art với gradient
                        int currentLine = Console.CursorTop;
                        foreach (var line in artLines)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                Console.SetCursorPosition(lPad, Console.CursorTop);
                                Color gradientColor = TitleGradient[(Console.CursorTop - currentLine) % TitleGradient.Length];
                                string centered = CenterText(line.TrimEnd('\r'), cWidth);
                                Console.WriteLine($"║{centered}║", gradientColor);
                            }
                        }

                        // SubTitle
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{CenterText(subTitle, cWidth)}║", Color.Khaki);
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");

                        // Form fields
                        for (int i = 0; i < fields.Length; i++)
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            string label = $"→ {fields[i]}: ";
                            string value = i switch
                            {
                                0 => username,
                                1 => email,
                                2 => new string('*', password.Length),
                                3 => new string('*', confirm.Length),
                                4 => answer,
                                _ => ""
                            };
                            string line = label + value;
                            string content = i == field ? $"▶ {line}" : $"  {line}";
                            string centered = CenterText(content, cWidth);
                            Console.WriteLine($"║{centered}║", i == field ? Color.Yellow : Color.DarkGray);
                        }

                        // Bottom border
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╚" + new string('═', cWidth) + "╝");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.");

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Tab || (key.Key == ConsoleKey.DownArrow))
                            field = (field + 1) % fields.Length;
                        else if (key.Key == ConsoleKey.UpArrow || (key.Modifiers.HasFlag(ConsoleModifiers.Shift) && key.Key == ConsoleKey.Tab))
                            field = (field - 1 + fields.Length) % fields.Length;
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            Console.SetCursorPosition(lPad + (cWidth - 30) / 2, Console.CursorTop);
                            switch (field)
                            {
                                case 0:
                                    Console.Write("Nhập Tên đăng nhập: ");
                                    username = Console.ReadLine();
                                    break;
                                case 1:
                                    Console.Write("Nhập Email: ");
                                    email = Console.ReadLine();
                                    break;
                                case 2:
                                    Console.Write("Nhập Mật khẩu: ");
                                    password = ReadPassword(20);
                                    break;
                                case 3:
                                    Console.Write("Xác nhận Mật khẩu: ");
                                    confirm = ReadPassword(20);
                                    break;
                                case 4:
                                    Console.SetCursorPosition(lPad + 4, Console.CursorTop);
                                    Console.WriteLine("Chọn câu hỏi bảo mật:");
                                    int startY = Console.CursorTop;
                                    for (int i = 0; i < SecurityQuestions.Length; i++)
                                    {
                                        Console.SetCursorPosition(lPad + 4, startY + i);
                                        if (i == selectedSecurityQuestion)
                                            Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                        else
                                            Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                    }
                                    while (true)
                                    {
                                        var qKey = Console.ReadKey(true);
                                        if (qKey.Key == ConsoleKey.UpArrow)
                                        {
                                            selectedSecurityQuestion = (selectedSecurityQuestion - 1 + SecurityQuestions.Length) % SecurityQuestions.Length;
                                            for (int i = 0; i < SecurityQuestions.Length; i++)
                                            {
                                                Console.SetCursorPosition(lPad + 4, startY + i);
                                                if (i == selectedSecurityQuestion)
                                                    Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                                else
                                                    Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                            }
                                        }
                                        else if (qKey.Key == ConsoleKey.DownArrow)
                                        {
                                            selectedSecurityQuestion = (selectedSecurityQuestion + 1) % SecurityQuestions.Length;
                                            for (int i = 0; i < SecurityQuestions.Length; i++)
                                            {
                                                Console.SetCursorPosition(lPad + 4, startY + i);
                                                if (i == selectedSecurityQuestion)
                                                    Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                                else
                                                    Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                            }
                                        }
                                        else if (qKey.Key == ConsoleKey.Enter)
                                        {
                                            Console.SetCursorPosition(lPad + 4, startY + SecurityQuestions.Length + 1);
                                            Console.Write($"Câu hỏi: {SecurityQuestions[selectedSecurityQuestion]}");
                                            Console.SetCursorPosition(lPad + 4, startY + SecurityQuestions.Length + 2);
                                            Console.Write("Câu trả lời của bạn: ");
                                            answer = Console.ReadLine();
                                            break;
                                        }
                                        else if (qKey.Key == ConsoleKey.Escape)
                                        {
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && 
                            !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirm) && 
                            !string.IsNullOrEmpty(answer))
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            string message = "[Giả lập] Đăng ký thành công! Nhấn phím bất kỳ để quay lại menu chính.";
                            Console.WriteLine($"║{CenterText(message, cWidth)}║", Color.Green);
                            Console.ReadKey();
                            return;
                        }
                    }
                }
                else if (subTitle == "[QUÊN MẬT KHẨU]")
                {
                    string username = "";
                    string email = "";
                    string answer = "";
                    int field = 0;
                    string[] fields = { "Tên đăng nhập", "Email", "Câu trả lời bảo mật" };
                    while (true)
                    {
                        Console.Clear();
                        int winW = Console.WindowWidth;
                        string[] artLines = TitleArt.Split('\n');
                        int artWidth = 0;
                        foreach (var line in artLines)
                        {
                            if (line.Length > artWidth) artWidth = line.Length;
                        }
                        int cWidth = Math.Max(artWidth, 50);
                        cWidth = Math.Min(cWidth, winW - 4);
                        int lPad = Math.Max(0, (winW - cWidth - 2) / 2);
                        int tHeight = 7 + artLines.Length;
                        int tPad = Math.Max(0, (Console.WindowHeight - tHeight) / 2);
                        
                        for (int i = 0; i < tPad; i++) Console.WriteLine();
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╔" + new string('═', cWidth) + "╗");

                        // Title Art với gradient
                        int currentLine = Console.CursorTop;
                        foreach (var line in artLines)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                Console.SetCursorPosition(lPad, Console.CursorTop);
                                Color gradientColor = TitleGradient[(Console.CursorTop - currentLine) % TitleGradient.Length];
                                string centered = CenterText(line.TrimEnd('\r'), cWidth);
                                Console.WriteLine($"║{centered}║", gradientColor);
                            }
                        }

                        // SubTitle
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{CenterText(subTitle, cWidth)}║", Color.Khaki);
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");

                        // Form fields
                        for (int i = 0; i < fields.Length; i++)
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            string label = $"→ {fields[i]}: ";
                            string value = i switch
                            {
                                0 => username,
                                1 => email,
                                2 => answer,
                                _ => ""
                            };
                            string line = label + value;
                            string content = i == field ? $"▶ {line}" : $"  {line}";
                            string centered = CenterText(content, cWidth);
                            Console.WriteLine($"║{centered}║", i == field ? Color.Yellow : Color.DarkGray);
                        }

                        // Bottom border
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine($"║{new string(' ', cWidth)}║");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("╚" + new string('═', cWidth) + "╝");
                        Console.SetCursorPosition(lPad, Console.CursorTop);
                        Console.WriteLine("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.");

                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Tab || (key.Key == ConsoleKey.DownArrow))
                            field = (field + 1) % fields.Length;
                        else if (key.Key == ConsoleKey.UpArrow || (key.Modifiers.HasFlag(ConsoleModifiers.Shift) && key.Key == ConsoleKey.Tab))
                            field = (field - 1 + fields.Length) % fields.Length;
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            Console.SetCursorPosition(lPad + (cWidth - 30) / 2, Console.CursorTop);
                            switch (field)
                            {
                                case 0:
                                    Console.Write("Nhập Tên đăng nhập: ");
                                    username = Console.ReadLine();
                                    break;
                                case 1:
                                    Console.Write("Nhập Email: ");
                                    email = Console.ReadLine();
                                    break;
                                case 2:
                                    Console.SetCursorPosition(lPad + 4, Console.CursorTop);
                                    Console.WriteLine("Chọn câu hỏi bảo mật:");
                                    int startY = Console.CursorTop;
                                    for (int i = 0; i < SecurityQuestions.Length; i++)
                                    {
                                        Console.SetCursorPosition(lPad + 4, startY + i);
                                        if (i == selectedSecurityQuestion)
                                            Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                        else
                                            Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                    }
                                    while (true)
                                    {
                                        var qKey = Console.ReadKey(true);
                                        if (qKey.Key == ConsoleKey.UpArrow)
                                        {
                                            selectedSecurityQuestion = (selectedSecurityQuestion - 1 + SecurityQuestions.Length) % SecurityQuestions.Length;
                                            for (int i = 0; i < SecurityQuestions.Length; i++)
                                            {
                                                Console.SetCursorPosition(lPad + 4, startY + i);
                                                if (i == selectedSecurityQuestion)
                                                    Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                                else
                                                    Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                            }
                                        }
                                        else if (qKey.Key == ConsoleKey.DownArrow)
                                        {
                                            selectedSecurityQuestion = (selectedSecurityQuestion + 1) % SecurityQuestions.Length;
                                            for (int i = 0; i < SecurityQuestions.Length; i++)
                                            {
                                                Console.SetCursorPosition(lPad + 4, startY + i);
                                                if (i == selectedSecurityQuestion)
                                                    Console.WriteLine($"▶ {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.Yellow);
                                                else
                                                    Console.WriteLine($"  {i + 1}. {SecurityQuestions[i]}".PadRight(cWidth - 4), Color.DarkGray);
                                            }
                                        }
                                        else if (qKey.Key == ConsoleKey.Enter)
                                        {
                                            Console.SetCursorPosition(lPad + 4, startY + SecurityQuestions.Length + 1);
                                            Console.Write($"Câu hỏi: {SecurityQuestions[selectedSecurityQuestion]}");
                                            Console.SetCursorPosition(lPad + 4, startY + SecurityQuestions.Length + 2);
                                            Console.Write("Câu trả lời của bạn: ");
                                            answer = Console.ReadLine();
                                            break;
                                        }
                                        else if (qKey.Key == ConsoleKey.Escape)
                                        {
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(answer))
                        {
                            Console.SetCursorPosition(lPad, Console.CursorTop);
                            string message = "[Giả lập] Nếu thông tin đúng, bạn sẽ nhận được email đặt lại mật khẩu! Nhấn phím bất kỳ để quay lại menu chính.";
                            Console.WriteLine($"║{CenterText(message, cWidth)}║", Color.Green);
                            Console.ReadKey();
                            return;
                        }
                    }
                }
                else
                {
                    foreach (var line in contentLines)
                    {
                        Console.SetCursorPosition(leftPad, Console.CursorTop);
                        string centered = CenterText(line, contentWidth);
                        Console.WriteLine("║" + centered + "║");
                    }
                    // Empty line
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("║" + new string(' ', contentWidth) + "║");
                    // Border bottom
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("╚" + new string('═', contentWidth) + "╝");
                    Console.SetCursorPosition(leftPad, Console.CursorTop);
                    Console.WriteLine("Nhấn phím bất kỳ để quay lại menu chính.");
                    Console.ReadKey();
                }
            }
        }

        private static string[] GetMainMenuContent() => new[] { "Đăng nhập", "Đăng ký", "Quên mật khẩu", "Thoát" };
        private static string[] GetLoginFormContent() => new[] { "→ Tên đăng nhập: ", "→ Mật khẩu: " };
        private static string[] GetRegisterFormContent() => new[] { "→ Tên đăng nhập: ", "→ Email: ", "→ Mật khẩu: ", "→ Xác nhận mật khẩu: ", "→ Câu trả lời bảo mật: " };
        private static string[] GetForgotFormContent() => new[] { "→ Tên đăng nhập: ", "→ Email: ", "→ Câu trả lời bảo mật: " };

        private static string GetFigletText(string title)
        {
            var figlet = FigletFont.Default;
            return new Figlet(figlet).ToAscii(title).ToString();
        }
        private static int GetMaxLineWidth(string text)
        {
            int max = 0;
            foreach (var line in text.Split('\n'))
                if (line.Length > max) max = line.Length;
            return max;
        }

        private static string CenterText(string text, int width)
        {
            if (width <= text.Length) return text;
            int pad = (width - text.Length) / 2;
            return new string(' ', pad) + text + new string(' ', width - pad - text.Length);
        }

        // Hàm nhập password ẩn ký tự, giới hạn độ dài
        private static string ReadPassword(int maxLen)
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    System.Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar) && pass.Length < maxLen)
                {
                    pass += key.KeyChar;
                    System.Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);
            System.Console.WriteLine();
            return pass;
        }

        public static void ShowLoginForm()
        {
            int width = 80;
            int height = 15;
            int left = (Console.WindowWidth - width) / 2;
            int top = (Console.WindowHeight - height) / 2;

            string[] fields = { "Tên đăng nhập", "Mật khẩu" };
            string[] values = new string[fields.Length];
            int selectedField = 0;

            while (true)
            {
                Console.Clear();
                DrawBox(width, height, left, top);

                // Draw title
                Console.SetCursorPosition(left + (width - 13) / 2, top + 1);
                Console.Write("[ĐĂNG NHẬP]", Color.Yellow);

                // Draw fields
                DrawFormContent(fields, selectedField, values, width, left, top + 3);

                // Draw instructions
                Console.SetCursorPosition(left + 2, top + height - 2);
                Console.Write("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.", Color.Gray);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Tab)
                    selectedField = (selectedField + 1) % fields.Length;
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(left + 4, top + selectedField + 3);
                    Console.Write($"{fields[selectedField]}: ");
                    values[selectedField] = Console.ReadLine();
                }
                else if (key.Key == ConsoleKey.Escape)
                    return;

                if (!string.IsNullOrEmpty(values[0]) && !string.IsNullOrEmpty(values[1]))
                {
                    Console.SetCursorPosition(left + 2, top + height - 3);
                    Console.Write("[Giả lập] Đăng nhập thành công! Nhấn phím bất kỳ để tiếp tục...", Color.Green);
                    Console.ReadKey(true);
                    return;
                }
            }
        }

        public static void ShowRegisterForm()
        {
            int width = 80;
            int height = 20;
            int left = (Console.WindowWidth - width) / 2;
            int top = (Console.WindowHeight - height) / 2;

            string[] fields = { "Tên đăng nhập", "Email", "Mật khẩu", "Xác nhận mật khẩu", "Câu trả lời bảo mật" };
            string[] values = new string[fields.Length];
            int selectedField = 0;

            while (true)
            {
                Console.Clear();
                DrawBox(width, height, left, top);

                // Draw title
                Console.SetCursorPosition(left + (width - 10) / 2, top + 1);
                Console.Write("[ĐĂNG KÝ]", Color.Yellow);

                // Draw fields
                DrawFormContent(fields, selectedField, values, width, left, top + 3);

                // Draw instructions
                Console.SetCursorPosition(left + 2, top + height - 2);
                Console.Write("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.", Color.Gray);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Tab)
                    selectedField = (selectedField + 1) % fields.Length;
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (selectedField == 4)
                    {
                        // Hiển thị danh sách câu hỏi bảo mật
                        ShowSecurityQuestions(left + 4, top + 8, width - 8);
                    }
                    else
                    {
                        Console.SetCursorPosition(left + 4, top + selectedField + 3);
                        Console.Write($"{fields[selectedField]}: ");
                        values[selectedField] = Console.ReadLine();
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                    return;

                if (values.All(v => !string.IsNullOrEmpty(v)))
                {
                    Console.SetCursorPosition(left + 2, top + height - 3);
                    Console.Write("[Giả lập] Đăng ký thành công! Nhấn phím bất kỳ để tiếp tục...", Color.Green);
                    Console.ReadKey(true);
                    return;
                }
            }
        }

        public static void ShowForgotPasswordForm()
        {
            int width = 80;
            int height = 15;
            int left = (Console.WindowWidth - width) / 2;
            int top = (Console.WindowHeight - height) / 2;

            string[] fields = { "Tên đăng nhập", "Email", "Câu trả lời bảo mật" };
            string[] values = new string[fields.Length];
            int selectedField = 0;

            while (true)
            {
                Console.Clear();
                DrawBox(width, height, left, top);

                // Draw title
                Console.SetCursorPosition(left + (width - 15) / 2, top + 1);
                Console.Write("[QUÊN MẬT KHẨU]", Color.Yellow);

                // Draw fields
                DrawFormContent(fields, selectedField, values, width, left, top + 3);

                // Draw instructions
                Console.SetCursorPosition(left + 2, top + height - 2);
                Console.Write("Tab/Shift+Tab để chuyển, Enter để nhập, Esc để quay lại.", Color.Gray);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Tab)
                    selectedField = (selectedField + 1) % fields.Length;
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(left + 4, top + selectedField + 3);
                    Console.Write($"{fields[selectedField]}: ");
                    values[selectedField] = Console.ReadLine();
                }
                else if (key.Key == ConsoleKey.Escape)
                    return;

                if (values.All(v => !string.IsNullOrEmpty(v)))
                {
                    Console.SetCursorPosition(left + 2, top + height - 3);
                    Console.Write("[Giả lập] Yêu cầu đặt lại mật khẩu đã được gửi! Nhấn phím bất kỳ để tiếp tục...", Color.Green);
                    Console.ReadKey(true);
                    return;
                }
            }
        }

        private static void ShowSecurityQuestions(int left, int top, int width)
        {
            string[] questions = {
                "Tên trường tiểu học đầu tiên của bạn là gì?",
                "Họ và tên đệm của mẹ bạn là gì?",
                "Thú cưng đầu tiên của bạn tên gì?",
                "Bạn sinh ra ở thành phố nào?",
                "Môn thể thao yêu thích của bạn là gì?"
            };

            int selectedQuestion = 0;
            while (true)
            {
                Console.SetCursorPosition(left, top);
                Console.Write("Chọn câu hỏi bảo mật:", Color.Yellow);

                for (int i = 0; i < questions.Length; i++)
                {
                    Console.SetCursorPosition(left, top + i + 1);
                    if (i == selectedQuestion)
                    {
                        Console.Write($"▶ {i + 1}. {questions[i]}", Color.Yellow);
                    }
                    else
                    {
                        Console.Write($"  {i + 1}. {questions[i]}", Color.DarkGray);
                    }
                }

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                    selectedQuestion = (selectedQuestion - 1 + questions.Length) % questions.Length;
                else if (key.Key == ConsoleKey.DownArrow)
                    selectedQuestion = (selectedQuestion + 1) % questions.Length;
                else if (key.Key == ConsoleKey.Enter)
                    return;
                else if (key.Key == ConsoleKey.Escape)
                    return;
            }
        }
    }
} 