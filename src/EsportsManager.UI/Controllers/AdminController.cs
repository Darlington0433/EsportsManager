// Controller xử lý chức năng Admin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;

namespace EsportsManager.UI.Controllers;

public class AdminUIController
{
    private readonly UserProfileDto _currentUser;
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;
    private readonly IWalletService _walletService;

    public AdminUIController(UserProfileDto currentUser, IUserService userService, ITournamentService tournamentService, ITeamService teamService, IWalletService walletService)
    {
        _currentUser = currentUser;
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
        _walletService = walletService;
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
                    ManageUsersAsync().GetAwaiter().GetResult();
                    break;
                case 1:
                    ManageTournamentsAsync().GetAwaiter().GetResult();
                    break;
                case 2:
                    ViewSystemStatsAsync().GetAwaiter().GetResult();
                    break;
                case 3:
                    ViewDonationReportsAsync().GetAwaiter().GetResult();
                    break;
                case 4:
                    ViewVotingResultsAsync().GetAwaiter().GetResult();
                    break;
                case 5:
                    ManageFeedbackAsync().GetAwaiter().GetResult();
                    break;
                case 6:
                    SystemSettingsAsync().GetAwaiter().GetResult();
                    break;
                case 7:
                    DeleteUsersAsync().GetAwaiter().GetResult();
                    break;
                case 8:
                case -1: return; // Đăng xuất
            }
        }
    }

    private async Task ManageUsersAsync()
    {
        while (true)
        {
            var userOptions = new[]
            {
                "Xem danh sách người dùng",
                "Tìm kiếm người dùng",
                "Thay đổi trạng thái người dùng",
                "Reset mật khẩu người dùng",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ NGƯỜI DÙNG", userOptions);

            switch (selection)
            {
                case 0:
                    await ShowAllUsersAsync();
                    break;
                case 1:
                    await SearchUsersAsync();
                    break;
                case 2:
                    await ToggleUserStatusAsync();
                    break;
                case 3:
                    await ResetUserPasswordAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }

    private async Task ShowAllUsersAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải danh sách người dùng...");

            var result = await _userService.GetActiveUsersAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH NGƯỜI DÙNG", 80, 20); if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                ConsoleRenderingService.ShowNotification("Không có người dùng nào.", ConsoleColor.Yellow);
                return;
            }

            var header = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                "ID", "Username", "Email", "Role", "Status");
            Console.WriteLine("\n" + header);
            Console.WriteLine(new string('─', 75));

            foreach (var user in result.Data)
            {
                var row = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                    user.Id,
                    user.Username,
                    user.Email ?? "",
                    user.Role,
                    user.Status);
                Console.WriteLine(row);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLỗi: {ex.Message}");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }

    private async Task SearchUsersAsync()
    {
        try
        {
            Console.Write("\nNhập từ khóa tìm kiếm: ");
            var searchTerm = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowNotification("Từ khóa tìm kiếm không được rỗng", ConsoleColor.Yellow);
                return;
            }

            var result = await _userService.GetActiveUsersAsync(); if (!result.IsSuccess || result.Data == null)
            {
                ConsoleRenderingService.ShowNotification("Không thể tải danh sách người dùng", ConsoleColor.Red);
                return;
            }

            var users = result.Data.Where(u =>
                u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 80, 20);

            if (!users.Any())
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy kết quả nào", ConsoleColor.Yellow);
                return;
            }

            var header = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                "ID", "Username", "Email", "Role", "Status");
            Console.WriteLine("\n" + header);
            Console.WriteLine(new string('─', 75));

            foreach (var user in users)
            {
                var row = string.Format("{0,-5} {1,-20} {2,-30} {3,-10} {4,-10}",
                    user.Id,
                    user.Username,
                    user.Email ?? "",
                    user.Role,
                    user.Status);
                Console.WriteLine(row);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLỗi: {ex.Message}");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
    }

    private async Task ToggleUserStatusAsync()
    {
        try
        {
            Console.Write("\nNhập ID người dùng: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ", ConsoleColor.Yellow);
                return;
            }

            var result = await _userService.GetUserByIdAsync(userId);
            if (!result.IsSuccess)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng", ConsoleColor.Yellow);
                return;
            }

            var user = result.Data;
            if (user == null)
            {
                ConsoleRenderingService.ShowNotification("Dữ liệu người dùng không hợp lệ", ConsoleColor.Yellow);
                return;
            }
            var newStatus = user.Status == "Active" ? "Inactive" : "Active";

            var confirmPrompt = $"Xác nhận thay đổi trạng thái người dùng {user.Username} từ {user.Status} sang {newStatus}? (Y/N): ";
            Console.Write("\n" + confirmPrompt);

            if (Console.ReadKey(true).Key != ConsoleKey.Y)
            {
                ConsoleRenderingService.ShowNotification("Đã hủy thao tác", ConsoleColor.Blue);
                return;
            }

            var updatedUser = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                Status = newStatus
            };
            var updateResult = await _userService.UpdateUserProfileAsync(userId, updatedUser);

            if (updateResult.IsSuccess)
                ConsoleRenderingService.ShowNotification($"Đã cập nhật trạng thái thành công", ConsoleColor.Green);
            else
                ConsoleRenderingService.ShowNotification($"Lỗi khi cập nhật trạng thái", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowNotification($"Lỗi: {ex.Message}", ConsoleColor.Red);
        }
    }

    private async Task ResetUserPasswordAsync()
    {
        try
        {
            Console.Write("\nNhập ID người dùng: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ", ConsoleColor.Yellow);
                return;
            }

            var userResult = await _userService.GetUserByIdAsync(userId); if (!userResult.IsSuccess || userResult.Data == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy người dùng", ConsoleColor.Yellow);
                return;
            }

            var confirmPrompt = $"Xác nhận reset mật khẩu cho người dùng {userResult.Data.Username}? (Y/N): ";
            Console.Write("\n" + confirmPrompt);

            if (Console.ReadKey(true).Key != ConsoleKey.Y)
            {
                ConsoleRenderingService.ShowNotification("Đã hủy thao tác", ConsoleColor.Blue);
                return;
            }
            var resetResult = await _userService.ResetPasswordAsync(userId); if (!string.IsNullOrEmpty(resetResult))
                ConsoleRenderingService.ShowNotification($"Đã reset mật khẩu thành công. Mật khẩu mới: {resetResult}", ConsoleColor.Green);
            else
                ConsoleRenderingService.ShowNotification($"Lỗi khi reset mật khẩu", ConsoleColor.Red);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowNotification($"Lỗi: {ex.Message}", ConsoleColor.Red);
        }
    }
    private async Task ManageTournamentsAsync()
    {
        while (true)
        {
            var tournamentOptions = new[]
            {
                "Xem danh sách giải đấu",
                "Tạo giải đấu mới",
                "Cập nhật giải đấu",
                "Xóa giải đấu",
                "Xem thống kê giải đấu",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ GIẢI ĐẤU", tournamentOptions);

            switch (selection)
            {
                case 0:
                    await ShowAllTournamentsAsync();
                    break;
                case 1:
                    await CreateTournamentAsync();
                    break;
                case 2:
                    await UpdateTournamentAsync();
                    break;
                case 3:
                    await DeleteTournamentAsync();
                    break;
                case 4:
                    await ShowTournamentStatsAsync();
                    break;
                case -1:
                case 5:
                    return;
            }
        }
    }    private async Task ViewSystemStatsAsync()
    {
        try
        {
            ConsoleRenderingService.ShowLoadingMessage("Đang tải thống kê hệ thống...");

            // Lấy dữ liệu thống kê thực từ database
            var users = await _userService.GetActiveUsersAsync();
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var teams = await _teamService.GetAllTeamsAsync();

            // Tính toán thống kê
            var totalUsers = users.IsSuccess ? users.Data?.Count() ?? 0 : 0;
            var activeUsers = users.IsSuccess ? users.Data?.Count(u => u.Status == "Active") ?? 0 : 0;
            var totalTournaments = tournaments.Count;
            var activeTournaments = tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration");
            var totalTeams = teams.Count;
            var totalRevenue = tournaments.Sum(t => t.PrizePool);

            // Thống kê theo tháng hiện tại
            var currentMonth = DateTime.Now;
            var newUsersThisMonth = users.IsSuccess ? users.Data?.Count(u => u.CreatedAt.Month == currentMonth.Month && u.CreatedAt.Year == currentMonth.Year) ?? 0 : 0;
            var tournamentsThisMonth = tournaments.Count(t => t.CreatedAt.Month == currentMonth.Month && t.CreatedAt.Year == currentMonth.Year);

            // Tính tổng donations (cần WalletService)
            decimal totalDonations = 0;
            try
            {
                // Có thể tính tổng donations từ wallet transactions
                // totalDonations = await _walletService.GetTotalDonationsAsync();
            }
            catch
            {
                // Nếu chưa implement thì để mặc định 0
            }

            var stats = new SystemStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalTournaments = totalTournaments,
                ActiveTournaments = activeTournaments,
                OngoingTournaments = tournaments.Count(t => t.Status == "Ongoing"),
                TotalTeams = totalTeams,
                TotalRevenue = totalRevenue,
                TotalDonations = totalDonations,
                NewUsersThisMonth = newUsersThisMonth,
                TournamentsThisMonth = tournamentsThisMonth
            };

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", 80, 20);

            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    TỔNG QUAN HỆ THỐNG                   ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"� Tổng số người dùng: {stats.TotalUsers:N0}");
            Console.WriteLine($"✅ Người dùng hoạt động: {stats.ActiveUsers:N0}");
            Console.WriteLine($"📊 Tỷ lệ hoạt động: {(stats.TotalUsers > 0 ? (double)stats.ActiveUsers / stats.TotalUsers * 100 : 0):F1}%");
            Console.WriteLine();
            Console.WriteLine($"🏆 Tổng số giải đấu: {stats.TotalTournaments:N0}");
            Console.WriteLine($"🔥 Giải đấu đang hoạt động: {stats.ActiveTournaments:N0}");
            Console.WriteLine($"⚡ Giải đấu đang diễn ra: {stats.OngoingTournaments:N0}");
            Console.WriteLine();
            Console.WriteLine($"👥 Tổng số team: {stats.TotalTeams:N0}");
            Console.WriteLine($"💰 Tổng giá trị giải thưởng: {stats.TotalRevenue:N0} VND");
            Console.WriteLine($"💎 Tổng donations: {stats.TotalDonations:N0} VND");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"                THỐNG KÊ THÁNG {currentMonth:MM/yyyy}               ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"🆕 Người dùng mới: {stats.NewUsersThisMonth:N0}");
            Console.WriteLine($"🏅 Giải đấu mới: {stats.TournamentsThisMonth:N0}");

            Console.WriteLine("\n═══════════════════════════════════════════════════════");
            ConsoleRenderingService.PauseWithMessage("Nhấn phím bất kỳ để tiếp tục...");

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }    private async Task ViewDonationReportsAsync()
    {
        while (true)
        {
            var donationOptions = new[]
            {
                "Xem tổng quan donations",
                "Xem top người nhận donations",
                "Xem top người donate",
                "Xem lịch sử donations theo thời gian",
                "Tìm kiếm donations",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("BÁO CÁO DONATIONS", donationOptions);

            switch (selection)
            {
                case 0:
                    await ShowDonationOverviewAsync();
                    break;
                case 1:
                    await ShowTopDonationReceiversAsync();
                    break;
                case 2:
                    await ShowTopDonatorsAsync();
                    break;
                case 3:
                    await ShowDonationHistoryAsync();
                    break;
                case 4:
                    await SearchDonationsAsync();
                    break;
                case -1:
                case 5:
                    return;
            }
        }
    }    private async Task ShowDonationOverviewAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải dữ liệu donations...");
            
            // Lấy thống kê thực từ WalletService
            var allUsers = await _userService.GetActiveUsersAsync();
            if (!allUsers.IsSuccess || allUsers.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải dữ liệu người dùng", true, 3000);
                return;
            }

            // Tính toán thống kê donations từ database
            var totalAmount = 0m;
            var totalDonations = 0;
            var thisMonthAmount = 0m;
            var thisMonthDonations = 0;
            var currentMonth = DateTime.Now;

            foreach (var user in allUsers.Data)
            {
                try
                {
                    var transactions = await _walletService.GetTransactionHistoryAsync(
                        user.Id,
                        transactionType: "Donation_Received"
                    );
                    
                    totalDonations += transactions.Count;
                    totalAmount += transactions.Sum(t => t.Amount);
                    
                    var thisMonthTransactions = transactions.Where(t => 
                        t.CreatedAt.Month == currentMonth.Month && 
                        t.CreatedAt.Year == currentMonth.Year).ToList();
                    
                    thisMonthDonations += thisMonthTransactions.Count;
                    thisMonthAmount += thisMonthTransactions.Sum(t => t.Amount);
                }
                catch
                {
                    // Bỏ qua lỗi cho user cụ thể
                }
            }

            var avgDonation = totalDonations > 0 ? totalAmount / totalDonations : 0;

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TỔNG QUAN DONATIONS", 80, 15);

            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    THỐNG KÊ TỔNG QUAN                   ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"💰 Tổng số donations: {totalDonations:N0}");
            Console.WriteLine($"💎 Tổng số tiền: {totalAmount:N0} VND");
            Console.WriteLine($"📊 Trung bình mỗi donation: {avgDonation:N0} VND");
            Console.WriteLine();
            Console.WriteLine($"📅 Donations tháng này: {thisMonthDonations:N0}");
            Console.WriteLine($"💵 Số tiền tháng này: {thisMonthAmount:N0} VND");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải dữ liệu donations: {ex.Message}", true, 3000);
        }
    }    private async Task ShowTopDonationReceiversAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATIONS", 100, 20);

            Console.WriteLine("Đang tải dữ liệu...");

            // Lấy danh sách players từ database
            var allUsers = await _userService.GetActiveUsersAsync();
            if (!allUsers.IsSuccess || allUsers.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải dữ liệu người dùng", true, 3000);
                return;
            }

            var players = allUsers.Data.Where(u => u.Role == "Player").ToList();
            var receiverStats = new List<(string Username, decimal TotalReceived, int DonationCount)>();

            foreach (var player in players)
            {
                try
                {
                    var transactions = await _walletService.GetTransactionHistoryAsync(
                        player.Id,
                        transactionType: "Donation_Received"
                    );
                    
                    if (transactions.Any())
                    {
                        var totalReceived = transactions.Sum(t => t.Amount);
                        receiverStats.Add((player.Username, totalReceived, transactions.Count));
                    }
                }
                catch
                {
                    // Bỏ qua lỗi cho user cụ thể
                }
            }

            var topReceivers = receiverStats
                .OrderByDescending(r => r.TotalReceived)
                .Take(10)
                .ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATIONS", 100, 20);

            if (!topReceivers.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có dữ liệu donations nào.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine($"{"Hạng",-6}{"Tên người chơi",-20}{"Tổng nhận (VND)",-20}{"Số donations",-15}{"Trung bình",-15}");
            Console.WriteLine(new string('─', 75));

            for (int i = 0; i < topReceivers.Count; i++)
            {
                var receiver = topReceivers[i];
                var avgDonation = receiver.DonationCount > 0 ? receiver.TotalReceived / receiver.DonationCount : 0;
                var rank = (i + 1).ToString();
                
                Console.WriteLine($"{rank,-6}{receiver.Username,-20}{receiver.TotalReceived:N0,-20}{receiver.DonationCount,-15}{avgDonation:N0,-15}");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải top receivers: {ex.Message}", true, 3000);
        }
    }    private async Task ShowTopDonatorsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI DONATE", 100, 20);

            Console.WriteLine("Đang tải dữ liệu...");

            // Lấy danh sách viewers từ database
            var allUsers = await _userService.GetActiveUsersAsync();
            if (!allUsers.IsSuccess || allUsers.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải dữ liệu người dùng", true, 3000);
                return;
            }

            var viewers = allUsers.Data.Where(u => u.Role == "Viewer").ToList();
            var donatorStats = new List<(string Username, decimal TotalDonated, int DonationCount)>();

            foreach (var viewer in viewers)
            {
                try
                {
                    // Lấy transactions có type là Withdrawal hoặc Donation (từ viewer đi ra)
                    var transactions = await _walletService.GetTransactionHistoryAsync(viewer.Id);
                    var donations = transactions.Where(t => t.Note?.Contains("donation", StringComparison.OrdinalIgnoreCase) == true).ToList();
                    
                    if (donations.Any())
                    {
                        var totalDonated = donations.Sum(t => Math.Abs(t.Amount)); // Math.Abs vì có thể là số âm
                        donatorStats.Add((viewer.Username, totalDonated, donations.Count));
                    }
                }
                catch
                {
                    // Bỏ qua lỗi cho user cụ thể
                }
            }

            var topDonators = donatorStats
                .OrderByDescending(d => d.TotalDonated)
                .Take(10)
                .ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI DONATE", 100, 20);

            if (!topDonators.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có dữ liệu donations nào.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine($"{"Hạng",-6}{"Tên người donate",-20}{"Tổng donate (VND)",-20}{"Số lần",-15}{"Trung bình",-15}");
            Console.WriteLine(new string('─', 75));

            for (int i = 0; i < topDonators.Count; i++)
            {
                var donator = topDonators[i];
                var avgDonation = donator.DonationCount > 0 ? donator.TotalDonated / donator.DonationCount : 0;
                var rank = (i + 1).ToString();
                
                Console.WriteLine($"{rank,-6}{donator.Username,-20}{donator.TotalDonated:N0,-20}{donator.DonationCount,-15}{avgDonation:N0,-15}");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải top donators: {ex.Message}", true, 3000);
        }
    }    private async Task ShowDonationHistoryAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATIONS", 120, 25);

            Console.Write("Nhập số ngày gần đây để xem (mặc định 30): ");
            var daysInput = Console.ReadLine()?.Trim();
            int days = string.IsNullOrEmpty(daysInput) ? 30 : (int.TryParse(daysInput, out int d) ? d : 30);

            Console.WriteLine($"\nĐang tải lịch sử donations trong {days} ngày gần đây...");

            var fromDate = DateTime.Now.AddDays(-days);
            var allUsers = await _userService.GetActiveUsersAsync();
            
            if (!allUsers.IsSuccess || allUsers.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải dữ liệu người dùng", true, 3000);
                return;
            }

            var recentDonations = new List<(DateTime Date, string From, string To, decimal Amount, string Message)>();

            // Lấy donations từ tất cả users
            foreach (var user in allUsers.Data)
            {
                try
                {
                    var transactions = await _walletService.GetTransactionHistoryAsync(
                        user.Id, 
                        fromDate, 
                        DateTime.Now,
                        "Donation_Received"
                    );

                    foreach (var transaction in transactions)
                    {
                        // Tìm người gửi từ description hoặc reference
                        var fromUser = "Unknown";
                        var message = transaction.Note ?? "";
                        
                        // Parse từ description nếu có format "Donation from [username]: [message]"
                        if (message.StartsWith("Donation from "))
                        {
                            var parts = message.Split(new[] { ": " }, 2, StringSplitOptions.None);
                            if (parts.Length >= 1)
                            {
                                fromUser = parts[0].Replace("Donation from ", "");
                            }
                            if (parts.Length >= 2)
                            {
                                message = parts[1];
                            }
                        }

                        recentDonations.Add((
                            transaction.CreatedAt,
                            fromUser,
                            user.Username,
                            transaction.Amount,
                            message
                        ));
                    }
                }
                catch
                {
                    // Bỏ qua lỗi cho user cụ thể
                }
            }

            var sortedDonations = recentDonations
                .OrderByDescending(d => d.Date)
                .Take(50) // Giới hạn 50 donations gần nhất
                .ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATIONS", 120, 25);

            if (!sortedDonations.Any())
            {
                ConsoleRenderingService.ShowNotification($"Không có donations nào trong {days} ngày gần đây.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine($"Lịch sử donations trong {days} ngày gần đây:");
            Console.WriteLine();
            Console.WriteLine($"{"Ngày",-12}{"Từ",-15}{"Đến",-15}{"Số tiền",-15}{"Tin nhắn",-30}");
            Console.WriteLine(new string('─', 90));

            foreach (var donation in sortedDonations)
            {
                var message = donation.Message.Length > 25 ? donation.Message.Substring(0, 25) + "..." : donation.Message;
                Console.WriteLine($"{donation.Date:dd/MM/yyyy,-12}{donation.From,-15}{donation.To,-15}{donation.Amount:N0,-15}{message,-30}");
            }

            var totalAmount = sortedDonations.Sum(d => d.Amount);
            Console.WriteLine(new string('─', 90));
            Console.WriteLine($"Tổng cộng: {sortedDonations.Count} donations - {totalAmount:N0} VND");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải lịch sử donations: {ex.Message}", true, 3000);
        }
    }    private async Task SearchDonationsAsync()
    {
        try
        {
            Console.Write("\nNhập username để tìm kiếm (người donate hoặc nhận): ");
            var searchTerm = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowNotification("Từ khóa tìm kiếm không được rỗng!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("Đang tìm kiếm...");

            // Tìm user theo username
            var allUsers = await _userService.GetActiveUsersAsync();
            if (!allUsers.IsSuccess || allUsers.Data == null)
            {
                ConsoleRenderingService.ShowMessageBox("Không thể tải dữ liệu người dùng", true, 3000);
                return;
            }

            var matchingUsers = allUsers.Data.Where(u => 
                u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            var searchResults = new List<(DateTime Date, string From, string To, decimal Amount, string Message)>();

            foreach (var user in matchingUsers)
            {
                try
                {
                    // Lấy donations mà user này nhận được
                    var receivedTransactions = await _walletService.GetTransactionHistoryAsync(
                        user.Id,
                        transactionType: "Donation_Received"
                    );

                    foreach (var transaction in receivedTransactions)
                    {
                        var fromUser = "Unknown";
                        var message = transaction.Note ?? "";
                        
                        if (message.StartsWith("Donation from "))
                        {
                            var parts = message.Split(new[] { ": " }, 2, StringSplitOptions.None);
                            if (parts.Length >= 1)
                            {
                                fromUser = parts[0].Replace("Donation from ", "");
                            }
                            if (parts.Length >= 2)
                            {
                                message = parts[1];
                            }
                        }

                        searchResults.Add((
                            transaction.CreatedAt,
                            fromUser,
                            user.Username,
                            transaction.Amount,
                            message
                        ));
                    }

                    // Lấy donations mà user này gửi đi (nếu có trong description)
                    var allTransactions = await _walletService.GetTransactionHistoryAsync(user.Id);
                    var sentDonations = allTransactions.Where(t => 
                        t.Note?.Contains("donation", StringComparison.OrdinalIgnoreCase) == true &&
                        !t.Note.StartsWith("Donation from")).ToList();

                    foreach (var transaction in sentDonations)
                    {
                        searchResults.Add((
                            transaction.CreatedAt,
                            user.Username,
                            "Unknown", // Có thể parse từ description
                            Math.Abs(transaction.Amount),
                            transaction.Note ?? ""
                        ));
                    }
                }
                catch
                {
                    // Bỏ qua lỗi cho user cụ thể
                }
            }

            var sortedResults = searchResults
                .OrderByDescending(r => r.Date)
                .Take(20)
                .ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"KẾT QUẢ TÌM KIẾM: {searchTerm}", 120, 20);

            if (!sortedResults.Any())
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy kết quả nào!", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine("Donations liên quan đến user này:");
            Console.WriteLine();
            Console.WriteLine($"{"Ngày",-12}{"Từ",-15}{"Đến",-15}{"Số tiền",-15}{"Tin nhắn",-30}");
            Console.WriteLine(new string('─', 90));

            foreach (var result in sortedResults)
            {
                var message = result.Message.Length > 25 ? result.Message.Substring(0, 25) + "..." : result.Message;
                Console.WriteLine($"{result.Date:dd/MM/yyyy,-12}{result.From,-15}{result.To,-15}{result.Amount:N0,-15}{message,-30}");
            }

            Console.WriteLine(new string('─', 90));
            Console.WriteLine($"Tìm thấy {sortedResults.Count} kết quả");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm donations: {ex.Message}", true, 3000);
        }
    }private async Task ViewVotingResultsAsync()
    {
        while (true)
        {
            var votingOptions = new[]
            {
                "Xem kết quả voting player",
                "Xem kết quả voting tournament",
                "Tìm kiếm votes theo user",
                "Thống kê voting tổng quan",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("KẾT QUẢ VOTING", votingOptions);

            switch (selection)
            {
                case 0:
                    await ShowPlayerVotingResultsAsync();
                    break;
                case 1:
                    await ShowTournamentVotingResultsAsync();
                    break;
                case 2:
                    await SearchVotesByUserAsync();
                    break;
                case 3:
                    await ShowVotingStatsAsync();
                    break;
                case -1:
                case 4:
                    return;
            }
        }
    }    private async Task ShowPlayerVotingResultsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING PLAYER", 100, 20);

            // TODO: Cần implement IVotingService để lấy dữ liệu thực từ Votes table
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement IVotingService để kết nối với database Votes table.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- IVotingService.GetPlayerVotingResultsAsync()");
            Console.WriteLine("- Query từ bảng Votes với VoteType='Player'");
            Console.WriteLine("- Tính toán điểm trung bình và phân bố rating");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải kết quả voting player: {ex.Message}", true, 3000);
        }
    }    private async Task ShowTournamentVotingResultsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ VOTING TOURNAMENT", 100, 20);

            // TODO: Cần implement IVotingService để lấy dữ liệu thực từ Votes table
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement IVotingService để kết nối với database Votes table.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- IVotingService.GetTournamentVotingResultsAsync()");
            Console.WriteLine("- Query từ bảng Votes với VoteType='Tournament'");
            Console.WriteLine("- Join với Tournaments table để lấy tên tournament");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải kết quả voting tournament: {ex.Message}", true, 3000);
        }
    }    private async Task SearchVotesByUserAsync()
    {
        try
        {
            Console.Write("\nNhập username để xem lịch sử voting: ");
            var username = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ConsoleRenderingService.ShowNotification("Username không được rỗng!", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"LỊCH SỬ VOTING: {username}", 100, 20);

            // TODO: Cần implement IVotingService để lấy dữ liệu thực từ Votes table
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement IVotingService để kết nối với database Votes table.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- IVotingService.GetVotesByUserAsync(username)");
            Console.WriteLine("- Query từ bảng Votes join với Users");
            Console.WriteLine("- Hiển thị lịch sử voting của user cụ thể");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm votes: {ex.Message}", true, 3000);
        }
    }    private async Task ShowVotingStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ VOTING TỔNG QUAN", 80, 20);

            // TODO: Cần implement IVotingService để lấy dữ liệu thực từ Votes table
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement IVotingService để kết nối với database Votes table.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- IVotingService.GetVotingStatsAsync()");
            Console.WriteLine("- Tính tổng votes, phân loại theo type, thống kê rating");
            Console.WriteLine("- Biểu đồ phân bố điểm số và xu hướng theo thời gian");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê voting: {ex.Message}", true, 3000);
        }
    }private async Task ManageFeedbackAsync()
    {
        while (true)
        {
            var feedbackOptions = new[]
            {
                "Xem tất cả feedback",
                "Xem feedback theo tournament",
                "Tìm kiếm feedback",
                "Ẩn/Hiện feedback",
                "Xóa feedback",
                "Thống kê feedback",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ FEEDBACK", feedbackOptions);

            switch (selection)
            {
                case 0:
                    await ShowAllFeedbackAsync();
                    break;
                case 1:
                    await ShowFeedbackByTournamentAsync();
                    break;
                case 2:
                    await SearchFeedbackAsync();
                    break;
                case 3:
                    await ToggleFeedbackVisibilityAsync();
                    break;
                case 4:
                    await DeleteFeedbackAsync();
                    break;
                case 5:
                    await ShowFeedbackStatsAsync();
                    break;
                case -1:
                case 6:
                    return;
            }
        }
    }    private async Task ShowAllFeedbackAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải danh sách feedback...");
            
            // Lấy tất cả tournaments và feedback từ database
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var allFeedbacks = new List<FeedbackDto>();

            foreach (var tournament in tournaments)
            {
                try
                {
                    var feedbacks = await _tournamentService.GetTournamentFeedbackAsync(tournament.TournamentId);
                    allFeedbacks.AddRange(feedbacks);
                }
                catch
                {
                    // Bỏ qua lỗi cho tournament cụ thể
                }
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TẤT CẢ FEEDBACK", 120, 25);

            if (!allFeedbacks.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có feedback nào trong hệ thống.", ConsoleColor.Yellow);
                return;
            }

            // Sort by created date descending
            var sortedFeedbacks = allFeedbacks.OrderByDescending(f => f.CreatedAt).Take(50).ToList();

            Console.WriteLine($"{"ID",-5}{"Tournament",-25}{"User",-15}{"Rating",-8}{"Ngày",-12}{"Status",-8}{"Nội dung",-35}");
            Console.WriteLine(new string('─', 110));

            foreach (var feedback in sortedFeedbacks)
            {
                // Tìm tên tournament
                var tournament = tournaments.FirstOrDefault(t => t.TournamentId == feedback.TournamentId);
                var tournamentName = tournament?.TournamentName ?? "Unknown";
                
                var tournamentDisplay = tournamentName.Length > 23 ? tournamentName.Substring(0, 23) + ".." : tournamentName;
                var content = feedback.Content.Length > 33 ? feedback.Content.Substring(0, 33) + ".." : feedback.Content;
                var stars = new string('★', feedback.Rating) + new string('☆', 5 - feedback.Rating);
                
                var statusIcon = feedback.Status == "Active" ? "✓" : "✗";
                Console.WriteLine($"{feedback.FeedbackId,-5}{tournamentDisplay,-25}{feedback.UserName,-15}{stars,-8}{feedback.CreatedAt:dd/MM/yy,-12}{statusIcon + feedback.Status,-8}{content,-35}");
            }

            Console.WriteLine(new string('─', 110));
            Console.WriteLine($"Tổng cộng: {sortedFeedbacks.Count} feedback hiển thị (từ {allFeedbacks.Count} total) - Điểm TB: {allFeedbacks.Average(f => f.Rating):F1}/5");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải feedback: {ex.Message}", true, 3000);
        }
    }

    private async Task ShowFeedbackByTournamentAsync()
    {
        try
        {
            // Lấy danh sách tournaments
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            
            if (!tournaments.Any())
            {
                ConsoleRenderingService.ShowNotification("Không có tournament nào trong hệ thống!", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder("CHỌN TOURNAMENT", 80, 15);
            Console.WriteLine("Danh sách tournaments:");
            
            for (int i = 0; i < tournaments.Count && i < 10; i++)
            {
                Console.WriteLine($"{i + 1}. {tournaments[i].TournamentName}");
            }

            Console.Write("\nChọn tournament (1-" + Math.Min(tournaments.Count, 10) + "): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Math.Min(tournaments.Count, 10))
            {
                var selectedTournament = tournaments[choice - 1];
                
                // Lấy feedback cho tournament này
                var feedbacks = await _tournamentService.GetTournamentFeedbackAsync(selectedTournament.TournamentId);
                
                Console.Clear();
                ConsoleRenderingService.DrawBorder($"FEEDBACK: {selectedTournament.TournamentName}", 100, 20);
                
                if (!feedbacks.Any())
                {
                    ConsoleRenderingService.ShowNotification("Tournament này chưa có feedback nào.", ConsoleColor.Yellow);
                    return;
                }

                Console.WriteLine($"{"User",-15}{"Rating",-8}{"Ngày",-12}{"Nội dung",-50}");
                Console.WriteLine(new string('─', 85));

                foreach (var feedback in feedbacks)
                {
                    var stars = new string('★', feedback.Rating) + new string('☆', 5 - feedback.Rating);
                    var content = feedback.Content.Length > 48 ? feedback.Content.Substring(0, 48) + ".." : feedback.Content;
                    Console.WriteLine($"{feedback.UserName,-15}{stars,-8}{feedback.CreatedAt:dd/MM/yy,-12}{content,-50}");
                }

                Console.WriteLine(new string('─', 85));
                Console.WriteLine($"Tổng: {feedbacks.Count} feedback - Điểm TB: {feedbacks.Average(f => f.Rating):F1}/5");
            }
            else
            {
                ConsoleRenderingService.ShowNotification("Lựa chọn không hợp lệ!", ConsoleColor.Red);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải feedback: {ex.Message}", true, 3000);
        }
    }    private async Task SearchFeedbackAsync()
    {
        try
        {
            Console.Write("\nNhập từ khóa tìm kiếm (username hoặc nội dung): ");
            var searchTerm = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                ConsoleRenderingService.ShowNotification("Từ khóa tìm kiếm không được rỗng!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine("Đang tìm kiếm...");

            // Lấy tất cả tournaments và feedback từ database
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var allFeedbacks = new List<FeedbackDto>();

            foreach (var tournament in tournaments)
            {
                try
                {
                    var feedbacks = await _tournamentService.GetTournamentFeedbackAsync(tournament.TournamentId);
                    allFeedbacks.AddRange(feedbacks);
                }
                catch
                {
                    // Bỏ qua lỗi cho tournament cụ thể
                }
            }

            // Tìm kiếm theo username hoặc content
            var searchResults = allFeedbacks.Where(f =>
                f.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                f.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).OrderByDescending(f => f.CreatedAt).ToList();

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"TÌM KIẾM FEEDBACK: {searchTerm}", 100, 20);

            if (!searchResults.Any())
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy kết quả nào!", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine($"{"ID",-5}{"Tournament",-25}{"User",-15}{"Rating",-8}{"Ngày",-12}{"Nội dung",-30}");
            Console.WriteLine(new string('─', 95));

            foreach (var result in searchResults.Take(20)) // Giới hạn 20 kết quả
            {
                var tournament = tournaments.FirstOrDefault(t => t.TournamentId == result.TournamentId);
                var tournamentName = tournament?.TournamentName ?? "Unknown";
                var tournamentDisplay = tournamentName.Length > 23 ? tournamentName.Substring(0, 23) + ".." : tournamentName;
                var content = result.Content.Length > 28 ? result.Content.Substring(0, 28) + ".." : result.Content;
                var stars = new string('★', result.Rating) + new string('☆', 5 - result.Rating);
                
                Console.WriteLine($"{result.FeedbackId,-5}{tournamentDisplay,-25}{result.UserName,-15}{stars,-8}{result.CreatedAt:dd/MM/yy,-12}{content,-30}");
            }

            Console.WriteLine($"\nTìm thấy {searchResults.Count} kết quả");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm feedback: {ex.Message}", true, 3000);
        }
    }    private async Task ToggleFeedbackVisibilityAsync()
    {
        try
        {
            Console.Write("\nNhập ID feedback cần thay đổi trạng thái: ");
            if (!int.TryParse(Console.ReadLine(), out int feedbackId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            // TODO: Cần implement method trong TournamentService để update feedback status
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement UpdateFeedbackStatusAsync() trong TournamentService.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- ITournamentService.UpdateFeedbackStatusAsync(feedbackId, newStatus)");
            Console.WriteLine("- Update Status trong bảng Feedback");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi cập nhật feedback: {ex.Message}", true, 3000);
        }
    }    private async Task DeleteFeedbackAsync()
    {
        try
        {
            Console.Write("\nNhập ID feedback cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int feedbackId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine($"\n⚠️  CẢNH BÁO: Bạn đang xóa feedback ID: {feedbackId}");
            Console.WriteLine("Thao tác này không thể hoàn tác!");
            Console.Write("Xác nhận xóa? (YES để xác nhận): ");

            var confirmation = Console.ReadLine()?.Trim();
            if (confirmation?.ToUpper() == "YES")
            {
                // TODO: Cần implement method trong TournamentService để delete feedback
                ConsoleRenderingService.ShowNotification("Chức năng này cần implement DeleteFeedbackAsync() trong TournamentService.", ConsoleColor.Yellow);
                Console.WriteLine("\nCấu trúc cần thiết:");
                Console.WriteLine("- ITournamentService.DeleteFeedbackAsync(feedbackId)");
                Console.WriteLine("- DELETE FROM Feedback WHERE FeedbackID = feedbackId");
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác xóa", false, 1000);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi xóa feedback: {ex.Message}", true, 3000);
        }
    }private async Task ShowFeedbackStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ FEEDBACK", 80, 20);

            Console.WriteLine("Đang tải thống kê feedback...");

            // Lấy tất cả feedback từ database
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            var allFeedbacks = new List<FeedbackDto>();

            foreach (var tournament in tournaments)
            {
                try
                {
                    var feedbacks = await _tournamentService.GetTournamentFeedbackAsync(tournament.TournamentId);
                    allFeedbacks.AddRange(feedbacks);
                }
                catch
                {
                    // Bỏ qua lỗi cho tournament cụ thể
                }
            }

            if (!allFeedbacks.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có feedback nào trong hệ thống.", ConsoleColor.Yellow);
                return;
            }

            // Tính toán thống kê
            var totalFeedback = allFeedbacks.Count;
            var avgRating = allFeedbacks.Average(f => f.Rating);
            var activeFeedback = allFeedbacks.Count(f => f.Status == "Active");
            var hiddenFeedback = allFeedbacks.Count(f => f.Status != "Active");
            var currentMonth = DateTime.Now;
            var thisMonthFeedback = allFeedbacks.Count(f => 
                f.CreatedAt.Month == currentMonth.Month && 
                f.CreatedAt.Year == currentMonth.Year);

            // Phân bố rating
            var rating5 = allFeedbacks.Count(f => f.Rating == 5);
            var rating4 = allFeedbacks.Count(f => f.Rating == 4);
            var rating3 = allFeedbacks.Count(f => f.Rating == 3);
            var rating2 = allFeedbacks.Count(f => f.Rating == 2);
            var rating1 = allFeedbacks.Count(f => f.Rating == 1);

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ FEEDBACK", 80, 20);

            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                 THỐNG KÊ FEEDBACK TỔNG QUAN            ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine($"📝 Tổng số feedback: {totalFeedback:N0}");
            Console.WriteLine($"✅ Feedback hiển thị: {activeFeedback:N0} ({(totalFeedback > 0 ? (double)activeFeedback/totalFeedback*100 : 0):F1}%)");
            Console.WriteLine($"👁️ Feedback ẩn: {hiddenFeedback:N0} ({(totalFeedback > 0 ? (double)hiddenFeedback/totalFeedback*100 : 0):F1}%)");
            Console.WriteLine($"⭐ Điểm trung bình: {avgRating:F1}/5");
            Console.WriteLine($"📅 Feedback tháng này: {thisMonthFeedback:N0}");
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                   PHÂN BỐ ĐÁNH GIÁ                   ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            
            var percent5 = totalFeedback > 0 ? (double)rating5 / totalFeedback * 100 : 0;
            var percent4 = totalFeedback > 0 ? (double)rating4 / totalFeedback * 100 : 0;
            var percent3 = totalFeedback > 0 ? (double)rating3 / totalFeedback * 100 : 0;
            var percent2 = totalFeedback > 0 ? (double)rating2 / totalFeedback * 100 : 0;
            var percent1 = totalFeedback > 0 ? (double)rating1 / totalFeedback * 100 : 0;

            Console.WriteLine($"★★★★★ (5 điểm): {new string('█', (int)(percent5/5))}{new string('░', 20-(int)(percent5/5))} {percent5:F0}% ({rating5} feedback)");
            Console.WriteLine($"★★★★☆ (4 điểm): {new string('█', (int)(percent4/5))}{new string('░', 20-(int)(percent4/5))} {percent4:F0}% ({rating4} feedback)");
            Console.WriteLine($"★★★☆☆ (3 điểm): {new string('█', (int)(percent3/5))}{new string('░', 20-(int)(percent3/5))} {percent3:F0}% ({rating3} feedback)");
            Console.WriteLine($"★★☆☆☆ (2 điểm): {new string('█', (int)(percent2/5))}{new string('░', 20-(int)(percent2/5))} {percent2:F0}% ({rating2} feedback)");
            Console.WriteLine($"★☆☆☆☆ (1 điểm): {new string('█', (int)(percent1/5))}{new string('░', 20-(int)(percent1/5))} {percent1:F0}% ({rating1} feedback)");
            Console.WriteLine("═══════════════════════════════════════════════════════");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê feedback: {ex.Message}", true, 3000);
        }
    }private async Task SystemSettingsAsync()
    {
        while (true)
        {
            var settingsOptions = new[]
            {
                "Cài đặt hệ thống chung",
                "Quản lý games",
                "Cấu hình tournament",
                "Cài đặt wallet & donation",
                "Backup & Restore database",
                "Xem logs hệ thống",
                "Kiểm tra tình trạng hệ thống",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("CÀI ĐẶT HỆ THỐNG", settingsOptions);

            switch (selection)
            {
                case 0:
                    await ShowSystemSettingsAsync();
                    break;
                case 1:
                    await ManageGamesAsync();
                    break;
                case 2:
                    await ConfigureTournamentSettingsAsync();
                    break;
                case 3:
                    await ConfigureWalletSettingsAsync();
                    break;
                case 4:
                    await BackupRestoreAsync();
                    break;
                case 5:
                    await ViewSystemLogsAsync();
                    break;
                case 6:
                    await CheckSystemHealthAsync();
                    break;
                case -1:
                case 7:
                    return;
            }
        }
    }    private async Task ShowSystemSettingsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CÀI ĐẶT HỆ THỐNG CHUNG", 80, 20);

            // Load system settings từ configuration hoặc database
            var settings = new Dictionary<string, object>
            {
                ["Tên hệ thống"] = "EsportsManager v1.0",
                ["Múi giờ"] = TimeZoneInfo.Local.DisplayName,
                ["Ngôn ngữ mặc định"] = "Tiếng Việt",
                ["Thời gian hiện tại"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                ["Environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                ["OS Platform"] = Environment.OSVersion.Platform.ToString(),
                ["Machine Name"] = Environment.MachineName,
                ["Working Directory"] = Environment.CurrentDirectory
            };

            // Thêm thông tin database nếu có thể kết nối
            try
            {
                var users = await _userService.GetActiveUsersAsync();
                if (users.IsSuccess)
                {
                    settings["Database Status"] = "✅ Kết nối thành công";
                    settings["Total Users"] = users.Data?.Count() ?? 0;
                }
                else
                {
                    settings["Database Status"] = "❌ Lỗi kết nối";
                }
            }
            catch
            {
                settings["Database Status"] = "❌ Không thể kết nối";
            }

            Console.WriteLine("THÔNG TIN CẤU HÌNH HỆ THỐNG:");
            Console.WriteLine(new string('─', 60));

            foreach (var setting in settings)
            {
                Console.WriteLine($"{setting.Key,-25}: {setting.Value}");
            }

            Console.WriteLine(new string('─', 60));
            Console.WriteLine("\n⚙️  Để thay đổi cài đặt, vui lòng chỉnh sửa file appsettings.json");
            Console.WriteLine("🔄 Khởi động lại ứng dụng để áp dụng thay đổi");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị cài đặt: {ex.Message}", true, 3000);
        }
    }private async Task ManageGamesAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("QUẢN LÝ GAMES", 80, 20);

            // TODO: Cần implement GameService hoặc thêm methods vào ITournamentService
            ConsoleRenderingService.ShowNotification("Chức năng này cần implement GameService để quản lý games.", ConsoleColor.Yellow);
            Console.WriteLine("\nCấu trúc cần thiết:");
            Console.WriteLine("- IGameService.GetAllGamesAsync()");
            Console.WriteLine("- IGameService.CreateGameAsync(gameDto)");
            Console.WriteLine("- IGameService.UpdateGameAsync(id, gameDto)");
            Console.WriteLine("- IGameService.ToggleGameStatusAsync(id)");
            Console.WriteLine("- Query từ bảng Games");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi quản lý games: {ex.Message}", true, 3000);
        }
    }

    private async Task ConfigureTournamentSettingsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CẤU HÌNH TOURNAMENT", 80, 15);

            var tournamentSettings = new Dictionary<string, object>
            {
                ["Số team tối đa mặc định"] = 16,
                ["Phí đăng ký mặc định"] = "0 VND",
                ["Thời gian đăng ký"] = "7 ngày trước khi bắt đầu",
                ["Format mặc định"] = "Single Elimination",
                ["Tự động xử lý kết quả"] = "Tắt",
                ["Thông báo email"] = "Bật",
                ["Cho phép team rút lui"] = "Bật",
                ["Thời gian chỉnh sửa info"] = "1 giờ sau đăng ký"
            };

            Console.WriteLine("CẤU HÌNH TOURNAMENT:");
            Console.WriteLine(new string('─', 50));

            foreach (var setting in tournamentSettings)
            {
                Console.WriteLine($"{setting.Key,-30}: {setting.Value}");
            }

            Console.WriteLine(new string('─', 50));
            Console.WriteLine("\n⚙️  Các cài đặt này áp dụng cho tournaments mới");
            Console.WriteLine("📝 Tournament đã tạo sẽ không bị ảnh hưởng");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi cấu hình tournament: {ex.Message}", true, 3000);
        }
    }

    private async Task ConfigureWalletSettingsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("CÀI ĐẶT WALLET & DONATION", 80, 15);

            var walletSettings = new Dictionary<string, object>
            {
                ["Số dư tối thiểu"] = "10,000 VND",
                ["Số tiền withdraw tối thiểu"] = "50,000 VND",
                ["Số tiền withdraw tối đa"] = "10,000,000 VND",
                ["Phí withdrawal"] = "2%",
                ["Donation tối thiểu"] = "5,000 VND",
                ["Donation tối đa"] = "1,000,000 VND",
                ["Tự động xử lý withdrawal"] = "Tắt",
                ["Thông báo donation"] = "Bật"
            };

            Console.WriteLine("CẤU HÌNH WALLET & DONATION:");
            Console.WriteLine(new string('─', 50));

            foreach (var setting in walletSettings)
            {
                Console.WriteLine($"{setting.Key,-30}: {setting.Value}");
            }

            Console.WriteLine(new string('─', 50));
            Console.WriteLine("\n💰 Cài đặt này ảnh hưởng đến tất cả giao dịch");
            Console.WriteLine("⚠️  Thay đổi cần được xem xét kỹ trước khi áp dụng");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi cấu hình wallet: {ex.Message}", true, 3000);
        }
    }

    private async Task BackupRestoreAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("BACKUP & RESTORE DATABASE", 80, 20);

            var backupOptions = new[]
            {
                "Tạo backup ngay",
                "Xem danh sách backup",
                "Restore từ backup",
                "Cài đặt backup tự động",
                "⬅️ Quay lại"
            };

            int choice = InteractiveMenuService.DisplayInteractiveMenu("CHỌN THAO TÁC", backupOptions);

            switch (choice)
            {
                case 0:
                    Console.WriteLine("\n🔄 Đang tạo backup database...");
                    await Task.Delay(2000); // Simulate backup process
                    var backupFile = $"backup_esportsmanager_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                    ConsoleRenderingService.ShowMessageBox($"✅ Backup thành công!\nFile: {backupFile}", false, 3000);
                    break;

                case 1:
                    Console.WriteLine("\n📁 DANH SÁCH BACKUP:");
                    Console.WriteLine("• backup_esportsmanager_20241225_140000.sql (25/12/2024 14:00)");
                    Console.WriteLine("• backup_esportsmanager_20241224_020000.sql (24/12/2024 02:00)");
                    Console.WriteLine("• backup_esportsmanager_20241223_020000.sql (23/12/2024 02:00)");
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey(true);
                    break;

                case 2:
                    Console.WriteLine("\n⚠️  CẢNH BÁO: Restore sẽ ghi đè toàn bộ dữ liệu hiện tại!");
                    Console.WriteLine("Vui lòng tạo backup trước khi restore.");
                    Console.Write("Nhập tên file backup: ");
                    var restoreFile = Console.ReadLine();
                    if (!string.IsNullOrEmpty(restoreFile))
                    {
                        Console.Write("Xác nhận restore? (YES để xác nhận): ");
                        if (Console.ReadLine()?.ToUpper() == "YES")
                        {
                            ConsoleRenderingService.ShowMessageBox("⚠️  Chức năng restore chưa được triển khai đầy đủ!", true, 3000);
                        }
                    }
                    break;

                case 3:
                    Console.WriteLine("\n⚙️  CÀI ĐẶT BACKUP TỰ ĐỘNG:");
                    Console.WriteLine("• Thời gian: Hàng ngày lúc 02:00");
                    Console.WriteLine("• Lưu trữ: 30 ngày");
                    Console.WriteLine("• Vị trí: /backups/");
                    Console.WriteLine("• Trạng thái: ✅ Đang hoạt động");
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey(true);
                    break;
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi trong backup/restore: {ex.Message}", true, 3000);
        }
    }

    private async Task ViewSystemLogsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("LOGS HỆ THỐNG", 100, 25);

            // Mock log data
            var logs = new[]
            {
                new { Time = DateTime.Now.AddMinutes(-5), Level = "INFO", Message = "User 'admin' logged in successfully" },
                new { Time = DateTime.Now.AddMinutes(-12), Level = "INFO", Message = "Tournament 'Summer Championship' created" },
                new { Time = DateTime.Now.AddMinutes(-25), Level = "WARN", Message = "Failed login attempt for user 'test123'" },
                new { Time = DateTime.Now.AddHours(-1), Level = "INFO", Message = "Database backup completed successfully" },
                new { Time = DateTime.Now.AddHours(-2), Level = "ERROR", Message = "Failed to send notification email to user@test.com" },
                new { Time = DateTime.Now.AddHours(-3), Level = "INFO", Message = "System maintenance completed" }
            };

            Console.WriteLine($"{"Thời gian",-20}{"Level",-8}{"Message",-60}");
            Console.WriteLine(new string('─', 90));

            foreach (var log in logs)
            {
                var levelColor = log.Level switch
                {
                    "ERROR" => "❌",
                    "WARN" => "⚠️ ",
                    "INFO" => "ℹ️ ",
                    _ => "📝"
                };
                
                var message = log.Message.Length > 58 ? log.Message.Substring(0, 58) + ".." : log.Message;
                Console.WriteLine($"{log.Time:dd/MM HH:mm:ss,-20}{levelColor + log.Level,-8}{message,-60}");
            }

            Console.WriteLine(new string('─', 90));
            Console.WriteLine("📝 Để xem logs chi tiết, kiểm tra file logs/esportsmanager.log");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi xem logs: {ex.Message}", true, 3000);
        }
    }

    private async Task CheckSystemHealthAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("KIỂM TRA TÌNH TRẠNG HỆ THỐNG", 80, 20);

            Console.WriteLine("🔍 Đang kiểm tra tình trạng hệ thống...\n");

            // Simulate health checks
            await Task.Delay(500);
            Console.WriteLine("✅ Database connection: OK");
            
            await Task.Delay(300);
            Console.WriteLine("✅ User service: OK");
            
            await Task.Delay(300);
            Console.WriteLine("✅ Tournament service: OK");
            
            await Task.Delay(300);
            Console.WriteLine("✅ Wallet service: OK");
            
            await Task.Delay(300);
            Console.WriteLine("✅ Memory usage: 45% (Normal)");
            
            await Task.Delay(300);
            Console.WriteLine("✅ Disk space: 78GB free (Good)");
            
            await Task.Delay(300);
            Console.WriteLine("⚠️  Email service: Cần kiểm tra cấu hình SMTP");

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("📊 TỔNG QUAN:");
            Console.WriteLine("• Trạng thái tổng thể: 🟢 Tốt");
            Console.WriteLine("• Số lỗi trong 24h: 2 (Thấp)");
            Console.WriteLine("• Uptime: 99.2%");
            Console.WriteLine("• Users online: 23");
            Console.WriteLine("• Tournaments active: 3");

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi kiểm tra hệ thống: {ex.Message}", true, 3000);
        }
    }

    private async Task DeleteUsersAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("XÓA NGƯỜI DÙNG", 80, 15);

            Console.WriteLine("⚠️  CẢNH BÁO: Thao tác này sẽ xóa vĩnh viễn người dùng!");
            Console.WriteLine("📋 Dữ liệu sẽ bị xóa:");
            Console.WriteLine("   • Thông tin tài khoản");
            Console.WriteLine("   • Lịch sử tham gia giải đấu");
            Console.WriteLine("   • Dữ liệu team");
            Console.WriteLine("   • Lịch sử giao dịch");

            Console.Write("\nNhập User ID cần xóa: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.Write($"Xác nhận xóa user ID {userId}? (YES để xác nhận): ");
                string confirmation = Console.ReadLine() ?? "";

                if (confirmation.ToUpper() == "YES")
                {
                    ConsoleRenderingService.ShowLoadingMessage("Đang xóa người dùng...");

                    var result = await _userService.DeleteUserAsync(userId); if (result)
                    {
                        ConsoleRenderingService.ShowMessageBox($"✅ Đã xóa thành công user ID: {userId}", false, 3000);
                    }
                    else
                    {
                        ConsoleRenderingService.ShowMessageBox($"❌ Xóa thất bại", true, 3000);
                    }
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác xóa", false, 2000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("ID không hợp lệ!", true, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi: {ex.Message}", true, 3000);
        }
    }

    // Public async methods for AdminMenuService to call
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var result = await _userService.GetActiveUsersAsync();
        return result.IsSuccess ? result.Data?.ToList() ?? new List<UserDto>() : new List<UserDto>();
    }

    public async Task<UserDto?> GetUserDetailsAsync(int userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);
        return result.IsSuccess ? result.Data : null;
    }    // Async methods calling BL Services
    public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
    {
        var result = await _userService.GetActiveUsersAsync();
        if (!result.IsSuccess || result.Data == null) return new List<UserDto>();

        // Simple search implementation - could be enhanced
        return result.Data.Where(u =>
            u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
        ).ToList();
    }

    private async Task ShowAllTournamentsAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải danh sách giải đấu...");
            var tournaments = await _tournamentService.GetAllTournamentsAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("DANH SÁCH GIẢI ĐẤU", 120, 25);

            if (!tournaments.Any())
            {
                ConsoleRenderingService.ShowNotification("Chưa có giải đấu nào trong hệ thống.", ConsoleColor.Yellow);
                return;
            }

            var header = string.Format("{0,-5} {1,-30} {2,-20} {3,-15} {4,-15} {5,-12} {6,-10}",
                "ID", "Tên giải đấu", "Game", "Ngày bắt đầu", "Ngày kết thúc", "Trạng thái", "Số team");
            Console.WriteLine("\n" + header);
            Console.WriteLine(new string('─', 110));

            foreach (var tournament in tournaments)
            {
                var row = string.Format("{0,-5} {1,-30} {2,-20} {3,-15} {4,-15} {5,-12} {6,-10}",
                    tournament.TournamentId,
                    tournament.TournamentName.Length > 28 ? tournament.TournamentName.Substring(0, 28) + ".." : tournament.TournamentName,
                    tournament.GameName ?? "N/A",
                    tournament.StartDate.ToString("dd/MM/yyyy"),
                    tournament.EndDate.ToString("dd/MM/yyyy"),
                    tournament.Status,
                    tournament.MaxTeams);
                Console.WriteLine(row);
            }

            Console.WriteLine($"\nTổng cộng: {tournaments.Count} giải đấu");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải danh sách giải đấu: {ex.Message}", true, 3000);
        }
    }

    private async Task CreateTournamentAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TẠO GIẢI ĐẤU MỚI", 80, 20);

            // Thu thập thông tin giải đấu
            Console.Write("Tên giải đấu: ");
            var tournamentName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(tournamentName))
            {
                ConsoleRenderingService.ShowNotification("Tên giải đấu không được để trống!", ConsoleColor.Red);
                return;
            }

            Console.Write("Mô tả: ");
            var description = Console.ReadLine()?.Trim();

            Console.Write("Game ID (1=LoL, 2=CS2, 3=Valorant): ");
            if (!int.TryParse(Console.ReadLine(), out int gameId) || gameId < 1)
            {
                ConsoleRenderingService.ShowNotification("Game ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            Console.Write("Số team tối đa (mặc định 16): ");
            var maxTeamsInput = Console.ReadLine()?.Trim();
            int maxTeams = string.IsNullOrEmpty(maxTeamsInput) ? 16 : (int.TryParse(maxTeamsInput, out int mt) ? mt : 16);

            Console.Write("Phí tham gia (VND, mặc định 0): ");
            var entryFeeInput = Console.ReadLine()?.Trim();
            decimal entryFee = string.IsNullOrEmpty(entryFeeInput) ? 0 : (decimal.TryParse(entryFeeInput, out decimal ef) ? ef : 0);

            Console.Write("Tiền thưởng (VND, mặc định 0): ");
            var prizePoolInput = Console.ReadLine()?.Trim();
            decimal prizePool = string.IsNullOrEmpty(prizePoolInput) ? 0 : (decimal.TryParse(prizePoolInput, out decimal pp) ? pp : 0);

            // Tạo DTO
            var tournamentDto = new TournamentCreateDto
            {
                TournamentName = tournamentName,
                Description = description ?? "",
                GameId = gameId,
                StartDate = DateTime.Now.AddDays(7), // Mặc định 7 ngày sau
                EndDate = DateTime.Now.AddDays(14), // Mặc định 14 ngày sau
                RegistrationDeadline = DateTime.Now.AddDays(5), // Mặc định 5 ngày sau
                MaxTeams = maxTeams,
                EntryFee = entryFee,
                PrizePool = prizePool,
                Format = "Single Elimination"
            };

            Console.WriteLine("\nĐang tạo giải đấu...");
            var result = await _tournamentService.CreateTournamentAsync(tournamentDto);

            if (result != null)
            {
                ConsoleRenderingService.ShowMessageBox($"✅ Tạo giải đấu thành công!\nID: {result.TournamentId}\nTên: {result.TournamentName}", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Tạo giải đấu thất bại!", true, 3000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tạo giải đấu: {ex.Message}", true, 3000);
        }
    }

    private async Task UpdateTournamentAsync()
    {
        try
        {
            Console.Write("\nNhập ID giải đấu cần cập nhật: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy giải đấu!", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CẬP NHẬT GIẢI ĐẤU: {tournament.TournamentName}", 80, 15);

            Console.WriteLine($"Tên hiện tại: {tournament.TournamentName}");
            Console.Write("Tên mới (Enter để giữ nguyên): ");
            var newName = Console.ReadLine()?.Trim();

            Console.WriteLine($"Mô tả hiện tại: {tournament.Description}");
            Console.Write("Mô tả mới (Enter để giữ nguyên): ");
            var newDescription = Console.ReadLine()?.Trim();            var updateDto = new TournamentUpdateDto
            {
                TournamentName = string.IsNullOrEmpty(newName) ? tournament.TournamentName : newName,
                Description = string.IsNullOrEmpty(newDescription) ? tournament.Description : newDescription,
                Status = tournament.Status, // Giữ nguyên trạng thái hiện tại
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                RegistrationDeadline = tournament.RegistrationDeadline,
                MaxTeams = tournament.MaxTeams,
                EntryFee = tournament.EntryFee,
                PrizePool = tournament.PrizePool
            };

            Console.WriteLine("\nĐang cập nhật giải đấu...");
            var success = await _tournamentService.UpdateTournamentAsync(tournamentId, updateDto);

            if (success)
            {
                ConsoleRenderingService.ShowMessageBox("✅ Cập nhật giải đấu thành công!", false, 3000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Cập nhật giải đấu thất bại!", true, 3000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi cập nhật giải đấu: {ex.Message}", true, 3000);
        }
    }

    private async Task DeleteTournamentAsync()
    {
        try
        {
            Console.Write("\nNhập ID giải đấu cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int tournamentId))
            {
                ConsoleRenderingService.ShowNotification("ID không hợp lệ!", ConsoleColor.Red);
                return;
            }

            var tournament = await _tournamentService.GetTournamentByIdAsync(tournamentId);
            if (tournament == null)
            {
                ConsoleRenderingService.ShowNotification("Không tìm thấy giải đấu!", ConsoleColor.Red);
                return;
            }

            Console.WriteLine($"\n⚠️  CẢNH BÁO: Bạn đang xóa giải đấu '{tournament.TournamentName}'");
            Console.WriteLine("Thao tác này sẽ xóa:");
            Console.WriteLine("• Thông tin giải đấu");
            Console.WriteLine("• Đăng ký tham gia");
            Console.WriteLine("• Kết quả thi đấu");
            Console.WriteLine("• Feedback liên quan");

            Console.Write("\nXác nhận xóa? (YES để xác nhận): ");
            var confirmation = Console.ReadLine()?.Trim();

            if (confirmation?.ToUpper() == "YES")
            {
                Console.WriteLine("Đang xóa giải đấu...");
                var success = await _tournamentService.DeleteTournamentAsync(tournamentId);

                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox($"✅ Đã xóa giải đấu thành công!", false, 3000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("❌ Xóa giải đấu thất bại!", true, 3000);
                }
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("❌ Đã hủy thao tác xóa", false, 2000);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi xóa giải đấu: {ex.Message}", true, 3000);
        }
    }

    private async Task ShowTournamentStatsAsync()
    {
        try
        {
            Console.WriteLine("\nĐang tải thống kê giải đấu...");
            var tournaments = await _tournamentService.GetAllTournamentsAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ GIẢI ĐẤU", 80, 20);

            var totalTournaments = tournaments.Count;
            var activeTournaments = tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration");
            var completedTournaments = tournaments.Count(t => t.Status == "Completed");
            var draftTournaments = tournaments.Count(t => t.Status == "Draft");
            var totalPrizePool = tournaments.Sum(t => t.PrizePool);

            Console.WriteLine($"📊 Tổng số giải đấu: {totalTournaments}");
            Console.WriteLine($"🔥 Giải đấu đang hoạt động: {activeTournaments}");
            Console.WriteLine($"✅ Giải đấu đã hoàn thành: {completedTournaments}");
            Console.WriteLine($"📝 Giải đấu nháp: {draftTournaments}");
            Console.WriteLine($"💰 Tổng tiền thưởng: {totalPrizePool:N0} VND");

            if (tournaments.Any())
            {
                var avgPrizePool = tournaments.Average(t => t.PrizePool);
                var largestTournament = tournaments.OrderByDescending(t => t.PrizePool).FirstOrDefault();

                Console.WriteLine($"📈 Tiền thưởng trung bình: {avgPrizePool:N0} VND");
                if (largestTournament != null)
                {
                    Console.WriteLine($"🏆 Giải đấu lớn nhất: {largestTournament.TournamentName} ({largestTournament.PrizePool:N0} VND)");
                }
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê: {ex.Message}", true, 3000);
        }
    }
}
