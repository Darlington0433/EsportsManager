using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using Microsoft.Extensions.Logging;

namespace EsportsManager.BL.Services;

/// <summary>
/// Team Service Implementation - Mock data for development
/// Production: Replace with real database operations
/// </summary>
public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private static readonly List<TeamInfoDto> _mockTeams = new();
    private static readonly List<TeamMemberDto> _mockTeamMembers = new();
    private static readonly List<JoinRequestDto> _mockJoinRequests = new();
    private static int _nextTeamId = 1;
    private static int _nextJoinRequestId = 1;

    public TeamService(ILogger<TeamService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeMockData();
    }

    public async Task<TeamInfoDto> CreateTeamAsync(TeamCreateDto createDto, int creatorUserId)
    {
        if (createDto == null)
            throw new ArgumentNullException(nameof(createDto));

        if (string.IsNullOrWhiteSpace(createDto.Name))
            throw new ArgumentException("Tên team không được để trống", nameof(createDto.Name));

        // Kiểm tra người chơi đã có team chưa
        var existingTeam = _mockTeams.FirstOrDefault(t =>
            _mockTeamMembers.Any(m => m.UserId == creatorUserId && m.Status == "Active" && m.Role == "Leader"));
        if (existingTeam != null)
            throw new InvalidOperationException($"Người chơi đã là leader của team {existingTeam.Name}");

        try
        {
            // Tạo team mới
            var team = new TeamInfoDto
            {
                Id = _nextTeamId++,
                Name = createDto.Name,
                Description = createDto.Description,
                Logo = createDto.Logo,
                MemberCount = 1,
                LeaderId = creatorUserId,
                LeaderName = $"user_{creatorUserId}", // Thực tế sẽ lấy username từ repository
                CreatedAt = DateTime.Now,
                Status = "Active"
            };

            // Thêm người tạo vào team với role Leader
            _mockTeamMembers.Add(new TeamMemberDto
            {
                UserId = creatorUserId,
                Username = $"user_{creatorUserId}", // Thực tế sẽ lấy username từ repository
                Role = "Leader",
                JoinedAt = DateTime.Now,
                Status = "Active"
            });

            _mockTeams.Add(team);

            await Task.Delay(30); // Simulate async operation
            _logger.LogInformation("Created team {TeamId}: {Name} by user {UserId}", team.Id, team.Name, creatorUserId);

            return team;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team {Name} by user {UserId}", createDto.Name, creatorUserId);
            throw;
        }
    }

    public async Task<TeamInfoDto?> GetTeamByIdAsync(int teamId)
    {
        await Task.Delay(20); // Simulate async operation
        return _mockTeams.FirstOrDefault(t => t.Id == teamId);
    }

    public async Task<List<TeamInfoDto>> GetAllTeamsAsync()
    {
        await Task.Delay(30); // Simulate async operation
        return _mockTeams.Where(t => t.Status == "Active").ToList();
    }

    public async Task<TeamInfoDto?> GetPlayerTeamAsync(int playerId)
    {
        await Task.Delay(20); // Simulate async operation
        var member = _mockTeamMembers.FirstOrDefault(m => m.UserId == playerId && m.Status == "Active");
        if (member == null)
            return null;

        var teamId = _mockTeams.First(t =>
            _mockTeamMembers.Any(m => m.UserId == playerId && m.Status == "Active")).Id;

        return await GetTeamByIdAsync(teamId);
    }

    public async Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId)
    {
        await Task.Delay(30); // Simulate async operation
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return new List<TeamMemberDto>();

        return _mockTeamMembers.Where(m =>
            _mockTeams.First(t => t.LeaderId == m.UserId ||
                _mockTeamMembers.Any(tm => tm.UserId == m.UserId)).Id == teamId).ToList();
    }

    public async Task<bool> UpdateTeamAsync(int teamId, TeamUpdateDto updateDto, int requestUserId)
    {
        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto));

        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        // Kiểm tra quyền cập nhật
        if (!await IsTeamLeaderAsync(requestUserId, teamId))
            throw new UnauthorizedAccessException("Chỉ leader mới có quyền cập nhật thông tin team");

        try
        {
            // Update các trường không null
            if (!string.IsNullOrEmpty(updateDto.Name))
                team.Name = updateDto.Name;

            if (updateDto.Description != null)
                team.Description = updateDto.Description;

            if (updateDto.Logo != null)
                team.Logo = updateDto.Logo;

            if (updateDto.Status != null)
                team.Status = updateDto.Status;

            if (updateDto.LeaderId.HasValue)
            {
                // Kiểm tra xem người được chỉ định có phải là thành viên team không
                if (!await IsPlayerInTeamAsync(updateDto.LeaderId.Value, teamId))
                    throw new InvalidOperationException("Người được chỉ định làm leader phải là thành viên team");

                // Cập nhật leader cũ thành Member
                var oldLeaderMember = _mockTeamMembers.FirstOrDefault(m => m.UserId == team.LeaderId && m.Role == "Leader");
                if (oldLeaderMember != null)
                    oldLeaderMember.Role = "Member";

                // Cập nhật leader mới
                var newLeaderMember = _mockTeamMembers.FirstOrDefault(m => m.UserId == updateDto.LeaderId.Value);
                if (newLeaderMember != null)
                    newLeaderMember.Role = "Leader";

                team.LeaderId = updateDto.LeaderId.Value;
                team.LeaderName = $"user_{updateDto.LeaderId.Value}"; // Thực tế sẽ lấy username từ repository
            }

            await Task.Delay(30);
            _logger.LogInformation("Updated team {TeamId}: {Name} by user {UserId}", teamId, team.Name, requestUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId} by user {UserId}", teamId, requestUserId);
            throw;
        }
    }

    public async Task<bool> IsPlayerInTeamAsync(int playerId, int teamId)
    {
        await Task.Delay(10);
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        return _mockTeamMembers.Any(m => m.UserId == playerId && m.Status == "Active" &&
            _mockTeams.First(t => t.LeaderId == m.UserId ||
                _mockTeamMembers.Any(tm => tm.UserId == m.UserId)).Id == teamId);
    }

    public async Task<bool> IsTeamLeaderAsync(int playerId, int teamId)
    {
        await Task.Delay(10);
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        return team?.LeaderId == playerId;
    }

    public async Task<bool> DisbandTeamAsync(int teamId, int requestUserId)
    {
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        // Kiểm tra quyền giải tán
        if (!await IsTeamLeaderAsync(requestUserId, teamId))
            throw new UnauthorizedAccessException("Chỉ leader mới có quyền giải tán team");

        // Kiểm tra xem team đang tham gia giải đấu không
        if (team.CurrentTournamentId.HasValue)
            throw new InvalidOperationException("Không thể giải tán team đang tham gia giải đấu");

        try
        {
            team.Status = "Disbanded";

            // Cập nhật trạng thái tất cả thành viên
            var members = await GetTeamMembersAsync(teamId);
            foreach (var member in members)
            {
                member.Status = "Inactive";
            }

            await Task.Delay(40);
            _logger.LogInformation("Disbanded team {TeamId}: {Name} by user {UserId}", teamId, team.Name, requestUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disbanding team {TeamId} by user {UserId}", teamId, requestUserId);
            throw;
        }
    }

    public async Task<bool> AddMemberToTeamAsync(int teamId, int playerId, int requestUserId)
    {
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        // Kiểm tra quyền thêm thành viên
        if (!await IsTeamLeaderAsync(requestUserId, teamId))
            throw new UnauthorizedAccessException("Chỉ leader mới có quyền thêm thành viên");

        // Kiểm tra số lượng thành viên
        if (team.MemberCount >= team.MaxMembers)
            throw new InvalidOperationException($"Team đã đạt số lượng thành viên tối đa ({team.MaxMembers})");

        // Kiểm tra người chơi đã có team chưa
        if (await GetPlayerTeamAsync(playerId) != null)
            throw new InvalidOperationException("Người chơi đã là thành viên của team khác");

        try
        {
            // Thêm thành viên mới
            _mockTeamMembers.Add(new TeamMemberDto
            {
                UserId = playerId,
                Username = $"user_{playerId}", // Thực tế sẽ lấy username từ repository
                Role = "Member",
                JoinedAt = DateTime.Now,
                Status = "Active"
            });

            // Cập nhật số lượng thành viên
            team.MemberCount++;

            await Task.Delay(30);
            _logger.LogInformation("Added player {PlayerId} to team {TeamId}: {Name} by user {UserId}",
                playerId, teamId, team.Name, requestUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player {PlayerId} to team {TeamId} by user {UserId}",
                playerId, teamId, requestUserId);
            throw;
        }
    }

    public async Task<bool> RemoveMemberAsync(int teamId, int playerId, int requestUserId)
    {
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        // Không thể xóa leader (phải chuyển quyền leader trước)
        if (team.LeaderId == playerId)
            throw new InvalidOperationException("Không thể xóa leader. Hãy chuyển quyền leader trước.");

        // Kiểm tra quyền xóa thành viên (leader hoặc tự rời đi)
        if (requestUserId != playerId && !await IsTeamLeaderAsync(requestUserId, teamId))
            throw new UnauthorizedAccessException("Bạn không có quyền xóa thành viên này");

        // Kiểm tra người chơi có phải là thành viên team không
        if (!await IsPlayerInTeamAsync(playerId, teamId))
            return false;

        try
        {
            // Tìm và cập nhật trạng thái thành viên
            var member = _mockTeamMembers.FirstOrDefault(m =>
                m.UserId == playerId && m.Status == "Active");

            if (member == null)
                return false;

            member.Status = "Inactive";

            // Cập nhật số lượng thành viên
            team.MemberCount--;

            await Task.Delay(30);
            _logger.LogInformation("Removed player {PlayerId} from team {TeamId}: {Name} by user {UserId}",
                playerId, teamId, team.Name, requestUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing player {PlayerId} from team {TeamId} by user {UserId}",
                playerId, teamId, requestUserId);
            throw;
        }
    }

    public async Task<bool> RequestToJoinTeamAsync(int teamId, int playerId, string? message = null)
    {
        var team = _mockTeams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
            return false;

        // Kiểm tra người chơi đã có team chưa
        if (await GetPlayerTeamAsync(playerId) != null)
            throw new InvalidOperationException("Bạn đã là thành viên của team khác");

        // Kiểm tra team còn chỗ không
        if (team.MemberCount >= team.MaxMembers)
            throw new InvalidOperationException($"Team đã đạt số lượng thành viên tối đa ({team.MaxMembers})");

        // Kiểm tra đã gửi yêu cầu chưa
        var existingRequest = _mockJoinRequests.FirstOrDefault(r =>
            r.TeamId == teamId && r.PlayerId == playerId && r.Status == "Pending");

        if (existingRequest != null)
            throw new InvalidOperationException("Bạn đã gửi yêu cầu tham gia team này rồi");

        try
        {
            // Tạo yêu cầu mới
            var request = new JoinRequestDto
            {
                Id = _nextJoinRequestId++,
                TeamId = teamId,
                TeamName = team.Name,
                PlayerId = playerId,
                PlayerName = $"user_{playerId}", // Thực tế sẽ lấy username từ repository
                Message = message,
                RequestedAt = DateTime.Now,
                Status = "Pending"
            };

            _mockJoinRequests.Add(request);

            await Task.Delay(20);
            _logger.LogInformation("Player {PlayerId} requested to join team {TeamId}: {Name}",
                playerId, teamId, team.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting to join team {TeamId} by player {PlayerId}",
                teamId, playerId);
            throw;
        }
    }

    public async Task<List<TeamInfoDto>> SearchTeamsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllTeamsAsync();

        await Task.Delay(30);
        return _mockTeams
            .Where(t => t.Status == "Active" &&
                (t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)))
            .ToList();
    }

    /// <summary>
    /// Khởi tạo dữ liệu mẫu
    /// </summary>
    private void InitializeMockData()
    {
        if (_mockTeams.Any())
            return;

        // Tạo team mẫu
        _mockTeams.Add(new TeamInfoDto
        {
            Id = _nextTeamId++,
            Name = "Team Alpha",
            Description = "Đội tuyển chuyên nghiệp hàng đầu",
            MemberCount = 5,
            MaxMembers = 5,
            LeaderId = 2, // Giả sử đây là ID của một player
            LeaderName = "pro_gamer_vn",
            CreatedAt = DateTime.Now.AddMonths(-6),
            Achievements = new List<string> { "Vô địch Esports Championship 2024", "Top 3 Mobile Legends Cup" },
            Status = "Active"
        });

        _mockTeams.Add(new TeamInfoDto
        {
            Id = _nextTeamId++,
            Name = "Team Beta",
            Description = "Đội tuyển trẻ đầy triển vọng",
            MemberCount = 4,
            MaxMembers = 5,
            LeaderId = 4,
            LeaderName = "mobile_legends_pro",
            CreatedAt = DateTime.Now.AddMonths(-2),
            Achievements = new List<string> { "Top 8 Esports Championship 2024" },
            Status = "Active"
        });

        // Thêm thành viên cho team
        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 2,
            Username = "pro_gamer_vn",
            Role = "Leader",
            JoinedAt = DateTime.Now.AddMonths(-6),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 5,
            Username = "gamer123",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-5),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 6,
            Username = "esports_player",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-5),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 7,
            Username = "pro_player1",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-4),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 8,
            Username = "gaming_master",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-3),
            Status = "Active"
        });

        // Team Beta members
        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 4,
            Username = "mobile_legends_pro",
            Role = "Leader",
            JoinedAt = DateTime.Now.AddMonths(-2),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 9,
            Username = "ml_player1",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-2),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 10,
            Username = "ml_player2",
            Role = "Member",
            JoinedAt = DateTime.Now.AddMonths(-1),
            Status = "Active"
        });

        _mockTeamMembers.Add(new TeamMemberDto
        {
            UserId = 11,
            Username = "game_expert",
            Role = "Member",
            JoinedAt = DateTime.Now.AddDays(-15),
            Status = "Active"
        });

        // Thêm yêu cầu tham gia mẫu
        _mockJoinRequests.Add(new JoinRequestDto
        {
            Id = _nextJoinRequestId++,
            TeamId = 1,
            TeamName = "Team Alpha",
            PlayerId = 12,
            PlayerName = "new_player",
            Message = "Tôi muốn tham gia team của bạn",
            RequestedAt = DateTime.Now.AddDays(-2),
            Status = "Pending"
        });
    }
}
