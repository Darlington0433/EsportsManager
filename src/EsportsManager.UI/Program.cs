using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EsportsManager.UI.ConsoleUI;
using EsportsManager.UI.Utilities;
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
      // Truyền serviceProvider vào ConsoleAppRunner
      ConsoleAppRunner.RunApplication(serviceProvider);
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
  {        // ═══════════════════════════════════════════════════════════════
           // CONFIGURATION SERVICES
           // ═══════════════════════════════════════════════════════════════

    // Load .env file if it exists
    try
    {
      dotenv.net.DotEnv.Load(options: new dotenv.net.DotEnvOptions(
          envFilePaths: new[] { Path.Combine(Directory.GetCurrentDirectory(), ".env") },
          ignoreExceptions: true)
      );
    }
    catch
    {
      // Continue if .env file doesn't exist or can't be loaded
      Console.WriteLine("No .env file found or could not be loaded. Using default configuration.");
    }

    // Get environment name
    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

    // Đăng ký Configuration từ appsettings.json và .env
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
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
    });    // ═══════════════════════════════════════════════════════════════
    // BUSINESS LOGIC LAYER SERVICES
    // ═══════════════════════════════════════════════════════════════    // Business Services - Scoped lifetime để tránh state conflicts
    services.AddScoped<IUserService, UserService>();
    // services.AddScoped<ITeamService, EsportsManager.BL.Services.TeamService>(); // TODO: Fix TeamService reference    // ServiceManager để tích hợp UI và BL
    services.AddScoped<EsportsManager.UI.Services.ServiceManager>();

    // Thêm SystemIntegrityService để kiểm tra toàn vẹn database khi khởi động
    services.AddScoped<EsportsManager.UI.Services.SystemIntegrityService>();

    // Business Utilities - Static classes, không cần register trong DI
    // PasswordHasher và InputValidator là static utilities

    // ═══════════════════════════════════════════════════════════════
    // DATA ACCESS LAYER SERVICES
    // ═══════════════════════════════════════════════════════════════    // Data Context - Scoped để maintain connection consistency
    services.AddScoped<DataContext>();    // Repository Pattern - Scoped để share với DataContext
    services.AddScoped<IUsersRepository, UsersRepository>();
    // services.AddScoped<EsportsManager.DAL.Interfaces.ITeamRepository, EsportsManager.DAL.Repositories.TeamRepository>(); // TODO: Fix TeamRepository reference

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

  // ═══════════════════════════════════════════════════════════════
  // CONSOLE WINDOW CONFIGURATION
  // ═══════════════════════════════════════════════════════════════
}

// =============================================================================
// END OF PROGRAM.CS
// 
// TÍCH HỢP VỚI KIẾN TRÚC:
// Program.cs → ConsoleAppRunner → InteractiveMenuService → Forms → Services → Repositories
// 
// LUỒNG DỮ LIỆU:
// User Input → UI Forms → Business Services → Repository → Database
// Database → Repository → Business Services → UI Forms → User Display
// 
// =============================================================================
