using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// Entity model cho bảng Teams trong database
    /// </summary>
    [Table("Teams")]
    public class Team
    {
        [Key]
        [Column("TeamID")]
        public int TeamID { get; set; }

        [Required]
        [Column("TeamName")]
        [StringLength(100)]
        public string TeamName { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("GameID")]
        public int GameID { get; set; }

        [Required]
        [Column("CreatedBy")]
        public int CreatedBy { get; set; }

        [Column("LogoURL")]
        [StringLength(255)]
        public string? LogoURL { get; set; }

        [Column("MaxMembers")]
        public int MaxMembers { get; set; } = 5;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("Status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation properties (nếu sử dụng Entity Framework)
        [ForeignKey("GameID")]
        public virtual Game? Game { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Users? Creator { get; set; }

        public virtual ICollection<TeamMember>? TeamMembers { get; set; }
    }

    /// <summary>
    /// Entity model cho bảng TeamMembers trong database
    /// </summary>
    [Table("TeamMembers")]
    public class TeamMember
    {
        [Key]
        [Column("TeamMemberID")]
        public int TeamMemberID { get; set; }

        [Required]
        [Column("TeamID")]
        public int TeamID { get; set; }

        [Required]
        [Column("UserID")]
        public int UserID { get; set; }

        [Column("JoinDate")]
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [Column("IsLeader")]
        public bool IsLeader { get; set; } = false;

        [Column("Position")]
        [StringLength(50)]
        public string? Position { get; set; }

        [Column("Status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation properties
        [ForeignKey("TeamID")]
        public virtual Team? Team { get; set; }

        [ForeignKey("UserID")]
        public virtual Users? User { get; set; }
    }

    /// <summary>
    /// Entity model cho bảng Games trong database
    /// </summary>
    [Table("Games")]
    public class Game
    {
        [Key]
        [Column("GameID")]
        public int GameID { get; set; }

        [Required]
        [Column("GameName")]
        [StringLength(100)]
        public string GameName { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Genre")]
        [StringLength(50)]
        public string? Genre { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Team>? Teams { get; set; }
    }
}
