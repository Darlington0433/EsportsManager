using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho tạo giải đấu mới
    /// </summary>
    public class TournamentCreateDto
    {
        /// <summary>
        /// Tên giải đấu
        /// </summary>
        public required string TournamentName { get; set; }

        /// <summary>
        /// Mô tả về giải đấu
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// ID của game
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Ngày bắt đầu giải đấu
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc giải đấu
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Hạn chót đăng ký
        /// </summary>
        public DateTime RegistrationDeadline { get; set; }

        /// <summary>
        /// Số đội tối đa được tham gia
        /// </summary>
        public int MaxTeams { get; set; }

        /// <summary>
        /// Phí đăng ký tham gia
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Tổng giá trị giải thưởng
        /// </summary>
        public decimal PrizePool { get; set; }

        /// <summary>
        /// Người tạo giải đấu
        /// </summary>
        public int CreatedBy { get; set; }
    }
}

