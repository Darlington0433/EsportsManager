// Controller xử lý chức năng Viewer

using System;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Controllers;

public class ViewerController
{
    private readonly UserProfileDto _currentUser;

    public ViewerController(UserProfileDto currentUser)
    {
        _currentUser = currentUser;
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

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU VIEWER - {_currentUser.Username}", menuOptions);

            switch (selection)
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

        int selection = InteractiveMenuService.DisplayInteractiveMenu("VOTING", voteOptions);
        
        switch (selection)
        {
            case 0:
                ConsoleRenderingService.ShowMessageBox("Chức năng vote player đang được phát triển", false, 2000);
                break;
            case 1:
                ConsoleRenderingService.ShowMessageBox("Chức năng vote tournament đang được phát triển", false, 2000);
                break;
            case 2:
                ConsoleRenderingService.ShowMessageBox("Chức năng vote sport đang được phát triển", false, 2000);
                break;
            case 3:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem kết quả voting đang được phát triển", false, 2000);
                break;
        }
    }

    private void DonateToPlayer()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng donate cho player đang được phát triển", false, 2000);
    }

    private void ViewPersonalInfo()
    {
        var info = new[]
        {
            $"Username: {_currentUser.Username}",
            $"Email: {_currentUser.Email}",            $"Username: {_currentUser.Username}",
            $"Email: {_currentUser.Email}",
            $"Role: {_currentUser.Role}",
            $"Ngày tạo: {_currentUser.CreatedAt:dd/MM/yyyy}",
            $"Lần đăng nhập cuối: {_currentUser.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập"}",
            $"Trạng thái: {_currentUser.Status}"
        };

        InteractiveMenuService.DisplayInteractiveMenu("THÔNG TIN CÁ NHÂN", info);
    }

    private void UpdatePersonalInfo()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
    }

    private void ForgotPassword()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng quên mật khẩu đang được phát triển", false, 2000);
    }
}
