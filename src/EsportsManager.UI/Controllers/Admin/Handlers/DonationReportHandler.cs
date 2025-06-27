using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.UI.Controllers.Admin.Interfaces;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public interface IDonationReportHandler
{
    Task ViewDonationReportsAsync();
    Task ShowDonationOverviewAsync();
    Task ShowTopDonationReceiversAsync();
    Task ShowTopDonatorsAsync();
    Task ShowDonationHistoryAsync();
    Task SearchDonationsAsync();
}

public class DonationReportHandler : IDonationReportHandler
{
    private readonly IWalletService _walletService;
    private readonly IUserService _userService;

    public DonationReportHandler(IWalletService walletService, IUserService userService)
    {
        _walletService = walletService;
        _userService = userService;
    }

    public async Task ViewDonationReportsAsync()
    {
        while (true)
        {
            var options = new[]
            {
                "Tổng quan donation",
                "Top người nhận donation nhiều nhất",
                "Top người donation nhiều nhất",
                "Lịch sử donation",
                "Tìm kiếm donation",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("BÁO CÁO DONATION", options);

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
    }

    public async Task ShowDonationOverviewAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TỔNG QUAN DONATION", 80, 20);

            // Hiển thị thông báo đang tải
            Console.WriteLine("Đang tải dữ liệu...");

            // Lấy dữ liệu từ service
            var overview = await _walletService.GetDonationOverviewAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TỔNG QUAN DONATION", 80, 20);

            // Hiển thị thông tin tổng quan
            Console.WriteLine("📊 THỐNG KÊ DONATION:");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine($"💰 Tổng số donation: {overview.TotalDonations:N0} lượt");
            Console.WriteLine($"🎯 Số người nhận donation: {overview.TotalReceivers:N0}");
            Console.WriteLine($"👥 Số người donation: {overview.TotalDonators:N0}");
            Console.WriteLine($"📈 Tổng giá trị: {overview.TotalDonationAmount:N0} VND");
            Console.WriteLine($"⏱️ Cập nhật lần cuối: {overview.LastUpdated:dd/MM/yyyy HH:mm:ss}");

            // Hiển thị thống kê theo loại
            Console.WriteLine("\n📊 THỐNG KÊ THEO LOẠI:");
            Console.WriteLine(new string('─', 50));
            foreach (var item in overview.DonationByType)
            {
                string type = item.Key == "Tournament" ? "Giải đấu" :
                              item.Key == "Team" ? "Đội" : item.Key;
                Console.WriteLine($"- {type}: {item.Value:N0} VND");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi chi tiết hơn
            string errorMessage = ex.Message;
            string suggestion = "";

            if (ex.Message.Contains("doesn't exist") || ex.Message.Contains("does not exist"))
            {
                suggestion = "\n\n💡 HƯỚNG DẪN SỬA LỖI:\n" +
                           "1. Mở MySQL Workbench\n" +
                           "2. Chạy file: database/DONATION_QUICK_FIX.sql\n" +
                           "3. Hoặc xem hướng dẫn trong: SỬA_LỖI_DONATION_NHANH.md";
            }
            else if (ex.Message.Contains("connection") || ex.Message.Contains("database"))
            {
                suggestion = "\n\n💡 KIỂM TRA:\n" +
                           "1. MySQL server đang chạy?\n" +
                           "2. Database 'EsportsManager' đã tồn tại?\n" +
                           "3. Thông tin kết nối đúng?";
            }

            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải tổng quan donation: {errorMessage}{suggestion}", true, 5000);
        }
    }

    public async Task ShowTopDonationReceiversAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATION", 80, 20);

            // Hiển thị thông báo đang tải
            Console.WriteLine("Đang tải dữ liệu...");

            // Lấy data từ service (mặc định là 10 người)
            var topReceivers = await _walletService.GetTopDonationReceiversAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATION", 80, 20);

            Console.WriteLine("🏆 TOP NGƯỜI NHẬN DONATION NHIỀU NHẤT:");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"{"Hạng",5} {"Tên",15} {"Loại",10} {"Số donation",12} {"Tổng tiền",15} {"Donation gần nhất",20}");
            Console.WriteLine(new string('─', 70));

