using EsportsManager.DAL.Constants;

namespace EsportsManager.DAL.Configuration
{    /// <summary>
     /// Cấu hình mặc định kết nối cơ sở dữ liệu MySQL
     /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Connection string mặc định cho MySQL
        /// Format: Server={0};Database={1};Uid={2};Pwd={3};CharSet={4};Port={5}
        /// </summary>
        private const string CONNECTION_STRING_FORMAT = 
            "Server={0};Database={1};Uid={2};Pwd={3};CharSet={4};Port={5}";

        /// <summary>
        /// Các thông số mặc định cho kết nối
        /// </summary>
        private static class Defaults
        {
            public const string SERVER = DatabaseConstants.MySQL.DEFAULT_HOST;
            public const string DATABASE = DatabaseConstants.MySQL.DEFAULT_DATABASE;
            public const string USER = DatabaseConstants.MySQL.DEFAULT_USER;
            public const string PASSWORD = DatabaseConstants.MySQL.DEFAULT_PASSWORD;
            public const string CHARSET = DatabaseConstants.Connection.DEFAULT_CHARSET;
            public const string PORT = DatabaseConstants.MySQL.DEFAULT_PORT;
        }

        /// <summary>
        /// Lấy connection string mặc định
        /// </summary>
        public static string DefaultConnectionString => string.Format(
            CONNECTION_STRING_FORMAT,
            Defaults.SERVER,
            Defaults.DATABASE, 
            Defaults.USER,
            Defaults.PASSWORD,
            Defaults.CHARSET,
            Defaults.PORT
        );

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
        public static int CommandTimeout => DatabaseConstants.Connection.DEFAULT_COMMAND_TIMEOUT;

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
        public static int RetryCount => DatabaseConstants.Connection.DEFAULT_RETRY_COUNT;

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
        public static int RetryDelay => DatabaseConstants.Connection.DEFAULT_RETRY_DELAY;

        /// <summary>
        /// Lấy delay giữa các lần retry từ ConfigurationManager
        /// </summary>
        public static int GetRetryDelay()
        {
            return ConfigurationManager.GetRetryDelay();
        }
    }
}
