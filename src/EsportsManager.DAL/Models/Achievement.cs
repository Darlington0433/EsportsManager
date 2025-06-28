using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Achievement Model - Thành tích của người chơi
    /// </summary>
    [Table("Achievements")]
    public class Achievement
    {
        [Key]
        public int AchievementID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string AchievementType { get; set; } = string.Empty;

        public DateTime DateAchieved { get; set; } = DateTime.Now;

        [Required]
        public int AssignedBy { get; set; }

        public int? TournamentID { get; set; }

        public int? TeamID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
