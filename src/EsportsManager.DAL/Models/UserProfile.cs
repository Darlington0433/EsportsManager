using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsportsManager.DAL.Models
{
    /// <summary>
    /// User Profile Model - Thông tin mở rộng của người dùng
    /// </summary>
    [Table("UserProfiles")]
    public class UserProfile
    {
        [Key]
        public int ProfileID { get; set; }

        [Required]
        public int UserID { get; set; }

        public string? Bio { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Social Links stored as JSON
        /// </summary>
        public string? SocialLinks { get; set; }

        /// <summary>
        /// Achievements stored as JSON
        /// </summary>
        public string? Achievements { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
