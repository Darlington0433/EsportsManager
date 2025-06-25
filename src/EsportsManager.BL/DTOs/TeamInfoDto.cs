using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho thông tin team
    /// </summary>
    public class TeamInfoDto
    {
        /// <summary>
        /// ID của team
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên của team
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tên game mà team tham gia
        /// </summary>
        public string GameName { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả về team
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Ngày thành lập team
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Số thành viên trong team
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// ID của captain/team leader
        /// </summary>
        public int? CaptainId { get; set; }

        /// <summary>
        /// Tên của captain
        /// </summary>
        public string CaptainName { get; set; } = string.Empty;

        /// <summary>
        /// ID của team leader
        /// </summary>
        public int LeaderId { get; set; }

        /// <summary>
        /// Tên của team leader
        /// </summary>
        public string LeaderName { get; set; } = string.Empty;

        /// <summary>
        /// Logo URL của team
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Xếp hạng của team
        /// </summary>
        public int Ranking { get; set; }

        /// <summary>
        /// Điểm số của team
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Trạng thái của team
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Thống kê thắng/thua
        /// </summary>
        public int Wins { get; set; }
        public int Losses { get; set; }

        /// <summary>
        /// Số thành viên tối đa
        /// </summary>
        public int MaxMembers { get; set; } = 10;

        /// <summary>
        /// Ngày tạo team
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thành tích của team
        /// </summary>
        public string? Achievements { get; set; }

        /// <summary>
        /// ID tournament hiện tại team đang tham gia
        /// </summary>
        public int? CurrentTournamentId { get; set; }
    }
}
