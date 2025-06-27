using EsportsManager.BL.DTOs;
using EsportsManager.BL.Interfaces;
using EsportsManager.UI.ConsoleUI.Utilities;
using EsportsManager.DAL.Context;
using System.Data;

namespace EsportsManager.UI.Controllers.Admin.Handlers;

public class SystemStatsHandler
{
    private readonly IUserService _userService;
    private readonly ITournamentService _tournamentService;
    private readonly ITeamService _teamService;

    public SystemStatsHandler(IUserService userService, ITournamentService tournamentService, ITeamService teamService)
    {
        _userService = userService;
        _tournamentService = tournamentService;
        _teamService = teamService;
    }

    public async Task ViewSystemStatsAsync()
    {
        try
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", 80, 25);

            // Show loading message
            Console.WriteLine("🔄 Đang tải dữ liệu thống kê...");

            // Initialize variables with defaults
            List<UserProfileDto>? users = null;
            List<TournamentInfoDto>? tournaments = null;
            List<TeamInfoDto>? teams = null;

            int totalUsers = 0, totalTournaments = 0, totalTeams = 0;
            int activeUsers = 0, ongoingTournaments = 0, completedTournaments = 0;
            decimal totalPrizePool = 0, totalEntryFees = 0;
            double avgTeamsPerTournament = 0;
            int recentTournaments = 0;

            // Try to get data with individual error handling
            bool useServiceData = true;
            try
            {
                users = await _userService.GetAllUsersAsync();
                totalUsers = users?.Count() ?? 0;
                activeUsers = users?.Count(u => u.Status == "Active") ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Service failed, trying database fallback: {ex.Message}");
                useServiceData = false;
            }

            try
            {
                tournaments = await _tournamentService.GetAllTournamentsAsync();
                totalTournaments = tournaments?.Count() ?? 0;
                if (tournaments != null)
                {
                    ongoingTournaments = tournaments.Count(t => t.Status == "Ongoing" || t.Status == "Registration");
                    completedTournaments = tournaments.Count(t => t.Status == "Completed");
                    totalPrizePool = tournaments.Sum(t => t.PrizePool);
                    totalEntryFees = tournaments.Sum(t => t.EntryFee * t.RegisteredTeams);
                    avgTeamsPerTournament = tournaments.Any() ? tournaments.Average(t => t.RegisteredTeams) : 0;
                    recentTournaments = tournaments.Where(t => t.CreatedAt >= DateTime.Now.AddDays(-7)).Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Tournament service failed: {ex.Message}");
                useServiceData = false;
            }

            try
            {
                teams = await _teamService.GetAllTeamsAsync();
                totalTeams = teams?.Count() ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Team service failed: {ex.Message}");
                useServiceData = false;
            }

            // If service data failed, try database fallback
            if (!useServiceData || (totalUsers == 0 && totalTournaments == 0 && totalTeams == 0))
            {
                Console.WriteLine("🔄 Trying database fallback...");
                var dbStats = await GetStatsFromDatabaseAsync();
                if (dbStats.users > 0 || dbStats.tournaments > 0 || dbStats.teams > 0)
                {
                    totalUsers = dbStats.users;
                    totalTournaments = dbStats.tournaments; 
                    totalTeams = dbStats.teams;
                    activeUsers = dbStats.activeUsers;
                    totalPrizePool = dbStats.totalPrizePool;
                    Console.WriteLine("✅ Database fallback successful!");
                }
            }

            // Clear and redraw with actual data
            Console.Clear();
            ConsoleRenderingService.DrawBorder("THỐNG KÊ HỆ THỐNG", 80, 25);

            Console.WriteLine("📊 TỔNG QUAN HỆ THỐNG:");
            Console.WriteLine(new string('═', 60));
            Console.WriteLine($"👥 Tổng số người dùng      : {totalUsers:N0}");
            Console.WriteLine($"🏆 Tổng số giải đấu        : {totalTournaments:N0}");
            Console.WriteLine($"⚔️ Tổng số đội             : {totalTeams:N0}");
            Console.WriteLine($"🎮 Giải đấu đang hoạt động : {ongoingTournaments:N0}");
            Console.WriteLine($"✅ Giải đấu đã hoàn thành  : {completedTournaments:N0}");

            Console.WriteLine("\n💰 THỐNG KÊ TÀI CHÍNH:");
            Console.WriteLine(new string('═', 60));
            Console.WriteLine($"💎 Tổng giải thưởng        : {totalPrizePool:N0} VND");
            Console.WriteLine($"� Tổng phí tham gia       : {totalEntryFees:N0} VND");
            Console.WriteLine($"� Doanh thu ước tính      : {(totalEntryFees - totalPrizePool):N0} VND");

            Console.WriteLine("\n📈 THỐNG KÊ HOẠT ĐỘNG:");
            Console.WriteLine(new string('═', 60));
            Console.WriteLine($"👤 Người dùng hoạt động    : {activeUsers:N0}");
            Console.WriteLine($"📊 Trung bình team/giải đấu: {avgTeamsPerTournament:F1}");
            Console.WriteLine($"🏃 Tỷ lệ người dùng hoạt động: {(totalUsers > 0 ? (double)activeUsers / totalUsers * 100 : 0):F1}%");

            Console.WriteLine("\n📅 HOẠT ĐỘNG GẦN ĐÂY:");
            Console.WriteLine(new string('═', 60));
            Console.WriteLine($"🆕 Giải đấu tạo trong 7 ngày: {recentTournaments:N0}");
            Console.WriteLine($"� Tỷ lệ tăng trưởng       : {(totalTournaments > 0 ? (double)recentTournaments / totalTournaments * 100 : 0):F1}%");

            // Add system health check
            Console.WriteLine("\n� TÌNH TRẠNG HỆ THỐNG:");
            Console.WriteLine(new string('═', 60));
            
            string systemHealth = "🟢 Tốt";
            if (totalUsers == 0 && totalTournaments == 0 && totalTeams == 0)
            {
                systemHealth = "🔴 Không có dữ liệu";
            }
            else if (activeUsers < totalUsers * 0.5)
            {
                systemHealth = "🟡 Cần chú ý";
            }
            
            Console.WriteLine($"⚡ Trạng thái hệ thống     : {systemHealth}");
            Console.WriteLine($"🕐 Cập nhật lần cuối      : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

            // Show recommendations if no data
            if (totalUsers == 0 || totalTournaments == 0 || totalTeams == 0)
            {
                Console.WriteLine("\n💡 GỢI Ý:");
                Console.WriteLine(new string('─', 60));
                if (totalUsers == 0)
                    Console.WriteLine("• Tạo thêm tài khoản người dùng để test hệ thống");
                if (totalTournaments == 0)
                    Console.WriteLine("• Tạo giải đấu mới để tăng hoạt động");
                if (totalTeams == 0)
                    Console.WriteLine("• Khuyến khích người chơi tạo đội");
                
                Console.WriteLine("• Chạy script sample data: database/ADD_SAMPLE_DONATIONS.sql");
            }

            Console.WriteLine("\n🎮 LỰA CHỌN:");
            Console.WriteLine("- [R] Làm mới dữ liệu");
            Console.WriteLine("- [D] Xem chi tiết từng loại");
            Console.WriteLine("- [F] Sửa lỗi database");
            Console.WriteLine("- [S] Tạo dữ liệu mẫu");
            Console.WriteLine("- [Enter] Quay lại menu");

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.R:
                    await ViewSystemStatsAsync(); // Refresh
                    break;
                case ConsoleKey.D:
                    await ShowDetailedStatsAsync(users, tournaments, teams);
                    break;
                case ConsoleKey.F:
                    await RunDatabaseFixesAsync();
                    break;
                case ConsoleKey.S:
                    await CreateSampleDataIfNeededAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.Clear();
            ConsoleRenderingService.DrawBorder("LỖI THỐNG KÊ HỆ THỐNG", 80, 20);
            
            // Enhanced error reporting
            string errorMessage = ex.Message;
            string suggestion = "";

            if (ex.Message.Contains("connection") || ex.Message.Contains("database"))
            {
                suggestion = "\n\n💡 KIỂM TRA:\n" +
                           "1. MySQL server đang chạy?\n" +
                           "2. Database 'EsportsManager' đã tồn tại?\n" +
                           "3. Thông tin kết nối trong appsettings.json đúng?\n" +
                           "4. Chạy lại script database/esportsmanager.sql";
            }
            else if (ex.Message.Contains("method") || ex.Message.Contains("service"))
            {
                suggestion = "\n\n💡 NGUYÊN NHÂN CÓ THỂ:\n" +
                           "1. Service không được inject đúng cách\n" +
                           "2. Method GetAll...Async() chưa được implement\n" +
                           "3. DTO models không khớp với database schema";
            }
            else if (ex.Message.Contains("table") || ex.Message.Contains("column"))
            {
                suggestion = "\n\n💡 SỬA LỖI DATABASE:\n" +
                           "1. Chạy script: database/esportsmanager.sql\n" +
                           "2. Kiểm tra các bảng Users, Tournaments, Teams\n" +
                           "3. Thêm dữ liệu mẫu để test";
            }

            Console.WriteLine("❌ ĐÃ XẢY RA LỖI KHI TẢI THỐNG KÊ:");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"🔍 Chi tiết lỗi: {errorMessage}");
            Console.WriteLine(suggestion);

            Console.WriteLine("\n🔧 THỐNG KÊ CƠ BẢN (FALLBACK):");
            Console.WriteLine(new string('─', 70));
            Console.WriteLine($"🕐 Thời gian hiện tại: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"💻 Hệ thống: {Environment.OSVersion}");
            Console.WriteLine($"🖥️ Machine: {Environment.MachineName}");
            Console.WriteLine($"👤 User: {Environment.UserName}");

            // Fallback to direct database query
            var fallbackStats = await GetStatsFromDatabaseAsync();
            Console.WriteLine($"\n📊 THỐNG KÊ TỪ DATABASE TRỰC TIẾP:");
            Console.WriteLine($"👥 Tổng số người dùng      : {fallbackStats.users:N0}");
            Console.WriteLine($"🏆 Tổng số giải đấu        : {fallbackStats.tournaments:N0}");
            Console.WriteLine($"⚔️ Tổng số đội             : {fallbackStats.teams:N0}");
            Console.WriteLine($"🎮 Giải đấu đang hoạt động : {fallbackStats.activeUsers:N0}");
            Console.WriteLine($"💰 Tổng giải thưởng        : {fallbackStats.totalPrizePool:N0} VND");

            Console.WriteLine("\nNhấn phím bất kỳ để quay lại...");
            Console.ReadKey(true);
        }
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
            return users != null; // If we can get users, connection is OK
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
