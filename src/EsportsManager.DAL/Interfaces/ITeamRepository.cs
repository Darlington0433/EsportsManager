using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.DAL.Models;

namespace EsportsManager.DAL.Interfaces
{
    /// <summary>
    /// Interface cho Team Repository
    /// Quản lý các operations liên quan đến Teams và TeamMembers
    /// </summary>
    public interface ITeamRepository
    {
        // Team CRUD Operations
        Task<Team> CreateAsync(Team team);
        Task<Team?> GetByIdAsync(int teamId);
        Task<Team?> GetByNameAsync(string teamName);
        Task<List<Team>> GetAllActiveAsync();
        Task<bool> UpdateAsync(Team team);
        Task<bool> DeleteAsync(int teamId);
        Task<bool> DisbandAsync(int teamId);

        // Team Search and Filter
        Task<List<Team>> SearchByNameAsync(string searchTerm);
        Task<List<Team>> GetByGameIdAsync(int gameId);
        Task<List<Team>> GetByCreatorAsync(int creatorId);
        Task<List<Team>> GetAllAsync(); // Method để lấy tất cả team bao gồm pending

        // Team Member Operations
        Task<bool> AddMemberAsync(int teamId, int userId, bool isLeader = false, string? position = null);
        Task<bool> RemoveMemberAsync(int teamId, int userId);
        Task<bool> UpdateMemberAsync(int teamId, int userId, bool? isLeader = null, string? position = null, string? status = null);
        Task<List<TeamMember>> GetTeamMembersAsync(int teamId);
        Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId);
        Task<Team?> GetPlayerTeamAsync(int playerId);
        Task<bool> IsPlayerInTeamAsync(int playerId, int teamId);
        Task<bool> IsTeamLeaderAsync(int playerId, int teamId);

        // Team Statistics
        Task<int> GetMemberCountAsync(int teamId);
        Task<bool> CanAddMemberAsync(int teamId);
        Task<bool> ExistsAsync(int teamId);
        Task<bool> IsTeamNameExistsAsync(string teamName);

        // Advanced Operations
        Task<bool> TransferLeadershipAsync(int teamId, int currentLeaderId, int newLeaderId);
        Task<List<Team>> GetTeamsWithMemberCountAsync();
        Task<bool> IsPlayerLeaderOfAnyTeamAsync(int playerId);
    }
}
