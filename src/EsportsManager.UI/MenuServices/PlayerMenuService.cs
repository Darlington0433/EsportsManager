using System;
using EsportsManager.UI.Controllers;
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

            int selection = InteractiveMenuService.DisplayInteractiveMenu("PLAYER CONTROL PANEL", menuOptions); switch (selection)
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
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
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
            var team = _playerController.CreateTeamAsync(teamDto).GetAwaiter().GetResult();

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

    /// <summary>
    /// Hiển thị thông tin team
    /// </summary>
    private void ShowTeamInfo(TeamInfoDto team)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder($"TEAM: {team.Name}", 100, 20);
        Console.WriteLine($"📝 Mô tả: {team.Description}");
        Console.WriteLine($"📅 Ngày tạo: {team.CreatedAt:dd/MM/yyyy}");
        Console.WriteLine($"👥 Số thành viên: {team.MemberCount}/{team.MaxMembers}");

        // Note: To show team members, we would need to get the members separately
        // as the TeamInfoDto doesn't contain the members list

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }    /// <summary>
         /// Cập nhật thông tin cá nhân
         /// </summary>
    private void ShowUpdatePersonalInfo()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 15);

            var currentInfo = _playerController.GetPersonalInfoAsync().GetAwaiter().GetResult();

            Console.WriteLine("Thông tin hiện tại:");
            Console.WriteLine($"Email: {currentInfo.Email ?? "Chưa có"}");
            Console.WriteLine($"Họ tên: {currentInfo.FullName ?? "Chưa có"}");
            Console.WriteLine();

            Console.Write("Email mới (Enter để bỏ qua): ");
            string newEmail = Console.ReadLine()?.Trim();

            Console.Write("Họ tên mới (Enter để bỏ qua): ");
            string newFullName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newFullName))
            {
                var updateDto = new UpdateUserDto
                {
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : currentInfo.Email,
                    FullName = !string.IsNullOrEmpty(newFullName) ? newFullName : currentInfo.FullName
                };

                ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật...");
                bool success = _playerController.UpdatePersonalInfoAsync(updateDto).GetAwaiter().GetResult();

                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox("Cập nhật thông tin thành công!", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Cập nhật thất bại!", true, 2000);
                }
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
    }    /// <summary>
         /// Thay đổi mật khẩu
         /// </summary>
    private void ShowChangePassword()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THAY ĐỔI MẬT KHẨU", 80, 12);

            Console.Write("Mật khẩu hiện tại: ");
            string oldPassword = ReadPassword();

            Console.Write("\nMật khẩu mới: ");
            string newPassword = ReadPassword();

            Console.Write("\nXác nhận mật khẩu mới: ");
            string confirmPassword = ReadPassword();

            if (newPassword != confirmPassword)
            {
                ConsoleRenderingService.ShowMessageBox("Mật khẩu xác nhận không khớp!", true, 2000);
                return;
            }

            if (newPassword.Length < 6)
            {
                ConsoleRenderingService.ShowMessageBox("Mật khẩu mới phải có ít nhất 6 ký tự!", true, 2000);
                return;
            }

            var changePasswordDto = new UpdatePasswordDto
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword
            };

            ConsoleRenderingService.ShowLoadingMessage("Đang thay đổi mật khẩu...");
            bool success = _playerController.ChangePasswordAsync(changePasswordDto).GetAwaiter().GetResult();

            if (success)
            {
                ConsoleRenderingService.ShowMessageBox("Thay đổi mật khẩu thành công!", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Thay đổi mật khẩu thất bại! Kiểm tra lại mật khẩu hiện tại.", true, 3000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Đọc mật khẩu ẩn
    /// </summary>
    private string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else
            {
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
            }
        }
        while (key.Key != ConsoleKey.Enter);
        return password;
    }    /// <summary>
         /// Xem danh sách giải đấu
         /// </summary>
    private void ShowTournamentList()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải danh sách giải đấu...");

            var tournaments = _playerController.GetAllTournamentsAsync().GetAwaiter().GetResult();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 100, 20);

            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào", false, 2000);
                return;
            }

            Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-30} {"Trạng thái",-15} {"Ngày bắt đầu",-15} {"Phí tham gia",-15}");
            Console.WriteLine(new string('=', 90));

            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"{i + 1,-5} {tournament.Name,-30} {tournament.Status,-15} {tournament.StartDate:dd/MM/yyyy,-15} {tournament.EntryFee:N0,-15}");
            }

            Console.WriteLine(new string('=', 90));
            Console.Write("\nNhập số thứ tự để xem chi tiết (0 để quay lại): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= tournaments.Count)
            {
                var selectedTournament = tournaments[choice - 1];
                ShowTournamentDetail(selectedTournament);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Hiển thị chi tiết giải đấu
    /// </summary>
    private void ShowTournamentDetail(TournamentInfoDto tournament)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder($"CHI TIẾT GIẢI ĐẤU: {tournament.Name}", 100, 20);

        Console.WriteLine($"📝 Mô tả: {tournament.Description}");
        Console.WriteLine($"📅 Ngày bắt đầu: {tournament.StartDate:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"📅 Ngày kết thúc: {tournament.EndDate:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"🎯 Trạng thái: {tournament.Status}");
        Console.WriteLine($"💰 Phí tham gia: {tournament.EntryFee:N0} VND");
        Console.WriteLine($"🏆 Tổng giải thưởng: {tournament.PrizePool:N0} VND");
        Console.WriteLine($"👥 Số người tham gia: {tournament.CurrentParticipants}/{tournament.MaxParticipants}");
        Console.WriteLine($"📍 Địa điểm: {tournament.Location}");

        if (!string.IsNullOrEmpty(tournament.Rules))
        {
            Console.WriteLine($"\n📋 Luật thi đấu:\n{tournament.Rules}");
        }

        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }    /// <summary>
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
            var feedbackDto = new FeedbackDto();
            // TODO: Assign properties when FeedbackDto is fully compiled
            // feedbackDto.Title = title;
            // feedbackDto.Content = content.Trim();
            // feedbackDto.FeedbackType = feedbackType switch
            // {
            //     1 => "Tournament",
            //     2 => "System", 
            //     3 => "General",
            //     _ => "General"
            // };

            Console.WriteLine($"📝 Gửi feedback: {title} - {content.Trim()}");

            ConsoleRenderingService.ShowLoadingMessage("Đang gửi feedback...");
            bool success = _playerController.SubmitFeedbackAsync(feedbackDto).GetAwaiter().GetResult();

            if (success)
            {
                ConsoleRenderingService.ShowMessageBox("Gửi feedback thành công! Cảm ơn bạn đã đóng góp ý kiến.", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Gửi feedback thất bại!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }
}
