using System;
using System.Collections.Generic;

namespace EsportsManager.BL.DTOs
{

    /// <summary>
    /// DTO tạo team mới
    /// </summary>
    public class TeamCreateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }

    /// <summary>
    /// DTO cập nhật thông tin team
    /// </summary>
    public class TeamUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public int? LeaderId { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO thành viên team
    /// </summary>
    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "Member"; // Leader, Member
        public DateTime JoinedAt { get; set; }
        public string Status { get; set; } = "Active";
    }

    /// <summary>
    /// DTO thông tin công khai của team cho viewer
    /// </summary>
    public class TeamPublicInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public string? Achievements { get; set; }
        public DateTime EstablishedDate { get; set; }
    }

    /// <summary>
    /// DTO yêu cầu tham gia team
    /// </summary>
    public class JoinRequestDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected
    }

    /// <summary>
    /// PagedResult - Kết quả phân trang
    /// </summary>
    public class PagedResult<T> where T : class
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
