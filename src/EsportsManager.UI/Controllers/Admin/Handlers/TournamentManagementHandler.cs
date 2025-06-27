using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.BL.Constants;
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
                "Duyệt đăng ký giải đấu",
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
                case 5:
                    await ApproveTournamentRegistrationsAsync();
                    break;
                case -1:
                case 6:
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

            if (tournaments == null || !tournaments.Any())
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
            var stats = TournamentStatsService.CalculateTournamentStats(tournaments);

            DisplayTournamentStats(stats);

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }

    public async Task ApproveTournamentRegistrationsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DUYỆT ĐĂNG KÝ GIẢI ĐẤU", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            // TODO: Cần bổ sung phương thức GetPendingRegistrationsAsync vào ITournamentService và triển khai trong TournamentService
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Tính năng chưa được triển khai đầy đủ");
            Console.WriteLine();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Cần bổ sung các phương thức sau vào ITournamentService:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine("- GetPendingRegistrationsAsync()");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
            Console.WriteLine("- ApproveRegistrationAsync(int registrationId)");

            Console.SetCursorPosition(borderLeft + 2, borderTop + 8);
            Console.WriteLine("Vui lòng liên hệ với team phát triển để hoàn thiện tính năng này.");

            Console.ResetColor();
            Console.SetCursorPosition(borderLeft + 2, borderTop + 10);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);

            await Task.CompletedTask; // Để đảm bảo phương thức có await

            /* TODO: Triển khai khi bổ sung các phương thức vào ITournamentService
            var pendingRegistrations = await _tournamentService.GetPendingRegistrationsAsync();
            
            if (pendingRegistrations == null || !pendingRegistrations.Any())
            {
                ConsoleRenderingService.ShowNotification("Không có đăng ký nào đang chờ duyệt", ConsoleColor.Yellow);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{"ID",-5} {"Player",-15} {"Tournament",-20} {"Ngày đăng ký",-15} {"Team",-10}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 4;
            foreach (var registration in pendingRegistrations.Take(10))
            {
                Console.SetCursorPosition(borderLeft + 2, currentRow);
                Console.ForegroundColor = ConsoleColor.Yellow;
                var row = string.Format("{0,-5} {1,-15} {2,-20} {3,-15} {4,-10}",
                    registration.Id,
                    registration.PlayerName,
                    registration.TournamentName.Length > TournamentConstants.MAX_TOURNAMENT_NAME_SHORT ? 
                        TournamentStatsService.FormatTournamentNameShort(registration.TournamentName) : registration.TournamentName,
                    registration.RegisterDate.ToString("dd/MM/yyyy"),
                    registration.TeamName ?? "Individual");
                Console.WriteLine(row);
                currentRow++;
            }

            Console.ResetColor();
            Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
            Console.WriteLine($"Tổng cộng: {pendingRegistrations.Count} đăng ký chờ duyệt");

            Console.Write("\nNhập Registration ID cần duyệt (0 để quay lại): ");
            if (int.TryParse(Console.ReadLine(), out int registrationId) && registrationId > 0)
            {
                var result = await _tournamentService.ApproveRegistrationAsync(registrationId);
                if (result)
                {
                    ConsoleRenderingService.ShowMessageBox("✅ Đã duyệt đăng ký thành công!", false, 2000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Duyệt đăng ký thất bại!", true, 2000);
                }
            }
            */
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    public async Task ManageTeamsAsync()
    {
        while (true)
        {
            var teamOptions = new[]
            {
                "Xem danh sách đội",
                "Tìm kiếm đội",
                "Duyệt đội mới",
                "Duyệt thành viên đội",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ ĐỘI/TEAM", teamOptions);

            switch (selection)
            {
                case 0:
                    await ShowTeamListAsync();
                    break;
                case 1:
                    await SearchTeamsAsync();
                    break;
                case 2:
                    await ApproveNewTeamsAsync();
                    break;
                case 3:
                    await ApproveTeamMembersAsync();
                    break;
                case -1:
                case 4:
                    return;
            }

            await Task.Delay(100); // Prevent compiler warning
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
            var tournamentName = TournamentStatsService.FormatTournamentNameForDisplay(tournament.TournamentName) +
                (tournament.TournamentName.Length > TournamentConstants.MAX_TOURNAMENT_NAME_DISPLAY ? ".." : "");

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
            var nameValidation = TournamentValidationService.ValidateTournamentName(tournamentName);
            if (!nameValidation.IsValid)
            {
                ConsoleRenderingService.ShowNotification(nameValidation.ErrorMessage, ConsoleColor.Red);
                return null;
            }

            Console.Write("Mô tả: ");
            var description = Console.ReadLine()?.Trim();

            Console.Write($"Game ID ({string.Join(", ", TournamentConstants.GAME_TYPES.Select(g => $"{g.Key}={g.Value}"))}): ");
            if (!int.TryParse(Console.ReadLine(), out int gameId))
            {
                ConsoleRenderingService.ShowNotification("Game ID phải là số!", ConsoleColor.Red);
                return null;
            }

            var gameValidation = TournamentValidationService.ValidateGameId(gameId);
            if (!gameValidation.IsValid)
            {
                ConsoleRenderingService.ShowNotification(gameValidation.ErrorMessage, ConsoleColor.Red);
                return null;
            }

            Console.Write($"Số team tối đa (mặc định {TournamentConstants.DEFAULT_MAX_TEAMS}): ");
            var maxTeamsInput = Console.ReadLine()?.Trim();
            var maxTeamsValidation = TournamentValidationService.ValidateMaxTeams(maxTeamsInput);
            if (!maxTeamsValidation.IsValid)
            {
                ConsoleRenderingService.ShowNotification(maxTeamsValidation.ErrorMessage, ConsoleColor.Red);
                return null;
            }

            Console.Write($"Phí tham gia (VND, mặc định {TournamentConstants.DEFAULT_ENTRY_FEE}): ");
            var entryFeeInput = Console.ReadLine()?.Trim();
            var entryFeeValidation = TournamentValidationService.ValidateEntryFee(entryFeeInput);
            if (!entryFeeValidation.IsValid)
            {
                ConsoleRenderingService.ShowNotification(entryFeeValidation.ErrorMessage, ConsoleColor.Red);
                return null;
            }

            var defaultDates = TournamentValidationService.GetDefaultTournamentDates();

            return new TournamentCreateDto
            {
                TournamentName = tournamentName!,
                Description = description ?? string.Empty,
                GameId = gameId,
                MaxTeams = maxTeamsValidation.ValidatedValue,
                EntryFee = entryFeeValidation.ValidatedValue,
                StartDate = defaultDates.StartDate,
                EndDate = defaultDates.EndDate,
                RegistrationDeadline = defaultDates.RegistrationDeadline,
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

    private void DisplayTournamentStats(TournamentStatsDto stats)
    {
        Console.WriteLine("THỐNG KÊ GIẢI ĐẤU:");
        Console.WriteLine(new string('─', 50));
        Console.WriteLine($"{"Tổng giải đấu",-25}: {stats.TotalTournaments}");
        Console.WriteLine($"{"Giải đang hoạt động",-25}: {stats.ActiveTournaments}");
        Console.WriteLine($"{"Giải đã hoàn thành",-25}: {stats.CompletedTournaments}");
        Console.WriteLine($"{"Tổng giải thưởng",-25}: {stats.TotalPrizePool:N0} VND");
        Console.WriteLine($"{"TB team/giải",-25}: {stats.AvgTeamsPerTournament:F1}");
    }

    /// <summary>
    /// Hiển thị danh sách tất cả đội
    /// </summary>
    private async Task ShowTeamListAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH ĐỘI", 80, 20);

            // Sample team data
            var sampleTeams = new[]
            {
                new { Id = 1, Name = "Shadow Legends", Leader = "ProPlayer1", Members = 5, Game = "LoL", Status = "Active" },
                new { Id = 2, Name = "Fire Dragons", Leader = "GameMaster", Members = 4, Game = "CS:GO", Status = "Active" },
                new { Id = 3, Name = "Storm Raiders", Leader = "TacticalLead", Members = 6, Game = "Valorant", Status = "Active" },
                new { Id = 4, Name = "Ice Wolves", Leader = "ColdStrike", Members = 5, Game = "Dota 2", Status = "Pending" },
                new { Id = 5, Name = "Thunder Bolts", Leader = "QuickStrike", Members = 4, Game = "CS:GO", Status = "Active" }
            };

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("📋 Danh sách tất cả đội trong hệ thống:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            Console.WriteLine(new string('─', 70));

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine($"{"ID",-3} {"Tên đội",-18} {"Leader",-12} {"Members",-8} {"Game",-10} {"Status",-8}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 6;
            for (int i = 0; i < sampleTeams.Length; i++)
            {
                var team = sampleTeams[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i);

                Console.ForegroundColor = team.Status == "Active" ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.WriteLine($"{team.Id,-3} {team.Name,-18} {team.Leader,-12} {team.Members,-8} {team.Game,-10} {team.Status,-8}");
                Console.ResetColor();
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
            Console.WriteLine($"Tổng cộng: {sampleTeams.Length} đội");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Tìm kiếm đội theo tên
    /// </summary>
    private async Task SearchTeamsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TÌM KIẾM ĐỘI", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.Write("🔍 Nhập tên đội cần tìm: ");
            var searchTerm = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowMessageBox("❌ Vui lòng nhập từ khóa tìm kiếm!", true, 2000);
                return;
            }

            // Sample search results
            var allTeams = new[]
            {
                new { Id = 1, Name = "Shadow Legends", Leader = "ProPlayer1", Members = 5, Game = "LoL", Status = "Active" },
                new { Id = 2, Name = "Fire Dragons", Leader = "GameMaster", Members = 4, Game = "CS:GO", Status = "Active" },
                new { Id = 3, Name = "Storm Raiders", Leader = "TacticalLead", Members = 6, Game = "Valorant", Status = "Active" },
                new { Id = 4, Name = "Ice Wolves", Leader = "ColdStrike", Members = 5, Game = "Dota 2", Status = "Pending" },
                new { Id = 5, Name = "Thunder Bolts", Leader = "QuickStrike", Members = 4, Game = "CS:GO", Status = "Active" }
            };

            var searchResults = allTeams.Where(t => t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToArray();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine($"📊 Kết quả tìm kiếm cho: '{searchTerm}'");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('─', 70));

            if (!searchResults.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không tìm thấy đội nào phù hợp.");
            }
            else
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine($"{"ID",-3} {"Tên đội",-18} {"Leader",-12} {"Members",-8} {"Game",-10} {"Status",-8}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.WriteLine(new string('─', 70));

                int currentRow = borderTop + 8;
                for (int i = 0; i < searchResults.Length; i++)
                {
                    var team = searchResults[i];
                    Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                    Console.ForegroundColor = team.Status == "Active" ? ConsoleColor.Green : ConsoleColor.Yellow;
                    Console.WriteLine($"{team.Id,-3} {team.Name,-18} {team.Leader,-12} {team.Members,-8} {team.Game,-10} {team.Status,-8}");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                Console.WriteLine($"Tìm thấy: {searchResults.Length} đội");
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Phê duyệt đội mới
    /// </summary>
    private async Task ApproveNewTeamsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DUYỆT ĐỘI MỚI", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("✅ Phê duyệt các đội mới đăng ký");
            Console.WriteLine();

            // Sample pending teams
            var pendingTeams = new[]
            {
                new { Id = 6, Name = "Lightning Strikes", Leader = "FastPlayer", Members = 3, Game = "Valorant", Status = "Pending" },
                new { Id = 7, Name = "Mystic Warriors", Leader = "MagicUser", Members = 5, Game = "LoL", Status = "Pending" },
                new { Id = 8, Name = "Cyber Ninjas", Leader = "SilentKill", Members = 4, Game = "CS:GO", Status = "Pending" }
            };

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Đội chờ phê duyệt:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 6;
            for (int i = 0; i < pendingTeams.Length; i++)
            {
                var team = pendingTeams[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 2);
                Console.WriteLine($"{i + 1}. {team.Name} ({team.Game})");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 2 + 1);
                Console.WriteLine($"   👤 Leader: {team.Leader} | 👥 {team.Members} members");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + pendingTeams.Length * 2 + 1);
            Console.Write("Chọn đội để duyệt (1-3, 0=thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= pendingTeams.Length)
            {
                var selectedTeam = pendingTeams[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + pendingTeams.Length * 2 + 3);
                Console.Write($"Duyệt đội '{selectedTeam.Name}'? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt đội '{selectedTeam.Name}' thành công!", false, 2500);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy", false, 1000);
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Phê duyệt thành viên đội
    /// </summary>
    private async Task ApproveTeamMembersAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DUYỆT THÀNH VIÊN", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("👥 Phê duyệt thành viên gia nhập đội");
            Console.WriteLine();

            // Sample member requests
            var memberRequests = new[]
            {
                new { Id = 1, Player = "NewPlayer123", Team = "Shadow Legends", Role = "Support", Exp = "2 years" },
                new { Id = 2, Player = "ProGamer456", Team = "Fire Dragons", Role = "Carry", Exp = "3 years" },
                new { Id = 3, Player = "SkillMaster", Team = "Storm Raiders", Role = "Mid", Exp = "1.5 years" }
            };

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Yêu cầu gia nhập đội:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 6;
            for (int i = 0; i < memberRequests.Length; i++)
            {
                var req = memberRequests[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 3);
                Console.WriteLine($"{i + 1}. {req.Player} → {req.Team}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 1);
                Console.WriteLine($"   🎯 Role: {req.Role}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 2);
                Console.WriteLine($"   ⏱️ Experience: {req.Exp}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + memberRequests.Length * 3 + 1);
            Console.Write("Chọn yêu cầu để duyệt (1-3, 0=thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= memberRequests.Length)
            {
                var selectedReq = memberRequests[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + memberRequests.Length * 3 + 3);
                Console.Write($"Duyệt {selectedReq.Player} gia nhập {selectedReq.Team}? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt {selectedReq.Player} gia nhập đội {selectedReq.Team}!", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã từ chối", false, 1000);
                }
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }
}
