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
        private readonly IVotesRepository _votesRepository;
        private readonly ILogger<VotingService> _logger;

        public VotingService(IVotesRepository votesRepository, ILogger<VotingService> logger)
        {
            _votesRepository = votesRepository;
            _logger = logger;
        }

        /// <summary>
        /// Lấy kết quả voting cho players
        /// </summary>
        public async Task<List<VotingResultDto>> GetPlayerVotingResultsAsync(int? limit = null)
        {
            try
            {
                var votes = await _votesRepository.GetVotesByTargetAsync("Player", 0);
                var results = votes.GroupBy(v => v.TargetID)
                    .Select(g => new VotingResultDto
                    {
                        TargetId = g.Key,
                        Type = "Player",
                        TotalVotes = g.Count(),
                        AverageRating = Math.Round(g.Average(v => v.Rating), 2),
                        RatingDistribution = g.GroupBy(v => v.Rating)
                            .ToDictionary(r => r.Key, r => r.Count())
                    })
                    .OrderByDescending(r => r.AverageRating)
                    .ToList();

                if (limit.HasValue)
                {
                    results = results.Take(limit.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả voting cho players");
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
                var votes = await _votesRepository.GetVotesByTargetAsync("Tournament", 0);
                var results = votes.GroupBy(v => v.TargetID)
                    .Select(g => new VotingResultDto
                    {
                        TargetId = g.Key,
                        Type = "Tournament",
                        TotalVotes = g.Count(),
                        AverageRating = Math.Round(g.Average(v => v.Rating), 2),
                        RatingDistribution = g.GroupBy(v => v.Rating)
                            .ToDictionary(r => r.Key, r => r.Count())
                    })
                    .OrderByDescending(r => r.AverageRating)
                    .ToList();

                if (limit.HasValue)
                {
                    results = results.Take(limit.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả voting cho tournaments");
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
                var votes = await _votesRepository.SearchVotesAsync(
                    voterId: searchDto.UserId,
                    username: searchDto.Username ?? string.Empty,
                    voteType: searchDto.VoteType ?? string.Empty,
                    targetId: searchDto.TargetId,
                    minRating: searchDto.MinRating,
                    maxRating: searchDto.MaxRating,
                    fromDate: searchDto.FromDate,
                    toDate: searchDto.ToDate,
                    page: searchDto.Page,
                    pageSize: searchDto.PageSize);

                return votes.Select(v => new VotingDto
                {
                    VotingId = v.VoteID,
                    UserId = v.VoterID,
                    VoteType = v.VoteType,
                    TargetId = v.TargetID,
                    Rating = v.Rating,
                    Comment = v.Comment,
                    VoteDate = v.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm kiếm vote");
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
                var totalPlayerVotes = await _votesRepository.GetVoteCountByTypeAsync("Player");
                var totalTournamentVotes = await _votesRepository.GetVoteCountByTypeAsync("Tournament");
                var uniqueVoters = await _votesRepository.GetUniqueVotersCountAsync();
                var votesByMonth = await _votesRepository.GetVotesByMonthAsync();
                var topPlayers = await GetPlayerVotingResultsAsync(5);
                var topTournaments = await GetTournamentVotingResultsAsync(5);

                return new VotingStatsDto
                {
                    TotalVotes = totalPlayerVotes + totalTournamentVotes,
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
                _logger.LogError(ex, "Lỗi khi lấy thống kê voting");
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
                var votes = await _votesRepository.GetVotesByVoterAsync(userId);
                return votes.Select(v => new VotingDto
                {
                    VotingId = v.VoteID,
                    UserId = v.VoterID,
                    VoteType = v.VoteType,
                    TargetId = v.TargetID,
                    Rating = v.Rating,
                    Comment = v.Comment,
                    VoteDate = v.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy votes của user");
                throw;
            }
        }

        /// <summary>
        /// Gửi một vote mới
        /// </summary>
        public async Task<bool> SubmitVoteAsync(VotingDto votingDto)
        {
            try
            {
                var vote = new Vote
                {
                    VoterID = votingDto.UserId,
                    VoteType = votingDto.VoteType,
                    TargetID = votingDto.TargetId,
                    Rating = votingDto.Rating,
                    Comment = votingDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _votesRepository.AddVoteAsync(vote);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm vote mới");
                return false;
            }
        }

        /// <summary>
        /// Lấy kết quả voting cho esports categories
        /// </summary>
        public async Task<List<VotingResultDto>> GetEsportsCategoryVotingResultsAsync(int? limit = null)
        {
            try
            {
                var votes = await _votesRepository.GetVotesByTargetAsync("EsportsCategory", 0);
                var results = votes.GroupBy(v => v.TargetID)
                    .Select(g => new VotingResultDto
                    {
                        TargetId = g.Key,
                        Type = "EsportsCategory",
                        TotalVotes = g.Count(),
                        AverageRating = Math.Round(g.Average(v => v.Rating), 2),
                        RatingDistribution = g.GroupBy(v => v.Rating)
                            .ToDictionary(r => r.Key, r => r.Count())
                    })
                    .OrderByDescending(r => r.AverageRating)
                    .ToList();

                if (limit.HasValue)
                {
                    results = results.Take(limit.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy kết quả voting cho esports categories");
                throw;
            }
        }

        public async Task<bool> HasUserVotedAsync(int userId, string voteType, int targetId)
        {
            try
            {
                return await _votesRepository.HasVotedAsync(userId, voteType, targetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra vote của user");
                return false;
            }
        }
    }
}
