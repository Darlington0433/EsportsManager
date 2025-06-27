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
                            "Rời khỏi team",
                            "Xem danh sách thành viên",
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
                                await LeaveTeamInteractiveAsync(myTeam);
                                break;
                            case 2:
                                await ShowTeamMembersAsync(myTeam.Id);
                                break;
                            case -1:
                            case 3:
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

            Console.Write("Tên team: ");
            string teamName = Console.ReadLine() ?? "";

            Console.Write("Mô tả team: ");
            string description = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(teamName))
            {
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
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task JoinTeamInteractiveAsync()
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("THAM GIA TEAM", 80, 15);

                Console.Write("Nhập tên team muốn tham gia: ");
                string teamName = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(teamName))
                {
                    ConsoleRenderingService.ShowMessageBox("Tên team không được rỗng!", true, 2000);
                    return;
                }

                // Search for team by name
                var teams = await _teamService.SearchTeamsAsync(teamName);
                if (!teams.Any())
                {
                    ConsoleRenderingService.ShowMessageBox("Không tìm thấy team nào với tên này!", true, 2000);
                    return;
                }

                Console.WriteLine("\nKết quả tìm kiếm:");
                for (int i = 0; i < teams.Count(); i++)
                {
                    var team = teams.ElementAt(i);
                    Console.WriteLine($"{i + 1}. {team.Name} - {team.Description} ({team.MemberCount} thành viên)");
                }

                Console.Write($"\nChọn team (1-{teams.Count()}): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= teams.Count())
                {
                    var selectedTeam = teams.ElementAt(choice - 1);
                    var result = await _teamService.RequestToJoinTeamAsync(selectedTeam.Id, _currentUser.Id);
                    
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
                    ConsoleRenderingService.ShowNotification("Chưa có team nào trong hệ thống", ConsoleColor.Yellow);
                    return;
                }

                int borderLeft = (Console.WindowWidth - 100) / 2;
                int borderTop = (Console.WindowHeight - 20) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"ID",-5} {"Tên Team",-25} {"Mô tả",-30} {"Thành viên",-12} {"Trạng thái",-10}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine(new string('─', 85));

                int currentRow = borderTop + 4;
                foreach (var team in teams.Take(12))
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow);
                    Console.ForegroundColor = ConsoleColor.Green;
                    var row = string.Format("{0,-5} {1,-25} {2,-30} {3,-12} {4,-10}",
                        team.Id,
                        DisplayFormattingService.FormatTeamNameForDisplay(team.Name),
                        DisplayFormattingService.FormatTeamDescriptionForDisplay(team.Description),
                        $"{team.MemberCount}/{team.MaxMembers}",
                        team.Status);
                    Console.WriteLine(row);
                    currentRow++;
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 17);
                Console.WriteLine($"Tổng cộng: {teams.Count()} team");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 18);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
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

                string[] teamInfo = {
                    $"📝 Tên team: {team.Name}",
                    $"📄 Mô tả: {team.Description ?? "Không có mô tả"}",
                    $"📅 Ngày tạo: {team.CreatedAt:dd/MM/yyyy HH:mm}",
                    $"👥 Số thành viên: {team.MemberCount}/{team.MaxMembers}",
                    $"🏆 Trạng thái: {team.Status}",
                    "",
                    "📋 Danh sách thành viên:"
                };

                ConsoleRenderingService.WriteMultipleInBorder(teamInfo, left, top, 0);

                // Show team members
                var members = await _teamService.GetTeamMembersAsync(team.Id);
                int memberRow = 8;
                foreach (var member in members.Take(8))
                {
                    Console.SetCursorPosition(left + 2, top + memberRow);
                    var roleText = member.Role == "Leader" ? "👑 Leader" : "👤 Member";
                    var statusText = member.Status == "Active" ? "✅" : "⏳";
                    Console.WriteLine($"{statusText} {member.Username} - {roleText}");
                    memberRow++;
                }

                Console.SetCursorPosition(left, top + 17);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }

        private async Task LeaveTeamInteractiveAsync(TeamInfoDto team)
        {
            try
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("RỜI KHỎI TEAM", 80, 12);

                Console.WriteLine($"⚠️  Bạn có chắc chắn muốn rời khỏi team '{team.Name}' không?");
                Console.WriteLine("Hành động này không thể hoàn tác.");
                Console.Write("\nXác nhận rời team (YES để xác nhận): ");

                var confirmation = Console.ReadLine()?.Trim();
                if (confirmation?.ToUpper() == "YES")
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
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Đã hủy thao tác rời team", false, 1000);
                }
            }
            catch (Exception ex)
            {
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
                    ConsoleRenderingService.ShowNotification("Team chưa có thành viên nào", ConsoleColor.Yellow);
                    return;
                }

                int borderLeft = (Console.WindowWidth - 80) / 2;
                int borderTop = (Console.WindowHeight - 15) / 4;

                Console.SetCursorPosition(borderLeft + 2, borderTop + 2);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{"Username",-20} {"Vai trò",-15} {"Ngày tham gia",-15} {"Trạng thái",-10}");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 3);
                Console.WriteLine(new string('─', 65));

                int currentRow = borderTop + 4;
                foreach (var member in members.Take(8))
                {
                    Console.SetCursorPosition(borderLeft + 2, currentRow);
                    Console.ForegroundColor = member.Role == "Leader" ? ConsoleColor.Yellow : ConsoleColor.Green;
                    var role = member.Role == "Leader" ? "👑 Leader" : "👤 Member";
                    var status = member.Status == "Active" ? "✅ Active" : "⏳ Pending";
                    Console.WriteLine($"{member.Username,-20} {role,-15} {member.JoinDate:dd/MM/yyyy,-15} {status,-10}");
                    currentRow++;
                }

                Console.ResetColor();
                Console.SetCursorPosition(borderLeft + 2, borderTop + 12);
                Console.WriteLine($"Tổng cộng: {members.Count()} thành viên");
                Console.SetCursorPosition(borderLeft + 2, borderTop + 13);
                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
            }
        }
    }
}
