using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.BL.Constants;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;
using EsportsManager.UI.Controllers.MenuHandlers.Shared;
using EsportsManager.UI.Constants;
using EsportsManager.UI.Utilities;
using EsportsManager.UI.Controllers.Shared.Handlers;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

/// <summary>
/// Handler cho quản lý giải đấu
/// </summary>
public class TournamentManagementHandler : BaseHandler
{
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public TournamentManagementHandler(
        UserProfileDto currentUser,
        ITournamentService tournamentService,
        ITeamService teamService) : base(currentUser)
    {
        _tournamentService = tournamentService;
        _teamService = teamService;
    }

    /// <summary>
    /// Xử lý tạo giải đấu mới
    /// </summary>
    public async Task CreateTournamentAsync()
    {
        await ExecuteOperationAsync(async () =>
        {
            DrawTitledBorder("TẠO GIẢI ĐẤU MỚI", UIConstants.Border.LARGE_WIDTH, UIConstants.Border.LARGE_HEIGHT);

            var tournamentData = CollectTournamentDataAsync(); // now sync
            if (tournamentData == null)
            {
                ShowWarningMessage("Đã hủy tạo giải đấu");
                return;
            }

            try
            {
                var result = await _tournamentService.CreateTournamentAsync(tournamentData);
                if (result != null && !string.IsNullOrEmpty(result.TournamentName))
                {
                    ShowSuccessMessage("Tạo giải đấu thành công!");
                }
                else
                {
                    ShowErrorMessage($"Tạo giải đấu thất bại: Không xác định");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Tạo giải đấu thất bại: {ex.Message}");
            }
        }, "tạo giải đấu");
    }

    /// <summary>
    /// Thu thập thông tin giải đấu từ người dùng
    /// </summary>
    private TournamentCreateDto? CollectTournamentDataAsync()
    {
        var tournamentName = UIHelper.ReadString("Tên giải đấu: ", 3, 100, false);
        if (tournamentName == null) return null;

        var description = UIHelper.ReadString("Mô tả: ", allowEmpty: true) ?? string.Empty;

        if (!UIHelper.TryReadInt($"Game ID ({string.Join(", ", TournamentConstants.GAME_TYPES.Select(g => $"{g.Key}={g.Value}"))}): ", out int gameId, "Game ID"))
        {
            return null;
        }

        var gameValidation = TournamentValidationService.ValidateGameId(gameId);
        if (!gameValidation.IsValid)
        {
            ShowErrorMessage(gameValidation.ErrorMessage);
            return null;
        }

        var maxTeamsInput = UIHelper.ReadString($"Số team tối đa (mặc định {TournamentConstants.DEFAULT_MAX_TEAMS}): ", allowEmpty: true);
        var maxTeamsValidation = TournamentValidationService.ValidateMaxTeams(maxTeamsInput);
        if (!maxTeamsValidation.IsValid)
        {
            ShowErrorMessage(maxTeamsValidation.ErrorMessage);
            return null;
        }

        var entryFeeInput = UIHelper.ReadString($"Phí tham gia (VND, mặc định {TournamentConstants.DEFAULT_ENTRY_FEE}): ", allowEmpty: true);
        var entryFeeValidation = TournamentValidationService.ValidateEntryFee(entryFeeInput);
        if (!entryFeeValidation.IsValid)
        {
            ShowErrorMessage(entryFeeValidation.ErrorMessage);
            return null;
        }

        var defaultDates = TournamentValidationService.GetDefaultTournamentDates();

        return new TournamentCreateDto
        {
            TournamentName = tournamentName,
            Description = description,
            GameId = gameId,
            MaxTeams = maxTeamsValidation.ValidatedValue,
            EntryFee = entryFeeValidation.ValidatedValue,
            StartDate = defaultDates.StartDate,
            EndDate = defaultDates.EndDate,
            RegistrationDeadline = defaultDates.RegistrationDeadline,
            CreatedBy = _currentUser.Id
        };
    }

    /// <summary>
    /// Xử lý xóa giải đấu
    /// </summary>
    public async Task DeleteTournamentAsync()
    {
        await ExecuteOperationAsync(async () =>
        {
            if (!UIHelper.TryReadInt("Nhập Tournament ID cần xóa: ", out int tournamentId, "Tournament ID"))
            {
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ShowErrorMessage("Không tìm thấy giải đấu!");
                return;
            }

            Console.WriteLine($"\n{UIConstants.Icons.WARNING} CẢNH BÁO: Bạn đang xóa giải đấu: {tournament.TournamentName}");
            if (!ShowDeleteConfirmationDialog())
            {
                ShowInfoMessage(UIConstants.Messages.OPERATION_CANCELLED);
                return;
            }

            try
            {
                var result = await _tournamentService.DeleteTournamentAsync(tournamentId);
                if (result)
                {
                    ShowSuccessMessage("Xóa giải đấu thành công!");
                }
                else
                {
                    ShowErrorMessage("Xóa giải đấu thất bại!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Xóa giải đấu thất bại: {ex.Message}");
            }
        }, "xóa giải đấu");
    }

    /// <summary>
    /// Xử lý cập nhật giải đấu
    /// </summary>
    public async Task UpdateTournamentAsync()
    {
        await ExecuteOperationAsync(async () =>
        {
            if (!UIHelper.TryReadInt("Nhập Tournament ID cần cập nhật: ", out int tournamentId, "Tournament ID"))
            {
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ShowErrorMessage("Không tìm thấy giải đấu!");
                return;
            }

            var updateData = CollectTournamentUpdateDataAsync(tournament); // now sync
            if (updateData == null)
            {
                ShowWarningMessage("Đã hủy cập nhật giải đấu");
                return;
            }

            try
            {
                var result = await _tournamentService.UpdateTournamentAsync(tournamentId, updateData);
                if (result)
                {
                    ShowSuccessMessage("Cập nhật giải đấu thành công!");
                }
                else
                {
                    ShowErrorMessage($"Cập nhật giải đấu thất bại: Không xác định");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Cập nhật giải đấu thất bại: {ex.Message}");
            }
        }, "cập nhật giải đấu");
    }

    /// <summary>
    /// Thu thập thông tin cập nhật giải đấu
    /// </summary>
    private TournamentUpdateDto? CollectTournamentUpdateDataAsync(TournamentInfoDto currentTournament)
    {
        Console.WriteLine($"\nThông tin hiện tại của giải đấu: {currentTournament.TournamentName}");
        Console.WriteLine("Nhấn Enter để giữ nguyên giá trị hiện tại\n");

        var name = UIHelper.ReadString($"Tên giải đấu ({currentTournament.TournamentName}): ", allowEmpty: true);
        name = string.IsNullOrEmpty(name) ? currentTournament.TournamentName : name;

        var description = UIHelper.ReadString($"Mô tả ({currentTournament.Description}): ", allowEmpty: true);
        description = string.IsNullOrEmpty(description) ? currentTournament.Description : description;

        return new TournamentUpdateDto
        {
            TournamentName = name,
            Description = description,
            Status = currentTournament.Status
        };
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
            int borderWidth = 120;
            int borderHeight = 25;
            int maxRows = 15;
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            if (tournaments == null || !tournaments.Any())
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Chưa có giải đấu nào trong hệ thống.".PadRight(width));
                Console.ResetColor();
                Console.SetCursorPosition(left, top + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
                Console.ReadKey(true);
                return;
            }
            // Header
            var header = string.Format("{0,-5} {1,-30} {2,-15} {3,-12} {4,-12} {5,-15}",
                "ID", "Tên giải đấu", "Trạng thái", "Ngày bắt đầu", "Ngày kết thúc", "Tổng giải thưởng");
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            // Kẻ ngang full border, không có ký tự đầu/cuối
            DrawSeparatorLine(left, top + 1, width);
            // Data rows
            int displayCount = Math.Min(tournaments.Count, maxRows);
            for (int i = 0; i < displayCount; i++)
            {
                var t = tournaments[i];
                Console.SetCursorPosition(left, top + 3 + i);
                var row = string.Format("{0,-5} {1,-30} {2,-15} {3,-12} {4,-12} {5,-15}",
                    t.TournamentId,
                    t.TournamentName.Length > 29 ? t.TournamentName.Substring(0, 29) : t.TournamentName,
                    t.Status,
                    t.StartDate.ToString("dd/MM/yyyy"),
                    t.EndDate.ToString("dd/MM/yyyy"),
                    t.PrizePool.ToString("N0"));
                Console.WriteLine(row.Length > width ? row.Substring(0, width) : row.PadRight(width));
            }
            Console.ResetColor();
            // Footer
            int footerY = top + 3 + maxRows;
            string totalInfo = $"Tổng cộng: {tournaments.Count} giải đấu";
            if (totalInfo.Length > width) totalInfo = totalInfo.Substring(0, width);
            Console.SetCursorPosition(left, footerY);
            Console.WriteLine(totalInfo.PadRight(width));
            Console.SetCursorPosition(left, footerY + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải danh sách giải đấu: {ex.Message}", true, 3000);
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
            ConsoleRenderingService.DrawBorder("DUYỆT ĐĂNG KÝ GIẢI ĐẤU", 120, 25);

            // Get pending tournament registrations using the new service method
            var pendingRegistrations = await _tournamentService.GetPendingRegistrationsAsync();
            
            if (!pendingRegistrations.Any())
            {
                ConsoleRenderingService.ShowNotification("Không có đăng ký nào đang chờ duyệt", ConsoleColor.Yellow);
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            int borderLeft = (Console.WindowWidth - 120) / 2;
            int borderTop = (Console.WindowHeight - 25) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{"ID",-5} {"Team",-20} {"Tournament",-25} {"Leader",-15} {"Date",-12} {"Members",-8}");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            DrawSeparatorLine(borderLeft + 2, borderTop + 3, 110);

            int currentRow = borderTop + 4;
            for (int i = 0; i < Math.Min(pendingRegistrations.Count, 15); i++)
            {
                var registration = pendingRegistrations[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                Console.ForegroundColor = ConsoleColor.Yellow;
                
                var tournamentName = registration.TournamentName.Length > 24 ? 
                    registration.TournamentName.Substring(0, 21) + "..." : registration.TournamentName;
                var teamName = registration.TeamName.Length > 19 ? 
                    registration.TeamName.Substring(0, 16) + "..." : registration.TeamName;
                var leaderName = registration.RegisteredByName.Length > 14 ? 
                    registration.RegisteredByName.Substring(0, 11) + "..." : registration.RegisteredByName;
                
                Console.WriteLine($"{registration.RegistrationId,-5} {teamName,-20} {tournamentName,-25} {leaderName,-15} {registration.RegistrationDate:dd/MM/yyyy,-12} {registration.TeamMemberCount,-8}");
            }

            Console.ResetColor();
            Console.SetCursorPosition(borderLeft + 2, borderTop + 21);
            Console.WriteLine($"Tổng cộng: {pendingRegistrations.Count} đăng ký chờ duyệt");

            Console.SetCursorPosition(borderLeft + 2, borderTop + 22);
            Console.Write("Nhập Registration ID cần duyệt (0 để quay lại): ");
            
            if (int.TryParse(Console.ReadLine(), out int registrationId) && registrationId > 0)
            {
                var selectedRegistration = pendingRegistrations.FirstOrDefault(r => r.RegistrationId == registrationId);
                if (selectedRegistration == null)
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Không tìm thấy đăng ký với ID này!", true, 2000);
                    return;
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 23);
                Console.Write($"Duyệt đăng ký của {selectedRegistration.TeamName} cho {selectedRegistration.TournamentName}? (y/n/r=reject): ");
                var confirm = Console.ReadLine()?.ToLower().Trim();

                if (confirm == "y" || confirm == "yes")
                {
                    bool success = await _tournamentService.ApproveRegistrationAsync(registrationId);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt đăng ký của {selectedRegistration.TeamName} thành công!", false, 2500);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Duyệt đăng ký thất bại! Vui lòng thử lại.", true, 2500);
                    }
                }
                else if (confirm == "r" || confirm == "reject")
                {
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 24);
                    Console.Write("Nhập lý do từ chối: ");
                    var reason = Console.ReadLine()?.Trim() ?? "Không đạt yêu cầu";
                    
                    bool rejected = await _tournamentService.RejectRegistrationAsync(registrationId);
                    if (rejected)
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Đã từ chối đăng ký của {selectedRegistration.TeamName}!", false, 2500);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Từ chối đăng ký thất bại! Vui lòng thử lại.", true, 2500);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác", false, 1000);
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi duyệt đăng ký: {ex.Message}", true, 3000);
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

    private string CenterText(string text, int width)
    {
        if (string.IsNullOrEmpty(text)) return new string(' ', width);
        int padding = width - text.Length;
        int padLeft = padding / 2;
        int padRight = padding - padLeft;
        return new string(' ', padLeft) + text + new string(' ', padRight);
    }

    private void DisplayTournamentsTable(IEnumerable<TournamentInfoDto> tournaments)
    {
        // Lấy vị trí content của border ngoài
        int borderWidth = 120;
        int borderHeight = 25;
        var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
        // Định nghĩa format với độ rộng phù hợp cho ngày dd/MM/yyyy (10 ký tự)
        string format = "{0,-4} {1,-28} {2,-14} {3,-12} {4,-12} {5,-18}";
        var header = string.Format(format,
            "ID", "Tên giải đấu", "Trạng thái", "Ngày bắt đầu", "Ngày kết thúc", "Tổng giải thưởng");
        int width = header.Length;
        Console.SetCursorPosition(left, Console.CursorTop + 1);
        Console.WriteLine(header);
        DrawSeparatorLine(left, Console.CursorTop, width);

        foreach (var t in tournaments)
        {
            var tournamentName = t.TournamentName.Length > 28 ? t.TournamentName.Substring(0, 28) : t.TournamentName;
            var row = string.Format(format,
                t.TournamentId,
                tournamentName,
                t.Status,
                t.StartDate.ToString("dd/MM/yyyy"),
                t.EndDate.ToString("dd/MM/yyyy"),
                t.PrizePool.ToString("N0"));
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(row);
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

            // Get real team data from service
            var teams = await _teamService.GetAllTeamsAsync();

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("📋 Danh sách tất cả đội trong hệ thống:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
            DrawSeparatorLine(borderLeft + 2, borderTop + 3, 70);

            if (!teams.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Chưa có đội nào trong hệ thống.");
                Console.ResetColor();
            }
            else
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.WriteLine($"{"ID",-3} {"Tên đội",-18} {"Leader",-12} {"Members",-8} {"Created",-12} {"Status",-8}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
                DrawSeparatorLine(borderLeft + 2, borderTop + 5, 70);

                int currentRow = borderTop + 6;
                for (int i = 0; i < Math.Min(teams.Count, 8); i++)
                {
                    var team = teams[i];
                    Console.SetCursorPosition(borderLeft + 2, currentRow + i);

                    Console.ForegroundColor = team.Status == "Active" ? ConsoleColor.Green : ConsoleColor.Yellow;
                    var createdDate = team.CreatedAt.ToString("dd/MM/yyyy");
                    Console.WriteLine($"{team.Id,-3} {team.Name,-18} {team.LeaderName,-12} {team.MemberCount,-8} {createdDate,-12} {team.Status,-8}");
                    Console.ResetColor();
                }

                if (teams.Count > 8)
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow + 8);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"... và {teams.Count - 8} đội khác");
                    Console.ResetColor();
                }
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
            Console.WriteLine($"Tổng cộng: {teams.Count} đội");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải danh sách đội: {ex.Message}", true, 3000);
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

            // Use real team search service
            var searchResults = await _teamService.SearchTeamsAsync(searchTerm);

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine($"📊 Kết quả tìm kiếm cho: '{searchTerm}'");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            DrawSeparatorLine(borderLeft + 2, borderTop + 5, 70);

            if (!searchResults.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không tìm thấy đội nào phù hợp.");
                Console.ResetColor();
            }
            else
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine($"{"ID",-3} {"Tên đội",-18} {"Leader",-12} {"Members",-8} {"Created",-12} {"Status",-8}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
                DrawSeparatorLine(borderLeft + 2, borderTop + 7, 70);

                int currentRow = borderTop + 8;
                for (int i = 0; i < Math.Min(searchResults.Count, 6); i++)
                {
                    var team = searchResults[i];
                    Console.SetCursorPosition(borderLeft + 2, currentRow + i);
                    Console.ForegroundColor = team.Status == "Active" ? ConsoleColor.Green : ConsoleColor.Yellow;
                    var createdDate = team.CreatedAt.ToString("dd/MM/yyyy");
                    Console.WriteLine($"{team.Id,-3} {team.Name,-18} {team.LeaderName,-12} {team.MemberCount,-8} {createdDate,-12} {team.Status,-8}");
                    Console.ResetColor();
                }

                if (searchResults.Count > 6)
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow + 6);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"... và {searchResults.Count - 6} đội khác");
                    Console.ResetColor();
                }

                Console.SetCursorPosition(borderLeft + 2, borderTop + 16);
                Console.WriteLine($"Tìm thấy: {searchResults.Count} đội");
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi tìm kiếm: {ex.Message}", true, 3000);
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

            // Get real pending teams from service
            var pendingTeams = await _teamService.GetPendingTeamsAsync();

            if (!pendingTeams.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Hiện tại không có đội nào đang chờ phê duyệt.");
                Console.ResetColor();
                
                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Đội chờ phê duyệt:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            DrawSeparatorLine(borderLeft + 2, borderTop + 5, 70);

            int currentRow = borderTop + 6;
            for (int i = 0; i < Math.Min(pendingTeams.Count, 5); i++)
            {
                var team = pendingTeams[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 2);
                Console.WriteLine($"{i + 1}. {team.Name}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 2 + 1);
                var createdDate = team.CreatedAt.ToString("dd/MM/yyyy");
                Console.WriteLine($"   👤 Leader: {team.LeaderName} | 👥 {team.MemberCount} members | 📅 {createdDate}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(pendingTeams.Count, 5) * 2 + 1);
            Console.Write($"Chọn đội để duyệt (1-{Math.Min(pendingTeams.Count, 5)}, 0=thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Math.Min(pendingTeams.Count, 5))
            {
                var selectedTeam = pendingTeams[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(pendingTeams.Count, 5) * 2 + 3);
                Console.Write($"Duyệt đội '{selectedTeam.Name}'? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    bool success = await _teamService.ApproveTeamAsync(selectedTeam.Id);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt đội '{selectedTeam.Name}' thành công!", false, 2500);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Không thể duyệt đội '{selectedTeam.Name}'. Vui lòng thử lại.", true, 2500);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy", false, 1000);
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi duyệt đội: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Phê duyệt thành viên đội
    /// </summary>
    /// <summary>
    /// Phê duyệt thành viên gia nhập đội
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

            // Get real join requests from service
            var joinRequests = await _teamService.GetPendingTeamJoinRequestsAsync();

            if (!joinRequests.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Hiện tại không có yêu cầu gia nhập đội nào đang chờ phê duyệt.");
                Console.ResetColor();
                
                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Yêu cầu gia nhập đội:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            DrawSeparatorLine(borderLeft + 2, borderTop + 5, 70);

            int currentRow = borderTop + 6;
            for (int i = 0; i < Math.Min(joinRequests.Count, 4); i++)
            {
                var req = joinRequests[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 3);
                Console.WriteLine($"{i + 1}. {req.PlayerName} → {req.TeamName}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 1);
                var requestDate = req.RequestDate.ToString("dd/MM/yyyy");
                Console.WriteLine($"   📅 Ngày yêu cầu: {requestDate}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 2);
                Console.WriteLine($"   💬 Lời nhắn: {req.Message ?? "Không có"}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(joinRequests.Count, 4) * 3 + 1);
            Console.Write($"Chọn yêu cầu để duyệt (1-{Math.Min(joinRequests.Count, 4)}, 0=thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Math.Min(joinRequests.Count, 4))
            {
                var selectedReq = joinRequests[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(joinRequests.Count, 4) * 3 + 3);
                Console.Write($"Duyệt {selectedReq.PlayerName} gia nhập {selectedReq.TeamName}? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    bool success = await _teamService.ApproveTeamJoinRequestAsync(selectedReq.RequestId);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt {selectedReq.PlayerName} gia nhập đội {selectedReq.TeamName}!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Không thể duyệt yêu cầu. Vui lòng thử lại.", true, 2500);
                    }
                }
                else
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow + Math.Min(joinRequests.Count, 4) * 3 + 5);
                    Console.Write("Từ chối yêu cầu này? (y/n): ");
                    var rejectConfirm = Console.ReadLine()?.ToLower();
                    
                    if (rejectConfirm == "y" || rejectConfirm == "yes")
                    {
                        bool rejected = await _teamService.RejectTeamJoinRequestAsync(selectedReq.RequestId);
                        if (rejected)
                        {
                            ConsoleRenderingService.ShowMessageBox($"❌ Đã từ chối yêu cầu của {selectedReq.PlayerName}", false, 2000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác", false, 1000);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi duyệt thành viên: {ex.Message}", true, 3000);
        }
    }

    private static void DrawSeparatorLine(int left, int top, int width)
    {
        Console.SetCursorPosition(left, top);
        Console.WriteLine(new string('─', width));
    }

    /// <summary>
    /// Hiển thị prompt "Nhấn phím bất kỳ để tiếp tục..." ở dòng cuối cùng ngoài border, an toàn cho mọi kích thước console.
    /// </summary>
    private static void ShowContinuePromptOutsideBorder()
    {
        int lastLine = Math.Max(Console.WindowTop + Console.WindowHeight - 2, 0);
        Console.SetCursorPosition(0, lastLine);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }
}
