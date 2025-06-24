using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{
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
}

