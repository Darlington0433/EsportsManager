using System;

namespace EsportsManager.UI.Constants;

/// <summary>
/// Constants cho UI
/// </summary>
public static class UIConstants
{
    /// <summary>
    /// Kích thước border mặc định
    /// </summary>
    public static class Border
    {
        public const int DEFAULT_WIDTH = 80;
        public const int DEFAULT_HEIGHT = 20;
        public const int LARGE_WIDTH = 90;
        public const int LARGE_HEIGHT = 30;
        public const int SMALL_WIDTH = 60;
        public const int SMALL_HEIGHT = 15;
    }

    /// <summary>
    /// Thời gian hiển thị thông báo (milliseconds)
    /// </summary>
    public static class MessageTimeout
    {
        public const int SHORT = 1000;
        public const int NORMAL = 2000;
        public const int LONG = 3000;
        public const int ERROR = 3000;
    }

    /// <summary>
    /// Biểu tượng cho các loại thông báo
    /// </summary>
    public static class Icons
    {
        public const string SUCCESS = "✅";
        public const string ERROR = "❌";
        public const string WARNING = "⚠️";
        public const string INFO = "ℹ️";
        public const string MONEY = "💰";
        public const string USER = "👤";
        public const string USERS = "👥";
        public const string SETTINGS = "⚙️";
        public const string CALENDAR = "📅";
        public const string CLIPBOARD = "📋";
        public const string TROPHY = "🏆";
        public const string MEDAL = "🏅";
        public const string CHART = "📊";
        public const string MAIL = "📧";
        public const string KEY = "🔑";
        public const string LOCK = "🔒";
        public const string UNLOCK = "🔓";
    }

    /// <summary>
    /// Màu sắc cho console
    /// </summary>
    public static class Colors
    {
        public static readonly ConsoleColor Primary = ConsoleColor.White;
        public static readonly ConsoleColor Secondary = ConsoleColor.Gray;
        public static readonly ConsoleColor Success = ConsoleColor.Green;
        public static readonly ConsoleColor Error = ConsoleColor.Red;
        public static readonly ConsoleColor Warning = ConsoleColor.Yellow;
        public static readonly ConsoleColor Info = ConsoleColor.Cyan;
        public static readonly ConsoleColor Highlight = ConsoleColor.DarkCyan;
        public static readonly ConsoleColor Background = ConsoleColor.Black;
    }

    /// <summary>
    /// Các thông báo chung
    /// </summary>
    public static class Messages
    {
        public const string LOADING = "Đang tải dữ liệu...";
        public const string NO_DATA = "Không có dữ liệu";
        public const string OPERATION_SUCCESS = "Thao tác thành công!";
        public const string OPERATION_FAILED = "Thao tác thất bại!";
        public const string OPERATION_CANCELLED = "Đã hủy thao tác";
        public const string CONFIRM_DELETE = "Bạn có chắc chắn muốn xóa? Thao tác này không thể hoàn tác!";
        public const string CONFIRM_UPDATE = "Bạn có chắc chắn muốn cập nhật?";
        public const string INVALID_INPUT = "Dữ liệu không hợp lệ!";
        public const string PRESS_ANY_KEY = "Nhấn phím bất kỳ để tiếp tục...";
        public const string UNAUTHORIZED = "Bạn không có quyền thực hiện thao tác này!";
    }

    /// <summary>
    /// Các ký tự đặc biệt
    /// </summary>
    public static class SpecialChars
    {
        public const string HORIZONTAL_LINE = "─";
        public const string VERTICAL_LINE = "│";
        public const string TOP_LEFT = "┌";
        public const string TOP_RIGHT = "┐";
        public const string BOTTOM_LEFT = "└";
        public const string BOTTOM_RIGHT = "┘";
        public const string BULLET = "•";
        public const string ARROW = "→";
    }
} 