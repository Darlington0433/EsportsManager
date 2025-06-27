using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho tổng quan về donation trong hệ thống
    /// </summary>
    public class DonationOverviewDto
    {
        public int TotalDonations { get; set; }
        public int TotalDonators { get; set; }
        public int TotalReceivers { get; set; }
        public decimal TotalDonationAmount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public Dictionary<string, decimal> DonationByType { get; set; } = new(); // Tournament, Team
    }

    /// <summary>
    /// DTO cho chi tiết người nhận/gửi donation
    /// </summary>
    public class TopDonationUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // Player, Team, Tournament
        public int DonationCount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime FirstDonation { get; set; }
        public DateTime LastDonation { get; set; }
    }

    /// <summary>
    /// DTO cho các báo cáo tìm kiếm donation
    /// </summary>
    public class DonationSearchFilterDto
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public int? TeamId { get; set; }
        public int? TournamentId { get; set; }
        public string? DonationType { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
