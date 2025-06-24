using System;

namespace EsportsManager.BL.DTOs {
    /// <summary>
    /// DTO feedback c?a viewer
    /// </summary>
    public class ViewerFeedbackDto
    {
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public string? Category { get; set; } // 'Tournament', 'Team', 'System', etc.
    }
}

