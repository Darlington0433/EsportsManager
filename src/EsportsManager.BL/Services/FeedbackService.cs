using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services
{
    /// <summary>
    /// Service xử lý feedback từ người dùng - sử dụng repository thực tế
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        private readonly ILogger<FeedbackService> _logger;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUsersRepository _usersRepository;

        public FeedbackService(
            ILogger<FeedbackService> logger, 
            IFeedbackRepository feedbackRepository,
            IUsersRepository usersRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        /// <summary>
        /// Lấy tất cả feedback
        /// </summary>
        public async Task<List<FeedbackDto>> GetAllFeedbackAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetAllFeedbackAsync(page, pageSize);
                return await MapFeedbacksToDtos(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all feedback");
                throw;
            }
        }

        /// <summary>
        /// Lấy feedback theo tournament
        /// </summary>
        public async Task<List<FeedbackDto>> GetFeedbackByTournamentAsync(int tournamentId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbackByTournamentAsync(tournamentId);
                return await MapFeedbacksToDtos(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback by tournament");
                throw;
            }
        }

        /// <summary>
        /// Tìm kiếm feedback
        /// </summary>
        public async Task<List<FeedbackDto>> SearchFeedbackAsync(string keyword, string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // Lấy tất cả feedback và lọc bằng LINQ (có thể tối ưu bằng cách thêm search method vào repository)
                var feedbacks = await _feedbackRepository.GetAllFeedbackAsync(1, 1000); // Giới hạn 1000 để an toàn
                
                var filteredFeedbacks = feedbacks.Where(f => 
                    (string.IsNullOrEmpty(keyword) || f.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase)) &&
                    (status == null || f.Status == status) &&
                    (!fromDate.HasValue || f.CreatedAt >= fromDate) &&
                    (!toDate.HasValue || f.CreatedAt <= toDate)
                ).ToList();
                
                return await MapFeedbacksToDtos(filteredFeedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching feedback");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái hiển thị của feedback
        /// </summary>
        public async Task<bool> ToggleFeedbackVisibilityAsync(int feedbackId, string newStatus)
        {
            try
            {
                return await _feedbackRepository.UpdateFeedbackStatusAsync(feedbackId, newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling feedback visibility for ID: {FeedbackId}", feedbackId);
                throw;
            }
        }

        /// <summary>
        /// Xóa feedback
        /// </summary>
        public async Task<bool> DeleteFeedbackAsync(int feedbackId)
        {
            try
            {
                // Soft delete bằng cách cập nhật status thành "Removed"
                return await _feedbackRepository.UpdateFeedbackStatusAsync(feedbackId, "Removed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback ID: {FeedbackId}", feedbackId);
                throw;
            }
        }

        /// <summary>
        /// Lấy thống kê về feedback
        /// </summary>
        public async Task<FeedbackStatsDto> GetFeedbackStatsAsync()
        {
            try
            {
                // Lấy tất cả feedbacks để tính toán thống kê
                var feedbacks = await _feedbackRepository.GetAllFeedbackAsync(1, 10000); // Lấy tối đa 10000
                
                // Tính toán các thống kê
                var visibleCount = feedbacks.Count(f => f.Status == "Active");
                var hiddenCount = feedbacks.Count(f => f.Status == "Hidden");
                var averageRating = feedbacks.Any() ? Math.Round(feedbacks.Average(f => f.Rating), 2) : 0;
                
                // Phân bố rating
                var ratingDistribution = new Dictionary<int, int>();
                for (int i = 1; i <= 5; i++)
                {
                    ratingDistribution[i] = feedbacks.Count(f => f.Rating == i);
                }
                
                // Phân bố theo tháng
                var feedbackByMonth = feedbacks
                    .GroupBy(f => new { Year = f.CreatedAt.Year, Month = f.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:D2}",
                        g => g.Count()
                    );
                
                // Top tournaments theo số lượng feedback
                var topTournamentIds = feedbacks
                    .GroupBy(f => f.TournamentID)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList();
                
                var topTournaments = new List<TournamentFeedbackSummary>();
                foreach (var tournamentId in topTournamentIds)
                {
                    var tournamentFeedbacks = feedbacks.Where(f => f.TournamentID == tournamentId).ToList();
                    
                    topTournaments.Add(new TournamentFeedbackSummary
                    {
                        TournamentId = tournamentId,
                        TournamentName = $"Tournament {tournamentId}", // Thực tế cần lấy từ TournamentService
                        FeedbackCount = tournamentFeedbacks.Count,
                        AverageRating = Math.Round(tournamentFeedbacks.Average(f => f.Rating), 2)
                    });
                }
                
                return new FeedbackStatsDto
                {
                    TotalFeedback = feedbacks.Count,
                    VisibleFeedback = visibleCount,
                    HiddenFeedback = hiddenCount,
                    AverageRating = averageRating,
                    RatingDistribution = ratingDistribution,
                    FeedbackByMonth = feedbackByMonth,
                    TopTournaments = topTournaments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback stats");
                throw;
            }
        }

        /// <summary>
        /// Chuyển đổi từ Entity sang DTO
        /// </summary>
        private async Task<List<FeedbackDto>> MapFeedbacksToDtos(List<Feedback> feedbacks)
        {
            var dtos = new List<FeedbackDto>();
            
            foreach (var feedback in feedbacks)
            {
                // Lấy thông tin user từ repository
                var user = await _usersRepository.GetByIdAsync(feedback.UserID);
                
                dtos.Add(new FeedbackDto
                {
                    FeedbackId = feedback.FeedbackID,
                    TournamentId = feedback.TournamentID,
                    UserId = feedback.UserID,
                    UserName = user?.Username ?? "Unknown User",
                    Content = feedback.Content,
                    Rating = feedback.Rating,
                    CreatedAt = feedback.CreatedAt,
                    Status = feedback.Status
                });
            }
            
            return dtos;
        }
    }
}
