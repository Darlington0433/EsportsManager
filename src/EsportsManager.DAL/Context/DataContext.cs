// Context quản lý truy cập dữ liệu
// Cung cấp connection string và database connectivity

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EsportsManager.DAL.Configuration;

namespace EsportsManager.DAL.Context;

/// <summary>
/// DataContext - Lớp quản lý context dữ liệu (phiên bản đơn giản)
/// 
/// MỤC ĐÍCH:
/// - Cung cấp connection string cho database
/// - Quản lý lifecycle của database context
/// - Implement IDisposable pattern để release resources
/// 
/// DESIGN PATTERNS:
/// - Disposable Pattern: Implement IDisposable để cleanup resources
/// - Dependency Injection: Inject IConfiguration và ILogger
/// 
/// LƯU Ý: 
/// - Đây là phiên bản đơn giản để đảm bảo build compatibility
/// - Trong thực tế có thể sử dụng Entity Framework DbContext
/// </summary>
public class DataContext : IDisposable
{
    #region Private Fields - Các trường riêng tư

    /// <summary>
    /// Configuration để đọc connection string từ appsettings.json
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Logger để ghi log các hoạt động của context
    /// </summary>
    private readonly ILogger<DataContext> _logger;

    /// <summary>
    /// Flag để track việc dispose, tránh dispose nhiều lần
    /// </summary>
    private bool _disposed = false;

    #endregion

    #region Constructor - Hàm khởi tạo

    /// <summary>
    /// Khởi tạo DataContext với dependency injection
    /// </summary>
    /// <param name="configuration">Configuration để đọc settings</param>
    /// <param name="logger">Logger để ghi log</param>
    /// <exception cref="ArgumentNullException">Ném ra khi có dependency null</exception>
    public DataContext(IConfiguration configuration, ILogger<DataContext> logger)
    {
        // Validate dependencies không được null (Defensive Programming)
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Public Methods - Các phương thức công khai
    /// <summary>
    /// Lấy connection string để kết nối database
    /// 
    /// BUSINESS LOGIC:
    /// - Đọc connection string từ configuration
    /// - Nếu không có trong config, sử dụng default connection string từ DatabaseConfig
    /// - Support cả SQL Server và MySQL
    /// </summary>
    /// <returns>Connection string để kết nối database</returns>
    public string GetConnectionString()
    {
        // Đọc connection string từ appsettings.json
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        // Nếu không có thì dùng default từ DatabaseConfig
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = DatabaseConfig.GetConnectionString();
            _logger.LogWarning("Using default connection string from DatabaseConfig");
        }

        return connectionString;
    }

    /// <summary>
    /// Tạo database connection mới
    /// </summary>
    /// <returns>IDbConnection đã được configure</returns>
    public IDbConnection CreateConnection()
    {
        try
        {
            var connectionString = GetConnectionString();
            var connection = new SqlConnection(connectionString);

            _logger.LogDebug("Database connection created successfully");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database connection");
            throw new InvalidOperationException("Cannot create database connection", ex);
        }
    }    /// <summary>
         /// Kiểm tra kết nối database
         /// </summary>
         /// <returns>True nếu kết nối thành công</returns>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = CreateConnection();
            if (connection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();
            }
            else
            {
                connection.Open();
            }
            _logger.LogInformation("Database connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return false;
        }
    }

    #endregion

    #region IDisposable Implementation - Triển khai IDisposable

    /// <summary>
    /// Dispose resources để giải phóng bộ nhớ
    /// Implement Disposable Pattern để đảm bảo không memory leak
    /// </summary>
    public void Dispose()
    {
        // Chỉ dispose nếu chưa được dispose trước đó
        if (!_disposed)
        {
            // Log hoạt động dispose (có thể bỏ comment để debug)
            // _logger.LogDebug("DataContext disposed");

            // Đánh dấu đã dispose
            _disposed = true;

            // Suppress finalization vì đã cleanup thủ công
            GC.SuppressFinalize(this);
        }
    }

    #endregion
}
