using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho cập nhật thông tin user
/// </summary>
public class UserUpdateDto
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
}

/// <summary>
/// DTO thông tin giải đấu
/// </summary>
public class TournamentInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Game { get; set; }
    public string? Format { get; set; }
    public string? Rules { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public decimal EntryFee { get; set; }
    public decimal? PrizePool { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public string Status { get; set; } = "Upcoming"; // Upcoming, InProgress, Completed, Cancelled
    public bool IsRegistrationOpen { get; set; }
    public List<string>? Sponsors { get; set; }
    public string? Location { get; set; }
    public bool IsOnline { get; set; }
}

/// <summary>
/// DTO tạo giải đấu mới
/// </summary>
public class TournamentCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? Game { get; set; }
    public string? Format { get; set; }
    public string? Rules { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    [Required]
    public decimal EntryFee { get; set; }

    public decimal? PrizePool { get; set; }

    [Required]
    public int MaxParticipants { get; set; }

    public List<string>? Sponsors { get; set; }
    public string? Location { get; set; }
    public bool IsOnline { get; set; }
}

/// <summary>
/// DTO cập nhật giải đấu
/// </summary>
public class TournamentUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Game { get; set; }
    public string? Format { get; set; }
    public string? Rules { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public decimal? EntryFee { get; set; }
    public decimal? PrizePool { get; set; }
    public int? MaxParticipants { get; set; }
    public List<string>? Sponsors { get; set; }
    public string? Location { get; set; }
    public bool? IsOnline { get; set; }
    public string? Status { get; set; }
    public bool? IsRegistrationOpen { get; set; }
}

/// <summary>
/// DTO cho feedback
/// </summary>
public class FeedbackDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int? TournamentId { get; set; }
    public string? TournamentName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public string Status { get; set; } = "New"; // New, Read, Responded, Closed
}
