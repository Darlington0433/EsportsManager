using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;

namespace EsportsManager.BL.Controllers
{
    /// <summary>
    /// AdminControllerV2 - Xử lý business logic cho Admin operations
    /// Tách biệt hoàn toàn khỏi UI concerns
    /// </summary>
    public class AdminControllerV2
    {
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly UserProfileDto _currentAdmin;

        public AdminControllerV2(IUserService userService, ITournamentService tournamentService,
                             ITeamService teamService, UserProfileDto currentAdmin)
        {
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            this._teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            this._currentAdmin = currentAdmin ?? throw new ArgumentNullException(nameof(currentAdmin));

            // Validate admin permissions
            if (this._currentAdmin.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền truy cập controller này.");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // USER MANAGEMENT OPERATIONS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Lấy danh sách tất cả người dùng (chỉ Admin)
        /// </summary>
        public async Task<List<UserProfileDto>> GetAllUsersAsync()
        {
            try
            {
                return await _userService.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy danh sách người dùng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo từ khóa
        /// </summary>
        public async Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Từ khóa tìm kiếm không được rỗng", nameof(searchTerm));

            try
            {
                return await _userService.SearchUsersAsync(searchTerm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi tìm kiếm người dùng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thay đổi trạng thái người dùng (Active/Inactive)
        /// </summary>
        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ", nameof(userId));

            try
            {
                return await _userService.ToggleUserStatusAsync(userId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi thay đổi trạng thái người dùng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reset mật khẩu người dùng
        /// </summary>
        public async Task<string> ResetUserPasswordAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ", nameof(userId));

            try
            {
                string newPassword = await _userService.ResetPasswordAsync(userId);
                await LogAdminActionAsync("ResetPassword", $"Reset password for user {userId}");
                return newPassword;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi reset mật khẩu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa người dùng (soft delete)
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ", nameof(userId));

            // Không cho phép Admin xóa chính mình
            if (userId == _currentAdmin.Id)
                throw new InvalidOperationException("Không thể xóa tài khoản của chính mình");

            try
            {
                bool result = await _userService.DeleteUserAsync(userId);
                if (result)
                {
                    await LogAdminActionAsync("DeleteUser", $"Deleted user {userId}");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi xóa người dùng: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết người dùng
        /// </summary>
        public async Task<UserProfileDto?> GetUserDetailsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId không hợp lệ", nameof(userId));

            try
            {
                var users = await _userService.GetAllUsersAsync();
                return users.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thông tin người dùng: {ex.Message}", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SYSTEM STATISTICS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Lấy thống kê tổng quan hệ thống
        /// </summary>
        public async Task<SystemStatsDto> GetSystemStatsAsync()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var tournaments = await _tournamentService.GetAllTournamentsAsync();
                var teams = await _teamService.GetAllTeamsAsync();

                return new SystemStatsDto
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.Status == "Active"),
                    TotalTournaments = tournaments.Count,
                    ActiveTournaments = tournaments.Count(t => t.Status == "Active"),
                    TotalTeams = teams.Count,
                    TotalRevenue = 0 // TODO: Calculate from actual revenue data
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thống kê hệ thống: {ex.Message}", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // TOURNAMENT MANAGEMENT (ADMIN FUNCTIONS)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Lấy danh sách tất cả giải đấu (Admin only)
        /// </summary>
        public async Task<List<TournamentInfoDto>> GetAllTournamentsAsync()
        {
            try
            {
                return await _tournamentService.GetAllTournamentsAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy danh sách giải đấu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo giải đấu mới
        /// </summary>
        public async Task<bool> CreateTournamentAsync(TournamentCreateDto tournamentDto)
        {
            if (tournamentDto == null)
                throw new ArgumentNullException(nameof(tournamentDto));

            if (string.IsNullOrWhiteSpace(tournamentDto.TournamentName))
                throw new ArgumentException("Tournament name cannot be empty", nameof(tournamentDto));

            try
            {
                tournamentDto.CreatedBy = _currentAdmin.Id;
                var createdTournament = await _tournamentService.CreateTournamentAsync(tournamentDto);
                return createdTournament != null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error creating tournament: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết giải đấu (Admin only)
        /// </summary>
        public async Task<TournamentInfoDto?> GetTournamentDetailsAsync(int tournamentId)
        {
            if (tournamentId <= 0)
                throw new ArgumentException("TournamentId không hợp lệ", nameof(tournamentId));

            try
            {
                return await _tournamentService.GetTournamentByIdAsync(tournamentId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thông tin giải đấu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hủy giải đấu (Admin only)
        /// </summary>
        public async Task<bool> CancelTournamentAsync(int tournamentId, string reason)
        {
            if (tournamentId <= 0)
                throw new ArgumentException("TournamentId không hợp lệ", nameof(tournamentId));
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Lý do hủy không được rỗng", nameof(reason));

            try
            {
                // TODO: Implement tournament cancellation via TournamentService
                bool result = true; // Mock result for now
                if (result)
                {
                    await LogAdminActionAsync("CancelTournament", $"Cancelled tournament ID: {tournamentId}, Reason: {reason}");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi hủy giải đấu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê giải đấu (Admin only)
        /// </summary>
        public async Task<TournamentStatsDto> GetTournamentStatsAsync()
        {
            try
            {
                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                return new TournamentStatsDto
                {
                    TotalTournaments = tournaments.Count,
                    ActiveTournaments = tournaments.Count(t => t.Status == "Active"),
                    CompletedTournaments = tournaments.Count(t => t.Status == "Completed"),
                    TotalParticipants = tournaments.Sum(t => t.CurrentParticipants),
                    TotalPrizePool = tournaments.Sum(t => t.PrizePool)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy thống kê giải đấu: {ex.Message}", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ADMIN LOGGING
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Log admin actions for audit trail
        /// </summary>
        private async Task LogAdminActionAsync(string action, string details)
        {
            try
            {
                // TODO: Implement proper logging to database
                await Task.Delay(10); // Simulate async operation
                Console.WriteLine($"[ADMIN LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {_currentAdmin.Username}: {action} - {details}");
            }
            catch (Exception ex)
            {
                // Log error but don't throw - logging shouldn't break business operations
                Console.WriteLine($"Logging error: {ex.Message}");
            }
        }
    }
}
