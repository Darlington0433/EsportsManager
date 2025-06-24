// Controller xử lý chức năng Player

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Utilities;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;

namespace EsportsManager.UI.Controllers;

public class PlayerController
{
    private readonly UserProfileDto _currentUser;
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public PlayerController(UserProfileDto currentUser, IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _currentUser = currentUser;
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
    }

    public void ShowPlayerMenu()
    {
        while (true)
        {
            var menuOptions = new[]
            {
                "Đăng ký tham gia giải đấu",
                "Quản lý team",
                "Xem thông tin cá nhân",
                "Cập nhật thông tin cá nhân",
                "Xem danh sách giải đấu",
                "Gửi feedback giải đấu",
                "Quản lý ví điện tử",
                "Thành tích cá nhân",
                "Đăng xuất"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu($"MENU PLAYER - {_currentUser.Username}", menuOptions); switch (selection)
            {
                case 0:
                    RegisterForTournamentAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    ManageTeamAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    ViewPersonalInfoAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    UpdatePersonalInfoAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    ViewTournamentListAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    SubmitTournamentFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    ManageWalletAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                    ViewPlayerAchievementsAsync().GetAwaiter().GetResult();
                    break;
                case 8:
                case -1:
                    return; // Đăng xuất
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }    // UI Methods calling BL Services
    private async Task RegisterForTournamentAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("ĐĂNG KÝ THAM GIA GIẢI ĐẤU", 80, 15);

            var tournaments = await GetAvailableTournamentsAsync();

            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào mở đăng ký", false, 2000);
                return;
            }

            Console.WriteLine("🏆 Danh sách giải đấu có sẵn:");
            for (int i = 0; i < tournaments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tournaments[i].Name} - Phí: {tournaments[i].EntryFee:N0} VND");
            }

            Console.Write($"\nNhập số thứ tự giải đấu muốn tham gia (1-{tournaments.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= tournaments.Count)
            {
                var selectedTournament = tournaments[choice - 1];
                bool success = await RegisterForTournamentAsync(selectedTournament.Id);

                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox($"Đã đăng ký tham gia '{selectedTournament.Name}' thành công!", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Đăng ký thất bại! Bạn cần tham gia team trước.", true, 3000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    private async Task ManageTeamAsync()
    {
        try
        {
            var myTeam = await GetMyTeamAsync();

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

            ConsoleRenderingService.ShowLoadingMessage("Đang tạo team...");
            var team = await CreateTeamAsync(teamDto);

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

    private async Task ViewPersonalInfoAsync()
    {
        try
        {
            var userInfo = await GetPersonalInfoAsync();

            if (userInfo == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin cá nhân!", true, 2000);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THÔNG TIN CÁ NHÂN", 80, 15);

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

    private async Task UpdatePersonalInfoAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 15);

            var currentInfo = await GetPersonalInfoAsync();
            if (currentInfo == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải thông tin hiện tại!", true, 2000);
                return;
            }

            Console.WriteLine("Thông tin hiện tại:");
            Console.WriteLine($"Email: {currentInfo.Email ?? "Chưa có"}");
            Console.WriteLine($"Họ tên: {currentInfo.FullName ?? "Chưa có"}");
            Console.WriteLine();

            Console.Write("Email mới (Enter để bỏ qua): "); string newEmail = Console.ReadLine()?.Trim();

            Console.Write("Họ tên mới (Enter để bỏ qua): ");
            string newFullName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newFullName))
            {
                var updateDto = new UpdateUserDto
                {
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : currentInfo?.Email,
                    FullName = !string.IsNullOrEmpty(newFullName) ? newFullName : currentInfo?.FullName
                };

                ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật...");
                bool success = await UpdatePersonalInfoAsync(updateDto);

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
    }

    private async Task ViewTournamentListAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 100, 20);

            var tournaments = await GetAllTournamentsAsync();

            if (tournaments.Count == 0)
            {
                ConsoleRenderingService.ShowMessageBox("Hiện tại không có giải đấu nào", false, 2000);
                return;
            }

            Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-35} {"Trạng thái",-15} {"Phí tham gia",-15}");
            Console.WriteLine(new string('=', 70));

            for (int i = 0; i < tournaments.Count; i++)
            {
                var tournament = tournaments[i];
                Console.WriteLine($"{i + 1,-5} {tournament.Name,-35} {tournament.Status,-15} {tournament.EntryFee:N0,-15}");
            }

            Console.WriteLine(new string('=', 70));
            ConsoleRenderingService.PauseWithMessage();
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    private async Task SubmitTournamentFeedbackAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GỬI FEEDBACK GIẢI ĐẤU", 80, 20);

            Console.WriteLine("📝 LOẠI FEEDBACK:");
            Console.WriteLine("1. Báo cáo lỗi kỹ thuật");
            Console.WriteLine("2. Góp ý cải thiện");
            Console.WriteLine("3. Khiếu nại về kết quả");

            Console.Write("\nChọn loại feedback (1-3): ");
            if (int.TryParse(Console.ReadLine(), out int type) && type >= 1 && type <= 3)
            {
                Console.Write("Tiêu đề feedback: ");
                string title = Console.ReadLine() ?? "";

                Console.Write("Nội dung chi tiết: ");
                string content = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                {
                    var feedbackDto = new FeedbackDto
                    {
                        UserId = _currentUser.Id,
                        Content = content,
                        CreatedAt = DateTime.Now
                    };

                    ConsoleRenderingService.ShowLoadingMessage("Đang gửi feedback...");
                    bool success = await SubmitFeedbackAsync(feedbackDto);

                    if (success)
                    {
                        ConsoleRenderingService.ShowMessageBox("Feedback đã được gửi thành công!", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox("Gửi feedback thất bại!", true, 2000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Vui lòng nhập đầy đủ thông tin!", true, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    private async Task ManageWalletAsync()
    {
        // TODO: Implement wallet management calling BL Wallet Service
        ConsoleRenderingService.ShowMessageBox("🏦 Chức năng ví điện tử sẽ được kết nối với BL WalletService", false, 2000);
    }

    private async Task ViewPlayerAchievementsAsync()
    {
        // TODO: Implement achievements calling BL Achievement Service
        ConsoleRenderingService.ShowMessageBox("🏆 Chức năng thành tích sẽ được kết nối với BL Achievement Service", false, 2000);
    }
    private void UpdatePersonalInfo()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẬP NHẬT THÔNG TIN CÁ NHÂN", 80, 20);

            Console.WriteLine("Thông tin hiện tại:");
            Console.WriteLine($"📧 Email: {_currentUser.Email ?? "Chưa có"}");
            Console.WriteLine($"👤 Họ tên: {_currentUser.FullName ?? "Chưa có"}");
            Console.WriteLine($"📱 Số điện thoại: {_currentUser.PhoneNumber ?? "Chưa có"}");
            Console.WriteLine($"📝 Bio: {_currentUser.Bio ?? "Chưa có"}");
            Console.WriteLine();

            Console.Write("Email mới (Enter để bỏ qua): ");
            string newEmail = Console.ReadLine() ?? "";

            Console.Write("Họ tên mới (Enter để bỏ qua): ");
            string newFullName = Console.ReadLine() ?? "";

            Console.Write("Số điện thoại mới (Enter để bỏ qua): ");
            string newPhone = Console.ReadLine() ?? "";

            Console.Write("Bio mới (Enter để bỏ qua): ");
            string newBio = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(newEmail) || !string.IsNullOrWhiteSpace(newFullName) ||
                !string.IsNullOrWhiteSpace(newPhone) || !string.IsNullOrWhiteSpace(newBio))
            {
                ConsoleRenderingService.ShowLoadingMessage("Đang cập nhật thông tin...");

                // Simulate update process
                System.Threading.Thread.Sleep(2000);

                ConsoleRenderingService.ShowMessageBox("✅ Cập nhật thông tin thành công!\n💡 Chức năng sẽ được kết nối với database trong phiên bản tiếp theo", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Không có thông tin nào được thay đổi", false, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }
    private void ViewTournamentList()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 100, 20);

        Console.WriteLine($"{"STT",-5} {"Tên giải đấu",-35} {"Trạng thái",-15} {"Phí tham gia",-15} {"Giải thưởng",-15}");
        Console.WriteLine(new string('=', 90));

        // Dữ liệu mẫu
        var tournaments = new[]
        {
            ("League of Legends Championship 2025", "Đang mở", "50,000", "1,000,000"),
            ("CS:GO Masters Tournament", "Sắp diễn ra", "30,000", "500,000"),
            ("PUBG Mobile Vietnam Cup", "Đang diễn ra", "25,000", "750,000"),
            ("FIFA Online 4 League", "Đang mở", "20,000", "400,000"),
            ("Valorant Premier Series", "Sắp diễn ra", "40,000", "800,000")
        };

        for (int i = 0; i < tournaments.Length; i++)
        {
            var (name, status, fee, prize) = tournaments[i];
            Console.WriteLine($"{i + 1,-5} {name,-35} {status,-15} {fee,-15} {prize,-15}");
        }

        Console.WriteLine(new string('=', 90));
        Console.WriteLine("\n💡 Dữ liệu thực tế sẽ được kết nối từ database trong phiên bản tiếp theo");

        ConsoleRenderingService.PauseWithMessage();
    }
    private void SubmitTournamentFeedback()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("GỬI FEEDBACK GIẢI ĐẤU", 80, 20);

            Console.WriteLine("📝 LOẠI FEEDBACK:");
            Console.WriteLine("1. Báo cáo lỗi kỹ thuật");
            Console.WriteLine("2. Góp ý cải thiện");
            Console.WriteLine("3. Khiếu nại về kết quả");
            Console.WriteLine("4. Đề xuất tính năng mới");
            Console.WriteLine("5. Phản hồi chung");

            Console.Write("\nChọn loại feedback (1-5): ");
            if (int.TryParse(Console.ReadLine(), out int type) && type >= 1 && type <= 5)
            {
                string[] types = { "", "Báo cáo lỗi kỹ thuật", "Góp ý cải thiện", "Khiếu nại về kết quả", "Đề xuất tính năng mới", "Phản hồi chung" };

                Console.WriteLine($"\nLoại feedback: {types[type]}");
                Console.Write("Tiêu đề feedback: ");
                string title = Console.ReadLine() ?? "";

                Console.Write("Nội dung chi tiết: ");
                string content = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                {
                    ConsoleRenderingService.ShowLoadingMessage("Đang gửi feedback...");

                    // Simulate sending process
                    System.Threading.Thread.Sleep(2000);

                    Console.Clear();
                    ConsoleRenderingService.DrawBorder("FEEDBACK ĐÃ GỬI THÀNH CÔNG", 80, 12);
                    Console.WriteLine($"📋 Mã feedback: FB{DateTime.Now:yyyyMMddHHmm}");
                    Console.WriteLine($"📂 Loại: {types[type]}");
                    Console.WriteLine($"📝 Tiêu đề: {title}");
                    Console.WriteLine($"⏰ Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    Console.WriteLine("\n✅ Feedback của bạn đã được ghi nhận!");
                    Console.WriteLine("💡 Chúng tôi sẽ phản hồi trong vòng 24-48 giờ");

                    ConsoleRenderingService.PauseWithMessage();
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Vui lòng nhập đầy đủ thông tin!", true, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Lựa chọn không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }
    private void ManageWallet()
    {
        // Mock data cho wallet
        decimal currentBalance = 250000; // Mock số dư hiện tại

        while (true)
        {
            var walletOptions = new[]
            {
                $"Xem số dư ví (Hiện tại: {currentBalance:N0} VND)",
                "Nạp tiền vào ví",
                "Rút tiền từ ví",
                "Lịch sử giao dịch",
                "Donate cho player khác",
                "Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ VÍ ĐIỆN TỬ", walletOptions);
            switch (selection)
            {
                case 0:
                    ViewWalletBalance(currentBalance);
                    break;
                case 1:
                    currentBalance = DepositMoney(currentBalance);
                    break;
                case 2:
                    currentBalance = WithdrawMoney(currentBalance);
                    break;
                case 3:
                    ViewTransactionHistory();
                    break;
                case 4:
                    DonateMoney(currentBalance);
                    break;
                case 5:
                case -1:
                    return;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }

    private void ViewWalletBalance(decimal balance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(50, windowWidth - 6);
        int boxHeight = Math.Min(10, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);

        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[SỐ DƯ VÍ]", true);

        Console.SetCursorPosition(left + 3, top + 3);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Số dư hiện tại: {balance:N0} VND");

        Console.SetCursorPosition(left + 3, top + 6);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Nhấn phím bất kỳ để tiếp tục...");

        Console.ResetColor();
        Console.ReadKey(true);
    }

    private decimal DepositMoney(decimal currentBalance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(60, windowWidth - 6);
        int boxHeight = Math.Min(12, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);

        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[NẠP TIỀN VÀO VÍ]", true);

        Console.SetCursorPosition(left + 3, top + 3);
        Console.Write($"Số dư hiện tại: {currentBalance:N0} VND");

        Console.SetCursorPosition(left + 3, top + 5);
        Console.Write("Nhập số tiền muốn nạp (VND): ");

        Console.SetCursorPosition(left + 32, top + 5);
        string? amountStr = UnifiedInputService.ReadText(20, c => char.IsDigit(c));

        if (decimal.TryParse(amountStr, out decimal amount) && amount > 0)
        {
            decimal newBalance = currentBalance + amount;
            ConsoleRenderingService.ShowMessageBox($"Nạp tiền thành công! Số dư mới: {newBalance:N0} VND", false, 2000);
            return newBalance;
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", true, 2000);
            return currentBalance;
        }
    }

    private decimal WithdrawMoney(decimal currentBalance)
    {
        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(60, windowWidth - 6);
        int boxHeight = Math.Min(12, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);

        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[RÚT TIỀN TỪ VÍ]", true);

        Console.SetCursorPosition(left + 3, top + 3);
        Console.Write($"Số dư hiện tại: {currentBalance:N0} VND");

        Console.SetCursorPosition(left + 3, top + 5);
        Console.Write("Nhập số tiền muốn rút (VND): ");

        Console.SetCursorPosition(left + 32, top + 5);
        string? amountStr = UnifiedInputService.ReadText(20, c => char.IsDigit(c));

        if (decimal.TryParse(amountStr, out decimal amount) && amount > 0)
        {
            if (amount <= currentBalance)
            {
                decimal newBalance = currentBalance - amount;
                ConsoleRenderingService.ShowMessageBox($"Rút tiền thành công! Số dư còn lại: {newBalance:N0} VND", false, 2000);
                return newBalance;
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Số dư không đủ để rút!", true, 2000);
                return currentBalance;
            }
        }
        else
        {
            ConsoleRenderingService.ShowMessageBox("Số tiền không hợp lệ!", true, 2000);
            return currentBalance;
        }
    }

    private void ViewTransactionHistory()
    {
        // Mock data cho lịch sử giao dịch
        var transactions = new[]
        {
            "15/06/2024 09:30 - Nạp tiền: +100,000 VND",
            "12/06/2024 14:15 - Rút tiền: -50,000 VND",
            "10/06/2024 11:20 - Donate: -25,000 VND (→ PlayerXYZ)",
            "08/06/2024 16:45 - Nạp tiền: +200,000 VND",
            "05/06/2024 13:30 - Phí giải đấu: -30,000 VND"
        };

        Console.Clear();
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        int boxWidth = Math.Min(70, windowWidth - 6);
        int boxHeight = Math.Min(15, windowHeight - 4);
        int left = Math.Max(1, (windowWidth - boxWidth) / 2);
        int top = Math.Max(1, (windowHeight - boxHeight) / 2);

        ConsoleRenderingService.DrawBorder(left, top, boxWidth, boxHeight, "[LỊCH SỬ GIAO DỊCH]", true);

        Console.SetCursorPosition(left + 3, top + 3);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("5 giao dịch gần nhất:");

        for (int i = 0; i < transactions.Length; i++)
        {
            Console.SetCursorPosition(left + 3, top + 5 + i);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{i + 1}. {transactions[i]}");
        }

        Console.SetCursorPosition(left + 3, top + boxHeight - 3);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Nhấn phím bất kỳ để quay lại...");

        Console.ResetColor();
        Console.ReadKey(true);
    }
    private void DonateMoney(decimal currentBalance)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("DONATE TIỀN", 80, 10);
        Console.WriteLine($"💰 Số dư hiện tại: {currentBalance:N0} VND");
        Console.WriteLine("📊 Chức năng donate sẽ được kết nối với database");
        Console.WriteLine("💡 Chọn player/team để donate và nhập số tiền");
        Console.WriteLine("🔄 Xử lý transaction và update wallet balance");
        ConsoleRenderingService.PauseWithMessage();
    }
    private void ViewPlayerAchievements()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("THÀNH TÍCH CÁ NHÂN", 80, 20);

        Console.WriteLine($"🏆 Thành tích của {_currentUser.Username}:");
        Console.WriteLine();

        // Dữ liệu mẫu thành tích
        Console.WriteLine("📊 THỐNG KÊ TỔNG QUAN:");
        Console.WriteLine($"• Tổng số giải đấu tham gia: 12");
        Console.WriteLine($"• Số giải thắng: 3");
        Console.WriteLine($"• Tỷ lệ thắng: 25%");
        Console.WriteLine($"• Tổng tiền thưởng: 2,500,000 VND");
        Console.WriteLine();

        Console.WriteLine("🏅 GIẢI THƯỞNG:");
        Console.WriteLine("• 🥇 Vô địch League of Legends Spring 2024");
        Console.WriteLine("• 🥈 Á quân CS:GO Summer Championship 2024");
        Console.WriteLine("• 🥉 Hạng 3 PUBG Mobile Winter Cup 2024");
        Console.WriteLine();

        Console.WriteLine("📈 XẾP HẠNG:");
        Console.WriteLine($"• Hạng hiện tại: Diamond III");
        Console.WriteLine($"• Điểm đánh giá: 1,847 points");
        Console.WriteLine($"• Vị trí trong bảng xếp hạng: #127 toàn quốc");
        Console.WriteLine();

        Console.WriteLine("💡 Dữ liệu thực tế sẽ được kết nối từ database trong phiên bản tiếp theo");
        ConsoleRenderingService.PauseWithMessage();
    }    // Async methods needed by PlayerMenuService - calling BL Services
    public async Task<UserDto?> GetPersonalInfoAsync()
    {
        var result = await _userService.GetUserByIdAsync(_currentUser.Id);
        return result.IsSuccess ? result.Data : null;
    }

    public async Task<bool> UpdatePersonalInfoAsync(UpdateUserDto updateDto)
    {
        // Convert UpdateUserDto to UserDto for the service call
        var userDto = new UserDto
        {
            Id = _currentUser.Id,
            Username = _currentUser.Username,
            Email = updateDto.Email ?? _currentUser.Email,
            FullName = updateDto.FullName,
            Role = _currentUser.Role
        };

        var result = await _userService.UpdateUserProfileAsync(_currentUser.Id, userDto);
        return result.IsSuccess;
    }

    public async Task<bool> ChangePasswordAsync(UpdatePasswordDto passwordDto)
    {
        passwordDto.UserId = _currentUser.Id;
        var result = await _userService.UpdatePasswordAsync(passwordDto);
        return result.IsSuccess;
    }

    public async Task<List<TournamentInfoDto>> GetAllTournamentsAsync()
    {
        return await _tournamentService.GetAllTournamentsAsync();
    }

    public async Task<List<TournamentInfoDto>> GetAvailableTournamentsAsync()
    {
        return await _tournamentService.GetAvailableTournamentsAsync();
    }

    public async Task<bool> RegisterForTournamentAsync(int tournamentId)
    {
        // Get player's team first
        var team = await _teamService.GetPlayerTeamAsync(_currentUser.Id);
        if (team == null)
        {
            return false; // Player needs to be in a team to register
        }

        return await _tournamentService.RegisterTeamForTournamentAsync(tournamentId, team.Id);
    }

    public async Task<TeamInfoDto?> GetMyTeamAsync()
    {
        return await _teamService.GetPlayerTeamAsync(_currentUser.Id);
    }

    public async Task<TeamInfoDto?> CreateTeamAsync(TeamCreateDto teamInfo)
    {
        return await _teamService.CreateTeamAsync(teamInfo, _currentUser.Id);
    }

    public async Task<bool> SubmitFeedbackAsync(FeedbackDto feedback)
    {
        return await _tournamentService.SubmitFeedbackAsync(_currentUser.Id, feedback);
    }
}
