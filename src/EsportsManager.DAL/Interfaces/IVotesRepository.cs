using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Repository interface cho bảng Votes
    /// </summary>
    public interface IVotesRepository
    {
        /// <summary>
        /// Thêm vote mới
        /// </summary>
        Task<Vote> AddVoteAsync(Vote vote);

        /// <summary>
        /// Lấy danh sách votes theo VoteType và TargetID
        /// </summary>
        Task<List<Vote>> GetVotesByTargetAsync(string voteType, int targetId);

        /// <summary>
        /// Lấy danh sách votes theo người vote
        /// </summary>
        Task<List<Vote>> GetVotesByVoterAsync(int voterId);

        /// <summary>
        /// Kiểm tra xem user đã vote cho đối tượng này chưa
        /// </summary>
        Task<bool> HasVotedAsync(int voterId, string voteType, int targetId);

        /// <summary>
        /// Tìm kiếm votes theo nhiều tiêu chí
        /// </summary>
        Task<List<Vote>> SearchVotesAsync(
            int? voterId = null,
            string username = null,
            string voteType = null,
            int? targetId = null,
            int? minRating = null,
            int? maxRating = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20);

        /// <summary>
        /// Lấy số lượng votes theo loại
        /// </summary>
        Task<int> GetVoteCountByTypeAsync(string voteType);

        /// <summary>
        /// Lấy số lượng người vote độc nhất
        /// </summary>
        Task<int> GetUniqueVotersCountAsync();

        /// <summary>
        /// Lấy thống kê votes theo tháng
        /// </summary>
        Task<Dictionary<string, int>> GetVotesByMonthAsync();
    }
}
