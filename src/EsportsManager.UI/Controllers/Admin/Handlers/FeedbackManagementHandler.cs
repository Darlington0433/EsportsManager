using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class FeedbackManagementHandler
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly IFeedbackService _feedbackService;

    public FeedbackManagementHandler(
        IUserService userService,
        ITournamentService tournamentService,
        IFeedbackService feedbackService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _tournamentService = tournamentService ?? throw new ArgumentNullException(nameof(tournamentService));
        _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
    }

    public async Task ManageFeedbackAsync()
    {
        while (true)
        {
            var options = new[]
            {
                "Xem tất cả feedback",
                "Xem feedback theo tournament",
                "Tìm kiếm feedback",
                "Ẩn/hiện feedback",
                "Xóa feedback",
                "Thống kê feedback",
                "⬅️ Quay lại"
            };

            int selection = InteractiveMenuService.DisplayInteractiveMenu("QUẢN LÝ FEEDBACK", options);

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
    }

    public async Task ShowAllFeedbackAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("TẤT CẢ FEEDBACK", 100, 25);

            Console.WriteLine("📝 TẤT CẢ FEEDBACK:");
            Console.WriteLine("------------------");

            // Lấy dữ liệu từ service
            var feedbacks = await _feedbackService.GetAllFeedbackAsync();

            if (feedbacks == null || !feedbacks.Any())
            {
                Console.WriteLine("\n⚠️ Không có dữ liệu feedback.");
            }
            else
            {
                // Header
                Console.WriteLine($"{"ID",-5}{"User",-15}{"Tournament",-10}{"Rating",-8}{"Ngày tạo",-12}{"Trạng thái",-10}{"Nội dung",-40}");
                Console.WriteLine(new string('-', 95));

                // Data
                foreach (var feedback in feedbacks)
                {
                    string shortContent = feedback.Content.Length > 35 ? feedback.Content.Substring(0, 35) + "..." : feedback.Content;

                    Console.WriteLine(
                        $"{feedback.FeedbackId,-5}{feedback.UserName,-15}{feedback.TournamentId,-10}{feedback.Rating + "★",-8}" +
                        $"{feedback.CreatedAt:yyyy-MM-dd,-12}{feedback.Status,-10}{shortContent,-40}");
                }

                Console.WriteLine($"\nTổng số: {feedbacks.Count} feedback");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải feedback: {ex.Message}", true, 3000);
        }
    }

    public async Task ShowFeedbackByTournamentAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("FEEDBACK THEO TOURNAMENT", 100, 25);

            Console.WriteLine("🏆 FEEDBACK THEO TOURNAMENT:");
            Console.WriteLine("--------------------------");

            // Lấy danh sách tournaments từ service
            var tournaments = await _tournamentService.GetAllTournamentsAsync();

            // Nếu không có tournaments nào
            if (tournaments == null || !tournaments.Any())
            {
                Console.WriteLine("\n⚠️ Không có tournament nào trong hệ thống.");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Hiển thị danh sách tournaments
            Console.WriteLine("\nChọn tournament để xem feedback:");
            for (int i = 0; i < tournaments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tournaments[i].TournamentName} (ID: {tournaments[i].Id})");
            }

            Console.Write("\nNhập số thứ tự tournament (0 để quay lại): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > tournaments.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            if (choice == 0)
                return;

            var selectedTournament = tournaments[choice - 1];

            // Lấy feedback cho tournament đã chọn
            var feedbacks = await _feedbackService.GetFeedbackByTournamentAsync(selectedTournament.Id);

            Console.Clear();
            ConsoleRenderingService.DrawBorder($"FEEDBACK - {selectedTournament.TournamentName}", 100, 25);
            Console.WriteLine($"🏆 FEEDBACK CHO: {selectedTournament.TournamentName}");
            Console.WriteLine("------------------------------------------------------");

            if (feedbacks == null || !feedbacks.Any())
            {
                Console.WriteLine("\n⚠️ Không có feedback nào cho tournament này.");
            }
            else
            {
                // Header
                Console.WriteLine($"{"ID",-5}{"User",-15}{"Rating",-8}{"Ngày tạo",-12}{"Trạng thái",-10}{"Nội dung",-40}");
                Console.WriteLine(new string('-', 90));

                // Data
                foreach (var feedback in feedbacks)
                {
                    string shortContent = feedback.Content.Length > 35 ? feedback.Content.Substring(0, 35) + "..." : feedback.Content;

                    Console.WriteLine(
                        $"{feedback.FeedbackId,-5}{feedback.UserName,-15}{feedback.Rating + "★",-8}" +
                        $"{feedback.CreatedAt:yyyy-MM-dd,-12}{feedback.Status,-10}{shortContent,-40}");
                }

                Console.WriteLine($"\nTổng số: {feedbacks.Count} feedback cho tournament này");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải feedback theo tournament: {ex.Message}", true, 3000);
        }
    }

    public async Task SearchFeedbackAsync()
    {
        try
        {
            bool searching = true;

            while (searching)
            {
                Console.Clear();
                ConsoleRenderingService.DrawBorder("TÌM KIẾM FEEDBACK", 100, 25);

                Console.WriteLine("🔍 TÌM KIẾM FEEDBACK:");
                Console.WriteLine("-------------------");

                // Nhập thông tin tìm kiếm
                Console.Write("Từ khóa (nội dung hoặc tên người dùng): ");
                string keyword = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(keyword))
                {
                    Console.WriteLine("Tìm kiếm đã hủy!");
                    break;
                }

                Console.Write("Trạng thái (Visible/Hidden/Pending, để trống để tất cả): ");
                string status = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.Write("Từ ngày (yyyy-MM-dd, để trống để bỏ qua): ");
                string fromDateStr = Console.ReadLine()?.Trim() ?? string.Empty;
                DateTime? fromDate = !string.IsNullOrEmpty(fromDateStr) && DateTime.TryParse(fromDateStr, out var date)
                    ? date
                    : null;

                Console.Write("Đến ngày (yyyy-MM-dd, để trống để bỏ qua): ");
                string toDateStr = Console.ReadLine()?.Trim() ?? string.Empty;
                DateTime? toDate = !string.IsNullOrEmpty(toDateStr) && DateTime.TryParse(toDateStr, out var date2)
                    ? date2
                    : null;

                // Thực hiện tìm kiếm
                string? nullableStatus = string.IsNullOrEmpty(status) ? null : status;
                var results = await _feedbackService.SearchFeedbackAsync(keyword, nullableStatus, fromDate, toDate);

                Console.WriteLine("\nKẾT QUẢ TÌM KIẾM:");
                Console.WriteLine("-----------------");

                if (results == null || !results.Any())
                {
                    Console.WriteLine("Không tìm thấy kết quả nào phù hợp.");
                }
                else
                {
                    // Header
                    Console.WriteLine($"{"ID",-5}{"User",-15}{"Tournament",-10}{"Rating",-8}{"Ngày tạo",-12}{"Trạng thái",-10}{"Nội dung",-40}");
                    Console.WriteLine(new string('-', 95));

                    // Data
                    foreach (var feedback in results)
                    {
                        string shortContent = feedback.Content.Length > 35 ? feedback.Content.Substring(0, 35) + "..." : feedback.Content;

                        Console.WriteLine(
                            $"{feedback.FeedbackId,-5}{feedback.UserName,-15}{feedback.TournamentId,-10}{feedback.Rating + "★",-8}" +
                            $"{feedback.CreatedAt:yyyy-MM-dd,-12}{feedback.Status,-10}{shortContent,-40}");
                    }

                    Console.WriteLine($"\nTìm thấy {results.Count} kết quả");
                }

                Console.WriteLine("\nBạn có muốn tìm kiếm tiếp? (Y/N): ");
                var key = Console.ReadKey(true);
                searching = (key.Key == ConsoleKey.Y);
            }
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tìm kiếm feedback: {ex.Message}", true, 3000);
        }
    }

    public async Task ToggleFeedbackVisibilityAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("ẨN/HIỆN FEEDBACK", 100, 25);

            Console.WriteLine("👁️ ẨN/HIỆN FEEDBACK:");
            Console.WriteLine("------------------");

            // Lấy danh sách tất cả feedback
            var feedbacks = await _feedbackService.GetAllFeedbackAsync();

            if (feedbacks == null || !feedbacks.Any())
            {
                Console.WriteLine("\n⚠️ Không có dữ liệu feedback nào.");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Hiển thị danh sách feedback
            Console.WriteLine($"{"ID",-5}{"User",-15}{"Status",-10}{"Rating",-8}{"Nội dung",-50}");
            Console.WriteLine(new string('-', 90));

            foreach (var feedback in feedbacks)
            {
                string shortContent = feedback.Content.Length > 45 ? feedback.Content.Substring(0, 45) + "..." : feedback.Content;
                Console.WriteLine($"{feedback.FeedbackId,-5}{feedback.UserName,-15}{feedback.Status,-10}{feedback.Rating + "★",-8}{shortContent,-50}");
            }

            // Chọn feedback để thay đổi trạng thái
            Console.Write("\nNhập ID feedback để thay đổi trạng thái (0 để quay lại): ");
            if (!int.TryParse(Console.ReadLine(), out int feedbackId) || feedbackId < 0)
            {
                Console.WriteLine("ID không hợp lệ!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            if (feedbackId == 0)
                return;

            var selectedFeedback = feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (selectedFeedback == null)
            {
                Console.WriteLine("Không tìm thấy feedback với ID này!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Hiển thị thông tin chi tiết và cho phép thay đổi trạng thái
            Console.WriteLine($"\nFeedback ID: {selectedFeedback.FeedbackId}");
            Console.WriteLine($"User: {selectedFeedback.UserName}");
            Console.WriteLine($"Đánh giá: {selectedFeedback.Rating}★");
            Console.WriteLine($"Nội dung: {selectedFeedback.Content}");
            Console.WriteLine($"Trạng thái hiện tại: {selectedFeedback.Status}");

            Console.WriteLine("\nChọn trạng thái mới:");
            Console.WriteLine("1. Visible (hiện)");
            Console.WriteLine("2. Hidden (ẩn)");
            Console.WriteLine("3. Pending (chờ xét duyệt)");
            Console.Write("Lựa chọn của bạn (0 để hủy): ");

            if (!int.TryParse(Console.ReadLine(), out int statusChoice) || statusChoice < 0 || statusChoice > 3)
            {
                Console.WriteLine("Lựa chọn không hợp lệ!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            if (statusChoice == 0)
                return;

            string newStatus = statusChoice switch
            {
                1 => "Visible",
                2 => "Hidden",
                3 => "Pending",
                _ => selectedFeedback.Status
            };

            // Cập nhật trạng thái
            bool success = await _feedbackService.ToggleFeedbackVisibilityAsync(selectedFeedback.FeedbackId, newStatus);

            if (success)
            {
                ConsoleRenderingService.ShowMessageBox($"Đã thay đổi trạng thái feedback #{selectedFeedback.FeedbackId} thành {newStatus}", false, 2000);
            }
            else
            {
                ConsoleRenderingService.ShowMessageBox("Không thể thay đổi trạng thái feedback!", true, 2000);
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi thay đổi trạng thái feedback: {ex.Message}", true, 3000);
        }
    }

    public async Task DeleteFeedbackAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("XÓA FEEDBACK", 100, 25);

            Console.WriteLine("🗑️ XÓA FEEDBACK:");
            Console.WriteLine("-------------");

            // Lấy danh sách tất cả feedback
            var feedbacks = await _feedbackService.GetAllFeedbackAsync();

            if (feedbacks == null || !feedbacks.Any())
            {
                Console.WriteLine("\n⚠️ Không có dữ liệu feedback nào.");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Hiển thị danh sách feedback
            Console.WriteLine($"{"ID",-5}{"User",-15}{"Status",-10}{"Rating",-8}{"Nội dung",-50}");
            Console.WriteLine(new string('-', 90));

            foreach (var feedback in feedbacks)
            {
                string shortContent = feedback.Content.Length > 45 ? feedback.Content.Substring(0, 45) + "..." : feedback.Content;
                Console.WriteLine($"{feedback.FeedbackId,-5}{feedback.UserName,-15}{feedback.Status,-10}{feedback.Rating + "★",-8}{shortContent,-50}");
            }

            // Chọn feedback để xóa
            Console.Write("\nNhập ID feedback để xóa (0 để quay lại): ");
            if (!int.TryParse(Console.ReadLine(), out int feedbackId) || feedbackId < 0)
            {
                Console.WriteLine("ID không hợp lệ!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            if (feedbackId == 0)
                return;

            var selectedFeedback = feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
            if (selectedFeedback == null)
            {
                Console.WriteLine("Không tìm thấy feedback với ID này!");
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey(true);
                return;
            }

            // Xác nhận xóa
            Console.WriteLine($"\nFeedback ID: {selectedFeedback.FeedbackId}");
            Console.WriteLine($"User: {selectedFeedback.UserName}");
            Console.WriteLine($"Đánh giá: {selectedFeedback.Rating}★");
            Console.WriteLine($"Nội dung: {selectedFeedback.Content}");

            Console.Write("\n⚠️ Bạn có chắc chắn muốn xóa feedback này? (Y/N): ");
            var key = Console.ReadKey(false);

            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("\nĐang xóa...");
                bool success = await _feedbackService.DeleteFeedbackAsync(selectedFeedback.FeedbackId);

                if (success)
                {
                    ConsoleRenderingService.ShowMessageBox($"Đã xóa feedback #{selectedFeedback.FeedbackId}", false, 2000);
                }
                else
                {
                    ConsoleRenderingService.ShowMessageBox("Không thể xóa feedback!", true, 2000);
                }
            }
            else
            {
                Console.WriteLine("\nHủy xóa feedback.");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi xóa feedback: {ex.Message}", true, 3000);
        }
    }

    public async Task ShowFeedbackStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ FEEDBACK", 100, 30);

            Console.WriteLine("📊 THỐNG KÊ FEEDBACK:");
            Console.WriteLine("-------------------");

            // Lấy dữ liệu thống kê từ service
            var stats = await _feedbackService.GetFeedbackStatsAsync();

            // Hiển thị các chỉ số tổng quan
            Console.WriteLine($"Tổng số feedback: {stats.TotalFeedback}");
            Console.WriteLine($"Feedback đang hiển thị: {stats.VisibleFeedback} ({(double)stats.VisibleFeedback / stats.TotalFeedback:P1})");
            Console.WriteLine($"Feedback đang ẩn: {stats.HiddenFeedback} ({(double)stats.HiddenFeedback / stats.TotalFeedback:P1})");
            Console.WriteLine($"Điểm đánh giá trung bình: {stats.AverageRating:F1}★");

            // Hiển thị phân bố rating
            Console.WriteLine("\nPHÂN BỐ ĐIỂM ĐÁNH GIÁ:");
            Console.WriteLine("---------------------");

            if (stats.RatingDistribution.Any())
            {
                int maxRating = stats.RatingDistribution.Values.Max();
                int barWidth = 40;

                for (int i = 5; i >= 1; i--)
                {
                    int count = stats.RatingDistribution.ContainsKey(i) ? stats.RatingDistribution[i] : 0;

                    // Tính toán chiều dài thanh biểu đồ
                    int barLength = maxRating > 0 ? (int)Math.Round((double)count / maxRating * barWidth) : 0;
                    string bar = new string('█', barLength);

                    Console.WriteLine($"{i}★ {"",-5} {bar,-40} {count,4} ({(double)count / stats.TotalFeedback:P1})");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu phân bố điểm đánh giá");
            }

            // Hiển thị phân bố theo tháng
            Console.WriteLine("\nPHÂN BỐ THEO THÁNG:");
            Console.WriteLine("------------------");

            if (stats.FeedbackByMonth.Any())
            {
                int maxValue = stats.FeedbackByMonth.Values.Max();
                int barWidth = 40;

                foreach (var entry in stats.FeedbackByMonth.OrderBy(e => e.Key))
                {
                    int month = int.Parse(entry.Key.Split('-')[1]);
                    int year = int.Parse(entry.Key.Split('-')[0]);
                    string monthName = new DateTime(year, month, 1).ToString("MMM yyyy");
                    int count = entry.Value;

                    // Tính toán chiều dài thanh biểu đồ
                    int barLength = (int)Math.Round((double)count / maxValue * barWidth);
                    string bar = new string('█', barLength);

                    Console.WriteLine($"{monthName,-10}: {bar,-40} {count,4}");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu phân bố theo tháng");
            }

            // Hiển thị Top tournaments
            Console.WriteLine("\nTOP TOURNAMENTS THEO ĐÁNH GIÁ:");
            Console.WriteLine("----------------------------");

            if (stats.TopTournaments.Any())
            {
                Console.WriteLine($"{"Tournament",-20}{"Điểm TB",-10}{"Số feedback",-12}");
                Console.WriteLine(new string('-', 42));

                foreach (var tournament in stats.TopTournaments)
                {
                    string stars = new string('★', (int)Math.Round(tournament.AverageRating));
                    Console.WriteLine($"{tournament.TournamentName,-20}{stars} ({tournament.AverageRating:F1}){tournament.FeedbackCount,-12}");
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu top tournaments");
            }

            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            ConsoleRenderingService.ShowMessageBox($"Lỗi khi tải thống kê feedback: {ex.Message}", true, 3000);
        }
    }
}
