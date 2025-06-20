// Context quản lý truy cập dữ liệu
// Cung cấp connection string và database connectivity

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
    /// - Nếu không có trong config, sử dụng default connection string
    /// - Default connection dùng SQL Server với Windows Authentication
    /// </summary>
    /// <returns>Connection string để kết nối database</returns>
    public string GetConnectionString()
    {
        // Đọc connection string từ appsettings.json
        // Nếu không có thì dùng default connection string
        return _configuration.GetConnectionString("DefaultConnection") ?? 
               "Data Source=localhost;Initial Catalog=EsportsManager;Integrated Security=true;";
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
