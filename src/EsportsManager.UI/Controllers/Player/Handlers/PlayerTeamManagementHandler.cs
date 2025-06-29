using System;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.BL.Services;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.Controllers.Player.Handlers
{
    /// <summary>
    /// Handler cho quản lý team của player
    /// Single Responsibility: Chỉ lo việc quản lý team
    /// </summary>
    public class PlayerTeamManagementHandler
    {
        private readonly UserProfileDto _currentUser;
        private readonly ITeamService _teamService;

        public PlayerTeamManagementHandler(
            UserProfileDto currentUser,
            ITeamService teamService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
        }

        public async Task HandleTeamManagementAsync()
        {
            while (true)
            {
                try
                {
                    var myTeam = await _teamService.GetPlayerTeamAsync(_currentUser.Id);

                    string[] menuOptions;
                    if (myTeam == null)
                    {
                        // Player chưa có team
                        menuOptions = new[]
                        {
                            "Tạo team mới",
                            "Tìm kiếm và tham gia team",
                            "Xem danh sách các team",
                            "⬅️ Quay lại"
                        };
                    }
                    else
                    {
                        // Player đã có team
                        menuOptions = new[]
                        {
                            "Xem thông tin team hiện tại",
                            "Xem danh sách thành viên",
                            "Rời khỏi team",
                            "Xem danh sách các team khác",
                            "⬅️ Quay lại"
                        };
                    }

                    int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ TEAM", menuOptions);

                    if (myTeam == null)
                    {
                        switch (selection)
                        {
                            case 0:
                                await CreateTeamInteractiveAsync();
                                break;
                            case 1:
                                await JoinTeamInteractiveAsync();
                                break;
                            case 2:
                                await ViewAllTeamsAsync();
                                break;
                            case -1:
                            case 3:
                                return;
                        }
                    }
                    else
                    {
                        switch (selection)
                        {
                            case 0:
                                await ShowDetailedTeamInfoAsync(myTeam);
                                break;
                            case 1:
                                await ShowTeamMembersAsync(myTeam.Id);
                                break;
                            case 2:
                                await LeaveTeamInteractiveAsync(myTeam);
                                break;
                            case 3:
                                await ViewAllTeamsAsync();
                                break;
                            case -1:
                            case 4:
                                return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
                    break;
                }
            }
        }

        private async Task CreateTeamInteractiveAsync()
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TẠO TEAM MỚI", 80, 12);
            int borderLeft = (Console.WindowWidth - 80) / 2;
            int borderTop = (Console.WindowHeight - 12) / 4;
            int cursorY = borderTop + 2;

            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.Write("Tên team: ");
            string teamName = Console.ReadLine() ?? "";
            Console.SetCursorPosition(borderLeft + 2, cursorY++);
            Console.Write("Mô tả team: ");
            string description = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(teamName))
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowMessageBox("Tên team không được rỗng!", true, 2000);
                return;
            }

            try
            {
                var teamDto = new TeamCreateDto
                {
                    Name = teamName,
                    Description = description
                };

                var team = await _teamService.CreateTeamAsync(teamDto, _currentUser.Id);

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                if (team != null)
                {
                    ConsoleRenderingService.ShowMessageBox($"Tạo team '{teamName}' thành công!", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Tạo team thất bại!", true, 2000);
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task JoinTeamInteractiveAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THAM GIA TEAM", 80, 15);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;
                int cursorY = borderTop + 2;

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Nhập tên team muốn tham gia: ");
                string teamName = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(teamName))
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("Tên team không được rỗng!", true, 2000);
                    return;
                }

                var teams = await _teamService.SearchTeamsAsync(teamName);
                if (!teams.Any())
                {
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy team nào với tên này!", true, 2000);
                    return;
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("Kết quả tìm kiếm:");
                for (int i = 0; i < teams.Count(); i++)
                {
                    var team = teams.ElementAt(i);
                    Console.SetCursorPosition(borderLeft + 4, cursorY++);
                    Console.WriteLine($"{i + 1}. {team.Name} - {team.Description} ({team.MemberCount} thành viên)");
                }

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write($"Chọn team (1-{teams.Count()}): ");
                Console.SetCursorPosition(borderLeft + 24, cursorY - 1);
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= teams.Count())
                {
                    var selectedTeam = teams.ElementAt(choice - 1);
                    var result = await _teamService.RequestToJoinTeamAsync(selectedTeam.Id, _currentUser.Id);
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    if (result)
                    {
                        ConsoleRenderingService.ShowMessageBox($"Đã gửi yêu cầu tham gia team '{selectedTeam.Name}' thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Gửi yêu cầu thất bại!", true, 2000);
                    }
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 13);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task ViewAllTeamsAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH CÁC TEAM", 100, 20);

                var teams = await _teamService.GetAllTeamsAsync();
                if (!teams.Any())
                {
                    int borderLeft = (Console.WindowWidth - 100) / 2;
                    int borderTop = (Console.WindowHeight - 20) / 4;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    ConsoleRenderingService.ShowNotification("Chưa có team nào trong hệ thống", ConsoleColor.Yellow);
                    return;
                }

                int borderLeft2 = (Console.WindowWidth - 100) / 2;
                int borderTop2 = (Console.WindowHeight - 20) / 4;
                int cursorY = borderTop2 + 2;

                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"ID",-5} {"Tên Team",-25} {"Mô tả",-30} {"Thành viên",-12} {"Trạng thái",-10}");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine(new string('═', 85));

                foreach (var team in teams)
                {
                    Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                    var row = string.Format("{0,-5} {1,-25} {2,-30} {3,-12} {4,-10}",
                        team.Id, team.Name, team.Description, team.MemberCount, team.Status);
                    Console.WriteLine(row);
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine($"Tổng cộng: {teams.Count()} team");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine("Nhấn Enter để tiếp tục...");
                Console.SetCursorPosition(borderLeft2 + 30, cursorY - 1);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 100) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task ShowDetailedTeamInfoAsync(TeamInfoDto team)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder($"THÔNG TIN TEAM: {team.Name}", 100, 20);
                var (left, top, contentWidth) = ConsoleRenderingService.GetBorderContentPosition(100, 20);
                int cursorY = top + 2;

                string[] teamInfo = {
                    $"📝 Tên team: {team.Name}",
                    $"📄 Mô tả: {team.Description ?? "Không có mô tả"}",
                    $"📅 Ngày tạo: {team.CreatedAt:dd/MM/yyyy HH:mm}",
                    $"👥 Số thành viên: {team.MemberCount}/{team.MaxMembers}",
                    $"🏆 Trạng thái: {team.Status}",
                    "",
                    "📋 Danh sách thành viên:"
                };
                foreach (var line in teamInfo)
                {
                    Console.SetCursorPosition(left + 2, cursorY++);
                    Console.WriteLine(line);
                }

                // Show team members
                var members = await _teamService.GetTeamMembersAsync(team.Id);
                foreach (var member in members.Take(8))
                {
                    Console.SetCursorPosition(left + 4, cursorY++);
                    var roleText = member.Role == "Leader" ? "👑 Leader" : "👤 Member";
                    var statusText = member.Status == "Active" ? "✅" : "⏳";
                    Console.WriteLine($"{statusText} {member.Username} - {roleText}");
                }

                Console.SetCursorPosition(left + 2, top + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                var (left, top, _) = ConsoleRenderingService.GetBorderContentPosition(100, 20);
                Console.SetCursorPosition(left + 2, top + 18);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task LeaveTeamInteractiveAsync(TeamInfoDto team)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("RỜI KHỎI TEAM", 80, 12);
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 12) / 4;
                int cursorY = borderTop + 2;

                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine($"⚠️  Bạn có chắc chắn muốn rời khỏi team '{team.Name}' không?");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.WriteLine("Hành động này không thể hoàn tác.");
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                Console.Write("Xác nhận rời team (YES để xác nhận): ");

                var confirmation = Console.ReadLine()?.Trim();
                Console.SetCursorPosition(borderLeft + 2, cursorY++);
                if (confirmation?.ToUpper() == "YES")
                {
                    // Lấy danh sách thành viên khác để chuyển giao leader
                    var members = await _teamService.GetTeamMembersAsync(team.Id);
                    var otherMembers = members.Where(m => m.UserId != _currentUser.Id && m.Status == "Active").ToList();

                    if (otherMembers.Any())
                    {
                        int memberListY = cursorY;
                        Console.SetCursorPosition(borderLeft + 2, memberListY++);
                        Console.WriteLine($"⚠️  Bạn là leader của team '{team.Name}'.");
                        Console.SetCursorPosition(borderLeft + 2, memberListY++);
                        Console.WriteLine("Để rời team, bạn cần chuyển giao quyền leader cho thành viên khác.");
                        Console.SetCursorPosition(borderLeft + 2, memberListY++);
                        Console.WriteLine("Danh sách thành viên có thể làm leader mới:");
                        for (int i = 0; i < otherMembers.Count; i++)
                        {
                            Console.SetCursorPosition(borderLeft + 4, memberListY++);
                            Console.WriteLine($"{i + 1}. {otherMembers[i].Username}");
                        }
                        Console.SetCursorPosition(borderLeft + 2, memberListY++);
                        Console.Write($"Chọn thành viên mới làm leader (1-{otherMembers.Count}) hoặc 0 để hủy: ");
                        Console.SetCursorPosition(borderLeft + 54, memberListY - 1);
                        if (int.TryParse(Console.ReadLine(), out int choice))
                        {
                            if (choice == 0)
                            {
                                ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác rời team", false, 1000);
                                return;
                            }
                            else if (choice >= 1 && choice <= otherMembers.Count)
                            {
                                var newLeader = otherMembers[choice - 1];

                                // Chuyển giao leader
                                try
                                {
                                    var transferResult = await _teamService.TransferLeadershipAsync(team.Id, _currentUser.Id, newLeader.UserId);
                                    if (transferResult)
                                    {
                                        // Sau khi chuyển giao thành công, rời team
                                        var leaveResult = await _teamService.RemoveMemberAsync(team.Id, _currentUser.Id, _currentUser.Id);
                                        if (leaveResult)
                                        {
                                            ConsoleRenderingService.ShowMessageBox($"Đã chuyển giao leader cho {newLeader.Username} và rời khỏi team '{team.Name}' thành công!", false, 3000);
                                        }
                                        else
                                        {
                                            ConsoleRenderingService.ShowMessageBox("Chuyển giao leader thành công nhưng rời team thất bại!", true, 3000);
                                        }
                                    }
                                    else
                                    {
                                        ConsoleRenderingService.ShowMessageBox("Chuyển giao leader thất bại!", true, 2000);
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    ConsoleRenderingService.ShowMessageBox($"Không có quyền: {ex.Message}", true, 3000);
                                }
                                catch (InvalidOperationException ex)
                                {
                                    ConsoleRenderingService.ShowMessageBox($"Thao tác không hợp lệ: {ex.Message}", true, 3000);
                                }
                                catch (Exception ex)
                                {
                                    ConsoleRenderingService.ShowMessageBox($"Lỗi chuyển giao leader: {ex.Message}", true, 3000);
                                }
                            }
                            else
                            {
                                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                            }
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                        }
                    }
                    else
                    {
                        int disbandY = cursorY;
                        Console.SetCursorPosition(borderLeft + 2, disbandY++);
                        Console.WriteLine($"⚠️  Bạn là leader duy nhất của team '{team.Name}'.");
                        Console.SetCursorPosition(borderLeft + 2, disbandY++);
                        Console.WriteLine("Bạn có thể giải tán team hoặc hủy thao tác.");
                        Console.SetCursorPosition(borderLeft + 2, disbandY++);
                        Console.Write("Nhập 'DISBAND' để giải tán team hoặc Enter để hủy: ");
                        Console.SetCursorPosition(borderLeft + 48, disbandY - 1);
                        var disbandConfirmation = Console.ReadLine()?.Trim();
                        if (disbandConfirmation?.ToUpper() == "DISBAND")
                        {
                            try
                            {
                                var disbandResult = await _teamService.DisbandTeamAsync(team.Id, _currentUser.Id);
                                if (disbandResult)
                                {
                                    ConsoleRenderingService.ShowMessageBox($"Đã giải tán team '{team.Name}' thành công!", false, 3000);
                                }
                                else
                                {
                                    ConsoleRenderingService.ShowMessageBox("Giải tán team thất bại!", true, 2000);
                                }
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                ConsoleRenderingService.ShowMessageBox($"Không có quyền giải tán team: {ex.Message}", true, 3000);
                            }
                            catch (Exception ex)
                            {
                                ConsoleRenderingService.ShowMessageBox($"Lỗi giải tán team: {ex.Message}", true, 3000);
                            }
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác", false, 1000);
                        }
                    }
                }
                else
                {
                    // Thành viên thường rời team
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine($"⚠️  Bạn có chắc chắn muốn rời khỏi team '{team.Name}' không?");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.WriteLine("Hành động này không thể hoàn tác.");
                    Console.SetCursorPosition(borderLeft + 2, cursorY++);
                    Console.Write("Xác nhận rời team (YES để xác nhận): ");
                    Console.SetCursorPosition(borderLeft + 38, cursorY - 1);
                    var leaveConfirmation = Console.ReadLine()?.Trim();
                    if (leaveConfirmation?.ToUpper() == "YES")
                    {
                        try
                        {
                            var result = await _teamService.RemoveMemberAsync(team.Id, _currentUser.Id, _currentUser.Id);
                            if (result)
                            {
                                ConsoleRenderingService.ShowMessageBox($"Đã rời khỏi team '{team.Name}' thành công!", false, 3000);
                            }
                            else
                            {
                                ConsoleRenderingService.ShowMessageBox("Rời team thất bại!", true, 2000);
                            }
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            ConsoleRenderingService.ShowMessageBox($"Không có quyền rời team: {ex.Message}", true, 3000);
                        }
                        catch (Exception ex)
                        {
                            ConsoleRenderingService.ShowMessageBox($"Lỗi rời team: {ex.Message}", true, 3000);
                        }
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác rời team", false, 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 12) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 10);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task ShowTeamMembersAsync(int teamId)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("DANH SÁCH THÀNH VIÊN", 80, 15);

                var members = await _teamService.GetTeamMembersAsync(teamId);
                if (!members.Any())
                {
                    int borderLeft = (Console.WindowWidth - 80) / 2;
                    int borderTop = (Console.WindowHeight - 15) / 4;
                    Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                    ConsoleRenderingService.ShowNotification("Team chưa có thành viên nào", ConsoleColor.Yellow);
                    return;
                }

                int borderLeft2 = (Console.WindowWidth - 80) / 2;
                int borderTop2 = (Console.WindowHeight - 15) / 4;
                int cursorY = borderTop2 + 2;

                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"Username",-20} {"Vai trò",-15} {"Ngày tham gia",-15} {"Trạng thái",-10}");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine(new string('═', 65));

                foreach (var member in members.Take(8))
                {
                    Console.SetCursorPosition(borderLeft2 + 2, cursorY);
                    Console.ForegroundColor = member.Role == "Leader" ? ConsoleColor.Yellow : ConsoleColor.Green;
                    var role = member.Role == "Leader" ? "👑 Leader" : "👤 Member";
                    var status = member.Status == "Active" ? "✅ Active" : "⏳ Pending";
                    Console.WriteLine($"{member.Username,-20} {role,-15} {member.JoinDate:dd/MM/yyyy,-15} {status,-10}");
                    cursorY++;
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine($"Tổng cộng: {members.Count()} thành viên");
                Console.SetCursorPosition(borderLeft2 + 2, cursorY++);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;
                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }
    }
}
