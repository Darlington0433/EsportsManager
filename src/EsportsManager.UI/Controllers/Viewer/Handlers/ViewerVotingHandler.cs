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

        public ViewerVotingHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            IUserService userService)
        {
            _currentUser = currentUser;
            _tournamentService = tournamentService;
            _userService = userService;
        }

        public async Task HandleVoteForPlayerAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("VOTE CHO PLAYER YÊU THÍCH", 80, 15);

                // Implement player voting logic here
                Console.WriteLine("🗳️ Chức năng vote cho player đang được phát triển...");
                
                await Task.Delay(100); // Placeholder async operation
                
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
                ConsoleRenderingService.DrawBorder("VOTE CHO GIẢI ĐẤU HAY NHẤT", 80, 15);

                // Implement tournament voting logic here
                Console.WriteLine("🗳️ Chức năng vote cho giải đấu đang được phát triển...");
                
                await Task.Delay(100); // Placeholder async operation
                
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
                ConsoleRenderingService.DrawBorder("VOTE CHO MÔTHER SPORT ESPORTS", 80, 15);

                // Implement sport voting logic here
                Console.WriteLine("🗳️ Chức năng vote cho môn thể thao đang được phát triển...");
                
                await Task.Delay(100); // Placeholder async operation
                
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
                            HandleViewVotingResults();
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

                // Mock player list for voting
                var mockPlayers = new[] { "Player1", "Player2", "Player3", "Player4" };
                
                Console.WriteLine("👥 Chọn player để vote:");
                for (int i = 0; i < mockPlayers.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {mockPlayers[i]}");
                }

                Console.Write($"\nNhập số thứ tự player (1-{mockPlayers.Length}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && 
                    choice >= 1 && choice <= mockPlayers.Length)
                {
                    var selectedPlayer = mockPlayers[choice - 1];
                    
                    await Task.Delay(500); // Simulate processing
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedPlayer}!", true, 2000);
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
                        
                        await Task.Delay(500); // Simulate processing
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedTournament.Name}!", true, 2000);
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
                    
                    await Task.Delay(500); // Simulate processing
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã vote cho {selectedCategory}!", true, 2000);
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

        private void HandleViewVotingResults()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING", 80, 20);
                
                Console.WriteLine("📊 KẾT QUẢ VOTING TỔNG HỢP");
                Console.WriteLine("─".PadRight(78, '─'));
                
                Console.WriteLine("\n🏆 TOP 5 PLAYER YÊU THÍCH:");
                var mockPlayerResults = new[]
                {
                    ("Player1", 150),
                    ("Player2", 120),
                    ("Player3", 95),
                    ("Player4", 80),
                    ("Player5", 65)
                };
                
                foreach (var (name, votes) in mockPlayerResults)
                {
                    Console.WriteLine($"  • {name}: {votes} votes");
                }
                
                Console.WriteLine("\n🎮 TOP 5 GIẢI ĐẤU HAY NHẤT:");
                var mockTournamentResults = new[]
                {
                    ("LOL Championship", 200),
                    ("CS:GO Masters", 180),
                    ("PUBG Mobile Cup", 150),
                    ("FIFA Online League", 120),
                    ("Valorant Series", 100)
                };
                
                foreach (var (name, votes) in mockTournamentResults)
                {
                    Console.WriteLine($"  • {name}: {votes} votes");
                }
                
                Console.WriteLine("\n🏅 TOP 3 MÔN THỂ THAO ESPORTS:");
                var mockCategoryResults = new[]
                {
                    ("League of Legends", 300),
                    ("Counter-Strike: GO", 250),
                    ("PUBG Mobile", 220)
                };
                
                foreach (var (name, votes) in mockCategoryResults)
                {
                    Console.WriteLine($"  • {name}: {votes} votes");
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
