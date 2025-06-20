using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace EsportsManager.UI.Menus
{
    public class AdminMenu
    {
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly ILogger<AdminMenu> _logger;
        private readonly int _currentUserId;
        private readonly string _currentUsername;

        public AdminMenu(
            IUserService userService,
            ITournamentService tournamentService,
            ITeamService teamService,
            ILogger<AdminMenu> logger,
            int currentUserId,
            string currentUsername)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserId = currentUserId;
            _currentUsername = currentUsername;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                try
                {
                    ConsoleHelper.ShowHeader($"MENU ADMIN");

                    Console.WriteLine("1. Quản lý người dùng");
                    Console.WriteLine("2. Quản lý giải đấu");
                    Console.WriteLine("3. Quản lý đội");
                    Console.WriteLine("4. Thống kê");
                    Console.WriteLine("0. Đăng xuất");
                    Console.WriteLine();

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 4);

                    switch (choice)
                    {
                        case 1:
                            await ManageUsersAsync();
                            break;
                        case 2:
                            await ManageTournamentsAsync();
                            break;
                        case 3:
                            await ManageTeamsAsync();
                            break;
                        case 4:
                            await ShowStatisticsAsync();
                            break;
                        case 0:
                            if (ConsoleInput.GetConfirmation("Bạn có chắc muốn đăng xuất không?"))
                            {
                                ConsoleHelper.ShowInfo("Đăng xuất thành công!");
                                return;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in admin menu");
                    ConsoleHelper.ShowError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }

        private async Task ManageUsersAsync()
        {
            ConsoleHelper.ShowHeader("Quản lý người dùng");

            try
            {
                var usersResult = await _userService.GetAllUsersAsync();

                if (!usersResult.IsSuccess || usersResult.Data == null)
                {
                    ConsoleHelper.ShowError("Không thể tải danh sách người dùng.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var users = usersResult.Data;

                Console.WriteLine($"{"ID",-5} {"Username",-20} {"Email",-30} {"Role",-10} {"Status",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id,-5} {user.Username,-20} {user.Email ?? "N/A",-30} {user.Role,-10} {user.Status,-10}");
                }

                Console.WriteLine();
                Console.WriteLine("1. Thêm người dùng mới");
                Console.WriteLine("2. Chỉnh sửa người dùng");
                Console.WriteLine("3. Vô hiệu hóa người dùng");
                Console.WriteLine("4. Kích hoạt người dùng");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 4);

                switch (choice)
                {
                    case 1:
                        await AddUserAsync();
                        break;
                    case 2:
                        await EditUserAsync();
                        break;
                    case 3:
                        await DeactivateUserAsync();
                        break;
                    case 4:
                        await ActivateUserAsync();
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing users");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi quản lý người dùng.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task AddUserAsync()
        {
            ConsoleHelper.ShowHeader("Thêm người dùng mới");

            try
            {
                var username = ConsoleInput.GetUsername("Tên đăng nhập");
                var email = ConsoleInput.GetEmail("Email (không bắt buộc)", false);
                var password = ConsoleInput.GetPassword("Mật khẩu");
                var confirmPassword = ConsoleInput.GetPassword("Xác nhận mật khẩu");
                var role = ConsoleInput.GetString("Vai trò (Admin/Player/Viewer)", "Viewer");

                if (!new[] { "Admin", "Player", "Viewer" }.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    ConsoleHelper.ShowError("Vai trò không hợp lệ. Sử dụng Admin, Player hoặc Viewer.");
                    return;
                }

                var registerDto = new EsportsManager.BL.DTOs.RegisterDto
                {
                    Username = username,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    Role = role
                };

                var result = await _userService.RegisterAsync(registerDto);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Tạo tài khoản thành công!");
                }
                else
                {
                    if (result.Errors.Any())
                    {
                        ConsoleHelper.ShowErrors(result.Errors);
                    }
                    else
                    {
                        ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể tạo tài khoản.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi thêm người dùng.");
            }
        }

        private async Task EditUserAsync()
        {
            var userId = ConsoleInput.GetInt("Nhập ID người dùng cần chỉnh sửa", 1);

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                ConsoleHelper.ShowError($"Không tìm thấy người dùng với ID {userId}");
                return;
            }

            ConsoleHelper.ShowHeader($"Chỉnh sửa người dùng: {userResult.Data.Username}");

            try
            {
                var email = ConsoleInput.GetEmail($"Email mới (hiện tại: {userResult.Data.Email ?? "N/A"}, bỏ qua để giữ nguyên)", false);
                var role = ConsoleInput.GetString($"Vai trò mới (hiện tại: {userResult.Data.Role}, bỏ qua để giữ nguyên)", string.Empty);

                if (!string.IsNullOrWhiteSpace(role) && !new[] { "Admin", "Player", "Viewer" }.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    ConsoleHelper.ShowError("Vai trò không hợp lệ. Sử dụng Admin, Player hoặc Viewer.");
                    return;
                }

                var updateUserDto = new EsportsManager.BL.DTOs.UpdateUserDto
                {
                    Id = userId,
                    Email = string.IsNullOrWhiteSpace(email) ? userResult.Data.Email : email,
                    Role = string.IsNullOrWhiteSpace(role) ? userResult.Data.Role : role
                };

                var result = await _userService.UpdateUserAsync(updateUserDto);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Cập nhật thông tin người dùng thành công!");
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể cập nhật thông tin người dùng.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing user");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi chỉnh sửa người dùng.");
            }
        }

        private async Task DeactivateUserAsync()
        {
            var userId = ConsoleInput.GetInt("Nhập ID người dùng cần vô hiệu hóa", 1);

            if (userId == _currentUserId)
            {
                ConsoleHelper.ShowError("Không thể vô hiệu hóa tài khoản của bạn.");
                return;
            }

            var result = await _userService.DeactivateUserAsync(userId);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Đã vô hiệu hóa người dùng thành công.");
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể vô hiệu hóa người dùng.");
            }
        }

        private async Task ActivateUserAsync()
        {
            var userId = ConsoleInput.GetInt("Nhập ID người dùng cần kích hoạt", 1);

            var result = await _userService.ActivateUserAsync(userId);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Đã kích hoạt người dùng thành công.");
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể kích hoạt người dùng.");
            }
        }

        private async Task ManageTournamentsAsync()
        {
            ConsoleHelper.ShowHeader("Quản lý giải đấu");

            try
            {
                var tournamentsResult = await _tournamentService.GetAllAsync();

                if (!tournamentsResult.IsSuccess || tournamentsResult.Data == null)
                {
                    ConsoleHelper.ShowError("Không thể tải danh sách giải đấu.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var tournaments = tournamentsResult.Data;

                Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Trạng thái",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (var tournament in tournaments)
                {
                    Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.Status,-10}");
                }

                Console.WriteLine();
                Console.WriteLine("1. Thêm giải đấu mới");
                Console.WriteLine("2. Chỉnh sửa giải đấu");
                Console.WriteLine("3. Xóa giải đấu");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 3);

                switch (choice)
                {
                    case 1:
                        await AddTournamentAsync();
                        break;
                    case 2:
                        await EditTournamentAsync();
                        break;
                    case 3:
                        await DeleteTournamentAsync();
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing tournaments");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi quản lý giải đấu.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task AddTournamentAsync()
        {
            ConsoleHelper.ShowHeader("Thêm giải đấu mới");

            try
            {
                var name = ConsoleInput.GetString("Tên giải đấu", null, true);
                var description = ConsoleInput.GetString("Mô tả", null, true);
                var gameId = ConsoleInput.GetInt("ID game", 1);
                var startDate = ConsoleInput.GetDateTime("Ngày bắt đầu (dd/MM/yyyy)", "dd/MM/yyyy");
                var endDate = ConsoleInput.GetDateTime("Ngày kết thúc (dd/MM/yyyy)", "dd/MM/yyyy");
                var entryFee = ConsoleInput.GetDecimal("Phí tham gia", 0, 10000);
                var prizePool = ConsoleInput.GetDecimal("Giải thưởng", 0, 1000000);
                var maxTeams = ConsoleInput.GetInt("Số đội tối đa", 2, 64);
                var minTeamSize = ConsoleInput.GetInt("Số người tối thiểu trong đội", 1, 10);
                var maxTeamSize = ConsoleInput.GetInt("Số người tối đa trong đội", minTeamSize, 20);

                var tournament = new Tournament
                {
                    TournamentName = name,
                    Description = description,
                    GameId = gameId,
                    StartDate = startDate,
                    EndDate = endDate,
                    EntryFee = entryFee,
                    PrizePool = prizePool,
                    MaxTeams = maxTeams,
                    MinTeamSize = minTeamSize,
                    MaxTeamSize = maxTeamSize,
                    Status = "Upcoming",
                    CreatedBy = _currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _tournamentService.CreateAsync(tournament);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Tạo giải đấu thành công!");
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể tạo giải đấu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tournament");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi thêm giải đấu.");
            }
        }

        private async Task EditTournamentAsync()
        {
            var tournamentId = ConsoleInput.GetInt("Nhập ID giải đấu cần chỉnh sửa", 1);

            var tournamentResult = await _tournamentService.GetByIdAsync(tournamentId);
            if (!tournamentResult.IsSuccess || tournamentResult.Data == null)
            {
                ConsoleHelper.ShowError($"Không tìm thấy giải đấu với ID {tournamentId}");
                return;
            }

            var tournament = tournamentResult.Data;
            ConsoleHelper.ShowHeader($"Chỉnh sửa giải đấu: {tournament.TournamentName}");

            try
            {
                var name = ConsoleInput.GetString($"Tên giải đấu (hiện tại: {tournament.TournamentName}, bỏ qua để giữ nguyên)", string.Empty);
                var description = ConsoleInput.GetString($"Mô tả (hiện tại: {tournament.Description}, bỏ qua để giữ nguyên)", string.Empty);
                var startDateStr = ConsoleInput.GetString($"Ngày bắt đầu (hiện tại: {tournament.StartDate:dd/MM/yyyy}, bỏ qua để giữ nguyên)", string.Empty);
                var endDateStr = ConsoleInput.GetString($"Ngày kết thúc (hiện tại: {tournament.EndDate:dd/MM/yyyy}, bỏ qua để giữ nguyên)", string.Empty);
                var statusStr = ConsoleInput.GetString($"Trạng thái (hiện tại: {tournament.Status}, bỏ qua để giữ nguyên)", string.Empty);

                if (!string.IsNullOrWhiteSpace(name))
                    tournament.TournamentName = name;

                if (!string.IsNullOrWhiteSpace(description))
                    tournament.Description = description;

                if (!string.IsNullOrWhiteSpace(startDateStr) && DateTime.TryParseExact(startDateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var startDate))
                    tournament.StartDate = startDate;

                if (!string.IsNullOrWhiteSpace(endDateStr) && DateTime.TryParseExact(endDateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var endDate))
                    tournament.EndDate = endDate;

                if (!string.IsNullOrWhiteSpace(statusStr) && new[] { "Upcoming", "Active", "Completed", "Cancelled" }.Contains(statusStr, StringComparer.OrdinalIgnoreCase))
                    tournament.Status = statusStr;

                var result = await _tournamentService.UpdateAsync(tournament);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Cập nhật giải đấu thành công!");
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể cập nhật giải đấu.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing tournament");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi chỉnh sửa giải đấu.");
            }
        }

        private async Task DeleteTournamentAsync()
        {
            var tournamentId = ConsoleInput.GetInt("Nhập ID giải đấu cần xóa", 1);

            if (!ConsoleInput.GetConfirmation($"Bạn có chắc muốn xóa giải đấu có ID {tournamentId}?"))
            {
                return;
            }

            var result = await _tournamentService.DeleteAsync(tournamentId);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Đã xóa giải đấu thành công.");
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể xóa giải đấu.");
            }
        }

        private async Task ManageTeamsAsync()
        {
            ConsoleHelper.ShowHeader("Quản lý đội");

            try
            {
                var teamsResult = await _teamService.GetAllAsync();

                if (!teamsResult.IsSuccess || teamsResult.Data == null)
                {
                    ConsoleHelper.ShowError("Không thể tải danh sách đội.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var teams = teamsResult.Data;

                Console.WriteLine($"{"ID",-5} {"Tên đội",-30} {"ID đội trưởng",-15} {"Số thành viên",-15} {"Trạng thái",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (var team in teams)
                {
                    Console.WriteLine($"{team.TeamId,-5} {team.TeamName,-30} {team.CaptainId,-15} {team.MemberIds.Count + 1,-15} {team.Status,-10}");
                }

                Console.WriteLine();
                Console.WriteLine("1. Thêm đội mới");
                Console.WriteLine("2. Chỉnh sửa đội");
                Console.WriteLine("3. Xóa đội");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 3);

                switch (choice)
                {
                    case 1:
                        await AddTeamAsync();
                        break;
                    case 2:
                        await EditTeamAsync();
                        break;
                    case 3:
                        await DeleteTeamAsync();
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing teams");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi quản lý đội.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task AddTeamAsync()
        {
            ConsoleHelper.ShowHeader("Thêm đội mới");

            try
            {
                var name = ConsoleInput.GetString("Tên đội", null, true);
                var captainId = ConsoleInput.GetInt("ID đội trưởng", 1);

                // Verify captain exists
                var captainResult = await _userService.GetUserByIdAsync(captainId);
                if (!captainResult.IsSuccess || captainResult.Data == null)
                {
                    ConsoleHelper.ShowError($"Không tìm thấy người dùng với ID {captainId}");
                    return;
                }

                var team = new Team
                {
                    TeamName = name,
                    CaptainId = captainId,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _teamService.CreateAsync(team);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Tạo đội thành công!");
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể tạo đội.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding team");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi thêm đội.");
            }
        }

        private async Task EditTeamAsync()
        {
            var teamId = ConsoleInput.GetInt("Nhập ID đội cần chỉnh sửa", 1);

            var teamResult = await _teamService.GetByIdAsync(teamId);
            if (!teamResult.IsSuccess || teamResult.Data == null)
            {
                ConsoleHelper.ShowError($"Không tìm thấy đội với ID {teamId}");
                return;
            }

            var team = teamResult.Data;
            ConsoleHelper.ShowHeader($"Chỉnh sửa đội: {team.TeamName}");

            try
            {
                var name = ConsoleInput.GetString($"Tên đội (hiện tại: {team.TeamName}, bỏ qua để giữ nguyên)", string.Empty);
                var statusStr = ConsoleInput.GetString($"Trạng thái (hiện tại: {team.Status}, bỏ qua để giữ nguyên)", string.Empty);

                if (!string.IsNullOrWhiteSpace(name))
                    team.TeamName = name;

                if (!string.IsNullOrWhiteSpace(statusStr) && new[] { "Active", "Inactive" }.Contains(statusStr, StringComparer.OrdinalIgnoreCase))
                    team.Status = statusStr;

                var result = await _teamService.UpdateAsync(team);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Cập nhật đội thành công!");

                    // Show team members
                    Console.WriteLine("\nThành viên đội:");
                    Console.WriteLine($"Đội trưởng ID: {team.CaptainId}");

                    if (team.MemberIds.Any())
                    {
                        Console.WriteLine("Thành viên:");
                        foreach (var memberId in team.MemberIds)
                        {
                            Console.WriteLine($"  - ID: {memberId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Không có thành viên nào khác.");
                    }

                    Console.WriteLine("\n1. Thêm thành viên");
                    Console.WriteLine("2. Xóa thành viên");
                    Console.WriteLine("0. Quay lại");

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 2);

                    switch (choice)
                    {
                        case 1:
                            var addMemberId = ConsoleInput.GetInt("Nhập ID người dùng cần thêm vào đội", 1);
                            var addResult = await _teamService.AddMemberAsync(teamId, addMemberId);
                            if (addResult.IsSuccess)
                            {
                                ConsoleHelper.ShowSuccess($"Đã thêm thành viên có ID {addMemberId} vào đội thành công.");
                            }
                            else
                            {
                                ConsoleHelper.ShowError(addResult.ErrorMessage ?? "Không thể thêm thành viên vào đội.");
                            }
                            break;
                        case 2:
                            var removeMemberId = ConsoleInput.GetInt("Nhập ID người dùng cần xóa khỏi đội", 1);
                            var removeResult = await _teamService.RemoveMemberAsync(teamId, removeMemberId);
                            if (removeResult.IsSuccess)
                            {
                                ConsoleHelper.ShowSuccess($"Đã xóa thành viên có ID {removeMemberId} khỏi đội thành công.");
                            }
                            else
                            {
                                ConsoleHelper.ShowError(removeResult.ErrorMessage ?? "Không thể xóa thành viên khỏi đội.");
                            }
                            break;
                    }
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể cập nhật đội.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing team");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi chỉnh sửa đội.");
            }
        }

        private async Task DeleteTeamAsync()
        {
            var teamId = ConsoleInput.GetInt("Nhập ID đội cần xóa", 1);

            if (!ConsoleInput.GetConfirmation($"Bạn có chắc muốn xóa đội có ID {teamId}?"))
            {
                return;
            }

            var result = await _teamService.DeleteAsync(teamId);

            if (result.IsSuccess)
            {
                ConsoleHelper.ShowSuccess("Đã xóa đội thành công.");
            }
            else
            {
                ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể xóa đội.");
            }
        }

        private async Task ShowStatisticsAsync()
        {
            ConsoleHelper.ShowHeader("Thống kê hệ thống");

            try
            {
                // User statistics
                var totalUsersResult = await _userService.GetTotalUsersCountAsync();
                var activeUsersResult = await _userService.GetActiveUsersCountAsync();
                var adminCountResult = await _userService.GetUserCountByRoleAsync("Admin");
                var playerCountResult = await _userService.GetUserCountByRoleAsync("Player");
                var viewerCountResult = await _userService.GetUserCountByRoleAsync("Viewer");

                Console.WriteLine("== THỐNG KÊ NGƯỜI DÙNG ==");
                if (totalUsersResult.IsSuccess)
                {
                    Console.WriteLine($"Tổng số người dùng: {totalUsersResult.Data}");
                }

                if (activeUsersResult.IsSuccess)
                {
                    Console.WriteLine($"Số người dùng đang hoạt động: {activeUsersResult.Data}");
                }

                if (adminCountResult.IsSuccess && playerCountResult.IsSuccess && viewerCountResult.IsSuccess)
                {
                    Console.WriteLine($"Admin: {adminCountResult.Data}");
                    Console.WriteLine($"Player: {playerCountResult.Data}");
                    Console.WriteLine($"Viewer: {viewerCountResult.Data}");
                }

                // Tournament statistics
                var tournamentsResult = await _tournamentService.GetAllAsync();
                if (tournamentsResult.IsSuccess && tournamentsResult.Data != null)
                {
                    var tournaments = tournamentsResult.Data;
                    var upcomingCount = tournaments.Count(t => t.StartDate > DateTime.UtcNow);
                    var activeCount = tournaments.Count(t => t.StartDate <= DateTime.UtcNow && t.EndDate >= DateTime.UtcNow);
                    var completedCount = tournaments.Count(t => t.EndDate < DateTime.UtcNow || t.Status == "Completed");

                    Console.WriteLine("\n== THỐNG KÊ GIẢI ĐẤU ==");
                    Console.WriteLine($"Tổng số giải đấu: {tournaments.Count}");
                    Console.WriteLine($"Giải đấu sắp tới: {upcomingCount}");
                    Console.WriteLine($"Giải đấu đang diễn ra: {activeCount}");
                    Console.WriteLine($"Giải đấu đã kết thúc: {completedCount}");
                }

                // Team statistics
                var teamsResult = await _teamService.GetAllAsync();
                if (teamsResult.IsSuccess && teamsResult.Data != null)
                {
                    var teams = teamsResult.Data;
                    var activeTeams = teams.Count(t => t.Status == "Active");

                    Console.WriteLine("\n== THỐNG KÊ ĐỘI ==");
                    Console.WriteLine($"Tổng số đội: {teams.Count}");
                    Console.WriteLine($"Đội đang hoạt động: {activeTeams}");
                    Console.WriteLine($"Đội không hoạt động: {teams.Count - activeTeams}");

                    if (teams.Any())
                    {
                        var avgTeamSize = teams.Average(t => t.MemberIds.Count + 1); // +1 for captain
                        Console.WriteLine($"Số thành viên trung bình mỗi đội: {avgTeamSize:F1}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing system statistics");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi hiển thị thống kê hệ thống.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
}
