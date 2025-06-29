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

                int borderLeft = 2;
                int borderTop = 3;
                int currentLine = borderTop;
                int borderWidth = 80 - 4; // 2 bên viền

                Console.SetCursorPosition(borderLeft, currentLine++);
                Console.WriteLine("🏆 Danh sách giải đấu có sẵn:".PadRight(borderWidth));
                for (int i = 0; i < tournaments.Count; i++)
                {
                    Console.SetCursorPosition(borderLeft, currentLine++);
                    string line = $"{i + 1}. {tournaments[i].Name} - Phí: {tournaments[i].EntryFee:N0} VND - Status: {tournaments[i].Status}";
                    Console.WriteLine(line.Length > borderWidth ? line.Substring(0, borderWidth) : line.PadRight(borderWidth));
                }

                Console.SetCursorPosition(borderLeft, currentLine++);
                string prompt = $"Nhập số thứ tự giải đấu muốn tham gia (1-{tournaments.Count}): ";
                Console.Write(prompt.Length > borderWidth ? prompt.Substring(0, borderWidth) : prompt.PadRight(borderWidth));
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

        private void DisplayRegistrationsTableInBorder(IEnumerable<TournamentRegistrationDto> registrations, int startX, int startY, int maxWidth)
        {
            var headers = new[] { "ID", "Tên đội", "Người đại diện", "Thời gian đăng ký", "Trạng thái" };
            var rows = registrations.Select(r => new[] {
                r.RegistrationId.ToString(),
                r.TeamName.Length > 18 ? r.TeamName.Substring(0, 18) : r.TeamName,
                r.RegisteredByName.Length > 14 ? r.RegisteredByName.Substring(0, 14) : r.RegisteredByName,
                r.RegistrationDate.ToString("dd/MM/yyyy HH:mm"),
                r.Status
            }).ToList();
            int borderWidth = maxWidth;
            int borderHeight = 16;
            int[] colWidths = { 5, 20, 16, 20, 14 }; // Tổng + phân cách <= borderWidth - 4
            UIHelper.PrintTableInBorder(headers, rows, borderWidth, borderHeight, startX, startY, colWidths);
            int infoY = startY + 2 + rows.Count + 2;
            UIHelper.PrintPromptInBorder($"Tổng cộng: {registrations.Count()} đăng ký", startX, infoY, borderWidth - 4);
            Console.SetCursorPosition(0, startY + borderHeight + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
        }
    }
}
