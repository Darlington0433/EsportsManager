using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.DAL.Context;
using EsportsManager.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace EsportsManager.UI.Configuration;

/// <summary>
/// Dependency Injection Container - áp dụng Dependency Inversion Principle
/// Cấu hình tất cả dependencies trong application
/// </summary>
public static class DIContainer
{
    /// <summary>
    /// Configure services for dependency injection
    /// </summary>
    public static IServiceProvider ConfigureServices(IConfiguration configuration)
    {        var services = new ServiceCollection();

        // Logging - simplified
        services.AddLogging();

        // Configuration
        services.AddSingleton(configuration);
          // Data Access Layer
        services.AddScoped<DataContext>();
        services.AddScoped<IUserRepository, EsportsManager.DAL.Repositories.UserRepository>();
        
        // Business Logic Layer
        services.AddScoped<IUserService, UserService>();

        // Build service provider
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Create simple configuration
    /// </summary>
    public static IConfiguration CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        return builder.Build();
    }
}
