using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thông tin voting
    /// </summary>
    public class VotingDto
    {
        public int VotingId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string VoteType { get; set; } = string.Empty; // Player hoặc Tournament
        public int TargetId { get; set; } // PlayerID hoặc TournamentID
        public string TargetName { get; set; } = string.Empty;
        public int Rating { get; set; } // Thang điểm từ 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime VoteDate { get; set; }
    }

    /// <summary>
    /// DTO cho tổng hợp kết quả voting
    /// </summary>
    public class VotingResultDto
    {
        public int TargetId { get; set; }
        public string TargetName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Player hoặc Tournament
        public int TotalVotes { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>(); // Key: điểm, Value: số lượng vote
    }

    /// <summary>
    /// DTO cho thống kê tổng quát về voting
    /// </summary>
    public class VotingStatsDto
    {
        public int TotalVotes { get; set; }
        public int TotalPlayerVotes { get; set; }
        public int TotalTournamentVotes { get; set; }
        public int UniqueVoters { get; set; }
        public Dictionary<string, int> VotesByMonth { get; set; } = new Dictionary<string, int>();
        public List<VotingResultDto> TopPlayers { get; set; } = new List<VotingResultDto>();
        public List<VotingResultDto> TopTournaments { get; set; } = new List<VotingResultDto>();
    }

    /// <summary>
    /// DTO cho tìm kiếm voting
    /// </summary>
    public class VotingSearchDto
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? VoteType { get; set; }
        public int? TargetId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
