using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;

namespace EsportsManager.BL.Interfaces
{

    /// <summary>
    /// Team Service Interface - Quản lý các thao tác liên quan đến team
    /// Áp dụng Interface Segregation Principle
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// Tạo team mới
        /// </summary>
        Task<TeamInfoDto> CreateTeamAsync(TeamCreateDto createDto, int creatorUserId);

        /// <summary>
        /// Lấy thông tin team theo ID
        /// </summary>
        Task<TeamInfoDto?> GetTeamByIdAsync(int teamId);

        /// <summary>
        /// Lấy danh sách tất cả team
        /// </summary>
        Task<List<TeamInfoDto>> GetAllTeamsAsync();

        /// <summary>
        /// Lấy team của người chơi
        /// </summary>
        Task<TeamInfoDto?> GetPlayerTeamAsync(int playerId);

        /// <summary>
        /// Lấy danh sách các thành viên của team
        /// </summary>
        Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId);

        /// <summary>
        /// Cập nhật thông tin team
        /// </summary>
        Task<bool> UpdateTeamAsync(int teamId, TeamUpdateDto updateDto, int requestUserId);

        /// <summary>
        /// Kiểm tra xem người chơi có phải là thành viên của team không
        /// </summary>
        Task<bool> IsPlayerInTeamAsync(int playerId, int teamId);

        /// <summary>
        /// Kiểm tra xem người chơi có phải là leader của team không
        /// </summary>
        Task<bool> IsTeamLeaderAsync(int playerId, int teamId);

        /// <summary>
        /// Giải tán team
        /// </summary>
        Task<bool> DisbandTeamAsync(int teamId, int requestUserId);

        /// <summary>
        /// Thêm thành viên vào team
        /// </summary>
        Task<bool> AddMemberToTeamAsync(int teamId, int playerId, int requestUserId);

        /// <summary>
        /// Xóa thành viên khỏi team
        /// </summary>
        Task<bool> RemoveMemberAsync(int teamId, int playerId, int requestUserId);

        /// <summary>
        /// Yêu cầu tham gia team
        /// </summary>
        Task<bool> RequestToJoinTeamAsync(int teamId, int playerId, string? message = null);

        /// <summary>
        /// Chuyển giao quyền leader cho thành viên khác
        /// </summary>
        Task<bool> TransferLeadershipAsync(int teamId, int currentLeaderId, int newLeaderId);

        /// <summary>
        /// Tìm kiếm team theo tên
        /// </summary>
        Task<List<TeamInfoDto>> SearchTeamsAsync(string searchTerm);

        /// <summary>
        /// Lấy danh sách team chờ phê duyệt (Admin function)
        /// </summary>
        Task<List<TeamInfoDto>> GetPendingTeamsAsync();

        /// <summary>
        /// Phê duyệt team mới (Admin function)
        /// </summary>
        Task<bool> ApproveTeamAsync(int teamId);

        /// <summary>
        /// Từ chối team mới (Admin function)
        /// </summary>
        Task<bool> RejectTeamAsync(int teamId);

        /// <summary>
        /// Lấy danh sách yêu cầu tham gia team chờ phê duyệt (Admin function)
        /// </summary>
        Task<List<TeamJoinRequestDto>> GetPendingTeamJoinRequestsAsync();

        /// <summary>
        /// Phê duyệt yêu cầu tham gia team (Admin function)
        /// </summary>
        Task<bool> ApproveTeamJoinRequestAsync(int requestId);

        /// <summary>
        /// Từ chối yêu cầu tham gia team (Admin function)
        /// </summary>
        Task<bool> RejectTeamJoinRequestAsync(int requestId);
    }
}
