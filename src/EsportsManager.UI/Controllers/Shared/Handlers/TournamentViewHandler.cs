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

                // Sử dụng helper method để tính vị trí
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(100, 20);
                
                // Header
                ConsoleRenderingService.WriteInBorder($"{"STT",-5} {"Tên giải đấu",-35} {"Trạng thái",-15} {"Phí tham gia",-15}", left, top, 0);
                ConsoleRenderingService.WriteInBorder(new string('=', 96), left, top, 1);

                // Data rows (giới hạn 14 dòng để vừa border)
                for (int i = 0; i < Math.Min(tournaments.Count, 14); i++)
                {
                    var tournament = tournaments[i];
                    string row = $"{i + 1,-5} {tournament.Name,-35} {tournament.Status,-15} {tournament.EntryFee:N0,-15}";
                    ConsoleRenderingService.WriteInBorder(row, left, top, 2 + i);
                }

                // Footer
                ConsoleRenderingService.WriteInBorder($"Tổng cộng: {tournaments.Count} giải đấu", left, top, 15);
                ConsoleRenderingService.WriteInBorder("Nhấn phím bất kỳ để tiếp tục...", left, top, 16);
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }
    }
}
