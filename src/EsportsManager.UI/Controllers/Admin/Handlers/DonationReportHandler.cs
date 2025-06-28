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
            int borderWidth = 80;
            int borderHeight = 20;
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TỔNG QUAN DONATION", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Hiển thị thông báo đang tải
            Console.SetCursorPosition(left, top);
            Console.WriteLine("Đang tải dữ liệu...".PadRight(width));
            // Lấy dữ liệu từ service
            var overview = await _walletService.GetDonationOverviewAsync();
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TỔNG QUAN DONATION", borderWidth, borderHeight);
            (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Hiển thị thông tin tổng quan
            string[] lines = {
                "📊 THỐNG KÊ DONATION:",
                new string('─', Math.Min(50, width)),
                $"💰 Tổng số donation: {overview.TotalDonations:N0} lượt",
                $"🎯 Số người nhận donation: {overview.TotalReceivers:N0}",
                $"👥 Số người donation: {overview.TotalDonators:N0}",
                $"📈 Tổng giá trị: {overview.TotalDonationAmount:N0} VND",
                $"⏱️ Cập nhật lần cuối: {overview.LastUpdated:dd/MM/yyyy HH:mm:ss}",
                "",
                "📊 THỐNG KÊ THEO LOẠI:",
                new string('─', Math.Min(50, width))
            };
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.WriteLine(lines[i].Length > width ? lines[i].Substring(0, width) : lines[i].PadRight(width));
            }
            int row = top + lines.Length;
            foreach (var item in overview.DonationByType)
            {
                string type = item.Key == "Tournament" ? "Giải đấu" : item.Key == "Team" ? "Đội" : item.Key;
                string line = $"- {type}: {item.Value:N0} VND";
                Console.SetCursorPosition(left, row++);
                Console.WriteLine(line.Length > width ? line.Substring(0, width) : line.PadRight(width));
            }
            Console.SetCursorPosition(left, row + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
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

            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải tổng quan donation: {ex.Message}{suggestion}", true, 5000);
        }
    }

    public async Task ShowTopDonationReceiversAsync()
    {
        try
        {
            int borderWidth = 80;
            int borderHeight = 20;
            int maxRows = 10;
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATION", borderWidth, borderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Hiển thị thông báo đang tải
            Console.SetCursorPosition(left, top);
            Console.WriteLine("Đang tải dữ liệu...".PadRight(width));
            // Lấy data từ service (mặc định là 10 người)
            var topReceivers = await _walletService.GetTopDonationReceiversAsync();
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TOP NGƯỜI NHẬN DONATION", borderWidth, borderHeight);
            (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(borderWidth, borderHeight);
            // Header
            var header = string.Format("{0,5} {1,-15} {2,-10} {3,12} {4,15} {5,20}",
                "Hạng", "Tên", "Loại", "Số donation", "Tổng tiền", "Donation gần nhất");
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(header.Length > width ? header.Substring(0, width) : header.PadRight(width));
            Console.SetCursorPosition(left, top + 1);
            Console.WriteLine(new string('─', header.Length));
            int rank = 1;
            int row = top + 2;
            foreach (var receiver in topReceivers.Take(maxRows))
            {
                string formattedName = receiver.Username.Length > 15 ? receiver.Username.Substring(0, 12) + "..." : receiver.Username;
                string formattedType = receiver.UserType == "Tournament" ? "Giải đấu" : receiver.UserType == "Team" ? "Đội" : receiver.UserType;
                var line = string.Format("{0,5} {1,-15} {2,-10} {3,12} {4,15:N0} {5,20:dd/MM/yyyy HH:mm}",
                    rank, formattedName, formattedType, receiver.DonationCount, receiver.TotalAmount, receiver.LastDonation);
                Console.SetCursorPosition(left, row++);
                Console.WriteLine(line.Length > width ? line.Substring(0, width) : line.PadRight(width));
                rank++;
            }
            Console.ResetColor();
            Console.SetCursorPosition(left, row + 1);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
            Console.SetCursorPosition(0, row + borderHeight + 2);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
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
            Console.WriteLine(new string('═', 70));
            Console.WriteLine($"{"Hạng",5} {"Username",15} {"Số donation",12} {"Tổng tiền",15} {"Donation gần nhất",20}");
            Console.WriteLine(new string('═', 70));

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
            Console.SetCursorPosition(0, 22);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
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
            int pageSize = 10; // Tăng số lượng record để hiển thị nhiều hơn
            bool viewingHistory = true;
            var filter = new DonationSearchFilterDto
            {
                PageNumber = currentPage,
                PageSize = pageSize
            };

            while (viewingHistory)
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATION", 100, 30);

                Console.WriteLine("🔄 Đang tải dữ liệu...");

                // Lấy lịch sử donation
                var donations = await _walletService.GetDonationHistoryAsync(filter);

                Console.Clear();
                ConsoleRenderingService.DrawBorder("LỊCH SỬ DONATION", 100, 30);

                // Hiển thị thông tin filter hiện tại (nếu có)
                string filterInfo = GetFilterInfoString(filter);
                if (!string.IsNullOrEmpty(filterInfo))
                {
                    Console.WriteLine($"� Bộ lọc hiện tại: {filterInfo}");
                    Console.WriteLine();
                }

                Console.WriteLine("📚 DANH SÁCH DONATION:");
                Console.WriteLine(new string('═', 95));
                Console.WriteLine($"{"ID",5} {"Người dùng",15} {"Số tiền",12} {"Đối tượng",18} {"Thời gian",18} {"Ghi chú",25}");
                Console.WriteLine(new string('─', 95));

                if (!donations.Any())
                {
                    Console.WriteLine("❌ Không có dữ liệu donation nào để hiển thị.");
                    Console.WriteLine("\nCó thể do:");
                    Console.WriteLine("- Chưa có donation nào trong hệ thống");
                    Console.WriteLine("- Bộ lọc quá nghiêm ngặt");
                    Console.WriteLine("- Database chưa có dữ liệu mẫu");
                }
                else
                {
                    foreach (var donation in donations)
                    {
                        // Hiển thị thông tin donation với format đẹp hơn
                        string formattedUser = donation.Username.Length > 15
                            ? donation.Username.Substring(0, 12) + "..."
                            : donation.Username;

                        string targetType = donation.RelatedEntityType switch
                        {
                            "Tournament" => "🏆",
                            "Team" => "👥",
                            "Player" => "🎮",
                            _ => "❓"
                        };

                        string target = $"{targetType}{donation.RelatedEntityType} #{donation.RelatedEntityId}";
                        if (target.Length > 18)
                            target = target.Substring(0, 15) + "...";

                        string note = donation.Note?.Length > 25
                            ? donation.Note.Substring(0, 22) + "..."
                            : donation.Note ?? "Không có ghi chú";

                        // Màu sắc cho số tiền (dùng emoji)
                        string amountDisplay = donation.Amount >= 500 ? $"💰{Math.Abs(donation.Amount):N0}" : $"{Math.Abs(donation.Amount):N0}";

                        Console.WriteLine($"{donation.Id,5} {formattedUser,-15} {amountDisplay,12} " +
                                         $"{target,-18} {donation.CreatedAt,18:dd/MM HH:mm} {note,-25}");
                    }
                }

                Console.WriteLine(new string('═', 95));
                
                // Thông tin phân trang và lựa chọn
                Console.WriteLine($"📄 Trang {currentPage} | Hiển thị {donations.Count} donation");
                Console.WriteLine();
                Console.WriteLine("🎮 ĐIỀU KHIỂN:");
                Console.WriteLine("- [◀ P] Trang trước    [N ▶] Trang tiếp theo    [D] Chi tiết donation");
                Console.WriteLine("- [F] Thêm bộ lọc      [C] Xóa bộ lọc          [R] Làm mới dữ liệu");
                Console.WriteLine("- [S] Thống kê         [Q] Quay lại menu chính");

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
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Đã ở trang đầu tiên!", false, 1000);
                        }
                        break;

                    case ConsoleKey.N: // Next page
                        if (donations.Count == pageSize) // Nếu đủ items, khả năng có trang tiếp theo
                        {
                            currentPage++;
                            filter.PageNumber = currentPage;
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Đã ở trang cuối cùng!", false, 1000);
                        }
                        break;

                    case ConsoleKey.D: // View details
                        if (donations.Any())
                        {
                            await PromptForDonationDetails(donations);
                        }
                        else
                        {
                            ConsoleRenderingService.ShowMessageBox("Không có donation nào để xem chi tiết!", true, 2000);
                        }
                        break;

                    case ConsoleKey.F: // Add filter
                        SetupFilters(filter);
                        currentPage = 1; // Reset về trang 1 khi thay đổi filter
                        filter.PageNumber = currentPage;
                        break;

                    case ConsoleKey.C: // Clear filters
                        filter = new DonationSearchFilterDto
                        {
                            PageNumber = 1,
                            PageSize = pageSize
                        };
                        currentPage = 1;
                        ConsoleRenderingService.ShowMessageBox("Đã xóa tất cả bộ lọc!", false, 1500);
                        break;

                    case ConsoleKey.R: // Refresh
                        ConsoleRenderingService.ShowMessageBox("Đang làm mới dữ liệu...", false, 1000);
                        break;

                    case ConsoleKey.S: // Statistics
                        ShowQuickStats(donations);
                        break;

                    case ConsoleKey.Q: // Quit
                        viewingHistory = false;
                        break;

                    default:
                        ConsoleRenderingService.ShowMessageBox("Phím không hợp lệ. Vui lòng chọn lại!", true, 1000);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            string errorSuggestion = ex.Message.Contains("doesn't exist") || ex.Message.Contains("does not exist")
                ? "\n\n💡 GIẢI PHÁP:\n1. Chạy file: database/DONATION_HISTORY_FIX.sql\n2. Kiểm tra stored procedure sp_GetDonationHistory"
                : ex.Message.Contains("connection")
                ? "\n\n💡 KIỂM TRA:\n1. MySQL server đang chạy?\n2. Thông tin kết nối đúng?"
                : "";

            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải lịch sử donation: {ex.Message}{errorSuggestion}", true, 5000);
        }
    }

    // Helper method để hiển thị thông tin filter
    private string GetFilterInfoString(DonationSearchFilterDto filter)
    {
        var filterParts = new List<string>();

        if (!string.IsNullOrEmpty(filter.Username))
            filterParts.Add($"User: {filter.Username}");
        if (filter.TeamId.HasValue)
            filterParts.Add($"Team ID: {filter.TeamId}");
        if (filter.TournamentId.HasValue)
            filterParts.Add($"Tournament ID: {filter.TournamentId}");
        if (!string.IsNullOrEmpty(filter.DonationType))
            filterParts.Add($"Type: {filter.DonationType}");
        if (filter.MinAmount.HasValue)
            filterParts.Add($"Min: {filter.MinAmount:N0}");
        if (filter.MaxAmount.HasValue)
            filterParts.Add($"Max: {filter.MaxAmount:N0}");
        if (filter.FromDate.HasValue)
            filterParts.Add($"Từ: {filter.FromDate:dd/MM/yyyy}");
        if (filter.ToDate.HasValue)
            filterParts.Add($"Đến: {filter.ToDate:dd/MM/yyyy}");

        return filterParts.Any() ? string.Join(" | ", filterParts) : "";
    }

    // Helper method để prompt cho việc xem chi tiết donation
    private async Task PromptForDonationDetails(List<TransactionDto> donations)
    {
        Console.WriteLine("\n🔍 Nhập ID donation để xem chi tiết:");
        Console.WriteLine($"   (Có thể chọn từ: {string.Join(", ", donations.Take(5).Select(d => d.Id))})");
        Console.Write("   ID: ");
        
        string input = Console.ReadLine() ?? "";

        if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int donationId))
        {
            if (donations.Any(d => d.Id == donationId))
            {
                await ShowDonationDetailsAsync(donationId);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox($"ID {donationId} không có trong trang hiện tại!", true, 2000);
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            ConsoleRenderingService.ShowMessageBox("ID không hợp lệ!", true, 1500);
        }
    }

    // Helper method để setup filters
    private void SetupFilters(DonationSearchFilterDto filter)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("THIẾT LẬP BỘ LỌC", 70, 20);

        Console.WriteLine("🔍 Thiết lập bộ lọc cho lịch sử donation:");
        Console.WriteLine("(Nhấn Enter để giữ giá trị hiện tại hoặc bỏ qua)");
        Console.WriteLine();

        // Username filter
        Console.WriteLine($"👤 Tên người dùng hiện tại: {filter.Username ?? "Tất cả"}");
        Console.Write("   Nhập tên mới: ");
        var username = Console.ReadLine() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(username))
            filter.Username = username;

        // Amount range
        Console.WriteLine($"💰 Khoảng số tiền hiện tại: {filter.MinAmount:N0} - {filter.MaxAmount:N0}");
        Console.Write("   Số tiền tối thiểu: ");
        var minAmount = Console.ReadLine() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(minAmount) && decimal.TryParse(minAmount, out decimal min))
            filter.MinAmount = min;

        Console.Write("   Số tiền tối đa: ");
        var maxAmount = Console.ReadLine() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(maxAmount) && decimal.TryParse(maxAmount, out decimal max))
            filter.MaxAmount = max;

        // Date range
        Console.WriteLine($"📅 Khoảng thời gian hiện tại: {filter.FromDate:dd/MM/yyyy} - {filter.ToDate:dd/MM/yyyy}");
        Console.Write("   Từ ngày (dd/MM/yyyy): ");
        var fromDate = Console.ReadLine() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(fromDate) && DateTime.TryParse(fromDate, out DateTime from))
            filter.FromDate = from;

        Console.Write("   Đến ngày (dd/MM/yyyy): ");
        var toDate = Console.ReadLine() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(toDate) && DateTime.TryParse(toDate, out DateTime to))
            filter.ToDate = to;

        ConsoleRenderingService.ShowMessageBox("Đã áp dụng bộ lọc mới!", false, 1500);
    }

    // Helper method để hiển thị thống kê nhanh
    private void ShowQuickStats(List<TransactionDto> donations)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("THỐNG KÊ NHANH", 60, 15);

        if (!donations.Any())
        {
            Console.WriteLine("❌ Không có dữ liệu để thống kê.");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("📊 THỐNG KÊ TRANG HIỆN TẠI:");
        Console.WriteLine(new string('─', 50));
        Console.WriteLine($"💰 Tổng số donation    : {donations.Count}");
        Console.WriteLine($"📈 Tổng số tiền        : {donations.Sum(d => Math.Abs(d.Amount)):N0} VND");
        Console.WriteLine($"📊 Số tiền trung bình  : {donations.Average(d => Math.Abs(d.Amount)):N0} VND");
        Console.WriteLine($"🏆 Donation cao nhất   : {donations.Max(d => Math.Abs(d.Amount)):N0} VND");
        Console.WriteLine($"💎 Donation thấp nhất  : {donations.Min(d => Math.Abs(d.Amount)):N0} VND");

        // Thống kê theo loại
        var byType = donations.GroupBy(d => d.RelatedEntityType ?? "Unknown");
        Console.WriteLine("\n🎯 THEO LOẠI ĐỐI TƯỢNG:");
        foreach (var group in byType)
        {
            string typeName = group.Key switch
            {
                "Tournament" => "🏆 Giải đấu",
                "Team" => "👥 Đội",
                "Player" => "🎮 Người chơi",
                _ => "❓ Khác"
            };
            Console.WriteLine($"   {typeName}: {group.Count()} donation ({group.Sum(d => Math.Abs(d.Amount)):N0} VND)");
        }

        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }

    // Helper method để hiển thị chi tiết một donation
    private async Task ShowDonationDetailsAsync(int donationId)
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CHI TIẾT DONATION #{donationId}", 80, 20);

            Console.WriteLine("Đang tải thông tin chi tiết...");

            // Tìm donation theo ID hiệu quả hơn
            var filter = new DonationSearchFilterDto
            {
                PageNumber = 1,
                PageSize = 1000 // Lấy nhiều để tìm
            };
            var allDonations = await _walletService.GetDonationHistoryAsync(filter);
            var donation = allDonations.FirstOrDefault(d => d.Id == donationId);

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"CHI TIẾT DONATION #{donationId}", 80, 25);

            if (donation == null)
            {
                Console.WriteLine($"❌ Không tìm thấy donation với ID {donationId}");
                Console.WriteLine("\nCó thể donation này không tồn tại hoặc đã bị xóa.");
                Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
                Console.ReadKey(true);
                return;
            }

            // Hiển thị thông tin chi tiết đẹp hơn
            Console.WriteLine("💰 THÔNG TIN CHI TIẾT DONATION:");
            Console.WriteLine(new string('─', 60));
            Console.WriteLine($"📋 ID giao dịch      : {donation.Id}");
            Console.WriteLine($"🔗 Mã tham chiếu     : {donation.ReferenceCode ?? "N/A"}");
            Console.WriteLine($"👤 Người donation    : {donation.Username} (ID: {donation.UserId})");
            Console.WriteLine($"💵 Số tiền           : {Math.Abs(donation.Amount):N0} VND");
            Console.WriteLine($"📅 Thời gian         : {donation.CreatedAt:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"📊 Trạng thái        : {donation.Status}");

            string targetType = donation.RelatedEntityType switch
            {
                "Tournament" => "🏆 Giải đấu",
                "Team" => "👥 Đội",
                "Player" => "🎮 Người chơi",
                _ => "❓ Không xác định"
            };

            Console.WriteLine($"🎯 Đối tượng         : {targetType} (ID: {donation.RelatedEntityId})");
            Console.WriteLine($"📝 Ghi chú           : {donation.Note ?? "Không có ghi chú"}");

            Console.WriteLine(new string('─', 60));
            
            // Thêm thông tin phân tích
            Console.WriteLine("\n📈 PHÂN TÍCH:");
            var donationTime = donation.CreatedAt;
            var timeAgo = DateTime.Now - donationTime;
            
            string timeAgoText = timeAgo.TotalDays >= 1 
                ? $"{(int)timeAgo.TotalDays} ngày trước"
                : timeAgo.TotalHours >= 1 
                    ? $"{(int)timeAgo.TotalHours} giờ trước"
                    : $"{(int)timeAgo.TotalMinutes} phút trước";
                    
            Console.WriteLine($"⏱️ Thời gian từ khi donation: {timeAgoText}");
            
            // Phân loại số tiền
            string amountCategory = donation.Amount switch
            {
                <= 50 => "💎 Donation nhỏ",
                <= 200 => "💰 Donation trung bình",
                <= 500 => "🏆 Donation lớn",
                _ => "👑 Donation khủng"
            };
            Console.WriteLine($"💸 Mức độ: {amountCategory}");

            Console.WriteLine("\n📋 LỰA CHỌN:");
            Console.WriteLine("- [R] Xem donation liên quan của user này");
            Console.WriteLine("- [H] Xem lịch sử donation của đối tượng này");
            Console.WriteLine("- [Enter] Quay lại danh sách");

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.R:
                    await ShowUserRelatedDonationsAsync(donation.UserId, donation.Username);
                    break;
                case ConsoleKey.H:
                    await ShowTargetDonationHistoryAsync(donation.RelatedEntityType, donation.RelatedEntityId);
                    break;
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị chi tiết donation: {ex.Message}", true, 3000);
        }
    }

    // Helper method để hiển thị donation liên quan của user
    private async Task ShowUserRelatedDonationsAsync(int userId, string username)
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder($"DONATION CỦA {username.ToUpper()}", 80, 20);

            var filter = new DonationSearchFilterDto
            {
                UserId = userId,
                PageNumber = 1,
                PageSize = 10
            };

            var userDonations = await _walletService.GetDonationHistoryAsync(filter);

            Console.WriteLine($"📊 LỊCH SỬ DONATION CỦA {username}:");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"{"ID",5} {"Số tiền",12} {"Đối tượng",20} {"Thời gian",20}");
            Console.WriteLine(new string('─', 70));

            foreach (var donation in userDonations)
            {
                string target = $"{donation.RelatedEntityType} #{donation.RelatedEntityId}";
                Console.WriteLine($"{donation.Id,5} {Math.Abs(donation.Amount),12:N0} {target,-20} {donation.CreatedAt,20:dd/MM/yyyy HH:mm}");
            }

            Console.WriteLine($"\nTổng {userDonations.Count} donation được hiển thị.");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị donation của user: {ex.Message}", true, 3000);
        }
    }

    // Helper method để hiển thị lịch sử donation của đối tượng
    private async Task ShowTargetDonationHistoryAsync(string? entityType, int? entityId)
    {
        try
        {
            if (string.IsNullOrEmpty(entityType) || !entityId.HasValue)
            {
                ConsoleRenderingService.ShowMessageBox("Thông tin đối tượng không hợp lệ", true, 2000);
                return;
            }

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"DONATION CHO {entityType.ToUpper()} #{entityId}", 80, 20);

            var filter = new DonationSearchFilterDto
            {
                DonationType = entityType,
                PageNumber = 1,
                PageSize = 10
            };

            if (entityType == "Team")
                filter.TeamId = entityId.Value;
            else if (entityType == "Tournament")
                filter.TournamentId = entityId.Value;

            var targetDonations = await _walletService.GetDonationHistoryAsync(filter);

            Console.WriteLine($"🎯 DONATION CHO {entityType} #{entityId}:");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"{"ID",5} {"Người donation",15} {"Số tiền",12} {"Thời gian",20}");
            Console.WriteLine(new string('─', 70));

            decimal totalAmount = 0;
            foreach (var donation in targetDonations.Where(d => d.RelatedEntityId == entityId))
            {
                Console.WriteLine($"{donation.Id,5} {donation.Username,-15} {Math.Abs(donation.Amount),12:N0} {donation.CreatedAt,20:dd/MM/yyyy HH:mm}");
                totalAmount += Math.Abs(donation.Amount);
            }

            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"📊 Tổng số donation: {targetDonations.Count(d => d.RelatedEntityId == entityId)}");
            Console.WriteLine($"💰 Tổng số tiền: {totalAmount:N0} VND");
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi hiển thị donation của đối tượng: {ex.Message}", true, 3000);
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
            Console.SetCursorPosition(0, 25);
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm donation: {ex.Message}", true, 3000);
        }
    }

    /// <summary>
    /// Hiển thị prompt "Nhấn phím bất kỳ để tiếp tục..." ở dòng cuối cùng ngoài border, an toàn cho mọi kích thước console.
    /// </summary>
    private static void ShowContinuePromptOutsideBorder()
    {
        int lastLine = Math.Max(Console.WindowTop + Console.WindowHeight - 2, 0);
        Console.SetCursorPosition(0, lastLine);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }
}
