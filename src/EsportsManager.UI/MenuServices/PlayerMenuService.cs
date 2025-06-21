using System;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;

namespace EsportsManager.UI.MenuServices;

/// <summary>
/// PlayerMenuService - Xử lý UI menu cho Player
/// </summary>
public class PlayerMenuService
{
    private readonly PlayerController _playerController;

    public PlayerMenuService(PlayerController playerController)
    {
        _playerController = playerController ?? throw new ArgumentNullException(nameof(playerController));
    }    /// <summary>
    /// Hiển thị menu Player
    /// </summary>
    public void ShowPlayerMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Hiển thị menu chính của Player
    /// </summary>
    public void ShowMainMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "🏆 Đăng ký tham gia giải đấu",
                "👥 Quản lý team",
                "👤 Xem thông tin cá nhân",
                "✏️ Cập nhật thông tin cá nhân",
                "🔑 Thay đổi mật khẩu", 
                "📋 Xem danh sách giải đấu",
                "📝 Gửi feedback giải đấu",
                "🚪 Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("PLAYER CONTROL PANEL", menuOptions);

            switch (selection)
            {
                case 0:
                    ShowTournamentRegistration();
                    break;
                case 1:
                    ShowTeamManagement();
                    break;
                case 2:
                    ShowPersonalInfo();
                    break;
                case 3:
                    ShowUpdatePersonalInfo();
                    break;
                case 4:
                    ShowChangePassword();
                    break;
                case 5:
                    ShowTournamentList();
                    break;
                case 6:
                    ShowSendFeedback();
                    break;
                case 7:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    /// <summary>
    /// Hiển thị thông tin cá nhân
    /// </summary>
    private void ShowPersonalInfo()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thông tin cá nhân...");
            
            var userInfo = _playerController.GetPersonalInfoAsync().GetAwaiter().GetResult();
            
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÔNG TIN CÁ NHÂN", 80, 15);
            
            Console.WriteLine($"👤 ID: {userInfo.Id}");
            Console.WriteLine($"📧 Username: {userInfo.Username}");
            Console.WriteLine($"✉️ Email: {userInfo.Email ?? "Chưa cập nhật"}");
            Console.WriteLine($"🎭 Role: {userInfo.Role}");
            Console.WriteLine($"📅 Ngày tạo: {userInfo.CreatedAt:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"🕐 Lần đăng nhập cuối: {userInfo.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");
            Console.WriteLine($"🔢 Tổng số lần đăng nhập: {userInfo.TotalLogins}");
            Console.WriteLine($"⏱️ Tổng thời gian online: {userInfo.TotalTimeOnline.TotalHours:F1} giờ");
            
            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Đăng ký tham gia giải đấu
    /// </summary>
    private void ShowTournamentRegistration()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách giải đấu...");
            
            var tournaments = _playerController.GetAvailableTournamentsAsync().GetAwaiter().GetResult();
            
            Console.Clear();
            ConsoleRenderingService.DrawBorder("ĐĂNG KÝ THAM GIA GIẢI ĐẤU", 100, 20);
            
            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào mở đăng ký", false, 2000);
                return;
            }

            Console.WriteLine("Danh sách giải đấu có thể tham gia:");
            Console.WriteLine(new string('=', 90));
            
            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"{i + 1}. {tournament.Name}");
                Console.WriteLine($"   📝 {tournament.Description}");
                Console.WriteLine($"   📅 Ngày bắt đầu: {tournament.StartDate:dd/MM/yyyy}");
                Console.WriteLine($"   💰 Phí tham gia: {tournament.EntryFee:N0} VND");
                Console.WriteLine($"   👥 Đã đăng ký: {tournament.CurrentParticipants}/{tournament.MaxParticipants}");
                Console.WriteLine();
            }
            
            Console.Write("Nhập số thứ tự giải đấu muốn đăng ký (0 để hủy): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= tournaments.Count)
            {
                var selectedTournament = tournaments[choice - 1];
                ConsoleRenderingService.ShowLoadingMessage("Đang đăng ký...");
                
                bool success = _playerController.RegisterForTournamentAsync(selectedTournament.Id).GetAwaiter().GetResult();
                
                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox($"Đăng ký thành công giải đấu: {selectedTournament.Name}", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Đăng ký thất bại!", true, 2000);
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Quản lý team
    /// </summary>
    private void ShowTeamManagement()
    {
        try
        {
            var myTeam = _playerController.GetMyTeamAsync().GetAwaiter().GetResult();
            
            if (myTeam == null)
            {
                // Player chưa có team - hiển thị option tạo team
                ShowCreateTeam();
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

    /// <summary>
    /// Tạo team mới
    /// </summary>
    private void ShowCreateTeam()
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
            
            ConsoleRenderingService.ShowLoadingMessage("Đang tạo team...");
            
            bool success = _playerController.CreateTeamAsync(teamDto).GetAwaiter().GetResult();
            
            if (success)
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

    /// <summary>
    /// Hiển thị thông tin team
    /// </summary>
    private void ShowTeamInfo(TeamInfoDto team)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder($"TEAM: {team.Name}", 100, 20);
        
        Console.WriteLine($"📝 Mô tả: {team.Description}");
        Console.WriteLine($"📅 Ngày tạo: {team.CreatedAt:dd/MM/yyyy}");
        Console.WriteLine($"👥 Số thành viên: {team.Members.Count}");
        
        if (team.Members.Count > 0)
        {
            Console.WriteLine("\nDanh sách thành viên:");
            Console.WriteLine(new string('-', 50));
            foreach (var member in team.Members)
            {
                Console.WriteLine($"• {member.Username} ({member.Role}) - Tham gia: {member.JoinedAt:dd/MM/yyyy}");
            }
        }
        
        ConsoleRenderingService.PauseWithMessage();
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân
    /// </summary>
    private void ShowUpdatePersonalInfo()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Thay đổi mật khẩu
    /// </summary>
    private void ShowChangePassword()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng thay đổi mật khẩu đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Xem danh sách giải đấu
    /// </summary>
    private void ShowTournamentList()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem danh sách giải đấu đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Gửi feedback
    /// </summary>
    private void ShowSendFeedback()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng gửi feedback đang được phát triển", false, 2000);
    }
}
