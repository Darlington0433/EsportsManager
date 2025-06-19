using System;
using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace EsportManager.UI
{
    public static class UIHelper
    {
        public static void ShowWelcomeScreen()
        {
            while (true)
            {
                System.Console.Clear();

                // FIGlet Fonts tiêu đề
                var figlet = FigletFont.Default;
                var figletText = new Figlet(figlet).ToAscii("ESPORTS MANAGER");
                int maxFigletWidth = 0;
                foreach (var line in figletText.ToString().Split('\n'))
                    if (line.Length > maxFigletWidth) maxFigletWidth = line.Length;

                // Form width tối thiểu
                string userLabel = "→ Username: ";
                string passLabel = "→ Password: ";
                int minFormWidth = Math.Max(userLabel.Length, passLabel.Length) + 12; // 12 ký tự cho input

                int contentWidth = Math.Max(Math.Max(60, minFormWidth), maxFigletWidth);
                int width = contentWidth + 2;
                string horizontal = new string('═', contentWidth);

                // Vẽ khung trên
                System.Console.WriteLine("╔" + horizontal + "╗");

                // Dòng trống trên
                System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

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

                // Dòng trống giữa tiêu đề và LOGIN
                System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

                // Nút LOGIN căn giữa
                string login = "[LOGIN]";
                int loginPad = (contentWidth - login.Length) / 2;
                System.Console.WriteLine("║" + new string(' ', loginPad) + login + new string(' ', contentWidth - loginPad - login.Length) + "║");

                // Dòng trống dưới
                for (int i = 0; i < 3; i++)
                    System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");

                // Vẽ khung dưới
                System.Console.WriteLine("╚" + horizontal + "╝");

                // Nhập username và password ở ngoài khung
                var userLabelStyle = new StyleSheet(Color.White);
                userLabelStyle.AddStyle(userLabel, Color.Cyan);
                Console.WriteStyled(userLabel, userLabelStyle);
                string username = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(username)) username = "seller001";

                var passLabelStyle = new StyleSheet(Color.White);
                passLabelStyle.AddStyle(passLabel, Color.Cyan);
                Console.WriteStyled(passLabel, passLabelStyle);
                string password = ReadPassword(12);
                if (string.IsNullOrWhiteSpace(password)) password = "********";

                // Chọn role bằng phím lên/xuống
                string[] roles = { "Admin", "Player", "Viewer" };
                int selected = 0;
                while (true)
                {
                    System.Console.Clear();
                    // Vẽ lại khung login như trên (bạn có thể refactor ra hàm riêng nếu muốn)
                    System.Console.WriteLine("╔" + horizontal + "╗");
                    System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");
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
                    System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");
                    System.Console.WriteLine("║" + new string(' ', loginPad) + login + new string(' ', contentWidth - loginPad - login.Length) + "║");
                    for (int i = 0; i < 3; i++)
                        System.Console.WriteLine("║" + new string(' ', contentWidth) + "║");
                    System.Console.WriteLine("╚" + horizontal + "╝");

                    // Hiển thị username, password đã nhập
                    System.Console.WriteLine();
                    Console.WriteStyled(userLabel, userLabelStyle);
                    System.Console.WriteLine(username);
                    Console.WriteStyled(passLabel, passLabelStyle);
                    System.Console.WriteLine(new string('*', password.Length));
                    System.Console.WriteLine();

                    // Hiển thị chọn role
                    System.Console.WriteLine("Chọn vai trò (↑/↓, Enter):");
                    for (int i = 0; i < roles.Length; i++)
                    {
                        if (i == selected)
                        {
                            Console.WriteLine($"> {roles[i]}", Color.LimeGreen);
                        }
                        else
                        {
                            Console.WriteLine($"  {roles[i]}", Color.Gray);
                        }
                    }
                    var key = System.Console.ReadKey(true);
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        selected = (selected - 1 + roles.Length) % roles.Length;
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        selected = (selected + 1) % roles.Length;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                // Đăng nhập thành công, chuyển sang menu theo role
                switch (roles[selected])
                {
                    case "Admin":
                        EsportManager.UI.Menus.AdminMenu.Show();
                        break;
                    case "Player":
                        EsportManager.UI.Menus.PlayerMenu.Show();
                        break;
                    case "Viewer":
                        EsportManager.UI.Menus.ViewerMenu.Show();
                        break;
                }
                // Khi menu trả về (chọn đăng xuất), sẽ quay lại đăng nhập
            }
        }

        public static void ShowMainMenu()
        {
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine("===== MENU CHÍNH =====");
                System.Console.WriteLine("1. Menu Admin");
                System.Console.WriteLine("2. Menu Player");
                System.Console.WriteLine("3. Menu Viewer");
                System.Console.WriteLine("4. Đăng xuất");
                System.Console.Write("Chọn chức năng: ");
                var key = System.Console.ReadKey();
                System.Console.WriteLine();
                switch (key.KeyChar)
                {
                    case '1':
                        EsportManager.UI.Menus.AdminMenu.Show();
                        System.Console.ReadKey();
                        break;
                    case '2':
                        EsportManager.UI.Menus.PlayerMenu.Show();
                        System.Console.ReadKey();
                        break;
                    case '3':
                        EsportManager.UI.Menus.ViewerMenu.Show();
                        System.Console.ReadKey();
                        break;
                    case '4':
                        // Đăng xuất, quay lại đăng nhập
                        return;
                    default:
                        System.Console.WriteLine("Lựa chọn không hợp lệ!");
                        System.Console.ReadKey();
                        break;
                }
            }
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
    }
} 