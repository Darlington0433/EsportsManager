using System;
using System.ComponentModel.DataAnnotations;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho tạo giải đấu mới
/// </summary>
public class TournamentCreateDto
{
    [Required(ErrorMessage = "Tên giải đấu không được để trống")]
    [StringLength(100, ErrorMessage = "Tên giải đấu không được vượt quá 100 ký tự")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Mô tả giải đấu không được để trống")]
    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "Game không được để trống")]
    [StringLength(50, ErrorMessage = "Tên game không được vượt quá 50 ký tự")]
    public required string Game { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    public DateTime EndDate { get; set; }

    [Range(2, 1000, ErrorMessage = "Số lượng tham gia phải từ 2 đến 1000")]
    public int MaxParticipants { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Phí tham gia phải >= 0")]
    public decimal EntryFee { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giải thưởng phải >= 0")]
    public decimal PrizePool { get; set; }

    public TournamentFormat Format { get; set; } = TournamentFormat.SingleElimination;

    public TournamentType Type { get; set; } = TournamentType.Team;

    [StringLength(200, ErrorMessage = "Quy định không được vượt quá 200 ký tự")]
    public string? Rules { get; set; }

    public bool IsPublic { get; set; } = true;
}

/// <summary>
/// DTO cho thông tin giải đấu
/// </summary>
public class TournamentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Game { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public TournamentFormat Format { get; set; }
    public TournamentType Type { get; set; }
    public TournamentStatus Status { get; set; }
    public string? Rules { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedByUsername { get; set; }
}

/// <summary>
/// DTO cho cập nhật giải đấu
/// </summary>
public class TournamentUpdateDto
{
    [StringLength(100, ErrorMessage = "Tên giải đấu không được vượt quá 100 ký tự")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Range(2, 1000, ErrorMessage = "Số lượng tham gia phải từ 2 đến 1000")]
    public int? MaxParticipants { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Phí tham gia phải >= 0")]
    public decimal? EntryFee { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giải thưởng phải >= 0")]
    public decimal? PrizePool { get; set; }

    [StringLength(200, ErrorMessage = "Quy định không được vượt quá 200 ký tự")]
    public string? Rules { get; set; }

    public bool? IsPublic { get; set; }
    public TournamentStatus? Status { get; set; }
}

/// <summary>
/// Enum định dạng giải đấu
/// </summary>
public enum TournamentFormat
{
    SingleElimination = 1,  // Loại trực tiếp
    DoubleElimination = 2,  // Loại kép
    RoundRobin = 3,         // Vòng tròn
    Swiss = 4,              // Swiss System
    GroupStage = 5          // Vòng bảng
}

/// <summary>
/// Enum loại giải đấu
/// </summary>
public enum TournamentType
{
    Individual = 1,         // Cá nhân
    Team = 2,              // Đội nhóm
    Mixed = 3              // Hỗn hợp
}

/// <summary>
/// Enum trạng thái giải đấu
/// </summary>
public enum TournamentStatus
{
    Draft = 0,             // Nháp
    Published = 1,         // Đã công bố
    RegistrationOpen = 2,  // Mở đăng ký
    RegistrationClosed = 3, // Đóng đăng ký
    InProgress = 4,        // Đang diễn ra
    Completed = 5,         // Hoàn thành
    Cancelled = 6,         // Đã hủy
    Postponed = 7          // Hoãn lại
}
