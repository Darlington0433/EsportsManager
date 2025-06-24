using System;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO donation
    /// </summary>
    public class DonationDto
    {
        [Required(ErrorMessage = "Số tiền không được để trống")]
        [Range(10000, 10000000, ErrorMessage = "Số tiền donate phải từ 10.000 đến 10.000.000 VND")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Thông điệp không được để trống")]
        [StringLength(200, ErrorMessage = "Thông điệp không được vượt quá 200 ký tự")]
        public required string Message { get; set; }

        public int? TournamentId { get; set; }
        public int? TeamId { get; set; }

        [Required(ErrorMessage = "Loại donate không được để trống")]
        public required string DonationType { get; set; } // 'Tournament', 'Team'
    }
}

