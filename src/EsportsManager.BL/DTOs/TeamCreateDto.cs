using System;

namespace EsportsManager.BL.DTOs
{
    /// <summary>
    /// DTO cho tạo team mới
    /// </summary>
    public class TeamCreateDto
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public int MaxMembers { get; set; } = 10;
    }
}
