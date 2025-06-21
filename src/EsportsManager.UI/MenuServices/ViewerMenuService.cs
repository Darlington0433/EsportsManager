using System;
using EsportsManager.BL.Controllers;
using EsportsManager.BL.DTOs;
using EsportsManager.UI.ConsoleUI.Utilities;

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

            int selection = InteractiveMenuService.DisplayInteractiveMenu("VIEWER CONTROL PANEL", menuOptions);

            switch (selection)
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
                    ShowUpdatePersonalInfo();
                    break;
                case 9:
                case -1:
                    return; // Đăng xuất
            }
        }
    }

    /// <summary>
    /// Xem danh sách giải đấu
    /// </summary>
    private void ShowTournamentList()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách giải đấu...");
            
            var tournaments = _viewerController.GetAllTournamentsAsync().GetAwaiter().GetResult();
            
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 100, 25);
            
            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào", false, 2000);
                return;
            }

            Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-30} {"Trạng thái",-20} {"Số người tham gia",-15}");
            Console.WriteLine(new string('=', 90));
            
            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"{i + 1,-5} {tournament.Name,-30} {tournament.Status,-20} {tournament.ParticipantCount}/{tournament.MaxParticipants,-15}");
            }
            
            Console.WriteLine(new string('=', 90));
            Console.Write("\nNhập số thứ tự để xem chi tiết (0 để quay lại): ");
            
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= tournaments.Count)
            {
                ShowTournamentDetail(tournaments[choice - 1].Id);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
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
            Console.WriteLine($"🏆 Tổng giải thưởng: {tournament.PrizePool:N0} VND");
            Console.WriteLine($"👥 Số người tham gia: {tournament.ParticipantCount}/{tournament.MaxParticipants}");
            Console.WriteLine($"🏢 Ban tổ chức: {tournament.Organizer}");
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
                
                var matches = _viewerController.GetMatchScheduleAsync(tournamentId).GetAwaiter().GetResult();
                
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
                
                ConsoleRenderingService.PauseWithMessage();
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Xem kết quả trận đấu
    /// </summary>
    private void ShowMatchResults()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng xem kết quả trận đấu đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Xem danh sách team
    /// </summary>
    private void ShowTeamList()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách team...");
            
            var teams = _viewerController.GetAllTeamsAsync().GetAwaiter().GetResult();
            
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH TEAM", 100, 20);
            
            if (teams.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có team nào", false, 2000);
                return;
            }

            Console.WriteLine($"{"STT",-5} {"Tên Team",-25} {"Mô tả",-30} {"Thành viên",-10} {"Thành tích",-20}");
            Console.WriteLine(new string('=', 95));
            
            for (int i = 0; i < teams.Count; i++)
            {
                var team = teams[i];
                Console.WriteLine($"{i + 1,-5} {team.Name,-25} {team.Description,-30} {team.MemberCount,-10} {team.Achievements,-20}");
            }
            
            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
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
                
                bool success = _viewerController.VoteForTeamAsync(teamId).GetAwaiter().GetResult();
                
                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox("Vote thành công!", false, 2000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Vote thất bại!", true, 2000);
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Donate
    /// </summary>
    private void ShowDonation()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng donate đang được phát triển", false, 2000);
    }

    /// <summary>
    /// Gửi feedback
    /// </summary>
    private void ShowSendFeedback()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng gửi feedback đang được phát triển", false, 2000);
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
            
            Console.WriteLine($"👤 ID: {userInfo.Id}");
            Console.WriteLine($"📧 Username: {userInfo.Username}");
            Console.WriteLine($"✉️ Email: {userInfo.Email ?? "Chưa cập nhật"}");
            Console.WriteLine($"🎭 Role: {userInfo.Role}");
            Console.WriteLine($"📅 Ngày tạo: {userInfo.CreatedAt:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"🕐 Lần đăng nhập cuối: {userInfo.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa có"}");
            
            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân
    /// </summary>
    private void ShowUpdatePersonalInfo()
    {
        ConsoleRenderingService.ShowMessageBox("Chức năng cập nhật thông tin đang được phát triển", false, 2000);
    }
}
