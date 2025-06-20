using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EsportsManager.UI.ConsoleUI;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Repositories;
using EsportsManager.DAL.Context;

namespace EsportsManager.UI;

/// <summary>
/// Entry point ứng dụng với cấu hình Dependency Injection
/// </summary>
class Program
{    /// <summary>
    /// Khởi chạy ứng dụng console
    /// </summary>
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        using var serviceProvider = services.BuildServiceProvider();
        
        try
        {
            ConsoleAppRunner.RunApplication();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger?.LogError(ex, "Lỗi không mong muốn");
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Cấu hình DI container cho các layers
    /// </summary>
    /// <param name="services">ServiceCollection để đăng ký services</param>
    private static void ConfigureServices(ServiceCollection services)
    {
        // ═══════════════════════════════════════════════════════════════
        // CONFIGURATION SERVICES
        // ═══════════════════════════════════════════════════════════════
        
        // Đăng ký Configuration từ appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables() // Support environment variables override
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        
        // ═══════════════════════════════════════════════════════════════
        // LOGGING SERVICES  
        // ═══════════════════════════════════════════════════════════════
          // Configure logging providers
        services.AddLogging(builder =>
        {
            builder.AddConsole();      // Log ra console
            builder.SetMinimumLevel(LogLevel.Information); // Minimum log level
        });
        
        // ═══════════════════════════════════════════════════════════════
        // BUSINESS LOGIC LAYER SERVICES
        // ═══════════════════════════════════════════════════════════════
          // Business Services - Scoped lifetime để tránh state conflicts
        services.AddScoped<IUserService, UserService>();
        
        // Business Utilities - Static classes, không cần register trong DI
        // PasswordHasher và InputValidator là static utilities
        
        // ═══════════════════════════════════════════════════════════════
        // DATA ACCESS LAYER SERVICES
        // ═══════════════════════════════════════════════════════════════
        
        // Data Context - Scoped để maintain connection consistency
        services.AddScoped<DataContext>();
        
        // Repository Pattern - Scoped để share với DataContext
        services.AddScoped<IUserRepository, UserRepository>();
        
        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        
        // Có thể thêm các services khác ở đây:
        // - Email Service
        // - File Service  
        // - Validation Service
        // - Caching Service
        // - etc.
    }
}

// =============================================================================
// END OF PROGRAM.CS
// 
// TÍCH HỢP VỚI KIẾN TRÚC:
// Program.cs → LegacyUIRunner → InteractiveMenuService → Forms → Services → Repositories
// 
// LUỒNG DỮ LIỆU:
// User Input → UI Forms → Business Services → Repository → Database
// Database → Repository → Business Services → UI Forms → User Display
// 
// =============================================================================
