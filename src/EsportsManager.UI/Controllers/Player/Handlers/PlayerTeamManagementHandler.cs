using System;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
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
            try
            {
                var myTeam = await _teamService.GetPlayerTeamAsync(_currentUser.Id);

                if (myTeam == null)
                {
                    // Player chưa có team - hiển thị option tạo team
                    await CreateTeamInteractiveAsync();
                }
                else
                {
                    // Player đã có team - hiển thị thông tin team
                    ShowTeamInfo(myTeam);
                }
            }
            catch (Exception ex)
            {
                ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
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

        private void ShowTeamInfo(TeamInfoDto team)
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder($"TEAM: {team.Name}", 100, 20);
            Console.WriteLine($"📝 Mô tả: {team.Description}");
            Console.WriteLine($"📅 Ngày tạo: {team.CreatedAt:dd/MM/yyyy}");
            Console.WriteLine($"👥 Số thành viên: {team.MemberCount}/{team.MaxMembers}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
