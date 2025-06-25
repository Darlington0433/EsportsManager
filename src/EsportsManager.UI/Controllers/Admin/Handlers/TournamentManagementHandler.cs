using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class TournamentManagementHandler
{
    private readonly ITournamentService _tournamentService;

    public TournamentManagementHandler(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    public async Task ManageTournamentsAsync()
    {
        while (true)
        {
            var tournamentOptions = new[]
            {
                "Xem danh sách giải đấu",
                "Tạo giải đấu mới",
                "Cập nhật giải đấu",
                "Xóa giải đấu",
                "Xem thống kê giải đấu",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ GIẢI ĐẤU", tournamentOptions);

            switch (selection)
            {
                case 0:
                    await ShowAllTournamentsAsync();
                    break;
                case 1:
                    await CreateTournamentAsync();
                    break;
                case 2:
                    await UpdateTournamentAsync();
                    break;
                case 3:
                    await DeleteTournamentAsync();
                    break;
                case 4:
                    await ShowTournamentStatsAsync();
                    break;
                case -1:
                case 5:
                    return;
            }
        }
    }

    public async Task ShowAllTournamentsAsync()
    {
        try
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 120, 25);

            if (!tournaments.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có giải đấu nào trong hệ thống.", ConsoleColor.Yellow);
                return;
            }

            DisplayTournamentsTable(tournaments);

            Console.WriteLine($"\nTổng cộng: {tournaments.Count} giải đấu");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải danh sách giải đấu: {ex.Message}", true, 3000);
        }
    }

    public async Task CreateTournamentAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TẠO GIẢI ĐẤU MỚI", 80, 20);

            var tournamentData = CollectTournamentData();
            if (tournamentData == null) return;

            var result = await _tournamentService.CreateTournamentAsync(tournamentData);
            if (result != null)
            {
                ConsoleRenderingService.ShowMessageBox($"✅ Tạo giải đấu thành công! ID: {result.TournamentId}", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Tạo giải đấu thất bại!", true, 3000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tạo giải đấu: {ex.Message}", true, 3000);
        }
    }

    public async Task UpdateTournamentAsync()
    {
        try
        {
            Console.Write("Nhập Tournament ID cần cập nhật: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId))
            {
                ConsoleRenderingService.ShowNotification("Tournament ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy giải đấu!", ConsoleColor.Red);
                return;
            }

            var updateData = CollectTournamentUpdateData(tournament);
            if (updateData == null) return;

            var success = await _tournamentService.UpdateTournamentAsync(tournamentId, updateData);
            if (success)
            {
                ConsoleRenderingService.ShowMessageBox("✅ Cập nhật giải đấu thành công!", false, 2000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Cập nhật thất bại!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi cập nhật giải đấu: {ex.Message}", true, 3000);
        }
    }

    public async Task DeleteTournamentAsync()
    {
        try
        {
            Console.Write("Nhập Tournament ID cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId))
            {
                ConsoleRenderingService.ShowNotification("Tournament ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy giải đấu!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine($"\n⚠️  CẢNH BÁO: Bạn đang xóa giải đấu: {tournament.TournamentName}");
            Console.WriteLine("Thao tác này không thể hoàn tác!");
            Console.Write("Xác nhận xóa? (YES để xác nhận): ");

            var confirmation = Console.ReadLine()?.Trim();
            if (confirmation?.ToUpper() == "YES")
            {
                var success = await _tournamentService.DeleteTournamentAsync(tournamentId);
                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox("✅ Xóa giải đấu thành công!", false, 2000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Xóa thất bại!", true, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác xóa", false, 1000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi xóa giải đấu: {ex.Message}", true, 3000);
        }
    }

    public async Task ShowTournamentStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ GIẢI ĐẤU", 80, 20);

            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var stats = CalculateTournamentStats(tournaments);

            DisplayTournamentStats(stats);

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }

    private void DisplayTournamentsTable(IEnumerable<TournamentInfoDto> tournaments)
    {
        var header = string.Format("{0,-5} {1,-30} {2,-20} {3,-15} {4,-15} {5,-12} {6,-10}",
            "ID", "Tên giải đấu", "Game", "Ngày bắt đầu", "Ngày kết thúc", "Trạng thái", "Số team");
        Console.WriteLine("\n" + header);
        Console.WriteLine(new string('─', 110));

        foreach (var tournament in tournaments)
        {
            var tournamentName = tournament.TournamentName.Length > 28 ? 
                tournament.TournamentName.Substring(0, 28) + ".." : 
                tournament.TournamentName;

            var row = string.Format("{0,-5} {1,-30} {2,-20} {3,-15} {4,-15} {5,-12} {6,-10}",
                tournament.TournamentId,
                tournamentName,
                tournament.GameName ?? "N/A",
                tournament.StartDate.ToString("dd/MM/yyyy"),
                tournament.EndDate.ToString("dd/MM/yyyy"),
                tournament.Status,
                tournament.RegisteredTeams);
            Console.WriteLine(row);
        }
    }

    private TournamentCreateDto? CollectTournamentData()
    {
        try
        {
            Console.Write("Tên giải đấu: ");
            var tournamentName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(tournamentName))
            {
                ConsoleRenderingService.ShowNotification("Tên giải đấu không được để trống!", ConsoleColor.Red);
                return null;
            }

            Console.Write("Mô tả: ");
            var description = Console.ReadLine()?.Trim();

            Console.Write("Game ID (1=LoL, 2=CS2, 3=Valorant): ");
            if (!int.TryParse(Console.ReadLine(), out int gameId) || gameId < 1)
            {
                ConsoleRenderingService.ShowNotification("Game ID không hợp lệ!", ConsoleColor.Red);
                return null;
            }

            Console.Write("Số team tối đa (mặc định 16): ");
            var maxTeamsInput = Console.ReadLine()?.Trim();
            int maxTeams = string.IsNullOrEmpty(maxTeamsInput) ? 16 : (int.TryParse(maxTeamsInput, out int mt) ? mt : 16);

            Console.Write("Phí tham gia (VND, mặc định 0): ");
            var entryFeeInput = Console.ReadLine()?.Trim();
            decimal entryFee = string.IsNullOrEmpty(entryFeeInput) ? 0 : (decimal.TryParse(entryFeeInput, out decimal ef) ? ef : 0);

            return new TournamentCreateDto
            {
                TournamentName = tournamentName,
                Description = description ?? string.Empty,
                GameId = gameId,
                MaxTeams = maxTeams,
                EntryFee = entryFee,
                StartDate = DateTime.Now.AddDays(7), // Default 7 days from now
                EndDate = DateTime.Now.AddDays(14), // Default 14 days from now
                RegistrationDeadline = DateTime.Now.AddDays(5), // Default 5 days from now
                CreatedBy = 1 // Default admin user, should be passed from context
            };
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi thu thập dữ liệu: {ex.Message}", true, 3000);
            return null;
        }
    }

    private TournamentUpdateDto? CollectTournamentUpdateData(TournamentInfoDto currentTournament)
    {
        try
        {
            Console.WriteLine($"\nThông tin hiện tại của giải đấu: {currentTournament.TournamentName}");
            Console.WriteLine("Nhấn Enter để giữ nguyên giá trị hiện tại\n");

            Console.Write($"Tên giải đấu ({currentTournament.TournamentName}): ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(name)) name = currentTournament.TournamentName;

            Console.Write($"Mô tả ({currentTournament.Description}): ");
            var description = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(description)) description = currentTournament.Description;

            return new TournamentUpdateDto
            {
                TournamentName = name,
                Description = description,
                Status = currentTournament.Status,
                StartDate = currentTournament.StartDate,
                EndDate = currentTournament.EndDate,
                RegistrationDeadline = currentTournament.RegistrationDeadline,
                MaxTeams = currentTournament.MaxTeams,
                EntryFee = currentTournament.EntryFee,
                PrizePool = currentTournament.PrizePool
                // Add other fields as needed
            };
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi thu thập dữ liệu cập nhật: {ex.Message}", true, 3000);
            return null;
        }
    }

    private object CalculateTournamentStats(IEnumerable<TournamentInfoDto> tournaments)
    {
        return new
        {
            TotalTournaments = tournaments.Count(),
            ActiveTournaments = tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration"),
            CompletedTournaments = tournaments.Count(t => t.Status == "Completed"),
            TotalPrizePool = tournaments.Sum(t => t.PrizePool),
            AvgTeamsPerTournament = tournaments.Any() ? tournaments.Average(t => t.RegisteredTeams) : 0
        };
    }

    private void DisplayTournamentStats(object stats)
    {
        var statsProps = stats.GetType().GetProperties();
        Console.WriteLine("THỐNG KÊ GIẢI ĐẤU:");
        Console.WriteLine(new string('─', 50));

        foreach (var prop in statsProps)
        {
            var value = prop.GetValue(stats);
            Console.WriteLine($"{prop.Name,-25}: {value}");
        }
    }
}
