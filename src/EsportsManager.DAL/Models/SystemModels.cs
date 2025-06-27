using System;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Entity class cho bảng SystemSettings
    /// </summary>
    public class SystemSetting
    {
        /// <summary>
        /// Key của setting
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Value của setting
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả setting
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Entity class cho system logs
    /// </summary>
    public class SystemLog
    {
        public int LogID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
    }
}
