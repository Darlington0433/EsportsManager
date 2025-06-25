using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Shared;

namespace EsportsManager.UI.Controllers.Shared
{
    /// <summary>
    /// Handler cho xem danh sách giải đấu
    /// Single Responsibility: Chỉ lo việc hiển thị tournament list
    /// </summary>
    public class TournamentViewHandler : ITournamentViewHandler
    {
        private readonly ITournamentService _tournamentService;

        public TournamentViewHandler(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        }

        public async Task HandleViewTournamentListAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 100, 20);

                var tournaments = await _tournamentService.GetAllTournamentsAsync();

                if (tournaments.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào", false, 2000);
                    return;
                }

                Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-35} {"Trạng thái",-15} {"Phí tham gia",-15}");
                Console.WriteLine(new string('=', 70));

                for (int i = 0; i < tournaments.Count; i++)
                {
                    var tournament = tournaments[i];
                    Console.WriteLine($"{i + 1,-5} {tournament.Name,-35} {tournament.Status,-15} {tournament.EntryFee:N0,-15}");
                }

                Console.WriteLine(new string('=', 70));
                ConsoleRenderingService.PauseWithMessage();
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }
    }
}
