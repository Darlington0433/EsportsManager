namespace EsportsManager.DAL.Constants;

/// <summary>
/// Constants cho Database
/// </summary>
public static class DatabaseConstants
{
    /// <summary>
    /// Các giá trị mặc định cho kết nối database
    /// </summary>
    public static class Connection
    {
        public const int DEFAULT_COMMAND_TIMEOUT = 30;
        public const int DEFAULT_RETRY_COUNT = 3;
        public const int DEFAULT_RETRY_DELAY = 1000;
        public const string DEFAULT_CHARSET = "utf8mb4";
    }

    /// <summary>
    /// Các giá trị mặc định cho MySQL
    /// </summary>
    public static class MySQL
    {
        public const string DEFAULT_HOST = "localhost";
        public const string DEFAULT_DATABASE = "EsportsManager";
        public const string DEFAULT_USER = "root";
        public const string DEFAULT_PASSWORD = "";
        public const string DEFAULT_PORT = "3306";
    }

    /// <summary>
    /// Các biến môi trường
    /// </summary>
    public static class Environment
    {
        public const string CONNECTION_STRING_VAR = "ESPORTS_DB_CONNECTION";
        public const string MYSQL_HOST_VAR = "ESPORTS_MYSQL_HOST";
        public const string MYSQL_PORT_VAR = "ESPORTS_MYSQL_PORT";
        public const string MYSQL_DATABASE_VAR = "ESPORTS_MYSQL_DATABASE";
        public const string MYSQL_USER_VAR = "ESPORTS_MYSQL_USER";
        public const string MYSQL_PASSWORD_VAR = "ESPORTS_MYSQL_PASSWORD";
    }
} 