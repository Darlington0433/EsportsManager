using EsportsManager.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Interface cho các dịch vụ liên quan đến feedback
    /// </summary>
    public interface IFeedbackService
    {
        /// <summary>
        /// Lấy tất cả feedback
        /// </summary>
        Task<List<FeedbackDto>> GetAllFeedbackAsync(int page = 1, int pageSize = 20);

        /// <summary>
        /// Lấy feedback theo tournament
        /// </summary>
        Task<List<FeedbackDto>> GetFeedbackByTournamentAsync(int tournamentId);

        /// <summary>
        /// Tìm kiếm feedback
        /// </summary>
        Task<List<FeedbackDto>> SearchFeedbackAsync(string keyword, string? status = null, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Cập nhật trạng thái hiển thị của feedback
        /// </summary>
        Task<bool> ToggleFeedbackVisibilityAsync(int feedbackId, string newStatus);

        /// <summary>
        /// Xóa feedback
        /// </summary>
        Task<bool> DeleteFeedbackAsync(int feedbackId);

        /// <summary>
        /// Lấy thống kê về feedback
        /// </summary>
        Task<FeedbackStatsDto> GetFeedbackStatsAsync();
    }

    /// <summary>
    /// DTO cho thống kê feedback
    /// </summary>
    public class FeedbackStatsDto
    {
        public int TotalFeedback { get; set; }
        public int VisibleFeedback { get; set; }
        public int HiddenFeedback { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> FeedbackByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
        public List<TournamentFeedbackSummary> TopTournaments { get; set; } = new List<TournamentFeedbackSummary>();
    }

    /// <summary>
    /// Tóm tắt feedback cho tournament
    /// </summary>
    public class TournamentFeedbackSummary
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public int FeedbackCount { get; set; }
        public double AverageRating { get; set; }
    }
}
