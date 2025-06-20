using System;
using EsportsManager.UI.Legacy.Forms;
using EsportsManager.UI.Legacy.Utilities;

namespace EsportsManager.UI.Legacy
{
    /// <summary>
    /// Lớp hỗ trợ UI cũ - đã được tích hợp với kiến trúc mới
    /// </summary>
    public static class LegacyUIHelper
    {
        /// <summary>
        /// ASCII Art tiêu đề - chuyển hướng đến MenuManager
        /// </summary>
        public static string TitleArt => MenuManager.TitleArt;

        /// <summary>
        /// Hiển thị màn hình chào mừng - chuyển hướng đến MenuManager
        /// </summary>
        public static void ShowWelcomeScreen()
        {
            MenuManager.ShowWelcomeScreen();
        }

        /// <summary>
        /// Hiển thị menu - chuyển hướng đến MenuManager
        /// </summary>
        public static int ShowMenu(string menuTitle, string[] options, int selectedIndex = 0)
        {
            return MenuManager.ShowMenu(menuTitle, options, selectedIndex);
        }

        /// <summary>
        /// Hiển thị form đăng nhập - chuyển hướng đến LoginForm
        /// </summary>
        public static void ShowLoginForm()
        {
            var form = new LoginForm();
            form.Show();
        }

        /// <summary>
        /// Hiển thị form đăng ký - chuyển hướng đến RegisterForm
        /// </summary>
        public static void ShowRegisterForm()
        {
            var form = new RegisterForm();
            form.Show();
        }

        /// <summary>
        /// Hiển thị form quên mật khẩu - chuyển hướng đến ForgotPasswordForm
        /// </summary>
        public static void ShowForgotPasswordForm()
        {
            var form = new ForgotPasswordForm();
            form.Show();
        }
        
        /// <summary>
        /// Vẽ ASCII art tiêu đề - chuyển hướng đến ConsoleDrawing
        /// </summary>
        public static void DrawTitleArt(string[] artLines, int contentWidth)
        {
            ConsoleDrawing.DrawTitleArt(artLines, contentWidth);
        }
        
        /// <summary>
        /// Căn giữa văn bản - chuyển hướng đến ConsoleDrawing
        /// </summary>
        public static string CenterText(string text, int width)
        {
            return ConsoleDrawing.CenterText(text, width);
        }
    }
}
