using System;

namespace EsportsManager.DAL.Configuration
{    /// <summary>
    /// Cấu hình mặc định kết nối cơ sở dữ liệu MySQL
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Connection string mặc định cho MySQL
        /// </summary>
        public static string DefaultConnectionString =>
            "Server=localhost;Database=EsportsManager;Uid=root;Pwd=;CharSet=utf8mb4;";

        /// <summary>
        /// Connection string cho MySQL (alias cho DefaultConnectionString)
        /// </summary>
        public static string MySqlConnectionString =>
            DefaultConnectionString;

        /// <summary>
        /// Lấy connection string từ ConfigurationManager
        /// </summary>
        public static string GetConnectionString()
        {
            return ConfigurationManager.GetConnectionString();
        }        /// <summary>
                 /// Timeout cho các command database (giây)
                 /// </summary>
        public static int CommandTimeout => 30;

        /// <summary>
        /// Lấy timeout từ ConfigurationManager
        /// </summary>
        public static int GetCommandTimeout()
        {
            return ConfigurationManager.GetCommandTimeout();
        }

        /// <summary>
        /// Số lần retry khi kết nối database thất bại
        /// </summary>
        public static int RetryCount => 3;

        /// <summary>
        /// Lấy số lần retry từ ConfigurationManager
        /// </summary>
        public static int GetRetryCount()
        {
            return ConfigurationManager.GetRetryCount();
        }

        /// <summary>
        /// Delay giữa các lần retry (milliseconds)
        /// </summary>
        public static int RetryDelay => 1000;

        /// <summary>
        /// Lấy delay giữa các lần retry từ ConfigurationManager
        /// </summary>
        public static int GetRetryDelay()
        {
            return ConfigurationManager.GetRetryDelay();
        }
    }
}
