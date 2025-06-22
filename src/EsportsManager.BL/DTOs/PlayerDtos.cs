using System;
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
}

/// <summary>
/// DTO thông tin giải đấu
/// </summary>
public class TournamentInfoDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EntryFee { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public bool IsRegistrationOpen { get; set; }
}

/// <summary>
/// DTO cho tạo team
/// </summary>
public class TeamCreateDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? LogoUrl { get; set; }
}

/// <summary>
/// DTO thông tin team
/// </summary>
public class TeamInfoDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
}

/// <summary>
/// DTO thành viên team
/// </summary>
public class TeamMemberDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// DTO cho feedback
/// </summary>
public class FeedbackDto
{
    public int TournamentId { get; set; }
    public required string Subject { get; set; }
    public required string Content { get; set; }
    public int Rating { get; set; } // 1-5 stars
}
