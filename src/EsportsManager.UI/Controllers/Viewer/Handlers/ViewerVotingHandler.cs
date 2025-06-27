using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.MenuHandlers;

namespace EsportsManager.UI.Controllers.Viewer.Handlers
{
    /// <summary>
    /// Handler cho chức năng voting của Viewer
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class ViewerVotingHandler : IViewerVotingHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITournamentService _tournamentService;
        private readonly IUserService _userService;
        private readonly IVotingService _votingService;

        public ViewerVotingHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            IUserService userService,
            IVotingService votingService)
        {
            _currentUser = currentUser;
            _tournamentService = tournamentService;
            _userService = userService;
            _votingService = votingService;
        }

        public async Task HandleVoteForPlayerAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO PLAYER YÊU THÍCH", 80, 15);                // Sử dụng HandlePlayerVotingAsync để xử lý việc vote cho player
                await HandlePlayerVotingAsync();

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi vote cho player: {ex.Message}", false, 3000);
            }
        }

        public async Task HandleVoteForTournamentAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO GIẢI ĐẤU HAY NHẤT", 80, 15);                // Sử dụng HandleTournamentVotingAsync để xử lý việc vote cho giải đấu
                await HandleTournamentVotingAsync();

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi vote cho giải đấu: {ex.Message}", false, 3000);
            }
        }

        public async Task HandleVoteForSportAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO MÔTHER SPORT ESPORTS", 80, 15);                // Sử dụng HandleEsportsVotingAsync để xử lý việc vote cho môn thể thao
                await HandleEsportsVotingAsync();

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi vote cho sport: {ex.Message}", false, 3000);
            }
        }

        // Keep the old method for backward compatibility during transition
        public async Task HandleVotingAsync()
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("VOTING", 80, 15);

                    var voteOptions = new[]
                    {
                        "Vote cho Player yêu thích",
                        "Vote cho Giải đấu hay nhất",
                        "Vote cho Môn thể thao esports",
                        "Xem kết quả voting",
                        "Quay lại menu chính"
                    };

                    int selection = InteractiveMenuService.DisplayInteractiveMenu("CHỌN LOẠI VOTING", voteOptions);

                    switch (selection)
                    {
                        case 0:
                            await HandlePlayerVotingAsync();
                            break;
                        case 1:
                            await HandleTournamentVotingAsync();
                            break;
                        case 2:
                            await HandleEsportsVotingAsync();
                            break;
                        case 3:
                            await HandleViewVotingResults();
                            break;
                        case 4:
                        case -1:
                            return;
                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
                }
            }
        }

        private async Task HandlePlayerVotingAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO PLAYER YÊU THÍCH", 80, 15);

                // Get real player list from database
                var playerResult = await _userService.GetUsersByRoleAsync("Player");

                if (playerResult.IsSuccess && playerResult.Data != null && playerResult.Data.Any())
                {
                    var players = playerResult.Data.ToList();

                    Console.WriteLine("👥 Chọn player để vote:");
                    for (int i = 0; i < players.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {players[i].Username}");
                    }

                    Console.Write($"\nNhập số thứ tự player (1-{players.Count}): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) &&
                        choice >= 1 && choice <= players.Count)
                    {
                        var selectedPlayer = players[choice - 1];

                        Console.WriteLine($"\nĐánh giá cho {selectedPlayer.Username}:");
                        Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                        Console.Write("Chọn số điểm (1-5): ");

                        int rating = 5; // Mặc định điểm cao nhất
                        if (int.TryParse(Console.ReadLine(), out int ratingInput) &&
                            ratingInput >= 1 && ratingInput <= 5)
                        {
                            rating = ratingInput;
                        }

                        Console.Write("Nhập nhận xét (tùy chọn): ");
                        string comment = Console.ReadLine() ?? string.Empty;

                        // Create voting object
                        var votingDto = new VotingDto
                        {
                            UserId = _currentUser.Id,
                            VoteType = "Player",
                            TargetId = selectedPlayer.Id,
                            TargetName = selectedPlayer.Username,
                            Rating = rating,
                            Comment = comment,
                            VoteDate = DateTime.Now
                        };

                        // Sử dụng IVotingService để lưu vote
                        try
                        {
                            var result = await _votingService.SubmitVoteAsync(votingDto);
                            if (result)
                            {
                                ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedPlayer.Username}!", true, 2000);
                            }
                            else
                            {
                                ConsoleRenderingService.ShowMessageBox($"❌ Không thể vote cho player.", false, 2000);
                            }
                        }
                        catch (Exception ex)
                        {
                            ConsoleRenderingService.ShowMessageBox($"❌ Lỗi khi vote: {ex.Message}", false, 2000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy Player nào trong hệ thống!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleTournamentVotingAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO GIẢI ĐẤU HAY NHẤT", 80, 15);

                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count > 0)
                {
                    Console.WriteLine("🏆 Chọn giải đấu để vote:");
                    for (int i = 0; i < tournaments.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {tournaments[i].Name}");
                    }

                    Console.Write($"\nNhập số thứ tự giải đấu (1-{tournaments.Count}): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) &&
                        choice >= 1 && choice <= tournaments.Count)
                    {
                        var selectedTournament = tournaments[choice - 1];

                        Console.WriteLine($"\nĐánh giá cho {selectedTournament.Name}:");
                        Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                        Console.Write("Chọn số điểm (1-5): ");

                        int rating = 5; // Mặc định điểm cao nhất
                        if (int.TryParse(Console.ReadLine(), out int ratingInput) &&
                            ratingInput >= 1 && ratingInput <= 5)
                        {
                            rating = ratingInput;
                        }

                        Console.Write("Nhập nhận xét (tùy chọn): ");
                        string comment = Console.ReadLine() ?? string.Empty;

                        // Tạo voting DTO
                        var votingDto = new VotingDto
                        {
                            UserId = _currentUser.Id,
                            VoteType = "Tournament",
                            TargetId = selectedTournament.TournamentId,
                            TargetName = selectedTournament.Name,
                            Rating = rating,
                            Comment = comment,
                            VoteDate = DateTime.Now
                        };

                        var result = await _votingService.SubmitVoteAsync(votingDto);
                        if (result)
                        {
                            ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedTournament.Name}!", true, 2000);
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox($"❌ Không thể vote cho giải đấu.", false, 2000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Không có giải đấu nào để vote!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleEsportsVotingAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO MÔN THỂ THAO ESPORTS", 80, 12);

                var esportsCategories = new[]
                {
                    "League of Legends",
                    "Counter-Strike: Global Offensive",
                    "Valorant",
                    "PUBG Mobile",
                    "FIFA Online 4",
                    "Mobile Legends: Bang Bang"
                };

                Console.WriteLine("🎮 Chọn môn thể thao esports yêu thích:");
                for (int i = 0; i < esportsCategories.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {esportsCategories[i]}");
                }

                Console.Write($"\nNhập số thứ tự (1-{esportsCategories.Length}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) &&
                    choice >= 1 && choice <= esportsCategories.Length)
                {
                    var selectedCategory = esportsCategories[choice - 1];

                    Console.WriteLine($"\nĐánh giá cho {selectedCategory}:");
                    Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                    Console.Write("Chọn số điểm (1-5): ");

                    int rating = 5; // Mặc định điểm cao nhất
                    if (int.TryParse(Console.ReadLine(), out int ratingInput) &&
                        ratingInput >= 1 && ratingInput <= 5)
                    {
                        rating = ratingInput;
                    }

                    Console.Write("Nhập nhận xét (tùy chọn): ");
                    string comment = Console.ReadLine() ?? string.Empty;

                    var votingDto = new VotingDto
                    {
                        UserId = _currentUser.Id,
                        VoteType = "EsportsCategory",
                        TargetId = choice, // Sử dụng index làm ID tạm thời
                        TargetName = selectedCategory,
                        Rating = rating,
                        Comment = comment,
                        VoteDate = DateTime.Now
                    };

                    var result = await _votingService.SubmitVoteAsync(votingDto);
                    if (result)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedCategory}!", true, 2000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Không thể vote cho esports category.", false, 2000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleViewVotingResults()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING", 80, 20);

                Console.WriteLine("📊 KẾT QUẢ VOTING TỔNG HỢP");
                Console.WriteLine("─".PadRight(78, '─'));

                Console.WriteLine("\n🏆 TOP 5 PLAYER YÊU THÍCH:");

                // Get actual voting results from service
                var playerResults = await _votingService.GetPlayerVotingResultsAsync(5);

                if (playerResults != null && playerResults.Count > 0)
                {
                    foreach (var result in playerResults)
                    {
                        Console.WriteLine($"  • {result.TargetName}: {result.TotalVotes} votes (⭐ {result.AverageRating:F1})");
                    }
                }
                else
                {
                    Console.WriteLine("  • Chưa có dữ liệu bình chọn");
                }

                Console.WriteLine("\n🎮 TOP 5 GIẢI ĐẤU HAY NHẤT:");

                // Get tournament voting results
                var tournamentResults = await _votingService.GetTournamentVotingResultsAsync(5);

                if (tournamentResults != null && tournamentResults.Count > 0)
                {
                    foreach (var result in tournamentResults)
                    {
                        Console.WriteLine($"  • {result.TargetName}: {result.TotalVotes} votes (⭐ {result.AverageRating:F1})");
                    }
                }
                else
                {
                    Console.WriteLine("  • Chưa có dữ liệu bình chọn cho giải đấu");
                }

                Console.WriteLine("\n🏅 TOP 3 MÔN THỂ THAO ESPORTS:");

                // Get esports category voting results
                var categoryResults = await _votingService.GetEsportsCategoryVotingResultsAsync(3);

                if (categoryResults != null && categoryResults.Count > 0)
                {
                    foreach (var result in categoryResults)
                    {
                        Console.WriteLine($"  • {result.TargetName}: {result.TotalVotes} votes (⭐ {result.AverageRating:F1})");
                    }
                }
                else
                {
                    Console.WriteLine("  • Chưa có dữ liệu bình chọn cho môn thể thao esports");
                }

                Console.WriteLine("\nNhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }
    }
}
