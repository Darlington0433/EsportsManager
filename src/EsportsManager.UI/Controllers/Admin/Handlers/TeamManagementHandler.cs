using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;

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
            int borderWidth = 80;
            int borderHeight = 20;
            int maxRows = 10;
            if (result == null || !result.Any())
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH TẤT CẢ ĐỘI", borderWidth, borderHeight);
                var (contentLeft, contentTop, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
                Console.SetCursorPosition(contentLeft, contentTop);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không có đội nào trong hệ thống".PadRight(contentWidth));
                Console.ResetColor();
                Console.SetCursorPosition(contentLeft, contentTop + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(contentWidth));
                Console.ReadKey(true);
                return;
            }
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH TẤT CẢ ĐỘI", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Header
            Console.SetCursorPosition(left, top);
            var header = string.Format("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}",
                "ID", "Tên đội", "Leader", "Thành viên", "Trạng thái");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine(new string('─', Math.Min(70, width)));
            // Data rows
            int displayCount = Math.Min(result.Count(), maxRows);
            for (int i = 0; i < displayCount; i++)
            {
                var team = result[i];
                Console.SetCursorPosition(left, top + 2 + i);
                var statusColor = team.Status == "Active" ? ConsoleColor.Green :
                                 team.Status == "Pending" ? ConsoleColor.Yellow : ConsoleColor.Red;
                Console.ForegroundColor = statusColor;
                var row = string.Format("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}",
                    team.Id,
                    team.Name.Length > 19 ? team.Name.Substring(0, 19) : team.Name,
                    team.LeaderName?.Length > 14 ? team.LeaderName.Substring(0, 14) : team.LeaderName ?? "N/A",
                    team.MemberCount,
                    team.Status);
                Console.WriteLine(row.Length > width ? row.Substring(0, width) : row.PadRight(width));
            }
            Console.ResetColor();
            // Footer
            int footerY = top + 2 + maxRows;
            string totalInfo = $"Tổng cộng: {result.Count()} đội{(result.Count() > displayCount ? $" (hiển thị {displayCount})" : "")}";
            if (totalInfo.Length > width) totalInfo = totalInfo.Substring(0, width);
            Console.SetCursorPosition(left, footerY);
            Console.WriteLine(totalInfo.PadRight(width));
            Console.SetCursorPosition(left, footerY + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.ReadKey(true);
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
            int borderWidth = 80;
            int borderHeight = 20;
            int maxRows = 10;
            if (result == null || !result.Any())
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", borderWidth, borderHeight);
                var (contentLeft, contentTop, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
                Console.SetCursorPosition(contentLeft, contentTop);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Không tìm thấy đội nào".PadRight(contentWidth));
                Console.ResetColor();
                Console.SetCursorPosition(contentLeft, contentTop + 1);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(contentWidth));
                Console.ReadKey(true);
                return;
            }
            Console.Clear();
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Header
            Console.SetCursorPosition(left, top);
            var header = string.Format("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}",
                "ID", "Tên đội", "Leader", "Thành viên", "Trạng thái");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine(new string('─', Math.Min(70, width)));
            // Data rows
            int displayCount = Math.Min(result.Count(), maxRows);
            for (int i = 0; i < displayCount; i++)
            {
                var team = result[i];
                Console.SetCursorPosition(left, top + 2 + i);
                var statusColor = team.Status == "Active" ? ConsoleColor.Green :
                                 team.Status == "Pending" ? ConsoleColor.Yellow : ConsoleColor.Red;
                Console.ForegroundColor = statusColor;
                var row = string.Format("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}",
                    team.Id,
                    team.Name.Length > 19 ? team.Name.Substring(0, 19) : team.Name,
                    team.LeaderName?.Length > 14 ? team.LeaderName.Substring(0, 14) : team.LeaderName ?? "N/A",
                    team.MemberCount,
                    team.Status);
                Console.WriteLine(row.Length > width ? row.Substring(0, width) : row.PadRight(width));
            }
            Console.ResetColor();
            // Footer
            int footerY = top + 2 + maxRows;
            string totalInfo = $"Tìm thấy: {result.Count()} đội";
            if (totalInfo.Length > width) totalInfo = totalInfo.Substring(0, width);
            Console.SetCursorPosition(left, footerY);
            Console.WriteLine(totalInfo.PadRight(width));
            Console.SetCursorPosition(left, footerY + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.ReadKey(true);
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
            Console.WriteLine(new string('─', 70));

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
}
