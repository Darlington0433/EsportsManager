using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EsportsManager.BL.Services;
using EsportsManager.BL.DTOs;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Repositories;
using EsportsManager.BL.Utilities;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== DEBUG ĐĂNG NHẬP ===");

        // Setup configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        var configuration = builder.Build();

        // Setup DI container
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        services.AddScoped<DataContext>();
        services.AddScoped<EsportsManager.DAL.Interfaces.IUsersRepository, UsersRepository>();
        services.AddScoped<EsportsManager.BL.Interfaces.IUserService, UserService>();

        var serviceProvider = services.BuildServiceProvider();

        // Test BCrypt trực tiếp
        Console.WriteLine("\n1. TEST BCRYPT TRỰC TIẾP:");
        TestBCryptDirect();

        // Test database connection
        Console.WriteLine("\n2. TEST KẾT NỐI DATABASE:");
        await TestDatabaseConnection(serviceProvider);

        // Test user repository
        Console.WriteLine("\n3. TEST USER REPOSITORY:");
        await TestUserRepository(serviceProvider);

        // Test user service
        Console.WriteLine("\n4. TEST USER SERVICE:");
        await TestUserService(serviceProvider);
    }    /// <summary>
    /// Kiểm tra BCrypt hash trực tiếp
    /// </summary>
    static void TestBCryptDirect()
    {
        string password = "admin123";
        string hash = "$2a$10$yGTZMMjfWyunReqDn.sZ1uMazm8Q.z7xYJYUkj50TBFKlJcX4X5F2";

        try
        {
            bool result = BCrypt.Net.BCrypt.Verify(password, hash);
            Console.WriteLine($"BCrypt.Verify('admin123', hash): {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi BCrypt: {ex.Message}");
        }
    }    /// <summary>
    /// Kiểm tra kết nối database thông qua DataContext
    /// </summary>
    static async Task TestDatabaseConnection(ServiceProvider serviceProvider)
    {
        try
        {
            var context = serviceProvider.GetRequiredService<DataContext>();
            bool connected = await context.TestConnectionAsync();
            Console.WriteLine($"Kết nối database: {(connected ? "✅ Thành công" : "❌ Thất bại")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi kết nối: {ex.Message}");
        }
    }    /// <summary>
    /// Kiểm tra User Repository (DAL layer)
    /// </summary>
    static async Task TestUserRepository(ServiceProvider serviceProvider)
    {
        try
        {
            var userRepo = serviceProvider.GetRequiredService<EsportsManager.DAL.Interfaces.IUsersRepository>();
            var user = await userRepo.GetByUsernameAsync("admin");

            if (user != null)
            {
                Console.WriteLine($"✅ Tìm thấy user: {user.Username}");
                Console.WriteLine($"   Trạng thái: {user.Status}");
                Console.WriteLine($"   Hash: {user.PasswordHash}");
                Console.WriteLine($"   Kích hoạt: {user.IsActive}");
                Console.WriteLine($"   Email xác thực: {user.IsEmailVerified}");
            }
            else
            {
                Console.WriteLine("❌ Không tìm thấy user admin");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi repository: {ex.Message}");
        }
    }    /// <summary>
    /// Kiểm tra User Service (BL layer)
    /// </summary>
    static async Task TestUserService(ServiceProvider serviceProvider)
    {
        try
        {
            var userService = serviceProvider.GetRequiredService<EsportsManager.BL.Interfaces.IUserService>();
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "admin123"
            };

            var result = await userService.AuthenticateAsync(loginDto);

            Console.WriteLine($"Kết quả đăng nhập: {(result.IsSuccess ? "✅ Thành công" : "❌ Thất bại")}");
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Lỗi: {result.ErrorMessage}");
            }
            else
            {
                Console.WriteLine($"ID người dùng: {result.UserId}");
                Console.WriteLine($"Tên đăng nhập: {result.Username}");
                Console.WriteLine($"Vai trò: {result.Role}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi service: {ex.Message}");
            Console.WriteLine($"Chi tiết lỗi: {ex.StackTrace}");
        }
    }
}
