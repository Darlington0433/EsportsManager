using System;

/// <summary>
/// Kiểm tra xác thực BCrypt cho tài khoản admin
/// </summary>
class BCryptTest
{
    static void Main()
    {
        // Kiểm tra với hash admin từ database
        string adminPassword = "admin123";
        string adminHashFromDB = "$2a$10$yGTZMMjfWyunReqDn.sZ1uMazm8Q.z7xYJYUkj50TBFKlJcX4X5F2";

        Console.WriteLine("=== KIỂM TRA BCRYPT ===");
        Console.WriteLine($"Mật khẩu: {adminPassword}");
        Console.WriteLine($"Hash từ cơ sở dữ liệu: {adminHashFromDB}");

        try
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(adminPassword, adminHashFromDB);
            Console.WriteLine($"Kết quả xác thực: {isValid}");

            if (!isValid)
            {
                Console.WriteLine("❌ HASH KHÔNG KHỚP!");

                // Tạo hash mới để so sánh
                string newHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
                Console.WriteLine($"Hash mới tạo: {newHash}");

                bool testNewHash = BCrypt.Net.BCrypt.Verify(adminPassword, newHash);
                Console.WriteLine($"Test hash mới: {testNewHash}");
            }
            else
            {
                Console.WriteLine("✅ HASH KHỚP CHÍNH XÁC!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ LỖI: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        // Test các hash khác
        Console.WriteLine("\n=== KIỂM TRA HASH PLAYER ===");
        string playerPassword = "player123";
        string playerHashFromDB = "$2a$10$r6zSu0g/uWY8oVWpMT5VTeNlCCgGjgq1xM6IO2ipe.kJiCd/WEAyO";

        try
        {
            bool isPlayerValid = BCrypt.Net.BCrypt.Verify(playerPassword, playerHashFromDB);
            Console.WriteLine($"Player hash khớp: {isPlayerValid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi player hash: {ex.Message}");
        }

        Console.WriteLine("\n=== KIỂM TRA HASH VIEWER ===");
        string viewerPassword = "viewer123";
        string viewerHashFromDB = "$2a$10$qvQmMu3WQhN9DnJ/OwFPL.K32cVUmrCXFOwnbFNhfYwZBX6EJsIUi";

        try
        {
            bool isViewerValid = BCrypt.Net.BCrypt.Verify(viewerPassword, viewerHashFromDB);
            Console.WriteLine($"Viewer hash khớp: {isViewerValid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi viewer hash: {ex.Message}");
        }
    }
}
