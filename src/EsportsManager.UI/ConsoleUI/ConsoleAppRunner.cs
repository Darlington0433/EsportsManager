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
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy UserService từ DI container
                var userService = _serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();
                
                // Tạo và hiển thị form đăng nhập với UserService
                var loginForm = new UserAuthenticationForm(userService);
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
        }        /// <summary>
        /// Hiển thị menu tương ứng với role của user
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
                
                // Tạo user profile mẫu
                var sampleUser = new UserProfileDto
                {
                    Id = 1,
                    Username = "demo_user",
                    Email = "demo@example.com",
                    FullName = "Demo User",
                    Role = role
                };
                
                switch (role.ToLower())
                {
                    case "player":
                        ShowPlayerMenu(serviceManager, sampleUser);
                        break;
                        
                    case "admin":
                        ShowAdminMenu(serviceManager, sampleUser);
                        break;
                        
                    case "viewer":
                    default:
                        ShowViewerMenu(serviceManager, sampleUser);
                        break;
                }
            }
            catch
            {
                // Xử lý lỗi menu - quay về menu chính
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
                if (_serviceProvider == null)
                {
                    ConsoleRenderingService.ShowMessageBox("Lỗi hệ thống: ServiceProvider chưa được khởi tạo", true, 3000);
                    return;
                }

                // Lấy UserService từ DI container
                var userService = _serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();
                
                // Tạo và hiển thị form khôi phục mật khẩu với UserService
                var forgotPasswordForm = new PasswordRecoveryForm(userService);                bool isCompleted = forgotPasswordForm.Show(); // Nhận kết quả từ form
                
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
                playerMenuService.ShowPlayerMenuAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị menu Player: {ex.Message}", false, 3000);
            }
        }/// <summary>
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
    }
}