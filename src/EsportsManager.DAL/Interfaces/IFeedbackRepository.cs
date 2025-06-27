using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Repository interface cho bảng Feedback
    /// </summary>
    public interface IFeedbackRepository
    {
        /// <summary>
        /// Thêm feedback mới
        /// </summary>
        Task<Feedback> AddFeedbackAsync(Feedback feedback);

        /// <summary>
        /// Lấy danh sách feedback theo tournament
        /// </summary>
        Task<List<Feedback>> GetFeedbackByTournamentAsync(int tournamentId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Lấy danh sách feedback theo user
        /// </summary>
        Task<List<Feedback>> GetFeedbackByUserAsync(int userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Lấy tất cả feedback
        /// </summary>
        Task<List<Feedback>> GetAllFeedbackAsync(int page = 1, int pageSize = 20);

        /// <summary>
        /// Kiểm tra xem user đã gửi feedback cho tournament này chưa
        /// </summary>
        Task<bool> HasFeedbackAsync(int userId, int tournamentId);

        /// <summary>
        /// Cập nhật trạng thái feedback
        /// </summary>
        Task<bool> UpdateFeedbackStatusAsync(int feedbackId, string status);

        /// <summary>
        /// Đếm tổng số feedback
        /// </summary>
        Task<int> CountFeedbackAsync();

        /// <summary>
        /// Lấy rating trung bình của tournament
        /// </summary>
        Task<double> GetAverageRatingAsync(int tournamentId);

        /// <summary>
        /// Lấy phân bố rating của tournament
        /// </summary>
        Task<Dictionary<int, int>> GetRatingDistributionAsync(int tournamentId);
    }
}
