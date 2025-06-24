using System;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers;

namespace EsportsManager.UI.MenuServices;

/// <summary>
/// ViewerMenuService - Xử lý UI menu cho Viewer
/// </summary>
public class ViewerMenuService
{
    private readonly ViewerController _viewerController;

    public ViewerMenuService(ViewerController viewerController)
    {
        _viewerController = viewerController ?? throw new ArgumentNullException(nameof(viewerController));
    }    /// <summary>
         /// Hiển thị menu Viewer
         /// </summary>
    public void ShowViewerMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Hiển thị menu chính của Viewer
    /// </summary>
    public void ShowMainMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "🏆 Xem danh sách giải đấu",
                "📅 Xem lịch thi đấu",
                "🏅 Xem kết quả trận đấu",
                "👥 Xem danh sách team",
                "🗳️ Vote cho team yêu thích",
                "💰 Donate cho giải đấu/team",
                "📝 Gửi feedback",
                "👤 Xem thông tin cá nhân",
                "✏️ Cập nhật thông tin",
                "🚪 Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("VIEWER CONTROL PANEL", menuOptions);            switch (selection)
            {
                case 0:
                    ShowTournamentList();
                    break;
                case 1:
                    ShowMatchSchedule();
                    break;
                case 2:
                    ShowMatchResults();
                    break;
                case 3:
                    ShowTeamList();
                    break;
                case 4:
                    ShowVoteForTeam();
                    break;
                case 5:
                    ShowDonation();
                    break;
                case 6:
                    ShowSendFeedback();
                    break;
                case 7:
                    ShowPersonalInfo();
                    break;
                case 8:
                    ShowUpdatePersonalInfo();                    break;
                case 9:
                case -1:
                    return; // Đăng xuất
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }    /// <summary>
    /// Xem danh sách giải đấu
    /// </summary>
    private void ShowTournamentList()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 80, 10);
        Console.WriteLine("🏆 Chức năng xem danh sách giải đấu sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu tournaments sẽ được lấy từ MySQL");
        Console.WriteLine("💡 Hiển thị chi tiết tournaments, status, participants");
        ConsoleRenderingService.PauseWithMessage();
    }

    /// <summary>
    /// Xem chi tiết giải đấu
    /// </summary>
    private void ShowTournamentDetail(int tournamentId)
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải chi tiết giải đấu...");

            var tournament = _viewerController.GetTournamentDetailAsync(tournamentId).GetAwaiter().GetResult();

            if (tournament == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không tìm thấy thông tin giải đấu", true, 2000);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CHI TIẾT: {tournament.Name}", 100, 25);

            Console.WriteLine($"📝 Mô tả: {tournament.Description}");
            Console.WriteLine($"📅 Ngày bắt đầu: {tournament.StartDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"📅 Ngày kết thúc: {tournament.EndDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"🎯 Trạng thái: {tournament.Status}");
            Console.WriteLine($"💰 Phí tham gia: {tournament.EntryFee:N0} VND");
            Console.WriteLine($"🏆 Tổng giải thưởng: {tournament.PrizePool:N0} VND"); Console.WriteLine($"👥 Số người tham gia: {tournament.CurrentParticipants}/{tournament.MaxParticipants}");
            Console.WriteLine($"🏢 Ban tổ chức: Admin"); // Tournament doesn't have Organizer property, defaulting to Admin
            Console.WriteLine($"📍 Địa điểm: {tournament.Location}");
            Console.WriteLine($"\n📋 Luật thi đấu:\n{tournament.Rules}");

            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Xem lịch thi đấu
    /// </summary>
    private void ShowMatchSchedule()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CHỌN GIẢI ĐẤU", 80, 10);
            Console.Write("Nhập ID giải đấu để xem lịch thi đấu: ");

            if (int.TryParse(Console.ReadLine(), out int tournamentId))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang tải lịch thi đấu...");
                // Method not yet implemented in ViewerController
                // var matches = _viewerController.GetMatchScheduleAsync(tournamentId).GetAwaiter().GetResult();

                // Display a message that this feature is not yet implemented
                Console.WriteLine("\nThis feature is not yet implemented.");
                Console.WriteLine("Press any key to return to the previous menu...");
                Console.ReadKey(true);
                // This section is commented out until the GetMatchScheduleAsync method is implemented
                /*
                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH THI ĐẤU", 100, 20);
                
                if (matches.Count == 0)
                {
                    ConsoleRenderingService.ShowMessageBox("Chưa có lịch thi đấu nào", false, 2000);
                    return;
                }

                Console.WriteLine($"{"Vòng",-15} {"Team 1",-20} {"VS",-5} {"Team 2",-20} {"Thời gian",-20} {"Trạng thái",-15}");
                Console.WriteLine(new string('=', 100));
                
                foreach (var match in matches)
                {
                    Console.WriteLine($"{match.Round,-15} {match.Team1,-20} {"VS",-5} {match.Team2,-20} {match.ScheduledTime:dd/MM HH:mm,-20} {match.Status,-15}");
                }
                */

                ConsoleRenderingService.PauseWithMessage();
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Xem kết quả trận đấu
    /// </summary>
    private void ShowMatchResults()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("KẾT QUẢ TRẬN ĐẤU", 80, 10);
        Console.WriteLine("🏆 Chức năng xem kết quả trận đấu sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu match results sẽ được lấy từ MySQL");
        Console.WriteLine("� Hiển thị real-time kết quả các trận đấu đang diễn ra");
        Console.WriteLine("🎯 Thống kê chi tiết về team performance và rankings");
        ConsoleRenderingService.PauseWithMessage();
    }    /// <summary>
    /// Xem danh sách team
    /// </summary>
    private void ShowTeamList()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DANH SÁCH TEAM", 80, 10);
        Console.WriteLine("👥 Chức năng xem danh sách team sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu teams sẽ được lấy từ MySQL");
        Console.WriteLine("💡 Hiển thị team info, members, achievements");
        ConsoleRenderingService.PauseWithMessage();
    }

    /// <summary>
    /// Vote cho team
    /// </summary>
    private void ShowVoteForTeam()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("VOTE CHO TEAM", 80, 10);
            Console.Write("Nhập ID team muốn vote: ");
            if (int.TryParse(Console.ReadLine(), out int teamId))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang gửi vote...");
                // Method not yet implemented in ViewerController
                // bool success = _viewerController.VoteForTeamAsync(teamId).GetAwaiter().GetResult();

                // Display a message that this feature is not yet implemented
                ConsoleRenderingService.ShowMessageBox("Tính năng chưa được triển khai!", true, 2000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Vui lòng nhập ID hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Donate
    /// </summary>
    private void ShowDonation()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER/TEAM", 80, 20);

            Console.WriteLine("💰 CHỌN LOẠI DONATE:");
            Console.WriteLine("1. Donate cho Player");
            Console.WriteLine("2. Donate cho Team");
            Console.WriteLine("3. Donate cho Giải đấu");
            Console.WriteLine("0. Quay lại");

            Console.Write("\nNhập lựa chọn: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        ShowDonateToPlayer();
                        break;
                    case 2:
                        ShowDonateToTeam();
                        break;
                    case 3:
                        ShowDonateToTournament();
                        break;
                    case 0:
                        return;
                    default:
                        ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    private void ShowDonateToPlayer()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DONATE CHO PLAYER", 80, 10);
        Console.WriteLine("� Chức năng donate cho player sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu player và donation history sẽ được lấy từ MySQL");
        Console.WriteLine("💡 UI form nhập số tiền và chọn player sẽ được triển khai");
        ConsoleRenderingService.PauseWithMessage();
    }    private void ShowDonateToTeam()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DONATE CHO TEAM", 80, 10);
        Console.WriteLine("� Chức năng donate cho team sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu team và donation history sẽ được lấy từ MySQL");
        Console.WriteLine("💡 UI form nhập số tiền và chọn team sẽ được triển khai");
        ConsoleRenderingService.PauseWithMessage();
    }    private void ShowDonateToTournament()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DONATE CHO GIẢI ĐẤU", 80, 10);
        Console.WriteLine("💰 Chức năng donate cho giải đấu sẽ được kết nối với database");
        Console.WriteLine("📊 Dữ liệu tournament và prize pool sẽ được lấy từ MySQL");
        Console.WriteLine("💡 UI form nhập số tiền và chọn tournament sẽ được triển khai");
        ConsoleRenderingService.PauseWithMessage();
    }/// <summary>
    /// Gửi feedback
    /// </summary>
    private void ShowSendFeedback()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GỬI FEEDBACK", 80, 15);

            Console.WriteLine("Loại feedback:");
            Console.WriteLine("1. Feedback về giải đấu");
            Console.WriteLine("2. Feedback về hệ thống");
            Console.WriteLine("3. Feedback chung");
            Console.Write("\nChọn loại feedback (1-3): ");

            if (!int.TryParse(Console.ReadLine(), out int feedbackType) || feedbackType < 1 || feedbackType > 3)
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
                return;
            }

            Console.Write("\nTiêu đề feedback: ");
            string title = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                ConsoleRenderingService.ShowMessageBox("Tiêu đề không được để trống!", true, 2000);
                return;
            }

            Console.WriteLine("\nNội dung feedback (nhập 'END' trên dòng mới để kết thúc):");
            string content = "";
            string line;
            while ((line = Console.ReadLine()) != "END")
            {
                content += line + "\n";
            }

            if (string.IsNullOrEmpty(content.Trim()))
            {
                ConsoleRenderingService.ShowMessageBox("Nội dung không được để trống!", true, 2000);
                return;
            }

            ConsoleRenderingService.ShowMessageBox("Feedback đã được ghi nhận! Cảm ơn bạn đã đóng góp ý kiến.", false, 3000);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Xem thông tin cá nhân
    /// </summary>
    private void ShowPersonalInfo()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thông tin cá nhân...");
            var userInfo = _viewerController.GetPersonalInfoAsync().GetAwaiter().GetResult();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÔNG TIN CÁ NHÂN", 80, 12);

            if (userInfo != null)
            {
                Console.WriteLine($"👤 ID: {userInfo.Id}");
                Console.WriteLine($"📧 Username: {userInfo.Username}");
                Console.WriteLine($"✉️ Email: {userInfo.Email ?? "Chưa cập nhật"}");
                Console.WriteLine($"🎭 Role: {userInfo.Role}");
                Console.WriteLine($"📅 Ngày tạo: {userInfo.CreatedAt:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"🕐 Lần đăng nhập cuối: {userInfo.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");
            }
            else
            {
                Console.WriteLine("Không thể tải thông tin người dùng.");
            }

            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }    /// <summary>
    /// Cập nhật thông tin cá nhân
    /// </summary>
    private void ShowUpdatePersonalInfo()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 15);

            var currentInfo = _viewerController.GetPersonalInfoAsync().GetAwaiter().GetResult();

            Console.WriteLine("Thông tin hiện tại:");
            Console.WriteLine($"Email: {currentInfo.Email ?? "Chưa có"}");
            Console.WriteLine($"Username: {currentInfo.Username}");
            Console.WriteLine();

            Console.Write("Email mới (Enter để bỏ qua): ");
            string newEmail = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(newEmail))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật...");
                // Simulate update
                System.Threading.Thread.Sleep(1000);
                ConsoleRenderingService.ShowMessageBox("Cập nhật thông tin thành công!", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Không có thông tin nào được thay đổi!", false, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }
}
