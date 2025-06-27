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
                ConsoleRenderingService.DrawBorder("VOTE CHO PLAYER YÊU THÍCH", 80, 15);
                await HandlePlayerVotingAsync();
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi vote cho player: {ex.Message}", false, 3000);
            }
        }

        public async Task HandleVoteForTournamentAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO GIẢI ĐẤU HAY NHẤT", 80, 15);
                await HandleTournamentVotingAsync();
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"Lỗi khi vote cho giải đấu: {ex.Message}", false, 3000);
            }
        }

        public async Task HandleVoteForSportAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO MÔTHER SPORT ESPORTS", 80, 15);
                await HandleEsportsVotingAsync();
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
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

                    var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                    int cursorY = top + 13;

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
                            Console.SetCursorPosition(left + 2, cursorY);
                            ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                    int cursorY = top + 13;
                    Console.SetCursorPosition(left + 2, cursorY);
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
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 1;

                // Get real player list from database
                var playerResult = await _userService.GetUsersByRoleAsync("Player");

                if (playerResult.IsSuccess && playerResult.Data != null && playerResult.Data.Any())
                {
                    var players = playerResult.Data.ToList();
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.WriteLine("👥 Chọn player để vote:");
                    for (int i = 0; i < players.Count; i++)
                    {
                        Console.SetCursorPosition(left + 4, cursorY++);
                        Console.WriteLine($"{i + 1}. {players[i].Username}");
                    }

                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.Write($"Nhập số thứ tự player (1-{players.Count}): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= players.Count)
                    {
                        var selectedPlayer = players[choice - 1];
                        cursorY++;
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.WriteLine($"Đánh giá cho {selectedPlayer.Username}:");
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.Write("Chọn số điểm (1-5): ");

                        int rating = 5;
                        if (int.TryParse(Console.ReadLine(), out int ratingInput) && ratingInput >= 1 && ratingInput <= 5)
                        {
                            rating = ratingInput;
                        }

                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.Write("Nhập nhận xét (tùy chọn): ");
                        string comment = Console.ReadLine() ?? string.Empty;

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
                        // Hiển thị lỗi trong border, dùng cursorY để luôn nằm trong border
                        Console.SetCursorPosition(left + 2, cursorY);
                        ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    }
                }
                else
                {
                    Console.SetCursorPosition(left + 2, cursorY);
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy Player nào trong hệ thống!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleTournamentVotingAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO GIẢI ĐẤU HAY NHẤT", 80, 15);
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 1;

                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count > 0)
                {
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.WriteLine("🏆 Chọn giải đấu để vote:");
                    for (int i = 0; i < tournaments.Count; i++)
                    {
                        Console.SetCursorPosition(left + 4, cursorY++);
                        Console.WriteLine($"{i + 1}. {tournaments[i].Name}");
                    }

                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.Write($"Nhập số thứ tự giải đấu (1-{tournaments.Count}): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
                    {
                        var selectedTournament = tournaments[choice - 1];
                        cursorY++;
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.WriteLine($"Đánh giá cho {selectedTournament.Name}:");
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.Write("Chọn số điểm (1-5): ");

                        int rating = 5;
                        if (int.TryParse(Console.ReadLine(), out int ratingInput) && ratingInput >= 1 && ratingInput <= 5)
                        {
                            rating = ratingInput;
                        }

                        Console.SetCursorPosition(left + 2, cursorY++);
                        Console.Write("Nhập nhận xét (tùy chọn): ");
                        string comment = Console.ReadLine() ?? string.Empty;

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
                        Console.SetCursorPosition(left + 2, top + 13);
                        ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                    }
                }
                else
                {
                    Console.SetCursorPosition(left + 2, top + 13);
                    ConsoleRenderingService.ShowMessageBox("Không có giải đấu nào để vote!", false, 2000);
                }
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 15);
                int cursorY = top + 13;
                Console.SetCursorPosition(left + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleEsportsVotingAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO MÔN THỂ THAO ESPORTS", 80, 12);
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 12);
                int cursorY = top + 1;

                var esportsCategories = new[]
                {
                    "League of Legends",
                    "Counter-Strike: Global Offensive",
                    "Valorant",
                    "PUBG Mobile",
                    "FIFA Online 4",
                    "Mobile Legends: Bang Bang"
                };

                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("🎮 Chọn môn thể thao esports yêu thích:");
                for (int i = 0; i < esportsCategories.Length; i++)
                {
                    Console.SetCursorPosition(left + 4, cursorY++);
                    Console.WriteLine($"{i + 1}. {esportsCategories[i]}");
                }

                Console.SetCursorPosition(left + 2, cursorY++);
                Console.Write($"Nhập số thứ tự (1-{esportsCategories.Length}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= esportsCategories.Length)
                {
                    var selectedCategory = esportsCategories[choice - 1];
                    cursorY++;
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.WriteLine($"Đánh giá cho {selectedCategory}:");
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.WriteLine("1 - ⭐ | 2 - ⭐⭐ | 3 - ⭐⭐⭐ | 4 - ⭐⭐⭐⭐ | 5 - ⭐⭐⭐⭐⭐");
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.Write("Chọn số điểm (1-5): ");

                    int rating = 5;
                    if (int.TryParse(Console.ReadLine(), out int ratingInput) && ratingInput >= 1 && ratingInput <= 5)
                    {
                        rating = ratingInput;
                    }

                    Console.SetCursorPosition(left + 2, cursorY++);
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
                    Console.SetCursorPosition(left + 2, top + 10);
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", false, 1500);
                }
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(80, 12);
                int cursorY = top + 10;
                Console.SetCursorPosition(left + 2, cursorY);
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }

        private async Task HandleViewVotingResults()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING", 80, 20);
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(80, 20);
                int cursorY = top + 1;

                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("📊 KẾT QUẢ VOTING TỔNG HỢP");
                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine(new string('─', 76));

                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("🏆 TOP 5 PLAYER YÊU THÍCH:");
                var playerResults = await _votingService.GetPlayerVotingResultsAsync(5);
                if (playerResults != null && playerResults.Count > 0)
                {
                    foreach (var result in playerResults)
                    {
                        Console.SetCursorPosition(left + 4, cursorY++);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"• {result.TargetName,-20} | {result.TotalVotes,3} votes | ⭐ {result.AverageRating:F1}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.SetCursorPosition(left + 4, cursorY++);
                    Console.WriteLine("• Chưa có dữ liệu bình chọn");
                }

                cursorY++;
                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("🎮 TOP 5 GIẢI ĐẤU HAY NHẤT:");
                var tournamentResults = await _votingService.GetTournamentVotingResultsAsync(5);
                if (tournamentResults != null && tournamentResults.Count > 0)
                {
                    foreach (var result in tournamentResults)
                    {
                        Console.SetCursorPosition(left + 4, cursorY++);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"• {result.TargetName,-20} | {result.TotalVotes,3} votes | ⭐ {result.AverageRating:F1}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.SetCursorPosition(left + 4, cursorY++);
                    Console.WriteLine("• Chưa có dữ liệu bình chọn cho giải đấu");
                }

                cursorY++;
                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("🏅 TOP 3 MÔN THỂ THAO ESPORTS:");
                var categoryResults = await _votingService.GetEsportsCategoryVotingResultsAsync(3);
                if (categoryResults != null && categoryResults.Count > 0)
                {
                    foreach (var result in categoryResults)
                    {
                        Console.SetCursorPosition(left + 4, cursorY++);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"• {result.TargetName,-20} | {result.TotalVotes,3} votes | ⭐ {result.AverageRating:F1}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.SetCursorPosition(left + 4, cursorY++);
                    Console.WriteLine("• Chưa có dữ liệu bình chọn cho môn thể thao esports");
                }

                cursorY += 2;
                Console.SetCursorPosition(left + 2, cursorY++);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"❌ Lỗi: {ex.Message}", false, 2000);
            }
        }
    }
}
