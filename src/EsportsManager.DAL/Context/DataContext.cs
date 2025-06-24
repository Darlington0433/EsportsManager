// Context quản lý truy cập dữ liệu
// Cung cấp connection string và database connectivity

using System;
using System.Data;
using System.Data.SqlClient;
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
        _logger.LogDebug("Using connection string from configuration");
        return connectionString;
    }

    /// <summary>
    /// Xác định loại database đang sử dụng
    /// </summary>
    /// <returns>Loại database (mysql hoặc sqlserver)</returns>
    public string GetDatabaseType()
    {
        var dbType = Environment.GetEnvironmentVariable("ESPORTS_DB_TYPE")?.ToLower() ??
                     _configuration.GetValue<string>("Database:Type")?.ToLower() ??
                     "mysql"; // Default to MySQL for our EsportsManager application

        return dbType;
    }

    /// <summary>
    /// Tạo database connection mới dựa trên loại database đã cấu hình
    /// </summary>
    /// <returns>IDbConnection đã được configure</returns>
    public IDbConnection CreateConnection()
    {
        try
        {
            var connectionString = GetConnectionString();
            var dbType = GetDatabaseType();

            IDbConnection connection;

            if (dbType == "mysql")
            {
                connection = new MySqlConnection(connectionString);
                _logger.LogDebug("MySQL database connection created");
            }
            else
            {
                connection = new SqlConnection(connectionString);
                _logger.LogDebug("SQL Server database connection created");
            }

            _logger.LogDebug("Database connection created successfully");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database connection");
            throw new InvalidOperationException("Cannot create database connection", ex);
        }
    }

    /// <summary>
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

            if (adapter is MySqlDataAdapter mysqlAdapter)
            {
                using (mysqlAdapter)
                {
                    mysqlAdapter.Fill(dataTable);
                }
            }
            else if (adapter is SqlDataAdapter sqlAdapter)
            {
                using (sqlAdapter)
                {
                    sqlAdapter.Fill(dataTable);
                }
            }
            else
            {
                throw new InvalidOperationException("Unsupported data adapter type");
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
    }

    /// <summary>
    /// Tạo tham số cho stored procedure tùy theo loại database
    /// </summary>
    /// <param name="name">Tên tham số</param>
    /// <param name="value">Giá trị của tham số</param>
    /// <param name="direction">Hướng tham số (Input, Output, etc)</param>
    /// <returns>IDbDataParameter phù hợp với loại database</returns>
    public IDbDataParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
    {
        var dbType = GetDatabaseType();

        if (dbType == "mysql")
        {
            var param = new MySqlParameter(name, value ?? DBNull.Value)
            {
                Direction = direction
            };
            return param;
        }
        else
        {
            var param = new SqlParameter(name, value ?? DBNull.Value)
            {
                Direction = direction
            };
            return param;
        }
    }    /// <summary>
         /// Tạo DataAdapter phù hợp với loại database
         /// </summary>
         /// <param name="command">Command để tạo adapter</param>
         /// <returns>IDataAdapter phù hợp</returns>
    private IDbDataAdapter GetDataAdapter(IDbCommand command)
    {
        var dbType = GetDatabaseType();

        if (dbType == "mysql")
        {
            if (command is not MySqlCommand mysqlCommand)
            {
                throw new ArgumentException("Expected MySqlCommand for MySQL database");
            }
            return new MySqlDataAdapter(mysqlCommand);
        }
        else
        {
            if (command is not SqlCommand sqlCommand)
            {
                throw new ArgumentException("Expected SqlCommand for SQL Server database");
            }
            return new SqlDataAdapter(sqlCommand);
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
