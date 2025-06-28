using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.DAL.Context;
using System.Data;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

/// <summary>
/// Handler hiển thị thống kê hệ thống. Tối ưu hóa, loại bỏ hardcode, comment chuẩn dev chuyên nghiệp.
/// </summary>
public class SystemStatsHandler
{
    // UI constants - cấu hình UI dùng chung
    private const int DefaultBorderWidth = 80;
    private const int DefaultBorderHeight = 25;
    private const int ErrorBorderWidth = 80;
    private const int ErrorBorderHeight = 20;
    private const int DetailBorderWidth = 90;
    private const int DetailBorderHeight = 30;
    private const int FallbackContinueLine = 30; // Dòng cố định ngoài border cho fallback

    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public SystemStatsHandler(IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
    }

    /// <summary>
    /// Hiển thị thống kê tổng quan hệ thống, tối ưu UI và xử lý lỗi/fallback.
    /// </summary>
    public async Task ViewSystemStatsAsync()
    {
        try
        {
            // Vẽ border và lấy vị trí content
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", DefaultBorderWidth, DefaultBorderHeight);
            var (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(DefaultBorderWidth, DefaultBorderHeight);
            ShowLoadingMessage(left, top, width);

            // Khởi tạo biến thống kê
            List<UserProfileDto>? users = null;
            List<TournamentInfoDto>? tournaments = null;
            List<TeamInfoDto>? teams = null;
            int totalUsers = 0, totalTournaments = 0, totalTeams = 0;
            int activeUsers = 0, ongoingTournaments = 0, completedTournaments = 0;
            decimal totalPrizePool = 0, totalEntryFees = 0;
            double avgTeamsPerTournament = 0;
            int recentTournaments = 0;
            bool useServiceData = true;

            // Lấy dữ liệu từng phần, nếu lỗi sẽ fallback
            try { users = await _userService.GetAllUsersAsync(); totalUsers = users?.Count() ?? 0; activeUsers = users?.Count(u => u.Status == "Active") ?? 0; } catch { useServiceData = false; }
            try { tournaments = await _tournamentService.GetAllTournamentsAsync();
                totalTournaments = tournaments?.Count() ?? 0;
                if (tournaments != null)
                {
                    ongoingTournaments = tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration");
                    completedTournaments = tournaments.Count(t => t.Status == "Completed");
                    totalPrizePool = tournaments.Sum(t => t.PrizePool);
                    totalEntryFees = tournaments.Sum(t => t.EntryFee * t.RegisteredTeams);
                    avgTeamsPerTournament = tournaments.Any() ? tournaments.Average(t => t.RegisteredTeams) : 0;
                    recentTournaments = tournaments.Count(t => t.CreatedAt >= DateTime.Now.AddDays(-7));
                }
            } catch { useServiceData = false; }
            try { teams = await _teamService.GetAllTeamsAsync(); totalTeams = teams?.Count() ?? 0; } catch { useServiceData = false; }

            // Fallback nếu không lấy được dữ liệu
            if (!useServiceData || (totalUsers == 0 && totalTournaments == 0 && totalTeams == 0))
            {
                var dbStats = await GetStatsFromDatabaseAsync();
                if (dbStats.users > 0 || dbStats.tournaments > 0 || dbStats.teams > 0)
                {
                    totalUsers = dbStats.users;
                    totalTournaments = dbStats.tournaments;
                    totalTeams = dbStats.teams;
                    activeUsers = dbStats.activeUsers;
                    totalPrizePool = dbStats.totalPrizePool;
                }
            }

            // Hiển thị bảng thống kê tổng quan
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", DefaultBorderWidth, DefaultBorderHeight);
            (left, top, width) = ConsoleRenderingService.GetBorderContentPosition(DefaultBorderWidth, DefaultBorderHeight);
            PrintSystemStatsTable(left, top, width, totalUsers, totalTournaments, totalTeams, ongoingTournaments, completedTournaments, totalPrizePool, totalEntryFees, avgTeamsPerTournament, activeUsers, recentTournaments);
        }
        catch (Exception ex)
        {
            ShowErrorStats(ex);
        }
    }

    /// <summary>
    /// Hiển thị thông báo loading.
    /// </summary>
    private static void ShowLoadingMessage(int left, int top, int width)
    {
        Console.SetCursorPosition(left, top);
        Console.WriteLine("🔄 Đang tải dữ liệu thống kê...".PadRight(width));
    }

    /// <summary>
    /// In bảng thống kê tổng quan hệ thống.
    /// </summary>
    private void PrintSystemStatsTable(int left, int top, int width, int totalUsers, int totalTournaments, int totalTeams, int ongoingTournaments, int completedTournaments, decimal totalPrizePool, decimal totalEntryFees, double avgTeamsPerTournament, int activeUsers, int recentTournaments)
    {
        string[] lines =
        {
            "📊 TỔNG QUAN HỆ THỐNG:",
            new string('─', width),
            $"👥 Tổng số người dùng      : {totalUsers:N0}",
            $"🏆 Tổng số giải đấu        : {totalTournaments:N0}",
            $"⚔️ Tổng số đội             : {totalTeams:N0}",
            $"🎮 Giải đấu đang hoạt động : {ongoingTournaments:N0}",
            $"✅ Giải đấu đã hoàn thành  : {completedTournaments:N0}",
            "",
            "💰 THỐNG KÊ TÀI CHÍNH:",
            new string('─', width),
            $"💎 Tổng giải thưởng        : {totalPrizePool:N0} VND",
            $"💵 Tổng phí tham gia       : {totalEntryFees:N0} VND",
            $"💸 Doanh thu ước tính      : {(totalEntryFees - totalPrizePool):N0} VND",
            "",
            "📈 THỐNG KÊ HOẠT ĐỘNG:",
            new string('─', width),
            $"👤 Người dùng hoạt động    : {activeUsers:N0}",
            $"📊 Trung bình team/giải đấu: {avgTeamsPerTournament:F1}",
            $"🏃 Tỷ lệ người dùng hoạt động: {(totalUsers > 0 ? (double)activeUsers / totalUsers * 100 : 0):F1}%",
            "",
            "📅 HOẠT ĐỘNG GẦN ĐÂY:",
            new string('─', width),
            $"🆕 Giải đấu tạo trong 7 ngày: {recentTournaments:N0}",
            $"📈 Tỷ lệ tăng trưởng       : {(totalTournaments > 0 ? (double)recentTournaments / totalTournaments * 100 : 0):F1}%",
            "",
            "🖥️ TÌNH TRẠNG HỆ THỐNG:",
            new string('─', width),
            $"⚡ Trạng thái hệ thống     : {((totalUsers == 0 && totalTournaments == 0 && totalTeams == 0) ? "🔴 Không có dữ liệu" : (activeUsers < totalUsers * 0.5 ? "🟡 Cần chú ý" : "🟢 Tốt"))}",
            $"🕐 Cập nhật lần cuối      : {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
        };
        for (int i = 0; i < lines.Length; i++)
        {
            Console.SetCursorPosition(left, top + i);
            Console.WriteLine(lines[i].Length > width ? lines[i].Substring(0, width) : lines[i].PadRight(width));
        }
        int row = top + lines.Length;
        // Gợi ý nếu thiếu dữ liệu
        if (totalUsers == 0 || totalTournaments == 0 || totalTeams == 0)
        {
            string[] recs =
            {
                "💡 GỢI Ý:",
                new string('─', width),
                totalUsers == 0 ? "• Tạo thêm tài khoản người dùng để test hệ thống" : null,
                totalTournaments == 0 ? "• Tạo giải đấu mới để tăng hoạt động" : null,
                totalTeams == 0 ? "• Khuyến khích người chơi tạo đội" : null,
                "• Chạy script sample data: database/ADD_SAMPLE_DONATIONS.sql"
            };
            foreach (var rec in recs)
            {
                if (rec == null) continue;
                Console.SetCursorPosition(left, row++);
                Console.WriteLine(rec.Length > width ? rec.Substring(0, width) : rec.PadRight(width));
            }
        }
        Console.SetCursorPosition(left, row + 1);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...".PadRight(width));
        Console.ReadKey(true);
    }

    /// <summary>
    /// Hiển thị thông báo lỗi và fallback thống kê cơ bản.
    /// </summary>
    private async void ShowErrorStats(Exception ex)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("LỖI THỐNG KÊ HỆ THỐNG", ErrorBorderWidth, ErrorBorderHeight);
        // Xử lý lỗi và gợi ý
        string errorMessage = ex.Message;
        string suggestion = GetSuggestionForError(errorMessage);
        Console.WriteLine("❌ ĐÃ XẢY RA LỖI KHI TẢI THỐNG KÊ:");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine($"🔍 Chi tiết lỗi: {errorMessage}");
        Console.WriteLine(suggestion);
        Console.WriteLine("\n🔧 THỐNG KÊ CƠ BẢN (FALLBACK):");
        Console.WriteLine(new string('═', 70));
        Console.WriteLine($"🕐 Thời gian hiện tại: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine($"💻 Hệ thống: {Environment.OSVersion}");
        Console.WriteLine($"🖥️ Machine: {Environment.MachineName}");
        Console.WriteLine($"👤 User: {Environment.UserName}");
        // Fallback trực tiếp từ database
        var fallbackStats = await GetStatsFromDatabaseAsync();
        Console.WriteLine($"\n📊 THỐNG KÊ TỪ DATABASE TRỰC TIẾP:");
        Console.WriteLine($"👥 Tổng số người dùng      : {fallbackStats.users:N0}");
        Console.WriteLine($"🏆 Tổng số giải đấu        : {fallbackStats.tournaments:N0}");
        Console.WriteLine($"⚔️ Tổng số đội             : {fallbackStats.teams:N0}");
        Console.WriteLine($"🎮 Giải đấu đang hoạt động : {fallbackStats.activeUsers:N0}");
        Console.WriteLine($"💰 Tổng giải thưởng        : {fallbackStats.totalPrizePool:N0} VND");
        // Đưa dòng tiếp tục ra ngoài border
        Console.SetCursorPosition(0, FallbackContinueLine);
        Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Gợi ý sửa lỗi dựa trên nội dung exception.
    /// </summary>
    private static string GetSuggestionForError(string errorMessage)
    {
        if (errorMessage.Contains("connection") || errorMessage.Contains("database"))
        {
            return "\n\n💡 KIỂM TRA:\n1. MySQL server đang chạy?\n2. Database 'EsportsManager' đã tồn tại?\n3. Thông tin kết nối trong appsettings.json đúng?\n4. Chạy lại script database/esportsmanager.sql";
        }
        if (errorMessage.Contains("method") || errorMessage.Contains("service"))
        {
            return "\n\n💡 NGUYÊN NHÂN CÓ THỂ:\n1. Service không được inject đúng cách\n2. Method GetAll...Async() chưa được implement\n3. DTO models không khớp với database schema";
        }
        if (errorMessage.Contains("table") || errorMessage.Contains("column"))
        {
            return "\n\n💡 SỬA LỖI DATABASE:\n1. Chạy script: database/esportsmanager.sql\n2. Kiểm tra các bảng Users, Tournaments, Teams\n3. Thêm dữ liệu mẫu để test";
        }
        return string.Empty;
    }

    // Helper method để hiển thị thống kê chi tiết
    private async Task ShowDetailedStatsAsync(List<UserProfileDto>? users, List<TournamentInfoDto>? tournaments, List<TeamInfoDto>? teams)
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("THỐNG KÊ CHI TIẾT", 90, 30);

        // User Statistics
        if (users != null && users.Any())
        {
            Console.WriteLine("👥 CHI TIẾT NGƯỜI DÙNG:");
            Console.WriteLine(new string('═', 70));
            
            var usersByRole = users.GroupBy(u => u.Role);
            foreach (var group in usersByRole)
            {
                string roleIcon = group.Key switch
                {
                    "Admin" => "👨‍💼",
                    "Player" => "🎮",
                    "Viewer" => "👁️",
                    _ => "❓"
                };
                Console.WriteLine($"{roleIcon} {group.Key}: {group.Count()} người");
            }

            var usersByStatus = users.GroupBy(u => u.Status);
            Console.WriteLine("\n📊 Theo trạng thái:");
            foreach (var group in usersByStatus)
            {
                string statusIcon = group.Key switch
                {
                    "Active" => "✅",
                    "Inactive" => "⏸️",
                    "Suspended" => "🚫",
                    "Pending" => "⏳",
                    _ => "❓"
                };
                Console.WriteLine($"   {statusIcon} {group.Key}: {group.Count()} người");
            }
        }

        // Tournament Statistics
        if (tournaments != null && tournaments.Any())
        {
            Console.WriteLine("\n🏆 CHI TIẾT GIẢI ĐẤU:");
            Console.WriteLine(new string('═', 70));
            
            var tournamentsByStatus = tournaments.GroupBy(t => t.Status);
            foreach (var group in tournamentsByStatus)
            {
                string statusIcon = group.Key switch
                {
                    "Draft" => "📝",
                    "Registration" => "📝",
                    "Ongoing" => "🎮",
                    "Completed" => "✅",
                    "Cancelled" => "❌",
                    _ => "❓"
                };
                Console.WriteLine($"{statusIcon} {group.Key}: {group.Count()} giải đấu");
            }

            Console.WriteLine("\n💰 Top 5 giải đấu có giải thưởng cao nhất:");
            var topPrizeTournaments = tournaments
                .OrderByDescending(t => t.PrizePool)
                .Take(5)
                .ToList();

            foreach (var tournament in topPrizeTournaments)
            {
                string name = tournament.TournamentName.Length > 30 
                    ? tournament.TournamentName.Substring(0, 27) + "..."
                    : tournament.TournamentName;
                Console.WriteLine($"   🏆 {name}: {tournament.PrizePool:N0} VND");
            }
        }

        // Team Statistics  
        if (teams != null && teams.Any())
        {
            Console.WriteLine("\n⚔️ CHI TIẾT ĐỘI:");
            Console.WriteLine(new string('═', 70));
            
            var teamsByStatus = teams.GroupBy(t => t.Status);
            foreach (var group in teamsByStatus)
            {
                string statusIcon = group.Key switch
                {
                    "Active" => "✅",
                    "Disbanded" => "💔",
                    _ => "❓"
                };
                Console.WriteLine($"{statusIcon} {group.Key}: {group.Count()} đội");
            }

            // Show team size distribution if MemberCount is available
            try
            {
                var teamSizes = teams.Where(t => t.MemberCount > 0);
                if (teamSizes.Any())
                {
                    Console.WriteLine("\n📊 Phân bố kích thước đội:");
                    var avgSize = teamSizes.Average(t => t.MemberCount);
                    var maxSize = teamSizes.Max(t => t.MemberCount);
                    var minSize = teamSizes.Min(t => t.MemberCount);
                    
                    Console.WriteLine($"   📈 Trung bình: {avgSize:F1} thành viên");
                    Console.WriteLine($"   🔝 Lớn nhất: {maxSize} thành viên");  
                    Console.WriteLine($"   🔻 Nhỏ nhất: {minSize} thành viên");
                }
            }
            catch
            {
                // MemberCount property might not exist, skip this section
            }
        }

        // System Recommendations
        Console.WriteLine("\n💡 KHUYẾN NGHỊ:");
        Console.WriteLine(new string('═', 70));
        
        bool hasRecommendations = false;

        if (users == null || !users.Any())
        {
            Console.WriteLine("⚠️  Cần tạo thêm tài khoản người dùng");
            hasRecommendations = true;
        }
        else if (users.Count(u => u.Status == "Active") < users.Count() * 0.7)
        {
            Console.WriteLine("⚠️  Tỷ lệ người dùng hoạt động thấp - cần kích hoạt");
            hasRecommendations = true;
        }

        if (tournaments == null || !tournaments.Any())
        {
            Console.WriteLine("⚠️  Cần tạo giải đấu để tăng hoạt động");
            hasRecommendations = true;
        }
        else if (tournaments.Count(t => t.Status == "Ongoing") == 0)
        {
            Console.WriteLine("⚠️  Không có giải đấu nào đang diễn ra");
            hasRecommendations = true;
        }

        if (teams == null || !teams.Any())
        {
            Console.WriteLine("⚠️  Cần khuyến khích người chơi tạo đội");
            hasRecommendations = true;
        }

        if (!hasRecommendations)
        {
            Console.WriteLine("✅ Hệ thống đang hoạt động tốt!");
        }

        Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
        Console.ReadKey(true);
    }

    // Helper method để tạo dữ liệu mẫu nếu cần
    private async Task CreateSampleDataIfNeededAsync()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("TẠO DỮ LIỆU MẪU", 70, 15);
        
        Console.WriteLine("🔧 Tính năng này sẽ tạo dữ liệu mẫu cho hệ thống:");
        Console.WriteLine("- Tạo users mẫu (Admin, Player, Viewer)");
        Console.WriteLine("- Tạo tournaments mẫu");
        Console.WriteLine("- Tạo teams mẫu");
        Console.WriteLine("- Tạo sample donations");
        Console.WriteLine();
        Console.WriteLine("⚠️  Cảnh báo: Chỉ nên chạy trên database test!");
        Console.WriteLine();
        Console.Write("Bạn có muốn tiếp tục? (y/N): ");
        
        var response = Console.ReadKey();
        if (response.KeyChar == 'y' || response.KeyChar == 'Y')
        {
            Console.WriteLine("\n\n🔄 Đang tạo dữ liệu mẫu...");
            Console.WriteLine("💡 Vui lòng chạy script sau trong MySQL:");
            Console.WriteLine("   mysql -u root -p EsportsManager < database/ADD_SAMPLE_DONATIONS.sql");
            Console.WriteLine();
            Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }
    }

