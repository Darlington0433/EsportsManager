using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EsportsManager.DAL.Context;

/// <summary>
/// Data Context - Simple version for build compatibility
/// </summary>
public class DataContext : IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataContext> _logger;
    private bool _disposed = false;

    public DataContext(IConfiguration configuration, ILogger<DataContext> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get connection string
    /// </summary>
    public string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection") ?? 
               "Data Source=localhost;Initial Catalog=EsportsManager;Integrated Security=true;";
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
