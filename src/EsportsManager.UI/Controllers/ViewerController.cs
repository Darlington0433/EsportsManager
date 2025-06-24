// Controller xử lý chức năng Viewer

using System;
using System.Threading.Tasks;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;

namespace EsportsManager.UI.Controllers;

public class ViewerController
{
    private readonly UserProfileDto _currentUser;
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;

    public ViewerController(UserProfileDto currentUser, IUserService userService, ITournamentService tournamentService)
    {
        _currentUser = currentUser;
        _userService = userService;
        _tournamentService = tournamentService;
    }

    public void ShowViewerMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Xem danh sách giải đấu",
                "Xem bảng xếp hạng giải đấu",
                "Vote cho Player/Tournament/Sport",
                "Donate cho Player",
                "Xem thông tin cá nhân",
                "Cập nhật thông tin cá nhân",
                "Quên mật khẩu",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU VIEWER - {_currentUser.Username}", menuOptions);            switch (selection)
            {
                case 0:
                    ViewTournamentList();
                    break;
                case 1:
                    ViewTournamentStandings();
                    break;
                case 2:
                    VoteForPlayerTournamentSport();
                    break;
                case 3:
                    DonateToPlayer();
                    break;
                case 4:
                    ViewPersonalInfo();
                    break;
                case 5:
                    UpdatePersonalInfo();
                    break;
                case 6:
                    ForgotPassword();
                    break;
                case 7:
                case -1:
                    return; // Đăng xuất
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }

    private void ViewTournamentList()
    {
        var tournaments = new[]
        {
            "Giải đấu LOL Mùa Xuân 2025",
            "CS:GO Championship 2025",
            "PUBG Mobile Tournament",
            "FIFA Online 4 Cup",
            "Valorant Masters"
        };

        InteractiveMenuService.DisplayInteractiveMenu("DANH SÁCH GIẢI ĐẤU", tournaments);
    }

    private void ViewTournamentStandings()
    {
        var standings = new[]
        {
            "1. Team Alpha - 150 điểm",
            "2. Team Beta - 120 điểm", 
            "3. Team Gamma - 100 điểm",
            "4. Team Delta - 80 điểm",
            "5. Team Epsilon - 60 điểm"
        };

        InteractiveMenuService.DisplayInteractiveMenu("BẢNG XẾP HẠNG", standings);
    }

    private void VoteForPlayerTournamentSport()
    {
        var voteOptions = new[]
        {
            "Vote cho Player yêu thích",
            "Vote cho Giải đấu hay nhất",
            "Vote cho Môn thể thao esports",
            "Xem kết quả voting"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("VOTING", voteOptions);        switch (selection)
        {
            case 0:
                Console.WriteLine("🗳️ Vote cho player sẽ được kết nối với database");
                ConsoleRenderingService.PauseWithMessage();
                break;
            case 1:
                Console.WriteLine("🏆 Vote cho tournament sẽ được kết nối với database");
                ConsoleRenderingService.PauseWithMessage();
                break;
            case 2:
                Console.WriteLine("🎮 Vote cho esports sẽ được kết nối với database");
                ConsoleRenderingService.PauseWithMessage();
                break;
            case 3:
                Console.WriteLine("📊 Xem kết quả voting sẽ được kết nối với database");
                ConsoleRenderingService.PauseWithMessage();
                break;
            case -1:
                return; // Quay lại menu chính
            default:
                Console.WriteLine("Lựa chọn không hợp lệ!");
                break;
        }
    }    private void DonateToPlayer()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 10);
        Console.WriteLine("💰 Chức năng donate cho player sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu player và wallet sẽ được lấy từ MySQL");
        Console.WriteLine("💡 UI form nhập số tiền và chọn player sẽ được triển khai");
        ConsoleRenderingService.PauseWithMessage();
    }private void ViewPersonalInfo()
    {
        var info = new[]
        {
            $"Username: {_currentUser.Username}",
            $"Email: {_currentUser.Email}",
            $"Role: {_currentUser.Role}",
            $"Ngày tạo: {_currentUser.CreatedAt:dd/MM/yyyy}",
            $"Lần đăng nhập cuối: {_currentUser.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập"}",
            $"Trạng thái: {_currentUser.Status}"
        };

        InteractiveMenuService.DisplayInteractiveMenu("THÔNG TIN CÁ NHÂN", info);
    }    private void UpdatePersonalInfo()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN", 80, 10);
        Console.WriteLine("✏️ Chức năng cập nhật thông tin sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu user profile sẽ được update trong MySQL");
        Console.WriteLine("💡 UI form edit email, phone, bio sẽ được triển khai");
        ConsoleRenderingService.PauseWithMessage();
    }    private void ForgotPassword()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("QUÊN MẬT KHẨU", 80, 10);        Console.WriteLine("🔑 Chức năng reset password sẽ được kết nối với database");
        Console.WriteLine("📧 Email verification và password reset sẽ được triển khai");
        Console.WriteLine("💡 Integration với email service để gửi reset link");
        ConsoleRenderingService.PauseWithMessage();
    }    // Async methods needed by ViewerMenuService - calling BL Services
    public async Task<TournamentInfoDto?> GetTournamentDetailAsync(int tournamentId)
    {
        return await _tournamentService.GetTournamentByIdAsync(tournamentId);
    }

    public async Task<UserDto?> GetPersonalInfoAsync()
    {
        var result = await _userService.GetUserByIdAsync(_currentUser.Id);
        return result.IsSuccess ? result.Data : null;
    }
}
