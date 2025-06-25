// Context quản lý truy cập dữ liệu MySQL
// Cung cấp connection string và database connectivity cho MySQL

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EsportsManager.DAL.Configuration;
using MySql.Data.MySqlClient;

namespace EsportsManager.DAL.Context;

/// <summary>
/// DataContext - Lớp quản lý context dữ liệu (phiên bản đơn giản)
/// 
/// MỤC ĐÍCH:
/// - Cung cấp connection string cho database
/// - Quản lý lifecycle của database context
/// - Implement IDisposable pattern để release resources
/// - Support cả SQL Server và MySQL
/// 
/// DESIGN PATTERNS:
/// - Disposable Pattern: Implement IDisposable để cleanup resources
/// - Dependency Injection: Inject IConfiguration và ILogger
/// - Factory Method: Tạo connection phù hợp với loại database
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
        // Đọc connection string từ ConfigurationManager
        var connectionString = DatabaseConfig.GetConnectionString();
        // _logger.LogDebug("Using connection string from configuration"); // Tắt debug log
        return connectionString;
    }    /// <summary>
         /// Xác định loại database đang sử dụng - chỉ hỗ trợ MySQL
         /// </summary>
         /// <returns>Luôn trả về "mysql"</returns>
    public string GetDatabaseType()
    {
        return "mysql"; // Chỉ hỗ trợ MySQL
    }/// <summary>
     /// Tạo database connection mới - chỉ hỗ trợ MySQL
     /// </summary>
     /// <returns>MySqlConnection đã được configure</returns>
    public IDbConnection CreateConnection()
    {
        try
        {
            var connectionString = GetConnectionString();            // _logger.LogDebug("Creating MySQL database connection..."); // Tắt debug log
            var connection = new MySqlConnection(connectionString);
            // _logger.LogDebug("MySQL database connection created successfully"); // Tắt debug log
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Không thể kết nối đến cơ sở dữ liệu MySQL. Vui lòng kiểm tra: 1) MySQL Server đã chạy chưa? 2) Connection string có đúng không? 3) Database 'EsportsManager' đã tồn tại chưa?");
            throw new InvalidOperationException("❌ Lỗi kết nối cơ sở dữ liệu MySQL. Vui lòng kiểm tra cấu hình và đảm bảo MySQL Server đang chạy.", ex);
        }
    }    /// <summary>
         /// Kiểm tra kết nối database MySQL
         /// </summary>
         /// <returns>True nếu kết nối thành công</returns>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = CreateConnection();
            if (connection is MySqlConnection mysqlConnection)
            {
                await mysqlConnection.OpenAsync();
            }
            else
            {
                connection.Open();
            }
            // _logger.LogInformation("✅ Kết nối cơ sở dữ liệu MySQL thành công"); // Tắt log để không hiển thị ra console
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Kiểm tra kết nối cơ sở dữ liệu MySQL thất bại");
            return false;
        }
    }

    /// <summary>
    /// Thực thi stored procedure với parameters và trả về DataTable
    /// </summary>
    /// <param name="procedureName">Tên stored procedure</param>
    /// <param name="parameters">Tham số cho stored procedure</param>
    /// <returns>DataTable chứa kết quả</returns>
    public DataTable ExecuteStoredProcedure(string procedureName, params IDbDataParameter[] parameters)
    {
        DataTable dataTable = new DataTable();

        try
        {
            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = DatabaseConfig.GetCommandTimeout(); if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            var adapter = GetDataAdapter(command);

            using (adapter as MySqlDataAdapter)
            {
                ((MySqlDataAdapter)adapter).Fill(dataTable);
            }

            _logger.LogDebug($"Successfully executed stored procedure: {procedureName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error executing stored procedure: {procedureName}");
            throw new InvalidOperationException($"Error executing stored procedure: {procedureName}", ex);
        }

        return dataTable;
    }

    /// <summary>
    /// Thực thi stored procedure với parameters và trả về số hàng bị ảnh hưởng
    /// </summary>
    /// <param name="procedureName">Tên stored procedure</param>
    /// <param name="parameters">Tham số cho stored procedure</param>
    /// <returns>Số hàng bị ảnh hưởng</returns>
    public int ExecuteNonQueryStoredProcedure(string procedureName, params IDbDataParameter[] parameters)
    {
        try
        {
            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = DatabaseConfig.GetCommandTimeout();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            int result = command.ExecuteNonQuery();
            _logger.LogDebug($"Successfully executed non-query procedure: {procedureName}, Affected rows: {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error executing non-query procedure: {procedureName}");
            throw new InvalidOperationException($"Error executing non-query procedure: {procedureName}", ex);
        }
    }    /// <summary>
         /// Tạo tham số cho stored procedure MySQL
         /// </summary>
         /// <param name="name">Tên tham số</param>
         /// <param name="value">Giá trị của tham số</param>
         /// <param name="direction">Hướng tham số (Input, Output, etc)</param>
         /// <returns>MySqlParameter cho MySQL</returns>
    public IDbDataParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
    {
        var param = new MySqlParameter(name, value ?? DBNull.Value)
        {
            Direction = direction
        };
        return param;
    }    /// <summary>
         /// Tạo DataAdapter cho MySQL
         /// </summary>
         /// <param name="command">MySqlCommand để tạo adapter</param>
         /// <returns>MySqlDataAdapter</returns>
    private IDbDataAdapter GetDataAdapter(IDbCommand command)
    {
        if (command is not MySqlCommand mysqlCommand)
        {
            throw new ArgumentException("Expected MySqlCommand for MySQL database");
        }
        return new MySqlDataAdapter(mysqlCommand);
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
