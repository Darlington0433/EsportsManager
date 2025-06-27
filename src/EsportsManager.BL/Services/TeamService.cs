using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.DAL.Interfaces;
using EsportsManager.DAL.Models;

namespace EsportsManager.BL.Services
{
    /// <summary>
    /// Team Service Implementation - Quản lý các thao tác liên quan đến team
    /// Áp dụng SOLID principles
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        }

        /// <summary>
        /// Tạo team mới
        /// </summary>
        public async Task<TeamInfoDto> CreateTeamAsync(TeamCreateDto createDto, int creatorUserId)
        {
            try
            {
                var team = new Team
                {
                    TeamName = createDto.Name,
                    Description = createDto.Description,
                    LogoURL = createDto.Logo,
                    MaxMembers = createDto.MaxMembers,
                    CreatedBy = creatorUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Status = "Active",
                    GameID = 1 // Default game ID, should be passed in createDto in future
                };

                var createdTeam = await _teamRepository.CreateAsync(team);
                
                // Add creator as team leader
                await _teamRepository.AddMemberAsync(createdTeam.TeamID, creatorUserId, true);

                return MapToTeamInfoDto(createdTeam);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error creating team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin team theo ID
        /// </summary>
        public async Task<TeamInfoDto?> GetTeamByIdAsync(int teamId)
        {
            try
            {
                var team = await _teamRepository.GetByIdAsync(teamId);
                return team != null ? MapToTeamInfoDto(team) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting team by ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả team
        /// </summary>
        public async Task<List<TeamInfoDto>> GetAllTeamsAsync()
        {
            try
            {
                var teams = await _teamRepository.GetAllActiveAsync();
                return teams.Select(MapToTeamInfoDto).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting all teams: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy team của người chơi
        /// </summary>
        public async Task<TeamInfoDto?> GetPlayerTeamAsync(int playerId)
        {
            try
            {
                var team = await _teamRepository.GetPlayerTeamAsync(playerId);
                return team != null ? MapToTeamInfoDto(team) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting player team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách các thành viên của team
        /// </summary>
        public async Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId)
        {
            try
            {
                var members = await _teamRepository.GetTeamMembersAsync(teamId);
                return members.Select(MapToTeamMemberDto).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting team members: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin team
        /// </summary>
        public async Task<bool> UpdateTeamAsync(int teamId, TeamUpdateDto updateDto, int requestUserId)
        {
            try
            {
                // Check if user is team leader
                if (!await _teamRepository.IsTeamLeaderAsync(requestUserId, teamId))
                {
                    throw new UnauthorizedAccessException("Only team leader can update team information");
                }

                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return false;
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(updateDto.Name))
                    team.TeamName = updateDto.Name;
                
                if (updateDto.Description != null)
                    team.Description = updateDto.Description;
                
                if (updateDto.Logo != null)
                    team.LogoURL = updateDto.Logo;
                
                if (!string.IsNullOrEmpty(updateDto.Status))
                    team.Status = updateDto.Status;

                return await _teamRepository.UpdateAsync(team);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra xem người chơi có phải là thành viên của team không
        /// </summary>
        public async Task<bool> IsPlayerInTeamAsync(int playerId, int teamId)
        {
            try
            {
                return await _teamRepository.IsPlayerInTeamAsync(playerId, teamId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error checking team membership: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra xem người chơi có phải là leader của team không
        /// </summary>
        public async Task<bool> IsTeamLeaderAsync(int playerId, int teamId)
        {
            try
            {
                return await _teamRepository.IsTeamLeaderAsync(playerId, teamId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error checking team leadership: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Giải tán team
        /// </summary>
        public async Task<bool> DisbandTeamAsync(int teamId, int requestUserId)
        {
            try
            {
                // Check if user is team leader
                if (!await _teamRepository.IsTeamLeaderAsync(requestUserId, teamId))
                {
                    throw new UnauthorizedAccessException("Only team leader can disband the team");
                }

                return await _teamRepository.DisbandAsync(teamId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error disbanding team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm thành viên vào team
        /// </summary>
        public async Task<bool> AddMemberToTeamAsync(int teamId, int playerId, int requestUserId)
        {
            try
            {
                // Check if requestor is team leader
                if (!await _teamRepository.IsTeamLeaderAsync(requestUserId, teamId))
                {
                    throw new UnauthorizedAccessException("Only team leader can add members");
                }

                // Check if team can add more members
                if (!await _teamRepository.CanAddMemberAsync(teamId))
                {
                    throw new InvalidOperationException("Team has reached maximum capacity");
                }

                // Check if player is already in a team
                var playerTeam = await _teamRepository.GetPlayerTeamAsync(playerId);
                if (playerTeam != null)
                {
                    throw new InvalidOperationException("Player is already in a team");
                }

                return await _teamRepository.AddMemberAsync(teamId, playerId, false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding member to team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa thành viên khỏi team
        /// </summary>
        public async Task<bool> RemoveMemberAsync(int teamId, int playerId, int requestUserId)
        {
            try
            {
                // Check if requestor is team leader or removing themselves
                bool isLeader = await _teamRepository.IsTeamLeaderAsync(requestUserId, teamId);
                bool isSelf = requestUserId == playerId;

                if (!isLeader && !isSelf)
                {
                    throw new UnauthorizedAccessException("Only team leader can remove members or players can leave themselves");
                }

                // Check if trying to remove the leader
                if (await _teamRepository.IsTeamLeaderAsync(playerId, teamId))
                {
                    throw new InvalidOperationException("Cannot remove team leader. Transfer leadership first or disband the team");
                }

                return await _teamRepository.RemoveMemberAsync(teamId, playerId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error removing member from team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Yêu cầu tham gia team
        /// </summary>
        public async Task<bool> RequestToJoinTeamAsync(int teamId, int playerId, string? message = null)
        {
            try
            {
                // Check if player is already in a team
                var playerTeam = await _teamRepository.GetPlayerTeamAsync(playerId);
                if (playerTeam != null)
                {
                    throw new InvalidOperationException("Player is already in a team");
                }

                // Check if team exists and is active
                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null || !team.IsActive)
                {
                    throw new InvalidOperationException("Team not found or inactive");
                }

                // Check if team can add more members
                if (!await _teamRepository.CanAddMemberAsync(teamId))
                {
                    throw new InvalidOperationException("Team has reached maximum capacity");
                }

                // For now, auto-approve join requests (in future, this could create a pending request)
                return await _teamRepository.AddMemberAsync(teamId, playerId, false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error requesting to join team: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm team theo tên
        /// </summary>
        public async Task<List<TeamInfoDto>> SearchTeamsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new List<TeamInfoDto>();
                }

                var teams = await _teamRepository.SearchByNameAsync(searchTerm);
                return teams.Select(MapToTeamInfoDto).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error searching teams: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Map Team entity to TeamInfoDto
        /// </summary>
        private TeamInfoDto MapToTeamInfoDto(Team team)
        {
            return new TeamInfoDto
            {
                Id = team.TeamID,
                Name = team.TeamName,
                Description = team.Description ?? string.Empty,
                GameName = "Default Game", // TODO: Get actual game name from Game entity
                CreatedAt = team.CreatedAt,
                DateCreated = team.CreatedAt,
                Status = team.Status,
                MaxMembers = team.MaxMembers,
                LeaderId = team.CreatedBy,
                Logo = team.LogoURL
            };
        }

        /// <summary>
        /// Map TeamMember entity to TeamMemberDto
        /// </summary>
        private TeamMemberDto MapToTeamMemberDto(TeamMember member)
        {
            return new TeamMemberDto
            {
                UserId = member.UserID,
                Username = "Unknown", // TODO: Get actual username from User entity
                Role = member.IsLeader ? "Leader" : "Member",
                JoinDate = member.JoinDate,
                Status = member.Status ?? "Active"
            };
        }

        #endregion

        #region Admin Methods

        /// <summary>
        /// Lấy danh sách team chờ phê duyệt (Admin function)
        /// </summary>
        public async Task<List<TeamInfoDto>> GetPendingTeamsAsync()
        {
            try
            {
                var pendingTeams = await _teamRepository.GetPendingTeamsAsync();
                return pendingTeams.Select(MapToTeamInfoDto).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting pending teams: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phê duyệt team mới (Admin function)
        /// </summary>
        public async Task<bool> ApproveTeamAsync(int teamId)
        {
            try
            {
                return await _teamRepository.ApproveTeamAsync(teamId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error approving team {teamId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Từ chối team mới (Admin function)
        /// </summary>
        public async Task<bool> RejectTeamAsync(int teamId)
        {
            try
            {
                return await _teamRepository.RejectTeamAsync(teamId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error rejecting team {teamId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy danh sách yêu cầu tham gia team chờ phê duyệt (Admin function)
        /// </summary>
        public async Task<List<TeamJoinRequestDto>> GetPendingTeamJoinRequestsAsync()
        {
            try
            {
                var pendingRequests = await _teamRepository.GetPendingJoinRequestsAsync();
                return pendingRequests.Select(MapToTeamJoinRequestDto).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting pending join requests: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phê duyệt yêu cầu tham gia team (Admin function)
        /// </summary>
        public async Task<bool> ApproveTeamJoinRequestAsync(int requestId)
        {
            try
            {
                return await _teamRepository.ApproveJoinRequestAsync(requestId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error approving join request {requestId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Từ chối yêu cầu tham gia team (Admin function)
        /// </summary>
        public async Task<bool> RejectTeamJoinRequestAsync(int requestId)
        {
            try
            {
                return await _teamRepository.RejectJoinRequestAsync(requestId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error rejecting join request {requestId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map TeamJoinRequest entity to TeamJoinRequestDto
        /// </summary>
        private TeamJoinRequestDto MapToTeamJoinRequestDto(object joinRequest)
        {
            // This is a placeholder mapping - will need to be implemented based on actual TeamJoinRequest entity
            // For now, returning a basic DTO structure
            return new TeamJoinRequestDto
            {
                RequestId = 0, // TODO: Map from actual entity
                TeamId = 0,    // TODO: Map from actual entity
                TeamName = "Unknown", // TODO: Map from actual entity
                PlayerId = 0,  // TODO: Map from actual entity
                PlayerName = "Unknown", // TODO: Map from actual entity
                RequestDate = DateTime.UtcNow, // TODO: Map from actual entity
                Status = "Pending", // TODO: Map from actual entity
                Message = string.Empty // TODO: Map from actual entity
            };
        }

        #endregion
    }
}
