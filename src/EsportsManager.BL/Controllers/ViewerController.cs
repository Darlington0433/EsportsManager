using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;

namespace EsportsManager.BL.Controllers;

/// <summary>
/// ViewerController - Xử lý business logic cho Viewer operations
/// </summary>
public class ViewerController
{
    private readonly IUserService _userService;
    private readonly UserProfileDto _currentViewer;
    private readonly ITournamentService? _tournamentService;
    private readonly ITeamService? _teamService;
    private readonly IWalletService? _walletService;

    public ViewerController(
        IUserService userService,
        UserProfileDto currentViewer,
        ITournamentService? tournamentService = null,
        ITeamService? teamService = null,
        IWalletService? walletService = null)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _currentViewer = currentViewer ?? throw new ArgumentNullException(nameof(currentViewer));
        _tournamentService = tournamentService;
        _teamService = teamService;
        _walletService = walletService;

        // Validate viewer permissions
        if (_currentViewer.Role != "Viewer")
        {
            throw new UnauthorizedAccessException("Chỉ Viewer mới có quyền truy cập controller này.");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // VIEWER PROFILE MANAGEMENT
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Lấy thông tin cá nhân của viewer
    /// </summary>
    public async Task<UserProfileDto?> GetPersonalInfoAsync()
    {
        try
        {
            // Lấy thông tin người dùng mới nhất từ database
            var result = await _userService.GetUserProfileAsync(_currentViewer.Id);
            return result.IsSuccess ? result.Data : null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy thông tin cá nhân: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân (hạn chế hơn Player)
    /// </summary>
    public async Task<bool> UpdatePersonalInfoAsync(UserUpdateDto updateDto)
    {
        if (updateDto == null)
            throw new ArgumentNullException(nameof(updateDto));

        try
        {
            // Chuyển từ UserUpdateDto sang UserDto
            var userDto = new UserDto
            {
                Id = _currentViewer.Id,
                Username = _currentViewer.Username,
                Role = _currentViewer.Role,
                Email = updateDto.Email,
                FullName = updateDto.FullName,
                Status = _currentViewer.Status
            };

            var result = await _userService.UpdateUserProfileAsync(_currentViewer.Id, userDto);
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi cập nhật thông tin: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Thay đổi mật khẩu
    /// </summary>
    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Mật khẩu hiện tại không được rỗng", nameof(currentPassword));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Mật khẩu mới không được rỗng", nameof(newPassword));

        try
        {
            var updatePasswordDto = new UpdatePasswordDto
            {
                UserId = _currentViewer.Id,
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var result = await _userService.UpdatePasswordAsync(updatePasswordDto);
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi thay đổi mật khẩu: {ex.Message}", ex);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // TOURNAMENT VIEWING
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Xem danh sách tất cả giải đấu (public)
    /// </summary>
    public async Task<List<EsportsManager.BL.DTOs.TournamentInfoDto>> GetAllTournamentsAsync()
    {
        try
        {
            if (_tournamentService is null)
                throw new InvalidOperationException("Dịch vụ giải đấu chưa được khởi tạo");

            return await _tournamentService.GetAllTournamentsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy danh sách giải đấu: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Xem chi tiết một giải đấu
    /// </summary>
    public async Task<EsportsManager.BL.DTOs.TournamentInfoDto?> GetTournamentDetailAsync(int tournamentId)
    {
        try
        {
            if (_tournamentService is null)
                throw new InvalidOperationException("Dịch vụ giải đấu chưa được khởi tạo");

            return await _tournamentService.GetTournamentByIdAsync(tournamentId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy chi tiết giải đấu: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Xem danh sách các team tham gia giải đấu
    /// </summary>
    public async Task<List<EsportsManager.BL.DTOs.TeamInfoDto>> GetTournamentTeamsAsync(int tournamentId)
    {
        try
        {
            if (_tournamentService is null)
                throw new InvalidOperationException("Dịch vụ giải đấu chưa được khởi tạo");

            return await _tournamentService.GetTournamentTeamsAsync(tournamentId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy danh sách đội tham gia: {ex.Message}", ex);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // TEAM VIEWING
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Xem danh sách tất cả team (public)
    /// </summary>
    public async Task<List<EsportsManager.BL.DTOs.TeamInfoDto>> GetAllTeamsAsync()
    {
        try
        {
            if (_teamService is null)
                throw new InvalidOperationException("Dịch vụ team chưa được khởi tạo");

            return await _teamService.GetAllTeamsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy danh sách team: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Xem chi tiết một team
    /// </summary>
    public async Task<EsportsManager.BL.DTOs.TeamInfoDto?> GetTeamDetailAsync(int teamId)
    {
        try
        {
            if (_teamService is null)
                throw new InvalidOperationException("Dịch vụ team chưa được khởi tạo");

            return await _teamService.GetTeamByIdAsync(teamId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy chi tiết team: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Xem danh sách thành viên của một team
    /// </summary>
    public async Task<List<EsportsManager.BL.DTOs.TeamMemberDto>> GetTeamMembersAsync(int teamId)
    {
        try
        {
            if (_teamService is null)
                throw new InvalidOperationException("Dịch vụ team chưa được khởi tạo");

            return await _teamService.GetTeamMembersAsync(teamId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy danh sách thành viên: {ex.Message}", ex);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // DONATION FEATURE
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Donate cho team hoặc giải đấu
    /// </summary>
    public async Task<TransactionResultDto> DonateAsync(EsportsManager.BL.DTOs.DonationDto donationDto)
    {
        if (donationDto == null)
            throw new ArgumentNullException(nameof(donationDto));

        try
        {
            if (_walletService is null)
                throw new InvalidOperationException("Dịch vụ ví điện tử chưa được khởi tạo");

            return await _walletService.DonateAsync(_currentViewer.Id, donationDto);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi donate: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Lấy thông tin ví điện tử
    /// </summary>
    public async Task<WalletInfoDto?> GetWalletInfoAsync()
    {
        try
        {
            if (_walletService is null)
                throw new InvalidOperationException("Dịch vụ ví điện tử chưa được khởi tạo");

            return await _walletService.GetWalletByUserIdAsync(_currentViewer.Id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi lấy thông tin ví: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Nạp tiền vào ví
    /// </summary>
    public async Task<TransactionResultDto> DepositAsync(DepositDto depositDto)
    {
        if (depositDto == null)
            throw new ArgumentNullException(nameof(depositDto));

        try
        {
            if (_walletService is null)
                throw new InvalidOperationException("Dịch vụ ví điện tử chưa được khởi tạo");

            return await _walletService.DepositAsync(_currentViewer.Id, depositDto);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi nạp tiền: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Rút tiền từ ví
    /// </summary>
    public async Task<TransactionResultDto> WithdrawAsync(WithdrawalDto withdrawalDto)
    {
        if (withdrawalDto == null)
            throw new ArgumentNullException(nameof(withdrawalDto));

        try
        {
            if (_walletService is null)
                throw new InvalidOperationException("Dịch vụ ví điện tử chưa được khởi tạo");

            return await _walletService.WithdrawAsync(_currentViewer.Id, withdrawalDto);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi rút tiền: {ex.Message}", ex);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // FEEDBACK SYSTEM
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Gửi feedback về giải đấu
    /// </summary>
    public async Task<bool> SendFeedbackAsync(EsportsManager.BL.DTOs.FeedbackDto feedbackDto)
    {
        if (feedbackDto == null)
            throw new ArgumentNullException(nameof(feedbackDto));

        try
        {
            if (_tournamentService is null)
                throw new InvalidOperationException("Dịch vụ giải đấu chưa được khởi tạo");

            return await _tournamentService.SubmitFeedbackAsync(_currentViewer.Id, feedbackDto);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Lỗi khi gửi feedback: {ex.Message}", ex);
        }
    }
}

// ═══════════════════════════════════════════════════════════════
// VIEWER-SPECIFIC DTOs
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// DTO cho cập nhật thông tin viewer (hạn chế)
/// </summary>
public class ViewerUpdateDto
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// DTO hiển thị giải đấu cho viewer
/// </summary>
public class TournamentViewDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public required string Status { get; set; }
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
}

/// <summary>
/// DTO chi tiết giải đấu
/// </summary>
public class TournamentDetailDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required string Status { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public required string Rules { get; set; }
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
    public required string Organizer { get; set; }
    public required string Location { get; set; }
}

/// <summary>
/// DTO lịch thi đấu
/// </summary>
public class MatchScheduleDto
{
    public int MatchId { get; set; }
    public required string Team1 { get; set; }
    public required string Team2 { get; set; }
    public DateTime ScheduledTime { get; set; }
    public required string Status { get; set; }
    public required string Round { get; set; }
}

/// <summary>
/// DTO kết quả trận đấu
/// </summary>
public class MatchResultDto
{
    public int MatchId { get; set; }
    public required string Team1 { get; set; }
    public required string Team2 { get; set; }
    public required string Winner { get; set; }
    public required string Score { get; set; }
    public DateTime CompletedTime { get; set; }
    public required string Round { get; set; }
}

/// <summary>
/// DTO thông tin công khai của team
/// </summary>
public class TeamPublicInfoDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int MemberCount { get; set; }
    public required string Achievements { get; set; }
    public DateTime EstablishedDate { get; set; }
}

/// <summary>
/// DTO feedback của viewer
/// </summary>
public class ViewerFeedbackDto
{
    public required string Subject { get; set; }
    public required string Content { get; set; }
    public string? Category { get; set; } // "Tournament", "Team", "System", etc.
}

/// <summary>
/// DTO donation
/// </summary>
public class DonationDto
{
    public decimal Amount { get; set; }
    public required string Message { get; set; }
    public int? TournamentId { get; set; }
    public int? TeamId { get; set; }
    public required string DonationType { get; set; } // "Tournament", "Team"
}
