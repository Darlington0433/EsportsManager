// Điều khiển ứng dụng console chính
using System;
using EsportsManager.UI.Forms;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers;
using EsportsManager.BL.Models;

namespace EsportsManager.UI.ConsoleUI
{    /// <summary>
    /// Điều khiển giao diện console
    /// </summary>
    public static class ConsoleAppRunner
    {        /// <summary>
        /// Chạy ứng dụng console
        /// </summary>
        public static void RunApplication()
        {
            Run(); // Delegate sang method Run() chính
        }
        
        /// <summary>
        /// Method chính điều khiển flow của ứng dụng
        /// Hiển thị welcome screen, sau đó vào vòng lặp main menu
        /// UPDATED: Sử dụng EnhancedMenuService cho giao diện đẹp hơn
        /// </summary>
        public static void Run()
        {            try
            {
                // Thiết lập màu nền cho toàn bộ ứng dụng
                try
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear(); // Xóa màn hình với background mới
                }
                catch
                {
                    // Bỏ qua lỗi nếu không thể thiết lập màu nền
                }

                // Vòng lặp chính của ứng dụng - chạy cho đến khi user thoát
                while (true)
                {
                    // Định nghĩa các lựa chọn trong menu chính
                    string[] mainMenuOptions = {
                        "Đăng nhập",        // Option 0: Đăng nhập vào hệ thống
                        "Đăng ký",          // Option 1: Đăng ký tài khoản mới
                        "Quên mật khẩu",    // Option 2: Khôi phục mật khẩu
                        "Giới thiệu"        // Option 3: Giới thiệu về ứng dụng
                    };
                    
                    // Hiển thị menu tương tác và nhận lựa chọn từ user
                    int selectedOption = InteractiveMenuService.DisplayInteractiveMenu("MENU CHÍNH", mainMenuOptions);
                    
                    // Xử lý lựa chọn của user bằng switch-case
                    switch (selectedOption)
                    {                        case 0: // User chọn "Đăng nhập"
                            HandleLogin();
                            break;
                            
                        case 1: // User chọn "Đăng ký"
                            HandleRegister();
                            break;
                            
                        case 2: // User chọn "Quên mật khẩu"
                            HandleForgotPassword();
                            break;
                            
                        case 3: // User chọn "Giới thiệu"
                            HandleAbout();
                            break;
                            
                        case -1: // User nhấn ESC (trả về -1)
                            // Thoát trực tiếp không cần xác nhận
                            Console.Clear();
                            Console.WriteLine("Cảm ơn bạn đã sử dụng Esports Manager!");
                            return; // Thoát khỏi method Run() = kết thúc ứng dụng
                    }
                }
            }            catch
            {
                // Bắt và xử lý các lỗi không mong muốn ở cấp cao nhất - im lặng thoát
            }
        }        /// <summary>
        /// Xử lý quy trình đăng nhập của user
        /// Hiển thị form đăng nhập và xử lý logic authentication
        /// </summary>
        private static void HandleLogin()
        {
            try
            {
                // Tạo và hiển thị form đăng nhập
                var loginForm = new UserAuthenticationForm();
                bool isCompleted = loginForm.Show(); // Nhận kết quả từ form
                
                if (isCompleted)
                {
                    // Đăng nhập thành công - cho user chọn role để demo
                    string selectedRole = UserRoleSelector.SelectUserRole();
                    ShowUserMenu(selectedRole);
                }
            }
            catch
            {
                // Xử lý lỗi riêng cho process đăng nhập - im lặng, không hiển thị gì
                // Log lỗi nếu cần thiết nhưng không làm phiền user
            }
        }
        
