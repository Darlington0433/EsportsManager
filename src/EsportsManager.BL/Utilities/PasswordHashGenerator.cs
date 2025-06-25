using System;
using System.Security.Cryptography;
using System.Text;

namespace EsportsManager.BL.Utilities
{
    /// <summary>
    /// Tiện ích tạo và kiểm tra hash mật khẩu
    /// </summary>
    public class PasswordHashGenerator
    {
        private const string DefaultSalt = "EsportsManager_Salt";

        /// <summary>
        /// Tạo hash cho mật khẩu sử dụng SHA256 với salt cố định
        /// </summary>
        /// <param name="password">Mật khẩu cần hash</param>
        /// <param name="salt">Salt sử dụng khi hash (mặc định: "EsportsManager_Salt")</param>
        /// <returns>Chuỗi hash dạng hex</returns>
        public static string HashPassword(string password, string salt = DefaultSalt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            // Kết hợp mật khẩu với salt
            string saltedPassword = password + salt;            // Tạo hash SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                // Chuyển đổi byte array thành chuỗi Base64 (giống cách làm trong AuthenticationService)
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Kiểm tra mật khẩu có khớp với hash không
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <param name="hashedPassword">Hash mật khẩu đã lưu trữ</param>
        /// <param name="salt">Salt sử dụng khi hash (mặc định: "EsportsManager_Salt")</param>
        /// <returns>True nếu mật khẩu khớp, False nếu không khớp</returns>
        public static bool VerifyPassword(string password, string hashedPassword, string salt = DefaultSalt)
        {
            string hashedInput = HashPassword(password, salt);
            return string.Equals(hashedInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Chương trình chính để tạo hash mật khẩu
        /// </summary>
        public static void GeneratePasswordHash(string password)
        {
            Console.WriteLine($"Mật khẩu: {password}");
            Console.WriteLine($"Hash SHA256: {HashPassword(password)}");
            Console.WriteLine();
        }

        /// <summary>
        /// Hàm chạy độc lập để sinh hash mật khẩu cho các tài khoản mẫu
        /// </summary>
        public static void GenerateSamplePasswordHashes()
        {
            Console.WriteLine("Sinh hash mật khẩu cho các tài khoản mẫu:");
            Console.WriteLine("---------------------------------------");

            GeneratePasswordHash("admin123");  // Admin
            GeneratePasswordHash("player123"); // Player
            GeneratePasswordHash("viewer123"); // Viewer
        }
    }
}