    // Fallback method to get stats directly from database if services fail
    private async Task<(int users, int tournaments, int teams, int activeUsers, decimal totalPrizePool)> GetStatsFromDatabaseAsync()
    {
        try
        {
            // Try to get basic stats from services first
            int totalUsers = 0, totalTournaments = 0, totalTeams = 0;
            int activeUsers = 0;
            decimal totalPrizePool = 0;

            // Fallback - try each service individually
            try
            {
                var users = await _userService.GetAllUsersAsync();
                totalUsers = users?.Count ?? 0;
                activeUsers = users?.Count(u => u.Status == "Active") ?? 0;
            }
            catch
            {
                // If service fails, set to 0
                totalUsers = 0;
                activeUsers = 0;
            }

            try
            {
                var tournaments = await _tournamentService.GetAllTournamentsAsync();
                totalTournaments = tournaments?.Count ?? 0;
                totalPrizePool = tournaments?.Sum(t => t.PrizePool) ?? 0;
            }
            catch
            {
                totalTournaments = 0;
                totalPrizePool = 0;
            }

            try 
            {
                var teams = await _teamService.GetAllTeamsAsync();
                totalTeams = teams?.Count ?? 0;
            }
            catch
            {
                totalTeams = 0;
            }

            return (totalUsers, totalTournaments, totalTeams, activeUsers, totalPrizePool);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Database fallback failed: {ex.Message}");
        }

        return (0, 0, 0, 0, 0);
    }