        /// <summary>
        /// Hiển thị menu tương ứng với role của user
        /// </summary>
        private static void ShowUserMenu(string role)
        {
            try
            {
                // Tạo user mẫu đơn giản
                var sampleUser = new
                {
                    Username = "demo_user",
                    Email = "demo@example.com",
                    FullName = "Demo User"
                };
                
                switch (role.ToLower())
                {
                    case "player":
                        ShowPlayerMenu(sampleUser.Username);
                        break;
                        
                    case "admin":
                        ShowAdminMenu(sampleUser.Username);
                        break;
                        
                    case "viewer":
                    default:
                        ShowViewerMenu(sampleUser.Username);
                        break;
                }
            }
            catch
            {
                // Xử lý lỗi menu - quay về menu chính
            }
        }/// <summary>
        /// Xử lý quy trình đăng ký tài khoản mới
        /// Hiển thị form đăng ký và xử lý logic tạo account
        /// </summary>
        private static void HandleRegister()
        {
            try
            {
                // Tạo và hiển thị form đăng ký
                var registerForm = new UserRegistrationForm();
                bool isCompleted = registerForm.Show(); // Nhận kết quả từ form                  if (isCompleted)
                {
                    // User hoàn thành form - đăng ký thành công, quay lại menu
                    return;
                }
            }            catch
            {
                // Xử lý lỗi riêng cho process đăng ký - im lặng, không hiển thị gì
            }
        }

        /// <summary>
        /// Xử lý quy trình khôi phục mật khẩu
        /// Hiển thị form khôi phục và xử lý logic reset password
        /// </summary>
        private static void HandleForgotPassword()
        {
            try
            {
                // Tạo và hiển thị form khôi phục mật khẩu
                var forgotPasswordForm = new PasswordRecoveryForm();                bool isCompleted = forgotPasswordForm.Show(); // Nhận kết quả từ form
                
                if (isCompleted)
                {
                    // User hoàn thành form - khôi phục mật khẩu thành công, quay lại menu
                    return;
                }
            }
            catch
            {
                // Xử lý lỗi riêng cho process khôi phục mật khẩu - im lặng, không hiển thị gì
            }
        }

        /// <summary>
        /// Xử lý hiển thị thông tin giới thiệu về ứng dụng
        /// </summary>
        private static void HandleAbout()
        {
            Console.Clear();
            Console.WriteLine("\n\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\t\t=== GIỚI THIỆU VỀ ESPORTS MANAGER ===\n");
            Console.ResetColor();
            Console.WriteLine("\tEsports Manager là phần mềm quản lý giải đấu Esports chuyên nghiệp.");
            Console.WriteLine("\tPhát triển bởi nhóm sinh viên VTC Academy.");
            Console.WriteLine("\tPhiên bản: 1.0.0");
            Console.WriteLine("\n\tTính năng chính:");
            Console.WriteLine("\t- Quản lý giải đấu");
            Console.WriteLine("\t- Quản lý đội tuyển");
            Console.WriteLine("\t- Quản lý thành viên");
            Console.WriteLine("\t- Thống kê và báo cáo");
            Console.WriteLine("\n\tNhấn phím bất kỳ để quay lại menu chính...");
            Console.ReadKey();
        }

        /// <summary>
        /// Hiển thị menu Player
        /// </summary>
        private static void ShowPlayerMenu(string username)
        {
            while (true)
            {
                var menuOptions = new[]
                {
                    "Đăng ký tham gia giải đấu",
                    "Quản lý team",
                    "Xem thông tin cá nhân",
                    "Cập nhật thông tin cá nhân", 
                    "Xem danh sách giải đấu",
                    "Gửi feedback giải đấu",
                    "Quản lý ví điện tử",
                    "Thành tích cá nhân",
                    "Đăng xuất"
                };

                int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU PLAYER - {username}", menuOptions);

                switch (selection)
                {
                    case 0:
                        ConsoleRenderingService.ShowMessageBox("Chức năng đăng ký giải đấu đang được phát triển", false, 2000);
                        break;
                    case 1:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý team đang được phát triển", false, 2000);
                        break;
                    case 2:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xem thông tin cá nhân đang được phát triển", false, 2000);
                        break;
                    case 3:
                        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
                        break;
                    case 4:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xem danh sách giải đấu đang được phát triển", false, 2000);
                        break;
                    case 5:
                        ConsoleRenderingService.ShowMessageBox("Chức năng gửi feedback đang được phát triển", false, 2000);
                        break;
                    case 6:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý ví điện tử đang được phát triển", false, 2000);
                        break;
                    case 7:
                        ConsoleRenderingService.ShowMessageBox("Chức năng thành tích cá nhân đang được phát triển", false, 2000);
                        break;
                    case 8:
                    case -1:
                        return; // Đăng xuất
                }
            }
        }

