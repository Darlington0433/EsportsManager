using System;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Entity class cho bảng Feedback
    /// </summary>
    public class Feedback
    {
        /// <summary>
        /// ID của feedback
        /// </summary>
        public int FeedbackID { get; set; }

        /// <summary>
        /// ID của tournament
        /// </summary>
        public int TournamentID { get; set; }

        /// <summary>
        /// ID của người đưa feedback
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Nội dung feedback
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Điểm đánh giá từ 1-5
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Thời gian tạo feedback
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời gian cập nhật feedback
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Trạng thái feedback
        /// </summary>
        public string Status { get; set; } = "Active";
    }
}
