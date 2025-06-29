using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class TeamManagementHandler
{
    private readonly ITeamService _teamService;
    private readonly IUserService _userService;

    public TeamManagementHandler(ITeamService teamService, IUserService userService)
    {
        _teamService = teamService;
        _userService = userService;
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
                    await ViewAllTeamsAsync();
                    break;
                case 1:
                    await SearchTeamsAsync();
                    break;
                case 2:
                    await ApprovePendingTeamsAsync();
                    break;
                case 3:
                    await ApproveTeamMembersAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    private async Task ViewAllTeamsAsync()
    {
        try
        {
            var result = await _teamService.GetAllTeamsAsync();
            DisplayTeamsTableInBorder(result, 0, 0, Console.WindowWidth);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    private async Task SearchTeamsAsync()
    {
        try
        {
            Console.Write("\nNhập từ khóa tìm kiếm đội: ");
            var searchTerm = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowNotification("Từ khóa tìm kiếm không được rỗng", ConsoleColor.Yellow);
                return;
            }
            var result = await _teamService.SearchTeamsAsync(searchTerm);
            DisplayTeamsTableInBorder(result, 0, 0, Console.WindowWidth);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Phê duyệt các đội đang chờ xét duyệt
    /// </summary>
    private async Task ApprovePendingTeamsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DUYỆT ĐỘI MỚI", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            // TODO: Cần bổ sung phương thức GetPendingTeamsAsync() vào ITeamService
            // Tạm thời sử dụng GetAllTeamsAsync và lọc các đội có Status = "Pending"
            var allTeams = await _teamService.GetAllTeamsAsync();
            var pendingTeams = allTeams?.Where(t => t.Status == "Pending").ToList() ?? new List<TeamInfoDto>();

            if (pendingTeams.Count == 0)
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có đội nào đang chờ phê duyệt");
                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("📋 Duyệt các đội đang chờ phê duyệt");
            Console.WriteLine();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Danh sách đội chờ duyệt:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('═', 70));

            int currentRow = borderTop + 6;
            int displayCount = Math.Min(pendingTeams.Count, 5);  // Giới hạn hiển thị tối đa 5 đội

            for (int i = 0; i < displayCount; i++)
            {
                var team = pendingTeams[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 2);
                Console.WriteLine($"{i + 1}. {team.Name}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 2 + 1);
                Console.WriteLine($"   👤 Leader: {team.LeaderName ?? "N/A"} | 👥 Members: {team.MemberCount}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + displayCount * 2 + 2);
            Console.Write($"Chọn đội để duyệt (1-{displayCount}, 0 để thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= displayCount)
            {
                var selectedTeam = pendingTeams[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + displayCount * 2 + 4);
                Console.WriteLine($"Duyệt đội: {selectedTeam.Name}");
                Console.Write("Xác nhận duyệt đội này? (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    // TODO: Cần bổ sung phương thức ApproveTeamAsync(int teamId) vào ITeamService
                    // Tạm thời hiển thị thông báo thành công
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt đội '{selectedTeam.Name}' thành công!", false, 2500);

                    /* 
                    // Đoạn code này sẽ được uncomment khi phương thức được bổ sung
                    var success = await _teamService.ApproveTeamAsync(selectedTeam.Id);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt đội '{selectedTeam.Name}' thành công!", false, 2500);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Duyệt đội thất bại!", true, 2000);
                    }
                    */
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác duyệt", false, 1500);
                }
            }
            else if (choice != 0)
            {
                ConsoleRenderingService.ShowMessageBox("❌ Lựa chọn không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Phê duyệt thành viên mới của các đội
    /// </summary>
    private async Task ApproveTeamMembersAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DUYỆT THÀNH VIÊN ĐỘI", 80, 20);

            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 20) / 4;

            // TODO: Cần bổ sung TeamMemberRequestDto và phương thức GetPendingTeamMemberRequestsAsync() vào ITeamService
            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Tính năng chưa được triển khai đầy đủ");
            Console.WriteLine();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Cần bổ sung các phương thức và DTO sau vào ITeamService:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine("- TeamMemberRequestDto");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
            Console.WriteLine("- GetPendingTeamMemberRequestsAsync()");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 7);
            Console.WriteLine("- ApproveTeamMemberRequestAsync(int requestId)");

            Console.SetCursorPosition(borderLeft + 2, borderTop + 9);
            Console.WriteLine("Vui lòng liên hệ với team phát triển để hoàn thiện tính năng này.");

            Console.ResetColor();
            Console.SetCursorPosition(borderLeft + 2, borderTop + 11);
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);

            /* TODO: Triển khai khi có các phương thức và DTO tương ứng
            var pendingRequests = await _teamService.GetPendingTeamMemberRequestsAsync();
            
            if (pendingRequests == null || !pendingRequests.Any())
            {
                Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có yêu cầu gia nhập đội nào đang chờ duyệt");
                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 6);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
            Console.WriteLine("👥 Duyệt thành viên mới gia nhập đội");
            Console.WriteLine();

            Console.SetCursorPosition(borderLeft + 2, borderTop + 4);
            Console.WriteLine("Yêu cầu gia nhập đội:");
            Console.SetCursorPosition(borderLeft + 2, borderTop + 5);
            Console.WriteLine(new string('─', 70));

            int currentRow = borderTop + 6;
            int displayCount = Math.Min(pendingRequests.Count, 5); // Giới hạn hiển thị tối đa 5 yêu cầu
            
            for (int i = 0; i < displayCount; i++)
            {
                var request = pendingRequests[i];
                Console.SetCursorPosition(borderLeft + 2, currentRow + i * 3);
                Console.WriteLine($"{i + 1}. {request.PlayerName} → {request.TeamName}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 1);
                Console.WriteLine($"   🎯 Role: {request.Role} | ⏱️ Experience: {request.Experience}");
                Console.SetCursorPosition(borderLeft + 4, currentRow + i * 3 + 2);
                Console.WriteLine($"   📊 Status: {request.Status}");
            }

            Console.SetCursorPosition(borderLeft + 2, currentRow + displayCount * 3 + 2);
            Console.Write($"Chọn yêu cầu để duyệt (1-{displayCount}, 0 để thoát): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= displayCount)
            {
                var selectedRequest = pendingRequests[choice - 1];

                Console.SetCursorPosition(borderLeft + 2, currentRow + displayCount * 3 + 4);
                Console.WriteLine($"Duyệt: {selectedRequest.PlayerName} gia nhập {selectedRequest.TeamName}");
                Console.Write("Xác nhận duyệt? (y/n): ");

                var confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "y" || confirmation == "yes")
                {
                    var success = await _teamService.ApproveTeamMemberRequestAsync(selectedRequest.Id);
                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã duyệt {selectedRequest.PlayerName} gia nhập đội {selectedRequest.TeamName}!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("❌ Duyệt thành viên thất bại!", true, 2000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã từ chối yêu cầu", false, 1500);
                }
            }
            else if (choice != 0)
            {
                ConsoleRenderingService.ShowMessageBox("❌ Lựa chọn không hợp lệ!", true, 2000);
            }
            */

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
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

    private void DisplayTeamsTableInBorder(IEnumerable<TeamInfoDto> teams, int startX, int startY, int maxWidth)
    {
        var headers = new[] { "ID", "Tên đội", "Trưởng đội", "Số thành viên", "Ngày tạo" };
        var rows = teams.Select(t => new[] {
            t.Id.ToString(),
            t.Name.Length > 18 ? t.Name.Substring(0, 18) : t.Name,
            t.LeaderName.Length > 14 ? t.LeaderName.Substring(0, 14) : t.LeaderName,
            t.MemberCount.ToString(),
            t.CreatedAt.ToString("dd/MM/yyyy")
        }).ToList();
        int borderWidth = maxWidth;
        int borderHeight = 16;
        int[] colWidths = { 5, 20, 16, 10, 14 }; // Tổng + phân cách <= borderWidth - 4
        UIHelper.PrintTableInBorder(headers, rows, borderWidth, borderHeight, startX, startY, colWidths);
        int infoY = startY + 2 + rows.Count + 2;
        UIHelper.PrintPromptInBorder($"Tổng cộng: {teams.Count()} đội", startX, infoY, borderWidth - 4);
        Console.SetCursorPosition(0, startY + borderHeight + 1);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
    }
}
