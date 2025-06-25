using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using dotenv.net;

namespace EsportsManager.DAL.Configuration
{
    /// <summary>
    /// Quản lý cấu hình ứng dụng
    /// </summary>
    public static class ConfigurationManager
    {
        private static IConfiguration _configuration;

        /// <summary>
        /// Khởi tạo cấu hình
        /// </summary>
        static ConfigurationManager()
        {
            // Load .env file if it exists
            try
            {
                DotEnv.Load(options: new DotEnvOptions(
                    envFilePaths: new[] { Path.Combine(AppContext.BaseDirectory, ".env") },
                    ignoreExceptions: true)
                );
            }
            catch
            {
                // Continue if .env file doesn't exist or can't be loaded
            }

            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        /// <summary>
        /// Lấy cấu hình
        /// </summary>
        public static IConfiguration Configuration => _configuration;        /// <summary>
                                                                             /// Lấy chuỗi kết nối MySQL từ cấu hình
                                                                             /// </summary>
        public static string GetConnectionString()
        {
            // Ưu tiên từ biến môi trường
            var envConnectionString = Environment.GetEnvironmentVariable("ESPORTS_DB_CONNECTION");
            if (!string.IsNullOrEmpty(envConnectionString))
                return envConnectionString;

            // Lấy từ cấu hình MySQL
            return _configuration.GetConnectionString("MySqlConnection") ??
                   _configuration.GetConnectionString("DefaultConnection") ??
                   DatabaseConfig.MySqlConnectionString;
        }

        /// <summary>
        /// Lấy timeout cho các command database (giây)
        /// </summary>
        public static int GetCommandTimeout()
        {
            return _configuration.GetValue<int>("Database:CommandTimeout", DatabaseConfig.CommandTimeout);
        }

        /// <summary>
        /// Lấy số lần retry khi kết nối database thất bại
        /// </summary>
        public static int GetRetryCount()
        {
            return _configuration.GetValue<int>("Database:RetryCount", DatabaseConfig.RetryCount);
        }

        /// <summary>
        /// Lấy delay giữa các lần retry (milliseconds)
        /// </summary>
        public static int GetRetryDelay()
        {
            return _configuration.GetValue<int>("Database:RetryDelay", DatabaseConfig.RetryDelay);
        }
    }
}