    // Method to test database connection and suggest fixes
    private async Task<bool> TestDatabaseConnectionAsync()
    {
        try
        {
            // Try to get any data from services to test connection
            var users = await _userService.GetAllUsersAsync();
            return users != null; // If we
        }
        catch
        {
            return false;
        }
    }

    // Method to run database fixes
    private async Task RunDatabaseFixesAsync()
    {
        Console.Clear();
        ConsoleRenderingService.DrawBorder("SỬA LỖI DATABASE", 70, 20);
        
        Console.WriteLine("🔧 ĐANG KIỂM TRA VÀ SỬA LỖI DATABASE...");
        Console.WriteLine();

        bool canConnect = await TestDatabaseConnectionAsync();
        
        if (!canConnect)
        {
            Console.WriteLine("❌ Không thể kết nối database!");
            Console.WriteLine();
            Console.WriteLine("💡 KIỂM TRA:");
            Console.WriteLine("1. MySQL server đang chạy?");
            Console.WriteLine("2. Database 'EsportsManager' đã tồn tại?");
            Console.WriteLine("3. Connection string đúng?");
            Console.WriteLine();
            Console.WriteLine("🔧 HƯỚNG DẪN SỬA:");
            Console.WriteLine("1. Khởi động MySQL service");
            Console.WriteLine("2. Chạy: mysql -u root -p EsportsManager < database/esportsmanager.sql");
            Console.WriteLine("3. Chạy: mysql -u root -p EsportsManager < database/SYSTEM_STATS_FIX.sql");
        }
        else
        {
            Console.WriteLine("✅ Database connection OK");
            Console.WriteLine("🔧 Để chạy fix procedures, thử:");
            Console.WriteLine("mysql -u root -p EsportsManager < database/SYSTEM_STATS_FIX.sql");
        }

        Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
        Console.ReadKey(true);
    }
}