        /// <summary>
        /// Hiển thị menu Admin
        /// </summary>
        private static void ShowAdminMenu(string username)
        {
            while (true)
            {
                var menuOptions = new[]
                {
                    "Quản lý người dùng",
                    "Quản lý giải đấu/trận đấu",
                    "Xem thống kê hệ thống",
                    "Xem báo cáo donation",
                    "Xem kết quả voting",
                    "Quản lý feedback",
                    "Cài đặt hệ thống",
                    "Xóa người dùng",
                    "Đăng xuất"
                };

                int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU ADMIN - {username}", menuOptions);

                switch (selection)
                {
                    case 0:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý người dùng đang được phát triển", false, 2000);
                        break;
                    case 1:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý giải đấu đang được phát triển", false, 2000);
                        break;
                    case 2:
                        ConsoleRenderingService.ShowMessageBox("Chức năng thống kê hệ thống đang được phát triển", false, 2000);
                        break;
                    case 3:
                        ConsoleRenderingService.ShowMessageBox("Chức năng báo cáo donation đang được phát triển", false, 2000);
                        break;
                    case 4:
                        ConsoleRenderingService.ShowMessageBox("Chức năng kết quả voting đang được phát triển", false, 2000);
                        break;
                    case 5:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quản lý feedback đang được phát triển", false, 2000);
                        break;
                    case 6:
                        ConsoleRenderingService.ShowMessageBox("Chức năng cài đặt hệ thống đang được phát triển", false, 2000);
                        break;
                    case 7:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xóa người dùng đang được phát triển", false, 2000);
                        break;
                    case 8:
                    case -1:
                        return; // Đăng xuất
                }
            }
        }

        /// <summary>
        /// Hiển thị menu Viewer
        /// </summary>
        private static void ShowViewerMenu(string username)
        {
            while (true)
            {
                var menuOptions = new[]
                {
                    "Xem danh sách giải đấu",
                    "Xem bảng xếp hạng giải đấu",
                    "Vote cho Player/Tournament/Sport",
                    "Donate cho Player",
                    "Xem thông tin cá nhân",
                    "Cập nhật thông tin cá nhân",
                    "Quên mật khẩu",
                    "Đăng xuất"
                };

                int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU VIEWER - {username}", menuOptions);

                switch (selection)
                {
                    case 0:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xem danh sách giải đấu đang được phát triển", false, 2000);
                        break;
                    case 1:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xem bảng xếp hạng đang được phát triển", false, 2000);
                        break;
                    case 2:
                        ConsoleRenderingService.ShowMessageBox("Chức năng voting đang được phát triển", false, 2000);
                        break;
                    case 3:
                        ConsoleRenderingService.ShowMessageBox("Chức năng donate đang được phát triển", false, 2000);
                        break;
                    case 4:
                        ConsoleRenderingService.ShowMessageBox("Chức năng xem thông tin cá nhân đang được phát triển", false, 2000);
                        break;
                    case 5:
                        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
                        break;
                    case 6:
                        ConsoleRenderingService.ShowMessageBox("Chức năng quên mật khẩu đang được phát triển", false, 2000);
                        break;
                    case 7:
                    case -1:
                        return; // Đăng xuất
                }
            }
        }
    }
}