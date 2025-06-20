using System;
using EsportsManager.UI.Legacy.Utilities;
using EsportsManager.UI.Legacy.Forms;
using EsportsManager.UI.Legacy.Menus;

namespace EsportsManager.UI.Legacy
{
    /// <summary>
    /// Lớp khởi tạo cho UI cũ (Legacy UI)
    /// </summary>
    public static class LegacyUIRunner
    {        /// <summary>
        /// Khởi động UI cũ
        /// </summary>
        public static void RunLegacyUI()
        {
            try
            {
                Console.Clear();

                // Khởi động MenuManager cũ
                MenuManager.ShowWelcomeScreen();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lỗi khi khởi động UI cũ: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Hiển thị menu cho Admin (Legacy)
        /// </summary>
        public static void ShowAdminMenu()
        {
            var adminMenu = new AdminMenu();
            adminMenu.Show();
        }

        /// <summary>
        /// Hiển thị menu cho Player (Legacy)
        /// </summary>
        public static void ShowPlayerMenu()
        {
            var playerMenu = new PlayerMenu();
            playerMenu.Show();
        }

        /// <summary>
        /// Hiển thị menu cho Viewer (Legacy)
        /// </summary>
        public static void ShowViewerMenu()
        {
            var viewerMenu = new ViewerMenu();
            viewerMenu.Show();
        }

        /// <summary>
        /// Test các form cũ
        /// </summary>
        public static void TestLegacyForms()
        {
            Console.Clear();
            Console.WriteLine("=== TEST CÁC FORM CŨ ===");
            
            // Test LoginForm
            Console.WriteLine("1. Test LoginForm");
            var loginForm = new LoginForm();
            var loginResult = loginForm.Show();
            if (loginResult.HasValue)
            {
                Console.WriteLine($"Đăng nhập: {loginResult.Value.Username} / {loginResult.Value.Password}");
            }

            // Test RegisterForm  
            Console.WriteLine("2. Test RegisterForm");
            var registerForm = new RegisterForm();
            var registerResult = registerForm.Show();
            if (registerResult.HasValue)
            {
                Console.WriteLine($"Đăng ký: {registerResult.Value.Username} / {registerResult.Value.Email}");
            }

            // Test ForgotPasswordForm
            Console.WriteLine("3. Test ForgotPasswordForm");
            var forgotForm = new ForgotPasswordForm();
            var forgotResult = forgotForm.Show();
            if (forgotResult.HasValue)
            {
                Console.WriteLine($"Quên mật khẩu: {forgotResult.Value.Username} / {forgotResult.Value.Email}");
            }

            Console.WriteLine("Test hoàn tất. Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
    }
}
