using EsportsManager.BL.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EsportsManager.BL.Interfaces
{
    /// <summary>
    /// Interface cho các dịch vụ liên quan đến voting
    /// </summary>
    public interface IVotingService
    {
        /// <summary>
        /// Lấy kết quả voting cho players
        /// </summary>
        Task<List<VotingResultDto>> GetPlayerVotingResultsAsync(int? limit = null);

        /// <summary>
        /// Lấy kết quả voting cho tournaments
        /// </summary>
        Task<List<VotingResultDto>> GetTournamentVotingResultsAsync(int? limit = null);

        /// <summary>
        /// Tìm kiếm votes theo các tiêu chí
        /// </summary>
        Task<List<VotingDto>> SearchVotesAsync(VotingSearchDto searchDto);

        /// <summary>
        /// Lấy thống kê tổng quát về voting
        /// </summary>
        Task<VotingStatsDto> GetVotingStatsAsync();

        /// <summary>
        /// Lấy chi tiết vote của một user
        /// </summary>
        Task<List<VotingDto>> GetUserVotesAsync(int userId);

        /// <summary>
        /// Gửi một vote mới
        /// </summary>
        Task<bool> SubmitVoteAsync(VotingDto votingDto);

        /// <summary>
        /// Lấy kết quả voting cho esports categories
        /// </summary>
        Task<List<VotingResultDto>> GetEsportsCategoryVotingResultsAsync(int? limit = null);
    }
}
