// Controller xử lý chức năng Admin

using System;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.DTOs;

namespace EsportsManager.UI.Controllers;

public class AdminController
{
    private readonly UserProfileDto _currentUser;

    public AdminController(UserProfileDto currentUser)
    {
        _currentUser = currentUser;
    }

    public void ShowAdminMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Quản lý người dùng",
                "Quản lý giải đấu/trận đấu",
                "Xem thống kê hệ thống",
                "Xem báo cáo donation",
                "Xem kết quả voting",
                "Quản lý feedback",
                "Cài đặt hệ thống",
                "Xóa người dùng",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU ADMIN - {_currentUser.Username}", menuOptions);

            switch (selection)
            {
                case 0:
                    ManageUsers();
                    break;
                case 1:
                    ManageTournaments();
                    break;
                case 2:
                    ViewSystemStats();
                    break;
                case 3:
                    ViewDonationReports();
                    break;
                case 4:
                    ViewVotingResults();
                    break;
                case 5:
                    ManageFeedback();
                    break;
                case 6:
                    SystemSettings();
                    break;
                case 7:
                    DeleteUsers();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    private void ManageUsers()
    {
        var userOptions = new[]
        {
            "Xem danh sách người dùng",
            "Tìm kiếm người dùng",
            "Thay đổi trạng thái người dùng",
            "Xem thông tin chi tiết người dùng",
            "Reset mật khẩu người dùng"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ NGƯỜI DÙNG", userOptions);
        
        switch (selection)
        {
            case 0:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem danh sách người dùng đang được phát triển", false, 2000);
                break;
            case 1:
                ConsoleRenderingService.ShowMessageBox("Chức năng tìm kiếm người dùng đang được phát triển", false, 2000);
                break;
            case 2:
                ConsoleRenderingService.ShowMessageBox("Chức năng thay đổi trạng thái đang được phát triển", false, 2000);
                break;
            case 3:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem chi tiết đang được phát triển", false, 2000);
                break;
            case 4:
                ConsoleRenderingService.ShowMessageBox("Chức năng reset mật khẩu đang được phát triển", false, 2000);
                break;
        }
    }

    private void ManageTournaments()
    {
        var tournamentOptions = new[]
        {
            "Tạo giải đấu mới",
            "Quản lý giải đấu hiện tại",
            "Xem lịch thi đấu",
            "Cập nhật kết quả trận đấu",
            "Quản lý team tham gia"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ GIẢI ĐẤU", tournamentOptions);
        
        switch (selection)
        {
            case 0:
                ConsoleRenderingService.ShowMessageBox("Chức năng tạo giải đấu đang được phát triển", false, 2000);
                break;
            case 1:
                ConsoleRenderingService.ShowMessageBox("Chức năng quản lý giải đấu đang được phát triển", false, 2000);
                break;
            case 2:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem lịch thi đấu đang được phát triển", false, 2000);
                break;
            case 3:
                ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật kết quả đang được phát triển", false, 2000);
                break;
            case 4:
                ConsoleRenderingService.ShowMessageBox("Chức năng quản lý team đang được phát triển", false, 2000);
                break;
        }
    }

    private void ViewSystemStats()
    {
        var stats = new[]
        {
            "Tổng số người dùng: 0",
            "Số người dùng hoạt động: 0", 
            "Tổng số giải đấu: 0",
            "Số giải đấu đang diễn ra: 0",
            "Tổng số team: 0",
            "Tổng doanh thu: 0 VND"
        };

        InteractiveMenuService.DisplayInteractiveMenu("THỐNG KÊ HỆ THỐNG", stats);
    }

    private void ViewDonationReports()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng báo cáo donation đang được phát triển", false, 2000);
    }

    private void ViewVotingResults()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem kết quả voting đang được phát triển", false, 2000);
    }

    private void ManageFeedback()
    {
        var feedbackOptions = new[]
        {
            "Xem tất cả feedback",
            "Phản hồi feedback",
            "Xóa feedback spam",
            "Thống kê feedback theo giải đấu"
        };

        int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ FEEDBACK", feedbackOptions);
        
        switch (selection)
        {
            case 0:
                ConsoleRenderingService.ShowMessageBox("Chức năng xem feedback đang được phát triển", false, 2000);
                break;
            case 1:
                ConsoleRenderingService.ShowMessageBox("Chức năng phản hồi feedback đang được phát triển", false, 2000);
                break;
            case 2:
                ConsoleRenderingService.ShowMessageBox("Chức năng xóa spam đang được phát triển", false, 2000);
                break;
            case 3:
                ConsoleRenderingService.ShowMessageBox("Chức năng thống kê feedback đang được phát triển", false, 2000);
                break;
        }
    }

    private void SystemSettings()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cài đặt hệ thống đang được phát triển", false, 2000);
    }

    private void DeleteUsers()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xóa người dùng đang được phát triển", false, 2000);
    }
}
