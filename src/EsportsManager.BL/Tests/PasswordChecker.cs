using System;
using EsportsManager.BL.Utilities;

class PasswordChecker
{
    static void Main()
    {
        // Mật khẩu người dùng
        string password = "admin123";

        // Hash BCrypt từ database
        string storedHash = "$2a$10$yGTZMMjfWyunReqDn.sZ1uMazm8Q.z7xYJYUkj50TBFKlJcX4X5F2";        // Kiểm tra với BCrypt trực tiếp
        bool isBCryptValid = false;
        try
        {
            isBCryptValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
            Console.WriteLine($"Kiểm tra BCrypt trực tiếp: {isBCryptValid}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi BCrypt: {ex.Message}");
        }

        // Kiểm tra lại với BCrypt
        bool isBCryptSecondCheck = BCrypt.Net.BCrypt.Verify(password, storedHash);
        Console.WriteLine($"Kiểm tra lại qua BCrypt: {isBCryptSecondCheck}");

        // Tạo hash mới để kiểm tra
        string newHash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine($"Hash mới tạo: {newHash}");

        // Kiểm tra hash mới
        bool isNewHashValid = BCrypt.Net.BCrypt.Verify(password, newHash);
        Console.WriteLine($"Kiểm tra hash mới: {isNewHashValid}");
    }
}
