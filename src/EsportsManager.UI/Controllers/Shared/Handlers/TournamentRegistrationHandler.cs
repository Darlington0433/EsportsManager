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
    /// Handler cho đăng ký tham gia giải đấu
    /// Single Responsibility: Chỉ lo việc đăng ký giải đấu
    /// </summary>
    public class TournamentRegistrationHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;

        public TournamentRegistrationHandler(
            UserProfileDto currentUser,
            ITournamentService tournamentService,
            ITeamService teamService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        }

        public async Task HandleTournamentRegistrationAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("ĐĂNG KÝ THAM GIA GIẢI ĐẤU", 80, 15);

                var tournaments = await _tournamentService.GetAvailableTournamentsAsync();

                if (tournaments.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào mở đăng ký", false, 2000);
                    return;
                }

                Console.WriteLine("🏆 Danh sách giải đấu có sẵn:");
                for (int i = 0; i < tournaments.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {tournaments[i].Name} - Phí: {tournaments[i].EntryFee:N0} VND - Status: {tournaments[i].Status}");
                }

                Console.Write($"\nNhập số thứ tự giải đấu muốn tham gia (1-{tournaments.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
                {
                    var selectedTournament = tournaments[choice - 1];
                    bool success = await RegisterForTournamentAsync(selectedTournament.Id);

                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"Đã đăng ký tham gia '{selectedTournament.Name}' thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Đăng ký thất bại! Có thể team đã đăng ký tournament này rồi hoặc tournament đã đầy.", true, 3000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task<bool> RegisterForTournamentAsync(int tournamentId)
        {
            try
            {
                // Get player's team first
                var team = await _teamService.GetPlayerTeamAsync(_currentUser.Id);
                if (team == null)
                {
                    return false; // Player needs to be in a team to register
                }

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
