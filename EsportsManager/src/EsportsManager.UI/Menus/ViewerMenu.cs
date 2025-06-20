using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Models;
using EsportsManager.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EsportsManager.UI.Menus
{
    public class ViewerMenu
    {
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IDonationService _donationService;
        private readonly IWalletService _walletService;
        private readonly IFeedbackService _feedbackService;
        private readonly IVoteService _voteService;
        private readonly ILogger<ViewerMenu> _logger;
        private readonly int _currentUserId;
        private readonly string _currentUsername;

        public ViewerMenu(
            IUserService userService,
            ITournamentService tournamentService,
            ITeamService teamService,
            IDonationService donationService,
            IWalletService walletService,
            IFeedbackService feedbackService,
            IVoteService voteService,
            ILogger<ViewerMenu> logger,
            int currentUserId,
            string currentUsername)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _donationService = donationService ?? throw new ArgumentNullException(nameof(donationService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _voteService = voteService ?? throw new ArgumentNullException(nameof(voteService));
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
                    ConsoleHelper.ShowHeader($"MENU VIEWER");

                    Console.WriteLine("1. Xem giải đấu");
                    Console.WriteLine("2. Donate cho đội/giải đấu");
                    Console.WriteLine("3. Gửi phản hồi");
                    Console.WriteLine("4. Quản lý ví điện tử");
                    Console.WriteLine("5. Bình chọn (Vote)");
                    Console.WriteLine("0. Đăng xuất");
                    Console.WriteLine();

                    var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 5);

                    switch (choice)
                    {
                        case 1:
                            await ViewTournamentsAsync();
                            break;
                        case 2:
                            await DonateTournamentAsync();
                            break;
                        case 3:
                            await SendFeedbackAsync();
                            break;
                        case 4:
                            await ManageWalletAsync();
                            break;
                        case 5:
                            await VoteAsync();
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
                    _logger.LogError(ex, "Error in viewer menu");
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
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 85));

                    foreach (var tournament in upcomingResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.PrizePool:C2,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có giải đấu nào sắp diễn ra.");
                }

                Console.WriteLine("\n== GIẢI ĐẤU ĐANG DIỄN RA ==");
                if (activeResult.IsSuccess && activeResult.Data != null && activeResult.Data.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 85));

                    foreach (var tournament in activeResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.PrizePool:C2,-15}");
                    }
                }
                else
                {
                    Console.WriteLine("Không có giải đấu nào đang diễn ra.");
                }

                Console.WriteLine("\n== GIẢI ĐẤU ĐÃ KẾT THÚC ==");
                if (completedResult.IsSuccess && completedResult.Data != null && completedResult.Data.Any())
                {
                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-30} {"Ngày bắt đầu",-15} {"Ngày kết thúc",-15} {"Giải thưởng",-15}");
                    Console.WriteLine(new string('-', 85));

                    foreach (var tournament in completedResult.Data.Take(5)) // Only show last 5
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-30} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EndDate:dd/MM/yyyy,-15} {tournament.PrizePool:C2,-15}");
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
            Console.WriteLine($"Trạng thái: {tournament.Status}");

            // Get participating teams
            var teamsResult = await _teamService.GetTeamsForTournamentAsync(tournamentId);
            if (teamsResult.IsSuccess && teamsResult.Data != null && teamsResult.Data.Any())
            {
                Console.WriteLine("\n== CÁC ĐỘI THAM GIA ==");
                Console.WriteLine($"{"ID",-5} {"Tên đội",-30} {"Số thành viên",-15}");
                Console.WriteLine(new string('-', 55));

                foreach (var team in teamsResult.Data)
                {
                    Console.WriteLine($"{team.TeamId,-5} {team.TeamName,-30} {team.MemberIds.Count + 1,-15}");
                }
            }
            else
            {
                Console.WriteLine("\nChưa có đội nào đăng ký tham gia giải đấu này.");
            }

            // Show actions
            Console.WriteLine("\nCác hành động:");
            Console.WriteLine("1. Donate cho giải đấu này");
            Console.WriteLine("2. Bình chọn cho giải đấu này");
            Console.WriteLine("3. Xem phản hồi về giải đấu");
            Console.WriteLine("0. Quay lại");

            var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 3);

            switch (choice)
            {
                case 1:
                    await DonateToTournamentAsync(tournament);
                    break;
                case 2:
                    await VoteForTournamentAsync(tournament);
                    break;
                case 3:
                    await ViewTournamentFeedbackAsync(tournament.TournamentId);
                    break;
            }
        }

        private async Task DonateToTournamentAsync(Tournament tournament)
        {
            // Check wallet first
            var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);
            if (!balanceResult.IsSuccess)
            {
                ConsoleHelper.ShowError("Bạn cần tạo ví trước khi donate. Vui lòng vào menu Quản lý ví điện tử.");
                return;
            }

            decimal balance = balanceResult.Data;
            if (balance <= 0)
            {
                ConsoleHelper.ShowError($"Số dư ví của bạn là {balance:C2}. Vui lòng nạp tiền trước khi donate.");
                return;
            }

            ConsoleHelper.ShowHeader($"Donate cho giải đấu: {tournament.TournamentName}");
            Console.WriteLine($"Số dư hiện tại: {balance:C2}");

            var amount = ConsoleInput.GetDecimal("Số tiền muốn donate", 1, balance);
            var message = ConsoleInput.GetString("Tin nhắn (không bắt buộc)", string.Empty);

            if (!ConsoleInput.GetConfirmation($"Xác nhận donate {amount:C2} cho giải đấu '{tournament.TournamentName}'?"))
            {
                ConsoleHelper.ShowInfo("Đã hủy donate.");
                return;
            }

            // Create donation
            var donation = new Donation
            {
                UserId = _currentUserId,
                Amount = amount,
                RecipientType = "Tournament",
                RecipientId = tournament.TournamentId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            // Process donation
            var donationResult = await _donationService.CreateAsync(donation);
            if (donationResult.IsSuccess)
            {
                // Withdraw from wallet
                var withdrawResult = await _walletService.WithdrawAsync(_currentUserId, amount, $"Donate cho giải đấu {tournament.TournamentName}");

                if (withdrawResult.IsSuccess)
                {
                    ConsoleHelper.ShowSuccess($"Donate thành công! Số dư mới: {withdrawResult.Data:C2}");
                }
                else
                {
                    ConsoleHelper.ShowError("Có lỗi khi cập nhật ví. Vui lòng kiểm tra lại.");
                }
            }
            else
            {
                ConsoleHelper.ShowError(donationResult.ErrorMessage ?? "Không thể thực hiện donate.");
            }
        }

        private async Task DonateTournamentAsync()
        {
            ConsoleHelper.ShowHeader("Donate cho đội/giải đấu");

            try
            {
                Console.WriteLine("Bạn muốn donate cho:");
                Console.WriteLine("1. Giải đấu");
                Console.WriteLine("2. Đội");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 2);

                if (choice == 0)
                {
                    return;
                }

                // Check wallet first
                var balanceResult = await _walletService.GetBalanceAsync(_currentUserId);
                if (!balanceResult.IsSuccess)
                {
                    ConsoleHelper.ShowError("Bạn cần tạo ví trước khi donate. Vui lòng vào menu Quản lý ví điện tử.");
                    return;
                }

                decimal balance = balanceResult.Data;
                if (balance <= 0)
                {
                    ConsoleHelper.ShowError($"Số dư ví của bạn là {balance:C2}. Vui lòng nạp tiền trước khi donate.");
                    return;
                }

                if (choice == 1) // Donate to tournament
                {
                    var tournamentsResult = await _tournamentService.GetAllAsync();
                    if (!tournamentsResult.IsSuccess || tournamentsResult.Data == null || !tournamentsResult.Data.Any())
                    {
                        ConsoleHelper.ShowError("Không có giải đấu nào.");
                        return;
                    }

                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-50} {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 70));

                    foreach (var tournament in tournamentsResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-50} {tournament.Status,-10}");
                    }

                    int tournamentId = ConsoleInput.GetInt("Nhập ID giải đấu muốn donate", 1);

                    var tournamentResult = await _tournamentService.GetByIdAsync(tournamentId);
                    if (!tournamentResult.IsSuccess || tournamentResult.Data == null)
                    {
                        ConsoleHelper.ShowError($"Không tìm thấy giải đấu với ID {tournamentId}");
                        return;
                    }

                    await DonateToTournamentAsync(tournamentResult.Data);
                }
                else // Donate to team
                {
                    var teamsResult = await _teamService.GetAllAsync();
                    if (!teamsResult.IsSuccess || teamsResult.Data == null || !teamsResult.Data.Any())
                    {
                        ConsoleHelper.ShowError("Không có đội nào.");
                        return;
                    }

                    Console.WriteLine($"{"ID",-5} {"Tên đội",-50} {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 70));

                    foreach (var team in teamsResult.Data)
                    {
                        Console.WriteLine($"{team.TeamId,-5} {team.TeamName,-50} {team.Status,-10}");
                    }

                    int teamId = ConsoleInput.GetInt("Nhập ID đội muốn donate", 1);

                    var teamResult = await _teamService.GetByIdAsync(teamId);
                    if (!teamResult.IsSuccess || teamResult.Data == null)
                    {
                        ConsoleHelper.ShowError($"Không tìm thấy đội với ID {teamId}");
                        return;
                    }
                    var teamDetails = teamResult.Data;
                    Console.WriteLine($"Số dư hiện tại: {balance:C2}");

                    var amount = ConsoleInput.GetDecimal("Số tiền muốn donate", 1, balance);
                    var message = ConsoleInput.GetString("Tin nhắn (không bắt buộc)", string.Empty); if (!ConsoleInput.GetConfirmation($"Xác nhận donate {amount:C2} cho đội '{teamDetails.TeamName}'?"))
                    {
                        ConsoleHelper.ShowInfo("Đã hủy donate.");
                        return;
                    }

                    // Create donation
                    var donation = new Donation
                    {
                        UserId = _currentUserId,
                        Amount = amount,
                        RecipientType = "Team",
                        RecipientId = teamDetails.TeamId,
                        Message = message,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Process donation
                    var donationResult = await _donationService.CreateAsync(donation);
                    if (donationResult.IsSuccess)
                    {                        // Withdraw from wallet
                        var withdrawResult = await _walletService.WithdrawAsync(_currentUserId, amount, $"Donate cho đội {teamDetails.TeamName}");

                        if (withdrawResult.IsSuccess)
                        {
                            ConsoleHelper.ShowSuccess($"Donate thành công! Số dư mới: {withdrawResult.Data:C2}");
                        }
                        else
                        {
                            ConsoleHelper.ShowError("Có lỗi khi cập nhật ví. Vui lòng kiểm tra lại.");
                        }
                    }
                    else
                    {
                        ConsoleHelper.ShowError(donationResult.ErrorMessage ?? "Không thể thực hiện donate.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error donating");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi thực hiện donate.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
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

        private async Task ViewTournamentFeedbackAsync(int tournamentId)
        {
            // In a real app, we'd have a method to get feedback for a specific tournament
            var feedbackResult = await _feedbackService.GetAllAsync();

            if (!feedbackResult.IsSuccess || feedbackResult.Data == null)
            {
                ConsoleHelper.ShowError("Không thể tải phản hồi.");
                return;
            }

            var tournamentFeedback = feedbackResult.Data
                .Where(f => f.TournamentId == tournamentId)
                .ToList();

            ConsoleHelper.ShowHeader("Phản hồi về giải đấu");

            if (tournamentFeedback.Any())
            {
                Console.WriteLine($"{"User ID",-10} {"Đánh giá",-10} {"Thời gian",-20} {"Nhận xét",-50}");
                Console.WriteLine(new string('-', 90));

                foreach (var feedback in tournamentFeedback)
                {
                    var ratingStars = new string('★', feedback.Rating) + new string('☆', 5 - feedback.Rating);
                    Console.WriteLine($"{feedback.UserId,-10} {ratingStars,-10} {feedback.CreatedAt:dd/MM/yyyy HH:mm,-20} {feedback.Comment,-50}");
                }

                var avgRating = tournamentFeedback.Average(f => f.Rating);
                Console.WriteLine($"\nĐánh giá trung bình: {avgRating:F1} sao");
            }
            else
            {
                Console.WriteLine("Chưa có phản hồi nào cho giải đấu này.");
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

        private async Task VoteAsync()
        {
            ConsoleHelper.ShowHeader("Bình chọn (Vote)");

            try
            {
                Console.WriteLine("Bạn muốn bình chọn cho:");
                Console.WriteLine("1. Giải đấu");
                Console.WriteLine("2. Đội");
                Console.WriteLine("0. Quay lại");

                var choice = ConsoleInput.GetChoice("Nhập lựa chọn của bạn", 0, 2);

                if (choice == 0)
                {
                    return;
                }

                if (choice == 1) // Vote for tournament
                {
                    var tournamentsResult = await _tournamentService.GetAllAsync();
                    if (!tournamentsResult.IsSuccess || tournamentsResult.Data == null || !tournamentsResult.Data.Any())
                    {
                        ConsoleHelper.ShowError("Không có giải đấu nào.");
                        return;
                    }

                    Console.WriteLine($"{"ID",-5} {"Tên giải đấu",-50} {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 70));

                    foreach (var tournament in tournamentsResult.Data)
                    {
                        Console.WriteLine($"{tournament.TournamentId,-5} {tournament.TournamentName,-50} {tournament.Status,-10}");
                    }

                    int tournamentId = ConsoleInput.GetInt("Nhập ID giải đấu muốn bình chọn", 1);

                    var tournamentResult = await _tournamentService.GetByIdAsync(tournamentId);
                    if (!tournamentResult.IsSuccess || tournamentResult.Data == null)
                    {
                        ConsoleHelper.ShowError($"Không tìm thấy giải đấu với ID {tournamentId}");
                        return;
                    }

                    await VoteForTournamentAsync(tournamentResult.Data);
                }
                else // Vote for team
                {
                    var teamsResult = await _teamService.GetAllAsync();
                    if (!teamsResult.IsSuccess || teamsResult.Data == null || !teamsResult.Data.Any())
                    {
                        ConsoleHelper.ShowError("Không có đội nào.");
                        return;
                    }

                    Console.WriteLine($"{"ID",-5} {"Tên đội",-50} {"Trạng thái",-10}");
                    Console.WriteLine(new string('-', 70));

                    foreach (var team in teamsResult.Data)
                    {
                        Console.WriteLine($"{team.TeamId,-5} {team.TeamName,-50} {team.Status,-10}");
                    }

                    int teamId = ConsoleInput.GetInt("Nhập ID đội muốn bình chọn", 1);

                    var teamResult = await _teamService.GetByIdAsync(teamId);
                    if (!teamResult.IsSuccess || teamResult.Data == null)
                    {
                        ConsoleHelper.ShowError($"Không tìm thấy đội với ID {teamId}");
                        return;
                    }

                    var teamDetails = teamResult.Data;

                    // Check if already voted
                    var hasVotedResult = await _voteService.HasUserVotedForEntityAsync(_currentUserId, "Team", teamId);
                    if (hasVotedResult.IsSuccess && hasVotedResult.Data)
                    {
                        ConsoleHelper.ShowError("Bạn đã bình chọn cho đội này rồi.");
                        return;
                    }

                    Console.WriteLine("1. Upvote (Thích)");
                    Console.WriteLine("2. Downvote (Không thích)");
                    var voteChoice = ConsoleInput.GetChoice("Chọn loại vote", 1, 2);
                    var isUpvote = voteChoice == 1;

                    var comment = ConsoleInput.GetString("Nhận xét (không bắt buộc)", string.Empty);

                    var vote = new Vote
                    {
                        UserId = _currentUserId,
                        EntityType = "Team",
                        EntityId = teamDetails.TeamId,
                        IsUpvote = isUpvote,
                        CreatedAt = DateTime.UtcNow,
                        Comment = comment
                    };

                    var voteResult = await _voteService.CreateAsync(vote);

                    if (voteResult.IsSuccess)
                    {
                        ConsoleHelper.ShowSuccess($"Đã bình chọn thành công cho đội {teamDetails.TeamName}!");
                    }
                    else
                    {
                        ConsoleHelper.ShowError(voteResult.ErrorMessage ?? "Không thể bình chọn.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error voting");
                ConsoleHelper.ShowError("Đã xảy ra lỗi khi bình chọn.");
            }

            ConsoleHelper.PressAnyKeyToContinue();
        }

        private async Task VoteForTournamentAsync(Tournament tournament)
        {
            // Check if already voted
            var hasVotedResult = await _voteService.HasUserVotedForEntityAsync(_currentUserId, "Tournament", tournament.TournamentId);
            if (hasVotedResult.IsSuccess && hasVotedResult.Data)
            {
                ConsoleHelper.ShowError("Bạn đã bình chọn cho giải đấu này rồi.");
                return;
            }

            Console.WriteLine("1. Upvote (Thích)");
            Console.WriteLine("2. Downvote (Không thích)");
            var voteChoice = ConsoleInput.GetChoice("Chọn loại vote", 1, 2);
            var isUpvote = voteChoice == 1;

            var comment = ConsoleInput.GetString("Nhận xét (không bắt buộc)", string.Empty);

            var vote = new Vote
            {
                UserId = _currentUserId,
                EntityType = "Tournament",
                EntityId = tournament.TournamentId,
                IsUpvote = isUpvote,
                CreatedAt = DateTime.UtcNow,
                Comment = comment
            };

            var voteResult = await _voteService.CreateAsync(vote);

            if (voteResult.IsSuccess)
            {
                ConsoleHelper.ShowSuccess($"Đã bình chọn thành công cho giải đấu {tournament.TournamentName}!");
            }
            else
            {
                ConsoleHelper.ShowError(voteResult.ErrorMessage ?? "Không thể bình chọn.");
            }
        }
    }
}