            int rank = 1;
            foreach (var receiver in topReceivers)
            {
                string formattedName = receiver.Username.Length > 15
                    ? receiver.Username.Substring(0, 12) + "..."
                    : receiver.Username;

                string formattedType = receiver.UserType == "Tournament" ? "Giải đấu" :
                                      receiver.UserType == "Team" ? "Đội" : receiver.UserType;

                Console.WriteLine($"{rank,5} {formattedName,-15} {formattedType,-10} {receiver.DonationCount,12} " +
                                 $"{receiver.TotalAmount,15:N0} {receiver.LastDonation,20:dd/MM/yyyy HH:mm}");
                rank++;
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            string suggestion = ex.Message.Contains("doesn't exist") || ex.Message.Contains("does not exist")
                ? "\n\n💡 Chạy file: database/DONATION_FIX_COMPLETE.sql để sửa lỗi"
                : "";
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải top người nhận: {ex.Message}{suggestion}", true, 4000);
        }
    }

    public async Task ShowTopDonatorsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI DONATION", 80, 20);

            // Hiển thị thông báo đang tải
            Console.WriteLine("Đang tải dữ liệu...");

            // Lấy data từ service (mặc định là 10 người)
            var topDonators = await _walletService.GetTopDonatorsAsync();

            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI DONATION", 80, 20);

            Console.WriteLine("🎖️ TOP NGƯỜI DONATION NHIỀU NHẤT:");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"{"Hạng",5} {"Username",15} {"Số donation",12} {"Tổng tiền",15} {"Donation gần nhất",20}");
            Console.WriteLine(new string('─', 70));

            int rank = 1;
            foreach (var donator in topDonators)
            {
                string formattedName = donator.Username.Length > 15
                    ? donator.Username.Substring(0, 12) + "..."
                    : donator.Username;

                Console.WriteLine($"{rank,5} {formattedName,-15} {donator.DonationCount,12} " +
                                 $"{donator.TotalAmount,15:N0} {donator.LastDonation,20:dd/MM/yyyy HH:mm}");
                rank++;
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            string suggestion = ex.Message.Contains("doesn't exist") || ex.Message.Contains("does not exist")
                ? "\n\n💡 Chạy file: database/DONATION_FIX_COMPLETE.sql để sửa lỗi"
                : "";
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải top người donation: {ex.Message}{suggestion}", true, 4000);
        }
    }

    public async Task ShowDonationHistoryAsync()
    {
        try
        {
            int currentPage = 1;
            int pageSize = 5; // Số lượng record trên một trang
            bool viewingHistory = true;
            var filter = new DonationSearchFilterDto
            {
                PageNumber = currentPage,
                PageSize = pageSize
            };

            while (viewingHistory)
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATION", 80, 20);

                Console.WriteLine("Đang tải dữ liệu...");

                // Lấy lịch sử donation
                var donations = await _walletService.GetDonationHistoryAsync(filter);

                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATION", 90, 25);

                Console.WriteLine("📚 LỊCH SỬ DONATION:");
                Console.WriteLine(new string('─', 80));
                Console.WriteLine($"{"ID",5} {"Người dùng",15} {"Số tiền",12} {"Đối tượng",15} {"Thời gian",20} {"Ghi chú",20}");
                Console.WriteLine(new string('─', 80));

                foreach (var donation in donations)
                {
                    // Hiển thị thông tin donation
                    string formattedUser = donation.Username.Length > 15
                        ? donation.Username.Substring(0, 12) + "..."
                        : donation.Username;

                    string target = (donation.RelatedEntityType ?? "Unknown") + " #" +
                                   (donation.RelatedEntityId?.ToString() ?? "?");

                    string note = donation.Note?.Length > 20
                        ? donation.Note.Substring(0, 17) + "..."
                        : donation.Note ?? "";

                    Console.WriteLine($"{donation.Id,5} {formattedUser,-15} {Math.Abs(donation.Amount),12:N0} " +
                                     $"{target,-15} {donation.CreatedAt,20:dd/MM/yyyy HH:mm} {note,-20}");
                }

                Console.WriteLine(new string('─', 80));
                Console.WriteLine($"Trang {currentPage} | [◀ Trang trước (P)] [Trang tiếp theo (N) ▶] [Chi tiết (D)] [Quay lại (Q)]");

                // Xử lý các lựa chọn điều hướng
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.P: // Previous page
                        if (currentPage > 1)
                        {
                            currentPage--;
                            filter.PageNumber = currentPage;
                        }
                        break;

                    case ConsoleKey.N: // Next page
                        if (donations.Count == pageSize) // Nếu đủ items, khả năng có trang tiếp theo
                        {
                            currentPage++;
                            filter.PageNumber = currentPage;
                        }
                        break;

                    case ConsoleKey.D: // View details
                        Console.WriteLine("\nNhập ID donation để xem chi tiết (hoặc nhấn Enter để tiếp tục):");
                        string input = Console.ReadLine() ?? "";

                        if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int donationId))
                        {
                            await ShowDonationDetailsAsync(donationId);
                        }
                        break;

                    case ConsoleKey.Q: // Quit
                        viewingHistory = false;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải lịch sử donation: {ex.Message}", true, 3000);
        }
    }

    // Helper method để hiển thị chi tiết một donation
    private async Task ShowDonationDetailsAsync(int donationId)
    {
        try
        {
            // Tìm donation theo ID
            var filter = new DonationSearchFilterDto();
            var allDonations = await _walletService.GetDonationHistoryAsync(filter);
            var donation = allDonations.FirstOrDefault(d => d.Id == donationId);

            if (donation == null)
            {
                ConsoleRenderingService.ShowMessageBox($"Không tìm thấy donation với ID {donationId}", true, 3000);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CHI TIẾT DONATION #{donationId}", 80, 20);

            Console.WriteLine($"ID giao dịch: {donation.Id}");
            Console.WriteLine($"Mã tham chiếu: {donation.ReferenceCode}");
            Console.WriteLine($"Người donation: {donation.Username} (ID: {donation.UserId})");
            Console.WriteLine($"Số tiền: {Math.Abs(donation.Amount):N0} VND");
            Console.WriteLine($"Thời gian: {donation.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"Trạng thái: {donation.Status}");

            string target = donation.RelatedEntityType == "Tournament" ? "Giải đấu" :
                          donation.RelatedEntityType == "Team" ? "Đội" :
                          donation.RelatedEntityType ?? "Unknown";

            Console.WriteLine($"Đối tượng: {target} (ID: {donation.RelatedEntityId})");
            Console.WriteLine($"Ghi chú: {donation.Note}");

            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị chi tiết donation: {ex.Message}", true, 3000);
        }
    }

    public async Task SearchDonationsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TÌM KIẾM DONATION", 80, 20);

            // Tạo filter mới để tìm kiếm
            var filter = new DonationSearchFilterDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            Console.WriteLine("🔍 TÌM KIẾM DONATION:");
            Console.WriteLine("(Nhấn Enter để bỏ qua trường không cần tìm kiếm)");
            Console.WriteLine();

            // Thu thập thông tin tìm kiếm từ người dùng
            Console.Write("Tên người dùng: ");
            string username = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(username))
                filter.Username = username;

            Console.Write("ID đội (nếu có): ");
            string teamIdInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(teamIdInput) && int.TryParse(teamIdInput, out int teamId))
                filter.TeamId = teamId;

            Console.Write("ID giải đấu (nếu có): ");
            string tournamentIdInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(tournamentIdInput) && int.TryParse(tournamentIdInput, out int tournamentId))
                filter.TournamentId = tournamentId;

            Console.Write("Loại donation (Team/Tournament): ");
            string typeInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(typeInput))
            {
                if (typeInput.Equals("team", StringComparison.OrdinalIgnoreCase))
                    filter.DonationType = "Team";
                else if (typeInput.Equals("tournament", StringComparison.OrdinalIgnoreCase))
                    filter.DonationType = "Tournament";
            }

            Console.Write("Số tiền tối thiểu: ");
            string minAmountInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(minAmountInput) && decimal.TryParse(minAmountInput, out decimal minAmount))
                filter.MinAmount = minAmount;

            Console.Write("Số tiền tối đa: ");
            string maxAmountInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(maxAmountInput) && decimal.TryParse(maxAmountInput, out decimal maxAmount))
                filter.MaxAmount = maxAmount;

            Console.Write("Từ ngày (dd/MM/yyyy): ");
            string fromDateInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(fromDateInput) && DateTime.TryParse(fromDateInput, out DateTime fromDate))
                filter.FromDate = fromDate;

            Console.Write("Đến ngày (dd/MM/yyyy): ");
            string toDateInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(toDateInput) && DateTime.TryParse(toDateInput, out DateTime toDate))
                filter.ToDate = toDate;

            // Hiển thị thông báo đang tìm kiếm
            Console.WriteLine("\nĐang tìm kiếm...");

            // Thực hiện tìm kiếm
            var results = await _walletService.SearchDonationsAsync(filter);

            Console.Clear();
            ConsoleRenderingService.DrawBorder("KẾT QUẢ TÌM KIẾM DONATION", 90, 25);

            if (!results.Any())
            {
                Console.WriteLine("Không tìm thấy kết quả nào phù hợp với tiêu chí tìm kiếm.");
            }
            else
            {
                Console.WriteLine($"Đã tìm thấy {results.Count} kết quả:");
                Console.WriteLine(new string('─', 80));
                Console.WriteLine($"{"ID",5} {"Người dùng",15} {"Số tiền",12} {"Đối tượng",15} {"Thời gian",20} {"Ghi chú",20}");
                Console.WriteLine(new string('─', 80));

                foreach (var donation in results)
                {
                    string formattedUser = donation.Username.Length > 15
                        ? donation.Username.Substring(0, 12) + "..."
                        : donation.Username;

                    string target = (donation.RelatedEntityType ?? "Unknown") + " #" +
                                   (donation.RelatedEntityId?.ToString() ?? "?");

                    string note = donation.Note?.Length > 20
                        ? donation.Note.Substring(0, 17) + "..."
                        : donation.Note ?? "";

                    Console.WriteLine($"{donation.Id,5} {formattedUser,-15} {Math.Abs(donation.Amount),12:N0} " +
                                     $"{target,-15} {donation.CreatedAt,20:dd/MM/yyyy HH:mm} {note,-20}");
                }
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm donation: {ex.Message}", true, 3000);
        }
    }
}
