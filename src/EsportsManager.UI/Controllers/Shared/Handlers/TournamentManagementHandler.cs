using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Shared.Handlers
{
    /// <summary>
    /// Handler cho quản lý giải đấu (xem, đăng ký, hủy đăng ký)
    /// </summary>
    public class TournamentManagementHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;

        public TournamentManagementHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            ITeamService teamService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        }

        public async Task HandleTournamentManagementAsync()
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                try
                {
                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("QUẢN LÝ GIẢI ĐẤU", 80, 15);

                    Console.WriteLine("🏆 Chọn chức năng quản lý giải đấu:");
                    Console.WriteLine("1. Xem giải đấu đã đăng ký");
                    Console.WriteLine("2. Đăng ký giải đấu mới");
                    Console.WriteLine("3. Xem thông tin chi tiết giải đấu");
                    Console.WriteLine("4. Hủy đăng ký giải đấu");
                    Console.WriteLine("0. Quay lại");

                    Console.Write("\nNhập lựa chọn của bạn: ");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await ShowRegisteredTournamentsAsync();
                            break;
                        case "2":
                            await RegisterForNewTournamentAsync();
                            break;
                        case "3":
                            await ShowTournamentDetailsAsync();
                            break;
                        case "4":
                            await UnregisterFromTournamentAsync();
                            break;
                        case "0":
                            keepRunning = false;
                            break;
                        default:
                            ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                            break;
                    }

                    if (keepRunning && choice != "0")
                    {
                        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
                }
            }
        }

        private async Task ShowRegisteredTournamentsAsync()
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GIẢI ĐẤU ĐÃ ĐĂNG KÝ", 80, 15);

            var team = await _teamService.GetPlayerTeamAsync(_currentUser.Id);
            if (team == null)
            {
                ConsoleRenderingService.ShowMessageBox("Bạn cần tham gia một team trước khi xem giải đấu đã đăng ký!", true, 3000);
                return;
            }

            var tournaments = await _tournamentService.GetTeamTournamentsAsync(team.Id);

            if (tournaments.Count == 0)
            {
                Console.WriteLine("🔍 Team của bạn chưa đăng ký tham gia giải đấu nào.");
                return;
            }

            Console.WriteLine($"🏆 Team '{team.Name}' đã đăng ký {tournaments.Count} giải đấu:");
            Console.WriteLine();

            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"═══ Giải đấu {i + 1} ═══");
                Console.WriteLine($"📋 Tên: {tournament.Name}");
                Console.WriteLine($"🎮 Game: {tournament.GameName}");
                Console.WriteLine($"📅 Thời gian: {tournament.StartDate:dd/MM/yyyy} - {tournament.EndDate:dd/MM/yyyy}");
                Console.WriteLine($"💰 Phí tham gia: {tournament.EntryFee:N0} VND");
                Console.WriteLine($"🏆 Giải thưởng: {tournament.PrizePool:N0} VND");
                Console.WriteLine($"📊 Trạng thái: {tournament.Status}");
                Console.WriteLine();
            }
        }

        private async Task RegisterForNewTournamentAsync()
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("ĐĂNG KÝ GIẢI ĐẤU MỚI", 80, 15);

            var team = await _teamService.GetPlayerTeamAsync(_currentUser.Id);
            if (team == null)
            {
                ConsoleRenderingService.ShowMessageBox("Bạn cần tham gia một team trước khi đăng ký giải đấu!", true, 3000);
                return;
            }

            var tournaments = await _tournamentService.GetAvailableTournamentsAsync();

            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào mở đăng ký", false, 2000);
                return;
            }

            Console.WriteLine("🏆 Danh sách giải đấu có thể đăng ký:");
            Console.WriteLine();

            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"═══ Lựa chọn {i + 1} ═══");
                Console.WriteLine($"📋 Tên: {tournament.Name}");
                Console.WriteLine($"🎮 Game: {tournament.GameName}");
                Console.WriteLine($"📅 Hạn đăng ký: {tournament.RegistrationDeadline:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"👥 Số team: {tournament.RegisteredTeams}/{tournament.MaxTeams}");
                Console.WriteLine($"💰 Phí: {tournament.EntryFee:N0} VND");
                Console.WriteLine($"🏆 Giải thưởng: {tournament.PrizePool:N0} VND");
                Console.WriteLine();
            }

            Console.Write($"Nhập số thứ tự giải đấu muốn tham gia (1-{tournaments.Count}, 0 để hủy): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
            {
                var selectedTournament = tournaments[choice - 1];

                Console.WriteLine();
                Console.WriteLine($"Bạn đã chọn: {selectedTournament.Name}");
                Console.Write("Xác nhận đăng ký? (y/n): ");

                var confirm = Console.ReadLine()?.ToLower();
                if (confirm == "y" || confirm == "yes")
                {
                    bool success = await RegisterForTournamentAsync(selectedTournament.Id, team);

                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã đăng ký team '{team.Name}' tham gia '{selectedTournament.Name}' thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Đăng ký thất bại! Team có thể đã đăng ký tournament này rồi hoặc tournament đã đầy.", true, 3000);
                    }
                }
                else
                {
                    Console.WriteLine("Đã hủy đăng ký.");
                }
            }
            else if (choice != 0)
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }

        private async Task ShowTournamentDetailsAsync()
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÔNG TIN CHI TIẾT GIẢI ĐẤU", 80, 15);

            var tournaments = await _tournamentService.GetAllTournamentsAsync();

            if (tournaments.Count == 0)
            {
                Console.WriteLine("Không có giải đấu nào trong hệ thống.");
                return;
            }

            Console.WriteLine("📋 Danh sách tất cả giải đấu:");
            for (int i = 0; i < tournaments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tournaments[i].Name} - {tournaments[i].Status}");
            }

            Console.Write($"\nNhập số thứ tự giải đấu để xem chi tiết (1-{tournaments.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
            {
                var tournament = tournaments[choice - 1];
                await DisplayTournamentDetailsAsync(tournament);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }

        private async Task DisplayTournamentDetailsAsync(TournamentInfoDto tournament)
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CHI TIẾT: {tournament.Name.ToUpper()}", 80, 20);

            Console.WriteLine($"📋 Tên giải đấu: {tournament.Name}");
            Console.WriteLine($"📝 Mô tả: {tournament.Description}");
            Console.WriteLine($"🎮 Game: {tournament.GameName}");
            Console.WriteLine($"📅 Thời gian bắt đầu: {tournament.StartDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"📅 Thời gian kết thúc: {tournament.EndDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"⏰ Hạn đăng ký: {tournament.RegistrationDeadline:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"👥 Số team tham gia: {tournament.RegisteredTeams}/{tournament.MaxTeams}");
            Console.WriteLine($"💰 Phí tham gia: {tournament.EntryFee:N0} VND");
            Console.WriteLine($"🏆 Tổng giải thưởng: {tournament.PrizePool:N0} VND");
            Console.WriteLine($"📊 Trạng thái: {tournament.Status}");
            Console.WriteLine();

            // Hiển thị danh sách team đã đăng ký
            try
            {
                var teams = await _tournamentService.GetTournamentTeamsAsync(tournament.Id);
                if (teams.Count > 0)
                {
                    Console.WriteLine("👥 Danh sách team đã đăng ký:");
                    for (int i = 0; i < teams.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {teams[i].Name} (Leader: {teams[i].LeaderName}, {teams[i].MemberCount} thành viên)");
                    }
                }
                else
                {
                    Console.WriteLine("👥 Chưa có team nào đăng ký.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Không thể tải danh sách team: {ex.Message}");
            }
        }

        private async Task UnregisterFromTournamentAsync()
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("HỦY ĐĂNG KÝ GIẢI ĐẤU", 80, 15);

            var team = await _teamService.GetPlayerTeamAsync(_currentUser.Id);
            if (team == null)
            {
                ConsoleRenderingService.ShowMessageBox("Bạn cần tham gia một team trước!", true, 3000);
                return;
            }

            var tournaments = await _tournamentService.GetTeamTournamentsAsync(team.Id);

            if (tournaments.Count == 0)
            {
                Console.WriteLine("Team của bạn chưa đăng ký giải đấu nào.");
                return;
            }

            Console.WriteLine($"🏆 Team '{team.Name}' đã đăng ký các giải đấu sau:");
            Console.WriteLine();

            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"{i + 1}. {tournament.Name} - {tournament.Status}");
                Console.WriteLine($"   📅 {tournament.StartDate:dd/MM/yyyy} - {tournament.EndDate:dd/MM/yyyy}");
                Console.WriteLine();
            }

            Console.Write($"Nhập số thứ tự giải đấu muốn hủy đăng ký (1-{tournaments.Count}, 0 để hủy): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
            {
                var selectedTournament = tournaments[choice - 1];

                Console.WriteLine();
                Console.WriteLine($"Bạn đã chọn hủy đăng ký: {selectedTournament.Name}");
                Console.Write("Xác nhận hủy đăng ký? (y/n): ");

                var confirm = Console.ReadLine()?.ToLower();
                if (confirm == "y" || confirm == "yes")
                {
                    bool success = await _tournamentService.UnregisterTeamFromTournamentAsync(selectedTournament.Id, team.Id);

                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã hủy đăng ký team '{team.Name}' khỏi '{selectedTournament.Name}' thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Hủy đăng ký thất bại! Có thể giải đấu đã bắt đầu.", true, 3000);
                    }
                }
                else
                {
                    Console.WriteLine("Đã hủy thao tác.");
                }
            }
            else if (choice != 0)
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }

        private async Task<bool> RegisterForTournamentAsync(int tournamentId, TeamInfoDto team)
        {
            try
            {
                var registrationResult = await _tournamentService.RegisterTeamForTournamentAsync(tournamentId, team.Id);
                return registrationResult;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
