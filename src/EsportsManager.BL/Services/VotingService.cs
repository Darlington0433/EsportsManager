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
    /// Dịch vụ xử lý các chức năng liên quan đến voting
    /// </summary>
    public class VotingService : IVotingService
    {
        private readonly ILogger<VotingService> _logger;
        private readonly IVotesRepository _votesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ITeamRepository _teamRepository;

        public VotingService(
            ILogger<VotingService> logger,
            IVotesRepository votesRepository,
            IUsersRepository usersRepository,
            ITeamRepository teamRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _votesRepository = votesRepository ?? throw new ArgumentNullException(nameof(votesRepository));
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        /// <summary>
        /// Lấy kết quả voting cho players
        /// </summary>
        public async Task<List<VotingResultDto>> GetPlayerVotingResultsAsync(int? limit = null)
        {
            try
            {
                // Lấy tất cả vote cho Player
                var votes = await _votesRepository.GetVotesByTargetAsync("Player", 0);

                // Nhóm theo targetId và tính toán kết quả
                var groupedVotes = votes.GroupBy(v => v.TargetID);
                var results = new List<VotingResultDto>();

                foreach (var group in groupedVotes)
                {
                    int targetId = group.Key;
                    var user = await _usersRepository.GetByIdAsync(targetId);

                    if (user == null) continue;

                    var votesForTarget = group.ToList();
                    var averageRating = Math.Round(votesForTarget.Average(v => v.Rating), 2);

                    var ratingDistribution = new Dictionary<int, int>();
                    for (int i = 1; i <= 5; i++)
                    {
                        ratingDistribution[i] = votesForTarget.Count(v => v.Rating == i);
                    }

                    results.Add(new VotingResultDto
                    {
                        TargetId = targetId,
                        TargetName = user.Username,
                        Type = "Player",
                        TotalVotes = votesForTarget.Count,
                        AverageRating = averageRating,
                        RatingDistribution = ratingDistribution
                    });
                }

                // Sắp xếp kết quả theo điểm trung bình giảm dần
                results = results.OrderByDescending(r => r.AverageRating).ToList();

                // Giới hạn số lượng kết quả nếu có
                if (limit.HasValue && limit.Value > 0)
                {
                    results = results.Take(limit.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player voting results");
                throw;
            }
        }

        /// <summary>
        /// Lấy kết quả voting cho tournaments
        /// </summary>
        public async Task<List<VotingResultDto>> GetTournamentVotingResultsAsync(int? limit = null)
        {
            try
            {
                // Lấy tất cả vote cho Tournament
                var votes = await _votesRepository.GetVotesByTargetAsync("Tournament", 0);

                // Nhóm theo targetId và tính toán kết quả
                var groupedVotes = votes.GroupBy(v => v.TargetID);
                var results = new List<VotingResultDto>();

                foreach (var group in groupedVotes)
                {
                    int targetId = group.Key;
                    // Gọi đến repository để lấy tên của tournament
                    // Giả sử có một hàm để lấy tên tournament từ ID
                    string tournamentName = $"Tournament {targetId}"; // Thay bằng repository call thực

                    var votesForTarget = group.ToList();
                    var averageRating = Math.Round(votesForTarget.Average(v => v.Rating), 2);

                    var ratingDistribution = new Dictionary<int, int>();
                    for (int i = 1; i <= 5; i++)
                    {
                        ratingDistribution[i] = votesForTarget.Count(v => v.Rating == i);
                    }

                    results.Add(new VotingResultDto
                    {
                        TargetId = targetId,
                        TargetName = tournamentName,
                        Type = "Tournament",
                        TotalVotes = votesForTarget.Count,
                        AverageRating = averageRating,
                        RatingDistribution = ratingDistribution
                    });
                }

                // Sắp xếp kết quả theo điểm trung bình giảm dần
                results = results.OrderByDescending(r => r.AverageRating).ToList();

                // Giới hạn số lượng kết quả nếu có
                if (limit.HasValue && limit.Value > 0)
                {
                    results = results.Take(limit.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tournament voting results");
                throw;
            }
        }

        /// <summary>
        /// Tìm kiếm votes theo các tiêu chí
        /// </summary>
        public async Task<List<VotingDto>> SearchVotesAsync(VotingSearchDto searchDto)
        {
            try
            {
                // Sử dụng repository để tìm kiếm thực tế
                var votes = await _votesRepository.SearchVotesAsync(
                    voterId: searchDto.UserId,
                    username: searchDto.Username,
                    voteType: searchDto.VoteType,
                    targetId: searchDto.TargetId,
                    minRating: searchDto.MinRating,
                    maxRating: searchDto.MaxRating,
                    fromDate: searchDto.FromDate,
                    toDate: searchDto.ToDate,
                    page: searchDto.Page,
                    pageSize: searchDto.PageSize);

                // Chuyển đổi từ entity sang DTO
                var result = votes.Select(v => MapVoteToVotingDto(v)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching votes");
                throw;
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quát về voting
        /// </summary>
        public async Task<VotingStatsDto> GetVotingStatsAsync()
        {
            try
            {
                // Lấy số lượng vote theo loại
                int totalPlayerVotes = await _votesRepository.GetVoteCountByTypeAsync("Player");
                int totalTournamentVotes = await _votesRepository.GetVoteCountByTypeAsync("Tournament");
                int totalVotes = totalPlayerVotes + totalTournamentVotes;

                // Lấy số lượng người vote độc nhất
                int uniqueVoters = await _votesRepository.GetUniqueVotersCountAsync();

                // Lấy thống kê theo tháng
                var votesByMonth = await _votesRepository.GetVotesByMonthAsync();

                // Lấy top players và tournaments
                var topPlayers = await GetPlayerVotingResultsAsync(5);
                var topTournaments = await GetTournamentVotingResultsAsync(5);

                return new VotingStatsDto
                {
                    TotalVotes = totalVotes,
                    TotalPlayerVotes = totalPlayerVotes,
                    TotalTournamentVotes = totalTournamentVotes,
                    UniqueVoters = uniqueVoters,
                    VotesByMonth = votesByMonth,
                    TopPlayers = topPlayers,
                    TopTournaments = topTournaments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voting stats");
                throw;
            }
        }

        /// <summary>
        /// Lấy chi tiết vote của một user
        /// </summary>
        public async Task<List<VotingDto>> GetUserVotesAsync(int userId)
        {
            try
            {
                // Sử dụng repository để lấy votes từ database
                var votes = await _votesRepository.GetVotesByVoterAsync(userId);

                // Chuyển đổi từ entity sang DTO
                var result = votes.Select(v => MapVoteToVotingDto(v)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user votes");
                throw;
            }
        }

        /// <summary>
        /// Chuyển đổi từ Vote entity sang VotingDto
        /// </summary>
        private VotingDto MapVoteToVotingDto(Vote vote)
        {
            return new VotingDto
            {
                VotingId = vote.VoteID,
                UserId = vote.VoterID,
                Username = "", // Có thể cần gọi đến repository để lấy username
                VoteType = vote.VoteType,
                TargetId = vote.TargetID,
                TargetName = "", // Cần gọi đến repository để lấy tên target
                Rating = vote.Rating,
                Comment = vote.Comment,
                VoteDate = vote.CreatedAt
            };
        }

        /// <summary>
        /// Gửi một vote mới
        /// </summary>
        public async Task<bool> SubmitVoteAsync(VotingDto votingDto)
        {
            try
            {
                // Chuyển đổi từ DTO sang entity
                var vote = new Vote
                {
                    VoterID = votingDto.UserId,
                    VoteType = votingDto.VoteType,
                    TargetID = votingDto.TargetId,
                    Rating = votingDto.Rating,
                    Comment = votingDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                // Thêm vote mới vào database
                var addedVote = await _votesRepository.AddVoteAsync(vote);

                return addedVote != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting vote for {VoteType} with ID {TargetId}",
                    votingDto.VoteType, votingDto.TargetId);
                throw;
            }
        }

        /// <summary>
        /// Lấy kết quả voting cho esports categories
        /// </summary>
        public async Task<List<VotingResultDto>> GetEsportsCategoryVotingResultsAsync(int? limit = null)
        {
            try
            {
                // Đây là một chức năng mở rộng, chúng ta sẽ trả về danh sách rỗng
                // hoặc có thể implement khi có dữ liệu thực về các esports category
                return new List<VotingResultDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting esports category voting results");
                throw;
            }
        }
    }
}
