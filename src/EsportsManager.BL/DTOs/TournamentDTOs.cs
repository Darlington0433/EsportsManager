using System;

namespace EsportsManager.BL.DTOs;

/// <summary>
/// DTO cho đăng ký tham gia giải đấu
/// </summary>
public class TournamentRegistrationDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string TournamentName { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Notes { get; set; }
}