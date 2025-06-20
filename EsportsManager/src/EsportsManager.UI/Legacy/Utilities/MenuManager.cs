using System;
using System.Linq;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EsportsManager.UI.Legacy.Utilities
{
    /// <summary>
    /// Quản lý hiển thị và tương tác với menu
    /// </summary>
    public static class MenuManager
    {
        public static readonly string TitleArt = @"
███████╗███████╗██████╗  ██████╗ ██████╗ ████████╗███████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ 
██╔════╝██╔════╝██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝██╔════╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╗ ██╔════╝██╔══██╗
█████╗  ███████╗██████╔╝██║   ██║██████╔╝   ██║   ███████╗    ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝
██╔══╝  ╚════██║██╔═══╝ ██║   ██║██╔══██╗   ██║   ╚════██║    ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗
███████╗███████║██║     ╚██████╔╝██║  ██║   ██║   ███████║    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║
╚══════╝╚══════╝╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝";

        /// <summary>
        /// Hiển thị menu và trả về lựa chọn của người dùng
        /// </summary>
        /// <param name="menuTitle">Tiêu đề menu</param>
        /// <param name="options">Các lựa chọn trong menu</param>
        /// <param name="selectedIndex">Vị trí được chọn mặc định</param>
        /// <returns>Vị trí người dùng chọn hoặc -1 nếu hủy</returns>
        public static int ShowMenu(string menuTitle, string[] options, int selectedIndex = 0)
        {
            Console.Clear();

            // Kích thước cửa sổ
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;            // Tính toán kích thước menu
            int contentWidth = Math.Min(140, windowWidth - 4); // Tăng lên 140 để chứa ASCII art đầy đủ

            // Tính số dòng cần cho ASCII art và menu options
            string[] titleArtLines = TitleArt.Split('\n');
            int artLineCount = titleArtLines.Length;
            int optionsCount = options.Length;
            int totalContentHeight = artLineCount + optionsCount + 8; // +8 cho tiêu đề, khoảng trắng và viền

            // Đảm bảo menu vừa với màn hình
            int contentHeight = Math.Min(totalContentHeight, windowHeight - 2);

            // Căn giữa trong console
            int leftPad = (windowWidth - contentWidth) / 2;
            int topPad = Math.Max(0, (windowHeight - contentHeight) / 2);
            if (leftPad < 0) leftPad = 0;
            if (topPad < 0) topPad = 0;

            int selected = selectedIndex;
            ConsoleKeyInfo key;
            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(leftPad, topPad);

                // Vẽ khung với tiêu đề
                Console.ForegroundColor = ConsoleColor.Green;

                // Top border với tiêu đề - xử lý đúng với Unicode
                // Tính toán độ dài thực của chuỗi Unicode
                int titleTextWidth = ConsoleDrawing.GetVisualWidth(menuTitle);
                int titleLength = titleTextWidth + 2; // +2 cho khoảng trắng hai bên

                // Đảm bảo tiêu đề căn giữa chính xác
                string leftBorder = new string('═', (contentWidth - titleLength) / 2);
                string rightBorder = new string('═', contentWidth - leftBorder.Length - titleLength);

                Console.SetCursorPosition(leftPad, topPad);
                Console.Write("╔");
                Console.Write(leftBorder);
                Console.Write(" " + menuTitle + " ");
                Console.Write(rightBorder);
                Console.WriteLine("╗");

                // ASCII art "ESPORTS MANAGER" - hiển thị màu xanh lam
                // Tách ASCII art thành các dòng mà không cắt
                string[] artLines = TitleArt.Split('\n');
                int maxArtLinesToShow = Math.Min(artLines.Length, contentHeight - options.Length - 4); // -4 cho viền và khoảng trống

                // Chỉ hiển thị số dòng art phù hợp với kích thước màn hình
                int currentLine = topPad + 1; // Dòng tiếp theo sau top border
                for (int i = 0; i < maxArtLinesToShow; i++)
                {
                    if (i >= artLines.Length) break;

                    string trimmed = artLines[i].TrimEnd('\r', '\n');
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        // Không cắt ASCII art, để nguyên để hiển thị đầy đủ
                        string artLine = trimmed;
                        string centered = ConsoleDrawing.CenterText(artLine, contentWidth);
                        Console.SetCursorPosition(leftPad, currentLine);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("║");
                        Console.ForegroundColor = ConsoleColor.Cyan;  // Đổi màu ASCII art thành xanh lam (Cyan)
                        Console.Write(centered);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("║");
                        Console.ResetColor();
                        currentLine++;
                    }
                }

                // Thêm 1 dòng trống giữa ASCII art và menu để trông đẹp hơn
                Console.SetCursorPosition(leftPad, currentLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("║" + new string(' ', contentWidth) + "║");
                currentLine++;

                // Menu options - căn giữa tất cả các lựa chọn
                int maxOptionLength = options.Max(o => o.Length);
                int centerStartPad = (contentWidth - maxOptionLength - 2) / 2; // -2 cho mũi tên và khoảng trắng
                centerStartPad = Math.Max(centerStartPad, 10); // Đảm bảo có đủ khoảng trắng

                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(leftPad, currentLine);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("║");

                    // Tính toán số khoảng trắng để căn giữa
                    int startPad = centerStartPad;
                    int endPad = contentWidth - startPad - options[i].Length - 2; // -2 cho mũi tên và khoảng trắng

                    Console.Write(new string(' ', startPad));

                    // Hiển thị mũi tên và nội dung tùy chọn
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("> ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  ");
                    }

                    Console.Write(options[i]);

                    // Hiển thị khoảng trắng sau tùy chọn và viền phải
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(new string(' ', endPad) + "║");
                    currentLine++;
                }

                // Empty line
                Console.SetCursorPosition(leftPad, currentLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("║" + new string(' ', contentWidth) + "║");
                currentLine++;

                // Bottom border
                Console.SetCursorPosition(leftPad, currentLine);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("╚" + new string('═', contentWidth) + "╝");
                Console.ResetColor();

                // Instructions
                try
                {
                    string instructions = "↑↓: Di chuyển | Enter: Chọn | Esc: Thoát";
                    int instructionTop = Math.Min(Console.CursorTop + 1, Console.WindowHeight - 1);
                    int instructionLeft = Math.Max(0, (windowWidth - instructions.Length) / 2);
                    Console.SetCursorPosition(instructionLeft, instructionTop);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(instructions);
                    Console.ResetColor();
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Bỏ qua nếu không thể hiển thị hướng dẫn do không đủ không gian
                }

                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selected = (selected > 0) ? selected - 1 : options.Length - 1;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selected = (selected < options.Length - 1) ? selected + 1 : 0;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return selected;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return -1;
                }
            }
        }

        // Hiển thị thông báo cho người dùng
        private static void ShowMessage(string message, bool isError = false)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine("\n" + message);
            Console.ResetColor();
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        /// <summary>
        /// Hiển thị màn hình chào mừng và menu đăng nhập
        /// </summary>
        public static void ShowWelcomeScreen()
        {
            // Khởi tạo DI và lấy UserService
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var provider = DIContainer.ConfigureServices(config);
            var userService = provider.GetService<IUserService>();

            while (true)
            {
                int selected = ShowMenu("[MENU CHÍNH]", new[] { "Đăng nhập", "Đăng ký", "Quên mật khẩu", "Thoát" });
                switch (selected)
                {
                    case 0:
                        var loginForm = new Forms.LoginForm();
                        var loginResult = loginForm.Show();
                        if (loginResult.HasValue)
                        {
                            // Xác thực đăng nhập
                            var loginDto = new LoginDto { Username = loginResult.Value.Username, Password = loginResult.Value.Password };
                            var authTask = userService.AuthenticateAsync(loginDto);
                            authTask.Wait();
                            var authResult = authTask.Result;
                            if (authResult != null && authResult.IsAuthenticated && !string.IsNullOrEmpty(authResult.Role))
                            {
                                // Điều hướng menu theo vai trò
                                switch (authResult.Role.ToLower())
                                {
                                    case "admin":
                                        Legacy.LegacyUIRunner.ShowAdminMenu();
                                        break;
                                    case "player":
                                        Legacy.LegacyUIRunner.ShowPlayerMenu();
                                        break;
                                    case "viewer":
                                        Legacy.LegacyUIRunner.ShowViewerMenu();
                                        break;
                                    default:
                                        ShowMessage("Không xác định vai trò người dùng!", true);
                                        break;
                                }
                            }
                            else
                            {
                                ShowMessage($"Đăng nhập thất bại! {authResult?.ErrorMessage ?? "Tài khoản hoặc mật khẩu không đúng."}", true);
                            }
                        }
                        break;
                    case 1:
                        var registerForm = new Forms.RegisterForm();
                        var registerResult = registerForm.Show();
                        if (registerResult.HasValue)
                        {
                            // Xử lý đăng ký
                            var reg = registerResult.Value;
                            var registerDto = new RegisterDto
                            {
                                Username = reg.Username,
                                Email = reg.Email,
                                Password = reg.Password,
                                ConfirmPassword = reg.Password // Đơn giản hóa, có thể sửa lại nếu form có xác nhận riêng
                            };
                            var regTask = userService.RegisterAsync(registerDto);
                            regTask.Wait();
                            var regResult = regTask.Result;
                            if (regResult != null && regResult.IsSuccess)
                            {
                                ShowMessage("Đăng ký thành công! Bạn có thể đăng nhập.", false);
                            }
                            else
                            {
                                ShowMessage($"Đăng ký thất bại! {regResult?.ErrorMessage ?? "Lỗi không xác định."}", true);
                            }
                        }
                        break;
                    case 2:
                        var forgotForm = new Forms.ForgotPasswordForm();
                        var forgotResult = forgotForm.Show();
                        if (forgotResult.HasValue)
                        {
                            var forgot = forgotResult.Value;
                            var resetDto = new ResetPasswordDto
                            {
                                Username = forgot.Username,
                                Email = forgot.Email
                            };
                            var resetTask = userService.ResetPasswordAsync(resetDto);
                            resetTask.Wait();
                            var resetResult = resetTask.Result;
                            if (resetResult != null && resetResult.IsSuccess)
                            {
                                ShowMessage("Yêu cầu đặt lại mật khẩu đã được gửi thành công!", false);
                            }
                            else
                            {
                                ShowMessage($"Đặt lại mật khẩu thất bại! {resetResult?.ErrorMessage ?? "Lỗi không xác định."}", true);
                            }
                        }
                        break;
                    case 3:
                    case -1:
                        Environment.Exit(0);
                        return;
                }
            }
        }
    }
}
