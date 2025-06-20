using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace EsportsManager.UI.Menus
{
    public class PlayerMenu
    {
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IAchievementService _achievementService;
        private readonly IWalletService _walletService;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<PlayerMenu> _logger;
        private readonly int _currentUserId;
        private readonly string _currentUsername;

        public PlayerMenu(
            IUserService userService,
            ITournamentService tournamentService,
            ITeamService teamService,
            IAchievementService achievementService,
            IWalletService walletService,
            IFeedbackService feedbackService,
            ILogger<PlayerMenu> logger,
            int currentUserId,
            string currentUsername)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
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
                    ConsoleHelper.ShowHeader($"MENU PLAYER");

                    Console.WriteLine("1. Xem giải đấu");
                    Console.WriteLine("2. Đăng ký tham gia");
                    Console.WriteLine("3. Quản lý đội của tôi");
                    Console.WriteLine("4. Xem thành tích");
                    Console.WriteLine("5. Quản lý ví");
                    Console.WriteLine("6. Gửi phản hồi");
                    Console.WriteLine("0. Đăng xuất");
                    Console.WriteLine();

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 6);

                    switch (choice)
                    {
                        case 1:
                            await ViewTournamentsAsync();
                            break;
                        case 2:
                            await RegisterForTournamentAsync();
                            break;
                        case 3:
                            await ManageMyTeamAsync();
                            break;
                        case 4:
                            await ViewAchievementsAsync();
                            break;
                        case 5:
                            await ManageWalletAsync();
                            break;
                        case 6:
                            await SendFeedbackAsync();
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
                    _logger.LogError(ex, "Error in player menu");
                    ConsoleHelper.ShowError("Đã xảy ra lỗi. Vui lòng thử lại.");
                    ConsoleHelper.PressAnyKeyToContinue();
                }
            }
        }

        private async Task ViewTournamentsAsync()
        {
            ConsoleHelper.ShowHeader("Xem giải đấu");

            try
            {
                var upcomingResult = await _tournamentService.GetUpcomingTournamentsAsync();
                var activeResult = await _tournamentService.GetActiveTournamentsAsync();
                var completedResult = await _tournamentService.GetCompletedTournamentsAsync();

                Console.WriteLine("== GIẢI ĐẤU SẮP DIỄN RA ==");
                if (upcomingResult.IsSuccess && upcomingResult.Data != null && upcomingResult.Data.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Phí tham gia",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 100));

                    foreach (var tournament in upcomingResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.EntryFee:C2,-15} {tournament.PrizePool:C2,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có giải đấu nào sắp diễn ra.");
                }

                Console.WriteLine("\n== GIẢI ĐẤU ĐANG DIỄN RA ==");
                if (activeResult.IsSuccess && activeResult.Data != null && activeResult.Data.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Phí tham gia",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 100));

                    foreach (var tournament in activeResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.EntryFee:C2,-15} {tournament.PrizePool:C2,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có giải đấu nào đang diễn ra.");
                }

                Console.WriteLine("\n== GIẢI ĐẤU ĐÃ KẾT THÚC ==");
                if (completedResult.IsSuccess && completedResult.Data != null && completedResult.Data.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Phí tham gia",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 100));

                    foreach (var tournament in completedResult.Data.Take(5)) // Only show last 5
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.EntryFee:C2,-15} {tournament.PrizePool:C2,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có giải đấu nào đã kết thúc.");
                }

                Console.WriteLine("\nNhập ID giải đấu để xem chi tiết (0 để quay lại): ");
                var tournamentId = ConsoleInput.GetInt("", 0);

                if (tournamentId > 0)
                {
                    await ViewTournamentDetailsAsync(tournamentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing tournaments");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi xem giải đấu.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task ViewTournamentDetailsAsync(int tournamentId)
        {
            var tournamentResult = await _tournamentService.GetByIdAsync(tournamentId);
            if (!tournamentResult.IsSuccess || tournamentResult.Data == null)
            {
                ConsoleHelper.ShowError($"Không tìm thấy giải đấu với ID {tournamentId}");
                return;
            }

            var tournament = tournamentResult.Data;
            ConsoleHelper.ShowHeader($"Chi tiết giải đấu: {tournament.TournamentName}");

            Console.WriteLine($"ID: {tournament.TournamentId}");
            Console.WriteLine($"Tên: {tournament.TournamentName}");
            Console.WriteLine($"Mô tả: {tournament.Description}");
            Console.WriteLine($"Game ID: {tournament.GameId}");
            Console.WriteLine($"Ngày bắt đầu: {tournament.StartDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Ngày kết thúc: {tournament.EndDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Phí tham gia: {tournament.EntryFee:C2}");
            Console.WriteLine($"Giải thưởng: {tournament.PrizePool:C2}");
            Console.WriteLine($"Số đội tối đa: {tournament.MaxTeams}");
            Console.WriteLine($"Kích thước đội: {tournament.MinTeamSize}-{tournament.MaxTeamSize} thành viên");
            Console.WriteLine($"Trạng thái: {tournament.Status}");
            Console.WriteLine($"Được tạo bởi: User ID {tournament.CreatedBy}");
            Console.WriteLine($"Được tạo lúc: {tournament.CreatedAt:dd/MM/yyyy HH:mm}");

            // Get participating teams
            var teamsResult = await _teamService.GetTeamsForTournamentAsync(tournamentId);
            if (teamsResult.IsSuccess && teamsResult.Data != null && teamsResult.Data.Any())
            {
                Console.WriteLine("\n== CÁC ĐỘI THAM GIA ==");
                Console.WriteLine($"{"ID",-5} {"Tên đội",-30} {"Đội trưởng ID",-15} {"Số thành viên",-15}");
                Console.WriteLine(new string('-', 70));

                foreach (var team in teamsResult.Data)
                {
                    Console.WriteLine($"{team.TeamId,-5} {team.TeamName,-30} {team.CaptainId,-15} {team.MemberIds.Count + 1,-15}");
                }
            }
            else
            {
                Console.WriteLine("\nChưa có đội nào đăng ký tham gia giải đấu này.");
            }
        }

        private async Task RegisterForTournamentAsync()
        {
            ConsoleHelper.ShowHeader("Đăng ký tham gia giải đấu");

            try
            {
                // Check if user has a team
                var teamResult = await _teamService.GetTeamByUserIdAsync(_currentUserId);
                if (!teamResult.IsSuccess)
                {
                    ConsoleHelper.ShowError("Bạn cần phải có một đội để đăng ký tham gia giải đấu.");
                    ConsoleHelper.ShowInfo("Hãy tạo đội mới từ menu 'Quản lý đội của tôi'.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var team = teamResult.Data!;

                // Check if user is team captain
                var isCaptainResult = await _teamService.IsUserTeamCaptainAsync(_currentUserId);
                if (!isCaptainResult.IsSuccess || !isCaptainResult.Data)
                {
                    ConsoleHelper.ShowError("Chỉ đội trưởng mới có thể đăng ký đội tham gia giải đấu.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                // Get upcoming tournaments
                var upcomingResult = await _tournamentService.GetUpcomingTournamentsAsync();
                if (!upcomingResult.IsSuccess || upcomingResult.Data == null || !upcomingResult.Data.Any())
                {
                    ConsoleHelper.ShowError("Không có giải đấu nào sắp diễn ra để đăng ký.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                Console.WriteLine("== GIẢI ĐẤU CÓ THỂ ĐĂNG KÝ ==");
                Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Phí tham gia",-15} {"Giải thưởng",-15}");
                Console.WriteLine(new string('-', 85));

                foreach (var tournament in upcomingResult.Data)
                {
                    Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EntryFee:C2,-15} {tournament.PrizePool:C2,-15}");
                }

                var tournamentId = ConsoleInput.GetInt("\nNhập ID giải đấu muốn đăng ký (0 để hủy)", 0);
                if (tournamentId == 0)
                {
                    return;
                }

                // Get tournament details
                var tournamentResult = await _tournamentService.GetByIdAsync(tournamentId);
                if (!tournamentResult.IsSuccess || tournamentResult.Data == null)
                {
                    ConsoleHelper.ShowError($"Không tìm thấy giải đấu với ID {tournamentId}");
                    return;
                }

                var tournamentDetails = tournamentResult.Data;                // Verify team size requirements
                if (team.MemberIds.Count + 1 < tournamentDetails.MinTeamSize || team.MemberIds.Count + 1 > tournamentDetails.MaxTeamSize)
                {
                    ConsoleHelper.ShowError($"Đội của bạn có {team.MemberIds.Count + 1} thành viên, nhưng giải đấu này yêu cầu {tournamentDetails.MinTeamSize}-{tournamentDetails.MaxTeamSize} thành viên.");
                    return;
                }

                // Check user wallet for entry fee
                var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);
                if (!balanceResult.IsSuccess)
                {
                    ConsoleHelper.ShowError("Không thể kiểm tra số dư ví. Hãy đảm bảo bạn đã tạo ví.");
                    return;
                }
                if (balanceResult.Data < tournamentDetails.EntryFee)
                {
                    ConsoleHelper.ShowError($"Số dư ví không đủ. Bạn cần {tournamentDetails.EntryFee:C2} để tham gia, nhưng hiện có {balanceResult.Data:C2}.");
                    return;
                }

                // Confirm registration
                if (!ConsoleInput.GetConfirmation($"Xác nhận đăng ký đội '{team.TeamName}' cho giải đấu '{tournamentDetails.TournamentName}'? Phí tham gia: {tournamentDetails.EntryFee:C2}"))
                {
                    ConsoleHelper.ShowInfo("Đã hủy đăng ký.");
                    return;
                }

                // In a real application, this would deduct the entry fee and register the team                ConsoleHelper.ShowSuccess("Đăng ký tham gia giải đấu thành công!");
                ConsoleHelper.ShowInfo($"Phí tham gia {tournamentDetails.EntryFee:C2} đã được trừ từ ví của bạn.");

                // For demo, we'll just do this
                var withdrawResult = await _walletService.WithdrawAsync(_currentUserId, tournamentDetails.EntryFee, $"Phí tham gia giải đấu {tournamentDetails.TournamentName}");
                if (withdrawResult.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Ví của bạn đã được cập nhật. Số dư mới: {withdrawResult.Data:C2}");
                }
                else
                {
                    ConsoleHelper.ShowError("Có lỗi khi cập nhật ví. Vui lòng kiểm tra lại sau.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering for tournament");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi đăng ký giải đấu.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task ManageMyTeamAsync()
        {
            ConsoleHelper.ShowHeader("Quản lý đội của tôi");

            try
            {
                // Check if user has a team
                var teamResult = await _teamService.GetTeamByUserIdAsync(_currentUserId);
                var isCaptainResult = await _teamService.IsUserTeamCaptainAsync(_currentUserId);

                if (!teamResult.IsSuccess || teamResult.Data == null)
                {
                    Console.WriteLine("Bạn chưa có đội. Bạn muốn tạo đội mới?");
                    if (ConsoleInput.GetConfirmation("Tạo đội mới?"))
                    {
                        await CreateNewTeamAsync();
                    }
                    return;
                }

                var team = teamResult.Data;
                var isCaptain = isCaptainResult.IsSuccess && isCaptainResult.Data;

                Console.WriteLine($"Tên đội: {team.TeamName}");
                Console.WriteLine($"Đội trưởng ID: {team.CaptainId}" + (team.CaptainId == _currentUserId ? " (Bạn)" : ""));
                Console.WriteLine($"Trạng thái: {team.Status}");
                Console.WriteLine($"Ngày tạo: {team.CreatedAt:dd/MM/yyyy}");

                Console.WriteLine("\nThành viên đội:");
                if (team.MemberIds.Any())
                {
                    foreach (var memberId in team.MemberIds)
                    {
                        // In a real app, we'd get the user's name
                        Console.WriteLine($"- Thành viên ID: {memberId}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có thành viên nào (ngoài đội trưởng).");
                }

                if (isCaptain)
                {
                    Console.WriteLine("\nTư cách đội trưởng, bạn có thể:");
                    Console.WriteLine("1. Thêm thành viên");
                    Console.WriteLine("2. Xóa thành viên");
                    Console.WriteLine("3. Đổi tên đội");
                    Console.WriteLine("0. Quay lại");

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 3);

                    switch (choice)
                    {
                        case 1:
                            var addMemberId = ConsoleInput.GetInt("Nhập ID người dùng cần thêm vào đội", 1);
                            var addResult = await _teamService.AddMemberAsync(team.TeamId, addMemberId);
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
                            var removeResult = await _teamService.RemoveMemberAsync(team.TeamId, removeMemberId);
                            if (removeResult.IsSuccess)
                            {
                                ConsoleHelper.ShowSuccess($"Đã xóa thành viên có ID {removeMemberId} khỏi đội thành công.");
                            }
                            else
                            {
                                ConsoleHelper.ShowError(removeResult.ErrorMessage ?? "Không thể xóa thành viên khỏi đội.");
                            }
                            break;
                        case 3:
                            var newName = ConsoleInput.GetString("Nhập tên mới cho đội", null, true);
                            team.TeamName = newName;
                            var updateResult = await _teamService.UpdateAsync(team);
                            if (updateResult.IsSuccess)
                            {
                                ConsoleHelper.ShowSuccess($"Đã đổi tên đội thành '{newName}' thành công.");
                            }
                            else
                            {
                                ConsoleHelper.ShowError(updateResult.ErrorMessage ?? "Không thể đổi tên đội.");
                            }
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nBạn có thể:");
                    Console.WriteLine("1. Rời đội");
                    Console.WriteLine("0. Quay lại");

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 1);

                    if (choice == 1)
                    {
                        if (ConsoleInput.GetConfirmation("Bạn có chắc muốn rời đội không?"))
                        {
                            // In a real app, we'd have a method for this
                            var removeResult = await _teamService.RemoveMemberAsync(team.TeamId, _currentUserId);
                            if (removeResult.IsSuccess)
                            {
                                ConsoleHelper.ShowSuccess("Bạn đã rời đội thành công.");
                            }
                            else
                            {
                                ConsoleHelper.ShowError(removeResult.ErrorMessage ?? "Không thể rời đội.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing team");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi quản lý đội.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task CreateNewTeamAsync()
        {
            ConsoleHelper.ShowHeader("Tạo đội mới");

            try
            {
                var name = ConsoleInput.GetString("Nhập tên đội", null, true);

                var team = new Team
                {
                    TeamName = name,
                    CaptainId = _currentUserId,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _teamService.CreateAsync(team);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Tạo đội thành công! Bạn là đội trưởng.");

                    // Let's add an achievement for creating a team
                    var achievement = new Achievement
                    {
                        UserId = _currentUserId,
                        Title = "Team Captain",
                        Description = "Created your first team",
                        AchievementDate = DateTime.UtcNow
                    };

                    await _achievementService.CreateAsync(achievement);
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể tạo đội.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi tạo đội.");
            }
        }

        private async Task ViewAchievementsAsync()
        {
            ConsoleHelper.ShowHeader("Thành tích của tôi");

            try
            {
                var achievementsResult = await _achievementService.GetAchievementsByUserIdAsync(_currentUserId);

                if (!achievementsResult.IsSuccess || achievementsResult.Data == null)
                {
                    ConsoleHelper.ShowError("Không thể tải thành tích.");
                    ConsoleHelper.PressAnyKeyToContinue();
                    return;
                }

                var achievements = achievementsResult.Data;

                if (achievements.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Thành tích",-30} {"Mô tả",-40} {"Ngày đạt được",-15}");
                    Console.WriteLine(new string('-', 95));

                    foreach (var achievement in achievements)
                    {
                        Console.WriteLine($"{achievement.AchievementId,-5} {achievement.Title,-30} {achievement.Description,-40} {achievement.AchievementDate:dd/MM/yyyy,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Bạn chưa có thành tích nào. Hãy tham gia các giải đấu để đạt được thành tích!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing achievements");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi xem thành tích.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task ManageWalletAsync()
        {
            try
            {
                // Create a new logger for the WalletMenu
                var walletLogger = Program.ServiceProvider.GetRequiredService<ILogger<WalletMenu>>();
                var walletMenu = new WalletMenu(_walletService, walletLogger, _currentUserId);
                await walletMenu.ShowAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error managing wallet");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi quản lý ví.");
                ConsoleHelper.PressAnyKeyToContinue();
            }
        }

        private async Task SendFeedbackAsync()
        {
            ConsoleHelper.ShowHeader("Gửi phản hồi");

            try
            {
                Console.WriteLine("Bạn muốn gửi phản hồi về?");
                Console.WriteLine("1. Giải đấu cụ thể");
                Console.WriteLine("2. Hệ thống nói chung");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 2);

                if (choice == 0)
                {
                    return;
                }

                int? tournamentId = null;
                if (choice == 1)
                {
                    var tournamentsResult = await _tournamentService.GetAllAsync();
                    if (!tournamentsResult.IsSuccess || tournamentsResult.Data == null || !tournamentsResult.Data.Any())
                    {
                        ConsoleHelper.ShowError("Không tìm thấy giải đấu nào.");
                        return;
                    }

                    Console.WriteLine("\n== GIẢI ĐẤU ==");
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-50} {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 70));

                    foreach (var tournament in tournamentsResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-50} {tournament.Status,-10}");
                    }

                    tournamentId = ConsoleInput.GetInt("Nhập ID giải đấu muốn phản hồi", 1);
                }

                var rating = ConsoleInput.GetInt("Đánh giá (1-5 sao)", 1, 5);
                var comment = ConsoleInput.GetString("Nhận xét của bạn", null, true);

                var feedback = new Feedback
                {
                    UserId = _currentUserId,
                    TournamentId = tournamentId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _feedbackService.CreateAsync(feedback);

                if (result.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess("Gửi phản hồi thành công! Cảm ơn bạn đã đóng góp ý kiến.");
                }
                else
                {
                    ConsoleHelper.ShowError(result.ErrorMessage ?? "Không thể gửi phản hồi.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending feedback");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi gửi phản hồi.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }
    }
}
