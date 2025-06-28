namespace EsportsManager.BL.Constants;

/// <summary>
/// Constants chung cho toàn bộ ứng dụng
/// </summary>
public static class AppConstants
{
    /// <summary>
    /// Các giá trị mặc định cho Database
    /// </summary>
    public static class Database
    {
        public const int DEFAULT_COMMAND_TIMEOUT = 30;
        public const int DEFAULT_RETRY_COUNT = 3;
        public const int DEFAULT_RETRY_DELAY = 1000;
        public const string DEFAULT_CHARSET = "utf8mb4";
    }

    /// <summary>
    /// Các giá trị mặc định cho UI
    /// </summary>
    public static class UI
    {
        public const int CONSOLE_WIDTH = 120;
        public const int CONSOLE_HEIGHT = 30;
        public const int CONSOLE_BUFFER = 3000;
        
        public const int DEFAULT_TIMEOUT = 1500;
        public const int ERROR_TIMEOUT = 2000;
        
        public const string PRESS_ANY_KEY = "Nhấn phím bất kỳ để tiếp tục...";
        public const string INVALID_OPTION = "Lựa chọn không hợp lệ!";
        public const string OPERATION_CANCELLED = "Thao tác đã bị hủy!";
    }

    /// <summary>
    /// Các giá trị mặc định cho validation
    /// </summary>
    public static class Validation 
    {
        public const int MIN_USERNAME_LENGTH = 3;
        public const int MAX_USERNAME_LENGTH = 50;
        public const int MIN_PASSWORD_LENGTH = 6;
        public const int MAX_PASSWORD_LENGTH = 50;
        public const int MAX_EMAIL_LENGTH = 100;
        
        public const string REQUIRED_FIELD = "Trường {0} không được để trống";
        public const string INVALID_LENGTH = "Trường {0} phải từ {1} đến {2} ký tự";
        public const string INVALID_FORMAT = "Trường {0} không đúng định dạng";
    }

    /// <summary>
    /// Các giá trị mặc định cho Currency
    /// </summary>
    public static class Currency
    {
        public const string CURRENCY_CODE = "VND";
        public const string CURRENCY_SYMBOL = "₫";
        public const string CURRENCY_FORMAT = "N0";
    }
} 