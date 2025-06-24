// Điều khiển ứng dụng console chính
using System;
using Microsoft.Extensions.DependencyInjection;
using EsportsManager.UI.Forms;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.MenuServices;
using EsportsManager.UI.Services;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.Models;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.ConsoleUI
{    /// <summary>
     /// Điều khiển giao diện console
     /// </summary>
    public static class ConsoleAppRunner
    {
        private static ServiceProvider? _serviceProvider;

        /// <summary>
        /// Chạy ứng dụng console với DI container
        /// </summary>
        public static void RunApplication(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Run(); // Delegate sang method Run() chính
        }        /// <summary>
                 /// Method chính điều khiển flow của ứng dụng
                 /// Hiển thị welcome screen, sau đó vào vòng lặp main menu
                 /// UPDATED: Sử dụng EnhancedMenuService cho giao diện đẹp hơn
                 /// </summary>
        public static void Run()
        {
            try
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
                // Kiểm tra toàn vẹn hệ thống trước khi bắt đầu ứng dụng
                var systemIntegrityService = _serviceProvider?.GetService<EsportsManager.UI.Services.SystemIntegrityService>();
                if (systemIntegrityService != null)
                {
                    bool databaseValid = systemIntegrityService.ValidateDatabaseSetupAsync().GetAwaiter().GetResult();
                    if (!databaseValid)
                    {
                        return; // Thoát ứng dụng nếu database không hợp lệ
                    }
                }
                else
                {
                    // Fallback nếu không tìm thấy service
                    bool databaseConnectionOk = TestDatabaseConnection().GetAwaiter().GetResult();
                    if (!databaseConnectionOk)
                    {
                        return; // Thoát ứng dụng nếu không kết nối được database
                    }
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
                    {
                        case 0: // User chọn "Đăng nhập"
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
            }
            catch
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
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy UserService từ DI container
                var userService = _serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();

                // Tạo và hiển thị form đăng nhập với UserService
                var loginForm = new UserAuthenticationForm(userService);
                bool isCompleted = loginForm.Show(); // Nhận kết quả từ form                if (isCompleted)
                {
                    // Đăng nhập thành công - kiểm tra và sử dụng thông tin user từ UserSessionManager
                    if (Services.UserSessionManager.IsLoggedIn && Services.UserSessionManager.CurrentUser != null)
                    {
                        ShowUserMenu(Services.UserSessionManager.CurrentUser.Role);
                    }
                }
            }
            catch
            {
                // Xử lý lỗi riêng cho process đăng nhập - im lặng, không hiển thị gì
                // Log lỗi nếu cần thiết nhưng không làm phiền user
            }
        }        /// <summary>
                 /// Hiển thị menu tương ứng với role của user đã đăng nhập
                 /// </summary>
        private static void ShowUserMenu(string role)
        {
            try
            {
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy ServiceManager từ DI container
                var serviceManager = _serviceProvider.GetRequiredService<ServiceManager>();

                // Lấy thông tin user đã xác thực từ UserSessionManager
                var currentUser = Services.UserSessionManager.CurrentUser;
                if (currentUser == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi: Thông tin người dùng không tồn tại", true, 3000);
                    return;
                }

                // Chuyển đến menu tương ứng với role thực của người dùng
                switch (role.ToLower())
                {
                    case "player":
                        ShowPlayerMenu(serviceManager, currentUser);
                        break;

                    case "admin":
                        ShowAdminMenu(serviceManager, currentUser);
                        break;

                    case "viewer":
                    default:
                        ShowViewerMenu(serviceManager, currentUser);
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị menu: {ex.Message}", true, 3000);
            }
        }        /// <summary>
                 /// Xử lý quy trình đăng ký tài khoản mới
                 /// Hiển thị form đăng ký và xử lý logic tạo account
                 /// </summary>
        private static void HandleRegister()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy UserService từ DI container
                var userService = _serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();

                // Tạo và hiển thị form đăng ký với UserService
                var registerForm = new UserRegistrationForm(userService);
                bool isCompleted = registerForm.Show(); // Nhận kết quả từ form                  if (isCompleted)
                {
                    // User hoàn thành form - đăng ký thành công, quay lại menu
                    return;
                }
            }
            catch
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
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy UserService từ DI container
                var userService = _serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();

                // Tạo và hiển thị form khôi phục mật khẩu với UserService
                var forgotPasswordForm = new PasswordRecoveryForm(userService); bool isCompleted = forgotPasswordForm.Show(); // Nhận kết quả từ form

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
        }        /// <summary>
                 /// Hiển thị menu Player với ServiceManager
                 /// </summary>
        private static void ShowPlayerMenu(ServiceManager serviceManager, UserProfileDto playerUser)
        {
            try
            {
                var playerMenuService = serviceManager.CreatePlayerMenuService(playerUser);
                playerMenuService.ShowPlayerMenu();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị menu Player: {ex.Message}", false, 3000);
            }
        }        /// <summary>
                 /// Hiển thị menu Admin với ServiceManager
                 /// </summary>
        private static void ShowAdminMenu(ServiceManager serviceManager, UserProfileDto adminUser)
        {
            try
            {
                var adminMenuService = serviceManager.CreateAdminMenuService(adminUser);
                adminMenuService.ShowAdminMenu();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị menu Admin: {ex.Message}", false, 3000);
            }
        }        /// <summary>
                 /// Hiển thị menu Viewer với ServiceManager
                 /// </summary>
        private static void ShowViewerMenu(ServiceManager serviceManager, UserProfileDto viewerUser)
        {
            try
            {
                var viewerMenuService = serviceManager.CreateViewerMenuService(viewerUser);
                viewerMenuService.ShowViewerMenu();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị menu Viewer: {ex.Message}", false, 3000);
            }
        }

        /// <summary>
        /// Phương thức kiểm tra kết nối database
        /// </summary>
        /// <returns>True nếu kết nối thành công</returns>
        private static async Task<bool> TestDatabaseConnection()
        {
            try
            {
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return false;
                }

                var dataContext = _serviceProvider.GetRequiredService<EsportsManager.DAL.Context.DataContext>();
                return await dataContext.TestConnectionAsync();
            }
            catch (Exception ex)
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("❌ LỖI KẾT NỐI CƠ SỞ DỮ LIỆU", 80, 16);
                Console.WriteLine("\nKhông thể kết nối đến MySQL Server. Vui lòng kiểm tra:");
                Console.WriteLine("  1. MySQL Server đã được khởi động chưa?");
                Console.WriteLine("  2. Connection string trong appsettings.json đã đúng chưa?");
                Console.WriteLine("  3. Database 'EsportsManager' đã được tạo chưa?");
                Console.WriteLine("  4. Username/password kết nối database đã chính xác chưa?");
                Console.WriteLine("\nChi tiết lỗi:");
                Console.WriteLine($"  {ex.Message}");
                Console.WriteLine("\n👉 HƯỚNG DẪN KHẮC PHỤC:");
                Console.WriteLine("  • Khởi động MySQL/MariaDB Server trên máy của bạn");
                Console.WriteLine("  • Import database từ file SQL trong thư mục database/");
                Console.WriteLine("  • Kiểm tra file appsettings.json và cập nhật connection string");
                Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
                Console.ReadKey(true);
                return false;
            }
        }
    }
}